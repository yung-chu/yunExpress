using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Transactions;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LMS.Services.FreightServices;
using LMS.Services.SequenceNumber;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Services.OutStorageServices
{
    public class OutStorageService : IOutStorageService
    {
        private IWayBillInfoRepository _wayBillInfoRepository;
        private IWorkContext _workContext;
        private IVenderFeeLogRepository _venderFeeLogRepository;
        private IOutStorageInfoRepository _outStorageInfoRepository;
        private ICustomerOrderStatusRepository _customerOrderStatusRepository;
        private IDeliveryChannelConfigurationRepository _deliveryChannelConfigurationRepository;
        //private IInTrackingLogInfoRepository _inTackingLogInfoRepository;
        private IFreightService _freightService;
        private IReceivingExpensRepository _receivingExpensRepository;
        private IReceivingExpenseInfoRepository _receivingExpenseInfoRepository;
        private IDeliveryFeeRepository _deliveryFeeRepository;
        private IDeliveryFeeInfoRepository _deliveryFeeInfoRepository;
        private IWaybillPackageDetailRepository _waybillPackageDetailRepository;
        private ITotalPackageInfoRepository _totalPackageInfoRepository;
        private ITotalPackageOutStorageRelationalInfoRepository _totalPackageOutStorageRelationalInfoRepository;
        private ITotalPackageTraceInfoRepository _totalPackageTraceInfoRepository;
        private ISystemConfigurationRepository _systemConfigurationRepository;
        private IMailPostBagInfoRepository _mailPostBagInfoRepository;
        private ICustomerOrderInfoRepository _customerOrderInfoRepository;
        private IB2CPreAlertLogsRepository _b2CPreAlertLogsRepository;
        public OutStorageService(IWayBillInfoRepository wayBillInfoRepository,
            IVenderFeeLogRepository venderFeeLogRepository,
            IOutStorageInfoRepository outStorageInfoRepository,
            IFreightService freightService,
            ICustomerOrderStatusRepository customerOrderStatusRepository = null,
            IDeliveryChannelConfigurationRepository deliveryChannelConfigurationRepository = null,
            IReceivingExpensRepository receivingExpensRepository = null,
            IReceivingExpenseInfoRepository receivingExpenseInfoRepository = null,
            IDeliveryFeeRepository deliveryFeeRepository = null,
            IDeliveryFeeInfoRepository deliveryFeeInfoRepository = null,
             IWaybillPackageDetailRepository waybillPackageDetailRepository = null,
            IWorkContext workContext = null,
            ITotalPackageInfoRepository totalPackageInfoRepository = null,
            ITotalPackageOutStorageRelationalInfoRepository totalPackageOutStorageRelationalInfoRepository = null,
            ITotalPackageTraceInfoRepository totalPackageTraceInfoRepository = null,
            ISystemConfigurationRepository systemConfigurationRepository = null,
            IMailPostBagInfoRepository mailPostBagInfoRepository = null,
            ICustomerOrderInfoRepository customerOrderInfoRepository= null,
			IB2CPreAlertLogsRepository b2CPreAlertLogsRepository=null)
        {
            _workContext = workContext;
            _wayBillInfoRepository = wayBillInfoRepository;
            _venderFeeLogRepository = venderFeeLogRepository;
            _outStorageInfoRepository = outStorageInfoRepository;
            _customerOrderStatusRepository = customerOrderStatusRepository;
            _deliveryChannelConfigurationRepository = deliveryChannelConfigurationRepository;
            //_inTackingLogInfoRepository = inTackingLogInfoRepository;
            _receivingExpensRepository = receivingExpensRepository;
            _receivingExpenseInfoRepository = receivingExpenseInfoRepository;
            _deliveryFeeRepository = deliveryFeeRepository;
            _deliveryFeeInfoRepository = deliveryFeeInfoRepository;
            _freightService = freightService;
            _waybillPackageDetailRepository = waybillPackageDetailRepository;
            _freightService = freightService;
            _totalPackageInfoRepository = totalPackageInfoRepository;
            _totalPackageOutStorageRelationalInfoRepository = totalPackageOutStorageRelationalInfoRepository;
            _totalPackageTraceInfoRepository = totalPackageTraceInfoRepository;
            _systemConfigurationRepository = systemConfigurationRepository;
            _mailPostBagInfoRepository = mailPostBagInfoRepository;
            _customerOrderInfoRepository = customerOrderInfoRepository;
            _b2CPreAlertLogsRepository = b2CPreAlertLogsRepository;
        }

        public void CreateOutStorage(CreateOutStorageExt createOutStorageExt)
        {
            Check.Argument.IsNotNull(createOutStorageExt, "createOutStorageExt");
            Check.Argument.IsNotNull(createOutStorageExt.OutStorage, "createOutStorageExt.OutStorage");
            Check.Argument.IsNotNull(createOutStorageExt.WayBillInfos, "createOutStorageExt.WayBillInfos");

            DateTime outStorageCreatedOn = DateTime.Now;
            List<string> waybillinfoIds = new List<string>();
            createOutStorageExt.WayBillInfos.Each(p => waybillinfoIds.Add(p.WayBillNumber));
            var wayBills = _wayBillInfoRepository.GetList(p => waybillinfoIds.Contains(p.WayBillNumber));

            List<WayBillEventLog> listWayBillEventLog = new List<WayBillEventLog>();
            List<OutStorageInfo> listOutStorageInfo = new List<OutStorageInfo>();
            List<CustomerOrderStatus> listCustomerOrderStatus = new List<CustomerOrderStatus>();
            List<string> listWaybillSend = new List<string>();
            List<string> listWaybillWaitOrder = new List<string>();
            List<int> listCustomerOrderId = new List<int>();

            var outShippingMethodId = createOutStorageExt.WayBillInfos.First().OutShippingMethodID;
            var outShippingMethodName = createOutStorageExt.WayBillInfos.First().OutShippingMethodName;

            createOutStorageExt.WayBillInfos.Each(p =>
                {
                    #region 修改运单资料信息和订单状态

                    var oldstatus = WayBill.StatusToValue(WayBill.StatusEnum.Have);
                    var wayBillInfo = wayBills.Find(w => w.WayBillNumber == p.WayBillNumber && w.Status == oldstatus);

                    if (wayBillInfo == null)
                    {
                        throw new ArgumentException("该运单号\"{0}\"不存在，或则是当前状态不是已收货！".FormatWith(p.WayBillNumber));
                    }

                    //有跟踪号
                    if (p.HaveTrackingNum)
                    {
                        if (p.TrackingNumber == "null" || string.IsNullOrEmpty(p.TrackingNumber))
                        {
                            //修改运单状态  待转单
                            listWaybillWaitOrder.Add(p.WayBillNumber);
                        }
                        else
                        {
                            //修改运单状态  已发货
                            listWaybillSend.Add(p.WayBillNumber);
                        }
                    }
                    else
                    {
                        //修改运单状态  已发货
                        listWaybillSend.Add(p.WayBillNumber);
                    }


                        //插入订单状态记录
                    listCustomerOrderStatus.Add(new CustomerOrderStatus
                        {
                            CustomerOrderID = wayBillInfo.CustomerOrderID.Value,
                            CreatedOn = DateTime.Now,
                            Status = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Send)
                        });

                    listCustomerOrderId.Add(wayBillInfo.CustomerOrderID.Value);

                    #region 录入内部信息

                    var wayBillEventLog = new WayBillEventLog()
                    {
                        WayBillNumber = wayBillInfo.WayBillNumber,
                        EventCode = (int)WayBillEvent.EventCodeEnum.OutStorage,
                        Description = WayBillEvent.GetEventCodeDescription((int)WayBillEvent.EventCodeEnum.OutStorage),
                        EventDate = outStorageCreatedOn,
                        LastUpdatedOn = outStorageCreatedOn,
                        Operator = _workContext.User.UserUame,
                    };
                    listWayBillEventLog.Add(wayBillEventLog);

                    #endregion

                    #endregion
                });


            //createOutStorageExt.WayBillInfos.Each(p =>
            //    {
            //        #region 修改运单资料信息和订单状态

            //        var oldstatus = WayBill.StatusToValue(WayBill.StatusEnum.Have);
            //        var wayBillInfo = wayBills.Find(w => w.WayBillNumber == p.WayBillNumber && w.Status == oldstatus);

            //        if (wayBillInfo == null)
            //        {
            //            throw new ArgumentException("该运单号\"{0}\"不存在，或则是当前状态不是已收货！".FormatWith(p.WayBillNumber));
            //        }
            //        wayBillInfo.OutShippingMethodID = p.OutShippingMethodID;
            //        wayBillInfo.OutShippingMethodName = p.OutShippingMethodName;
            //        wayBillInfo.OutStorageID = createOutStorageExt.OutStorage.OutStorageID;
            //        wayBillInfo.VenderCode = createOutStorageExt.OutStorage.VenderCode;

            //        //有跟踪号
            //        if (p.HaveTrackingNum)
            //        {
            //            if (p.TrackingNumber == "null" || string.IsNullOrEmpty(p.TrackingNumber))
            //            {
            //                //修改运单状态  待转单
            //                wayBillInfo.Status = WayBill.StatusToValue(WayBill.StatusEnum.WaitOrder);
            //            }
            //            else
            //            {
            //                //修改运单状态  已发货
            //                wayBillInfo.Status = WayBill.StatusToValue(WayBill.StatusEnum.Send);
            //            }
            //        }
            //        else
            //        {
            //            //修改运单状态  已发货
            //            wayBillInfo.Status = WayBill.StatusToValue(WayBill.StatusEnum.Send);
            //        }


            //        if (wayBillInfo.CustomerOrderID.HasValue)
            //        {
            //            //修改订单状态
            //            wayBillInfo.CustomerOrderInfo.Status =
            //                CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Send);
            //            wayBillInfo.CustomerOrderInfo.LastUpdatedBy = _workContext.User.UserUame;
            //            wayBillInfo.CustomerOrderInfo.LastUpdatedOn = DateTime.Now;

            //            //插入订单状态记录
            //            listCustomerOrderStatus.Add(new CustomerOrderStatus
            //            {
            //                CustomerOrderID = wayBillInfo.CustomerOrderID.Value,
            //                CreatedOn = DateTime.Now,
            //                Status = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Send)
            //            });

            //        }
            //        wayBillInfo.LastUpdatedBy = _workContext.User.UserUame;
            //        wayBillInfo.LastUpdatedOn = DateTime.Now;
            //        wayBillInfo.OutStorageCreatedOn = outStorageCreatedOn;
            //        _wayBillInfoRepository.Modify(wayBillInfo);

            //        #endregion

            //        #region 录入内部信息

            //        //Add By zxq
            //        //Time:2014-09-15
            //        var wayBillEventLog = new WayBillEventLog()
            //        {
            //            WayBillNumber = wayBillInfo.WayBillNumber,
            //            EventCode = (int)WayBillEvent.EventCodeEnum.OutStorage,
            //            Description = WayBillEvent.GetEventCodeDescription((int)WayBillEvent.EventCodeEnum.OutStorage),
            //            EventDate = DateTime.Now,
            //            LastUpdatedOn = DateTime.Now,
            //            Operator = _workContext.User.UserUame,
            //        };
            //        listWayBillEventLog.Add(wayBillEventLog);

            //        #endregion
            //    });

            //生成出仓资料
            var outStorage = new OutStorageInfo();
            outStorage = createOutStorageExt.OutStorage;
            outStorage.DeliveryStaff = outStorage.CreatedBy = outStorage.LastUpdatedBy = _workContext.User.UserUame;
            outStorage.CreatedOn = outStorage.LastUpdatedOn = outStorageCreatedOn;
            outStorage.Status = 1;

            if (createOutStorageExt.WayBillInfos.First().OutShippingMethodName == "国际小包优+")
            {
                var countryCode = createOutStorageExt.WayBillInfos.First().CountryCode;
                string sequenceNumber = SequenceNumberService.GetSequenceNumber("U-");
                _mailPostBagInfoRepository.Add(new MailPostBagInfo()
                    {
                        CountryCode = countryCode,
                        OutStorageID = outStorage.OutStorageID,
                        IsBattery = createOutStorageExt.WayBillInfos.First().IsBattery,
                        PostBagNumber = sequenceNumber.Replace("-", "-" + countryCode + "-"),
                        TotalWeight = createOutStorageExt.WayBillInfos.Sum(p => p.Weight),
                        CreatedBy = outStorage.LastUpdatedBy,
                        CreatedOn = DateTime.Now,
                        LastUpdatedBy = outStorage.LastUpdatedBy,
                        LastUpdatedOn = DateTime.Now,

                    });
            }
            //_outStorageInfoRepository.Add(outStorage);

            listOutStorageInfo.Add(outStorage);

            using (var transaction = new TransactionScope())
            {

                _wayBillInfoRepository.BulkInsert("WayBillEventLogs", listWayBillEventLog);
                _wayBillInfoRepository.BulkInsert("OutStorageInfos", listOutStorageInfo);
                _wayBillInfoRepository.BulkInsert("CustomerOrderStatuses", listCustomerOrderStatus);

                if (listWaybillSend.Any())
                {
                    _wayBillInfoRepository.Modify(w => new WayBillInfo()
                        {
                            OutShippingMethodID = outShippingMethodId,
                            OutShippingMethodName = outShippingMethodName,
                            OutStorageID = createOutStorageExt.OutStorage.OutStorageID,
                            VenderCode = createOutStorageExt.OutStorage.VenderCode,
                            LastUpdatedBy = _workContext.User.UserUame,
                            LastUpdatedOn = outStorageCreatedOn,
                            OutStorageCreatedOn = outStorageCreatedOn,
                            Status = WayBill.StatusToValue(WayBill.StatusEnum.Send),
                        }, w => listWaybillSend.Contains(w.WayBillNumber));
                }


                if (listWaybillWaitOrder.Any())
                {
                    _wayBillInfoRepository.Modify(w => new WayBillInfo()
                        {
                            OutShippingMethodID = outShippingMethodId,
                            OutShippingMethodName = outShippingMethodName,
                            OutStorageID = createOutStorageExt.OutStorage.OutStorageID,
                            VenderCode = createOutStorageExt.OutStorage.VenderCode,
                            LastUpdatedBy = _workContext.User.UserUame,
                            LastUpdatedOn = outStorageCreatedOn,
                            OutStorageCreatedOn = outStorageCreatedOn,
                            Status = WayBill.StatusToValue(WayBill.StatusEnum.WaitOrder),
                        }, w => listWaybillWaitOrder.Contains(w.WayBillNumber));

                }

                if (listCustomerOrderId.Any())
                {
                    _customerOrderInfoRepository.Modify(c => new CustomerOrderInfo()
                        {
                            Status = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Send),
                            LastUpdatedBy = _workContext.User.UserUame,
                            LastUpdatedOn = outStorageCreatedOn,
                        }, c => listCustomerOrderId.Contains(c.CustomerOrderID));

                }
                //_wayBillInfoRepository.UnitOfWork.Commit();

                _mailPostBagInfoRepository.UnitOfWork.Commit();

                transaction.Complete();
            }
            //创建总包号
            if (createOutStorageExt.IsCreateTotalPackageNumber.HasValue)
            {
                if (createOutStorageExt.IsCreateTotalPackageNumber.Value)
                {
                    //创建新的
                    var totalPackageInfo = new TotalPackageInfo
                        {
                            CreatedBy = _workContext.User.UserUame,
                            LastUpdatedBy = _workContext.User.UserUame,
                            CreatedOn = outStorageCreatedOn,
                            LastUpdatedOn = outStorageCreatedOn,
                            TotalPackageNumber = createOutStorageExt.TotalPackageNumber,
                            Remark = createOutStorageExt.Remark,
                            TotalQty = createOutStorageExt.TotalQty,
                            TotalVotes = createOutStorageExt.TotalVotes,
                            TotalWeight = createOutStorageExt.TotalWeight,
                            VenderCode = createOutStorageExt.OutStorage.VenderCode,
                            VenderName = createOutStorageExt.OutStorage.VenderName
                        };
                    _totalPackageInfoRepository.Add(totalPackageInfo);

                    var relational = new TotalPackageOutStorageRelationalInfo
                    {
                        OutStorageID = createOutStorageExt.OutStorage.OutStorageID,
                        TotalPackageNumber = createOutStorageExt.TotalPackageNumber,
                        CreatedOn = DateTime.Now
                    };
                    _totalPackageOutStorageRelationalInfoRepository.Add(relational);

                }
                else
                {
                    ////追加到已存在的总包号里面
                    var totalPackageInfo =
                        _totalPackageInfoRepository.Single(
                            p =>
                            p.TotalPackageNumber == createOutStorageExt.TotalPackageNumber &&
                            p.VenderCode == createOutStorageExt.OutStorage.VenderCode);
                    if (totalPackageInfo != null)
                    {
                        totalPackageInfo.TotalQty += createOutStorageExt.TotalQty;
                        totalPackageInfo.TotalVotes += createOutStorageExt.TotalVotes;
                        totalPackageInfo.TotalWeight += createOutStorageExt.TotalWeight;
                        totalPackageInfo.LastUpdatedBy = _workContext.User.UserUame;
                        totalPackageInfo.LastUpdatedOn = outStorageCreatedOn;
                        _totalPackageInfoRepository.Modify(totalPackageInfo);
                        var relational = new TotalPackageOutStorageRelationalInfo
                        {
                            OutStorageID = createOutStorageExt.OutStorage.OutStorageID,
                            TotalPackageNumber = createOutStorageExt.TotalPackageNumber,
                            CreatedOn = DateTime.Now
                        };
                        _totalPackageOutStorageRelationalInfoRepository.Add(relational);
                    }
                }
                using (var transaction = new TransactionScope())
                {
                    _totalPackageInfoRepository.UnitOfWork.Commit();
                    _totalPackageOutStorageRelationalInfoRepository.UnitOfWork.Commit();
                    transaction.Complete();
                }
            }

        }

        public OutStorageInfo GetOutStorageInfo(string outStorageId)
        {
            Check.Argument.IsNullOrWhiteSpace(outStorageId, "出仓单号");
            return _outStorageInfoRepository.First(p => p.OutStorageID == outStorageId);
        }

        public IPagedList<OutStorageInfoExt> GetOutStoragePagedList(OutStorageListSearchParam param)
        {
            var re = _outStorageInfoRepository.GetOutStoragePagedList(param);
            //GetPostBagNumber(re);
            return re;
        }

        //private void GetPostBagNumber(IPagedList<OutStorageInfo> re)
        //{
        //    if (re == null) return;
        //    if (re.InnerList == null) return;
        //    if (re.InnerList.Count <= 0) return;

        //    try
        //    {
        //        var ids = re.InnerList.Select(t => t.OutStorageID).ToList();
        //        var list = _mailPostBagInfoRepository
        //            .GetFiltered(t => ids.Contains(t.OutStorageID))
        //            .Select(t => new KeyValuePair<string, string>(t.OutStorageID, t.PostBagNumber))
        //            .ToList();

        //        foreach (var kv in list)
        //        {
        //            var mm = re.InnerList.FirstOrDefault(t => t.OutStorageID == kv.Key);
        //            if (mm != null)
        //                mm.PostBagNumber = kv.Value;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Exception(ex);
        //    }
        //}

        public List<WayBillInfo> GetWayBillInfoListByWayBillNumber(List<string> WayBillNumbers)
        {
            return _wayBillInfoRepository.GetList(p => WayBillNumbers.Contains(p.WayBillNumber)).ToList();
        }

        /// <summary>
        /// 导出出仓运单信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IList<ExportOutStorageInfo> GetExportOutStorageInfo(OutStorageListParam param)
        {
            return _wayBillInfoRepository.GetExportOutStorageInfo(param);
        }

        public WayBillInfo GetWayBillInfoByWayBillNumber(string wayBillNumber)
        {
            return _wayBillInfoRepository.First(p => p.WayBillNumber == wayBillNumber);
        }

        public List<VenderFeeLog> GetNoPriceOutStorageWayBillList()
        {
            Expression<Func<VenderFeeLog, bool>> filter = p => true;
            filter = filter.And(p => p.Freight == 0);
            return _venderFeeLogRepository.GetList(filter).ToList();
        }

        public void UpdateErrorRemark(string wayBillNumber, string message)
        {
            var venderFeeLog = _venderFeeLogRepository.First(p => p.WayBillNumber == wayBillNumber);
            if (venderFeeLog == null || venderFeeLog.WayBillNumber != wayBillNumber) return;
            venderFeeLog.Remark = message;
            _venderFeeLogRepository.Modify(venderFeeLog);
            _venderFeeLogRepository.UnitOfWork.Commit();
        }

        public void UpdateVenderPrice(string wayBillNumber, PriceProviderResult result)
        {
            if (result == null || !result.CanShipping) return;
            var venderLog = _venderFeeLogRepository.First(p => p.WayBillNumber == wayBillNumber);
            if (venderLog == null) return;
            venderLog.Freight = result.ShippingFee;//运费
            venderLog.FuelCharge = result.FuelFee;//燃油费
            venderLog.Register = result.RegistrationFee;//挂号费
            venderLog.SettleWeight = result.Weight;
            venderLog.Surcharge = result.RemoteAreaFee + result.OtherFee;//附加费
            venderLog.Remark = "";
            _venderFeeLogRepository.Modify(venderLog);
            _venderFeeLogRepository.UnitOfWork.Commit();
        }

        public VenderPackageModel GetVenderPriceModel(string wayBillNumber)
        {
            var wayBillInfo = _wayBillInfoRepository.First(p => p.WayBillNumber == wayBillNumber);
            var venderLog = _venderFeeLogRepository.First(p => p.WayBillNumber == wayBillNumber);
            if (wayBillInfo != null && venderLog != null)
            {
                return new VenderPackageModel()
                    {
                        Code = wayBillInfo.OutStorageInfo.VenderCode,
                        CountryCode = wayBillInfo.CountryCode,
                        Height = wayBillInfo.Height ?? 0,
                        Weight = wayBillInfo.Weight ?? 0,
                        Length = wayBillInfo.Length ?? 0,
                        Width = wayBillInfo.Width ?? 0,
                        ShippingMethodId = wayBillInfo.OutShippingMethodID ?? 0,
                        ShippingTypeId = venderLog.GoodsTypeID
                    };
            }
            return null;
        }

        public bool UpdateOutStoragePrice(List<string> wayBillNumbers)
        {
            return _wayBillInfoRepository.UpdateOutStoragePrice(wayBillNumbers);
        }

        #region 绑定渠道发货配置

        /// <summary>
        /// 查询渠道发货配置
        /// Add By zhengsong
        /// Time:2014-08-28
        /// </summary>
        /// <param name="inShippingMethodId"></param>
        /// <returns></returns>
        public List<DeliveryChannelConfiguration> GetDeliveryChannelConfigurations(int inShippingMethodId)
        {
            return _deliveryChannelConfigurationRepository.GetList(p => p.InShippingMethodId == inShippingMethodId).ToList();
        }

        /// <summary>
        /// 添加渠道发货配置
        /// Add By zhengsong
        /// Time:2014-08-28
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddDeliveryChannelConfiguration(DeliveryChannelConfiguration model)
        {
            bool result = false;
            try
            {
                var delivery = _deliveryChannelConfigurationRepository.GetList(p =>
                        p.InShippingMethodId == model.InShippingMethodId &&
                        p.OutShippingMethodId == model.OutShippingMethodId &&
                        p.VenderId == model.VenderId).FirstOrDefault();
                if (model != null && delivery == null)
                {
                    _deliveryChannelConfigurationRepository.Add(model);
                    _deliveryChannelConfigurationRepository.UnitOfWork.Commit();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 删除渠道发货配置
        /// Add By zhengsong
        /// Time:2014-08-28
        /// </summary>
        /// <param name="deliveryChannelConfigurationId"></param>
        /// <returns></returns>
        public bool DeleteDeliveryChannelConfiguration(int deliveryChannelConfigurationId)
        {
            bool result = false;
            try
            {
                var deleteDeliveryChannelConfigurations = _deliveryChannelConfigurationRepository.Get(deliveryChannelConfigurationId);
                _deliveryChannelConfigurationRepository.Remove(deleteDeliveryChannelConfigurations);
                _deliveryChannelConfigurationRepository.UnitOfWork.Commit();
                result = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 查询单个渠道发货配置
        /// Add By zhengsong
        /// Time:2014-08-28
        /// </summary>
        /// <param name="inShippingMethodId"></param>
        /// <param name="venderId"></param>
        /// <param name="outShippingMethodId"></param>
        /// <returns></returns>
        public DeliveryChannelConfiguration GetDeliveryChannelConfiguration(int inShippingMethodId, int venderId, int outShippingMethodId)
        {
            return _deliveryChannelConfigurationRepository.GetList(p => p.InShippingMethodId == inShippingMethodId && p.VenderId == venderId && p.OutShippingMethodId == outShippingMethodId).FirstOrDefault();
        }

        #endregion

        #region 出仓后修改出仓渠道
        /// <summary>
        /// 是否可以整批的修改出仓渠道
        /// </summary>
        /// <param name="outStorageID"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        public bool IsUpdateOutStorage(string outStorageID, int qty)
        {
            bool reuslt = false;
            const int ReGoodsInStorage = (int)WayBill.StatusEnum.ReGoodsInStorage;
            const int ReturnGoodsInStorage = (int)WayBill.StatusEnum.Return;
            //查询出该批出仓单号的运单
            try
            {
                List<string> wayBillList = new List<string>();
                var wayBillLists = _wayBillInfoRepository.GetList(p => p.OutStorageID == outStorageID && p.Status != ReGoodsInStorage && p.Status != ReturnGoodsInStorage);
                wayBillLists.ForEach(p => wayBillList.Add(p.WayBillNumber));
                //判断是否存在状态不一样的单
                if (wayBillList.Count == qty)
                {
                    // 找出是否有运单已经计算出费用
                    var deliveryFee = _deliveryFeeRepository.GetList(p => wayBillList.Contains(p.WayBillNumber));
                    List<int> deliveryFeeId = new List<int>();
                    deliveryFee.ForEach(p => deliveryFeeId.Add(p.DeliveryFeeID));
                    var deliveryFeeInfo = _deliveryFeeInfoRepository.GetFiltered(p => deliveryFeeId.Contains(p.DeliveryFeeID.Value)).FirstOrDefault();
                    if (deliveryFeeInfo == null)
                    {
                        reuslt = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return reuslt;
        }

        public List<DeliveryChannelConfiguration> GetOutStorageShippingMethods(int venderId)
        {
            return _deliveryChannelConfigurationRepository.GetList(p => p.VenderId == venderId);
        }

        //修改运单出仓运输方式，修改出仓信息
        // Add By zhengosng
        //Time:2014-09-11
        public ResponseResult UpdateOutStorageInfo(string outStorageId, int outshippingMethodId, string outshippingMethodName, string venderCode, string remark)
        {
            var result = new ResponseResult();
            result.Result = false;
            try
            {
                var wayBillList = _wayBillInfoRepository.GetList(p => p.OutStorageID == outStorageId);
                var vender = _freightService.GetVender(venderCode);
                //判断是否配置了相应的出仓渠道
                if (vender != null)
                {
                    int inShippingMethodId = wayBillList[0].InShippingMethodID ?? 0;
                    var deliveryChannelConfiguration = _deliveryChannelConfigurationRepository.GetList(p => p.InShippingMethodId == inShippingMethodId && p.VenderId == vender.VenderId && p.OutShippingMethodId == outshippingMethodId).FirstOrDefault();

                    if (deliveryChannelConfiguration == null)
                    {
                        result.Result = false;
                        result.Message = wayBillList[0].InShippingMethodName + "未配置相应的发货运输渠道！";
                        return result;
                    }
                }
                else
                {
                    result.Result = false;
                    result.Message = "服务商出差！";
                    return result;
                }

                //修改运单出仓运输方式
                wayBillList.ForEach(p =>
                    {
                        p.OutShippingMethodID = outshippingMethodId;
                        p.OutShippingMethodName = outshippingMethodName;
                        p.VenderCode = venderCode;
                        _wayBillInfoRepository.Modify(p);
                    });
                //修改出仓信息
                var outStorageInfo = _outStorageInfoRepository.Get(outStorageId);
                if (outStorageInfo != null && vender != null)
                {
                    outStorageInfo.LastUpdatedBy = _workContext.User.UserUame;
                    outStorageInfo.LastUpdatedOn = DateTime.Now;
                    outStorageInfo.VenderCode = vender.Code;
                    outStorageInfo.VenderName = vender.Name;
                    outStorageInfo.VenderName = remark;
                    _outStorageInfoRepository.Modify(outStorageInfo);
                }
                _wayBillInfoRepository.UnitOfWork.Commit();
                _outStorageInfoRepository.UnitOfWork.Commit();
                result.Result = true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                result.Result = false;
            }
            return result;
        }

        //收货页面运单出仓运输方式，修改出仓信息
        public ResponseResult UpdateOutStorageInfoAll(string wayBillLists, int outshippingMethodId, string outshippingMethodName, string venderCode, string remark, out List<string> outStorageIds)
        {
            var result = new ResponseResult();
            result.Result = false;
            List<string> aList = new List<string>();
            try
            {
                var wayBillList = wayBillLists.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var newwayBillList = _wayBillInfoRepository.GetList(p => wayBillList.Contains(p.WayBillNumber));
                var vender = _freightService.GetVender(venderCode);

                //修改之前出仓Id
                newwayBillList.ForEach(p =>
                {
                    if (!aList.Contains(p.OutStorageID))
                    {
                        aList.Add(p.OutStorageID);
                    }
                });
                //判断是否配置了相应的出仓渠道
                var deliveryChannelConfiguration = _deliveryChannelConfigurationRepository.GetList(p => p.VenderId == vender.VenderId && p.OutShippingMethodId == outshippingMethodId);
                List<int> inShippingMethodIds = new List<int>();
                deliveryChannelConfiguration.ForEach(p => inShippingMethodIds.Add(p.InShippingMethodId));
                List<string> errorinShippingMethod = new List<string>();
                newwayBillList.ForEach(p =>
                    {
                        if (p.InShippingMethodID == null || !inShippingMethodIds.Contains(p.InShippingMethodID.Value))
                        {
                            if (!errorinShippingMethod.Contains(p.InShippingMethodName))
                            {
                                errorinShippingMethod.Add(p.InShippingMethodName);
                            }
                        }
                    });
                if (errorinShippingMethod.Count > 0)
                {
                    result.Result = false;
                    errorinShippingMethod.ForEach(p =>
                        {
                            result.Message += "[" + p + "]";
                        });
                    result.Message += "未配置相应的发货运输渠道！";
                    outStorageIds = null;
                    return result;
                }
                //创建新出仓信息
                OutStorageInfo outStorage = new OutStorageInfo();
                outStorage.OutStorageID = SequenceNumberService.GetSequenceNumber(PrefixCode.OutStorageID);
                outStorage.VenderCode = venderCode;
                if (vender != null)
                {
                    outStorage.VenderName = vender.Name;
                }
                outStorage.Freight = 0;
                outStorage.FuelCharge = 0;
                outStorage.Register = 0;
                outStorage.TotalFee = 0;

                outStorage.TotalQty = newwayBillList.Count;
                newwayBillList.ForEach(p =>
                    {
                        outStorage.TotalWeight += p.SettleWeight ?? 0;
                    });
                outStorage.Surcharge = 0;
                outStorage.Status = 1;
                outStorage.Remark = remark;
                outStorage.CreatedBy = _workContext.User.UserUame;
                outStorage.CreatedOn = DateTime.Now;
                outStorage.LastUpdatedBy = _workContext.User.UserUame;
                outStorage.LastUpdatedOn = DateTime.Now;

                _outStorageInfoRepository.Add(outStorage);
                _outStorageInfoRepository.UnitOfWork.Commit();


                //修改运单出仓运输方式
                newwayBillList.ForEach(p =>
                {
                    p.OutShippingMethodID = outshippingMethodId;
                    p.OutShippingMethodName = outshippingMethodName;
                    p.OutStorageID = outStorage.OutStorageID;
                    p.VenderCode = venderCode;
                    p.OutStorageCreatedOn = DateTime.Now;
                    p.LastUpdatedBy = _workContext.User.UserUame;
                    p.LastUpdatedOn = DateTime.Now;
                    _wayBillInfoRepository.Modify(p);
                });
                _wayBillInfoRepository.UnitOfWork.Commit();
                result.Result = true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                result.Result = false;
            }
            outStorageIds = aList;
            return result;
        }


        //修改原来出仓信息
        public bool UpdateOldOutStorageInfo(List<string> outStorageIdList)
        {
            bool result = false;
            try
            {
                var outStorageList = _outStorageInfoRepository.GetList(p => outStorageIdList.Contains(p.OutStorageID));

                outStorageList.ForEach(p =>
                    {
                        var wayBillList = _wayBillInfoRepository.GetList(z => z.OutStorageID == p.OutStorageID);
                        p.TotalQty = wayBillList.Count;
                        p.TotalWeight = wayBillList.Sum(x => x.SettleWeight);
                        p.LastUpdatedBy = _workContext.User.UserUame;
                        p.LastUpdatedOn = DateTime.Now;
                        _outStorageInfoRepository.Modify(p);
                    });
                _outStorageInfoRepository.UnitOfWork.Commit();
                result = true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                result = false;
            }
            return result;
        }

        #endregion

        public string CreateTotalPackageNumber()
        {
            return SequenceNumberService.GetSequenceNumber(PrefixCode.TotalPackageID);
        }

        #region 总包号
        public void GetWayBillSummary(List<string> wayBillNumbers, out int totalQty, out decimal totalWeight)
        {
            if (wayBillNumbers != null && wayBillNumbers.Count > 0)
            {
                var wayBillList = _waybillPackageDetailRepository.GetList(w => wayBillNumbers.Contains(w.WayBillNumber));
                totalQty = wayBillList.Count;
                totalWeight = wayBillList.Sum(w => w.Weight ?? 0 + w.AddWeight ?? 0);
            }
            else
            {
                totalQty = 0;
                totalWeight = 0;
            }
        }

        public void GetWayBillTotalQty(List<string> wayBillNumbers, out int totalQty)
        {
            if (wayBillNumbers != null && wayBillNumbers.Count > 0)
            {
                totalQty = Int32.Parse(_waybillPackageDetailRepository.Count(w => wayBillNumbers.Contains(w.WayBillNumber)).ToString());
            }
            else
            {
                totalQty = 0;
            }
        }

        public void GetWayBillTotalWeight(List<string> wayBillNumbers, out decimal totalWeight)
        {
            if (wayBillNumbers != null && wayBillNumbers.Count > 0)
            {
                totalWeight = _waybillPackageDetailRepository.GetWayBillListWeight(wayBillNumbers);
            }
            else
            {
                totalWeight = 0;
            }
        }

        public List<string> GetTotalPackageNumberList(string venderCode)
        {
            return _outStorageInfoRepository.GetTotalPackageNumberList(venderCode);
        }

        public IPagedList<TotalPackageInfoExt> GetTotalPackageList(EditTotalPackageTimeParam param)
        {
            var result = new PagedList<TotalPackageInfoExt>
            {
                PageIndex = param.Page,
                PageSize = param.PageSize,
                TotalCount = 0,
                TotalPages = 0,
                InnerList = new List<TotalPackageInfoExt>()
            };
            var startTime = param.StartTime.HasValue ? param.StartTime.Value : new DateTime(2013, 1, 1);
            var endTime = param.EndTime.HasValue ? param.EndTime.Value : new DateTime(2020, 1, 1);
            var numberList = new List<string>();
            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberList =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();
            }
            Expression<Func<TotalPackageInfo, bool>> filter = p => true;
            if (numberList.Count > 0)
            {
                filter = filter.And(p => numberList.Contains(p.TotalPackageNumber));
            }
            else
            {
                filter = filter.And(p => p.CreatedOn >= startTime && p.CreatedOn <= endTime)
                               .AndIf(p => p.VenderCode == param.VenderCode, !param.VenderCode.IsNullOrWhiteSpace())
                               .AndIf(p => p.CreatedBy.Contains(param.CreateBy), !param.CreateBy.IsNullOrWhiteSpace());
            }
            Func<IQueryable<TotalPackageInfo>, IOrderedQueryable<TotalPackageInfo>>
                orderBy = o => o.OrderByDescending(p => p.CreatedOn);
            var list = _totalPackageInfoRepository.FindPagedList(param.Page, param.PageSize, filter, orderBy);
            if (list.Any())
            {
                result.PageIndex = list.PageIndex;
                result.PageSize = list.PageSize;
                result.TotalCount = list.TotalCount;
                result.TotalPages = list.TotalPages;
                var totalPackageNumberlist = list.InnerList.Select(p => p.TotalPackageNumber).ToList();
                var infolist =
                    _totalPackageTraceInfoRepository.GetList(s => totalPackageNumberlist.Contains(s.TotalPackageNumber));


                list.InnerList.ForEach(p => result.InnerList.Add(new TotalPackageInfoExt()
                {
                    Info = p,
                    TraceInfos = infolist.Where(s => s.TotalPackageNumber.Trim() == p.TotalPackageNumber.Trim()).ToList()
                }));
            }
            return result;
        }

        public TotalPackageAddressExt GetTotalPackageAddress()
        {
            var result = new TotalPackageAddressExt();
            var xml =
                _systemConfigurationRepository.Single(s => s.DictionaryID == sysConfig.TotalPackageAddress && s.IsEnable);
            if (xml != null && !xml.ConfigurationValue.IsNullOrWhiteSpace())
            {
                try
                {
                    result = SerializeUtil.DeserializeFromXml<TotalPackageAddressExt>(xml.ConfigurationValue);
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
            }
            return result;
        }

        public List<TotalPackageTraceInfo> GetTotalPackageTraceInfo(string totalPackageNumber)
        {
            return _totalPackageTraceInfoRepository.GetList(p => p.TotalPackageNumber == totalPackageNumber);
        }

        public void EditTotalPackageTraceTime(List<TotalPackageTraceInfo> model)
        {
            foreach (var p in model)
            {
                var t =
                    _totalPackageTraceInfoRepository.Single(
                        s => s.TotalPackageNumber == p.TotalPackageNumber && s.TraceEventCode == p.TraceEventCode);
                if (t != null)
                {
                    if ((t.TraceEventTime != p.TraceEventTime || t.TraceEventAddress != p.TraceEventAddress) &&
                        !t.IsJob)
                    {
                        t.TraceEventTime = p.TraceEventTime;
                        t.TraceEventAddress = p.TraceEventAddress;
                        t.LastUpdatedBy = _workContext.User.UserUame;
                        t.LastUpdatedOn = DateTime.Now;
                        _totalPackageTraceInfoRepository.Modify(t);
                    }
                }
                else
                {
                    _totalPackageTraceInfoRepository.Add(new TotalPackageTraceInfo()
                    {
                        CreatedBy = _workContext.User.UserUame,
                        CreatedOn = DateTime.Now,
                        IsJob = false,
                        LastUpdatedBy = _workContext.User.UserUame,
                        LastUpdatedOn = DateTime.Now,
                        TotalPackageNumber = p.TotalPackageNumber.Trim(),
                        TraceEventAddress = p.TraceEventAddress.Trim(),
                        TraceEventCode = p.TraceEventCode,
                        TraceEventContent = "",
                        TraceEventTime = p.TraceEventTime
                    });
                }
            }
            _totalPackageTraceInfoRepository.UnitOfWork.Commit();
        }

        public DateTime GetTotalPackageTraceLastTime(string totalPackageNumber)
        {
            return _totalPackageOutStorageRelationalInfoRepository.GetFiltered(p => p.TotalPackageNumber == totalPackageNumber)
                                                          .Max(p => p.CreatedOn);
        }

        public void DeleteTotalPackageTraceTime(List<TotalPackageTraceInfo> model)
        {
            foreach (var p in model)
            {
                if (p.TotalPackageNumber.IsNullOrWhiteSpace())
                    break;
                var t =
                    _totalPackageTraceInfoRepository.Single(
                        s => s.TotalPackageNumber == p.TotalPackageNumber && s.TraceEventCode == p.TraceEventCode);
                if (t != null && !t.IsJob)
                {
                    _totalPackageTraceInfoRepository.Remove(t);
                }
            }
            _totalPackageTraceInfoRepository.UnitOfWork.Commit();
        }

        public PagedList<B2CPreAlterExt> GetB2CPreAlterExtList(B2CPreAlterListParam param)
        {
            return _b2CPreAlertLogsRepository.GetB2CPreAlterExtList(param);
        }

        public bool PreAlterB2CBySearch(B2CPreAlterListParam param)
        {
            return _b2CPreAlertLogsRepository.PreAlterB2CBySearch(param);
        }

        public bool PreAlterB2CByWayBillNumber(List<string> wayBillNumbers)
        {
            return _b2CPreAlertLogsRepository.PreAlterB2CByWayBillNumber(wayBillNumbers);
        }

        #endregion
    }

}
