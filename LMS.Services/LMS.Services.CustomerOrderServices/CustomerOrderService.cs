using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using System.Text;
using System.Threading;
using System.Transactions;
using LMS.Core;
using LMS.Data.Entity;

using LMS.Data.Repository;
using LMS.Services.FreightServices;
using LMS.Services.OrderServices;
using LMS.Services.SequenceNumber;
using LMS.Services.TrackingNumberServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;
using Lightake.Common;
using Lightake.UEB.API;
using BusinessLogicException = LighTake.Infrastructure.Common.BusinessLogicException;
using LighTake.Infrastructure.CommonQueue;

namespace LMS.Services.CustomerOrderServices
{
    public class CustomerOrderService : ICustomerOrderService
    {
        private readonly ICustomerOrderInfoRepository _customerOrderInfoRepository;
        private readonly ICustomerOrderStatusRepository _statusRepository;
        private readonly IApplicationInfoRepository _applicationInfoRepository;
        private readonly IWayBillInfoRepository _wayBillInfoRepository;
        private readonly IEubWayBillApplicationInfoRepository _eubWayBillApplicationInfoRepository;
        private readonly IFreightService _freightService;
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly IEubAccountInfoRepository _eubAccountInfoRepository;
        private readonly ITrackingNumberDetailInfoRepository _trackingNumberDetailInfoRepository;
        private readonly ITrackingNumberService _trackingNumberService;
        private readonly IWayBillEventLogRepository _wayBillEventLogRepository;
        private readonly IEUBService _eubService = new EUBService();
        private static object lockCreateWayBillInfo = new object();

        public CustomerOrderService(IWorkContext workContext,
            ICustomerOrderInfoRepository customerOrderInfoRepository,
            ICustomerOrderStatusRepository statusRepository,
            IApplicationInfoRepository applicationInfoRepository,
            IWayBillInfoRepository wayBillInfoRepository,
            IOrderService orderService,
            IFreightService freightService,
            ITrackingNumberService trackingNumberService,
            IEubWayBillApplicationInfoRepository eubWayBillApplicationInfoRepository,
            IEubAccountInfoRepository eubAccountInfoRepository, ITrackingNumberDetailInfoRepository trackingNumberDetailInfoRepository,
            IWayBillEventLogRepository wayBillEventLogRepository)
        {
            _customerOrderInfoRepository = customerOrderInfoRepository;
            _statusRepository = statusRepository;
            _applicationInfoRepository = applicationInfoRepository;
            _wayBillInfoRepository = wayBillInfoRepository;
            _orderService = orderService;
            _workContext = workContext;
            _freightService = freightService;
            _trackingNumberService = trackingNumberService;
            _eubAccountInfoRepository = eubAccountInfoRepository;
            _eubWayBillApplicationInfoRepository = eubWayBillApplicationInfoRepository;
            _trackingNumberDetailInfoRepository = trackingNumberDetailInfoRepository;
            _wayBillEventLogRepository = wayBillEventLogRepository;
        }

        public bool Add(CustomerOrderInfo orderInfo)
        {

            BatchAdd(new List<CustomerOrderInfo> { orderInfo });

            return true;
        }

        /*private void AddItem(CustomerOrderInfo orderInfo)
        {
            Check.Argument.IsNotNull(orderInfo, "CustomerOrderInfo");
            Check.Argument.IsNotNull(orderInfo.ShippingInfo, "CustomerOrderInfo.ShippingInfo发货地址");
            Check.Argument.IsNotNull(orderInfo.ApplicationInfos, "CustomerOrderInfo.ApplicationInfos申报信息");

            orderInfo.Status = CustomerOrder.StatusEnum.None.GetStatusValue();

            orderInfo.CustomerOrderStatuses.Add(new CustomerOrderStatus { Status = CustomerOrder.StatusEnum.None.GetStatusValue(), CreatedOn = orderInfo.CreatedOn, Remark = "客户创建" });

            _customerOrderInfoRepository.Add(orderInfo);
        }*/

        /// <summary>
        /// 批量添加订单信息
        /// </summary>
        /// <param name="orderInfos"></param>
        /// <returns></returns>
        public bool BatchAdd(List<CustomerOrderInfo> orderInfos)
        {
            _customerOrderInfoRepository.BatchAdd(orderInfos);
            return true;
        }



        //public bool BatchAdd(List<WayBillInfo> wayBillInfos)
        //{
        //    _customerOrderInfoRepository.BatchAdd(wayBillInfos);

        //    return true;
        //}

        public bool Moditfy(CustomerOrderInfo orderInfo)
        {
            Check.Argument.IsNotNull(orderInfo, "CustomerOrderInfo");
            Check.Argument.IsNotNull(orderInfo.ShippingInfo, "CustomerOrderInfo.ShippingInfo发货地址");
            Check.Argument.IsNotNull(orderInfo.ApplicationInfos, "CustomerOrderInfo.ApplicationInfos申报信息");

            _customerOrderInfoRepository.Modify(orderInfo);

            _customerOrderInfoRepository.UnitOfWork.Commit();

            return true;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            var orderInfo = Get(id);

            if (orderInfo != null && IsCustomerOrderDelete(orderInfo.Status))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    var listStatuses = orderInfo.CustomerOrderStatuses.ToList();

                    foreach (var item in listStatuses)
                    {
                        _statusRepository.Remove(item);
                    }

                    var listProducts = orderInfo.ApplicationInfos.ToList();

                    foreach (var item in listProducts)
                    {
                        _applicationInfoRepository.Remove(item);
                    }

                    _customerOrderInfoRepository.Remove(orderInfo);

                    _customerOrderInfoRepository.UnitOfWork.Commit();
                    transaction.Complete();

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 取消客户订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Cancel(int id)
        {
            var orderInfo = Get(id);

            if (orderInfo != null && IsCustomerOrderCancel(orderInfo.Status))
            {
                var status = CustomerOrder.StatusEnum.None.GetStatusValue();

                orderInfo.Status = status;
                orderInfo.LastUpdatedBy = _workContext.User.UserUame;
                orderInfo.LastUpdatedOn = DateTime.Now;
                orderInfo.CustomerOrderStatuses.Add(new CustomerOrderStatus { Status = status, CreatedOn = orderInfo.LastUpdatedOn, Remark = "客户取消" });
                _customerOrderInfoRepository.Modify(orderInfo);

                _customerOrderInfoRepository.UnitOfWork.Commit();

                return true;
            }
            return false;
        }

        /// <summary>
        /// 取肖客户订单(批量)
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool BatchCancel(List<int> ids)
        {

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                foreach (var id in ids)
                {
                    var orderInfo = Get(id);
                    if (null == orderInfo) return false;
                    if (IsCustomerOrderCancel(orderInfo.Status))
                    {
                        var status = CustomerOrder.StatusEnum.None.GetStatusValue();

                        orderInfo.Status = status;
                        orderInfo.LastUpdatedBy = _workContext.User.UserUame;
                        orderInfo.LastUpdatedOn = DateTime.Now;
                        orderInfo.CustomerOrderStatuses.Add(new CustomerOrderStatus
                            {
                                Status = status,
                                CreatedOn = orderInfo.LastUpdatedOn,
                                Remark = "客户取消"
                            });
                        _customerOrderInfoRepository.Modify(orderInfo);
                    }
                }
                _customerOrderInfoRepository.UnitOfWork.Commit();
                transaction.Complete();
                return true;
            }
        }



		/// <summary>
		/// 删除客户订单(订单)
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		public bool BatchDelete(List<int> ids)
		{
			using (var transaction = new TransactionScope(TransactionScopeOption.Required))
			{
				foreach (var id in ids)
				{
					var orderInfo = Get(id);
					if (null == orderInfo) return false;
					if (IsCustomerOrderSubmitFail(orderInfo.Status))
					{
						var status = CustomerOrder.StatusEnum.Delete.GetStatusValue();
						orderInfo.Status = status;
						orderInfo.LastUpdatedBy = _workContext.User.UserUame;
						orderInfo.LastUpdatedOn = DateTime.Now;
						orderInfo.CustomerOrderStatuses.Add(new CustomerOrderStatus
						{
							Status = status,
							CreatedOn = orderInfo.LastUpdatedOn,
							Remark = "客户删除"
						});
						_customerOrderInfoRepository.Modify(orderInfo);
					}
				}
				_customerOrderInfoRepository.UnitOfWork.Commit();
				transaction.Complete();
				return true;
			}
		}



        public CustomerOrderInfo Get(int customerOrderId)
        {
            return _customerOrderInfoRepository.Get(customerOrderId);

        }

        public List<CustomerOrderInfo> GetListByCustomerOrderId(List<int> ids,int status)
        {
            return _customerOrderInfoRepository.GetList(p => ids.Contains(p.CustomerOrderID) && p.Status == status);
        }

        public List<CustomerOrderInfo> GetListByCustomerOrderId(List<int> ids)
        {
            return _customerOrderInfoRepository.GetList(p => ids.Contains(p.CustomerOrderID));
        }

        public List<CustomerOrderInfo> GetCustomerOrderInfos(CustomerOrderParam param)
        {
            int deleteSatus = CustomerOrder.StatusEnum.Delete.GetStatusValue();
            Expression<Func<CustomerOrderInfo, bool>> filter = o => o.Status != deleteSatus;
            filter.AndIf(o => o.IsHold == param.IsHold.Value, param.IsHold.HasValue);
            filter = filter.AndIf(o => o.Status == param.Status.Value, param.Status.HasValue);

            if (!param.WayBillNumber.IsNullOrWhiteSpace())
            {
                var numberList = param.WayBillNumber.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();

                if (numberList.Count > 0)
                {
                    string number = numberList[0];
                    List<int> customerOrderIds = new List<int>();
                    customerOrderIds = _wayBillInfoRepository.GetCustomerId(numberList);
                    if (customerOrderIds.Count > 0)
                    {
                        int customerOrderId = customerOrderIds[0];
                        filter = customerOrderIds.Count == 1 ? filter.And(o => o.CustomerOrderID == customerOrderId) : filter.And(o => customerOrderIds.Contains(o.CustomerOrderID));
                    }
                }
            }
            else if (!param.CustomerOrderNumber.IsNullOrWhiteSpace())
            {
                var numberList = param.CustomerOrderNumber.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();

                if (numberList.Count > 0)
                {
                    string number = numberList[0];
                    filter = numberList.Count == 1 ? filter.And(o => o.CustomerOrderNumber == number) : filter.And(o => numberList.Contains(o.CustomerOrderNumber));
                }
            }
            else
            {
                filter = filter
                .AndIf(o => o.ShippingMethodId == param.ShippingMethodId.Value, param.ShippingMethodId.HasValue)
                    //.AndIf(o => o.CustomerOrderNumber == param.CustomerOrderNumber, !param.CustomerOrderNumber.IsNullOrWhiteSpace())
                .AndIf(o => o.ShippingInfo.CountryCode == param.CountryCode, !param.CountryCode.IsNullOrWhiteSpace())
                .AndIf(o => o.CreatedOn >= param.CreatedOnFrom.Value, param.CreatedOnFrom.HasValue)
                .AndIf(o => o.CreatedOn <= param.CreatedOnTo.Value, param.CreatedOnTo.HasValue)
                .AndIf(o => o.IsHold == param.IsHold.Value, param.IsHold.HasValue)
                    //.AndIf(o => o. >= param.CreatedOnFrom.Value, param.CreatedOnFrom.HasValue)
                .AndIf(o => o.CustomerCode == param.CustomerCode, !param.CustomerCode.IsNullOrWhiteSpace())
                .AndIf(o => o.IsPrinted == param.IsPrinted.Value, param.IsPrinted.HasValue);
            }
            return _customerOrderInfoRepository.GetList(filter);
        }

        public List<string> GetCustomerOrderInfos(List<string> customerOrderNumber)
        {
             var deleteStatus = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Delete);
            var returnStatus = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Return);
            return _customerOrderInfoRepository.GetList(p => customerOrderNumber.Contains(p.CustomerOrderNumber) && p.Status != deleteStatus && p.Status != returnStatus).Select(p=>p.CustomerOrderNumber).ToList();
        }
        public List<string> GetCustomerOrderInfoByTrack(List<string> trackingNumbers)
        {
            var deleteStatus = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Delete);
            var returnStatus = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Return);
            return
                _customerOrderInfoRepository.GetList(
                    p => p.TrackingNumber != null && p.TrackingNumber != "" && trackingNumbers.Contains(p.TrackingNumber) && p.Status != deleteStatus && p.Status != returnStatus).Select(p => p.TrackingNumber).ToList();
        }

        public List<string> GetCustomerOrderIdByWayBillNumber(List<string> wayBillNumbers)
        {
            List<string> customerOrderIds=new List<string>();
            var list = _wayBillInfoRepository.GetList(p => wayBillNumbers.Contains(p.WayBillNumber));
            list.ForEach(p => customerOrderIds.Add((p.CustomerOrderID??0).ToString()));
            return customerOrderIds;
        }

        public IPagedList<CustomerOrderInfoExt> GetList(CustomerOrderParam param, int maxCustomerOrderId = 0)
        {

            return _customerOrderInfoRepository.GetCustomerOrderInfoList(param,maxCustomerOrderId);
            //int deleteSatus = CustomerOrder.StatusEnum.Delete.GetStatusValue();
            //Expression<Func<CustomerOrderInfo, bool>> filter = o => o.Status != deleteSatus;
            //filter.AndIf(o => o.IsHold == param.IsHold.Value, param.IsHold.HasValue);
            ////if (param.IsReceived && !param.Status.HasValue)
            ////{
            ////    var statusList = new List<int>()
            ////        {
            ////            CustomerOrder.StatusEnum.None.GetStatusValue(),
            ////            CustomerOrder.StatusEnum.OK.GetStatusValue(),
            ////            CustomerOrder.StatusEnum.Submitted.GetStatusValue(),
            ////        };
            ////    filter = filter.And(o => !statusList.Contains(o.Status));
            ////}
            ////else
            ////{
            ////    filter = filter.AndIf(o => o.Status == param.Status.Value, param.Status.HasValue);
            ////}

            //filter = filter.AndIf(o => o.Status == param.Status.Value, param.Status.HasValue);

            //if (!param.WayBillNumber.IsNullOrWhiteSpace())
            //{
            //    var numberList = param.WayBillNumber.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
            //             .ToList();

            //    if (numberList.Count > 0)
            //    {
            //        string number = numberList[0];
            //        List<int> customerOrderIds = new List<int>();
            //        customerOrderIds= _wayBillInfoRepository.GetCustomerId(numberList);
            //        if (customerOrderIds.Count > 0)
            //        {
            //            int customerOrderId = customerOrderIds[0];
            //            filter = customerOrderIds.Count == 1 ? filter.And(o => o.CustomerOrderID == customerOrderId) : filter.And(o => customerOrderIds.Contains(o.CustomerOrderID));
            //        }
            //    }
            //}else if (!param.CustomerOrderNumber.IsNullOrWhiteSpace())
            //{
            //    var numberList = param.CustomerOrderNumber.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
            //             .ToList();

            //    if (numberList.Count > 0)
            //    {
            //        string number = numberList[0];
            //        filter = numberList.Count == 1 ? filter.And(o => o.CustomerOrderNumber == number) : filter.And(o => numberList.Contains(o.CustomerOrderNumber));
            //    }
            //}
            //else
            //{
            //    filter = filter
            //    .AndIf(o => o.ShippingMethodId == param.ShippingMethodId.Value, param.ShippingMethodId.HasValue)
            //        //.AndIf(o => o.CustomerOrderNumber == param.CustomerOrderNumber, !param.CustomerOrderNumber.IsNullOrWhiteSpace())
            //    .AndIf(o => o.ShippingInfo.CountryCode == param.CountryCode, !param.CountryCode.IsNullOrWhiteSpace())
            //    .AndIf(o => o.CreatedOn >= param.CreatedOnFrom.Value, param.CreatedOnFrom.HasValue)
            //    .AndIf(o => o.CreatedOn <= param.CreatedOnTo.Value, param.CreatedOnTo.HasValue)
            //    .AndIf(o => o.IsHold == param.IsHold.Value, param.IsHold.HasValue)
            //        //.AndIf(o => o. >= param.CreatedOnFrom.Value, param.CreatedOnFrom.HasValue)
            //    .AndIf(o => o.CustomerCode == param.CustomerCode, !param.CustomerCode.IsNullOrWhiteSpace())
            //    .AndIf(o => o.IsPrinted == param.IsPrinted.Value, param.IsPrinted.HasValue);
            //}
            //return _customerOrderInfoRepository.FindPagedList(param, filter, o =>o.OrderByDescending(p => p.CreatedOn));
            ////return _customerOrderInfoRepository.FindPagedList(param, filter, o => o.OrderByDescending(p => p.WayBillInfos.Any(w => w.Status != status) ? p.WayBillInfos.FirstOrDefault(w => w.Status != status).TransferOrderDate : null).ThenByDescending(p => p.CreatedOn));

        }

        /// <summary>
        /// Add by zhengsong
        /// </summary>
        /// <param name="param">订单信息导出</param>
        /// <returns></returns>
        public List<CustomerOrderInfoExportExt> GetCustomerOrderInfoExport(CustomerOrderParam param)
        {
            int deleteSatus = CustomerOrder.StatusEnum.Delete.GetStatusValue();
            Expression<Func<CustomerOrderInfo, bool>> filter = o => o.Status != deleteSatus;
            filter.AndIf(o => o.IsHold == param.IsHold.Value, param.IsHold.HasValue);
            filter = filter.AndIf(o => o.Status == param.Status.Value, param.Status.HasValue);

            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                var numberList =
                     param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                          .ToList();
                if (param.SearchWhere.HasValue && numberList.Count > 0)
                {
                    switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
                    {
                        case WayBill.SearchFilterEnum.WayBillNumber:
                            List<int> customerOrderIds = new List<int>();
                            customerOrderIds = _wayBillInfoRepository.GetCustomerId(numberList);
                            if (customerOrderIds.Count > 0)
                            {
                                int customerOrderId = customerOrderIds[0];
                                filter = customerOrderIds.Count == 1
                                             ? filter.And(o => o.CustomerOrderID == customerOrderId)
                                             : filter.And(o => customerOrderIds.Contains(o.CustomerOrderID));
                            }
                            break;
                        case WayBill.SearchFilterEnum.TrackingNumber:
                            filter = filter.And(p => numberList.Contains(p.TrackingNumber));
                            break;
                        case WayBill.SearchFilterEnum.CustomerOrderNumber:
                            filter = filter.And(p => numberList.Contains(p.CustomerOrderNumber));
                            break;
                    }
                }
            }

            //if (!param.WayBillNumber.IsNullOrWhiteSpace())
            //{
            //    var numberList = param.WayBillNumber.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
            //             .ToList();

            //    if (numberList.Count > 0) 
            //    {
            //        string number = numberList[0];
            //        List<int> customerOrderIds = new List<int>();
            //        customerOrderIds = _wayBillInfoRepository.GetCustomerId(numberList);
            //        if (customerOrderIds.Count > 0)
            //        {
            //            int customerOrderId = customerOrderIds[0];
            //            filter = customerOrderIds.Count == 1 ? filter.And(o => o.CustomerOrderID == customerOrderId) : filter.And(o => customerOrderIds.Contains(o.CustomerOrderID));
            //        }
            //    }
            //}
            //else if (!param.CustomerOrderNumber.IsNullOrWhiteSpace())
            //{
            //    var numberList = param.CustomerOrderNumber.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
            //             .ToList();

            //    if (numberList.Count > 0)
            //    {
            //        string number = numberList[0];
            //        filter = numberList.Count == 1 ? filter.And(o => o.CustomerOrderNumber == number) : filter.And(o => numberList.Contains(o.CustomerOrderNumber));
            //    }
            //}
            //else
            //{
                filter = filter
                .AndIf(o => o.ShippingMethodId == param.ShippingMethodId.Value, param.ShippingMethodId.HasValue)
                    //.AndIf(o => o.CustomerOrderNumber == param.CustomerOrderNumber, !param.CustomerOrderNumber.IsNullOrWhiteSpace())
                .AndIf(o => o.ShippingInfo.CountryCode == param.CountryCode, !param.CountryCode.IsNullOrWhiteSpace())
                .AndIf(o => o.CreatedOn >= param.CreatedOnFrom.Value, param.CreatedOnFrom.HasValue)
                .AndIf(o => o.CreatedOn <= param.CreatedOnTo.Value, param.CreatedOnTo.HasValue)
                .AndIf(o => o.IsHold == param.IsHold.Value, param.IsHold.HasValue)
                    //.AndIf(o => o. >= param.CreatedOnFrom.Value, param.CreatedOnFrom.HasValue)
                .AndIf(o => o.CustomerCode == param.CustomerCode, !param.CustomerCode.IsNullOrWhiteSpace())
                .AndIf(o => o.IsPrinted == param.IsPrinted.Value, param.IsPrinted.HasValue);
            //}
            return _customerOrderInfoRepository.GetCustomerOrderInfoList(filter);
        }

        public List<ApplicationInfo> GetApplicationInfoList(List<int> ids)
        {
            return _applicationInfoRepository.GetList(p => ids.Contains(p.CustomerOrderID ?? 0));
        }

        private bool IsCustomerOrderDelete(int status)//失败订单也删除 update by yungchu
        {
            return (CustomerOrder.StatusEnum.None.GetStatusValue() == status||CustomerOrder.StatusEnum.SubmitFail.GetStatusValue()==status);
        }
        private bool IsCustomerOrderCancel(int status)
        {
            return (CustomerOrder.StatusEnum.OK.GetStatusValue() == status);
        }

        private bool IsCustomerOrderConfirm(int status)
        {
            return (CustomerOrder.StatusEnum.None.GetStatusValue() == status);
        }

		private bool IsCustomerOrderSubmitFail(int status)
	    {
			return (CustomerOrder.StatusEnum.SubmitFail.GetStatusValue() == status);
	    }

	    /// <summary>
        /// 客户订单确认
        /// </summary>
        /// <param name="customerOrderId">客户订单ID</param>
        /// <returns></returns>
        public bool CustomerOrderConfirm(int customerOrderId)
        {
            var info = Get(customerOrderId);

            if (info == null) return false;

            if (IsCustomerOrderConfirm(info.Status))
            {
                info.LastUpdatedBy = _workContext.User.UserUame;
                info.LastUpdatedOn = DateTime.Now;
                info.Status = CustomerOrder.StatusEnum.OK.GetStatusValue();

                info.CustomerOrderStatuses.Add(new CustomerOrderStatus
                    {
                        CreatedOn = info.LastUpdatedOn,
                        CustomerOrderID = info.CustomerOrderID,
                        Status = info.Status,
                        Remark = "客户确认"
                    });

                _customerOrderInfoRepository.Modify(info);
                _customerOrderInfoRepository.UnitOfWork.Commit();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 客户订单确认(批量)
        /// </summary>
        /// <param name="customerOrderIds">订单ID列表</param>
        /// <returns></returns>
        public bool CustomerOrderConfirmBatch(List<int> customerOrderIds)
        {
            try
            {
                foreach (var customerOrderId in customerOrderIds)
                {
                    var info = Get(customerOrderId);

                    if (info == null) return false;

                    if (IsCustomerOrderConfirm(info.Status))
                    {
                        info.LastUpdatedBy = _workContext.User.UserUame;
                        info.LastUpdatedOn = DateTime.Now;
                        info.Status = CustomerOrder.StatusEnum.OK.GetStatusValue();

                        info.CustomerOrderStatuses.Add(new CustomerOrderStatus
                        {
                            CreatedOn = info.LastUpdatedOn,
                            CustomerOrderID = info.CustomerOrderID,
                            Status = info.Status,
                            Remark = "客户确认"
                        });

                        _customerOrderInfoRepository.Modify(info);
                    }
                }
                using (var transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    _customerOrderInfoRepository.UnitOfWork.Commit();
                    transaction.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return false;
            }
            
        }



        private bool IsCustomerOrderSubmit(int status)
        {
            return (CustomerOrder.StatusEnum.OK.GetStatusValue() == status || CustomerOrder.StatusEnum.SubmitFail.GetStatusValue() == status);
        }

        private bool IsCustomerOrderHold(int status)
        {
            return (CustomerOrder.StatusEnum.Submitted.GetStatusValue() == status || CustomerOrder.StatusEnum.Have.GetStatusValue() == status);
        }

        /// <summary>
        /// 客户订单提交
        /// </summary>
        /// <param name="customerOrderId">客户订单号</param>
        /// <returns></returns>
        public bool CustomerOrderSubmit(int customerOrderId)
        {
            var info = Get(customerOrderId);

            if (info == null) return false;
            if (IsCustomerOrderSubmit(info.Status))
            {
                info.LastUpdatedBy = _workContext.User.UserUame;
                info.LastUpdatedOn = DateTime.Now;
                info.Status = CustomerOrder.StatusEnum.Submitted.GetStatusValue();
                info.CustomerOrderStatuses.Add(new CustomerOrderStatus
                {
                    CreatedOn = info.LastUpdatedOn,
                    CustomerOrderID = info.CustomerOrderID,
                    Status = info.Status,
                    Remark = "客户提交"
                });
                int status = WayBill.StatusEnum.Submitted.GetStatusValue();
                var wayBillInfo = new WayBillInfo
                    {
                        CustomerOrderID = info.CustomerOrderID,
                        CustomerCode = info.CustomerCode,
                        InShippingMethodID = info.ShippingMethodId,
                        InShippingMethodName = info.ShippingMethodName,
                        ShippingInfoID = info.ShippingInfoID,
                        GoodsTypeID = info.GoodsTypeID,
                        TrackingNumber = info.TrackingNumber,
                        IsReturn = info.IsReturn,
                        IsHold = false,
                        IsBattery = info.IsBattery,
                        Status = status,
                        CountryCode = info.ShippingInfo.CountryCode,
                        InsuredID = info.InsuredID,
                        CreatedOn = info.LastUpdatedOn,
                        CreatedBy = info.LastUpdatedBy,
                        LastUpdatedBy = info.LastUpdatedBy,
                        LastUpdatedOn = info.LastUpdatedOn
                    };

                using (var transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    _orderService.BatchCreateWayBillInfo(new List<WayBillInfo> { wayBillInfo });
                    _customerOrderInfoRepository.Modify(info);
                    _customerOrderInfoRepository.UnitOfWork.Commit();
                    transaction.Complete();
                    return true;
                }
            }

            return false;
        }

        private static readonly Hashtable CustomerOrderSubmitBatchTable = new Hashtable();

        /// <summary>
        /// 判断是否在正在提交的订单中
        /// </summary>
        public bool IsCustomerOrderSubmitWorkDoing(int[] customerOrderIds)
        {
            lock (CustomerOrderSubmitBatchTable.SyncRoot)
            {
                bool exist = customerOrderIds.Any(customerOrderId => CustomerOrderSubmitBatchTable.ContainsKey(customerOrderId));

                if (!exist)
                {
                    //不存在，则插入到字典
                    customerOrderIds.ToList().ForEach(p => CustomerOrderSubmitBatchTable.Add(p, ""));
                }

                return exist;
            }
        }

        /// <summary>
        /// 操作完成，从列表中删除
        /// </summary>
        public void RemoveCustomerOrderSubmitWorkDoing(int[] customerOrderIds)
        {
            lock (CustomerOrderSubmitBatchTable.SyncRoot)
            {
                customerOrderIds.ToList().ForEach(p => CustomerOrderSubmitBatchTable.Remove(p));
            }
        }

        /// <summary>
        /// 客户订单提交
        /// </summary>
        public void CustomerOrderSubmitQuick(List<int> customerOrderIds)
        {
            const string queueName = "SubmitOrder";

            //LtQueue.LtQueue ltQueue = new LtQueue.LtQueue();

            try
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required))
                {

                    Stopwatch stopwatch1 = new Stopwatch();
                    stopwatch1.Start();

                    //过滤掉已经正在处理中的订单
                    var submitingedCustomerOrderIDs = _customerOrderInfoRepository.SelectCustomerOrderSubmiting(customerOrderIds.ToArray());

                    customerOrderIds.RemoveAll(submitingedCustomerOrderIDs.Contains);

                    if (customerOrderIds.Count == 0)
                    {
                        throw new BusinessLogicException("没有需要提交的订单或订单已经提交");
                        //return;
                    }

                    _customerOrderInfoRepository.ModifCustomerOrderToSubmiting(customerOrderIds.ToArray(), _workContext.User.UserUame);

                    Debug.WriteLine(stopwatch1.Elapsed.TotalSeconds);

                    stopwatch1.Restart();

                    bool addSuccess = QueueHelper.Enqueue(queueName, customerOrderIds.Select(p=>p.ToString()).ToArray());

                    Debug.WriteLine(stopwatch1.Elapsed.TotalSeconds);

                    if (!addSuccess)
                    {
                        throw new BusinessLogicException("系统异常，请重试");
                    }

                    transaction.Complete();
                }
            }
            catch (BusinessLogicException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                throw new Exception("系统错误，请稍后重试");
            }
        }

        //已经被移植到异步提交程序，by daniel , 2014-10-23
        ///// <summary>
        ///// 客户订单提交(批量)
        ///// </summary>
        ///// <param name="customerOrderIds">订单ID列表</param>
        ///// <returns></returns>
        //public ResultExt CustomerOrderSubmitBatch(object customerOrderIds)
        //{
        //    return CustomerOrderSubmitBatch(customerOrderIds as List<int>);
        //}

        ///// <summary>
        ///// 客户订单提交(批量)
        ///// </summary>
        ///// <param name="customerOrderIds">订单ID列表</param>
        ///// <returns></returns>
        //public ResultExt CustomerOrderSubmitBatch(List<int> customerOrderIds)
        //{
        //    ResultExt resultExt = new ResultExt();
        //    resultExt.ReturnResult = true;

        //    //获取要提交的订单信息
        //    var customerOrderInfos = GetListByCustomerOrderId(customerOrderIds, CustomerOrder.StatusEnum.OK.GetStatusValue());
        //    if (customerOrderInfos.Count < 1)
        //    {
        //        resultExt.ReturnResult = false;
        //        //throw new Exception("提交的订单不是已确认状态");
        //    }

        //    //申请跟踪号
        //    //var firstWayBillNumber = SequenceNumberService.GetSequenceNumber(PrefixCode.OrderID, customerOrderIds.Count);
        //    //var number = firstWayBillNumber.Substring(PrefixCode.OrderID.Length, firstWayBillNumber.Length - PrefixCode.OrderID.Length).ConvertTo<long>();


        //    int trackingNumberDetailUpdateNumber = 0;//要更新跟踪号明细的数量
        //    List<int> failureShippingMethodId = new List<int>();

        //    //本次提交涉及到的运输方式
        //    List<int> shippingMethodIds = new List<int>();
        //    foreach (var info in customerOrderInfos)
        //    {
        //        var shippingMethodId = info.ShippingMethodId.HasValue ? info.ShippingMethodId.Value : 0;
        //        if (shippingMethodIds.Contains(shippingMethodId) || shippingMethodId == 0) continue;
        //        shippingMethodIds.Add(shippingMethodId);
        //    }
        //    var shippingMethodList = _freightService.GetShippingMethodsByIds(shippingMethodIds);


        //    List<TrackingNumberInfo> trackingNumbers = _trackingNumberService.GetTrackingNumbers(shippingMethodIds);
        //    //System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
        //    //stopwatch.Start();
        //    List<ApplicationInfo> applicationInfos = new List<ApplicationInfo>();
        //    applicationInfos = _applicationInfoRepository.GetList(a => customerOrderIds.Contains(a.CustomerOrderID ?? 0));
        //    List<string> customerOrderNumbers = new List<string>();
        //    List<string> UsedTrackingNumbers = new List<string>();
        //    //【add】表示是否有数据需要更新
        //    int add = 0;
        //    #region foreach
        //    List<WayBillInfo> modelList = new List<WayBillInfo>();

        //    foreach (var info in customerOrderInfos)
        //    {
        //        var wayBillInfo = new WayBillInfo
        //            {
        //                //WayBillNumber = PrefixCode.OrderID + number.ToString(),
        //                WayBillNumber = SequenceNumberService.GetWayBillNumber(info.CustomerCode),
        //                CustomerOrderID = info.CustomerOrderID,
        //                CustomerOrderNumber = info.CustomerOrderNumber,
        //                CustomerCode = info.CustomerCode,
        //                InShippingMethodID = info.ShippingMethodId,
        //                InShippingMethodName = info.ShippingMethodName,
        //                ShippingInfoID = info.ShippingInfoID,
        //                SenderInfoID = info.SenderInfoID,
        //                GoodsTypeID = info.GoodsTypeID,
        //                TrackingNumber = info.TrackingNumber,
        //                IsReturn = info.IsReturn,
        //                IsHold = false,
        //                IsBattery = info.IsBattery,
        //                Status = WayBill.StatusEnum.Submitted.GetStatusValue(),
        //                CountryCode = info.ShippingInfo.CountryCode.ToUpperInvariant(),
        //                InsuredID = info.InsuredID,
        //                Weight = info.Weight,
        //                Length = info.Length,
        //                Width = info.Width,
        //                Height = info.Height,
        //                CreatedOn = info.LastUpdatedOn,
        //                CreatedBy = info.LastUpdatedBy,
        //                LastUpdatedBy = info.LastUpdatedBy,
        //                LastUpdatedOn = info.LastUpdatedOn,
        //                EnableTariffPrepay = info.EnableTariffPrepay,
        //            };


        //        //Add By zxq
        //        //Time:2014-09-15
        //        var wayBillEventLog = new WayBillEventLog()
        //            {
        //                WayBillNumber = wayBillInfo.WayBillNumber,
        //                EventCode = (int) WayBillEvent.EventCodeEnum.Submit,
        //                Description = WayBillEvent.GetEventCodeDescription((int) WayBillEvent.EventCodeEnum.Submit),
        //                EventDate = DateTime.Now,
        //                LastUpdatedOn = DateTime.Now,
        //                Operator = _workContext.User.UserUame,
        //            };

        //        _wayBillEventLogRepository.Add(wayBillEventLog);


        //        #region 判断跟踪是否为空

        //        if (string.IsNullOrWhiteSpace(info.TrackingNumber))
        //        {
        //            var shippingMethodId = info.ShippingMethodId.HasValue ? info.ShippingMethodId.Value : 0;
        //            var model = shippingMethodList.Find(p => p.ShippingMethodId == shippingMethodId);
        //            if (failureShippingMethodId.Contains(shippingMethodId))
        //            {
        //                //number++;
        //                continue;
        //            }
        //            if (model != null && model.IsSysTrackNumber)
        //            {
        //                var trackingNumberList =
        //                    trackingNumbers.FindAll(
        //                        p =>
        //                        p.ShippingMethodID == shippingMethodId &&
        //                        p.ApplianceCountry.Contains(wayBillInfo.CountryCode));

        //                if (!trackingNumberList.Any())
        //                {
        //                    if (!failureShippingMethodId.Contains(shippingMethodId))
        //                    {
        //                        failureShippingMethodId.Add(shippingMethodId);
        //                    }
        //                    //number++;
        //                    continue;
        //                }
        //                TrackingNumberDetailInfo trackingNumberDetailInfo = new TrackingNumberDetailInfo();
        //                foreach (var trackingNumberInfo in trackingNumberList)
        //                {
        //                    trackingNumberDetailInfo =
        //                        trackingNumberInfo.TrackingNumberDetailInfos.ToList()
        //                                          .Find(
        //                                              p =>
        //                                              p.Status == (short) TrackingNumberDetailInfo.StatusEnum.NotUsed &&
        //                                              !UsedTrackingNumbers.Contains(p.TrackingNumber));

        //                }
        //                if (trackingNumberDetailInfo == null)
        //                {
        //                    if (!failureShippingMethodId.Contains(shippingMethodId))
        //                    {
        //                        failureShippingMethodId.Add(shippingMethodId);
        //                    }
        //                    //number++;
        //                    continue;
        //                }
        //                else
        //                {
        //                    // 更新trackingNumberDetail表的status改为已使用、将WayBillNumber字段的填充
        //                    wayBillInfo.TrackingNumber = trackingNumberDetailInfo.TrackingNumber;
        //                    info.TrackingNumber = trackingNumberDetailInfo.TrackingNumber;
        //                    trackingNumberDetailInfo.Status = (short) TrackingNumberDetailInfo.StatusEnum.Used;
        //                    trackingNumberDetailInfo.WayBillNumber = wayBillInfo.WayBillNumber;
        //                    _trackingNumberDetailInfoRepository.Modify(trackingNumberDetailInfo);
        //                    UsedTrackingNumbers.Add(trackingNumberDetailInfo.TrackingNumber);
        //                }
        //            }
        //        }

        //        //修改运单状态
        //        info.LastUpdatedBy = _workContext.User.UserUame;
        //        info.LastUpdatedOn = DateTime.Now;
        //        //info.Status = CustomerOrder.StatusEnum.Submitted.GetStatusValue();
        //        info.Status = CustomerOrder.StatusEnum.Submitted.GetStatusValue();
        //        info.CustomerOrderStatuses.Add(new CustomerOrderStatus
        //            {
        //                CreatedOn = info.LastUpdatedOn,
        //                CustomerOrderID = info.CustomerOrderID,
        //                Status = info.Status,
        //                Remark = "客户提交"
        //            });

        //        //更新ApplicationInfo表的WayBillNumber字段
        //        foreach (var appInfo in applicationInfos)
        //        {
        //            if (info.CustomerOrderID == appInfo.CustomerOrderID)
        //            {
        //                appInfo.WayBillNumber = wayBillInfo.WayBillNumber;
        //                appInfo.LastUpdatedBy = _workContext.User.UserUame;
        //                appInfo.LastUpdatedOn = DateTime.Now;
        //                _applicationInfoRepository.Modify(appInfo);
        //            }
        //        }
        //        _wayBillInfoRepository.Add(wayBillInfo);
        //        modelList.Add(wayBillInfo);
        //        _customerOrderInfoRepository.Modify(info);
        //        customerOrderNumbers.Add(info.CustomerOrderNumber);
        //        //number++;
        //        add++;

        //        #endregion
        //    }

        //    #endregion
        //    //stopwatch.Stop();
        //    //TimeSpan timespan = stopwatch.Elapsed;
        //    //double seconds = timespan.TotalSeconds;  //总秒数


        //    List<int> shippingMehtodIdList = new List<int>();
        //    if (failureShippingMethodId.Count > 0)
        //    {
        //        //找出跟踪号不足的运单的运输方式
        //        failureShippingMethodId.ForEach(p =>
        //        {
        //            var shippingMehtod = shippingMethodList.FirstOrDefault(s => s.ShippingMethodId == p && !shippingMehtodIdList.Contains(s.ShippingMethodId));
        //            if (shippingMehtod != null)
        //            {
        //                resultExt.Messge += "[" + shippingMehtod.FullName + "]";
        //                shippingMehtodIdList.Add(shippingMehtod.ShippingMethodId);
        //            }
        //        });
        //        resultExt.Messge += "运输方式跟踪号不足!";
        //        resultExt.ReturnResult = false;
        //    }
        //    var deleteStatus = WayBill.StatusToValue(WayBill.StatusEnum.Delete);
        //    var returnStatus = WayBill.StatusToValue(WayBill.StatusEnum.Return);
        //    try
        //    {
        //        lock (lockCreateWayBillInfo)
        //        {
        //            var wayBillInfoList = _wayBillInfoRepository.GetList(p => customerOrderNumbers.Contains(p.CustomerOrderNumber) && p.Status != deleteStatus && p.Status != returnStatus);
        //            if (wayBillInfoList.Count > 0)
        //            {
        //                string ErrorwayBillNumber = "";
        //                wayBillInfoList.ForEach(p => ErrorwayBillNumber += p.CustomerOrderNumber);
        //                Log.Error(ErrorwayBillNumber);
        //                throw new Exception("存在重复提交订单");
        //            }
        //            resultExt.ReturnNumber = add;
        //            using (var transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.MaxValue))
        //            {
        //                if (add > 0)
        //                {
        //                    _wayBillInfoRepository.UnitOfWork.Commit();
        //                    _customerOrderInfoRepository.UnitOfWork.Commit();
        //                    _applicationInfoRepository.UnitOfWork.Commit();
        //                    _wayBillEventLogRepository.UnitOfWork.Commit();
        //                    if (trackingNumberDetailUpdateNumber > 0)
        //                    {
        //                        _trackingNumberDetailInfoRepository.UnitOfWork.Commit();
        //                    }
        //                }
        //                transaction.Complete();
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Exception(ex);
        //        if (ex.Message != "存在重复提交订单")
        //        {
        //            throw new Exception(ex.Message + "请重新提交");
        //        }

        //    }
        //    return resultExt;
        //}



        public bool IsExists(string customerCode, string customerOrderNumber)
        {
            var deleteStatus = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Delete);
            customerOrderNumber = customerOrderNumber.Trim();
            return _customerOrderInfoRepository.Exists(o => o.CustomerCode == customerCode && o.CustomerOrderNumber == customerOrderNumber && o.Status != deleteStatus);
        }

        /// <summary>
        /// 拦截订单
        /// </summary>
        /// <param name="customerOrderId">订单ID</param>
        /// <param name="message">拦截订单的原因</param>
        /// <returns></returns>
        public bool IsHold(int customerOrderId, string message)
        {
            var orderInfo = Get(customerOrderId);

            if (orderInfo != null && IsCustomerOrderHold(orderInfo.Status) && !orderInfo.IsHold)
            {
                orderInfo.IsHold = true;
                orderInfo.LastUpdatedBy = _workContext.User.UserUame;
                orderInfo.LastUpdatedOn = DateTime.Now;

                int statusSubmitted = WayBill.StatusEnum.Submitted.GetStatusValue();
                int statusHave = WayBill.StatusEnum.Have.GetStatusValue();
                var listWayBill = orderInfo.WayBillInfos.Where(w => w.CustomerOrderID == customerOrderId && w.IsHold == false && (w.Status == statusHave || w.Status == statusSubmitted)).ToList();

                using (var transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    if (listWayBill.Count > 0)
                    {
                        listWayBill.ForEach(item =>
                            {
                                item.IsHold = true;
                                item.LastUpdatedBy = orderInfo.LastUpdatedBy;
                                item.LastUpdatedOn = orderInfo.LastUpdatedOn;
                                item.AbnormalWayBillLog = new AbnormalWayBillLog
                                    {
                                        AbnormalDescription = message,
                                        AbnormalStatus = WayBill.AbnormalStatusEnum.NO.GetAbnormalStatusValue(),
                                        OperateType = WayBill.AbnormalTypeEnum.Intercepted.GetAbnormalTypeValue(),
                                        CreatedOn = orderInfo.LastUpdatedOn,
                                        CreatedBy = orderInfo.LastUpdatedBy,
                                        LastUpdatedBy = orderInfo.LastUpdatedBy,
                                        LastUpdatedOn = orderInfo.LastUpdatedOn
                                    };
                                _wayBillInfoRepository.Modify(item);
                            });

                    }

                    _customerOrderInfoRepository.Modify(orderInfo);
                    _customerOrderInfoRepository.UnitOfWork.Commit();
                    transaction.Complete();
                    return true;
                }
            }


            return false;
        }
        /// <summary>
        /// 拦截客户订单(批量)
        /// </summary>
        /// <param name="customerOrderIds">订单ID列表</param>
        /// <param name="message">批量拦截客户订单原因</param>
        /// <returns></returns>
        public bool BatchHold(List<int> customerOrderIds, string message)
        {
            if (null != customerOrderIds && customerOrderIds.Count > 0)
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    foreach (var customerOrderId in customerOrderIds)
                    {
                        var orderInfo = Get(customerOrderId);

                        if (orderInfo != null && IsCustomerOrderHold(orderInfo.Status) && !orderInfo.IsHold)
                        {
                            orderInfo.IsHold = true;
                            orderInfo.LastUpdatedBy = _workContext.User.UserUame;
                            orderInfo.LastUpdatedOn = DateTime.Now;

                            int statusSubmitted = WayBill.StatusEnum.Submitted.GetStatusValue();
                            int statusHave = WayBill.StatusEnum.Have.GetStatusValue();
                            var listWayBill =
                                orderInfo.WayBillInfos.Where(
                                    w =>
                                    w.CustomerOrderID == customerOrderId && w.IsHold == false &&
                                    (w.Status == statusHave || w.Status == statusSubmitted)).ToList();


                            if (listWayBill.Count > 0)
                            {
                                listWayBill.ForEach(item =>
                                {
                                    item.IsHold = true;
                                    item.LastUpdatedBy = orderInfo.LastUpdatedBy;
                                    item.LastUpdatedOn = orderInfo.LastUpdatedOn;
                                    item.AbnormalWayBillLog = new AbnormalWayBillLog
                                    {
                                        AbnormalDescription = message,
                                        AbnormalStatus = WayBill.AbnormalStatusEnum.NO.GetAbnormalStatusValue(),
                                        OperateType =
                                            WayBill.AbnormalTypeEnum.Intercepted.GetAbnormalTypeValue(),
                                        CreatedOn = orderInfo.LastUpdatedOn,
                                        CreatedBy = orderInfo.LastUpdatedBy,
                                        LastUpdatedBy = orderInfo.LastUpdatedBy,
                                        LastUpdatedOn = orderInfo.LastUpdatedOn
                                    };
                                    _wayBillInfoRepository.Modify(item);
                                });
                            }
                            _customerOrderInfoRepository.Modify(orderInfo);
                        }
                    }
                    _customerOrderInfoRepository.UnitOfWork.Commit();
                    transaction.Complete();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 客户端批量删除订单
        /// Add by zhengsong
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public bool DeleteCustomerOrderInfoList(List<int> selected)
        {
            if (!selected.Any()) return false;
            try
            {
                int pagesize = 50;
                int pageindex = 1;
                do
                {
                    _customerOrderInfoRepository.DeleteCustomerOrderList(_workContext.User.UserUame,
                                                                         selected.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList());

                    pageindex++;
                } while (selected.Count > (pageindex - 1) * pagesize);
                //_customerOrderInfoRepository.DeleteCustomerOrderList(_workContext.User.UserUame,selected);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return false;
            }
            return true;
        }


        public PagedList<CustomerOrderInfoExt> GetCustomerOrderByBlockedList(CustomerOrderParam param, int maxCustomerOrderId = 0)
        {
            return _customerOrderInfoRepository.GetCustomerOrderByBlockedList(param,maxCustomerOrderId);
        }

        public CustomerOrderInfo Print(int customerOrderId)
        {
            //var orderInfo = Get(customerOrderId);
            var orderInfo = _customerOrderInfoRepository.First(p=>p.CustomerOrderID==customerOrderId);

            if (orderInfo == null) return null;

            if (!orderInfo.IsPrinted)
            {
                orderInfo.IsPrinted = true;
                orderInfo.LastUpdatedBy = _workContext.User.UserUame;
                orderInfo.LastUpdatedOn = DateTime.Now;
                _customerOrderInfoRepository.Modify(orderInfo);
                _customerOrderInfoRepository.UnitOfWork.Commit();
            }

            return orderInfo;
        }

        public CustomerOrderInfo PrintByCustomerCode(int customerOrderId)
        {
            int deleteStatus = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Delete);
            var orderInfo = _customerOrderInfoRepository.First(p => p.CustomerOrderID == customerOrderId && p.CustomerCode == _workContext.User.UserUame && p.Status != deleteStatus);

            if (orderInfo == null) return null;

            if (!orderInfo.IsPrinted)
            {
                orderInfo.IsPrinted = true;
                orderInfo.LastUpdatedBy = _workContext.User.UserUame;
                orderInfo.LastUpdatedOn = DateTime.Now;
                _customerOrderInfoRepository.Modify(orderInfo);
                _customerOrderInfoRepository.UnitOfWork.Commit();
            }

            return orderInfo;
        }

        public CustomerOrderInfo PrintByCustomerOrderNumber(string wayBillOrOrderNumber)
        {
            int deleteStatus = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Delete);
            int deleteWayStatus = WayBill.StatusToValue(WayBill.StatusEnum.Delete);

            var orderInfo =
                _customerOrderInfoRepository.First(
                    p =>
                    p.CustomerOrderNumber.Equals(wayBillOrOrderNumber) && p.CustomerCode == _workContext.User.UserUame &&
                    p.Status != deleteStatus);

            if (orderInfo == null)
            {
                var wayBillInfo = _wayBillInfoRepository.First(
                    p =>
                    p.WayBillNumber.Equals(wayBillOrOrderNumber) && p.CustomerCode.Equals(_workContext.User.UserUame) &&
                    !p.IsHold && p.Status != deleteWayStatus);
                if (wayBillInfo == null)
                    return null;
                if (wayBillInfo.CustomerOrderID.HasValue)
                {
                    orderInfo =
                        _customerOrderInfoRepository.First(
                            p =>
                            p.CustomerOrderID == wayBillInfo.CustomerOrderID.Value &&
                            p.CustomerCode == _workContext.User.UserUame && p.Status != deleteWayStatus);
                }
                if (orderInfo == null) return null;
            }

            if (!orderInfo.IsPrinted)
            {
                orderInfo.IsPrinted = true;
                orderInfo.LastUpdatedBy = _workContext.User.UserUame;
                orderInfo.LastUpdatedOn = DateTime.Now;
                _customerOrderInfoRepository.Modify(orderInfo);
                _customerOrderInfoRepository.UnitOfWork.Commit();
            }
            return orderInfo;
        }


        public List<CustomerOrderInfo> PrintByCustomerOrderIds(IEnumerable<int> customerOrderIds)
        {
            var enumerable = customerOrderIds as int[] ?? customerOrderIds.ToArray();

            if (enumerable.Length == 1)
            {
                return PrintByCustomerOrderId(enumerable[0]);
            }

            int deleteStatus = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Delete);
            var orderInfoList = _customerOrderInfoRepository.GetList(p => enumerable.Contains(p.CustomerOrderID) && p.Status != deleteStatus);
            orderInfoList.ForEach(p =>
                {
                    if (p.IsPrinted) return;
                    p.IsPrinted = true;
                    p.LastUpdatedBy = _workContext.User!=null?_workContext.User.UserUame:p.CustomerCode;
                    p.LastUpdatedOn = DateTime.Now;
                    _customerOrderInfoRepository.Modify(p);
                });

            _customerOrderInfoRepository.UnitOfWork.Commit();

            return orderInfoList;
        }

        public List<CustomerOrderInfo> PrintByCustomerOrderId(int customerOrderId)
        {
            int deleteStatus = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Delete);
            var orderInfo = _customerOrderInfoRepository.GetList(p => p.CustomerOrderID == customerOrderId && p.Status != deleteStatus).Single();

            if (!orderInfo.IsPrinted)
            {
                orderInfo.IsPrinted = true;
                orderInfo.LastUpdatedBy = _workContext.User != null ? _workContext.User.UserUame : orderInfo.CustomerCode;
                orderInfo.LastUpdatedOn = DateTime.Now;
                _customerOrderInfoRepository.Modify(orderInfo);
                _customerOrderInfoRepository.UnitOfWork.Commit();
            }

            return new List<CustomerOrderInfo>() {orderInfo};
        }

        /// <summary>
        /// 根据订单号获取订单信息
        /// </summary>
        /// <param name="customerOrderNumber">订单数组</param>
        /// <returns></returns> 
        public List<CustomerOrderInfoExt> GetCustomerOrderList(string[] customerOrderNumber, string customerCode)
        {
            return _customerOrderInfoRepository.GetCustomerOrderList(customerOrderNumber, customerCode);
        }

        //判断订单号是否存在
        public CustomerOrderInfo GetCustomerOrderInfo(string customerOrderNumber)
        {
            var delete = (int) CustomerOrder.StatusEnum.Delete;
            var ret = (int) CustomerOrder.StatusEnum.Return;
            return _customerOrderInfoRepository.First(p => p.CustomerOrderNumber == customerOrderNumber && p.Status != delete && p.Status != ret);
        }

        public int GetMaxCustomerOrderID()
        {
            return _customerOrderInfoRepository.GetMaxCustomerOrderID();
        }

        public IList<WayBillInfo> GetEUBWayBillList(string[] wayBillNumber, string customerCode)
        {
            return _wayBillInfoRepository.GetWayBillListByWayBillNumbers(wayBillNumber, customerCode);
        }

        public IList<WayBillInfo> GetWayBillList(string[] wayBillNumber, string customerCode)
        {
            return _wayBillInfoRepository.GetWayBillList(wayBillNumber, customerCode);
        }

        public WayBillInfo GetWayBill(string wayBillNumber, string customerCode)
        {
            return _wayBillInfoRepository.GetWayBill(wayBillNumber, customerCode);
        }

	    public WayBillInfo GetWayBill(string number)
	    {
			return _wayBillInfoRepository.GetWayBill(number);
	    }


	    #region Eub申请

        /// <summary>
        /// Eub运单申请 ，并返回未成功的运单列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        /// <isNoLogin></isNoLogin>
        public List<string> ApplyEubWayBillInfo(EubWayBillParam param)
        {
            var error = ForecastEubWayBillInfo(param);
            return error.Select(row => row.Key).ToList();
        }

        /// <summary>
        /// 后台job 执行预报
        /// Add By zhengsong
        /// Time:2015-02-02
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isNoLogin"></param>
        /// <returns></returns>
        public Dictionary<string,string> ForecastEubWayBillInfo(EubWayBillParam param, bool isNoLogin = false)
        {
            Dictionary<string, string> wList = new Dictionary<string, string>();
            string batchNumber = "";
            if (isNoLogin)
            {
                batchNumber = "System" + SequenceNumberService.GetSequenceNumber("");
            }
            else
            {
                batchNumber = _workContext.User.UserUame + SequenceNumberService.GetSequenceNumber("");
            }

            foreach (var item in param.WayBillInfos)
            {
                var eubAccountInfo = _eubAccountInfoRepository.GetList(p => p.ShippingMethodId == item.InShippingMethodID).FirstOrDefault();
                if (eubAccountInfo == null)
                {
                    Log.Error("没有配置API帐号，ID：" + item.InShippingMethodID);
                    wList.Add(item.WayBillNumber, "没有配置API帐号");
                    continue;
                }
                order o = new order
                {
                    orderid = item.CustomerOrderInfo.CustomerOrderNumber,
                    customercode = eubAccountInfo.Account,
                    vipcode = "",
                    clcttype = 1,
                    untread = "Returned",
                    volweight = (int)Math.Ceiling(((item.Length ?? 1 * item.Height ?? 1 * item.Width ?? 1) / 6000)),

                    printcode = param.PrintFormatValue,//打印格式
                    startdate = "2012-07-25T09:21:06",
                    enddate = "2013-12-24T09:21:06",
                    sku1 = "",
                    sku2 = ""
                };

                o.sender = new sender
                {
                    name = eubAccountInfo.Name,
                    postcode = eubAccountInfo.ZipCode,
                    phone = eubAccountInfo.Phone,
                    mobile = eubAccountInfo.Mobile,
                    country = eubAccountInfo.CountryCode,
                    province = eubAccountInfo.State,
                    city = eubAccountInfo.City,
                    county = eubAccountInfo.County,
                    company = eubAccountInfo.CompanyName,
                    street = eubAccountInfo.Address,
                    email = eubAccountInfo.Email
                };

                o.collect = new collect
                {
                    name = eubAccountInfo.Name,
                    postcode = eubAccountInfo.ZipCode,
                    phone = eubAccountInfo.Phone,
                    mobile = eubAccountInfo.Mobile,
                    country = eubAccountInfo.CountryCode,
                    province = eubAccountInfo.State,
                    city = eubAccountInfo.City,
                    county = eubAccountInfo.County,
                    company = eubAccountInfo.CompanyName,
                    street = eubAccountInfo.Address,
                    email = eubAccountInfo.Email
                };
                o.receiver = new receiver//收件人信息
                {
                    name = item.ShippingInfo.ShippingFirstName + item.ShippingInfo.ShippingLastName,
                    postcode = item.ShippingInfo.ShippingZip,
                    phone = item.ShippingInfo.ShippingPhone,
                    mobile = item.ShippingInfo.ShippingPhone,
                    country = item.ShippingInfo.CountryCode,
                    province = item.ShippingInfo.ShippingState,
                    city = item.ShippingInfo.ShippingCity,
                    //county = item.ShippingInfo.ShippingCity,
                    company = item.ShippingInfo.ShippingCompany,
                    street = item.ShippingInfo.ShippingAddress,
                    email = ""
                };
                o.items = new items();
                o.items.item = new List<item>();
                foreach (var appInfo in item.CustomerOrderInfo.ApplicationInfos.Where(appInfo => appInfo.UnitPrice != null))
                {

                    o.items.item.Add(new item
                    {
                        CDataCnName = appInfo.PickingName,
                        enname = appInfo.ApplicationName,
                        count = appInfo.Qty ?? 1,
                        delcarevalue = (float)(appInfo.Total ?? 0),
                        origin = "CN",
                        weight = (float)(appInfo.UnitWeight ?? 0)
                    });
                }
                orders orders = new orders();
                orders.order = new List<order>();
                orders.order.Add(o);
                ShipOrderResponse response = new ShipOrderResponse();
                OrderLabelRespones labelRespones = new OrderLabelRespones();
                var sterin = SerializeUtil.SerializeToXml(orders);
                var xml = _eubService.GetShipOrderXml(orders, eubAccountInfo.AuthorizationCode, eubAccountInfo.ServerUrl);
                try
                {
                    response = XmlSerialHelper.DeserializeFromXml<ShipOrderResponse>(xml);
                }
                catch (Exception ex)
                {
                    response = null;
                    Log.Error("返回xml内容：" + xml);
                }
                if (response == null)
                {
                    //if (param.WayBillInfos.Count == 1)
                    //{
                        try
                        {
                            labelRespones = XmlSerialHelper.DeserializeFromXml<OrderLabelRespones>(xml);
                            wList.Add(item.WayBillNumber, labelRespones.description);//申请失败的运单
                            Log.Info(labelRespones.description);
                            continue;
                        }
                        catch (Exception ex)
                        {
                            Log.Exception(ex);
                            wList.Add(item.WayBillNumber, ex.Message);
                        }

                    //}
                    //wList.Add(item.WayBillNumber);//申请失败的运单
                    continue;
                }
                if (!string.IsNullOrWhiteSpace(response.mailnum))
                {
                    //更新跟踪号，并插入Eub运单数据
                    try
                    {
                        UpdateTrackNumberAndAddEubWayBill(batchNumber, response.mailnum, param.PrintFormat, item, isNoLogin);
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                        wList.Add(item.WayBillNumber, "更新跟踪号出现错误");
                        //throw new BusinessLogicException("更新跟踪号出现错误");
                    }

                }
                else//Eub接口调用失败，没有产生跟踪号
                {
                    Log.Error("Eub接口调用失败,返回xml内容：" + xml);
                    wList.Add(item.WayBillNumber, "Eub接口调用失败，未获取到跟踪号");//申请失败的运单
                }
            }
            return wList;
        }


        private void UpdateTrackNumberAndAddEubWayBill(string batchNumber, string trackNumber, int printFormat, WayBillInfo wayBillInfo,bool isNoLogin=false)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                var wayBill = _wayBillInfoRepository.Get(wayBillInfo.WayBillNumber);
                EubWayBillApplicationInfo eubWayBill = new EubWayBillApplicationInfo();
                eubWayBill.BatchNumber = batchNumber;
                eubWayBill.ShippingMethodID = wayBillInfo.InShippingMethodID ?? 0;
                eubWayBill.WayBillNumber = wayBillInfo.WayBillNumber;
                eubWayBill.PrintFormat = printFormat;
                eubWayBill.Status = (int) EubWayBillApplicationInfo.StatusEnum.Apply;
                eubWayBill.CreatedOn = DateTime.Now;
                eubWayBill.LastUpdatedOn = DateTime.Now;
                if (isNoLogin)
                {
                    eubWayBill.CreatedBy = "System";
                    eubWayBill.LastUpdatedBy = "System";
                }
                else
                {
                    eubWayBill.CreatedBy = _workContext.User.UserUame;
                    eubWayBill.LastUpdatedBy = _workContext.User.UserUame;
                }
                    
                if (!_eubWayBillApplicationInfoRepository.Exists(p => p.WayBillNumber == wayBillInfo.WayBillNumber))
                {
                    _eubWayBillApplicationInfoRepository.Add(eubWayBill);
                }
                wayBill.TrackingNumber = trackNumber;
                wayBill.LastUpdatedOn = DateTime.Now;
                wayBill.LastUpdatedBy = isNoLogin ? "System" : _workContext.User.UserUame;
                wayBill.CustomerOrderInfo.TrackingNumber = trackNumber;
                wayBill.CustomerOrderInfo.LastUpdatedOn = DateTime.Now;
                wayBill.CustomerOrderInfo.LastUpdatedBy = !isNoLogin ? _workContext.User.UserUame : "System";
                _wayBillInfoRepository.Modify(wayBill);
                //_customerOrderInfoRepository.Modify(wayBillInfo.CustomerOrderInfo);
                _eubWayBillApplicationInfoRepository.UnitOfWork.Commit();
                _wayBillInfoRepository.UnitOfWork.Commit();
                //_customerOrderInfoRepository.UnitOfWork.Commit();
                transaction.Complete();
            }
        }

        /// <summary>
        /// Eub运单Api申请
        /// Add By zhengsong
        /// Time:2015-01-22
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<ResultEUB> ApplyApiEubWayBillInfo(EubWayBillParam param)
        {
            List<ResultEUB> ResultEubs = new List<ResultEUB>();
            //string batchNumber = _workContext.User.UserUame + SequenceNumberService.GetSequenceNumber("");
            foreach (var item in param.WayBillInfos)
            {

                ResultEUB resultEub = new ResultEUB();
                resultEub.IsSuccess = false;
                WayBillInfo item1 = item;
                var eubAccountInfo = _eubAccountInfoRepository.GetList(p => p.ShippingMethodId == item1.InShippingMethodID).FirstOrDefault();
                if (eubAccountInfo == null)
                {
                    resultEub.IsSuccess = false;
                    resultEub.AirwayBillNumber = "没有配置API帐号";
                    resultEub.WayBillNumber = item.WayBillNumber;
                    //Log.Error("没有配置API帐号，ID：" + item1.InShippingMethodID);
                    ResultEubs.Add(resultEub);
                    continue;
                }
                order o = new order
                {
                    orderid = item.CustomerOrderInfo.CustomerOrderNumber,
                    customercode = eubAccountInfo.Account,
                    vipcode = "",
                    clcttype = 1,
                    untread = "Returned",
                    volweight = (int)Math.Ceiling(((item.Length ?? 1 * item.Height ?? 1 * item.Width ?? 1) / 6000)),

                    printcode = param.PrintFormatValue,//打印格式
                    startdate = "2012-07-25T09:21:06",
                    enddate = "2013-12-24T09:21:06",
                    sku1 = "",
                    sku2 = ""
                };

                o.sender = new sender
                {
                    name = eubAccountInfo.Name,
                    postcode = eubAccountInfo.ZipCode,
                    phone = eubAccountInfo.Phone,
                    mobile = eubAccountInfo.Mobile,
                    country = eubAccountInfo.CountryCode,
                    province = eubAccountInfo.State,
                    city = eubAccountInfo.City,
                    county = eubAccountInfo.County,
                    company = eubAccountInfo.CompanyName,
                    street = eubAccountInfo.Address,
                    email = eubAccountInfo.Email
                };

                o.collect = new collect
                {
                    name = eubAccountInfo.Name,
                    postcode = eubAccountInfo.ZipCode,
                    phone = eubAccountInfo.Phone,
                    mobile = eubAccountInfo.Mobile,
                    country = eubAccountInfo.CountryCode,
                    province = eubAccountInfo.State,
                    city = eubAccountInfo.City,
                    county = eubAccountInfo.County,
                    company = eubAccountInfo.CompanyName,
                    street = eubAccountInfo.Address,
                    email = eubAccountInfo.Email
                };
                o.receiver = new receiver//收件人信息
                {
                    name = item.ShippingInfo.ShippingFirstName + item.ShippingInfo.ShippingLastName,
                    postcode = item.ShippingInfo.ShippingZip,
                    phone = item.ShippingInfo.ShippingPhone,
                    mobile = item.ShippingInfo.ShippingPhone,
                    country = item.ShippingInfo.CountryCode,
                    province = item.ShippingInfo.ShippingState,
                    city = item.ShippingInfo.ShippingCity,
                    //county = item.ShippingInfo.ShippingCity,
                    company = item.ShippingInfo.ShippingCompany,
                    street = item.ShippingInfo.ShippingAddress,
                    email = ""
                };
                o.items = new items();
                o.items.item = new List<item>();
                foreach (var appInfo in item.ApplicationInfos.Where(appInfo => appInfo.UnitPrice != null))
                {

                    o.items.item.Add(new item
                    {
                        CDataCnName = appInfo.PickingName,
                        enname = appInfo.ApplicationName,
                        count = appInfo.Qty ?? 1,
                        delcarevalue = (float)(appInfo.Total ?? 0),
                        origin = "CN",
                        weight = (float)(appInfo.UnitWeight ?? 0)
                    });
                }
                orders orders = new orders();
                orders.order = new List<order>();
                orders.order.Add(o);
                ShipOrderResponse response = new ShipOrderResponse();
                OrderLabelRespones labelRespones = new OrderLabelRespones();
                var sterin = SerializeUtil.SerializeToXml(orders);
                var xml = _eubService.GetShipOrderXml(orders, eubAccountInfo.AuthorizationCode, eubAccountInfo.ServerUrl);
                try
                {
                    response = XmlSerialHelper.DeserializeFromXml<ShipOrderResponse>(xml);
                }
                catch (Exception ex)
                {
                    response = null;
                    Log.Error("返回xml内容：" + xml);
                }
                if (response == null)
                {
                        try
                        {
                            labelRespones = XmlSerialHelper.DeserializeFromXml<OrderLabelRespones>(xml);
                            resultEub.IsSuccess = false;
                            resultEub.AirwayBillNumber = labelRespones.description;//失败原因
                            resultEub.WayBillNumber = item.WayBillNumber;
                            ResultEubs.Add(resultEub);
                        }
                        catch (Exception ex)
                        {
                            Log.Exception(ex);
                            resultEub.IsSuccess = false;
                            resultEub.AirwayBillNumber = ex.Message;
                            resultEub.WayBillNumber = item.WayBillNumber;
                            ResultEubs.Add(resultEub);
                        }
                    
                    continue;
                }
                if (!string.IsNullOrWhiteSpace(response.mailnum))
                {
                    //更新跟踪号
                    try
                    {
                        if (AddEubWayBillApplicationInfo(item))
                        {
                            resultEub.IsSuccess = true;
                            resultEub.AirwayBillNumber = response.mailnum;
                            resultEub.WayBillNumber = item.WayBillNumber;
                            ResultEubs.Add(resultEub);
                        }
                        else
                        {
                            resultEub.IsSuccess = false;
                            resultEub.AirwayBillNumber = "重复递交EUB预报";
                            resultEub.WayBillNumber = item.WayBillNumber;
                            ResultEubs.Add(resultEub);
                        }
                        //resultEub.EubWayBillApplicationInfo = AddEubWayBillApplicationInfo(batchNumber, response.mailnum, param.PrintFormat, item);
                        
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                        resultEub.IsSuccess = false;
                        resultEub.AirwayBillNumber = "更新跟踪号出现错误";
                        resultEub.WayBillNumber = item.WayBillNumber;
                        ResultEubs.Add(resultEub);
                    }

                }
                else//Eub接口调用失败，没有产生跟踪号
                {
                    Log.Error(",返回xml内容：" + xml);
                    resultEub.IsSuccess = false;
                    resultEub.AirwayBillNumber = "Eub接口调用失败";
                    resultEub.WayBillNumber = item.WayBillNumber;
                    ResultEubs.Add(resultEub);//申请失败的运单
                    continue;
                }
            }
            return ResultEubs;
        }

        public class ResultEUB
        {
            public string WayBillNumber { get; set; }
            public string AirwayBillNumber { get; set; }
            public bool IsSuccess { get; set; }

            public EubWayBillApplicationInfo EubWayBillApplicationInfo { get; set; }
        }

        /// <summary>
        /// 添加 EubWayBillApplicationInf表信息
        /// Add By zhengsong
        /// Time:2014-01-22
        /// </summary>
        /// <param name="wayBillInfos"></param>
        public void AddEubWayBillApplicationInfo(List<WayBillInfo> wayBillInfos)
        {
            try
            {
                wayBillInfos.ForEach(p =>
                {
                    EubWayBillApplicationInfo eubWayBill = new EubWayBillApplicationInfo
                    {
                        BatchNumber = p.CreatedBy + SequenceNumberService.GetSequenceNumber(""),
                        ShippingMethodID = p.InShippingMethodID ?? 0,
                        WayBillNumber = p.WayBillNumber,
                        PrintFormat = 2,
                        Status = (int)EubWayBillApplicationInfo.StatusEnum.Apply,
                        CreatedOn = DateTime.Now,
                        CreatedBy = p.CreatedBy,
                        LastUpdatedOn = DateTime.Now,
                        LastUpdatedBy = p.CreatedBy
                    };

                    if (!_eubWayBillApplicationInfoRepository.Exists(z => z.WayBillNumber == p.WayBillNumber))
                    {
                        _eubWayBillApplicationInfoRepository.Add(eubWayBill);
                    }
                });
                _eubWayBillApplicationInfoRepository.UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                throw new BusinessLogicException(ex.Message);
            }
        }

        /// <summary>
        /// 判断是否添加过 EubWayBillApplicationInf表信息
        /// Add By zhengsong
        /// Time:2014-01-22
        /// </summary>
        /// <param name="wayBillInfo"></param>
        public bool AddEubWayBillApplicationInfo(WayBillInfo wayBillInfo)
        {
            try
            {
                if (!_eubWayBillApplicationInfoRepository.Exists(z => z.WayBillNumber == wayBillInfo.WayBillNumber))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return false;
            }
        }
        /// <summary>
        /// 更新Eub订单的已打印状态
        /// </summary>
        /// <param name="eubOrderId"></param>
        public void UpdateEubWayBillStatus(int eubOrderId)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                EubWayBillApplicationInfo eubWayBill = _eubWayBillApplicationInfoRepository.Get(eubOrderId);
                if (eubWayBill != null)
                {
                    WayBillInfo wayBillInfo = _wayBillInfoRepository.Get(eubWayBill.WayBillNumber);
                    if (wayBillInfo != null)
                    {
                        CustomerOrderInfo customerOrderInfo =
                            _customerOrderInfoRepository.Get(wayBillInfo.CustomerOrderID);
                        if (customerOrderInfo != null)
                        {
                            customerOrderInfo.IsPrinted = true;
                            customerOrderInfo.LastUpdatedOn = DateTime.Now;
                            customerOrderInfo.LastUpdatedBy = _workContext.User.UserUame;
                            _customerOrderInfoRepository.Modify(customerOrderInfo);
                            _customerOrderInfoRepository.UnitOfWork.Commit();
                        }
                    }
                    eubWayBill.Status = (int)EubWayBillApplicationInfo.StatusEnum.Printer;
                    eubWayBill.LastUpdatedOn = DateTime.Now;
                    eubWayBill.LastUpdatedBy = _workContext.User.UserUame;
                    _eubWayBillApplicationInfoRepository.Modify(eubWayBill);
                    _eubWayBillApplicationInfoRepository.UnitOfWork.Commit();
                    transaction.Complete();
                }
            }
        }




        /// <summary>
        /// 更新Eub运单状态
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <param name="status"></param>
        public void UpdateEubWayBillInfoStatus(List<string> wayBillNumbers, int status)
        {
            var eubWayBillList = GetEubWayBillInfoList(wayBillNumbers);
            var customerOrders =
               _customerOrderInfoRepository.GetCustomerOrderListByWayBillNumber(wayBillNumbers.ToArray(),
                                                                                _workContext.User.UserUame);
            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                foreach (var eubWayBill in eubWayBillList)
                {
                    eubWayBill.Status = status;
                    eubWayBill.LastUpdatedOn = DateTime.Now;
                    eubWayBill.LastUpdatedBy = _workContext.User.UserUame;
                    _eubWayBillApplicationInfoRepository.Modify(eubWayBill);
                }
           
                foreach (var customerOrder in customerOrders)
                {
                    customerOrder.IsPrinted = true;
                    customerOrder.LastUpdatedOn = DateTime.Now;
                    customerOrder.LastUpdatedBy = _workContext.User.UserUame;
                    _customerOrderInfoRepository.Modify(customerOrder);
                }

                _eubWayBillApplicationInfoRepository.UnitOfWork.Commit();
                _customerOrderInfoRepository.UnitOfWork.Commit();
                transaction.Complete();
            }
        }



		/// <summary>
		/// 删除运单(改运单，订单状态)
		/// </summary>
		/// <param name="waybillNumber"></param>
		/// <returns></returns>
		public bool UpdateWaybillStatus(List<int> customerOrderIds)
		{
			bool result = true;
			try
			{
				using (var transaction = new TransactionScope(TransactionScopeOption.Required))
				{
					foreach (var customerOrderId in customerOrderIds)
					{
						WayBillInfo wayBillInfo = new WayBillInfo();
						wayBillInfo = _wayBillInfoRepository.First(a => a.CustomerOrderID == customerOrderId);
						if (wayBillInfo!=null)
						{
							wayBillInfo.Status = WayBill.StatusToValue(WayBill.StatusEnum.Delete);
							wayBillInfo.LastUpdatedOn = DateTime.Now;
							wayBillInfo.LastUpdatedBy = _workContext.User.UserUame;
							_wayBillInfoRepository.Modify(wayBillInfo);

						}


						CustomerOrderInfo customerOrderInfo = _customerOrderInfoRepository.First(a => a.CustomerOrderID == customerOrderId);
						if (customerOrderInfo != null)
						{
							customerOrderInfo.Status = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Delete);
							customerOrderInfo.LastUpdatedOn = DateTime.Now;
							customerOrderInfo.LastUpdatedBy = _workContext.User.UserUame;
							_customerOrderInfoRepository.Modify(customerOrderInfo);
						}

					}
					_customerOrderInfoRepository.UnitOfWork.Commit();
					_wayBillInfoRepository.UnitOfWork.Commit();
					transaction.Complete();
				}

			}
			catch (Exception)
			{
				result = false;
			}

			return result;
		}





	    public List<EubWayBillApplicationInfo> GetEubWayBillInfoList(List<string> wayBillNumbers)
        {
            return _eubWayBillApplicationInfoRepository.GetEubWayBillInfoList(wayBillNumbers);
        }
        /// <summary>
        /// 获取所有EUB运单
        /// </summary>
        /// <param name="shippingMethodIds">EUB运输方式列表</param>
        /// <returns></returns>
        public List<WayBillInfo> GetEubWayBillList(List<int> shippingMethodIds)
        {
            return _wayBillInfoRepository.GetEubWayBillList(shippingMethodIds, _workContext.User.UserUame).ToList();
        }

        /// <summary>
        /// add huhaiyou 2014-07-03
        /// </summary>
        /// <param name="shippingMethodIds"></param>
        /// <returns></returns>
        public int GetEubWayBillCount(List<int> shippingMethodIds)
        {
            return _wayBillInfoRepository.GetEubWayBillCount(shippingMethodIds, _workContext.User.UserUame);
        }

        public IPagedList<EubWayBillApplicationInfoExt> GetEubWayBillList(EubWayBillApplicationInfoParam param, int maxCustomerOrderId = 0)
        {
            return _eubWayBillApplicationInfoRepository.GetEubWayBillList(param, maxCustomerOrderId);
        }
        #endregion


        public List<string> CheckRemoteArea(List<int> customerOrderIds)
        {
            var listCustomerOrders = _customerOrderInfoRepository.GetList(p => customerOrderIds.Contains(p.CustomerOrderID));

            List<string> listRemoteAreaOrders=new List<string>();

            foreach (CustomerOrderInfo customerOrderInfo in listCustomerOrders)
            {
               if (_freightService.IsRemoteArea(customerOrderInfo.ShippingMethodId, customerOrderInfo.ShippingInfo.CountryCode, customerOrderInfo.ShippingInfo.ShippingCity, customerOrderInfo.ShippingInfo.ShippingZip))
               {
                   listRemoteAreaOrders.Add(customerOrderInfo.CustomerOrderNumber);
               }
            }

            return listRemoteAreaOrders;
        }
    }
}
