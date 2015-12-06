using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Transactions;
using LighTake.Infrastructure.Common.BizLogging;
using LighTake.Infrastructure.Common.BizLogging.Enums;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LMS.Data.Repository;
using LMS.Services.FinancialServices;
using LMS.Services.FreightServices;
using LMS.Services.OperateLogServices;
using LMS.Services.ReturnGoodsServices;
using LMS.Services.SequenceNumber;
using LMS.Services.TrackingNumberServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.InversionOfControl;
using LighTake.Infrastructure.Common.Logging;
using System.Data.Objects.SqlClient;

namespace LMS.Services.OrderServices
{
    public class OrderService : IOrderService
    {

        private IWayBillInfoRepository _wayBillInfoRepository;
        private ICustomerOrderInfoRepository _customerOrderInfoRepository;
        private ICustomerAmountRecordRepository _customerAmountRecordRepository;
        private IWorkContext _workContext;
        private IInsuredCalculationRepository _insuredCalculationRepository;
        private ISensitiveTypeInfoRepository _sensitiveTypeInfoRepository;
        private IShippingInfoRepository _shippingInfoRepository;
        private IApplicationInfoRepository _applicationInfoRepository;
        private ITrackingNumberDetailInfoRepository _trackingNumberDetailInfoRepository;
        private ITrackingNumberInfoRepository _trackingNumberInfoRepository;
        private IGoodsTypeInfoRepository _goodsTypeInfoRepository;
        private ITrackingNumberService _trackingNumberService;
        private IFreightService _freightService;
        private IFinancialService _financialService;
        private ISenderInfoRepository _senderInfoRepository;
        private IWayBillEventLogRepository _wayBillEventLogRepository;
        private IReturnGoodsRepository _returnGoodsRepository;
        private IReceivingExpenseInfoRepository _receivingExpenseInfoRepository;
        private IReceivingExpensRepository _receivingExpensRepository;
        private IOperateLogServices _operateLogServices;
	    private ICustomerOrderStatusRepository _customerOrderStatusRepository;
        private IFuzhouPostLogRepository _fuzhouPostLogRepository;

        private IAbnormalWayBillLogRepository _abnormalWayBillLogRepository;
        private IExpressResponsRepository _expressResponsRepository;
        private IEubWayBillApplicationInfoRepository _eubWayBillApplicationInfoRepository;
        private IJobErrorLogRepository _jobErrorLogRepository;
        private IDeliveryFeeRepository _deliveryFeeRepository;
        private IWayBillChangeLogRepository _wayBillChangeLogRepository;
        private IInStorageInfoRepository _inStorageInfoRepository;
        private IOutStorageInfoRepository _outStorageInfoRepository;
        private IWeightAbnormalLogRepository _weightAbnormalLogRepository;
        private INoForecastAbnormalRepository _noForecastAbnormalRepository;
        private IWayBillInfoImportTempRepository _wayBillInfoImportTempRepository;

        //private ITrackingNumberService _trackNumberService;

        public OrderService(IWayBillInfoRepository wayBillInfoRepository,
                            IWorkContext workContext,
                            ICustomerOrderInfoRepository customerOrderInfoRepository,
                            ICustomerAmountRecordRepository customerAmountRecordRepository,
                            IInsuredCalculationRepository insuredCalculationRepository,
                            ISensitiveTypeInfoRepository sensitiveTypeInfoRepository,
                            IShippingInfoRepository shippingInfoRepository,
                            IApplicationInfoRepository applicationInfoRepository,
                            ITrackingNumberDetailInfoRepository trackingNumberDetailInfoRepository,
                            ITrackingNumberInfoRepository trackingNumberInfoRepository,
                            IGoodsTypeInfoRepository goodsTypeInfoRepository,
                            IFreightService freightService,
                            IFinancialService financialService,
                            ISenderInfoRepository senderInfoRepository,
                            IWayBillEventLogRepository wayBillEventLogRepository,
                            ITrackingNumberService trackingNumberService,
                            IReturnGoodsRepository returnGoodsRepository,
                            IReceivingExpenseInfoRepository receivingExpenseInfoRepository,
                            IReceivingExpensRepository receivingExpensRepository,
                            IOperateLogServices operateLogServices,
                            IAbnormalWayBillLogRepository abnormalWayBillLogRepository,
                            IExpressResponsRepository expressResponsRepository,
                            IEubWayBillApplicationInfoRepository eubWayBillApplicationInfoRepository,
                            IJobErrorLogRepository jobErrorLogRepository,
                            IDeliveryFeeRepository deliveryFeeRepository,
                            IWayBillChangeLogRepository wayBillChangeLogRepository,
                            IInStorageInfoRepository inStorageInfoRepository,
                            IOutStorageInfoRepository outStorageInfoRepository,
                            IWeightAbnormalLogRepository weightAbnormalLogRepository,
                            INoForecastAbnormalRepository noForecastAbnormalRepository,
                            IWayBillInfoImportTempRepository wayBillInfoImportTempRepository,
			ICustomerOrderStatusRepository customerOrderStatusRepository,
IFuzhouPostLogRepository fuzhouPostLogRepository
            //ITrackingNumberService trackNumberService
            )
        {
            _wayBillInfoRepository = wayBillInfoRepository;
            _workContext = workContext;
            _customerOrderInfoRepository = customerOrderInfoRepository;
            _customerAmountRecordRepository = customerAmountRecordRepository;
            _insuredCalculationRepository = insuredCalculationRepository;
            _sensitiveTypeInfoRepository = sensitiveTypeInfoRepository;
            _shippingInfoRepository = shippingInfoRepository;
            _applicationInfoRepository = applicationInfoRepository;
            _trackingNumberDetailInfoRepository = trackingNumberDetailInfoRepository;
            _trackingNumberInfoRepository = trackingNumberInfoRepository;
            _goodsTypeInfoRepository = goodsTypeInfoRepository;
            _freightService = freightService;
            _financialService = financialService;
            _senderInfoRepository = senderInfoRepository;
            _wayBillEventLogRepository = wayBillEventLogRepository;
            _returnGoodsRepository = returnGoodsRepository;
            _trackingNumberService = trackingNumberService;
            _operateLogServices = operateLogServices;

            _receivingExpenseInfoRepository = receivingExpenseInfoRepository;
            _receivingExpensRepository = receivingExpensRepository;
            _abnormalWayBillLogRepository = abnormalWayBillLogRepository;
            _expressResponsRepository = expressResponsRepository;
            _eubWayBillApplicationInfoRepository = eubWayBillApplicationInfoRepository;
            _jobErrorLogRepository = jobErrorLogRepository;
            _deliveryFeeRepository = deliveryFeeRepository;
            _wayBillChangeLogRepository = wayBillChangeLogRepository;
            _inStorageInfoRepository = inStorageInfoRepository;
            _outStorageInfoRepository = outStorageInfoRepository;
            _weightAbnormalLogRepository = weightAbnormalLogRepository;
            _noForecastAbnormalRepository = noForecastAbnormalRepository;
            _wayBillInfoImportTempRepository = wayBillInfoImportTempRepository;
	        _customerOrderStatusRepository = customerOrderStatusRepository;
	        //_trackingNumberService = trackNumberService;
            _fuzhouPostLogRepository = fuzhouPostLogRepository;
            //_trackingNumberService = trackNumberService;
        }

        /// <summary>
        /// 批量创建运单
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool BatchCreateWayBillInfo(List<WayBillInfo> list)
        {
            var customerId = new List<string>();
            var loglist = new List<WayBillEventLog>();
            var importList = new List<WayBillInfoImportTemp>();
            //list.ForEach(p => customerId.Add(p.CustomerOrderNumber));

            //根据订单状态取值
            //var deleteStatus = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Delete);
            //var returnStatus = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Return);
            Log.Info("开始批量插入运单");
            try
            {
                //Dictionary<string, string> error = new Dictionary<string, string>();
                //string outValue;
                //string errorShippingMethod = "";
                foreach (var item in list)
                {
                    //item.WayBillNumber = SequenceNumberService.GetSequenceNumber(PrefixCode.OrderID);
                    if (item.CustomerCode.IsNullOrWhiteSpace())
                    {
                        throw new Exception("客户编码为空");
                    }
                    if (string.IsNullOrWhiteSpace(item.WayBillNumber))//运单号为空才生成 
                    {
                        item.WayBillNumber = SequenceNumberService.GetWayBillNumber(item.CustomerCode);
                    }
                    customerId.Add(item.CustomerOrderNumber);
                    ////判断是否是需要系统生成跟踪号
                    ////if (item.InShippingMethodID.HasValue)
                    ////{
                    //if (item.InShippingMethodID.HasValue && string.IsNullOrWhiteSpace(item.TrackingNumber) && _freightService.GetShippingMethod(item.InShippingMethodID ?? 0).IsSysTrackNumber)
                    //{
                    //    var trackingNumberDetail =
                    //        _trackingNumberDetailInfoRepository.GetTrackingNumberDetailInfo(
                    //            item.InShippingMethodID.Value, item.CountryCode, detailIds);
                    //    if (trackingNumberDetail != null)
                    //    {
                    //        item.TrackingNumber = trackingNumberDetail.TrackingNumber;
                    //        detailIds.Add(trackingNumberDetail.TrackingNumberDetailID);
                    //        trackingNumberDetail.Status = (short)TrackingNumberDetailInfo.StatusEnum.Used;
                    //        _trackingNumberDetailInfoRepository.Modify(trackingNumberDetail);
                    //    }
                    //    else
                    //    {
                    //        if (error.TryGetValue(item.InShippingMethodName, out outValue))
                    //        {
                    //            if (outValue == item.CountryCode)
                    //            {
                    //                continue;
                    //            }
                    //        }
                    //        //error.Add(item.InShippingMethodName, item.CountryCode);
                    //        errorShippingMethod += "运输方式[" + item.InShippingMethodName + "]国家[" +
                    //                               item.CountryCode + "]";
                    //    }
                    //}
                    ////}

                    //item.WayBillNumber = SequenceNumberService.GetWayBillNumber(item.CustomerCode);

                    if (string.IsNullOrWhiteSpace(item.WayBillNumber)) //运单号为空才生成 
                    {
                        item.WayBillNumber = SequenceNumberService.GetWayBillNumber(item.CustomerCode);
                    }   

                    customerId.Add(item.CustomerOrderNumber);                                 

                    #region 录入内部信息

                    //Add By zxq
                    //Time:2014-09-15
                    var wayBillEventLog = new WayBillEventLog()
                        {
                            WayBillNumber = item.WayBillNumber,
                            EventCode = (int)WayBillEvent.EventCodeEnum.Submit,
                            Description = WayBillEvent.GetEventCodeDescription((int)WayBillEvent.EventCodeEnum.Submit),
                            EventDate = DateTime.Now,
                            LastUpdatedOn = DateTime.Now,
                            Operator = _workContext.User != null ? _workContext.User.UserUame : item.CustomerCode,
                        };
                    if (item.CustomerOrderID.HasValue)
                    {
                        //判断是以确定到已提交
                        _wayBillEventLogRepository.Add(wayBillEventLog);
                        _wayBillInfoRepository.Add(item);
                    }
                    else
                    {
                        //后台和WebApi上传运单
                        loglist.Add(wayBillEventLog);
                        var temp = new WayBillInfoImportTemp()
                            {
                                CustomerOrderNumber = item.CustomerOrderNumber,
                                WayBillNumber = item.WayBillNumber
                            };
                        var model = new WayBillImportModel
                            {
                                CountryCode = item.CountryCode,
                                CreatedBy = item.CreatedBy,
                                CreatedOn = item.CreatedOn,
                                CustomerCode = item.CustomerCode,
                                CustomerOrderNumber = item.CustomerOrderNumber,
                                CustomerOrderInfo = new CustomerOrderInfoImportModel()
                                    {
                                        AppLicationType = item.CustomerOrderInfo.AppLicationType,
                                        CreatedBy = item.CustomerOrderInfo.CreatedBy,
                                        CreatedOn = item.CustomerOrderInfo.CreatedOn,
                                        CustomerCode = item.CustomerOrderInfo.CustomerCode,
                                        CustomerOrderNumber = item.CustomerOrderInfo.CustomerOrderNumber,
                                        EnableTariffPrepay = item.CustomerOrderInfo.EnableTariffPrepay,
                                        GoodsTypeID = item.CustomerOrderInfo.GoodsTypeID,
                                        Height = item.CustomerOrderInfo.Height,
                                        InsureAmount = item.CustomerOrderInfo.InsureAmount,
                                        InsuredID = item.CustomerOrderInfo.InsuredID,
                                        IsInsured = item.CustomerOrderInfo.IsInsured,
                                        IsReturn = item.CustomerOrderInfo.IsReturn,
                                        Length = item.CustomerOrderInfo.Length,
                                        LastUpdatedBy = item.CustomerOrderInfo.LastUpdatedBy,
                                        LastUpdatedOn = item.CustomerOrderInfo.LastUpdatedOn,
                                        PackageNumber = item.CustomerOrderInfo.PackageNumber,
                                        Remark = item.CustomerOrderInfo.Remark,
                                        StatusRemark = item.CustomerOrderInfo.CustomerOrderStatuses.ElementAtOrDefault(0).Remark,
                                        TrackingNumber = item.CustomerOrderInfo.TrackingNumber,
                                        SensitiveTypeID = item.CustomerOrderInfo.SensitiveTypeID,
                                        ShippingMethodId = item.CustomerOrderInfo.ShippingMethodId,
                                        ShippingMethodName = item.CustomerOrderInfo.ShippingMethodName,
                                        Status = item.CustomerOrderInfo.Status,
                                        Weight = item.CustomerOrderInfo.Weight,
                                        Width = item.CustomerOrderInfo.Width
                                    },
                                EnableTariffPrepay = item.EnableTariffPrepay,
                                GoodsTypeID = item.GoodsTypeID,
                                Height = item.Height,
                                InShippingMethodID = item.InShippingMethodID,
                                InShippingMethodName = item.InShippingMethodName,
                                InsuredID = item.InsuredID,
                                IsReturn = item.IsReturn,
                                Length = item.Length,
                                LastUpdatedBy = item.LastUpdatedBy,
                                LastUpdatedOn = item.LastUpdatedOn,
                                TrackingNumber = item.TrackingNumber,
                                Status = item.Status,
                                WayBillNumber = item.WayBillNumber,
                                Weight = item.Weight,
                                Width = item.Width,
                                SenderInfo = new SenderInfoImportModel()
                                    {
                                        CountryCode = item.SenderInfo.CountryCode,
                                        SenderAddress = item.SenderInfo.SenderAddress,
                                        SenderCity = item.SenderInfo.SenderCity,
                                        SenderCompany = item.SenderInfo.SenderCompany,
                                        SenderFirstName = item.SenderInfo.SenderFirstName,
                                        SenderLastName = item.SenderInfo.SenderLastName,
                                        SenderPhone = item.SenderInfo.SenderPhone,
                                        SenderState = item.SenderInfo.SenderState,
                                        SenderZip = item.SenderInfo.SenderZip
                                    },
                                ShippingInfo = new ShippingInfoImportModel()
                                    {
                                        CountryCode = item.ShippingInfo.CountryCode,
                                        ShippingAddress = item.ShippingInfo.ShippingAddress,
                                        ShippingAddress1 = item.ShippingInfo.ShippingAddress1,
                                        ShippingAddress2 = item.ShippingInfo.ShippingAddress2,
                                        ShippingCity = item.ShippingInfo.ShippingCity,
                                        ShippingCompany = item.ShippingInfo.ShippingCompany,
                                        ShippingFirstName = item.ShippingInfo.ShippingFirstName,
                                        ShippingLastName = item.ShippingInfo.ShippingLastName,
                                        ShippingPhone = item.ShippingInfo.ShippingPhone,
                                        ShippingState = item.ShippingInfo.ShippingState,
                                        ShippingTaxId = item.ShippingInfo.ShippingTaxId,
                                        ShippingZip = item.ShippingInfo.ShippingZip
                                    }
                            };
                        foreach (var app in item.ApplicationInfos)
                        {
                            model.ApplicationInfos.Add(new ApplicationInfoImportModel()
                                {
                                    ApplicationName = app.ApplicationName,
                                    HSCode = app.HSCode,
                                    PickingName = app.PickingName,
                                    ProductUrl = app.ProductUrl,
                                    UnitPrice = app.UnitPrice,
                                    UnitWeight = app.UnitWeight,
                                    Qty = app.Qty,
                                    Remark = app.Remark,
                                });
                        }
                        temp.ImportData = SerializeUtil.ObjectToXmlSerializer(model);
                        importList.Add(temp);
                    }


                    #endregion

                    //
                }
                //if (!string.IsNullOrWhiteSpace(errorShippingMethod))
                //{
                //    throw new Exception(string.Format("保存失败，{0}的跟踪号数量不足。", errorShippingMethod));
                //}
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                throw new Exception(ex.Message);
            }

            //获取订单号重复的订单
            //var customerList = _customerOrderInfoRepository.GetList(p => customerId.Contains(p.CustomerOrderNumber) && p.Status != deleteStatus && p.Status != returnStatus);
            //foreach (var item in list)
            //{
            //    if (customerList.FindAll(o => o.CustomerOrderNumber.ToUpper() == item.CustomerOrderNumber.ToUpper()).Count > 0)
            //    {
            //        throw new BusinessLogicException("插入重复订单号:" + item.CustomerOrderNumber);
            //    }
            //}
            Log.Info("开始查询重复当订单号");

            int pagesize = 100;
            int pageindex = 1;
            var customerList=new List<string>();
            do
            {
                customerList.AddRange(
                    _wayBillInfoImportTempRepository.GetIsEixtCustomerOrderNumber(
                        customerId.Skip((pageindex - 1)*pagesize).Take(pagesize).ToList()));
                
                pageindex++;
            } while (customerId.Count > (pageindex - 1) * pagesize);
            if (customerList.Any())
            {
                throw new BusinessLogicException("插入重复订单号:" + string.Join(",", customerList));
            }
            Log.Info("结束查询重复当订单号");
            bool isSuccess = true;
            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                //_trackingNumberDetailInfoRepository.UnitOfWork.Commit();
                if (importList.Any())
                {
                    _wayBillInfoRepository.BulkInsert("WayBillInfoImportTemp", importList);
                    _wayBillInfoRepository.BulkInsert("WayBillEventLogs", loglist);
                }
                _wayBillInfoRepository.UnitOfWork.Commit();
                _wayBillEventLogRepository.UnitOfWork.Commit();
                transaction.Complete();
            }
            if (importList.Any())
            {
                pageindex = 1;
                var wayBillNumberlist = importList.Select(p => p.WayBillNumber).ToList();
                do
                {
                    if (
                        !_wayBillInfoImportTempRepository.ImportWayBillInfo(
                            wayBillNumberlist.Skip((pageindex - 1)*pagesize).Take(pagesize).ToList()))
                    {
                        isSuccess = false;
                    }
                    pageindex++;
                } while (wayBillNumberlist.Count > (pageindex - 1) * pagesize);
            }
            Log.Info("完成批量插入运单");
            return isSuccess;
        }

        /// <summary>
        /// 获取未使用的跟踪号 {被废弃 , 2014-10-23 ,by daniel}
        /// Add by zhengsong
        /// </summary>
        /// <param name="shippingMethodId"></param>
        /// <param name="countryCode"></param>
        /// <param name="detailIds"></param>
        /// <returns></returns>
        //public TrackingNumberDetailInfo GetTrackingNumberDetailInfo(int shippingMethodId, string countryCode, List<int> detailIds)
        //{
        //    return _trackingNumberDetailInfoRepository.GetTrackingNumberDetailInfo(
        //                        shippingMethodId, countryCode, detailIds);
        //}

        /// <summary>
        /// 保存使用的跟踪号 {被废弃 , 2014-10-23 ,by daniel}
        /// Add by zhengsong
        /// </summary>
        /// <param name="trackingNumberDetailInfos"></param>
        /// <returns></returns>
        //public bool UpdateTrackingNumberDetail(List<TrackingNumberDetailInfo> trackingNumberDetailInfos)
        //{
        //    try
        //    {
        //        if (trackingNumberDetailInfos.Count > 0)
        //        {
        //            trackingNumberDetailInfos.ForEach(p => _trackingNumberDetailInfoRepository.Modify(p));
        //            _trackingNumberDetailInfoRepository.UnitOfWork.Commit();
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Exception(ex);
        //        return false;
        //    }
        //}

        /// <summary>
        /// 批量创建运单(其中更新TrackingNumberDetailInfo的跟踪号状态功能,已经废弃)
        /// </summary>
        /// <param name="list"></param>
        /// <param name="trackingNumberDetailInfos"></param>
        /// <returns></returns>
        public bool BatchCreateWayBillInfoAPI(List<WayBillInfo> list)
        {
            var deleteStatus = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Delete);
            var returnStatus = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Return);
            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                foreach (var item in list)
                {
                    var customerOrderNumber = item.CustomerOrderNumber.ToUpperInvariant();
                    if (
                        _customerOrderInfoRepository.Exists(
                            p =>
                            p.CustomerOrderNumber == customerOrderNumber && p.Status != deleteStatus &&
                            p.Status != returnStatus))
                    {
                        throw new BusinessLogicException(
                            "订单号{0}已存在".FormatWith(item.CustomerOrderNumber.ToUpperInvariant()));
                    }
                    _wayBillInfoRepository.Add(item);


                    //Add By zxq
                    //Time:2014-09-18

                    #region 插入内部操作信息

                    var wayBillEventLog = new WayBillEventLog()
                        {
                            WayBillNumber = item.WayBillNumber,
                            EventCode = (int)WayBillEvent.EventCodeEnum.Submit,
                            Description = WayBillEvent.GetEventCodeDescription((int)WayBillEvent.EventCodeEnum.Submit),
                            EventDate = DateTime.Now,
                            LastUpdatedOn = DateTime.Now,
                            Operator = item.CreatedBy,
                        };

                    _wayBillEventLogRepository.Add(wayBillEventLog);

                    #endregion
                }
                //foreach (var item in trackingNumberDetailInfos)
                //{
                //    item.Status = (short)TrackingNumberDetailInfo.StatusEnum.Used;
                //    _trackingNumberDetailInfoRepository.Modify(item);
                //}

                _wayBillEventLogRepository.UnitOfWork.Commit();
                _wayBillInfoRepository.UnitOfWork.Commit();
                //_trackingNumberDetailInfoRepository.UnitOfWork.Commit();
                transaction.Complete();
                return true;
            }
        }

        //{未被使用，被注释掉 by daniel 2014-10-23}
        //public static object locked = new object();
        ///// <summary>
        ///// 创建运单，并反回跟踪号 
        ///// </summary>
        ///// <param name="wayBillInfo"></param>
        ///// <returns></returns>
        //public TrackingNumberDetailInfo CreateWayBillInfoBySysTrackNumber(WayBillInfo wayBillInfo)
        //{
        //    lock (locked)
        //    {
        //        var trackingNumberDetail =
        //            _trackingNumberDetailInfoRepository.GetTrackingNumberDetailInfo(
        //                wayBillInfo.InShippingMethodID.Value, wayBillInfo.CountryCode);
        //        if (null == trackingNumberDetail)
        //        {
        //            return null;
        //        }
        //        if (string.IsNullOrWhiteSpace(trackingNumberDetail.TrackingNumber))
        //        {
        //            return null;
        //        }
        //        // 更新trackingNumberDetail表的status改为已使用、将WayBillNumber字段的填充
        //        wayBillInfo.TrackingNumber = trackingNumberDetail.TrackingNumber;
        //        trackingNumberDetail.Status = (short)TrackingNumberDetailInfo.StatusEnum.Used;
        //        _trackingNumberDetailInfoRepository.Modify(trackingNumberDetail);
        //        _trackingNumberDetailInfoRepository.UnitOfWork.Commit();
        //        //更新ApplicationInfo表的WayBillNumber字段
        //        if (wayBillInfo.CustomerOrderID.HasValue)
        //        {
        //            foreach (
        //                var appInfo in
        //                    _applicationInfoRepository.GetList(
        //                        p => p.CustomerOrderID == wayBillInfo.CustomerOrderID))
        //            {

        //                appInfo.WayBillNumber = wayBillInfo.WayBillNumber;
        //                appInfo.LastUpdatedBy = _workContext.User.UserUame;
        //                appInfo.LastUpdatedOn = DateTime.Now;
        //                // ApplicationInfo applicationInfo = appInfo;
        //                _applicationInfoRepository.Modify(appInfo);
        //            }
        //            // _applicationInfoRepository.UnitOfWork.Commit();
        //        }
        //        _wayBillInfoRepository.Add(wayBillInfo);
        //        return trackingNumberDetail;
        //    }

        //}

        private Expression<Func<WayBillInfo, bool>> GetWayBillInfoFilter(OrderListParam param)
        {

            var startTime = param.StartTime.HasValue ? param.StartTime.Value : new DateTime(2013, 1, 1);
            var endTime = param.EndTime.HasValue ? param.EndTime.Value : new DateTime(2020, 1, 1);
            var numberList = new List<string>();
            var deleteStutas = (int)WayBill.StatusEnum.Delete;
            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberList =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();
            }
            Expression<Func<WayBillInfo, bool>> filter = p => true;
            //根据订单号查询
            //if (param.SearchWhere.HasValue && numberList.Count > 0)
            //{
            //    switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
            //    {
            //        case WayBill.SearchFilterEnum.WayBillNumber:
            //            filter = filter.And(p => numberList.Contains(p.WayBillNumber));
            //            break;
            //        case WayBill.SearchFilterEnum.TrackingNumber:
            //            filter = filter.And(p => numberList.Contains(p.TrackingNumber));
            //            break;
            //        case WayBill.SearchFilterEnum.CustomerOrderNumber:
            //            filter = filter.And(p => numberList.Contains(p.CustomerOrderInfo.CustomerOrderNumber));
            //            break;
            //    }
            // }
            if (param.SearchWhere.HasValue && numberList.Count > 0)
            {
                switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
                {
                    case WayBill.SearchFilterEnum.WayBillNumber:
                        filter = filter.And(p => numberList.Contains(p.WayBillNumber));
                        break;
                    case WayBill.SearchFilterEnum.TrackingNumber:
                        filter = filter.And(p => numberList.Contains(p.TrackingNumber) || numberList.Contains(p.TrueTrackingNumber));
                        break;
                    case WayBill.SearchFilterEnum.CustomerOrderNumber:
                        filter = filter.And(p => numberList.Contains(p.CustomerOrderNumber));
                        break;
                    case WayBill.SearchFilterEnum.InStorageNumber:
                        filter = filter.And(p => numberList.Contains(p.InStorageID));
                        break;
                    case WayBill.SearchFilterEnum.OutStorageNumber:
                        filter = filter.And(p => numberList.Contains(p.OutStorageID));
                        break;
                }
            }
            //else
            //{
            //根据时间段查询
            //switch (WayBill.ParseToDateFilter(param.DateWhere))
            //{
            //    case WayBill.DateFilterEnum.CreatedOn:
            //        filter = filter.And(p => p.CreatedOn >= startTime && p.CreatedOn <= endTime);
            //        break;
            //    case WayBill.DateFilterEnum.TakeOverOn:
            //        if (param.StartTime.HasValue || param.EndTime.HasValue)
            //        {
            //            filter =
            //                filter.And(
            //                    p => p.InStorageInfo.CreatedOn >= startTime && p.InStorageInfo.CreatedOn <= endTime);
            //        }
            //        break;
            //    case WayBill.DateFilterEnum.DeliverOn:
            //        if (param.StartTime.HasValue || param.EndTime.HasValue)
            //        {
            //            filter =
            //                filter.And(
            //                    p => p.OutStorageInfo.CreatedOn >= startTime && p.OutStorageInfo.CreatedOn <= endTime);
            //        }
            //        break;
            //}
            switch (WayBill.ParseToDateFilter(param.DateWhere))
            {
                case WayBill.DateFilterEnum.CreatedOn:
                    filter = filter.And(p => p.CreatedOn >= startTime && p.CreatedOn <= endTime);
                    break;
                case WayBill.DateFilterEnum.TakeOverOn:
                    if (param.StartTime.HasValue || param.EndTime.HasValue)
                    {
                        filter =
                            filter.And(
                                p => p.InStorageCreatedOn >= startTime && p.InStorageCreatedOn <= endTime);
                    }
                    break;
                case WayBill.DateFilterEnum.DeliverOn:
                    if (param.StartTime.HasValue || param.EndTime.HasValue)
                    {
                        filter =
                            filter.And(
                                p => p.OutStorageCreatedOn >= startTime && p.OutStorageCreatedOn <= endTime);
                    }
                    break;
            }
            filter = filter.AndIf(p => p.CustomerCode == param.CustomerCode,
                                  !string.IsNullOrWhiteSpace(param.CustomerCode))
                           .AndIf(p => p.InShippingMethodID == param.ShippingMethodId, param.ShippingMethodId.HasValue)
                           .AndIf(p => p.CountryCode == param.CountryCode, !string.IsNullOrWhiteSpace(param.CountryCode))
                //.And(p => p.Status != deleteStutas)//Add By zhengsong
                           .AndIf(p => p.Status == param.Status, param.Status.HasValue) //KevinMo 13.07.30 Added
                           .AndIf(p => p.InStorageInfo.CreatedBy == param.Operator,
                                  param.OperatorType == 0 && !string.IsNullOrWhiteSpace(param.Operator))
                           .AndIf(p => p.OutStorageInfo.CreatedBy == param.Operator,
                                  param.OperatorType == 1 && !string.IsNullOrWhiteSpace(param.Operator))
                ;

            //状态下拉框多选 yungchu
            if (!string.IsNullOrEmpty(param.GetStatus) && param.GetStatus != ";")
            {
                var getStatus = param.GetStatus.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var statusList = getStatus.Select(p => Convert.ToInt32(p)).ToList();
                filter = filter.And(p => statusList.Contains(p.Status));
            }

            if (!param.ShowTestWaybill)
            {
                string[] customerExcludes = sysConfig.TestCustomerCode.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                filter = filter.AndIf(p => !customerExcludes.Contains(p.CustomerCode), customerExcludes.Any());
            }



            // Add by zhengsong
            // Time:2014-05-27
            // 查询排除hold运单
            if (param.IsHold ?? false)
            {
                filter = filter.And(p => p.IsHold == !param.IsHold.Value);
            }
            //}

            return filter;
        }

        public IPagedList<WayBillInfoListSilm> GetWayBillInfoPagedListSilm(OrderListParam param)
        {

            Expression<Func<WayBillInfo, bool>> filter = GetWayBillInfoFilter(param);



            Func<IQueryable<WayBillInfoListSilm>, IOrderedQueryable<WayBillInfoListSilm>>
                orderBy = o => o.OrderByDescending(p => p.CreatedOn);
            return _wayBillInfoRepository.FindPagedListSilm(param.Page, param.PageSize, filter, orderBy);
        }

        public IPagedList<WayBillInfo> GetWayBillInfoPagedList(OrderListParam param)
        {
            Expression<Func<WayBillInfo, bool>> filter = GetWayBillInfoFilter(param);

            Func<IQueryable<WayBillInfo>, IOrderedQueryable<WayBillInfo>>
                orderBy = o => o.OrderByDescending(p => p.CreatedOn);
            return _wayBillInfoRepository.FindPagedListExt(param.Page, param.PageSize, filter, orderBy);
        }

        /// <summary>
        /// 中美专线导出模板列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IPagedList<WayBillInfo> GetExportWayBillInfoPagedList(OrderListParam param)
        {
            var startTime = param.StartTime.HasValue ? param.StartTime.Value : new DateTime(2013, 1, 1);
            var endTime = param.EndTime.HasValue ? param.EndTime.Value : new DateTime(2020, 1, 1);
            var numberList = new List<string>();
            var deleteStutas = (int)WayBill.StatusEnum.Delete;
            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberList =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();
            }
            Expression<Func<WayBillInfo, bool>> filter = p => true;

            if (param.SearchWhere.HasValue && numberList.Count > 0)
            {
                switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
                {
                    case WayBill.SearchFilterEnum.WayBillNumber:
                        filter = filter.And(p => numberList.Contains(p.WayBillNumber));
                        break;
                    case WayBill.SearchFilterEnum.TrackingNumber:
                        filter = filter.And(p => numberList.Contains(p.TrackingNumber) || numberList.Contains(p.TrueTrackingNumber));
                        break;
                    case WayBill.SearchFilterEnum.CustomerOrderNumber:
                        filter = filter.And(p => numberList.Contains(p.CustomerOrderNumber));
                        break;
                    case WayBill.SearchFilterEnum.InStorageNumber:
                        filter = filter.And(p => numberList.Contains(p.InStorageID));
                        break;
                }
            }
            switch (WayBill.ParseToDateFilter(param.DateWhere))
            {
                case WayBill.DateFilterEnum.CreatedOn:
                    filter = filter.And(p => p.CreatedOn >= startTime && p.CreatedOn <= endTime);
                    break;
                case WayBill.DateFilterEnum.TakeOverOn:
                    if (param.StartTime.HasValue || param.EndTime.HasValue)
                    {
                        filter =
                            filter.And(
                                p => p.InStorageCreatedOn >= startTime && p.InStorageCreatedOn <= endTime);
                    }
                    break;
                case WayBill.DateFilterEnum.DeliverOn:
                    if (param.StartTime.HasValue || param.EndTime.HasValue)
                    {
                        filter =
                            filter.And(
                                p => p.OutStorageCreatedOn >= startTime && p.OutStorageCreatedOn <= endTime);
                    }
                    break;
            }
            filter = filter.AndIf(p => p.CustomerCode == param.CustomerCode,
                                  !string.IsNullOrWhiteSpace(param.CustomerCode))
                           .AndIf(p => p.InShippingMethodID == param.ShippingMethodId, param.ShippingMethodId.HasValue)
                           .AndIf(p => p.CountryCode == param.CountryCode, !string.IsNullOrWhiteSpace(param.CountryCode))
                           .And(p => p.Status != deleteStutas)
                           .AndIf(p => p.Status == param.Status, param.Status.HasValue); //KevinMo 13.07.30 Added
            // Add by zhengsong
            // Time:2014-05-27
            // 查询排除hold运单
            if (param.IsHold ?? false)
            {
                filter = filter.And(p => p.IsHold == !param.IsHold.Value);
            }
            //}
            Func<IQueryable<WayBillInfo>, IOrderedQueryable<WayBillInfo>>
                orderBy = o => o.OrderByDescending(p => p.CreatedOn);
            var aa = _wayBillInfoRepository.FindPagedList(param.Page, param.PageSize, filter, orderBy);

            return aa;
        }

        /// <summary>
        /// 中美专线导出模板导出
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerable<WayBillInfo> GetExportWayBillInfo(OrderListParam param)
        {
            var startTime = param.StartTime.HasValue ? param.StartTime.Value : new DateTime(2013, 1, 1);
            var endTime = param.EndTime.HasValue ? param.EndTime.Value : new DateTime(2020, 1, 1);
            var numberList = new List<string>();
            var deleteStutas = (int)WayBill.StatusEnum.Delete;
            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberList =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();
            }
            Expression<Func<WayBillInfo, bool>> filter = p => true;
            //根据订单号查询
            if (param.SearchWhere.HasValue && numberList.Count > 0)
            {
                switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
                {
                    case WayBill.SearchFilterEnum.WayBillNumber:
                        filter = filter.And(p => numberList.Contains(p.WayBillNumber));
                        break;
                    case WayBill.SearchFilterEnum.TrackingNumber:
                        filter = filter.And(p => numberList.Contains(p.TrackingNumber) || numberList.Contains(p.TrueTrackingNumber));
                        break;
                    case WayBill.SearchFilterEnum.CustomerOrderNumber:
                        filter = filter.And(p => numberList.Contains(p.CustomerOrderNumber));
                        break;
                    case WayBill.SearchFilterEnum.InStorageNumber:
                        filter = filter.And(p => numberList.Contains(p.InStorageID));
                        break;
                }
            }
            //根据时间段查询
            switch (WayBill.ParseToDateFilter(param.DateWhere))
            {
                case WayBill.DateFilterEnum.CreatedOn:
                    filter = filter.And(p => p.CreatedOn >= startTime && p.CreatedOn <= endTime);
                    break;
                case WayBill.DateFilterEnum.TakeOverOn:
                    if (param.StartTime.HasValue || param.EndTime.HasValue)
                    {
                        filter =
                            filter.And(
                                p => p.InStorageCreatedOn >= startTime && p.InStorageCreatedOn <= endTime);
                    }
                    break;
                case WayBill.DateFilterEnum.DeliverOn:
                    if (param.StartTime.HasValue || param.EndTime.HasValue)
                    {
                        filter =
                            filter.And(
                                p => p.OutStorageCreatedOn >= startTime && p.OutStorageCreatedOn <= endTime);
                    }
                    break;
            }
            bool hold = false;
            filter = filter.AndIf(p => p.CustomerCode == param.CustomerCode,
                                  !string.IsNullOrWhiteSpace(param.CustomerCode))
                           .AndIf(p => p.InShippingMethodID == param.ShippingMethodId, param.ShippingMethodId.HasValue)
                           .AndIf(p => p.CountryCode == param.CountryCode, !string.IsNullOrWhiteSpace(param.CountryCode))
                           .And(p => p.Status != deleteStutas)
                           .AndIf(p => p.Status == param.Status, param.Status.HasValue);
            // Add by zhengsong
            // Time:2014-05-27
            // 查询排除hold运单
            if (param.IsHold ?? false)
            {
                filter = filter.And(p => p.IsHold == !param.IsHold.Value);
            }
            Func<IQueryable<WayBillInfo>, IOrderedQueryable<WayBillInfo>>
                orderBy = o => o.OrderByDescending(p => p.CreatedOn);
            return _wayBillInfoRepository.GetFiltered(filter, orderBy);
        }

        /// <summary>
        /// 条件查询所有运单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerable<WayBillInfo> GetWayBillInfo(OrderListParam param)
        {
            var startTime = param.StartTime.HasValue ? param.StartTime.Value : new DateTime(2013, 1, 1);
            var endTime = param.EndTime.HasValue ? param.EndTime.Value : new DateTime(2020, 1, 1);
            var numberList = new List<string>();
            var deleteStutas = (int)WayBill.StatusEnum.Delete;
            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberList =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();
            }
            Expression<Func<WayBillInfo, bool>> filter = p => true;
            //根据订单号查询
            if (param.SearchWhere.HasValue && numberList.Count > 0)
            {
                switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
                {
                    case WayBill.SearchFilterEnum.WayBillNumber:
                        filter = filter.And(p => numberList.Contains(p.WayBillNumber));
                        break;
                    case WayBill.SearchFilterEnum.TrackingNumber:
                        filter = filter.And(p => numberList.Contains(p.TrackingNumber) || numberList.Contains(p.TrueTrackingNumber));
                        break;
                    case WayBill.SearchFilterEnum.CustomerOrderNumber:
                        filter = filter.And(p => numberList.Contains(p.CustomerOrderNumber));
                        break;
                    case WayBill.SearchFilterEnum.InStorageNumber:
                        filter = filter.And(p => numberList.Contains(p.InStorageID));
                        break;
                }
            }
            //根据时间段查询
            switch (WayBill.ParseToDateFilter(param.DateWhere))
            {
                case WayBill.DateFilterEnum.CreatedOn:
                    filter = filter.And(p => p.CreatedOn >= startTime && p.CreatedOn <= endTime);
                    break;
                case WayBill.DateFilterEnum.TakeOverOn:
                    if (param.StartTime.HasValue || param.EndTime.HasValue)
                    {
                        filter =
                            filter.And(
                                p => p.InStorageCreatedOn >= startTime && p.InStorageCreatedOn <= endTime);
                    }
                    break;
                case WayBill.DateFilterEnum.DeliverOn:
                    if (param.StartTime.HasValue || param.EndTime.HasValue)
                    {
                        filter =
                            filter.And(
                                p => p.OutStorageCreatedOn >= startTime && p.OutStorageCreatedOn <= endTime);
                    }
                    break;
            }
            bool hold = false;
            filter = filter.AndIf(p => p.CustomerCode == param.CustomerCode,
                                  !string.IsNullOrWhiteSpace(param.CustomerCode))
                           .AndIf(p => p.InShippingMethodID == param.ShippingMethodId, param.ShippingMethodId.HasValue)
                           .AndIf(p => p.CountryCode == param.CountryCode, !string.IsNullOrWhiteSpace(param.CountryCode))
                           .And(p => p.Status != deleteStutas)
                           .AndIf(p => p.Status == param.Status, param.Status.HasValue);

            //状态下拉框多选 zxq
            if (!string.IsNullOrEmpty(param.GetStatus) && param.GetStatus != ";")
            {
                var getStatus = param.GetStatus.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var statusList = getStatus.Select(p => Convert.ToInt32(p)).ToList();
                filter = filter.And(p => statusList.Contains(p.Status));
            }

            // Add by zhengsong
            // Time:2014-05-27
            // 查询排除hold运单
            if (param.IsHold ?? false)
            {
                filter = filter.And(p => p.IsHold == !param.IsHold.Value);
            }
            Func<IQueryable<WayBillInfo>, IOrderedQueryable<WayBillInfo>>
                orderBy = o => o.OrderByDescending(p => p.CreatedOn);
            return _wayBillInfoRepository.GetFiltered(filter, orderBy);
        }

        public IEnumerable<WayBillInfo> GetWayBillInfos(IEnumerable<string> wayBillOrTranckingNumers)
        {
            var enumerable = wayBillOrTranckingNumers as string[] ?? wayBillOrTranckingNumers.ToArray();
            if (enumerable.Length < 0)
                return null;
            return _wayBillInfoRepository.GetList(p => enumerable.Contains(p.WayBillNumber) || enumerable.Contains(p.CustomerOrderNumber) || enumerable.Contains(p.TrackingNumber) || enumerable.Contains(p.TrueTrackingNumber));
        }

        public IPagedList<AbnormalWayBillModel> GetAbnormalWayBillPagedList(AbnormalWayBillParam param)
        {
            var startTime = param.StartTime.HasValue ? param.StartTime.Value : new DateTime(2013, 1, 1);
            var endTime = param.EndTime.HasValue ? param.EndTime.Value : new DateTime(2020, 1, 1);
            var ishold = true;
            var numberList = new List<string>();
            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberList =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();
                param.Page = 1;
                param.PageSize = 2000;
            }
            Expression<Func<WayBillInfo, bool>> filter = p => true;
            filter = filter.And(p => p.AbnormalID.HasValue);
            if (param.SearchWhere.HasValue && numberList.Count > 0)
            {
                switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
                {
                    case WayBill.SearchFilterEnum.WayBillNumber:
                        filter = filter.And(p => numberList.Contains(p.WayBillNumber));
                        break;
                    case WayBill.SearchFilterEnum.TrackingNumber:
                        filter = filter.And(p => numberList.Contains(p.TrackingNumber));
                        break;
                    case WayBill.SearchFilterEnum.CustomerOrderNumber:
                        filter = filter.And(p => numberList.Contains(p.CustomerOrderNumber));
                        break;
                    case WayBill.SearchFilterEnum.InStorageNumber:
                        filter = filter.And(p => numberList.Contains(p.InStorageID));
                        break;
                    case WayBill.SearchFilterEnum.OutStorageNumber:
                        filter = filter.And(p => numberList.Contains(p.OutStorageID));
                        break;
                }
            }

            switch (WayBill.ParseToDateFilter(param.DateWhere))
            {
                case WayBill.DateFilterEnum.CreatedOn:
                    filter =
                        filter.And(
                            p =>
                            p.AbnormalWayBillLog.CreatedOn >= startTime && p.AbnormalWayBillLog.CreatedOn <= endTime);
                    break;
                case WayBill.DateFilterEnum.TakeOverOn:
                    if (param.StartTime.HasValue || param.EndTime.HasValue)
                    {
                        filter =
                            filter.And(
                                p => p.InStorageCreatedOn >= startTime && p.InStorageCreatedOn <= endTime);
                    }
                    break;
                case WayBill.DateFilterEnum.DeliverOn:
                    if (param.StartTime.HasValue || param.EndTime.HasValue)
                    {
                        filter =
                            filter.And(
                                p => p.OutStorageCreatedOn >= startTime && p.OutStorageCreatedOn <= endTime);
                    }
                    break;
            }


            int inAbnormal = (int)WayBill.AbnormalTypeEnum.InStorageWeightAbnormal;

            filter = filter.AndIf(p => p.CustomerCode == param.CustomerCode,
                                  !string.IsNullOrWhiteSpace(param.CustomerCode))
                           .AndIf(p => p.InShippingMethodID == param.ShippingMethodId, param.ShippingMethodId.HasValue)
                           .AndIf(p => p.CountryCode == param.CountryCode, !string.IsNullOrWhiteSpace(param.CountryCode))
                           .AndIf(p => p.AbnormalWayBillLog.AbnormalStatus == param.Status, param.Status.HasValue)
                           .AndIf(p => p.Status == param.WaybillStatus, param.WaybillStatus.HasValue) //运单状态 add by yungchu
                           .And(p => p.AbnormalWayBillLog.OperateType != inAbnormal) //排除入仓异常类型
                           .And(p => p.IsHold == ishold); //排出掉不是hold的运单

            Func<IQueryable<AbnormalWayBillModel>, IOrderedQueryable<AbnormalWayBillModel>>
                orderBy = o => o.OrderByDescending(p => p.AbnormalCreateOn);
            return _wayBillInfoRepository.FindPagedListAbnormalWayBillModel(param.Page, param.PageSize, filter, orderBy);
        }

        /// <summary>
        /// 导出异常运单
        /// Add By zhengsong
        /// Time:2014-07-09
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<AbnormalWayBillModel> GetAbnormalWayBillList(AbnormalWayBillParam param)
        {
            var startTime = param.StartTime.HasValue ? param.StartTime.Value : new DateTime(2013, 1, 1);
            var endTime = param.EndTime.HasValue ? param.EndTime.Value : new DateTime(2020, 1, 1);
            var ishold = true;
            var numberList = new List<string>();
            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberList =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();
            }
            Expression<Func<WayBillInfo, bool>> filter = p => true;
            filter = filter.And(p => p.AbnormalID.HasValue);
            if (param.SearchWhere.HasValue && numberList.Count > 0)
            {
                switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
                {
                    case WayBill.SearchFilterEnum.WayBillNumber:
                        filter = filter.And(p => numberList.Contains(p.WayBillNumber));
                        break;
                    case WayBill.SearchFilterEnum.TrackingNumber:
                        filter = filter.And(p => numberList.Contains(p.TrackingNumber));
                        break;
                    case WayBill.SearchFilterEnum.CustomerOrderNumber:
                        filter = filter.And(p => numberList.Contains(p.CustomerOrderNumber));
                        break;
                }
            }

            switch (WayBill.ParseToDateFilter(param.DateWhere))
            {
                case WayBill.DateFilterEnum.CreatedOn:
                    filter =
                        filter.And(
                            p =>
                            p.AbnormalWayBillLog.CreatedOn >= startTime && p.AbnormalWayBillLog.CreatedOn <= endTime);
                    break;
                case WayBill.DateFilterEnum.TakeOverOn:
                    if (param.StartTime.HasValue || param.EndTime.HasValue)
                    {
                        filter =
                            filter.And(
                                p => p.InStorageCreatedOn >= startTime && p.InStorageCreatedOn <= endTime);
                    }
                    break;
                case WayBill.DateFilterEnum.DeliverOn:
                    if (param.StartTime.HasValue || param.EndTime.HasValue)
                    {
                        filter =
                            filter.And(
                                p => p.OutStorageCreatedOn >= startTime && p.OutStorageCreatedOn <= endTime);
                    }
                    break;
            }
            filter = filter.AndIf(p => p.CustomerCode == param.CustomerCode,
                                  !string.IsNullOrWhiteSpace(param.CustomerCode))
                           .AndIf(p => p.InShippingMethodID == param.ShippingMethodId, param.ShippingMethodId.HasValue)
                           .AndIf(p => p.CountryCode == param.CountryCode, !string.IsNullOrWhiteSpace(param.CountryCode))
                           .AndIf(p => p.AbnormalWayBillLog.AbnormalStatus == param.Status, param.Status.HasValue)
                           .And(p => p.IsHold == ishold); //排出掉不是hold的运单;

            Func<IQueryable<AbnormalWayBillModel>, IOrderedQueryable<AbnormalWayBillModel>>
                orderBy = o => o.OrderByDescending(p => p.AbnormalCreateOn);

            return _wayBillInfoRepository.FindListAbnormalWayBillModel(filter, orderBy);
        }

        public List<GoodsTypeInfo> GetGoodsTypes()
        {
            List<GoodsTypeInfo> list = new List<GoodsTypeInfo>();
            list = _goodsTypeInfoRepository.GetList(p => p.IsDelete == false);
            return list;
        }

        /// <summary>
        /// 运单拦截（运单管理）
        /// </summary>
        /// <param name="wayBillNumber"></param>
        public void HoldWayBillInfo(string wayBillNumber)
        {
            HoldWayBillInfoData(wayBillNumber);
            _wayBillInfoRepository.UnitOfWork.Commit();
        }

        private void HoldWayBillInfoData(string wayBillNumber)
        {
            Check.Argument.IsNullOrWhiteSpace(wayBillNumber, "运单号");
            var status = WayBill.StatusToValue(WayBill.StatusEnum.Send);
            var model = _wayBillInfoRepository.First(p => p.WayBillNumber == wayBillNumber && p.Status < status);
            if (model == null)
            {
                throw new ArgumentException("该运单号\"{0}\"不存在，或则是已经发货！".FormatWith(wayBillNumber));
            }
            else
            {
                if (model.IsHold)
                {
                    throw new ArgumentException("该运单号\"{0}\"已经Hold！".FormatWith(wayBillNumber));
                }
                model.IsHold = true;
                model.LastUpdatedBy = _workContext.User.UserUame;
                model.LastUpdatedOn = DateTime.Now;
                if (model.CustomerOrderID.HasValue)
                {
                    model.CustomerOrderInfo.IsHold = true;
                    model.CustomerOrderInfo.LastUpdatedBy = _workContext.User.UserUame;
                    model.LastUpdatedOn = DateTime.Now;
                }
                var abnormalWayBillLog = new AbnormalWayBillLog();
                abnormalWayBillLog.CreatedBy = abnormalWayBillLog.LastUpdatedBy = _workContext.User.UserUame;
                abnormalWayBillLog.CreatedOn = DateTime.Now;
                abnormalWayBillLog.LastUpdatedOn = DateTime.Now;
                abnormalWayBillLog.OperateType = WayBill.AbnormalTypeToValue(WayBill.AbnormalTypeEnum.Intercepted);
                abnormalWayBillLog.AbnormalStatus = WayBill.AbnormalStatusToValue(WayBill.AbnormalStatusEnum.NO);
                //abnormalWayBillLog.AbnormalDescription = "时间：{0},操作人：{1},运单号：{2},内部拦截".FormatWith(DateTime.Now.ToString("yyyy-MM-dd HH;mm"), _workContext.User.UserUame, wayBillNumber);
                abnormalWayBillLog.AbnormalDescription = "客户要求Hold订单";
                model.AbnormalWayBillLog = abnormalWayBillLog;
                _wayBillInfoRepository.Modify(model);
            }

        }

        /// <summary>
        /// 批量拦截（运单管理）
        /// </summary>
        /// <param name="wayBillNumberList"></param>
        public void BatchHoldWayBillInfo(List<string> wayBillNumberList)
        {
            using (var transaction = new TransactionScope())
            {
                foreach (var wayBillNumber in wayBillNumberList)
                {
                    HoldWayBillInfoData(wayBillNumber);

                    // #region 操作日志
                    // //yungchu
                    // //敏感字-无
                    // BizLog bizlog = new BizLog()
                    // {
                    //	 Summary = "运单拦截",
                    //	 KeywordType = KeywordType.WayBillNumber,
                    //	 Keyword = wayBillNumber,
                    //	 UserCode = _workContext.User.UserUame,
                    //	 UserRealName = _workContext.User.UserUame,
                    //	 UserType = UserType.LMS_User,
                    //	 SystemCode = SystemType.LMS,
                    //	 ModuleName = "运单修改"
                    // };

                    // string number = wayBillNumber;
                    // WayBillInfoExtSilm wayBillInfoExtSilm=new WayBillInfoExtSilm();
                    // _wayBillInfoRepository.First(a => a.WayBillNumber == number).CopyTo(wayBillInfoExtSilm);

                    //_operateLogServices.WriteLog(bizlog,wayBillInfoExtSilm);
                    // #endregion

                }
                _wayBillInfoRepository.UnitOfWork.Commit();
                transaction.Complete();
            }
        }

        public void UpdateWayBillInfo(WayBillInfo wayBillInfo)
        {
            WayBillInfoData(wayBillInfo);
            _wayBillInfoRepository.UnitOfWork.Commit();
        }

        public void WayBillInfoData(WayBillInfo wayBillInfo)
        {
            Check.Argument.IsNotNull(wayBillInfo, "运单");
            Check.Argument.IsNullOrWhiteSpace(wayBillInfo.WayBillNumber, "运单号");
            //待转单
            var waitOrder = WayBill.StatusToValue(WayBill.StatusEnum.WaitOrder);
            var status = WayBill.StatusToValue(WayBill.StatusEnum.Delete);
            //var model =
            //    _wayBillInfoRepository.First(p => p.WayBillNumber == wayBillInfo.WayBillNumber && p.Status < status);
            var model =
                _wayBillInfoRepository.First(p => p.WayBillNumber == wayBillInfo.WayBillNumber && (p.Status < status || p.Status == waitOrder));
            if (model == null)
            {
                throw new ArgumentException("该运单号\"{0}\"不存在，或不是已经提交状态！".FormatWith(wayBillInfo.WayBillNumber));
            }
            if (IsExitTrackingNumber(wayBillInfo.TrackingNumber, wayBillInfo.WayBillNumber) && !string.IsNullOrEmpty(wayBillInfo.TrackingNumber))
            {
                throw new ArgumentException("[{0}]运单号的跟踪号[{1}]已经存在！".FormatWith(wayBillInfo.WayBillNumber, wayBillInfo.TrackingNumber));
            }
            //if (model.Status < WayBill.StatusToValue(WayBill.StatusEnum.Have) && wayBillInfo.InShippingMethodID.HasValue)
            ShippingMethodModel shippingMethodModel = new ShippingMethodModel();
            if (wayBillInfo.InShippingMethodID.HasValue)
            {

                shippingMethodModel = _freightService.GetShippingMethod(wayBillInfo.InShippingMethodID.Value);
                model.InShippingMethodID = wayBillInfo.InShippingMethodID;
                model.InShippingMethodName = wayBillInfo.InShippingMethodName;
                //修改客户订单信息
                if (model.CustomerOrderID.HasValue)
                {
                    model.CustomerOrderInfo.ShippingMethodId = wayBillInfo.InShippingMethodID;
                    model.CustomerOrderInfo.ShippingMethodName = wayBillInfo.InShippingMethodName;
                    if (shippingMethodModel != null && shippingMethodModel.IsHideTrackingNumber)
                    {
                    }
                    else if (!string.IsNullOrWhiteSpace(wayBillInfo.TrackingNumber))
                    {
                        model.CustomerOrderInfo.TrackingNumber = wayBillInfo.TrackingNumber.ToUpperInvariant();
                    }
                    model.CustomerOrderInfo.LastUpdatedBy = _workContext.User.UserUame;
                    model.CustomerOrderInfo.LastUpdatedOn = DateTime.Now;
                }
            }
            //Update By zhengsong;Time:2014-06-06
            //根据是否影藏跟踪号来进行跟踪号更改
            if (!string.IsNullOrEmpty(wayBillInfo.TrackingNumber) && wayBillInfo.TrackingNumber != model.TrackingNumber && shippingMethodModel != null)
            {
                //是否插入真实跟踪号
                if (shippingMethodModel.IsHideTrackingNumber)
                {
                    model.TrueTrackingNumber = wayBillInfo.TrackingNumber;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(model.TrackingNumber) &&
                        string.IsNullOrWhiteSpace(model.RawTrackingNumber))
                    {
                        model.RawTrackingNumber = model.TrackingNumber.ToUpperInvariant();
                        model.TrackingNumber = wayBillInfo.TrackingNumber.ToUpperInvariant();
                        model.TransferOrderDate = DateTime.Now;
                    }
                    else if (!string.IsNullOrWhiteSpace(model.TrackingNumber) &&
                             !string.IsNullOrWhiteSpace(model.RawTrackingNumber))
                    {
                        model.TrackingNumber = wayBillInfo.TrackingNumber.ToUpperInvariant();
                        model.TransferOrderDate = DateTime.Now;
                    }
                    else
                    {
                        model.TrackingNumber = wayBillInfo.TrackingNumber;
                    }
                }

            }
            model.LastUpdatedBy = _workContext.User.UserUame;
            model.LastUpdatedOn = DateTime.Now;
            _wayBillInfoRepository.Modify(model);
        }

        /// <summary>
        /// 运单日志保存记录时修改运单信息
        /// add by yungchu
        /// </summary>
        /// <param name="wayBillInfo"></param>
        public void UpdateWayBillInfos(WayBillInfo wayBillInfo)
        {


            Check.Argument.IsNotNull(wayBillInfo, "运单");
            Check.Argument.IsNullOrWhiteSpace(wayBillInfo.WayBillNumber, "运单号");

            var status = WayBill.StatusToValue(WayBill.StatusEnum.Send);
            var model = _wayBillInfoRepository.First(p => p.WayBillNumber == wayBillInfo.WayBillNumber && p.Status < status);
            if (model == null)
            {
                throw new ArgumentException("该运单号\"{0}\"不存在，或不是已经提交状态！".FormatWith(wayBillInfo.WayBillNumber));
            }



            model.VenderCode = wayBillInfo.VenderCode;
            model.OutShippingMethodID = wayBillInfo.OutShippingMethodID;

            List<int> getid = new List<int>() { };
            getid.Add(wayBillInfo.OutShippingMethodID.Value);
            List<ShippingMethodModel> ls = _freightService.GetShippingMethodsByIds(getid);
            foreach (var shippingMethodModel in ls)
            {
                model.OutShippingMethodName = shippingMethodModel.FullName;
            }

            //判断运输方式是否隐藏跟踪号
            ShippingMethodModel getshippingMethodModel = _freightService.GetShippingMethod(model.InShippingMethodID.Value);
            if (getshippingMethodModel.IsHideTrackingNumber)
            {
                model.TrueTrackingNumber = wayBillInfo.TrackingNumber;
            }
            else
            {
                model.TrackingNumber = wayBillInfo.TrackingNumber;
                //原跟踪号为空时
                //if (wayBillInfo.RawTrackingNumber == string.Empty)
                //{
                //	model.RawTrackingNumber = wayBillInfo.TrackingNumber;
                //}
            }

            model.TransferOrderDate = DateTime.Now;
            _wayBillInfoRepository.Modify(model);
            _wayBillInfoRepository.UnitOfWork.Commit();


        }

        /// <summary>
        /// 批量修改运输方式
        /// </summary>
        /// <param name="wayBillNumberList"></param>
        /// <param name="shippingMethodId">改运输方式ID</param>
        /// <param name="shippingMethodName">改运输方式名称</param>
        public void BatchUpdateWayBillInfo(List<string> wayBillNumberList, int? shippingMethodId, string shippingMethodName)
        {
            using (var transaction = new TransactionScope())
            {
                foreach (var wayBillNumber in wayBillNumberList)
                {
                    var wayBillInfo = new WayBillInfo() { WayBillNumber = wayBillNumber, InShippingMethodID = shippingMethodId, InShippingMethodName = shippingMethodName };

                    //#region 操作日志
                    ////yungchu
                    ////敏感字-无
                    //StringBuilder sb = new StringBuilder();
                    //sb.Append("");
                    //string number = wayBillNumber;
                    //WayBillInfo waybillinfo = _wayBillInfoRepository.First(a => a.WayBillNumber == number);
                    //if (waybillinfo.InShippingMethodID != shippingMethodId)
                    //{
                    //	sb.AppendFormat(" 运输方式从{0}更改为{1}", waybillinfo.InShippingMethodName, shippingMethodName);
                    //}

                    //BizLog bizlog = new BizLog()
                    //{
                    //	Summary = sb.ToString() != "" ? "[运单修改]" + sb : "运单修改",
                    //	KeywordType = KeywordType.WayBillNumber,
                    //	Keyword = number,
                    //	UserCode = _workContext.User.UserUame,
                    //	UserRealName = _workContext.User.UserUame,
                    //	UserType = UserType.LMS_User,
                    //	SystemCode = SystemType.LMS,
                    //	ModuleName = "运单修改"
                    //};
                    //WayBillInfoExtSilm wayBillInfoExtSilm = new WayBillInfoExtSilm();
                    //waybillinfo.CopyTo(wayBillInfoExtSilm);
                    //_operateLogServices.WriteLog(bizlog, wayBillInfoExtSilm);
                    //#endregion

                    UpdateWayBillInfoData(wayBillInfo);
                }
                _wayBillInfoRepository.UnitOfWork.Commit();
                //_trackingNumberDetailInfoRepository.UnitOfWork.Commit();
                transaction.Complete();
            }
        }

        public void UpdateWayBillInfoData(WayBillInfo wayBillInfo)
        {
            Check.Argument.IsNotNull(wayBillInfo, "运单");
            Check.Argument.IsNullOrWhiteSpace(wayBillInfo.WayBillNumber, "运单号");
            var statusSubmitted = WayBill.StatusToValue(WayBill.StatusEnum.Submitted);
            var model =
                _wayBillInfoRepository.First(p => p.WayBillNumber == wayBillInfo.WayBillNumber && p.Status == statusSubmitted);
            if (model == null)
            {
                throw new ArgumentException("该运单号\"{0}\"不存在，或不是已经提交状态！".FormatWith(wayBillInfo.WayBillNumber));
            }
            if (IsExitTrackingNumber(wayBillInfo.TrackingNumber, wayBillInfo.WayBillNumber))
            {
                throw new ArgumentException("[{0}]运单号的跟踪号[{1}]已经存在！".FormatWith(wayBillInfo.WayBillNumber, wayBillInfo.TrackingNumber));
            }
            //if (model.Status < WayBill.StatusToValue(WayBill.StatusEnum.Have) && wayBillInfo.InShippingMethodID.HasValue)
            ShippingMethodModel shippingMethodModel = new ShippingMethodModel();
            if (wayBillInfo.InShippingMethodID.HasValue)
            {

                shippingMethodModel = _freightService.GetShippingMethod(wayBillInfo.InShippingMethodID.Value);
                model.InShippingMethodID = wayBillInfo.InShippingMethodID;
                model.InShippingMethodName = wayBillInfo.InShippingMethodName;
                if (model.IsHold) //add by yungchu
                {
                    model.IsHold = false;
                }

                //修改客户订单信息
                if (model.CustomerOrderID.HasValue)
                {
                    model.CustomerOrderInfo.ShippingMethodId = wayBillInfo.InShippingMethodID;
                    model.CustomerOrderInfo.ShippingMethodName = wayBillInfo.InShippingMethodName;
                    if (model.CustomerOrderInfo.IsPrinted)
                    {
                        model.CustomerOrderInfo.IsPrinted = false; //add by yungchu
                    }
                    model.CustomerOrderInfo.LastUpdatedBy = _workContext.User.UserUame;
                    model.CustomerOrderInfo.LastUpdatedOn = DateTime.Now;
                    model.CustomerOrderInfo.IsHold = false;
                }

                if (shippingMethodModel != null && shippingMethodModel.IsSysTrackNumber)
                {
                    //跟踪号为空时才去系统分配 update by yungchu
                    if (string.IsNullOrEmpty(model.TrackingNumber))
                    {

                        List<int> shippingMethodIds = new List<int>();
                        shippingMethodIds.Add(shippingMethodModel.ShippingMethodId);
                        var trackingNumberDetailInfo = _trackingNumberService.TrackNumberAssignStandard(wayBillInfo.InShippingMethodID.Value, 1, model.CountryCode);

                        //List<TrackingNumberInfo> trackingNumbers = _trackingNumberService.GetTrackingNumbers(shippingMethodIds);
                        //var trackingNumberList = trackingNumbers.FindAll(p => p.ShippingMethodID == wayBillInfo.InShippingMethodID && p.ApplianceCountry.Contains(model.CountryCode));
                        //if (!trackingNumberList.Any())
                        //{
                        //    throw new ArgumentException("[{0}]运输方式无可分配的跟踪号！".FormatWith(model.InShippingMethodName));
                        //}
                        //TrackingNumberDetailInfo trackingNumberDetailInfo = new TrackingNumberDetailInfo();
                        //foreach (var trackingNumberInfo in trackingNumberList)
                        //{
                        //    trackingNumberDetailInfo =
                        //        trackingNumberInfo.TrackingNumberDetailInfos.ToList()
                        //                          .FirstOrDefault(p => p.Status == (short)TrackingNumberDetailInfo.StatusEnum.NotUsed);
                        //    if (trackingNumberDetailInfo != null)
                        //    {
                        //        break;
                        //    }
                        //}

                        if (trackingNumberDetailInfo != null && trackingNumberDetailInfo.Any())
                        {
                            model.TrackingNumber = trackingNumberDetailInfo[0];
                            model.CustomerOrderInfo.TrackingNumber = trackingNumberDetailInfo[0];

                            //trackingNumberDetailInfo.Status = (short)TrackingNumberDetailInfo.StatusEnum.Used;
                            //trackingNumberDetailInfo.WayBillNumber = wayBillInfo.WayBillNumber;
                            //_trackingNumberDetailInfoRepository.Modify(trackingNumberDetailInfo);
                        }
                        else
                        {
                            throw new ArgumentException("[{0}]运输方式无可分配的跟踪号！".FormatWith(model.InShippingMethodName));
                        }

                    }
                }
            }
            model.LastUpdatedBy = _workContext.User.UserUame;
            model.LastUpdatedOn = DateTime.Now;
            _wayBillInfoRepository.Modify(model);
        }

        public bool IsExitOrderNUmber(string orderNumber, string customerCode)
        {
            var deleteStatus = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Delete);
            var returnStatus = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Return);
            return
                _customerOrderInfoRepository.Exists(
                    p => p.CustomerOrderNumber == orderNumber && p.Status != deleteStatus && p.Status != returnStatus);
            //p.CustomerCode == customerCode && 
        }

        /// <summary>
        /// 判断运单的跟踪号是否存在
        /// Update By zhengsong
        /// Time:2014-05-28
        /// </summary>
        /// <param name="trackingNumber"></param>
        /// <returns></returns>
        public bool IsExitTrackingNumber(string trackingNumber)
        {
            var deleteStatus = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Delete);
            var returnStatus = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Return);
            return
                _customerOrderInfoRepository.Exists(
                    p => p.TrackingNumber == trackingNumber && p.Status != deleteStatus && p.Status != returnStatus);
        }

        /// <summary>
        /// 判断运单的跟踪号是否存在
        /// Add by zhengsong
        /// Time:2014-05-28
        /// </summary>
        /// <param name="trackingNumber"></param>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        public bool IsExitTrackingNumber(string trackingNumber, string wayBillNumber)
        {
            var deleteStatus = WayBill.StatusToValue(WayBill.StatusEnum.Delete);
            var returnStatus = WayBill.StatusToValue(WayBill.StatusEnum.Return);
            if (string.IsNullOrWhiteSpace(wayBillNumber))
            {
                return
                    _wayBillInfoRepository.Exists(
                        p => (p.TrackingNumber == trackingNumber || p.TrueTrackingNumber == trackingNumber) && p.Status != deleteStatus && p.Status != returnStatus);
            }
            else
            {
                return
                    _wayBillInfoRepository.Exists(
                        p => (p.TrackingNumber == trackingNumber || p.TrueTrackingNumber == trackingNumber) && p.Status != deleteStatus && p.Status != returnStatus && p.WayBillNumber != wayBillNumber);
            }
        }

        /// <summary>
        /// 取消拦截运单
        /// </summary>
        /// <param name="wayBillNumber"></param>
        public void CancelHoldWayBillInfo(string wayBillNumber)
        {
            CancelHoldWayBillInfoData(wayBillNumber);
            _wayBillInfoRepository.UnitOfWork.Commit();
            _wayBillEventLogRepository.UnitOfWork.Commit();
        }

        private void CancelHoldWayBillInfoData(string wayBillNumber)
        {
            Check.Argument.IsNullOrWhiteSpace(wayBillNumber, "运单号");
            var status = WayBill.StatusToValue(WayBill.StatusEnum.Send);
            var model = _wayBillInfoRepository.First(p => p.WayBillNumber == wayBillNumber && p.Status < status);
            if (model == null)
            {
                throw new ArgumentException("该运单号\"{0}\"不存在，或则是已经发货！".FormatWith(wayBillNumber));
            }
            else
            {
                if (!model.IsHold)
                {
                    throw new ArgumentException("该运单号\"{0}\"已经不是Hold！".FormatWith(wayBillNumber));
                }
                model.IsHold = false;
                model.LastUpdatedBy = _workContext.User.UserUame;
                model.LastUpdatedOn = DateTime.Now;
                if (model.CustomerOrderID.HasValue)
                {
                    model.CustomerOrderInfo.IsHold = false;
                    model.CustomerOrderInfo.LastUpdatedBy = _workContext.User.UserUame;
                    model.LastUpdatedOn = DateTime.Now;
                }
                if (model.AbnormalID.HasValue)
                {
                    model.AbnormalWayBillLog.AbnormalStatus =
                        WayBill.AbnormalStatusToValue(WayBill.AbnormalStatusEnum.OK);
                    model.AbnormalWayBillLog.LastUpdatedOn = DateTime.Now;
                    model.AbnormalWayBillLog.LastUpdatedBy = _workContext.User.UserUame;
                }
                _wayBillInfoRepository.Modify(model);

            }
        }

        /// <summary>
        /// 取消拦截运单(批量)
        /// </summary>
        /// <param name="wayBillNumberList"></param>
        public void BatchCancelHoldWayBillInfo(List<string> wayBillNumberList)
        {
            using (var transaction = new TransactionScope())
            {
                foreach (var wayBillNumber in wayBillNumberList)
                {
                    CancelHoldWayBillInfoData(wayBillNumber);
                }
                _wayBillInfoRepository.UnitOfWork.Commit();
                transaction.Complete();
            }

        }

        /// <summary>
        /// 删除WayBillInfo
        /// </summary>
        /// <param name="wayBillNumber"></param>
        public void DeleteWayBillInfo(string wayBillNumber)
        {
            using (var transaction = new TransactionScope())
            {
                DeleteWayBillInfoData(wayBillNumber);
                _wayBillInfoRepository.UnitOfWork.Commit();
                _receivingExpensRepository.UnitOfWork.Commit();
                transaction.Complete();
            }
        }

        private void DeleteWayBillInfoData(string wayBillNumber)
        {
            Check.Argument.IsNullOrWhiteSpace(wayBillNumber, "运单号");
            var status = WayBill.StatusToValue(WayBill.StatusEnum.Send);

           // var model = _wayBillInfoRepository.First(p => p.WayBillNumber == wayBillNumber && p.Status < status);
			var model = _wayBillInfoRepository.Single(p => p.WayBillNumber == wayBillNumber && p.Status < status);
            if (model == null)
            {
                throw new ArgumentException("该运单号\"{0}\"不存在，或则是已经发货！".FormatWith(wayBillNumber));
            }
            else
            {
				if (!model.IsHold)
				{
					throw new ArgumentException("该运单号\"{0}\"已经不是Hold！".FormatWith(wayBillNumber));
				}

                var isReturnCash = model.Status == WayBill.StatusToValue(WayBill.StatusEnum.Have);
                model.Status =
                    WayBill.StatusToValue(isReturnCash ? WayBill.StatusEnum.Return : WayBill.StatusEnum.Delete);
                if (model.CustomerOrderID.HasValue)
                {
                    model.CustomerOrderInfo.Status =
                        CustomerOrder.StatusToValue(isReturnCash
                                                        ? CustomerOrder.StatusEnum.Return
                                                        : CustomerOrder.StatusEnum.Delete);
                    model.CustomerOrderInfo.IsHold = false;
                    model.CustomerOrderInfo.LastUpdatedBy = _workContext.User.UserUame;
                    model.LastUpdatedOn = DateTime.Now;
                    var orderstatus = new CustomerOrderStatus
                        {
                            CreatedOn = DateTime.Now,
                            Status = CustomerOrder.StatusToValue(isReturnCash
                                                                     ? CustomerOrder.StatusEnum.Return
                                                                     : CustomerOrder.StatusEnum.Delete),
                            Remark = isReturnCash ? "已退款" : "已删除"
                        };
                    model.CustomerOrderInfo.CustomerOrderStatuses.Add(orderstatus);
                }
                model.LastUpdatedBy = _workContext.User.UserUame;
                model.LastUpdatedOn = DateTime.Now;
                model.AbnormalWayBillLog.AbnormalStatus = WayBill.AbnormalStatusToValue(WayBill.AbnormalStatusEnum.OK);
                model.AbnormalWayBillLog.LastUpdatedOn = DateTime.Now;
                model.AbnormalWayBillLog.LastUpdatedBy = _workContext.User.UserUame;
                //修改 bit 类型Hold 为 false(0)
                model.IsHold = false;
                _wayBillInfoRepository.Modify(model);

                ////获取该运单所有扣款
                //var cashlist =
                //    _customerAmountRecordRepository.GetList(
                //        p =>
                //        p.CustomerCode == model.CustomerCode && p.WayBillNumber == wayBillNumber &&
                //        p.MoneyChangeTypeID == 2).ToList();

                #region 录入内部信息

                if (model.Status == WayBill.StatusToValue(WayBill.StatusEnum.Return))
                {

                    //Add By zxq
                    //Time:2014-09-15
                    var wayBillEventLog = new WayBillEventLog()
                        {
                            WayBillNumber = model.WayBillNumber,
                            EventCode = (int)WayBillEvent.EventCodeEnum.ReturnGood,
                            Description = WayBillEvent.GetEventCodeDescription((int)WayBillEvent.EventCodeEnum.ReturnGood),
                            EventDate = DateTime.Now,
                            LastUpdatedOn = DateTime.Now,
                            Operator = _workContext.User.UserUame,
                        };
                    _wayBillEventLogRepository.Add(wayBillEventLog);



                    #region 新增退回记录 zxq

                    // Update By zhengsong Tiem:2014-07-12
                    ReceivingExpensesEditExt receivingExpensesEditExt = new ReceivingExpensesEditExt();
                    receivingExpensesEditExt = _financialService.GetReceivingExpensesEditExt(model.WayBillNumber);
                    var returnGoodsIds = SequenceNumberService.GetSequenceNumber(PrefixCode.ReturnGoodsID, 1);
                    ReturnGoods rgGoods = new ReturnGoods();
                    rgGoods.WayBillNumber = model.WayBillNumber;
                    rgGoods.Weight = model.Weight.Value;
                    rgGoods.CreatedBy = _workContext.User.UserUame;
                    rgGoods.CreatedOn = DateTime.Now;
                    rgGoods.LastUpdatedBy = _workContext.User.UserUame;
                    rgGoods.LastUpdatedOn = DateTime.Now;
                    rgGoods.Reason = "客户要求";
                    rgGoods.ReasonRemark = "后台退回";
                    rgGoods.ReGoodsId = returnGoodsIds;
                    rgGoods.Type = 2;
                    rgGoods.Status = (int)ReturnGood.ReturnStatusEnum.Audited;
                    rgGoods.ReturnSource = (int)ReturnGood.ReturnSourceStatusEnum.BSReturn;
                    rgGoods.Auditor = _workContext.User.UserUame;
                    rgGoods.AuditorDate = DateTime.Now;

                    if (receivingExpensesEditExt != null)
                    {
                        if (receivingExpensesEditExt.TotalFeeFinal.HasValue)
                        {
                            rgGoods.ShippingFee = receivingExpensesEditExt.TotalFeeFinal ?? 0;
                        }
                        else if (receivingExpensesEditExt.TotalFeeOriginal.HasValue)
                        {
                            rgGoods.ShippingFee = receivingExpensesEditExt.TotalFeeOriginal ?? 0;
                        }
                        rgGoods.IsReturnShipping = true;
                    }
                    else
                    {
                        rgGoods.ShippingFee = 0;

                        rgGoods.IsReturnShipping = false;
                    }
                    _returnGoodsRepository.Add(rgGoods);
                    _returnGoodsRepository.UnitOfWork.Commit();

                    #endregion

                    #region 在收货费用详细表添加一个退回数据，状态为4

                    _financialService.UpdateReceivingExpenseInfo(model.WayBillNumber, _workContext.User.UserUame);
                    _receivingExpensRepository.UnitOfWork.Commit();
                    _wayBillEventLogRepository.UnitOfWork.Commit();

                    #endregion
                }

                #endregion

                //cashlist.Each(p =>
                //    {
                //        if (p.Amount.HasValue && p.Amount.Value < 0)
                //        {
                //            _customerAmountRecordRepository.CreateCustomerAmountRecord(new CustomerAmountRecordParam
                //                                                                           ()
                //                {
                //                    Amount = Math.Abs(p.Amount.Value),
                //                    CreatedBy = _workContext.User.UserUame,
                //                    CustomerCode = p.CustomerCode,
                //                    FeeTypeId = p.FeeTypeID.Value,
                //                    MoneyChangeTypeId = 3,
                //                    TransactionNo = model.InStorageID,
                //                    WayBillNumber = p.WayBillNumber,
                //                    Remark = "运单号：{0}{1}退款".FormatWith(p.WayBillNumber, p.FeeType.FeeTypeName),
                //                });
                //        }
                //    });


            }
        }

        /// <summary>
        ///  删除WayBillInfo(批量)
        /// </summary>
        /// <param name="wayBillNumberList"></param>
        public void BatchDeleteWayBillInfo(List<string> wayBillNumberList)
        {
            using (var transaction = new TransactionScope())
            {
                foreach (var wayBillNumber in wayBillNumberList)
                {
                    DeleteWayBillInfoData(wayBillNumber);
                }
                _wayBillInfoRepository.UnitOfWork.Commit();
                transaction.Complete();
            }
        }

        /// <summary>
        /// 批量退回异常运单
        /// </summary>
        /// <param name="wayBillNumberList"></param>
        public void BatchReturnWayBillInfo(List<string> wayBillNumberList)
        {
            using (var transaction = new TransactionScope())
            {
                foreach (var wayBillNumber in wayBillNumberList)
                {
                    DeleteWayBillInfoData(wayBillNumber);
                }
                _wayBillInfoRepository.UnitOfWork.Commit();
                transaction.Complete();
            }
        }


        public void AddAbnormalWayBill(string wayBillNumber, WayBill.AbnormalTypeEnum typeEnum, string description)
        {
            AddAbnormalWayBill(wayBillNumber, typeEnum, description, _workContext.User.UserUame);
        }

        public void AddAbnormalWayBill(string wayBillNumber, WayBill.AbnormalTypeEnum typeEnum, string description, string userUame)
        {
            Check.Argument.IsNullOrWhiteSpace(wayBillNumber, "运单号");
            var status = WayBill.StatusToValue(WayBill.StatusEnum.Send);
            var model = _wayBillInfoRepository.First(p => p.WayBillNumber == wayBillNumber && p.Status < status);
            if (model == null)
            {
                throw new ArgumentException("该运单号\"{0}\"不存在，或则是已经发货！".FormatWith(wayBillNumber));
            }
            else
            {
                if (model.IsHold)
                {
                    throw new ArgumentException("该运单号\"{0}\"已经Hold！".FormatWith(wayBillNumber));
                }
                model.IsHold = true;
                model.LastUpdatedBy = userUame;
                model.LastUpdatedOn = DateTime.Now;
                if (model.CustomerOrderID.HasValue)
                {
                    model.CustomerOrderInfo.IsHold = true;
                    model.CustomerOrderInfo.LastUpdatedBy = userUame;
                    model.LastUpdatedOn = DateTime.Now;
                }
                var operateType = WayBill.AbnormalTypeToValue(typeEnum);
                var abnormalWayBillLog = new AbnormalWayBillLog
                {
                    AbnormalStatus = WayBill.AbnormalStatusToValue(WayBill.AbnormalStatusEnum.NO),
                    AbnormalDescription = description,
                    LastUpdatedOn = DateTime.Now,
                    LastUpdatedBy = userUame,
                    CreatedOn = DateTime.Now,
                    CreatedBy = userUame,
                    OperateType = operateType
                };
                model.AbnormalWayBillLog = abnormalWayBillLog;
                _wayBillInfoRepository.Modify(model);
                _wayBillInfoRepository.UnitOfWork.Commit();
            }
        }

        /// <summary>
        /// 运单跟踪号更改
        /// Add by zhengsong
        /// </summary>
        /// <returns></returns>
        public void ChangeWayBillTrackingNumber(List<WayBillInfo> wayBillInfos)
        {
            var wayBillNumbers = new List<string>();
            wayBillInfos.ForEach(p => wayBillNumbers.Add(p.WayBillNumber));
            List<WayBillInfo> ways =
                _wayBillInfoRepository.GetList(
                    p => wayBillNumbers.Contains(p.WayBillNumber) || wayBillNumbers.Contains(p.CustomerOrderNumber) || wayBillNumbers.Contains(p.TrackingNumber));
            using (var traction = new TransactionScope())
            {
                ways.ForEach(p =>
                    {
                        WayBillInfo newWayBillInfo = new WayBillInfo();
                        string newTrackingNumber = "";
                        if (p.TrackingNumber != null)
                        {
                            newWayBillInfo = wayBillInfos.First(w =>
                                                                w.WayBillNumber.ToUpperInvariant() == p.WayBillNumber.ToUpperInvariant() ||
                                                                w.WayBillNumber.ToUpperInvariant() == p.CustomerOrderNumber.ToUpperInvariant() ||
                                                                w.WayBillNumber.ToUpperInvariant() == p.TrackingNumber.ToUpperInvariant());
                        }
                        else
                        {
                            newWayBillInfo = wayBillInfos.First(w =>
                                                                w.WayBillNumber.ToUpperInvariant() == p.WayBillNumber.ToUpperInvariant() ||
                                                                w.WayBillNumber.ToUpperInvariant() == p.CustomerOrderNumber.ToUpperInvariant());
                        }
                        if (newWayBillInfo != null)
                        {
                            newTrackingNumber = newWayBillInfo.TrackingNumber;
                        }
                        _wayBillInfoRepository.Modify(UpdataWayBillTrackingNumber(p, newTrackingNumber));
                    });
                _wayBillInfoRepository.UnitOfWork.Commit();
                traction.Complete();
            }
        }

        ///// <summary>
        ///// Add by zhengsong
        ///// </summary>
        ///// <param name="wayBillInfo">运单转单逻辑</param>
        ///// <param name="trackingNumber"></param>
        ///// <returns></returns>
        //public WayBillInfo UpdataWayBillTrackingNumber(WayBillInfo wayBillInfo)
        //{
        //    List<WayBillInfo> wayBillInfos = new List<WayBillInfo>();
        //    WayBillInfo way = new WayBillInfo();
        //    OrderListParam param = new OrderListParam();
        //    wayBillInfos = GetWayBillInfo(param).ToList();
        //    way = wayBillInfos.FirstOrDefault(
        //        p => p.WayBillNumber == wayBillInfo.WayBillNumber || p.TrackingNumber == wayBillInfo.WayBillNumber);
        //    if (way != null)
        //    {
        //        if (string.IsNullOrWhiteSpace(way.TrackingNumber))
        //        {
        //            way.TrackingNumber = wayBillInfo.TrackingNumber;
        //            if (way.Status ==WayBill.StatusEnum.WaitOrder.GetStatusValue())
        //            {
        //                way.Status = WayBill.StatusEnum.Send.GetStatusValue();
        //            }
        //        }
        //        else if (!string.IsNullOrWhiteSpace(way.WayBillNumber) &&
        //                 !string.IsNullOrWhiteSpace(way.RawTrackingNumber))
        //        {
        //            way.TrackingNumber = wayBillInfo.TrackingNumber;
        //            way.TransferOrderDate = DateTime.Now;
        //            if (way.Status == WayBill.StatusEnum.WaitOrder.GetStatusValue())
        //            {
        //                way.Status = WayBill.StatusEnum.Send.GetStatusValue();
        //            }
        //        }
        //        else if (!string.IsNullOrWhiteSpace(way.WayBillNumber) &&
        //                 string.IsNullOrWhiteSpace(way.RawTrackingNumber))
        //        {
        //            way.RawTrackingNumber = way.TrackingNumber;
        //            way.TrackingNumber = wayBillInfo.TrackingNumber;
        //            way.TransferOrderDate = DateTime.Now;
        //            if (way.Status == WayBill.StatusEnum.WaitOrder.GetStatusValue())
        //            {
        //                way.Status = WayBill.StatusEnum.Send.GetStatusValue();
        //            }
        //        }
        //    }
        //    return way;
        //}

        /// <summary>
        /// Add by zhengsong
        /// Update By zhensong;Time:2014-06-06
        /// </summary>
        /// <param name="wayBillInfo">运单转单逻辑</param>
        /// <param name="trackingNumber">跟踪号</param>
        /// <returns></returns>
        public WayBillInfo UpdataWayBillTrackingNumber(WayBillInfo wayBillInfo, string trackingNumber)
        {
            if (wayBillInfo == null)
                return null;
            ShippingMethodModel shippingMethod = new ShippingMethodModel();
            if (wayBillInfo.InShippingMethodID != null)
            {
                shippingMethod = _freightService.GetShippingMethod(wayBillInfo.InShippingMethodID.Value);
            }
            if (shippingMethod != null && shippingMethod.IsHideTrackingNumber)
            {
                wayBillInfo.TrueTrackingNumber = trackingNumber;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(wayBillInfo.TrackingNumber) ||
                    wayBillInfo.TrackingNumber == trackingNumber)
                {
                    wayBillInfo.TrackingNumber = trackingNumber;
                    wayBillInfo.CustomerOrderInfo.TrackingNumber = trackingNumber;
                    if (wayBillInfo.Status == WayBill.StatusEnum.WaitOrder.GetStatusValue())
                    {
                        wayBillInfo.Status = WayBill.StatusEnum.Send.GetStatusValue();
                        wayBillInfo.CustomerOrderInfo.Status = CustomerOrder.StatusEnum.Send.GetStatusValue();
                    }
                }
                else if (!string.IsNullOrWhiteSpace(wayBillInfo.TrackingNumber) &&
                         !string.IsNullOrWhiteSpace(wayBillInfo.RawTrackingNumber))
                {
                    wayBillInfo.TrackingNumber = trackingNumber;
                    wayBillInfo.TransferOrderDate = DateTime.Now;
                    wayBillInfo.CustomerOrderInfo.TrackingNumber = trackingNumber;
                    if (wayBillInfo.Status == WayBill.StatusEnum.WaitOrder.GetStatusValue())
                    {
                        wayBillInfo.Status = WayBill.StatusEnum.Send.GetStatusValue();
                        wayBillInfo.CustomerOrderInfo.Status = CustomerOrder.StatusEnum.Send.GetStatusValue();
                    }
                }
                else if (!string.IsNullOrWhiteSpace(wayBillInfo.TrackingNumber) &&
                         string.IsNullOrWhiteSpace(wayBillInfo.RawTrackingNumber))
                {
                    wayBillInfo.RawTrackingNumber = wayBillInfo.TrackingNumber;
                    wayBillInfo.TrackingNumber = trackingNumber;
                    wayBillInfo.CustomerOrderInfo.TrackingNumber = trackingNumber;
                    wayBillInfo.TransferOrderDate = DateTime.Now;
                    if (wayBillInfo.Status == WayBill.StatusEnum.WaitOrder.GetStatusValue())
                    {
                        wayBillInfo.Status = WayBill.StatusEnum.Send.GetStatusValue();
                        wayBillInfo.CustomerOrderInfo.Status = CustomerOrder.StatusEnum.Send.GetStatusValue();
                    }
                }
            }


            return wayBillInfo;
        }

        /// <summary>
        /// 重写转单逻辑
        /// Add By zhengsong
        /// Time:2014-06-16
        /// </summary>
        /// <param name="wayBillInfo"></param>
        /// <param name="trackingNumber"></param>
        /// <returns></returns>
        public WayBillInfo InStorageWayBillTrackingNumber(WayBillInfo wayBillInfo, string trackingNumber)
        {
            if (wayBillInfo == null)
                return null;
            if (string.IsNullOrWhiteSpace(wayBillInfo.TrackingNumber) ||
                wayBillInfo.TrackingNumber == trackingNumber)
            {
                wayBillInfo.TrackingNumber = trackingNumber;
                wayBillInfo.CustomerOrderInfo.TrackingNumber = trackingNumber;
                if (wayBillInfo.Status == WayBill.StatusEnum.WaitOrder.GetStatusValue())
                {
                    wayBillInfo.Status = WayBill.StatusEnum.Send.GetStatusValue();
                    wayBillInfo.CustomerOrderInfo.Status = CustomerOrder.StatusEnum.Send.GetStatusValue();
                }
            }
            else if (!string.IsNullOrWhiteSpace(wayBillInfo.TrackingNumber) &&
                     !string.IsNullOrWhiteSpace(wayBillInfo.RawTrackingNumber))
            {
                wayBillInfo.TrackingNumber = trackingNumber;
                wayBillInfo.TransferOrderDate = DateTime.Now;
                wayBillInfo.CustomerOrderInfo.TrackingNumber = trackingNumber;
                if (wayBillInfo.Status == WayBill.StatusEnum.WaitOrder.GetStatusValue())
                {
                    wayBillInfo.Status = WayBill.StatusEnum.Send.GetStatusValue();
                    wayBillInfo.CustomerOrderInfo.Status = CustomerOrder.StatusEnum.Send.GetStatusValue();
                }
            }
            else if (!string.IsNullOrWhiteSpace(wayBillInfo.TrackingNumber) &&
                     string.IsNullOrWhiteSpace(wayBillInfo.RawTrackingNumber))
            {
                wayBillInfo.RawTrackingNumber = wayBillInfo.TrackingNumber;
                wayBillInfo.TrackingNumber = trackingNumber;
                wayBillInfo.CustomerOrderInfo.TrackingNumber = trackingNumber;
                wayBillInfo.TransferOrderDate = DateTime.Now;
                if (wayBillInfo.Status == WayBill.StatusEnum.WaitOrder.GetStatusValue())
                {
                    wayBillInfo.Status = WayBill.StatusEnum.Send.GetStatusValue();
                    wayBillInfo.CustomerOrderInfo.Status = CustomerOrder.StatusEnum.Send.GetStatusValue();
                }
            }
            return wayBillInfo;
        }

        //#region 存储内部信息
        ///// <summary>
        ///// Add By zhengsong
        ///// Time:2014-06-09
        ///// </summary>
        ///// <param name="inTackingLogInfo"></param>
        ///// <returns></returns>
        //public bool CreateInTackingLogInfo(InTrackingLogInfo inTackingLogInfo)
        //{
        //    try
        //    {
        //        _inTackingLogInfoRepository.Add(inTackingLogInfo);
        //        _inTackingLogInfoRepository.UnitOfWork.Commit();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Exception(ex);
        //    }
        //    return false;
        //}

        //#endregion

        #region 更改已签收的运单跟订单的状态

        /// <summary>
        /// Add By zhengsong
        /// Time:2014-06-11
        /// </summary>
        /// <param name="trueTrackeingNumber"></param>
        public bool UpdateWayBillAndOrderStust(string trueTrackeingNumber)
        {

            bool result = false;
            var wayBill = _wayBillInfoRepository.First(p => p.TrueTrackingNumber == trueTrackeingNumber || p.TrackingNumber == trueTrackeingNumber);
            if (wayBill != null)
            {
                var order = _customerOrderInfoRepository.Get(wayBill.CustomerOrderID);
                if (order != null)
                {
                    wayBill.Status = WayBill.StatusToValue(WayBill.StatusEnum.Delivered);
                    order.Status = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Delivered);
                    _wayBillInfoRepository.Modify(wayBill);
                    _customerOrderInfoRepository.Modify(order);
                }
                else
                {
                    wayBill.Status = WayBill.StatusToValue(WayBill.StatusEnum.Delivered);
                    _wayBillInfoRepository.Modify(wayBill);
                }
                try
                {
                    _wayBillInfoRepository.UnitOfWork.Commit();
                    _customerOrderInfoRepository.UnitOfWork.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    throw;
                }
            }
            return result;
        }

        #endregion

        #region 分批获取跟踪号

        /// <summary>
        /// Add By zhengsong
        /// Time:2014-06-11
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WayBillInfo> GetWayBillTakeList(List<int> shippingMehotdId, DateTime endTime)
        {
            var stust = WayBill.StatusToValue(WayBill.StatusEnum.Delivered);
            return _wayBillInfoRepository.GetList(p => p.Status != stust && (p.InShippingMethodID != null && shippingMehotdId.Contains(p.InShippingMethodID.Value)) && p.LastUpdatedOn > endTime).OrderBy(p => p.LastUpdatedOn).Take(2000);
        }

        /// <summary>
        /// 获取需要跟踪信息的跟踪号
        /// Add By zhengsong
        /// Time:2014-06-11
        /// </summary>
        /// <returns></returns>
        public string GetTrueTrackingNumber()
        {
            string trackingNumberList = "";

            //需要抓取的运输方式Code
            // string shippingMethodCode = "SPLUS,SPLUSZ,EUDDPG";
            var allShippingMehtod = _freightService.GetShippingMethods("", true);
            List<int> shippingMethods = new List<int>();
            allShippingMehtod.ForEach(p =>
                {
                    switch (p.Code)
                    {
                        case "SPLUS":
                            shippingMethods.Add(p.ShippingMethodId);
                            break;
                        case "SPLUSZ":
                            shippingMethods.Add(p.ShippingMethodId);
                            break;
                        case "EUDDPG":
                            shippingMethods.Add(p.ShippingMethodId);
                            break;
                        case "EUDDP":
                            shippingMethods.Add(p.ShippingMethodId);
                            break;

                    }
                });
            var stust = WayBill.StatusToValue(WayBill.StatusEnum.Delivered);
            var list = _wayBillInfoRepository.GetList(p => p.Status != stust && shippingMethods.Contains(p.InShippingMethodID ?? 0) && p.TrueTrackingNumber != "" && p.TrueTrackingNumber != null);
            list.ForEach(p =>
                {
                    if (!string.IsNullOrWhiteSpace(p.TrueTrackingNumber))
                    {
                        trackingNumberList += p.TrueTrackingNumber + ",";
                    }
                    else if (p.TrackingNumber != null)
                    {
                        trackingNumberList += p.TrackingNumber + ",";
                    }
                });
            return trackingNumberList;
        }

        #endregion

        #region 快递查看货物详细信息

        /// <summary>
        /// Add By zhengsong
        /// Time:2014-06-20
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IPagedList<ExpressWayBillViewExt> GetPagedExpressWayBillList(ExpressWayBillParam param)
        {
            return _wayBillInfoRepository.GetPagedWayBillDetailList(param);
        }

        /// <summary>
        /// Add By zhengsong
        /// Time:2014-06-23
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<ExpressWayBillExt> GetExprotWayBillList(ExpressWayBillParam param)
        {
            return _wayBillInfoRepository.GetExpressWayBillDetailList(param);
        }

        #endregion

        #region 已发货运单

        public IPagedList<ShippingWayBillExt> GetShippingWayBillPagedList(ShippingWayBillParam param)
        {
            return _wayBillInfoRepository.GetShippingWayBillPagedList(param);
        }

        public List<ShippingWayBillExt> GetShippingWayBillList(ShippingWayBillParam param)
        {
            return _wayBillInfoRepository.GetShippingWayBillList(param);
        }

        public string GetAllShippingWayBillList(ShippingWayBillParam param)
        {
            return _wayBillInfoRepository.GetAllShippingWayBillList(param);
        }

        public string GetIsUpdateShippingWayBillList(List<string> wayBillList)
        {
            return _wayBillInfoRepository.GetIsUpdateShippingWayBillList(wayBillList);
        }

        public IList<WayBillListExportModel> GetWayBillListExport(WayBillListExportParam param)
        {
            return _wayBillInfoRepository.GetWayBillListExport(param);
        }

        public IList<ApplicationInfoExportModel> GetApplicationInfoExport(WayBillListExportParam param)
        {
            return _wayBillInfoRepository.GetApplicationInfoExport(param);
        }

        #endregion

        #region 中邮挂号福州运单

        ///// <summary>
        ///// Add By zhengsong
        ///// Time:2014-09-25
        ///// </summary>
        ///// <param name="outShippingMethodId"></param>
        ///// <param name="wayBillNumbers"></param>
        ///// <returns></returns>
        //public List<FZWayBillInfoExt> GetPostalWayBillInfo(List<int> outShippingMethodId,List<string> wayBillNumbers)
        //{
        //    return _wayBillInfoRepository.GetFuZhouWayBillList(outShippingMethodId, wayBillNumbers);
        //}

        /// <summary>
        /// 查出福州邮政的运单
        /// Add By zhengsong
        /// Time:2014-11-05
        /// </summary>
        /// <param name="outShippingMethodId"></param>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public List<FZWayBillInfoExt> GetFZWayBillNumbers(List<int> outShippingMethodId, int numbers)
        {
            return _wayBillInfoRepository.GetFuZhouWayBillNumbers(outShippingMethodId, numbers);
        }

        #endregion

        #region 福州邮政申请记录表

        /// <summary>
        /// 查询是否已经记录
        /// Add By zhengsong
        /// Time"2014-11-04
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <returns></returns>
        public List<FuzhouPostLog> GetFuzhouPostLogByWayBill(List<string> wayBillNumbers)
        {
            return _fuzhouPostLogRepository.GetFiltered(p => wayBillNumbers.Contains(p.WayBillNumber)).ToList();
        }

        /// <summary>
        /// 更申请记录
        ///  Add By zhengsong
        /// Time"2014-11-04
        /// </summary>
        public void AddorUpdateFuzhouPostLog(List<ErrorWayBillExt> errorWayBills)
        {
            List<FuzhouPostLog> addfuzhouPostLogs =new List<FuzhouPostLog>();
            List<string> wayBillNumbers=new List<string>();
            errorWayBills.ForEach(p=> wayBillNumbers.Add(p.WayBillNumber));
            var fuzhouPostLogs = GetFuzhouPostLogByWayBill(wayBillNumbers);
            try
            {
                errorWayBills.ForEach(p =>
                {
                    var fuzhouPostLog = fuzhouPostLogs.FirstOrDefault(f => f.WayBillNumber == p.WayBillNumber);
                    if (fuzhouPostLog != null)
                    {
                        fuzhouPostLog.OutShippingMethodID = p.outShippingMethodID;
                        if (p.result)
                        {
                            fuzhouPostLog.Status = 1;
                            fuzhouPostLog.ErrorNumber = "";
                        }
                        else
                        {
                            fuzhouPostLog.Status = 2;
                            fuzhouPostLog.ErrorNumber = p.ErrorMassge;
                        }
                        fuzhouPostLog.LastUpdateOn = DateTime.Now;
                        _fuzhouPostLogRepository.Modify(fuzhouPostLog);
                    }
                    else
                    {
                        FuzhouPostLog addFuzhouPostLog = new FuzhouPostLog();
                        addFuzhouPostLog.WayBillNumber = p.WayBillNumber;
                        addFuzhouPostLog.OutShippingMethodID = p.outShippingMethodID;
                        if (p.result)
                        {
                            addFuzhouPostLog.Status = 1;
                        }
                        else
                        {
                            addFuzhouPostLog.Status = 2;
                            addFuzhouPostLog.ErrorNumber = p.ErrorMassge;
                        }
                        addFuzhouPostLog.CreateOn = DateTime.Now;
                        addFuzhouPostLog.LastUpdateOn = addFuzhouPostLog.CreateOn;
                        addfuzhouPostLogs.Add(addFuzhouPostLog);
                    }
                });
                _fuzhouPostLogRepository.bulkInsertFuzhouPostLog(addfuzhouPostLogs);
                _fuzhouPostLogRepository.UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            
        }

        #endregion

        #region DHL EUB 预报
        /// <summary>
        /// Add By zhengsong
        /// Time:2015-01-29
        /// </summary>
        /// <param name="inShippingMethodId"></param>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public List<WayBillInfo> GetDHLandEUBWayBillInfos(List<int> inShippingMethodId, int numbers)
        {
            return _wayBillInfoRepository.GetDHLandEUBWayBillInfos(inShippingMethodId, numbers);
        }

        /// <summary>
        /// hold 预报失败运单
        /// Add By zhengsong
        /// </summary>
        /// <param name="errorWayBills"></param>
        public void UpdateWayBillInfo(Dictionary<string, string> errorWayBills)
        {
            if (errorWayBills.Count > 0)
            {
                foreach (var row in errorWayBills)
                {
                    var wayBillInfo = _wayBillInfoRepository.Get(row.Key);
                    wayBillInfo.IsHold = true;
                    wayBillInfo.CustomerOrderInfo.IsHold = true;
                    wayBillInfo.LastUpdatedBy = "System";
                    wayBillInfo.CustomerOrderInfo.LastUpdatedBy = "System";
                    AbnormalWayBillLog abnormalWayBillLog= new AbnormalWayBillLog();
                    abnormalWayBillLog.OperateType = 7;
                    abnormalWayBillLog.AbnormalStatus = 1;
                    abnormalWayBillLog.AbnormalDescription = row.Value;
                    abnormalWayBillLog.CreatedOn = DateTime.Now;
                    abnormalWayBillLog.CreatedBy = "System";
                    abnormalWayBillLog.LastUpdatedOn = DateTime.Now;
                    abnormalWayBillLog.LastUpdatedBy = "System";
                    wayBillInfo.AbnormalWayBillLog = abnormalWayBillLog;
                    _wayBillInfoRepository.Modify(wayBillInfo);
                }
                _wayBillInfoRepository.UnitOfWork.Commit();
            }
        }

        #endregion

        /// <summary>
        /// 查询页面
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IPagedList<WaybillInfoUpdateExt> GetWaybillInfoUpdatePagedList(WaybillInfoUpdateParam param)
        {
            return _wayBillInfoRepository.GetWaybillInfoUpdatePagedList(param);
        }

        public List<string> IsWayBillnumberInFeeInfo(List<string> waybillNumber)
        {
            return _wayBillInfoRepository.IsWayBillnumberInFeeInfo(waybillNumber);
        }




        /// <summary>
        /// 当运单的状态为“已收货”则系统判断运单是否已出收货费用
        /// 1.未出费用：则直接删除原有运单，产生一个新的与原运单一样的状态为“已提交”的运单，重新入仓
        /// 2.已出费用：则系统自动退回原运单，退回费用，产生一个新的与原单一样的状态为“已提交”的运单，重新入库
        /// </summary>
        /// <param name="waybillNumberList"></param>
        public List<WayBillInfo> OperateWaybillByFee(List<string> waybillNumberList, int? shippingMethodId, string shippingMethodName, string trackNumber=null)
        {
            List<WayBillInfo> listWayBillInfo = new List<WayBillInfo>();

            //已出费用
            List<string> haveFeeWaybillnumber = new List<string>();
            IsWayBillnumberInFeeInfo(waybillNumberList).ForEach(a =>
            {
                if (!haveFeeWaybillnumber.Contains(a))
                {
                    haveFeeWaybillnumber.Add(a);
                }
            });

            //未出费用
            List<string> noHaveFeeWaybillnumber = waybillNumberList.Except(haveFeeWaybillnumber).ToList();


            if (haveFeeWaybillnumber.Count > 0)
            {
                //已出费用逻辑
                listWayBillInfo = CopyWayBillInfoList(haveFeeWaybillnumber, 1, shippingMethodId, shippingMethodName, trackNumber);
            }

            if (noHaveFeeWaybillnumber.Count > 0)
            {
                //未出费用逻辑
                listWayBillInfo = CopyWayBillInfoList(noHaveFeeWaybillnumber, null, shippingMethodId, shippingMethodName, trackNumber);
            }

            return listWayBillInfo;
        }


        //传入的单号状态改为"删除"，并新增一条"已提交"的新运单
        public List<WayBillInfo> CopyWayBillInfoList(List<string> waybillNumberList, int? flag, int? shippingMethodId, string shippingMethodName = null, string trackNumber=null)
        {
            var deleteStatus = (int)WayBill.StatusEnum.Delete;
            var submittedStatus = (int)WayBill.StatusEnum.Submitted;

            //返回生成新运单
            List<WayBillInfo> listWayBillInfo = new List<WayBillInfo>();


	        if (waybillNumberList != null && waybillNumberList.Count > 0)
	        {

			        foreach (var a in waybillNumberList)
			        {
				        var getWayBillInfo = new WayBillInfo();
						var shippingMethodModel = new ShippingMethodModel();
				        SenderInfo senderInfo = null;
				        ShippingInfo shippingInfo = null;
				        var customerOrderInfo = new CustomerOrderInfo();
				        WayBillInfo newWayBillInfo = new WayBillInfo(); //新运单
				        var newCustomerOrderInfo = new CustomerOrderInfo();//新订单 

						//原来的运单信息
						getWayBillInfo = _wayBillInfoRepository.Single(b => b.WayBillNumber == a);

						if (shippingMethodId.HasValue)
				        {
							shippingMethodModel = _freightService.GetShippingMethod(shippingMethodId.Value);
				        }

				    

						//避免执行一下步操作
				        if (trackNumber == null)
				        {
							if (string.IsNullOrEmpty(getWayBillInfo.TrackingNumber) && shippingMethodId.HasValue && shippingMethodModel.IsSysTrackNumber)
							{
								var trackNumbers = _trackingNumberService.TrackNumberAssignStandard(shippingMethodId.Value, 1,
									getWayBillInfo.CountryCode);
								if (trackNumbers == null)
								{
									throw new ArgumentException("[{0}]运输方式无可分配的跟踪号！".FormatWith(shippingMethodName));
								}
							}
				        }


				        /*****修改原来运单(改状态)*********/

						//已出费用, 退回原运单，退回费用
						if (flag != null)
						{
							DeleteWayBillInfo(a);
						}
					

				        if (flag == null) //未出费用
				        {
					        getWayBillInfo.Status = deleteStatus;
				        }

				        getWayBillInfo.IsHold = false; //解除拦截
				        getWayBillInfo.AbnormalWayBillLog.AbnormalStatus =
					        (int) WayBill.AbnormalStatusEnum.OK.GetAbnormalStatusValue();
				        getWayBillInfo.CreatedBy = _workContext.User.UserUame;
				        getWayBillInfo.LastUpdatedBy = _workContext.User.UserUame;
				        getWayBillInfo.LastUpdatedOn = System.DateTime.Now;
				     



				        /*****构建运单*********/

				        #region add 发货信息

				        if (getWayBillInfo.SenderInfo != null)
				        {
					        senderInfo = new SenderInfo();
					        senderInfo.CountryCode = getWayBillInfo.SenderInfo.CountryCode;
					        senderInfo.SenderFirstName = getWayBillInfo.SenderInfo.SenderFirstName;
					        senderInfo.SenderLastName = getWayBillInfo.SenderInfo.SenderLastName;
					        senderInfo.SenderCompany = getWayBillInfo.SenderInfo.SenderCompany;
					        senderInfo.SenderAddress = getWayBillInfo.SenderInfo.SenderAddress;
					        senderInfo.SenderCity = getWayBillInfo.SenderInfo.SenderCity;
					        senderInfo.SenderState = getWayBillInfo.SenderInfo.SenderState;
					        senderInfo.SenderZip = getWayBillInfo.SenderInfo.SenderZip;
					        senderInfo.SenderPhone = getWayBillInfo.SenderInfo.SenderPhone;
					        //_senderInfoRepository.Add(senderInfo);

				        }


				        #endregion

				        #region add 收货信息

				        if (getWayBillInfo.ShippingInfo != null)
				        {
					        shippingInfo = new ShippingInfo();

					        shippingInfo.CountryCode = getWayBillInfo.ShippingInfo.CountryCode;
					        shippingInfo.ShippingFirstName = getWayBillInfo.ShippingInfo.ShippingFirstName;
					        shippingInfo.ShippingLastName = getWayBillInfo.ShippingInfo.ShippingLastName;
					        shippingInfo.ShippingCompany = getWayBillInfo.ShippingInfo.ShippingCompany;
					        shippingInfo.ShippingAddress = getWayBillInfo.ShippingInfo.ShippingAddress;
					        shippingInfo.ShippingCity = getWayBillInfo.ShippingInfo.ShippingCity;
					        shippingInfo.ShippingState = getWayBillInfo.ShippingInfo.ShippingState;
					        shippingInfo.ShippingZip = getWayBillInfo.ShippingInfo.ShippingZip;
					        shippingInfo.ShippingPhone = getWayBillInfo.ShippingInfo.ShippingPhone;
					        shippingInfo.ShippingTaxId = getWayBillInfo.ShippingInfo.ShippingTaxId;
					        shippingInfo.ShippingAddress1 = getWayBillInfo.ShippingInfo.ShippingAddress1;
					        shippingInfo.ShippingAddress2 = getWayBillInfo.ShippingInfo.ShippingAddress2;
					        //_shippingInfoRepository.Add(shippingInfo);


				        }

				        #endregion


				        /*****修改原来的客户订单*********/
				        #region update 客户订单(原来的)

				        customerOrderInfo =_customerOrderInfoRepository.Single(c => c.CustomerOrderID == getWayBillInfo.CustomerOrderID);
				        if (customerOrderInfo != null)
				        {
							//退回状态
					        if (flag != null)
					        {
						        customerOrderInfo.Status = (int) CustomerOrder.StatusEnum.Return;
					        }
					        else
					        {
								customerOrderInfo.Status = (int)CustomerOrder.StatusEnum.Delete;
					        }
					        customerOrderInfo.IsPrinted = false; //改为未打印
					        customerOrderInfo.IsHold = false; //解除拦截
				        }

				        #endregion




						/*****生成新的客户订单*********/
						#region
						var getOldCustomerOrderInfo = _customerOrderInfoRepository.Single(c => c.CustomerOrderID == getWayBillInfo.CustomerOrderID);
						
						newCustomerOrderInfo.Status = (int)CustomerOrder.StatusEnum.Submitted;//已提交
						newCustomerOrderInfo.IsPrinted = false; //改为未打印
						newCustomerOrderInfo.IsHold = false; //解除拦截
						newCustomerOrderInfo.ShippingInfo = shippingInfo;//新生成
						newCustomerOrderInfo.SenderInfo = senderInfo;
						newCustomerOrderInfo.ShippingMethodId = shippingMethodId ?? getWayBillInfo.InShippingMethodID; //修改运输方式
						newCustomerOrderInfo.ShippingMethodName = shippingMethodName ?? getWayBillInfo.InShippingMethodName; //修改运输方式

						newCustomerOrderInfo.CustomerOrderNumber = getOldCustomerOrderInfo.CustomerOrderNumber;
						newCustomerOrderInfo.AppLicationType = getOldCustomerOrderInfo.AppLicationType;
				        newCustomerOrderInfo.CustomerCode = getOldCustomerOrderInfo.CustomerCode;
				        newCustomerOrderInfo.EnableTariffPrepay = getOldCustomerOrderInfo.EnableTariffPrepay;
						newCustomerOrderInfo.GoodsTypeID = getOldCustomerOrderInfo.GoodsTypeID;
				        newCustomerOrderInfo.GoodsTypeInfo = getOldCustomerOrderInfo.GoodsTypeInfo;
				        newCustomerOrderInfo.Height = getOldCustomerOrderInfo.Height;
				        newCustomerOrderInfo.InsureAmount = getOldCustomerOrderInfo.InsureAmount;
				        newCustomerOrderInfo.InsuredCalculation = getOldCustomerOrderInfo.InsuredCalculation;
						newCustomerOrderInfo.InsuredID = getOldCustomerOrderInfo.InsuredID;
				        newCustomerOrderInfo.IsBattery = getOldCustomerOrderInfo.IsBattery;
				        newCustomerOrderInfo.IsInsured = getOldCustomerOrderInfo.IsInsured;
				        newCustomerOrderInfo.IsReturn = getOldCustomerOrderInfo.IsReturn;
						newCustomerOrderInfo.Length = getOldCustomerOrderInfo.Length;
				        newCustomerOrderInfo.PackageNumber = getOldCustomerOrderInfo.PackageNumber;
				        newCustomerOrderInfo.Remark = getOldCustomerOrderInfo.Remark;
				        newCustomerOrderInfo.SensitiveTypeID = getOldCustomerOrderInfo.SensitiveTypeID;
				        newCustomerOrderInfo.SenderInfoID = getOldCustomerOrderInfo.SenderInfoID;
				        newCustomerOrderInfo.SensitiveTypeInfo = getOldCustomerOrderInfo.SensitiveTypeInfo;
				        newCustomerOrderInfo.TrackingNumber = getOldCustomerOrderInfo.TrackingNumber;
				        newCustomerOrderInfo.Weight = getOldCustomerOrderInfo.Weight;
				        newCustomerOrderInfo.Width = getOldCustomerOrderInfo.Width;
						newCustomerOrderInfo.CreatedBy = _workContext.User.UserUame;
						newCustomerOrderInfo.CreatedOn = System.DateTime.Now;
						newCustomerOrderInfo.LastUpdatedBy = _workContext.User.UserUame;
						newCustomerOrderInfo.LastUpdatedOn = System.DateTime.Now;
						_customerOrderInfoRepository.Add(newCustomerOrderInfo);
						#endregion




						/*生成客户订单状态*/
						#region
				        var customerOrderStatus = new CustomerOrderStatus();
				        customerOrderStatus.CustomerOrderInfo = newCustomerOrderInfo;
				        customerOrderStatus.CreatedOn = System.DateTime.Now;
				        customerOrderStatus.Status = (int)CustomerOrder.StatusEnum.Submitted;
						_customerOrderStatusRepository.Add(customerOrderStatus);
						#endregion





						/*****生成新运单*********/

				        #region 生成的新运单

				        newWayBillInfo.WayBillNumber = SequenceNumberService.GetWayBillNumber(getWayBillInfo.CustomerCode);

				        newWayBillInfo.ShippingInfo = shippingInfo;
				        newWayBillInfo.SenderInfo = senderInfo;

				        newWayBillInfo.Status = submittedStatus; //已提交
				        newWayBillInfo.InShippingMethodID = shippingMethodId ?? getWayBillInfo.InShippingMethodID; //修改运输方式
				        newWayBillInfo.InShippingMethodName = shippingMethodName ?? getWayBillInfo.InShippingMethodName; //修改运输方式
				        newWayBillInfo.IsHold = false;

				        //newWayBillInfo.CustomerOrderInfo = customerOrderInfo;
				        newWayBillInfo.CreatedBy = _workContext.User.UserUame;
				        newWayBillInfo.CreatedOn = System.DateTime.Now;
				        newWayBillInfo.LastUpdatedBy = _workContext.User.UserUame;
				        newWayBillInfo.LastUpdatedOn = System.DateTime.Now;

				        newWayBillInfo.CustomerCode = getWayBillInfo.CustomerCode;
				        newWayBillInfo.TrackingNumber = getWayBillInfo.TrackingNumber;


						//如果地址改发调用
				        if (!string.IsNullOrWhiteSpace(trackNumber))
				        {
					        newWayBillInfo.TrackingNumber = trackNumber;
					        newCustomerOrderInfo.TrackingNumber = trackNumber;
				        }
				        else
				        {
							//分配跟踪号
							if (string.IsNullOrEmpty(getWayBillInfo.TrackingNumber) && shippingMethodId.HasValue && shippingMethodModel.IsSysTrackNumber)
							{

								var trackNumbers = _trackingNumberService.TrackNumberAssignStandard(shippingMethodId.Value, 1,
																									getWayBillInfo.CountryCode);
								if (trackNumbers != null && trackNumbers.Any())
								{
									newWayBillInfo.TrackingNumber = trackNumbers[0];
									newCustomerOrderInfo.TrackingNumber = trackNumbers[0];
								}
								else
								{
									throw new ArgumentException("[{0}]运输方式无可分配的跟踪号！".FormatWith(shippingMethodName));
								}

							}
				        }

				   

				        newWayBillInfo.Weight = getWayBillInfo.Weight;
				        newWayBillInfo.Length = getWayBillInfo.Length;
				        newWayBillInfo.Width = getWayBillInfo.Width;
				        newWayBillInfo.Height = getWayBillInfo.Height;
				        newWayBillInfo.GoodsTypeID = getWayBillInfo.GoodsTypeID;
				        newWayBillInfo.IsReturn = getWayBillInfo.IsReturn;
				        newWayBillInfo.IsBattery = getWayBillInfo.IsBattery;
				        newWayBillInfo.CountryCode = getWayBillInfo.CountryCode;
				        newWayBillInfo.InsuredID = getWayBillInfo.InsuredID;
				        newWayBillInfo.CustomerOrderNumber = getWayBillInfo.CustomerOrderNumber;
				        newWayBillInfo.TrueTrackingNumber = getWayBillInfo.TrueTrackingNumber;
				        newWayBillInfo.EnableTariffPrepay = getWayBillInfo.EnableTariffPrepay;
						newCustomerOrderInfo.WayBillInfos.Add(newWayBillInfo);
				        //_wayBillInfoRepository.Add(newWayBillInfo);

				        #endregion

				        #region add 申报信息

				        List<ApplicationInfo> _applicationInfos = _applicationInfoRepository.GetList(b => b.WayBillNumber == a);

				        if (_applicationInfos != null && _applicationInfos.Count != 0)
				        {
					        //customerOrderInfo.ApplicationInfos.Clear();
					        foreach (var info in _applicationInfos)
					        {
						        ApplicationInfo applicationInfo = new ApplicationInfo();
						        applicationInfo.WayBillInfo = newWayBillInfo;
								applicationInfo.CustomerOrderInfo = newCustomerOrderInfo;
						        applicationInfo.ApplicationName = info.ApplicationName;
						        applicationInfo.Qty = info.Qty;
						        applicationInfo.UnitWeight = info.UnitWeight;
						        applicationInfo.UnitPrice = info.UnitPrice;
						        applicationInfo.Total = info.Total;
						        applicationInfo.HSCode = info.HSCode;
						        applicationInfo.IsDelete = info.IsDelete;
						        applicationInfo.PickingName = info.PickingName;
						        applicationInfo.Remark = info.Remark;
						        applicationInfo.ProductUrl = info.ProductUrl;
						        applicationInfo.CreatedBy = _workContext.User.UserUame;
						        applicationInfo.CreatedOn = DateTime.Now;
						        applicationInfo.LastUpdatedBy = _workContext.User.UserUame;
						        applicationInfo.LastUpdatedOn = DateTime.Now;
						        // customerOrderInfo.ApplicationInfos.Add(applicationInfo);

						        _applicationInfoRepository.Add(applicationInfo);

					        }

				        }

				        #endregion

				        //添加新运单
				        listWayBillInfo.Add(newWayBillInfo);



				        /*****记录删除运单日志(WayBillEventLogs)****/

				        #region  记录删除运单日志

				        WayBillChangeLog wayBillChangeLog = new WayBillChangeLog();
				        wayBillChangeLog.WayBillNumber = newWayBillInfo.WayBillNumber; //新单号
				        wayBillChangeLog.OriginalWayBillNumber = a; //原单号
				        wayBillChangeLog.CustomerOrderID = newWayBillInfo.CustomerOrderID.Value;
				        wayBillChangeLog.CustomerOrderNumber = newWayBillInfo.CustomerOrderNumber;
				        wayBillChangeLog.ChangeType = 1; //变更类型 1-客户要求,2-内部要求
				        wayBillChangeLog.ChangeReason = "客户要求";
				        wayBillChangeLog.CreatedBy = _workContext.User.UserUame;
				        wayBillChangeLog.CreatedOn = DateTime.Now;
				        wayBillChangeLog.LastUpdatedBy = _workContext.User.UserUame;
				        wayBillChangeLog.LastUpdatedOn = DateTime.Now;

				        _wayBillChangeLogRepository.Add(wayBillChangeLog);

				        #endregion

			        }
					using (var transaction = new TransactionScope(TransactionScopeOption.Required))
					{
						//_senderInfoRepository.UnitOfWork.Commit();
						//_shippingInfoRepository.UnitOfWork.Commit();
						_wayBillInfoRepository.UnitOfWork.Commit();
						_customerOrderStatusRepository.UnitOfWork.Commit();
						_customerOrderInfoRepository.UnitOfWork.Commit();

						_applicationInfoRepository.UnitOfWork.Commit();
						_wayBillChangeLogRepository.UnitOfWork.Commit();

						transaction.Complete();
					}
	        }
	        return listWayBillInfo;
        }

        #region 入仓重量对比异常运单 yungchu

        public IPagedList<InStorageWeightAbnormalExt> GetInStorageWeightAbnormalPagedList(InStorageWeightAbnormalParam param)
        {
            return _wayBillInfoRepository.GetInStorageWeightAbnormaPagedList(param);
        }

        public List<ExportInStorageWeightAbnormalExt> GetExportInStorageWeightAbnormalExt(InStorageWeightAbnormalParam param)
        {
            return _wayBillInfoRepository.GetExportInStorageWeightAbnormal(param);
        }


        public void AddInStorageWeightAbnormal(WeightAbnormalLog model)
        {
            _weightAbnormalLogRepository.Add(model);
            _weightAbnormalLogRepository.UnitOfWork.Commit();
        }

        public void DeleteInStorageWeightAbnormal(List<string> waybillNumbers)
        {
            for (int i = 0; i < waybillNumbers.Count; i++)
            {
                string waybillnumber = waybillNumbers[i];
                WeightAbnormalLog getItem = _weightAbnormalLogRepository.First(a => a.WayBillNumber == waybillnumber);

                if (getItem != null)
                {
                    _weightAbnormalLogRepository.Remove(getItem);
                    _weightAbnormalLogRepository.UnitOfWork.Commit();
                }
            }
        }


        public void UpdateInStorageWeightAbnormal(string waybillNumber, decimal weight)
        {
            WeightAbnormalLog weightAbnormalLog = _weightAbnormalLogRepository.First(a => a.WayBillNumber == waybillNumber);
            weightAbnormalLog.Weight = weight;
            _weightAbnormalLogRepository.Modify(weightAbnormalLog);
            _weightAbnormalLogRepository.UnitOfWork.Commit();
        }

        public bool IsExistInStorageWeightAbnormal(string waybillNumber)
        {
            return _weightAbnormalLogRepository.Exists(a => a.WayBillNumber == waybillNumber);

            //WeightAbnormalLog weightAbnormalLog = _weightAbnormalLogRepository.First(a => a.WayBillNumber.Contains(waybillNumber));

            //if (weightAbnormalLog != null)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }




        #endregion









        public SelectTrackingNumberExt GetTrackingNumberDetails(TrackingNumberParam param)
        {
            SelectTrackingNumberExt model = new SelectTrackingNumberExt();
            model.TrackingNumberIds = _trackingNumberDetailInfoRepository.GetTrackingNumberDetails(param).TrackingNumberIds;
            if (model.TrackingNumberIds.Count < 1)
            {
                return model;
            }

            List<ShippingMethodModel> shippingMethodModels = new List<ShippingMethodModel>();
            var freightService = EngineContext.Current.Resolve<IFreightService>();
            shippingMethodModels = freightService.GetShippingMethods(null, true);
            model.TrackingNumberIds.ForEach(p =>
                {
                    int used = 0;
                    int NotUsed = 0;
                    string shippingMethodName = "";
                    TrackingNumberInfo trInfo = new TrackingNumberInfo();

                    //GetList(t=>xxx).Count ,改为Count(t=>xx),by daniel 2014-10-23
                    used = (int)_trackingNumberDetailInfoRepository.Count(z => z.TrackingNumberID == p.TrackingNumberId && z.Status == 2);
                    NotUsed = (int)_trackingNumberDetailInfoRepository.Count(z => z.TrackingNumberID == p.TrackingNumberId && z.Status == 1);

                    trInfo = _trackingNumberInfoRepository.Get(p.TrackingNumberId);
                    if (shippingMethodModels.Find(x => x.ShippingMethodId == trInfo.ShippingMethodID) != null)
                    {
                        shippingMethodName = shippingMethodModels.Find(x => x.ShippingMethodId == trInfo.ShippingMethodID).FullName;
                    }
                    model.TrackingNumberDetaileds.Add(new TrackingNumber
                        {
                            TrackingNumberId = trInfo.TrackingNumberID,
                            ShippingMethodName = shippingMethodName,
                            Status = trInfo.Status,
                            Used = used,
                            NotUsed = NotUsed,
                            CreateNo = trInfo.CreatedNo,
                            ApplicableCountry = trInfo.ApplianceCountry.Substring(0, trInfo.ApplianceCountry.Length - 1)
                        });
                });
            return model;
        }

        public List<TrackingNumberExt> GetTrackingNumberExtList(TrackingNumberParam param)
        {
            return _trackingNumberInfoRepository.GetTrackingNumberExtList(param);
        }

        public List<InsuredCalculation> GetInsuredCalculationListAll()
        {
            return _insuredCalculationRepository.GetList(p => p.Status == 1).ToList();
        }

        public List<SensitiveTypeInfo> GetSensitiveTypeInfoListAll()
        {
            return _sensitiveTypeInfoRepository.GetList(p => p.IsDelete == false).ToList();
        }

        public ShippingInfo GetshippingInfoById(int? id)
        {
            if (id == 0)
            {
                return null;
            }
            return _shippingInfoRepository.Get(id);
        }

        public SenderInfo GetSenderInfoById(int? id)
        {
            if (id == 0)
            {
                return null;
            }
            return _senderInfoRepository.Get(id);
        }

        public InsuredCalculation GetInsuredCalculationById(int? id)
        {
            if (id == 0)
            {
                return null;
            }
            return _insuredCalculationRepository.Get(id);
        }

        public SensitiveTypeInfo GetSensitiveTypeInfoById(int? id)
        {
            if (id == 0)
            {
                return null;
            }
            return _sensitiveTypeInfoRepository.Get(id);
        }

        public CustomerOrderInfo GetCustomerOrderInfoById(int? id)
        {
            if (id == 0)
            {
                return null;
            }
            return _customerOrderInfoRepository.Get(id);
        }

        public IEnumerable<ApplicationInfo> GetApplicationInfoByWayBillNumber(string name)
        {
            return _applicationInfoRepository.GetList(p => p.WayBillNumber == name);
        }

        /// <summary>
        /// 从TrackingNumberDetail表中更新wayBillNumber表和CustomerOrderInfo表的跟踪号
        /// </summary>
        /// <param name="customerOrderId"></param>
        /// <param name="wayBillNumber"></param>
        /// <param name="shippingMethodId"></param>
        /// <param name="isSystemGenerate">是否需要系统自动生成跟踪号</param>
        /// <returns>返回跟踪号</returns>
        public string UpdateTrackingNumberByTrackingNumberDetail(int customerOrderId, string wayBillNumber, int shippingMethodId, string countryCode, bool isSystemGenerate = true)
        {
            string trackingNumber = null;
            if (!isSystemGenerate) return trackingNumber;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                var trackingNumberDetail = _trackingNumberService.TrackNumberAssignStandard(shippingMethodId, 1, countryCode);
                if (null != trackingNumberDetail && trackingNumberDetail.Any()) // 更新trackingNumberDetail表的status改为已使用、将WayBillNumber字段的填充
                {
                    trackingNumber = trackingNumberDetail[0];
                    //trackingNumberDetail.Status = (short)TrackingNumberDetailInfo.StatusEnum.Used;
                    //_trackingNumberDetailInfoRepository.Modify(trackingNumberDetail);
                    //_trackingNumberDetailInfoRepository.UnitOfWork.Commit();
                    //更新运单表
                    var wayBillInfo = _wayBillInfoRepository.Get(wayBillNumber);
                    wayBillInfo.TrackingNumber = trackingNumber;
                    wayBillInfo.LastUpdatedOn = DateTime.Now;
                    wayBillInfo.LastUpdatedBy = _workContext.User.UserUame;
                    _wayBillInfoRepository.Modify(wayBillInfo);
                    _wayBillInfoRepository.UnitOfWork.Commit();
                    //更新CustomerOrderInfo表
                    var customerOrderInfo = _customerOrderInfoRepository.Get(customerOrderId);
                    customerOrderInfo.TrackingNumber = trackingNumber;
                    customerOrderInfo.LastUpdatedOn = DateTime.Now;
                    customerOrderInfo.LastUpdatedBy = _workContext.User.UserUame;
                    _customerOrderInfoRepository.Modify(customerOrderInfo);
                    _customerOrderInfoRepository.UnitOfWork.Commit();
                }
                transaction.Complete();
            }
            return trackingNumber;

        }


        public void UpdateWayBillChangeLog(WayBillInfo wayBillInfo, string originalWayBillNumber, int changeType, string changeReason)
        {
            var wayBillChangeLog = _wayBillChangeLogRepository.First(p => p.WayBillNumber == wayBillInfo.WayBillNumber);

            if (wayBillChangeLog != null)
            {
                wayBillChangeLog.ChangeReason = changeReason;
                wayBillChangeLog.ChangeType = changeType;
            }
            else
            {
                wayBillChangeLog = new WayBillChangeLog()
                    {
                        WayBillNumber = wayBillInfo.WayBillNumber,
                        ChangeReason = changeReason,
                        ChangeType = changeType,
                        CreatedBy = _workContext.User.UserUame,
                        CreatedOn = DateTime.Now,
                        LastUpdatedBy = _workContext.User.UserUame,
                        LastUpdatedOn = DateTime.Now,
                        OriginalWayBillNumber = originalWayBillNumber,
                        CustomerOrderID = wayBillInfo.CustomerOrderID.Value,
                        CustomerOrderNumber = wayBillInfo.CustomerOrderNumber,
                    };

                _wayBillChangeLogRepository.Add(wayBillChangeLog);
            }
            _wayBillChangeLogRepository.UnitOfWork.Commit();
        }

        public IPagedList<NoForecastAbnormalExt> GetNoForecastAbnormalExtPagedList(NoForecastAbnormalParam param)
        {
            return _noForecastAbnormalRepository.GetNoForecastAbnormalExtPagedList(param);
        }

        public void UpdateNoForecastAbnormal(NoForecastAbnormal noForecastAbnormalExt)
        {
            var noForecastAbnormal = _noForecastAbnormalRepository.GetFiltered(p => p.Number == noForecastAbnormalExt.Number).FirstOrDefault();

            if (noForecastAbnormal == null)
            {
                _noForecastAbnormalRepository.Add(noForecastAbnormalExt);
                _noForecastAbnormalRepository.UnitOfWork.Commit();
            }
        }

        public void DeleteNoForecastAbnormal(int[] noForecastAbnormalIds)
        {
            _noForecastAbnormalRepository.Remove(p => noForecastAbnormalIds.Contains(p.NoForecastAbnormalId));
            _noForecastAbnormalRepository.UnitOfWork.Commit();
        }

        public void ReturnNoForecastAbnormal(int[] noForecastAbnormalIds)
        {
            var canReturnnoForecastAbnormalIds = _noForecastAbnormalRepository.GetNoForecastList(noForecastAbnormalIds).Select(p => p.NoForecastAbnormalId);

            _noForecastAbnormalRepository.GetList(p => canReturnnoForecastAbnormalIds.Contains(p.NoForecastAbnormalId)).ToList()
                                         .ForEach(
                                             p =>
                                             {
                                                 p.IsReturn = true;
                                                 p.LastUpdatedBy = _workContext.User.UserUame;
                                                 p.LastUpdatedOn = DateTime.Now;
                                                 _noForecastAbnormalRepository.Modify(p);
                                             });

            _noForecastAbnormalRepository.UnitOfWork.Commit();
        }

        public List<WaybillSummary> GetWaybillSummaryList(WaybillSummaryParam param)
        {
            return _wayBillInfoRepository.GetWaybillSummaryList(param);
        }

        public List<string> GetIsEixtCustomerOrderNumber(List<string> customerOrderNumbers)
        {
            return _wayBillInfoImportTempRepository.GetIsEixtCustomerOrderNumber(customerOrderNumbers);
        }
        public void BulkInsert<T>(string tableName, IList<T> list)
        {
            _wayBillInfoRepository.BulkInsert(tableName,list);
        }
    }

}
