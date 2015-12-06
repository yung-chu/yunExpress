using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Transactions;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LMS.Services.FreightServices;
using LMS.Services.OrderServices;
using LMS.Services.SequenceNumber;
using LighTake.Infrastructure.Common;
using LMS.Data.Entity.ExtModel;
using LighTake.Infrastructure.Common.Logging;
using Newtonsoft.Json;
using WayBillInfoExt = LMS.Data.Entity.ExtModel.WayBillInfoExt;
using LighTake.Infrastructure.CommonQueue;
using LighTake.Infrastructure.Common.Caching;
using LMS.Services.CustomerServices;
using System.Collections.Concurrent;


namespace LMS.Services.InStorageServices
{
    public class InStorageService : IInStorageService
    {
        private const string WAYBILL_INSTORAGE_QUEUENAME = "WayBillInStorageQueue";
        private const string RABBITMQ_CONFIGKEY = "lms";
        private static int In_Storage_Sync_On_Error_Max_Times = 2; // 默认重试2次

        private IGoodsTypeInfoRepository _goodsTypeInfoRepository;
        private IInStorageInfoRepository _inStorageInfoRepository;
        private IWayBillInfoRepository _wayBillInfoRepository;
        private ICustomerAmountRecordRepository _customerAmountRecordRepository;
        private ICustomerOrderStatusRepository _customerOrderStatusRepository;
        private IOrderService _orderService;
        private IWorkContext _workContext;
        private IWaybillPackageDetailRepository _waybillPackageDetailRepository;
        private IWayBillPrintLogRepository _wayBillPrintLogRepository;
        private IWayBillEventLogRepository _wayBillEventLogRepository;
        private ICustomerRepository _customerRepository;
        private IReceivingExpensRepository _receivingExpensRepository;
        private IReceivingBillRepository _receivingBillRepository;
        private IInStorageWeightDeviationRepository _inStorageWeightDeviationRepository;
        private ITaskRepository _taskRepository;
        private IInTrackingLogInfoRepository _inTackingLogInfoRepository;
        private IWayBillBusinessDateInfoRepository _wayBillBusinessDateInfoRepository;
        private ICustomerService _customerService;
        private IFreightService _freightService;
        private ILithuaniaInfoRepository _lithuaniaInfoRepository;


        public InStorageService(IGoodsTypeInfoRepository goodsTypeInfoRepository,
                                IWorkContext workContext,
                                IInStorageInfoRepository inStorageInfoRepository,
                                IWayBillInfoRepository wayBillInfoRepository,
                                ICustomerAmountRecordRepository customerAmountRecordRepository,
                                ICustomerOrderStatusRepository customerOrderStatusRepository,
                                IOrderService orderService,
                                IWayBillEventLogRepository wayBillEventLogRepository,
                                IWaybillPackageDetailRepository waybillPackageDetailRepository,
                                IWayBillPrintLogRepository wayBillPrintLogRepository,
                                ICustomerRepository customerRepository,
                                IReceivingExpensRepository receivingExpensRepository,
                                IReceivingBillRepository receivingBillRepository,
                                IInStorageWeightDeviationRepository inStorageWeightDeviationRepository,
                                IInTrackingLogInfoRepository inTackingLogInfoRepository,
                                ITaskRepository taskRepository,                              
                                ILithuaniaInfoRepository lithuaniaInfoRepository,                             
				IWayBillBusinessDateInfoRepository wayBillBusinessDateInfoRepository
            , IFreightService freightService
            , ICustomerService customerService
            )
        {
            GetInStorageSyncOnErrorMaxTimes();

            _goodsTypeInfoRepository = goodsTypeInfoRepository;
            _workContext = workContext;
            _inStorageInfoRepository = inStorageInfoRepository;
            _wayBillInfoRepository = wayBillInfoRepository;
            _customerAmountRecordRepository = customerAmountRecordRepository;
            _customerOrderStatusRepository = customerOrderStatusRepository;
            _orderService = orderService;
            _waybillPackageDetailRepository = waybillPackageDetailRepository;
            _wayBillPrintLogRepository = wayBillPrintLogRepository;
            _wayBillEventLogRepository = wayBillEventLogRepository;
            _customerRepository = customerRepository;
            _receivingExpensRepository = receivingExpensRepository;
            _receivingBillRepository = receivingBillRepository;
            _inStorageWeightDeviationRepository = inStorageWeightDeviationRepository;
            _taskRepository = taskRepository;
            _inTackingLogInfoRepository = inTackingLogInfoRepository;
            _lithuaniaInfoRepository = lithuaniaInfoRepository;
            _wayBillBusinessDateInfoRepository = wayBillBusinessDateInfoRepository;
            _customerService = customerService;
            _freightService = freightService;
        }

        private void GetInStorageSyncOnErrorMaxTimes()
        {
            int times = 0;
            try
            {
                if (int.TryParse(System.Configuration.ConfigurationManager.AppSettings["InStorageSyncOnErrorMaxTimes"], out times))
                {
                    In_Storage_Sync_On_Error_Max_Times = times;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex.InnerException == null ? ex : ex.InnerException);
            }
        }

        public List<GoodsTypeInfo> GetGoodsTypeList(bool? isDelete)
        {
            Expression<Func<GoodsTypeInfo, bool>> filter = p => true;
            filter = filter.AndIf(p => p.IsDelete == isDelete.Value, isDelete.HasValue);
            return _goodsTypeInfoRepository.GetList(filter).ToList();
        }

        public WayBillInfo GetWayBillInfo(string WayBillNumber)
        {
            Check.Argument.IsNullOrWhiteSpace(WayBillNumber, "运单号");
            return _wayBillInfoRepository.First(p => p.WayBillNumber == WayBillNumber) ??
                   _wayBillInfoRepository.First(p => p.TrackingNumber == WayBillNumber) ??
                   _wayBillInfoRepository.First(p => p.CustomerOrderNumber == WayBillNumber) ??
                   _wayBillInfoRepository.First(p => p.TrueTrackingNumber == WayBillNumber);

        }

        public WayBillInfo GetWayBillInfo(string numberStr, string customerCode)
        {
            return _wayBillInfoRepository.GetWayBillInfo(numberStr, customerCode);
            //Check.Argument.IsNullOrWhiteSpace(numberStr, "单号");
            //Check.Argument.IsNullOrWhiteSpace(customerCode, "客户编码");
            //return
            //    _wayBillInfoRepository.First(
            //        p =>
            //        p.CustomerCode == customerCode.ToUpperInvariant() &&
            //        (p.WayBillNumber == numberStr || p.TrackingNumber == numberStr ||
            //         p.CustomerOrderInfo.CustomerOrderNumber == numberStr) && p.Status != 6 && p.Status != 7);

        }

        /// <summary>
        /// 正在入仓的运单
        /// </summary>
        //public static Dictionary<string, string> InStorageWorkItemDic = new Dictionary<string, string>();

        /// <summary>
        /// 正在入仓的运单锁
        /// </summary>
        private static readonly object _cacheLock = new object();

        #region 判断运是否单入仓中

        /// <summary>
        /// 判断是否在正在入仓的运单中
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <returns></returns>
        public bool IsInStorageWorkDoing(string[] wayBillNumbers)
        {
            string cacheKeyTemplate = "/LMS/WayBill/InStorageSaveSync/{0}";
            StringBuilder sb = new StringBuilder();
            foreach (var s in wayBillNumbers)
            {
                sb.AppendFormat("{0},", s);
            }
            Log.Info("IsInStorageWorkDoing ,param wayBillNumbers :" + sb.ToString().TrimEnd(','));

            lock (_cacheLock)
            {
                foreach (var s in wayBillNumbers)
                {
                    if (string.IsNullOrWhiteSpace(s)) continue;

                    if (DistributedCache.Exists(string.Format(cacheKeyTemplate, s.ToLower().Trim())))
                    {
                        throw new BusinessLogicException(string.Format("运单[{0}]正在入仓中.", s));
                        //return true; //只要有一个存在，整批不允许入
                    }
                }
                //插入标志
                foreach (var s in wayBillNumbers)
                {
                    if (string.IsNullOrWhiteSpace(s)) continue;
                    DistributedCache.Add(
                        string.Format(cacheKeyTemplate, s.ToLower().Trim()),
                        DateTime.Now.ToString(),
                        DateTime.Now.AddDays(3));
                }
            }

            return false;

            //lock (LockInStorageWorkItemDic)
            //{
            //    bool exist = wayBillNumbers.Any(wayBillNumber => InStorageWorkItemDic.ContainsKey(wayBillNumber));
            //    if (!exist)
            //    {
            //        //不存在，则插入到字典
            //        wayBillNumbers.ToList().ForEach(p => InStorageWorkItemDic.Add(p, ""));
            //    }
            //    return exist;
            //}
        }

        /// <summary>
        /// 入仓操作完成，从正在入仓的运单列表中删除
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <returns></returns>
        public void RemoveInStorageWorkDoing(string[] wayBillNumbers)
        {
            string cacheKeyTemplate = "/LMS/WayBill/InStorageSaveSync/{0}";

            lock (_cacheLock)
            {
                foreach (var s in wayBillNumbers)
                {
                    if (string.IsNullOrWhiteSpace(s)) continue;
                    DistributedCache.Remove(string.Format(cacheKeyTemplate, s.ToLower().Trim()));
                }
            }
            //lock (LockInStorageWorkItemDic)
            //{
            //    wayBillNumbers.ToList().ForEach(p => InStorageWorkItemDic.Remove(p));
            //}
        }

        #endregion

        /// <summary>
        /// 创建入库
        /// </summary>
        /// <param name="createInStorageExt"></param>
        public void CreateInStorage(CreateInStorageExt createInStorageExt)
        {

            #region 记录请求日志

            //string requestString = JsonConvert.SerializeObject(createInStorageExt);
            //Log.Info(requestString);
            Log.Info(string.Format("CreateInStorage:一共{0}运单", createInStorageExt.WayBillInfos.Count));

            #endregion

            Check.Argument.IsNotNull(createInStorageExt, "createInStorageExt");
            Check.Argument.IsNotNull(createInStorageExt.InStorage, "createInStorageExt.InStorage");
            Check.Argument.IsNotNull(createInStorageExt.WayBillInfos, "createInStorageExt.WayBillInfos");

            Process process = new Process();
            DateTime inStorageCreateOn = DateTime.Now;
            List<string> wayBillNumbers = new List<string>();

            createInStorageExt.WayBillInfos.Each(p => wayBillNumbers.Add(p.WayBillNumber));

            #region 判断运单是否正在入仓

            if (IsInStorageWorkDoing(wayBillNumbers.ToArray()))
            {
                throw new Exception("你提交的运单中包含正在进行入仓操作的运单");
            }

            #endregion

            var wayBillList = _wayBillInfoRepository.GetList(p => wayBillNumbers.Contains(p.WayBillNumber));

            try
            {
                var customerCode = createInStorageExt.WayBillInfos[0].CustomerCode;
                var countryCode = wayBillList[0].CountryCode;
                var customer = _customerRepository.GetFiltered(c => c.CustomerCode == customerCode).First();
                bool calcFeeOnInStorage = customer.PaymentTypeID == 3 || customer.PaymentTypeID == 4;
                decimal totalWeight = 0;
                createInStorageExt.WayBillInfos.Each(p =>
                    {
                        #region 修改运单资料信息和订单状态

                        var oldstatus = WayBill.StatusToValue(WayBill.StatusEnum.Submitted);
                        var wayBillInfo = wayBillList.FirstOrDefault(w => w.WayBillNumber == p.WayBillNumber && w.CustomerCode.ToUpperInvariant() == p.CustomerCode.ToUpperInvariant() && w.Status == oldstatus);
                        if (wayBillInfo == null)
                        {
                            throw new ArgumentException("该运单号\"{0}\"不存在，或则是当前状态不是已提交！".FormatWith(p.WayBillNumber));
                        }

                        if (p.SettleWeight < p.Weight)
                        {
                            throw new BusinessLogicException(string.Format("运单结算重量[{0}]小于实际重量[{1}]", p.SettleWeight, p.Weight));
                        }

                        if (p.ShippingMethodId != wayBillInfo.InShippingMethodID)
                        {
                            throw new BusinessLogicException(string.Format("运单[{2}]实际入仓方式[{0}]与应入仓方式不相符[{1}]", p.ShippingMethodId, wayBillInfo.InShippingMethodID, p.WayBillNumber));
                        }

                        //所有客户都需要记录结算重量
                        //Update By zhengsong
                        //if (calcFeeOnInStorage)
                        //{
                            wayBillInfo.SettleWeight = p.SettleWeight;
                        //}
                        totalWeight += p.SettleWeight;
                        //wayBillInfo.TrackingNumber = !string.IsNullOrWhiteSpace(p.TrackingNumber) ? p.TrackingNumber : string.Empty;

                        wayBillInfo.Weight = p.Weight;
                        wayBillInfo.Width = p.Width;
                        wayBillInfo.Length = p.Length;
                        wayBillInfo.Height = p.Height;
                        wayBillInfo.GoodsTypeID = p.GoodsTypeID;
                        wayBillInfo.InStorageID = createInStorageExt.InStorage.InStorageID;
                        p.CustomerOrderID = wayBillInfo.CustomerOrderID;

                        if (wayBillInfo.CustomerOrderID.HasValue)
                        {
                            //修改订单状态
                            //wayBillInfo.CustomerOrderInfo.TrackingNumber = !string.IsNullOrWhiteSpace(p.TrackingNumber) ? p.TrackingNumber : string.Empty;

                            wayBillInfo.CustomerOrderInfo.GoodsTypeID = p.GoodsTypeID;
                            wayBillInfo.CustomerOrderInfo.Status =
                                CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Have);
                            wayBillInfo.CustomerOrderInfo.LastUpdatedBy = _workContext.User.UserUame;
                            wayBillInfo.CustomerOrderInfo.LastUpdatedOn = DateTime.Now;
                            //插入订单状态记录
                            _customerOrderStatusRepository.Add(new CustomerOrderStatus
                                {
                                    CustomerOrderID = wayBillInfo.CustomerOrderID.Value,
                                    CreatedOn = DateTime.Now,
                                    Status = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Have),
                                    Remark = "已收货"
                                });
                        }
                        wayBillInfo.LastUpdatedBy = _workContext.User.UserUame;
                        wayBillInfo.LastUpdatedOn = DateTime.Now;
                        wayBillInfo.InStorageCreatedOn = inStorageCreateOn;
                        wayBillInfo = _orderService.InStorageWayBillTrackingNumber(wayBillInfo, p.TrackingNumber);

                        wayBillInfo.Status = WayBill.StatusToValue(WayBill.StatusEnum.InStoraging);
                        wayBillInfo.CustomerOrderInfo.Status = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Have);
                        _wayBillInfoRepository.Modify(wayBillInfo);

                        #endregion

                    });

                //所有客户都需要记录结算重量
                //Update By zhengsong
                //if (!calcFeeOnInStorage)
                //{
                    createInStorageExt.InStorage.TotalWeight = totalWeight;
                //}

                createInStorageExt.InStorage.PhysicalTotalWeight = createInStorageExt.WayBillInfos.Sum(p => p.Weight);

                //生成入仓资料
                var inStorage = new InStorageInfo();
                inStorage = createInStorageExt.InStorage;
                inStorage.ReceivingClerk = inStorage.CreatedBy = inStorage.LastUpdatedBy = _workContext.User.UserUame;
                inStorage.CreatedOn = inStorage.LastUpdatedOn = inStorageCreateOn;
                inStorage.Status = 1;

                //结算方式 by daniel 2014-12-12
                inStorage.PaymentTypeID = customer.PaymentTypeID;

                _inStorageInfoRepository.Add(inStorage);

                //生成收货单号
                var receivingBillID = SequenceNumberService.GetSequenceNumber(PrefixCode.ReceivingBillID);
                var receivingBill = new ReceivingBill()
                    {
                        ReceivingBillID = receivingBillID,
                        CustomerCode = customerCode,
                        CustomerName = customer.Name, //  _customerRepository.GetFiltered(c => c.CustomerCode == customerCode).First().Name,
                        ReceivingBillDate = DateTime.Now,
                        ReceivingBillAuditor = "System-Auto",
                        BillStartTime = DateTime.Now,
                        BillEndTime = DateTime.Now,
                        ShippingMethodID = createInStorageExt.WayBillInfos[0].ShippingMethodId, // createInStorageExt.WayBillInfos[0].ShippingMethodId,
                        CountryCode = countryCode,
                        Search = 100, //100只需要生成账单excel
                        Status = 1,
                    };

                _receivingBillRepository.Add(receivingBill);


                #region Task准备

                string userName = _workContext.User.UserUame;

                ConcurrentBag<LMS.Data.Entity.Task> tasks = new ConcurrentBag<LMS.Data.Entity.Task>();
                System.Threading.Tasks.Parallel.ForEach(createInStorageExt.WayBillInfos,p=>
               // createInStorageExt.WayBillInfos.Each(p =>
                    {
                        if (p == null) return;
                        var aynscModel = new InStorageAsyncModel()
                            {
                                WayBillInfoExt = p,
                                Process = process,
                                InStorageCreateOn = inStorageCreateOn,
                                InStorageID = inStorage.InStorageID,
                                ReceivingBillID = receivingBillID,
                                UserUame = userName,
                                BusinessDate = createInStorageExt.BusinessDate ?? DateTime.Now
                            };
                        var t = new LMS.Data.Entity.Task()
                            {
                                Body = SerializeUtil.ToJson(aynscModel),
                                CreateOn = DateTime.Now,
                                Times = 0,
                                TaskType = 1,
                                TaskKey = p.WayBillNumber,
                                Status = 0
                            };
                        tasks.Add(t);
                    });

                Log.Info(string.Format("CreateInStorage:一共{0}task", tasks.Count));


                #endregion


                ////业务日期 add by yungchu
                //var listWayBillBusinessDate = new List<WayBillBusinessDateInfo>();
                //createInStorageExt.WayBillInfos.Each(p => listWayBillBusinessDate.Add(new WayBillBusinessDateInfo
                //{
                //    ReceivingDate = createInStorageExt.BusinessDate ?? DateTime.Now,
                //    WayBillNumber = p.WayBillNumber,
                //    CreatedOn = System.DateTime.Now,
                //    CreatedBy = _workContext.User.UserUame,
                //    LastUpdatedOn = System.DateTime.Now,
                //    LastUpdatedBy = _workContext.User.UserUame
                //}));


                ConcurrentBag<string> queueMessages = new ConcurrentBag<string>();
                using (var transaction = new TransactionScope())
                {

                    //_wayBillInfoRepository.BulkInsert("WayBillBusinessDateInfos", listWayBillBusinessDate);

                    _inStorageInfoRepository.UnitOfWork.Commit();
                    _wayBillInfoRepository.UnitOfWork.Commit();
                    //_inTackingLogInfoRepository.UnitOfWork.Commit();
                    //_receivingExpensRepository.UnitOfWork.Commit();
                    _receivingBillRepository.UnitOfWork.Commit();
                    _taskRepository.UnitOfWork.Commit();

                    //save & return TaskID
                    _taskRepository.Save(tasks);
                    Log.Info(string.Format("CreateInStorage:一共{0}task(保存之后)", tasks.Count));

                    //messages
                    System.Threading.Tasks.Parallel.ForEach(tasks,t=>
                    //tasks.Each(t =>
                        {
                            string json = SerializeUtil.ToJson(t);
                            if (string.IsNullOrWhiteSpace(json))
                                throw new BusinessLogicException("序列化异常");
                            queueMessages.Add(json);
                        });

                    Log.Info(string.Format("CreateInStorage:一共{0}messages", queueMessages.Count));

                    //enqueue
                    if (!QueueHelper.Enqueue(WAYBILL_INSTORAGE_QUEUENAME, queueMessages.ToArray(), RABBITMQ_CONFIGKEY))
                    {
                        throw new BusinessLogicException("提交失败,原因入队列失败,请稍后从事!");
                    }

                    //commit
                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                throw;
            }
            finally
            {
                //内存中的单号需要移除,否则下次无法提交
                RemoveInStorageWorkDoing(wayBillNumbers.ToArray());
            }

        }

        private List<Customer> GetGetCustomerListFromCache()
        {
            const string key = "List_Customer";

            object inCache = Cache.Get(key);

            if (inCache != null)
            {
                var listCustomer = inCache as List<Customer>;

                if (listCustomer != null) return listCustomer;
            }

            var listCustomerNewest = _customerService.GetCustomerList("", false);

            if (listCustomerNewest != null)
            {
                Cache.Add(key, listCustomerNewest, 60);
            }

            return listCustomerNewest;
        }

        /// <summary>
        /// 处理入仓中的数据
        /// </summary>
        public void FixInStoraging()
        {
            try
            {
                var wayBillList = _wayBillInfoRepository.GetList(t => t.Status == 350);
                Log.Info(string.Format("一共{0}运单需要修复", wayBillList.Count));

                List<LMS.Data.Entity.Task> tasks = new List<LMS.Data.Entity.Task>();
                wayBillList.Each(p =>
                {
                    if (p == null) return;

                    var storage = _inStorageInfoRepository.Get(p.InStorageID);

                    Guid customerId = Guid.Empty;
                    Customer customer = GetGetCustomerListFromCache().Find(c => c.CustomerCode == p.CustomerCode);
                    if (customer != null)
                        customerId = customer.CustomerID;
                    var result = _freightService.GetCustomerShippingPrice(new CustomerInfoPackageModel()
                    {
                        CountryCode = p.CountryCode,
                        CustomerTypeId = customer.CustomerTypeID.Value,
                        Height = p.Height ?? 0,
                        Length = p.Length ?? 0,
                        ShippingMethodId = p.InShippingMethodID.Value,
                        Weight = p.Weight.Value,
                        Width = p.Width ?? 0,
                        ShippingTypeId = p.GoodsTypeID.Value,
                        CustomerId = customerId,
                        EnableTariffPrepay = p.EnableTariffPrepay,
                    });


                    var aynscModel = new InStorageAsyncModel()
                    {
                        WayBillInfoExt = new Data.Entity.WayBillInfoExt()
                        {
                            CustomerCode = p.CustomerCode,
                            CustomerOrderID = p.CustomerOrderID,

                            Freight = result.ShippingFee, //运费
                            FuelCharge = result.FuelFee, //燃油费
                            Register = result.RegistrationFee,//挂号费
                            Surcharge = result.Value - (result.ShippingFee + result.FuelFee + result.RegistrationFee + result.TariffPrepayFee), //附加费
                            TariffPrepay = result.TariffPrepayFee,//关税预付服务费
                            SettleWeight = result.Weight, //结算重量

                            //CustomerType = p.
                            Height = p.Height.Value,
                            Length = p.Length.Value,
                            Width = p.Width.Value,
                            Weight = p.Weight.Value,
                            ShippingMethodId = p.ShippingInfoID.Value,
                            WayBillNumber = p.WayBillNumber,
                            TrackingNumber = p.TrackingNumber
                        },
                        Process = null,
                        InStorageCreateOn = storage.CreatedOn,// inStorageCreateOn,
                        InStorageID = p.InStorageID,
                        //ReceivingBillID = p.re,
                        UserUame = p.CreatedBy// _workContext.User.UserUame
                    };
                    var t = new LMS.Data.Entity.Task()
                    {
                        Body = SerializeUtil.ToJson(aynscModel),
                        CreateOn = DateTime.Now,
                        Times = 0,
                        TaskType = 1,
                        TaskKey = p.WayBillNumber,
                        Status = 0
                    };
                    tasks.Add(t);
                });

                Log.Info(string.Format("一共{0}task", tasks.Count));

                using (var transaction = new TransactionScope())
                {
                    //save & return TaskID
                    _taskRepository.Save(tasks);
                    Log.Info(string.Format("一共{0}task(保存之后)", tasks.Count));

                    List<string> queueMessages = new List<string>();
                    //messages
                    //System.Threading.Tasks.Parallel.ForEach(
                    tasks.Each(t =>
                    {
                        string json = SerializeUtil.ToJson(t);
                        if (string.IsNullOrWhiteSpace(json))
                            throw new BusinessLogicException("序列化异常");
                        queueMessages.Add(json);
                    });

                    Log.Info(string.Format("一共{0}messages", queueMessages.Count));

                    //enqueue
                    if (!QueueHelper.Enqueue(WAYBILL_INSTORAGE_QUEUENAME, queueMessages.ToArray(), RABBITMQ_CONFIGKEY))
                    {
                        throw new BusinessLogicException("提交失败,原因入队列失败,请稍后从事!");
                    }

                    //commit
                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// 创建入库(异步处理部分)
        /// </summary>
        /// <param name="task"></param>
        public void CreateInStorageAsync(Task task)
        {
            InStorageAsyncModel aynscModel = new InStorageAsyncModel();
            try
            {
                //处理中...
                _taskRepository.Update(task.TaskID, 1, string.Empty, false);

                aynscModel = SerializeUtil.FromJson<InStorageAsyncModel>(task.Body);

                var wayBill = _wayBillInfoRepository.Get(aynscModel.WayBillInfoExt.WayBillNumber);
                //_wayBillInfoRepository.GetList(p => aynscModel.WayBillInfoExt.WayBillNumber == aynscModel.WayBillInfoExt.WayBillNumber).FirstOrDefault();

                var customerCode = wayBill.CustomerCode;
                var countryCode = wayBill.CountryCode;
                var customer = _customerRepository.GetFiltered(c => c.CustomerCode == customerCode).FirstOrDefault();
                bool calcFeeOnInStorage = customer.PaymentTypeID == 3 || customer.PaymentTypeID == 4;

                //如果结算方式为预付或者现金
                if (calcFeeOnInStorage)
                {
                    #region 生成运费扣费记录

                    if (aynscModel.WayBillInfoExt.Freight > 0)
                    {
                        var freight = new CustomerAmountRecordParam
                            {
                                CreatedBy = aynscModel.UserUame,
                                Amount = aynscModel.WayBillInfoExt.Freight,
                                CustomerCode = aynscModel.WayBillInfoExt.CustomerCode,
                                WayBillNumber = aynscModel.WayBillInfoExt.WayBillNumber,
                                MoneyChangeTypeId = 2,
                                FeeTypeId = 3,
                                Remark = "运单号：{0}扣运费".FormatWith(aynscModel.WayBillInfoExt.WayBillNumber),
                                TransactionNo = aynscModel.InStorageID
                            };
                        //1-成功，2-以存在交易号，0-失败
                        var result = _customerAmountRecordRepository.CreateCustomerAmountRecord(freight);
                        if (result != 1)
                        {
                            throw new ArgumentException("该运单号\"{0}\"生成运费扣费记录失败！".FormatWith(aynscModel.WayBillInfoExt.WayBillNumber));
                        }
                        ;
                    }

                    #endregion

                    #region 生成挂号费扣费记录

                    if (aynscModel.WayBillInfoExt.Register > 0)
                    {
                        var register = new CustomerAmountRecordParam
                            {
                                CreatedBy = aynscModel.UserUame,
                                Amount = aynscModel.WayBillInfoExt.Register,
                                CustomerCode = aynscModel.WayBillInfoExt.CustomerCode,
                                WayBillNumber = aynscModel.WayBillInfoExt.WayBillNumber,
                                MoneyChangeTypeId = 2,
                                FeeTypeId = 4,
                                Remark = "运单号：{0}扣挂号费".FormatWith(aynscModel.WayBillInfoExt.WayBillNumber),
                                TransactionNo = aynscModel.InStorageID
                            };
                        var result = _customerAmountRecordRepository.CreateCustomerAmountRecord(register);
                        if (result != 1)
                        {
                            throw new ArgumentException("该运单号\"{0}\"生成挂号费扣费记录失败！".FormatWith(aynscModel.WayBillInfoExt.WayBillNumber));
                        }
                        ;
                    }

                    #endregion

                    #region 生成燃油费扣费记录

                    if (aynscModel.WayBillInfoExt.FuelCharge > 0)
                    {
                        var fuelCharge = new CustomerAmountRecordParam
                            {
                                CreatedBy = aynscModel.UserUame,
                                Amount = aynscModel.WayBillInfoExt.FuelCharge,
                                CustomerCode = aynscModel.WayBillInfoExt.CustomerCode,
                                WayBillNumber = aynscModel.WayBillInfoExt.WayBillNumber,
                                MoneyChangeTypeId = 2,
                                FeeTypeId = 5,
                                Remark = "运单号：{0}扣燃油费".FormatWith(aynscModel.WayBillInfoExt.WayBillNumber),
                                TransactionNo = aynscModel.InStorageID
                            };
                        var result = _customerAmountRecordRepository.CreateCustomerAmountRecord(fuelCharge);
                        if (result != 1)
                        {
                            throw new ArgumentException("该运单号\"{0}\"生成燃油费扣费记录失败！".FormatWith(aynscModel.WayBillInfoExt.WayBillNumber));
                        }
                        ;
                    }

                    #endregion

                    #region 生成附加费扣费记录

                    if (aynscModel.WayBillInfoExt.Surcharge > 0)
                    {
                        var surcharge = new CustomerAmountRecordParam
                            {
                                CreatedBy = aynscModel.UserUame,
                                Amount = aynscModel.WayBillInfoExt.Surcharge,
                                CustomerCode = aynscModel.WayBillInfoExt.CustomerCode,
                                WayBillNumber = aynscModel.WayBillInfoExt.WayBillNumber,
                                MoneyChangeTypeId = 2,
                                FeeTypeId = 2,
                                Remark = "运单号：{0}扣附加费".FormatWith(aynscModel.WayBillInfoExt.WayBillNumber),
                                TransactionNo = aynscModel.InStorageID
                            };
                        var result = _customerAmountRecordRepository.CreateCustomerAmountRecord(surcharge);
                        if (result != 1)
                        {
                            throw new ArgumentException("该运单号\"{0}\"生成附加费扣费记录失败！".FormatWith(aynscModel.WayBillInfoExt.WayBillNumber));
                        }
                        ;
                    }

                    #endregion

                    #region 生成关税预付服务费扣费记录

                    if (aynscModel.WayBillInfoExt.TariffPrepay > 0)
                    {
                        var tariffPrepay = new CustomerAmountRecordParam
                            {
                                CreatedBy = aynscModel.UserUame,
                                Amount = aynscModel.WayBillInfoExt.TariffPrepay,
                                CustomerCode = aynscModel.WayBillInfoExt.CustomerCode,
                                WayBillNumber = aynscModel.WayBillInfoExt.WayBillNumber,
                                MoneyChangeTypeId = 2,
                                FeeTypeId = 6,
                                Remark = "运单号：{0}扣关税预付服务费".FormatWith(aynscModel.WayBillInfoExt.WayBillNumber),
                                TransactionNo = aynscModel.InStorageID
                            };
                        var result = _customerAmountRecordRepository.CreateCustomerAmountRecord(tariffPrepay);
                        if (result != 1)
                        {
                            throw new ArgumentException("该运单号\"{0}\"生成附加费扣费记录失败！".FormatWith(aynscModel.WayBillInfoExt.WayBillNumber));
                        }
                        ;
                    }

                    #endregion

                }
                else
                {
                    #region 生成收货费用记录

                    //var wayBill = _wayBillInfoRepository.Get(aynscModel.WayBillInfoExt.WayBillNumber);

                    _receivingExpensRepository.Add(new ReceivingExpens()
                        {
                            WayBillNumber = aynscModel.WayBillInfoExt.WayBillNumber,
                            CustomerOrderNumber = wayBill.CustomerOrderNumber,
                            Status = (int)(Financial.ReceivingExpenseStatusEnum.UnAudited),
                            CreatedBy = aynscModel.UserUame,
                            CreatedOn = DateTime.Now,
                            LastUpdatedBy = aynscModel.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            AcceptanceDate = aynscModel.InStorageCreateOn,
                        });

                    #endregion
                }

                #region 录入内部信息

                //Add By zhengsong
                //Time:2014-06-09
                //var inTrackLogInfo = new InTrackingLogInfo
                //{
                //    WayBillNumber = aynscModel.WayBillInfoExt.WayBillNumber,
                //    ProcessDate = DateTime.Now,
                //    ProcessContent = aynscModel.Process.ProcessInScann,
                //    ProcessLocation = aynscModel.Process.ProcessAdderss,
                //    CreatedOn = DateTime.Now,
                //    CreatedBy = aynscModel.UserUame,
                //    LastUpdatedBy = aynscModel.UserUame,
                //    LastUpdatedOn = DateTime.Now
                //};
                //_inTackingLogInfoRepository.Add(inTrackLogInfo);


                var wayBillEventLog = new WayBillEventLog()
                    {
                        WayBillNumber = aynscModel.WayBillInfoExt.WayBillNumber,
                        EventCode = (int)WayBillEvent.EventCodeEnum.InStorage,
                        Description = WayBillEvent.GetEventCodeDescription((int)WayBillEvent.EventCodeEnum.InStorage),
                        EventDate = DateTime.Now,
                        LastUpdatedOn = DateTime.Now,
                        Operator = aynscModel.UserUame,
                    };
                _wayBillEventLogRepository.Add(wayBillEventLog);

                #endregion

                #region 生成入仓包裹明细

                var inPackage = new WaybillPackageDetail();
                inPackage.WayBillNumber = aynscModel.WayBillInfoExt.WayBillNumber;
                inPackage.Weight = aynscModel.WayBillInfoExt.Weight;
                inPackage.AddWeight = 0;
                inPackage.SettleWeight = aynscModel.WayBillInfoExt.SettleWeight;
                inPackage.Length = aynscModel.WayBillInfoExt.Length;
                inPackage.Width = aynscModel.WayBillInfoExt.Width;
                inPackage.Height = aynscModel.WayBillInfoExt.Height;
                inPackage.LengthiFee = 0;
                inPackage.WeightFee = 0;
                inPackage.CreatedOn = DateTime.Now;
                inPackage.CreatedBy = aynscModel.UserUame;
                inPackage.LastUpdatedOn = DateTime.Now;
                inPackage.LastUpdatedBy = aynscModel.UserUame;
                _waybillPackageDetailRepository.Add(inPackage);

                #endregion

                //如果结算方式为预付或者现金,生成收货费用信息
                if (calcFeeOnInStorage)
                {
                    #region 生成收货费用表记录

                    var listReceivingExpens = new List<ReceivingExpens>();

                    var receivingBillID = aynscModel.ReceivingBillID; // SequenceNumberService.GetSequenceNumber(PrefixCode.ReceivingBillID);
                    //var p = aynscModel.WayBillInfoExt;


                    // wayBillList = wayBillList;
                    //wayBillList.FirstOrDefault(
                    //w => w.WayBillNumber == aynscModel.WayBillInfoExt.WayBillNumber &&
                    //w.CustomerCode.ToUpperInvariant() == aynscModel.WayBillInfoExt.CustomerCode.ToUpperInvariant());

                    #region 生成收货费用表明细记录

                    List<ReceivingExpenseInfo> listReceivingExpenseInfos = new List<ReceivingExpenseInfo>();

                    listReceivingExpenseInfos.Add(new ReceivingExpenseInfo()
                        {
                            ReceivingBillID = receivingBillID,
                            Amount = aynscModel.WayBillInfoExt.Freight,
                            CreatedBy = aynscModel.UserUame,
                            CreatedOn = DateTime.Now,
                            FeeTypeID = (int)CustomerOrder.FeeTypeEnum.Freight,
                            LastUpdatedBy = aynscModel.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            OperationType = 1,
                        });


                    listReceivingExpenseInfos.Add(new ReceivingExpenseInfo()
                        {
                            ReceivingBillID = receivingBillID,
                            Amount = aynscModel.WayBillInfoExt.FuelCharge,
                            CreatedBy = aynscModel.UserUame,
                            CreatedOn = DateTime.Now,
                            FeeTypeID = (int)CustomerOrder.FeeTypeEnum.FuelCharge,
                            LastUpdatedBy = aynscModel.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            OperationType = 1
                        });


                    listReceivingExpenseInfos.Add(new ReceivingExpenseInfo()
                        {
                            ReceivingBillID = receivingBillID,
                            Amount = aynscModel.WayBillInfoExt.Register,
                            CreatedBy = aynscModel.UserUame,
                            CreatedOn = DateTime.Now,
                            FeeTypeID = (int)CustomerOrder.FeeTypeEnum.Register,
                            LastUpdatedBy = aynscModel.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            OperationType = 1
                        });

                    listReceivingExpenseInfos.Add(new ReceivingExpenseInfo()
                        {
                            ReceivingBillID = receivingBillID,
                            Amount = aynscModel.WayBillInfoExt.Surcharge,
                            CreatedBy = aynscModel.UserUame,
                            CreatedOn = DateTime.Now,
                            FeeTypeID = (int)CustomerOrder.FeeTypeEnum.Surcharge,
                            LastUpdatedBy = aynscModel.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            OperationType = 1
                        });

                    listReceivingExpenseInfos.Add(new ReceivingExpenseInfo()
                        {
                            ReceivingBillID = receivingBillID,
                            Amount = aynscModel.WayBillInfoExt.TariffPrepay,
                            CreatedBy = aynscModel.UserUame,
                            CreatedOn = DateTime.Now,
                            FeeTypeID = (int)CustomerOrder.FeeTypeEnum.TariffPrepayFee,
                            LastUpdatedBy = aynscModel.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            OperationType = 1
                        });

                    #endregion

                    var receivingExpens = new ReceivingExpens()
                        {
                            WayBillNumber = wayBill.WayBillNumber,
                            CustomerOrderNumber = wayBill.CustomerOrderNumber,
                            Status = (int)Financial.ReceivingExpenseStatusEnum.OutBilled,
                            Auditor = "System-Auto",
                            AuditorDate = DateTime.Now,
                            CreatedOn = DateTime.Now,
                            CreatedBy = aynscModel.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            LastUpdatedBy = aynscModel.UserUame,
                            IsNoGet = true,
                            AcceptanceDate = aynscModel.InStorageCreateOn,
                        };

                    listReceivingExpenseInfos.ForEach(receivingExpens.ReceivingExpenseInfos.Add);

                    listReceivingExpens.Add(receivingExpens);

                    listReceivingExpens.ForEach(_receivingExpensRepository.Add);

                    #endregion
                }

                using (var transaction = new TransactionScope())
                {
                    wayBill.Status = WayBill.StatusToValue(WayBill.StatusEnum.Have); //状态改为已收货
                    _wayBillInfoRepository.Modify(wayBill);

                    //业务日期
                    AddWayBillBusinessDateInfos(new WayBillBusinessDateInfo()
                    {
                        CreatedBy = aynscModel.UserUame,
                        CreatedOn = DateTime.Now,
                        LastUpdatedBy = aynscModel.UserUame,
                        LastUpdatedOn = DateTime.Now,
                        ReceivingDate = aynscModel.BusinessDate,
                        WayBillNumber = wayBill.WayBillNumber
                    });

                    //_wayBillInfoRepository.UnitOfWork.Commit();
                    //_inStorageInfoRepository.UnitOfWork.Commit();
                    _inTackingLogInfoRepository.UnitOfWork.Commit();
                    _receivingExpensRepository.UnitOfWork.Commit();
                    _wayBillEventLogRepository.UnitOfWork.Commit();


                    _taskRepository.Delete(task.TaskID); //处理成功。
                    _taskRepository.UnitOfWork.Commit();

                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                var x = ex.InnerException == null ? ex : ex.InnerException;

                Log.Error(task.TaskKey + "异步处理失败." + x.ToString());

                task.Status = -1;
                task.Times = task.Times + 1;
                task.Error = x.ToString();

                if (task.Times < In_Storage_Sync_On_Error_Max_Times) //如果没有达到最大失败次数,继续入队列
                {
                    Log.Info("失败未达到最大次数,将再次入队列," + task.TaskKey);

                    task.Status = 0;
                    _taskRepository.Update(task.TaskID, 0, x.ToString(), true); //继续处理

                    QueueHelper.Enqueue(WAYBILL_INSTORAGE_QUEUENAME
                                        , new string[] { SerializeUtil.ToJson(task) }
                                        , RABBITMQ_CONFIGKEY);
                }
                else
                {
                    Log.Info("失败达到最大次数," + task.TaskKey);
                    _taskRepository.Update(task.TaskID, -1, x.ToString(), true);
                }
            }
            //finally
            //{
            //    #region 入仓操作完成，从正在入仓的运单列表中删除

            //    RemoveInStorageWorkDoing(new string[] { task.TaskKey });

            //    #endregion
            //}
        }

        public IPagedList<TaskModel> GetTaskList(int taskType, int status, string taskKey, int pageIndex, int pageSize = 50)
        {
            var tasks = _taskRepository.List(taskType, status, taskKey, pageIndex, pageSize);
            var models = new PagedList<TaskModel>();

            System.Threading.Tasks.Parallel.ForEach(tasks.InnerList, t =>
                {
                    models.InnerList.Add(new TaskModel()
                        {
                            CreateOn = t.CreateOn,
                            Body = SerializeUtil.FromJson<InStorageAsyncModel>(t.Body),
                            Error = t.Error,
                            Status = t.Status,
                            TaskID = t.TaskID,
                            TaskKey = t.TaskKey,
                            TaskType = t.TaskType,
                            Times = t.Times
                        });
                });

            models.PageIndex = tasks.PageIndex;
            models.PageSize = tasks.PageSize;
            models.TotalCount = tasks.TotalCount;
            models.TotalPages = tasks.TotalPages;

            return models;
        }

        public bool Retry(long[] ids)
        {
            if (_taskRepository.Retry(ids)) //修改数据库状态
            {
                //再次入队列
                var tasks = _taskRepository.GetList(t => ids.Contains(t.TaskID)).ToList();
                List<string> queueMessages = new List<string>();
                System.Threading.Tasks.Parallel.ForEach(tasks, t =>
                    {
                        queueMessages.Add(SerializeUtil.ToJson(t));
                    });
                QueueHelper.Enqueue(WAYBILL_INSTORAGE_QUEUENAME, queueMessages.ToArray(), RABBITMQ_CONFIGKEY);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 快递入仓打印交接单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IList<PrintInStorageInvoiceExt> GetPrintInStorageInvoice(PrintInStorageInvoiceParam param)
        {
            return _wayBillInfoRepository.GetPrintInStorageInvoice(param);
        }

        /// <summary>
        /// CS版快递入仓 add by huhaiyou 2014-4-28
        /// </summary>
        /// <param name="createInStorageExtCS"></param>
        public void CreateInStorageCS(CreateInStorageExtCS createInStorageExtCS)
        {
            #region 记录请求日志

            string requestString = JsonConvert.SerializeObject(createInStorageExtCS);
            Log.Info(requestString);

            #endregion

            Check.Argument.IsNotNull(createInStorageExtCS, "createInStorageExt");
            Check.Argument.IsNotNull(createInStorageExtCS.InStorage, "createInStorageExt.InStorage");
            Check.Argument.IsNotNull(createInStorageExtCS.WayBillInfos, "createInStorageExt.WayBillInfos");

            DateTime inStorageCreateOn = DateTime.Now;
            List<string> wayBillNumbers = new List<string>();

            createInStorageExtCS.WayBillInfos.Each(p => wayBillNumbers.Add(p.WayBillNumber));

            #region 判断运单是否正在入仓

            if (IsInStorageWorkDoing(createInStorageExtCS.WayBillInfos.Select(w => w.WayBillNumber).ToArray()))
            {
                throw new Exception("你提交的运单中包含正在进行入仓操作的运单");
            }

            #endregion

            var wayBillList = _wayBillInfoRepository.GetList(p => wayBillNumbers.Contains(p.WayBillNumber));
            try
            {
                var listWayBillBusinessDate = new List<WayBillBusinessDateInfo>();

                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                {
                    var customerCode = createInStorageExtCS.InStorage.CustomerCode;

                    var customer = _customerRepository.GetFiltered(c => c.CustomerCode == customerCode).First();
                    createInStorageExtCS.InStorage.PaymentTypeID = customer.PaymentTypeID;

                    bool calcFeeOnInStorage = (customer.PaymentTypeID == 3 || customer.PaymentTypeID == 4);

                    string receivingBillID = SequenceNumberService.GetSequenceNumber(PrefixCode.ReceivingBillID);
                    decimal totalWeight = 0;
                    createInStorageExtCS.WayBillInfos.Each(p =>
                        {
                            #region 修改运单资料信息和订单状态

                            var oldstatus = WayBill.StatusToValue(WayBill.StatusEnum.Submitted);
                            var wayBillInfo = wayBillList.FirstOrDefault(w => w.WayBillNumber == p.WayBillNumber && w.CustomerCode.ToUpperInvariant() == p.CustomerCode.ToUpperInvariant() && w.Status == oldstatus);
                            if (wayBillInfo == null)
                            {
                                throw new ArgumentException("该运单号\"{0}\"不存在，或则是当前状态不是已提交！".FormatWith(p.WayBillNumber));
                            }

                            #region 数据校验

                            if (p.SettleWeight < p.Weight)
                            {
                                throw new BusinessLogicException(string.Format("运单[{2}]结算重量[{0}]小于实际重量[{1}]", p.SettleWeight, p.Weight, p.WayBillNumber));
                            }

                            if (p.ShippingMethodId != wayBillInfo.InShippingMethodID)
                            {
                                throw new BusinessLogicException(string.Format("运单[{2}]实际入仓方式[{0}]与应入仓方式不相符[{1}]", p.ShippingMethodId, wayBillInfo.InShippingMethodID, p.WayBillNumber));
                            }

                            var waybillPackageDetailSettleWeight = p.WaybillPackageDetailList.Where(w => w.WayBillNumber == p.WayBillNumber).Sum(pp => pp.SettleWeight);
                            if (waybillPackageDetailSettleWeight != 0 && waybillPackageDetailSettleWeight != p.SettleWeight)
                            {
                                throw new BusinessLogicException(string.Format("运单[{2}]运单结算重量[{0}]与包裹明细结算重量不相符[{1}]", p.SettleWeight, waybillPackageDetailSettleWeight, p.WayBillNumber));
                            }

                            #endregion

                            //if (calcFeeOnInStorage)
                            //{
                                wayBillInfo.SettleWeight = p.SettleWeight;
                            //}
                            totalWeight+= p.SettleWeight;
                            wayBillInfo.Weight = p.Weight;

                            if (p.IsBusinessExpress)
                            {
                                wayBillInfo.Width = 0;
                                wayBillInfo.Length = 0;
                                wayBillInfo.Height = 0;
                            }
                            else
                            {
                                wayBillInfo.Width = p.WaybillPackageDetailList.First().Width;
                                wayBillInfo.Length = p.WaybillPackageDetailList.First().Length;
                                wayBillInfo.Height = p.WaybillPackageDetailList.First().Height;
                            }

                            wayBillInfo.IsBattery = p.IsBattery;
                            wayBillInfo.CustomerOrderInfo.IsBattery = p.IsBattery;
                            wayBillInfo.CustomerOrderInfo.SensitiveTypeID = p.SensitiveType;
                            wayBillInfo.GoodsTypeID = p.GoodsTypeID;
                            wayBillInfo.InStorageID = createInStorageExtCS.InStorage.InStorageID;

                            if (wayBillInfo.CustomerOrderID.HasValue)
                            {

                                wayBillInfo.CustomerOrderInfo.Length = 0;
                                wayBillInfo.CustomerOrderInfo.Width = 0;
                                wayBillInfo.CustomerOrderInfo.Height = 0;

                                wayBillInfo.CustomerOrderInfo.GoodsTypeID = p.GoodsTypeID;
                                wayBillInfo.CustomerOrderInfo.Status =
                                    CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Have);
                                wayBillInfo.CustomerOrderInfo.LastUpdatedBy = createInStorageExtCS.InStorage.CreatedBy;
                                wayBillInfo.CustomerOrderInfo.LastUpdatedOn = DateTime.Now;
                                //插入订单状态记录
                                _customerOrderStatusRepository.Add(new CustomerOrderStatus
                                    {
                                        CustomerOrderID = wayBillInfo.CustomerOrderID.Value,
                                        CreatedOn = DateTime.Now,
                                        Status = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Have)
                                    });
                            }
                            wayBillInfo.LastUpdatedBy = createInStorageExtCS.InStorage.CreatedBy;
                            wayBillInfo.LastUpdatedOn = DateTime.Now;
                            wayBillInfo.InStorageCreatedOn = inStorageCreateOn;
                            //wayBillInfo = _orderService.UpdataWayBillTrackingNumber(wayBillInfo, p.TrackingNumber);

                            wayBillInfo.Status = WayBill.StatusToValue(WayBill.StatusEnum.Have);
                            wayBillInfo.CustomerOrderInfo.Status = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Have);
                            _wayBillInfoRepository.Modify(wayBillInfo);

                            #endregion

                            #region 记录运单收货日期
                            listWayBillBusinessDate.Add(new WayBillBusinessDateInfo
                           {
                               ReceivingDate = createInStorageExtCS.InStorage.ReceivingDate,
                               WayBillNumber = p.WayBillNumber,
                               CreatedOn = System.DateTime.Now,
                               CreatedBy = createInStorageExtCS.InStorage.CreatedBy,
                               LastUpdatedOn = System.DateTime.Now,
                               LastUpdatedBy = createInStorageExtCS.InStorage.CreatedBy
                           });
                            #endregion

                            //如果结算方式为预付或者现金
                            if (calcFeeOnInStorage)
                            {
                                //客户资金扣费记录
                                CreateCustomerAmountRecord(p.PriceResult, p, createInStorageExtCS.InStorage.InStorageID, createInStorageExtCS.InStorage.CreatedBy);

                                //收货费用表记录，标记为账单已出
                                CreateReceivingExpensRecord(p.PriceResult, wayBillInfo, receivingBillID, createInStorageExtCS.InStorage.CreatedBy);
                            }
                            //结算方式非预付和现金
                            else
                            {
                                #region 生成收货费用记录，待Job再算费用

                                var receivingExpens = new ReceivingExpens()
                                    {
                                        WayBillNumber = p.WayBillNumber,
                                        CustomerOrderNumber = wayBillInfo.CustomerOrderNumber,
                                        Status = (int)(Financial.ReceivingExpenseStatusEnum.UnAudited),
                                        CreatedBy = createInStorageExtCS.InStorage.CreatedBy,
                                        CreatedOn = DateTime.Now,
                                        LastUpdatedBy = createInStorageExtCS.InStorage.CreatedBy,
                                        LastUpdatedOn = DateTime.Now,
                                        AcceptanceDate = inStorageCreateOn,
                                    };

                                //_wayBillInfoRepository.BulkInsert("ReceivingExpenses", new[] { receivingExpens });
                                _receivingExpensRepository.Add(receivingExpens);

                                #endregion
                            }

                            #region 录入运单操作日志

                            //Add By zxq
                            //Time:2014-09-15
                            var wayBillEventLog = new WayBillEventLog()
                                {
                                    WayBillNumber = wayBillInfo.WayBillNumber,
                                    EventCode = (int)WayBillEvent.EventCodeEnum.InStorage,
                                    Description = WayBillEvent.GetEventCodeDescription((int)WayBillEvent.EventCodeEnum.InStorage),
                                    EventDate = DateTime.Now,
                                    LastUpdatedOn = DateTime.Now,
                                    Operator = createInStorageExtCS.InStorage.CreatedBy,
                                };

                            _wayBillEventLogRepository.Add(wayBillEventLog);
                            //_wayBillInfoRepository.BulkInsert("WayBillEventLogs", new[] { wayBillEventLog });

                            #endregion

                            #region 生成入仓包裹明细

                            List<WaybillPackageDetail> listWaybillPackageDetail = new List<WaybillPackageDetail>();
                            foreach (var package in p.WaybillPackageDetailList.Where(w => w.WayBillNumber == p.WayBillNumber))
                            {
                                var inPackage = new WaybillPackageDetail();
                                inPackage.WayBillNumber = package.WayBillNumber;
                                inPackage.Weight = package.Weight;
                                inPackage.AddWeight = package.AddWeight;
                                inPackage.SettleWeight = package.SettleWeight;
                                inPackage.Length = package.Length;
                                inPackage.Width = package.Width;
                                inPackage.Height = package.Height;
                                inPackage.LengthiFee = package.LengthFee;
                                inPackage.WeightFee = package.WeightFee;
                                inPackage.CreatedOn = DateTime.Now;
                                inPackage.CreatedBy = createInStorageExtCS.InStorage.CreatedBy;
                                inPackage.LastUpdatedOn = DateTime.Now;
                                inPackage.LastUpdatedBy = createInStorageExtCS.InStorage.CreatedBy;

                                //listWaybillPackageDetail.Add(inPackage);
                                _waybillPackageDetailRepository.Add(inPackage);
                            }
                            //_wayBillInfoRepository.BulkInsert("WaybillPackageDetails", listWaybillPackageDetail);

                            #endregion
                        });


                    //批量插入业务日期
                    _wayBillInfoRepository.BulkInsert("WayBillBusinessDateInfos", listWayBillBusinessDate);



                    //如果结算方式为预付或者现金,生成收货费用信息。
                    if (calcFeeOnInStorage)
                    {
                        #region 自动出账单

                        var receivingBill = new ReceivingBill()
                            {
                                ReceivingBillID = receivingBillID,
                                CustomerCode = createInStorageExtCS.WayBillInfos[0].CustomerCode,
                                CustomerName = customer.Name,
                                ReceivingBillDate = DateTime.Now,
                                ReceivingBillAuditor = "System-Auto",
                                BillStartTime = DateTime.Now,
                                BillEndTime = DateTime.Now,
                                Search = 100, //100只需要生成账单excel
                                Status = 1,
                            };

                        _receivingBillRepository.Add(receivingBill);
                        //_wayBillInfoRepository.BulkInsert("ReceivingBills", new[] { receivingBill });

                        #endregion
                    }

                    //不保存结算重量
                    //if (!calcFeeOnInStorage)
                    //{
                        createInStorageExtCS.InStorage.TotalWeight = totalWeight;
                    //}

                    createInStorageExtCS.InStorage.PhysicalTotalWeight = createInStorageExtCS.WayBillInfos.Sum(p => p.Weight);

                    //生成入仓资料
                    var inStorage = new InStorageInfo();
                    createInStorageExtCS.InStorage.CopyTo(inStorage);
                    inStorage.ReceivingClerk = inStorage.CreatedBy = inStorage.LastUpdatedBy = createInStorageExtCS.InStorage.CreatedBy;
                    inStorage.CreatedOn = inStorage.LastUpdatedOn = inStorageCreateOn;
                    inStorage.Status = 1;
                    _inStorageInfoRepository.Add(inStorage);

                    _wayBillEventLogRepository.UnitOfWork.Commit();
                    _waybillPackageDetailRepository.UnitOfWork.Commit();
                    _inStorageInfoRepository.UnitOfWork.Commit();
                    _wayBillInfoRepository.UnitOfWork.Commit();
                    _receivingExpensRepository.UnitOfWork.Commit();
                    _receivingBillRepository.UnitOfWork.Commit();

                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                throw;
            }

            finally
            {
                #region 入仓操作完成，从正在入仓的运单列表中删除

                RemoveInStorageWorkDoing(createInStorageExtCS.WayBillInfos.Select(w => w.WayBillNumber).ToArray());

                #endregion
            }
        }

        /// <summary>
        /// 新增客户资金扣费记录
        /// </summary>
        private void CreateCustomerAmountRecord(PriceProviderResult priceResult, WayBillInfoExt wayBillInfo, string inStorageId, string createdBy)
        {
            #region 生成运费扣费记录

            if (priceResult.ShippingFee > 0)
            {
                var freight = new CustomerAmountRecordParam
                    {
                        CreatedBy = createdBy,
                        Amount = priceResult.ShippingFee,
                        CustomerCode = wayBillInfo.CustomerCode,
                        WayBillNumber = wayBillInfo.WayBillNumber,
                        MoneyChangeTypeId = 2,
                        FeeTypeId = 3,
                        Remark = "运单号：{0}扣运费".FormatWith(wayBillInfo.WayBillNumber),
                        TransactionNo = inStorageId,
                    };

                //1-成功，2-以存在交易号，0-失败
                var result = _customerAmountRecordRepository.CreateCustomerAmountRecord(freight);
                if (result != 1)
                {
                    throw new ArgumentException("该运单号\"{0}\"生成运费扣费记录失败！".FormatWith(wayBillInfo.WayBillNumber));
                }
                ;
            }

            #endregion

            #region 生成挂号费扣费记录

            if (priceResult.RegistrationFee > 0)
            {
                var register = new CustomerAmountRecordParam
                    {
                        CreatedBy = createdBy,
                        Amount = priceResult.RegistrationFee,
                        CustomerCode = wayBillInfo.CustomerCode,
                        WayBillNumber = wayBillInfo.WayBillNumber,
                        MoneyChangeTypeId = 2,
                        FeeTypeId = 4,
                        Remark = "运单号：{0}扣挂号费".FormatWith(wayBillInfo.WayBillNumber),
                        TransactionNo = inStorageId,
                    };
                var result = _customerAmountRecordRepository.CreateCustomerAmountRecord(register);
                if (result != 1)
                {
                    throw new ArgumentException("该运单号\"{0}\"生成挂号费扣费记录失败！".FormatWith(wayBillInfo.WayBillNumber));
                }
                ;
            }

            #endregion

            #region 生成燃油费扣费记录

            if (priceResult.FuelFee > 0)
            {
                var fuelCharge = new CustomerAmountRecordParam
                    {
                        CreatedBy = createdBy,
                        Amount = priceResult.FuelFee,
                        CustomerCode = wayBillInfo.CustomerCode,
                        WayBillNumber = wayBillInfo.WayBillNumber,
                        MoneyChangeTypeId = 2,
                        FeeTypeId = 5,
                        Remark = "运单号：{0}扣燃油费".FormatWith(wayBillInfo.WayBillNumber),
                        TransactionNo = inStorageId,
                    };
                var result = _customerAmountRecordRepository.CreateCustomerAmountRecord(fuelCharge);
                if (result != 1)
                {
                    throw new ArgumentException("该运单号\"{0}\"生成燃油费扣费记录失败！".FormatWith(wayBillInfo.WayBillNumber));
                }
                ;
            }

            #endregion

            #region 生成关税预付服务费扣费记录

            if (priceResult.TariffPrepayFee > 0)
            {
                var tariffPrepay = new CustomerAmountRecordParam
                    {
                        CreatedBy = createdBy,
                        Amount = priceResult.TariffPrepayFee,
                        CustomerCode = wayBillInfo.CustomerCode,
                        WayBillNumber = wayBillInfo.WayBillNumber,
                        MoneyChangeTypeId = 2,
                        FeeTypeId = 6,
                        Remark = "运单号：{0}扣关税预付服务费".FormatWith(wayBillInfo.WayBillNumber),
                        TransactionNo = inStorageId
                    };
                var result = _customerAmountRecordRepository.CreateCustomerAmountRecord(tariffPrepay);
                if (result != 1)
                {
                    throw new ArgumentException("该运单号\"{0}\"生成附加费扣费记录失败！".FormatWith(wayBillInfo.WayBillNumber));
                }
                ;
            }

            #endregion

            #region 生成偏远附加费扣费记录

            if (priceResult.RemoteAreaFee > 0)
            {
                var tariffPrepay = new CustomerAmountRecordParam
                    {
                        CreatedBy = createdBy,
                        Amount = priceResult.RemoteAreaFee,
                        CustomerCode = wayBillInfo.CustomerCode,
                        WayBillNumber = wayBillInfo.WayBillNumber,
                        MoneyChangeTypeId = 2,
                        FeeTypeId = (int)CustomerOrder.FeeTypeEnum.RemoteAreaFee,
                        Remark = "运单号：{0}扣偏远附加费".FormatWith(wayBillInfo.WayBillNumber),
                        TransactionNo = inStorageId
                    };
                var result = _customerAmountRecordRepository.CreateCustomerAmountRecord(tariffPrepay);
                if (result != 1)
                {
                    throw new ArgumentException("该运单号\"{0}\"生成偏远附加费记录失败！".FormatWith(wayBillInfo.WayBillNumber));
                }
                ;
            }

            #endregion

            #region 生成附加费扣费记录

            //附加费
            decimal Surcharge = priceResult.Value - (priceResult.ShippingFee + priceResult.RegistrationFee + priceResult.FuelFee + priceResult.TariffPrepayFee + priceResult.RemoteAreaFee);

            if (Surcharge > 0)
            {
                var surcharge = new CustomerAmountRecordParam
                    {
                        CreatedBy = createdBy,
                        Amount = Surcharge,
                        CustomerCode = wayBillInfo.CustomerCode,
                        WayBillNumber = wayBillInfo.WayBillNumber,
                        MoneyChangeTypeId = 2,
                        FeeTypeId = 2,
                        Remark = "运单号：{0}扣附加费".FormatWith(wayBillInfo.WayBillNumber),
                        TransactionNo = inStorageId
                    };
                var result = _customerAmountRecordRepository.CreateCustomerAmountRecord(surcharge);
                if (result != 1)
                {
                    throw new ArgumentException("该运单号\"{0}\"生成附加费扣费记录失败！".FormatWith(wayBillInfo.WayBillNumber));
                }
                ;
            }

            #endregion

        }

        //生成收货费用表记录
        private void CreateReceivingExpensRecord(PriceProviderResult priceResult, WayBillInfo wayBillInfo, string receivingBillID, string createdBy)
        {

            #region 生成收货费用表明细记录

            List<ReceivingExpenseInfo> listReceivingExpenseInfos = new List<ReceivingExpenseInfo>();

            listReceivingExpenseInfos.Add(new ReceivingExpenseInfo()
                {
                    Amount = priceResult.ShippingFee,
                    CreatedBy = createdBy,
                    CreatedOn = DateTime.Now,
                    FeeTypeID = (int)CustomerOrder.FeeTypeEnum.Freight,
                    LastUpdatedBy = createdBy,
                    LastUpdatedOn = DateTime.Now,
                    ReceivingBillID = receivingBillID,
                    OperationType = 1
                });


            listReceivingExpenseInfos.Add(new ReceivingExpenseInfo()
                {
                    Amount = priceResult.FuelFee,
                    CreatedBy = createdBy,
                    CreatedOn = DateTime.Now,
                    FeeTypeID = (int)CustomerOrder.FeeTypeEnum.FuelCharge,
                    LastUpdatedBy = createdBy,
                    LastUpdatedOn = DateTime.Now,
                    ReceivingBillID = receivingBillID,
                    OperationType = 1
                });


            listReceivingExpenseInfos.Add(new ReceivingExpenseInfo()
                {
                    Amount = priceResult.RegistrationFee,
                    CreatedBy = createdBy,
                    CreatedOn = DateTime.Now,
                    FeeTypeID = (int)CustomerOrder.FeeTypeEnum.Register,
                    LastUpdatedBy = createdBy,
                    LastUpdatedOn = DateTime.Now,
                    ReceivingBillID = receivingBillID,
                    OperationType = 1
                });

            //附加费
            decimal surcharge = priceResult.Value - (priceResult.ShippingFee + priceResult.RegistrationFee + priceResult.FuelFee + priceResult.TariffPrepayFee + priceResult.RemoteAreaFee);

            listReceivingExpenseInfos.Add(new ReceivingExpenseInfo()
                {
                    Amount = surcharge,
                    CreatedBy = createdBy,
                    CreatedOn = DateTime.Now,
                    FeeTypeID = (int)CustomerOrder.FeeTypeEnum.Surcharge,
                    LastUpdatedBy = createdBy,
                    LastUpdatedOn = DateTime.Now,
                    ReceivingBillID = receivingBillID,
                    OperationType = 1
                });

            listReceivingExpenseInfos.Add(new ReceivingExpenseInfo()
                {
                    Amount = priceResult.TariffPrepayFee,
                    CreatedBy = createdBy,
                    CreatedOn = DateTime.Now,
                    FeeTypeID = (int)CustomerOrder.FeeTypeEnum.TariffPrepayFee,
                    LastUpdatedBy = createdBy,
                    LastUpdatedOn = DateTime.Now,
                    ReceivingBillID = receivingBillID,
                    OperationType = 1
                });

            listReceivingExpenseInfos.Add(new ReceivingExpenseInfo()
                {
                    Amount = priceResult.RemoteAreaFee,
                    CreatedBy = createdBy,
                    CreatedOn = DateTime.Now,
                    FeeTypeID = (int)CustomerOrder.FeeTypeEnum.RemoteAreaFee,
                    LastUpdatedBy = createdBy,
                    LastUpdatedOn = DateTime.Now,
                    ReceivingBillID = receivingBillID,
                    OperationType = 1
                });

            #endregion

            var receivingExpens = new ReceivingExpens()
                {
                    WayBillNumber = wayBillInfo.WayBillNumber,
                    CustomerOrderNumber = wayBillInfo.CustomerOrderNumber,
                    Status = (int)Financial.ReceivingExpenseStatusEnum.OutBilled,
                    Auditor = "System-Auto",
                    AuditorDate = DateTime.Now,
                    CreatedOn = DateTime.Now,
                    CreatedBy = createdBy,
                    LastUpdatedOn = DateTime.Now,
                    LastUpdatedBy = createdBy,
                    IsNoGet = true,
                    AcceptanceDate = DateTime.Now,
                };

            //_wayBillInfoRepository.BulkInsert("ReceivingExpenses", new[] { receivingExpens });
            //_wayBillInfoRepository.BulkInsert("ReceivingExpenseInfos", listReceivingExpenseInfos);

            listReceivingExpenseInfos.ForEach(receivingExpens.ReceivingExpenseInfos.Add);
            _receivingExpensRepository.Add(receivingExpens);
        }

        public InStorageInfo GetInStorageInfo(string InStorageId)
        {
            Check.Argument.IsNullOrWhiteSpace(InStorageId, "入仓单号");
            return _inStorageInfoRepository.GetInStorageInfo(InStorageId);
        }

        public IPagedList<InStorageInfo> GetInStoragePagedList(InStorageListSearchParam param)
        {
            var startTime = param.InStartDate.HasValue ? param.InStartDate.Value : new DateTime(2013, 1, 1);
            var endTime = param.InEndDate.HasValue ? param.InEndDate.Value : new DateTime(2020, 1, 1);
            Expression<Func<InStorageInfo, bool>> filter = p => true;
            filter = filter.AndIf(p => p.CustomerCode == param.CustomerCode, !string.IsNullOrWhiteSpace(param.CustomerCode))
                           .AndIf(p => p.InStorageID.Contains(param.InStorageID), !string.IsNullOrWhiteSpace(param.InStorageID))
                           .AndIf(p => p.WayBillInfos.FirstOrDefault().InShippingMethodID == param.ShippingMethodId,
                                  param.ShippingMethodId.HasValue)
                           .And(p => p.CreatedOn >= startTime && p.CreatedOn <= endTime);
            Func<IQueryable<InStorageInfo>, IOrderedQueryable<InStorageInfo>>
                orderBy = o => o.OrderByDescending(p => p.CreatedOn);
            return _inStorageInfoRepository.FindPagedList(param.Page, param.PageSize, filter, orderBy);
        }

        /// <summary>
        /// 获取物流快递打单列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedList<ExpressPrintWayBillExt> GetExpressPrintWayBillList(ExpressPrintWayBillParam param)
        {
            return _inStorageInfoRepository.GetExpressPrintWayBillList(param);
        }

        /// <summary>
        /// 获取入仓列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedList<InStorageInfoExt> GetInStorageInfoExtPagedList(InStorageListSearchParam param)
        {
            return _inStorageInfoRepository.GetInStorageInfoExtPagedList(param);
        }


        public IEnumerable<WayBillInfo> GetWayBillByWayBillNumbers(IEnumerable<string> wayBillNumbers)
        {
            var billNumbers = wayBillNumbers as string[] ?? wayBillNumbers.ToArray();
            if (billNumbers.Any())
                return _wayBillInfoRepository.GetList(p => billNumbers.Contains(p.WayBillNumber));
            return null;
        }

        public LithuaniaInfo GetLithuaniaInfoByWayBillNumber(string wayBillNumber)
        {
            return _lithuaniaInfoRepository.Get(wayBillNumber);
        }

        /// <summary>
        /// 批量更新运单的物流商
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        public void BatchUpdateWayBillByVenderCode(IEnumerable<WayBillInfo> wayBillNumbers)
        {
            foreach (var wayBillNumber in wayBillNumbers)
            {
                wayBillNumber.LastUpdatedBy = _workContext.User.UserUame;
                wayBillNumber.LastUpdatedOn = DateTime.Now;
                _wayBillInfoRepository.Modify(wayBillNumber);
                _wayBillInfoRepository.UnitOfWork.Commit();
            }
        }

        public bool IsExitTrackingNumber(string trackingNumber, string wayBillNumber)
        {
            var deleteStatus = WayBill.StatusToValue(WayBill.StatusEnum.Delete);
            var returnStatus = WayBill.StatusToValue(WayBill.StatusEnum.Return);
            if (string.IsNullOrWhiteSpace(wayBillNumber))
            {
                return
                    _wayBillInfoRepository.Exists(
                        p => p.TrackingNumber == trackingNumber && p.Status != deleteStatus && p.Status != returnStatus);
            }
            else
            {
                return
                    _wayBillInfoRepository.Exists(
                        p =>
                        p.TrackingNumber == trackingNumber && p.Status != deleteStatus && p.Status != returnStatus &&
                        p.WayBillNumber != wayBillNumber);
            }
        }

        //是否存在重复真实跟踪号
        public bool IsExitTrueTrackingNumber(string trueTrackingNumber, string wayBillNumber)
        {
            var deleteStatus = WayBill.StatusToValue(WayBill.StatusEnum.Delete);
            var returnStatus = WayBill.StatusToValue(WayBill.StatusEnum.Return);
            return
                _wayBillInfoRepository.Exists(
                    p =>
                    p.TrueTrackingNumber == trueTrackingNumber && p.Status != deleteStatus && p.Status != returnStatus &&
                    p.WayBillNumber != wayBillNumber);
        }

        public void UpdateWayBillTrackingNumber(WayBillInfo wayBillInfo)
        {
            wayBillInfo.LastUpdatedBy = _workContext.User.UserUame;
            wayBillInfo.LastUpdatedOn = DateTime.Now;
            wayBillInfo.CustomerOrderInfo.LastUpdatedBy = _workContext.User.UserUame;
            wayBillInfo.CustomerOrderInfo.LastUpdatedOn = DateTime.Now;
            _wayBillInfoRepository.Modify(wayBillInfo);
            _wayBillInfoRepository.UnitOfWork.Commit();
        }


        /// <summary>
        /// 获取快递日志列表
        /// add by yungchu
        /// </summary>
        /// <returns></returns>
        public List<WayBillPrintLog> GetWayBillPrintLogList(string wayBillNumber)
        {
            Expression<Func<WayBillPrintLog, bool>> filter = p => true;
            filter = filter.AndIf(a => a.waybillnumber == wayBillNumber, !string.IsNullOrEmpty(wayBillNumber));
            return _wayBillPrintLogRepository.GetList(filter);
        }

        /// <summary>
        /// 记录打印日志
        /// add by yungchu
        /// </summary>
        public bool AddWayBillPrintLog(WayBillPrintLog param)
        {
            try
            {
                _wayBillPrintLogRepository.Add(param);
                _wayBillPrintLogRepository.UnitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 获取运单实际入仓件数 zxq
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public int GetWaybillPackageDetailCount(WaybillPackageDetail param)
        {
            int count = _waybillPackageDetailRepository.GetFiltered(w => w.WayBillNumber == param.WayBillNumber).Count();
            return count;
        }

        /// <summary>
        /// 新增入库重量对比配置 yungchu
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool AddInStorageWeightDeviations(InStorageWeightDeviation entity)
        {

            Expression<Func<InStorageWeightDeviation, bool>> filter = p => true;
            filter = filter.AndIf(a => a.CustomerCode == entity.CustomerCode, !string.IsNullOrEmpty(entity.CustomerCode))
                           .AndIf(a => a.ShippingMethodID == entity.ShippingMethodID, entity.ShippingMethodID.HasValue);

            List<InStorageWeightDeviation> isExitData = _inStorageWeightDeviationRepository.GetList(filter);
            if (isExitData.Any())
            {
                throw new ArgumentException(string.Format("已存在客户 {0} 对应的运输方式 {1}", entity.CustomerName, entity.ShippingMethodName));
            }

            try
            {
                entity.CreatedBy = _workContext.User.UserUame;
                entity.CreatedOn = System.DateTime.Now;
                entity.LastUpdatedBy = _workContext.User.UserUame;
                entity.LastUpdatedOn = System.DateTime.Now;
                entity.Status = 1; //启用
                _inStorageWeightDeviationRepository.Add(entity);
                _inStorageWeightDeviationRepository.UnitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        /// 编辑入库重量对比配置 yungchu
        public bool EditInStorageWeightDeviations(InStorageWeightDeviation entity)
        {
            try
            {
                var getModel = _inStorageWeightDeviationRepository.Get(entity.InStorageWeightDeviationID);

                getModel.CustomerCode = entity.CustomerCode;
                getModel.CustomerName = entity.CustomerName;
                getModel.ShippingMethodID = entity.ShippingMethodID;
                getModel.ShippingMethodName = entity.ShippingMethodName;
                getModel.DeviationValue = entity.DeviationValue;
                getModel.Status = 1; //启用
                getModel.CreatedBy = _workContext.User.UserUame;
                getModel.LastUpdatedBy = _workContext.User.UserUame;
                getModel.LastUpdatedOn = System.DateTime.Now;

                _inStorageWeightDeviationRepository.Modify(getModel);
                _inStorageWeightDeviationRepository.UnitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        /// 删除入库重量对比配置 yungchu
        public bool DeleteInStorageWeightDeviations(int id)
        {
            try
            {
                InStorageWeightDeviation model = _inStorageWeightDeviationRepository.Get(id);
                _inStorageWeightDeviationRepository.Remove(model);
                _inStorageWeightDeviationRepository.UnitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        /// 查询入库重量对比配置 yungchu
        public IPagedList<InStorageWeightDeviation> GetInStorageWeightDeviationPagedList(WeightDeviationParam param)
        {
            Expression<Func<InStorageWeightDeviation, bool>> filter = p => true;
            filter = filter.AndIf(a => a.Status == param.Status, param.Status != 0)
                           .AndIf(a => a.CustomerCode.Contains(param.CustomerCode), !string.IsNullOrEmpty(param.CustomerCode))
                           .AndIf(a => a.ShippingMethodID == param.ShippingMethodID, param.ShippingMethodID.HasValue);

            Func<IQueryable<InStorageWeightDeviation>, IOrderedQueryable<InStorageWeightDeviation>>
                orderBy = o => o.OrderByDescending(p => p.CreatedOn);
            return _inStorageWeightDeviationRepository.FindPagedList(param.Page, param.PageSize, filter, orderBy);
        }

        //入仓重量对比 yungchu
        public InStorageWeightDeviation GetInStorageWeightCompareDeviationValue(string customerCode, int shippingMethodId)
        {
            Expression<Func<InStorageWeightDeviation, bool>> filter = p => true;
            filter = filter.AndIf(a => a.CustomerCode == customerCode, !string.IsNullOrEmpty(customerCode))
                           .AndIf(a => a.ShippingMethodID == shippingMethodId, shippingMethodId != 0);

            return _inStorageWeightDeviationRepository.First(filter);

        }

        public InStorageWeightDeviation GetInStorageWeightDeviation(int id)
        {
            return _inStorageWeightDeviationRepository.Get(id);
        }


        public string GetShippingMethodName(string InStorageId)
        {
            return _inStorageInfoRepository.GetShippingMethodName(InStorageId);
        }

        public List<InStorageTotalModel> GetInStorageTotals(string InStorageId)
        {
            return _inStorageInfoRepository.GetInStorageTotals(InStorageId);
        }

        public List<InStorageInfo> GetInStorageNoSettlementList(string customerCode)
        {
            return _inStorageInfoRepository.GetFiltered(p => p.CustomerCode == customerCode && !p.InStorageOrSettlementRelationals.Any() && (p.PaymentTypeID == 3 || p.PaymentTypeID == 4)).OrderByDescending(p => p.CreatedOn).ToList();
        }

        public List<InStorageProcess> GetInStorageProcess(List<string> inStorageIDs)
        {
            return _wayBillInfoRepository.GetInStorageProcess(inStorageIDs);
        }


        public bool AddWayBillBusinessDateInfos(WayBillBusinessDateInfo entity)
        {
            var getModel = _wayBillBusinessDateInfoRepository.First(a => a.WayBillNumber == entity.WayBillNumber);

            try
            {
                if (getModel != null)
                {
                    getModel.LastUpdatedOn = System.DateTime.Now;
                    getModel.LastUpdatedBy = _workContext.User.UserUame;
                    getModel.CreatedBy = _workContext.User.UserUame;
                    getModel.ReceivingDate = entity.ReceivingDate;

                    _wayBillBusinessDateInfoRepository.Modify(getModel);
                    _wayBillBusinessDateInfoRepository.UnitOfWork.Commit();
                }
                else
                {
                    var model = new WayBillBusinessDateInfo
                    {
                        WayBillNumber = entity.WayBillNumber,
                        ReceivingDate = entity.ReceivingDate,
                        LastUpdatedOn = System.DateTime.Now,
                        LastUpdatedBy = _workContext.User.UserUame,
                        CreatedBy = _workContext.User.UserUame,
                        CreatedOn = System.DateTime.Now
                    };


                    _wayBillBusinessDateInfoRepository.Add(model);
                    _wayBillBusinessDateInfoRepository.UnitOfWork.Commit();
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return false;
            }
        }



        public WayBillBusinessDateInfo GetWayBillBusinessDateInfo(string wayBillNumber)
        {
            return _wayBillBusinessDateInfoRepository.First(a => a.WayBillNumber == wayBillNumber);
        }
    }
}
