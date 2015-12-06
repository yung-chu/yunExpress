using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;
using System.IO;
using LighTake.Infrastructure.Common.BizLogging;
using LighTake.Infrastructure.Common.BizLogging.Enums;
using LMS.Core;
using LMS.Data.Context;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LMS.Services.OperateLogServices;
using LMS.Services.SequenceNumber;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Http;
using LighTake.Infrastructure.Http.Infrastructure;
using Newtonsoft.Json;

namespace LMS.Client.SubmitOrder
{
    class CustomerOrderService
    {

        //private static readonly object LockCreateWayBillInfo = new object();

        private ICustomerOrderInfoRepository _customerOrderInfoRepository;
        private IApplicationInfoRepository _applicationInfoRepository;
        private IWayBillInfoRepository _wayBillInfoRepository;
        private IWayBillEventLogRepository _wayBillEventLogRepository;
        private ITrackingNumberInfoRepository _trackingNumberInfoRepository;
        private ITrackingNumberDetailInfoRepository _trackingNumberDetailInfoRepository;

        //private readonly IOperateLogServices _operateLogServices;
        //private IWorkContext _workContext;
        private LMS_DbContext _lmsDbContext;
        private TrackNumberService _trackNumberService;

        //最大重试次数
        private readonly int _maxRetryTimes = Convert.ToInt32(ConfigurationManager.AppSettings["MaxRetryTimes"] ?? "3");
        //重试时间间隔
        private readonly int _retryInterval = Convert.ToInt32(ConfigurationManager.AppSettings["RetryInterval"] ?? "60000");

        public CustomerOrderService()//IOperateLogServices operateLogServices,IWorkContext workContext)
        {
        }

        private List<CustomerOrderInfo> GetListByCustomerOrderId(List<int> ids, int status)
        {
            return _customerOrderInfoRepository.GetList(p => ids.Contains(p.CustomerOrderID) && p.Status == status);
        }

        private List<CustomerOrderInfo> GetListByCustomerOrderId(List<int> ids)
        {
            return _customerOrderInfoRepository.GetList(p => ids.Contains(p.CustomerOrderID));
        }

        private List<ShippingMethodModel> GetShippingMethodsByIds(List<int> shippingMethodIds)
        {
            var url = sysConfig.LISAPIPath + "API/LIS/PostShippingMethodsByIds";
            try
            {
                var list = HttpHelper.DoRequest<List<ShippingMethodModel>>(url, EnumHttpMethod.POST,
                                                                           EnumContentType.Json, shippingMethodIds);
                //Log.Info(list.RawValue);
                return list.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + url);
                Log.Exception(ex);
            }
            return null;
        }

        private List<TrackingNumberInfo> GetTrackingNumbers(List<int> shippingMethodIds)
        {
            return _trackingNumberInfoRepository.GetList(p => shippingMethodIds.Contains(p.ShippingMethodID) && p.Status == (short)TrackingNumberInfo.StatusEnum.Enable);
        }

        /// <summary>
        /// 客户订单提交(批量)
        /// </summary>
        private void CustomerOrderSubmitBatch(List<OrderSubmitResult> listOrderSubmitResult)
        {
            _lmsDbContext = new LMS_DbContext();
            _customerOrderInfoRepository = new CustomerOrderInfoRepository(_lmsDbContext);
            _wayBillInfoRepository = new WayBillInfoRepository(_lmsDbContext);
            _trackingNumberInfoRepository = new TrackingNumberInfoRepository(_lmsDbContext);
            _wayBillEventLogRepository = new WayBillEventLogRepository(_lmsDbContext);
            _trackingNumberDetailInfoRepository = new TrackingNumberDetailInfoRepository(_lmsDbContext);
            _applicationInfoRepository = new ApplicationInfoRepository(_lmsDbContext);
            _trackNumberService = new TrackNumberService();

            //重试次数加1
            listOrderSubmitResult.ForEach(p =>
                {
                    p.RetryTimes++;
                    p.Result.Success = true;
                });

            try
            {
                var customerOrderIds = listOrderSubmitResult.Select(p => p.CustomerOrderId).ToList();

                //获取要提交的订单信息
                var listCustomerOrderInfos = GetListByCustomerOrderId(customerOrderIds);

                listCustomerOrderInfos.ForEach(p =>
                    {
                        //不是提交中的状态，视为已提交成功
                        if (p.Status != (int) CustomerOrder.StatusEnum.Submiting)
                        {
                            var orderSubmitResult =
                                listOrderSubmitResult.First(s => s.CustomerOrderId == p.CustomerOrderID);
                            orderSubmitResult.Result.Success = true;
                            orderSubmitResult.Result.Message = string.Format("{0}不是提交中的状态", p.CustomerOrderID);
                        }
                    });

                listCustomerOrderInfos.RemoveAll(p => p.Status != (int) CustomerOrder.StatusEnum.Submiting);

                if (listCustomerOrderInfos.Count == 0) return; //全部不是提交中的单，直接退出

                List<int> failureShippingMethodId = new List<int>();

                //本次提交涉及到的运输方式
                List<int> shippingMethodIds = new List<int>();
                foreach (var info in listCustomerOrderInfos)
                {
                    var shippingMethodId = info.ShippingMethodId.HasValue ? info.ShippingMethodId.Value : 0;
                    if (shippingMethodIds.Contains(shippingMethodId) || shippingMethodId == 0) continue;
                    shippingMethodIds.Add(shippingMethodId);
                }

                int getShippingMethodRetry = 3;

                var shippingMethodList = GetShippingMethodsByIds(shippingMethodIds);

                while (shippingMethodList == null && --getShippingMethodRetry > 0)
                {
                    Thread.Sleep(1000*2);
                    shippingMethodList = GetShippingMethodsByIds(shippingMethodIds);
                }

                if (shippingMethodList == null)
                {
                    throw new BusinessLogicException("获取订单运输方式信息失败，请稍后重试");
                }


                List<ApplicationInfo> applicationInfos = new List<ApplicationInfo>();
                applicationInfos =
                    _applicationInfoRepository.GetList(a => customerOrderIds.Contains(a.CustomerOrderID ?? 0));
                List<string> customerOrderNumbers = new List<string>();


                #region 遍历每一个订单

                //需要添加的运单
                List<WayBillInfo> listWayBillInfoAdd = new List<WayBillInfo>();

                //需要修改的订单
                List<CustomerOrderInfo> listCustomerOrderInfoModify = new List<CustomerOrderInfo>();

                foreach (var info in listCustomerOrderInfos)
                {
                    try
                    {
                        #region 生成运单基本信息

                        string wayBillNumber = SequenceNumberService.GetWayBillNumber(info.CustomerCode);
                        Log.Info(string.Format("订单：{0}所在线程：{1}申请到单号：{2}", info.CustomerOrderNumber,
                                               Thread.CurrentThread.Name, wayBillNumber));
                        var wayBillInfo = new WayBillInfo
                            {
                                //WayBillNumber = PrefixCode.OrderID + currentWayBillNumber++,
                                WayBillNumber = wayBillNumber,
                                CustomerOrderID = info.CustomerOrderID,
                                CustomerOrderNumber = info.CustomerOrderNumber,
                                CustomerCode = info.CustomerCode,
                                InShippingMethodID = info.ShippingMethodId,
                                InShippingMethodName = info.ShippingMethodName,
                                ShippingInfoID = info.ShippingInfoID,
                                SenderInfoID = info.SenderInfoID,
                                GoodsTypeID = info.GoodsTypeID,
                                TrackingNumber = info.TrackingNumber,
                                IsReturn = info.IsReturn,
                                IsHold = false,
                                IsBattery = info.IsBattery,
                                Status = WayBill.StatusEnum.Submitted.GetStatusValue(),
                                CountryCode = info.ShippingInfo.CountryCode.ToUpperInvariant(),
                                InsuredID = info.InsuredID,
                                Weight = info.Weight,
                                Length = info.Length,
                                Width = info.Width,
                                Height = info.Height,
                                CreatedOn = info.LastUpdatedOn,
                                CreatedBy = info.LastUpdatedBy,
                                LastUpdatedBy = info.LastUpdatedBy,
                                LastUpdatedOn = info.LastUpdatedOn,
                                EnableTariffPrepay = info.EnableTariffPrepay,
                            };

                        #endregion

                        #region 插入内部操作信息


                        //Add By zxq
                        //Time:2014-09-15
                        var wayBillEventLog = new WayBillEventLog()
                            {
                                WayBillNumber = wayBillInfo.WayBillNumber,
                                EventCode = (int) WayBillEvent.EventCodeEnum.Submit,
                                Description =
                                    WayBillEvent.GetEventCodeDescription((int) WayBillEvent.EventCodeEnum.Submit),
                                EventDate = DateTime.Now,
                                LastUpdatedOn = DateTime.Now,
                                Operator = info.LastUpdatedBy,
                            };

                        _wayBillEventLogRepository.Add(wayBillEventLog);


                        #endregion

                        #region 分配跟踪号

                        if (string.IsNullOrWhiteSpace(info.TrackingNumber))
                        {
                            var shippingMethodId = info.ShippingMethodId.HasValue ? info.ShippingMethodId.Value : 0;
                            var model = shippingMethodList.Find(p => p.ShippingMethodId == shippingMethodId);
                            if (failureShippingMethodId.Contains(shippingMethodId))
                            {
                                throw new BusinessLogicException("分配跟踪号失败");
                            }
                            if (model != null && model.IsSysTrackNumber)
                            {
                                while (true)
                                {
                                    var trackingNumberList =
                                        _trackNumberService.TrackNumberAssignStandard(shippingMethodId, 1,
                                                                                      wayBillInfo.CountryCode);

                                    if (!trackingNumberList.Any())
                                    {
                                        if (!failureShippingMethodId.Contains(shippingMethodId))
                                        {
                                            failureShippingMethodId.Add(shippingMethodId);
                                        }
                                        throw new BusinessLogicException("分配跟踪号失败");
                                    }
                                    else
                                    {
                                        var trackNumber = trackingNumberList[0];
                                        if (!listCustomerOrderInfos.Any(t => t.TrackingNumber == trackNumber))
                                        {
                                            wayBillInfo.TrackingNumber = trackNumber;
                                            info.TrackingNumber = wayBillInfo.TrackingNumber;

                                            //分配跟踪号成功 , 跳出循环
                                            break;
                                        }
                                        else
                                        {
                                            //[分配的跟踪号] 与 [上传的跟踪号] 有重复
                                            //进入下一次分配
                                        }
                                    }
                                }
                            }
                        }

                        #endregion

                        #region 修改运单状态

                        info.LastUpdatedBy = info.LastUpdatedBy;
                        info.LastUpdatedOn = DateTime.Now;
                        info.Status = CustomerOrder.StatusEnum.Submitted.GetStatusValue();
                        info.CustomerOrderStatuses.Add(new CustomerOrderStatus
                            {
                                CreatedOn = info.LastUpdatedOn,
                                CustomerOrderID = info.CustomerOrderID,
                                Status = info.Status,
                                Remark = "客户提交"
                            });

                        //更新ApplicationInfo表的WayBillNumber字段
                        foreach (var appInfo in applicationInfos)
                        {
                            if (info.CustomerOrderID == appInfo.CustomerOrderID)
                            {
                                appInfo.WayBillNumber = wayBillInfo.WayBillNumber;
                                appInfo.LastUpdatedBy = info.LastUpdatedBy;
                                appInfo.LastUpdatedOn = DateTime.Now;
                                _applicationInfoRepository.Modify(appInfo);
                            }
                        }
                        listWayBillInfoAdd.Add(wayBillInfo);
                        listCustomerOrderInfoModify.Add(info);
                        customerOrderNumbers.Add(info.CustomerOrderNumber);

                        #endregion

                    }
                    catch (Exception ex)
                    {
                        OrderSubmitResult orderSubmitResult =
                            listOrderSubmitResult.Find(p => p.CustomerOrderId == info.CustomerOrderID);
                        orderSubmitResult.Result.Success = false;
                        orderSubmitResult.Result.Message = ex.Message;
                        orderSubmitResult.ContinueRetry = ex is System.Data.DataException;
                    }
                }

                #endregion

                try
                {

                    //能够提交的订单号
                    var listCanSubmitCustomerOrderNumber =
                        listCustomerOrderInfos.Where(
                            p =>
                            listOrderSubmitResult.Find(t => t.CustomerOrderId == p.CustomerOrderID && t.Result.Success) !=
                            null).Select(p => p.CustomerOrderNumber).ToList();

                    //获取已经存在运单的订单
                    var listWayBillInfoExist =
                        _wayBillInfoRepository.GetExistCustomerOrderNumber(listCanSubmitCustomerOrderNumber);

                    //过滤掉已经存在运单的订单
                    listWayBillInfoExist.ForEach(p =>
                        {
                            OrderSubmitResult orderSubmitResult = listOrderSubmitResult.Find(t => t.CustomerOrderId == p);
                            if (orderSubmitResult != null)
                            {
                                orderSubmitResult.Result.Success = false;
                                orderSubmitResult.Result.Message = "已存在该订单对应的运单";
                            }
                        });

                    //最终需要修改的订单
                    listCustomerOrderInfoModify.ForEach(p =>
                        {
                            if (
                                listOrderSubmitResult.Find(
                                    t => t.CustomerOrderId == p.CustomerOrderID && t.Result.Success) != null)
                            {
                                _customerOrderInfoRepository.Modify(p);
                            }
                        });

                    //最终需要提交的运单
                    listWayBillInfoAdd.ForEach(p =>
                        {
                            if (
                                listOrderSubmitResult.Find(
                                    t => t.CustomerOrderId == p.CustomerOrderID && t.Result.Success) != null)
                            {
                                _wayBillInfoRepository.Add(p);

                                #region 操作日志

                                //yungchu
                                //敏感字-无
                                //BizLog bizlog = new BizLog()
                                //{
                                //    Summary = "订单批量提交",
                                //    KeywordType = KeywordType.WayBillNumber,
                                //    Keyword = p.WayBillNumber,
                                //    UserCode = _workContext.User.UserUame??"admin",
                                //    UserRealName = _workContext.User.UserUame??"admin",
                                //    UserType = UserType.LMS_User,
                                //    SystemCode = SystemType.LMS,
                                //    ModuleName = "订单批量提交"
                                //};

                                //_operateLogServices.WriteLog(bizlog, p); 

                                #endregion

                            }
                        });

                    using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.MaxValue)
                        )
                    {
                        _wayBillInfoRepository.UnitOfWork.Commit();
                        _customerOrderInfoRepository.UnitOfWork.Commit();
                        _applicationInfoRepository.UnitOfWork.Commit();
                        _wayBillEventLogRepository.UnitOfWork.Commit();
                        _trackingNumberDetailInfoRepository.UnitOfWork.Commit();

                        transaction.Complete();
                    }

                }
                catch (Exception ex)
                {
                    Log.Exception(ex);

                    listOrderSubmitResult.ForEach(p =>
                        {
                            if (p.Result.Success)
                            {
                                p.Result.Success = false;
                                p.ContinueRetry = ex is System.Data.DataException;
                                p.Result.Message = p.ContinueRetry ? "系统错误，请稍后再试" : ex.Message;
                            }
                        });
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);

                listOrderSubmitResult.ForEach(p =>
                {
                    if (p.Result.Success)
                    {
                        p.Result.Success = false;
                        p.ContinueRetry = ex is System.Data.DataException;
                        p.Result.Message = p.ContinueRetry ? "系统错误，请稍后再试" : ex.Message;
                    }
                });
            }


        }

        public void RecordOrderSubmitErrorToDb(List<OrderSubmitResult> listOrderSubmitResult)
        {
            Log.Info("开始处理错误信息...");
            _customerOrderInfoRepository = new CustomerOrderInfoRepository(new LMS_DbContext());
            listOrderSubmitResult.ForEach(p =>
                {

                    if (p.Result.Success) return;

                    Log.Error(string.Format("订单ID{0}提交失败，错误详情:{1}", p.CustomerOrderId, p.Result.Message));

                    var customerOrderInfo = _customerOrderInfoRepository.Get(p.CustomerOrderId);

                    if (customerOrderInfo.Status == (int)CustomerOrder.StatusEnum.OK) return;

                    customerOrderInfo.LastUpdatedOn = DateTime.Now;
                    customerOrderInfo.Status = CustomerOrder.StatusEnum.SubmitFail.GetStatusValue();
                    customerOrderInfo.Remark = p.Result.Message;

                    _customerOrderInfoRepository.Modify(customerOrderInfo);
                });

            _customerOrderInfoRepository.UnitOfWork.Commit();
            Log.Info("处理错误信息完成.");
        }

        public void OrderSubmit(object value)
        {
            List<int> customerOrderIds = value as List<int>;

            if (customerOrderIds == null) throw new Exception("订单id为空");

            List<OrderSubmitResult> listOrderSubmitResult =
                customerOrderIds.Select(p => new OrderSubmitResult(p)).ToList();

            List<OrderSubmitResult> listOrderSubmitResultError = new List<OrderSubmitResult>();

            int i = 0;
            while (true)
            {
                CustomerOrderSubmitBatch(listOrderSubmitResult);

                //移除提交成功的订单
                listOrderSubmitResult.RemoveAll(p => p.Result.Success);

                listOrderSubmitResult.ForEach(p =>
                    {
                        //重试次数
                        if (p.RetryTimes > _maxRetryTimes || !p.ContinueRetry)
                        {
                            //需要记录为提交失败的订单
                            listOrderSubmitResultError.Add(p);
                        }
                    });

                //失败的下次不再提交
                listOrderSubmitResult.RemoveAll(
                    p => listOrderSubmitResultError.Select(s => s.CustomerOrderId).Contains(p.CustomerOrderId));

                if (!listOrderSubmitResult.Any()) break;

                Log.Info(string.Format("需要重试的订单数：{0}", listOrderSubmitResult.Count));

                int interval = _retryInterval*(++i);

                Log.Info(string.Format("{0}毫秒后重试", interval));

                Thread.Sleep(interval);
            }

            if (!listOrderSubmitResultError.Any()) return;

            Log.Info(string.Format("最终提交失败订单数：{0}", listOrderSubmitResultError.Count));

            try
            {
                //尝试在数据库中修改订单状态
                RecordOrderSubmitErrorToDb(listOrderSubmitResultError);
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("记录订单错误信息失败：{0}", ex.Message));

                //记录到文件
                RecordSubmitResultErrorToFile(listOrderSubmitResultError);
            }

        }

        //记录提交失败到文件
        public void RecordSubmitResultErrorToFile(List<OrderSubmitResult> listOrderSubmitResultError)
        {
            string file = System.Environment.CurrentDirectory + "\\SubmitFail\\" + Guid.NewGuid().ToString() + ".txt";

            if (!Directory.Exists(Path.GetDirectoryName(file)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file));
            }

            string text = JsonConvert.SerializeObject(listOrderSubmitResultError);

            File.WriteAllText(file, text);
        }
    }

    public class OrderSubmitResult
    {
        public OrderSubmitResult(int customerOrderId)
        {
            this.CustomerOrderId = customerOrderId;
            this.Result = new Result();
        }
        public Result Result { get; private set; }
        public int CustomerOrderId { get; private set; }
        public int RetryTimes { get; set; }
        public bool ContinueRetry { get; set; }
    }

    public class Result
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
