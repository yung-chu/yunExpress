using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Transactions;
using LMS.Data.Context;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Repository
{
    public partial interface ICustomerOrderInfoRepository
    {
        PagedList<CustomerOrderInfoExt> GetCustomerOrderByBlockedList(CustomerOrderParam param, int maxCustomerOrderId = 0);
        void BatchAdd(List<CustomerOrderInfo> orderInfos);
        /// <summary>
        /// 根据订单号获取订单信息
        /// </summary>
        /// <param name="customerOrderNumber">订单数组</param>
        /// <returns></returns>
        List<CustomerOrderInfoExt> GetCustomerOrderList(string[] customerOrderNumber, string customerCode);

        List<CustomerOrderInfo> GetCustomerOrderListByWayBillNumber(string[] wayBillNumbers, string customerCode);

        List<CustomerOrderInfoExportExt> GetCustomerOrderInfoList(Expression<Func<CustomerOrderInfo, bool>> filter);

        PagedList<CustomerOrderInfoExt> GetCustomerOrderInfoList(CustomerOrderParam param, int maxCustomerOrderId = 0);

        bool DeleteCustomerOrderList(string customerCode, List<int> selected);

        void ModifCustomerOrderToSubmiting(int customerOrderId, string userName);

        /// <summary>
        /// 把订单的状态改为提交中
        /// </summary>
        /// <param name="customerOrderId"></param>
        /// <param name="updatedBy"></param>
        void ModifCustomerOrderToSubmiting(int[] customerOrderId, string updatedBy);


        List<int> SelectCustomerOrderSubmiting(int[] customerOrderId);

        int GetMaxCustomerOrderID();
    }

    public partial class CustomerOrderInfoRepository
    {
        public PagedList<CustomerOrderInfoExt> GetCustomerOrderByBlockedList(CustomerOrderParam param, int maxCustomerOrderId = 0)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            IQueryable<CustomerOrderInfoExt> list;
            int deleteSatus = CustomerOrder.StatusEnum.Delete.GetStatusValue();
            Expression<Func<CustomerOrderInfo, bool>> filter = o => o.Status != deleteSatus;
            filter = filter.AndIf(o => o.CustomerCode == param.CustomerCode, !param.CustomerCode.IsNullOrWhiteSpace());
            Expression<Func<WayBillInfo, bool>> filterWayBill = o => true;
            //if (!param.WayBillNumber.IsNullOrWhiteSpace())
            //{
            //    var numberList = param.WayBillNumber.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
            //             .ToList();

            //    if (numberList.Count > 0)
            //    {
            //        string number = numberList[0];
            //        filterWayBill = numberList.Count == 1 ? filterWayBill.And(a => a.WayBillNumber == number) : filterWayBill.And(a => numberList.Contains(a.WayBillNumber));
            //    }
            //}
            //else if (!param.CustomerOrderNumber.IsNullOrWhiteSpace())
            //{
            //    var numberList =
            //        param.CustomerOrderNumber.Split(Environment.NewLine.ToCharArray(),
            //                                        StringSplitOptions.RemoveEmptyEntries)
            //             .ToList();

            //    if (numberList.Count > 0)
            //    {
            //        string number = numberList[0];
            //        filter = numberList.Count == 1
            //                     ? filter.And(o => o.CustomerOrderNumber == number)
            //                     : filter.And(o => numberList.Contains(o.CustomerOrderNumber));
            //    }
            //}
            //else
            //{
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
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.WayBillNumber));
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
            else
            {
                if (!param.CustomerOrderNumber.IsNullOrWhiteSpace())
                {
                    var numberList =
                        param.CustomerOrderNumber.Split(Environment.NewLine.ToCharArray(),
                                                        StringSplitOptions.RemoveEmptyEntries)
                             .ToList();
                    filter = filter.AndIf(o => numberList.Contains(o.CustomerOrderNumber), numberList.Count > 0);

                }
            }



            //if (!param.WayBillNumber.IsNullOrWhiteSpace())
            //{
            //    var numberList =
            //        param.WayBillNumber.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
            //             .ToList();

            //    if (numberList.Count > 0)
            //    {
            //        string number = numberList[0];
            //        filterWayBill = numberList.Count == 1
            //                            ? filterWayBill.And(a => a.WayBillNumber == number)
            //                            : filterWayBill.And(a => numberList.Contains(a.WayBillNumber));
            //    }
            //}


            filterWayBill = filterWayBill.AndIf(o => o.CountryCode == param.CountryCode,
                                              !param.CountryCode.IsNullOrWhiteSpace());
            filter = filter
                    .AndIf(o => o.ShippingMethodId == param.ShippingMethodId.Value, param.ShippingMethodId.HasValue)
                    .AndIf(o => o.CreatedOn >= param.CreatedOnFrom.Value, param.CreatedOnFrom.HasValue)
                    .AndIf(o => o.CreatedOn <= param.CreatedOnTo.Value, param.CreatedOnTo.HasValue)
                    .AndIf(o => o.CustomerOrderID <= maxCustomerOrderId, maxCustomerOrderId > 0)
                    ;
            //}



            ////异常类型只查询，拦截与入仓异常状态
            //var lsitType = new List<int>()
            //    {
            //        WayBill.AbnormalTypeEnum.Intercepted.GetAbnormalTypeValue(),
            //        WayBill.AbnormalTypeEnum.OutAbnormal.GetAbnormalTypeValue()
            //    };
            //lsitType.Contains(l.OperateType)
            //查询异常状态为未完成的
            int abnormalStatus = WayBill.AbnormalStatusEnum.NO.GetAbnormalStatusValue();

            list = from w in ctx.WayBillInfos.Where(filterWayBill)
                   join l in ctx.AbnormalWayBillLogs on w.AbnormalID equals l.AbnormalID
                   join o in ctx.CustomerOrderInfos.Where(filter) on w.CustomerOrderID equals o.CustomerOrderID
                   where o.IsHold==true && l.AbnormalStatus == abnormalStatus
                   orderby o.CustomerOrderID descending
                   select new CustomerOrderInfoExt
                       {
                           CustomerCode = o.CustomerCode,
                           CustomerOrderID = o.CustomerOrderID,
                           CustomerOrderNumber = o.CustomerOrderNumber,
                           WayBillNumber = w.WayBillNumber,
                           Status = o.Status,
                           TrackingNumber = o.TrackingNumber,
                           CountryCode = w.CountryCode,
                           ShippingMethodId = w.InShippingMethodID,
                           ShippingMethodName = w.InShippingMethodName,
                           CreatedOn = o.CreatedOn,
                           AbnormalDescription = l.AbnormalDescription,
                           RawTrackingNumber = w.RawTrackingNumber,
                           TransferOrderDate = w.TransferOrderDate
                       };


            return list.ToPagedList(param.Page, param.PageSize);


        }


        public PagedList<CustomerOrderInfoExt> GetCustomerOrderInfoList(CustomerOrderParam param, int maxCustomerOrderId = 0)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            IQueryable<CustomerOrderInfoExt> list;
            int deleteSatus = CustomerOrder.StatusEnum.Delete.GetStatusValue();
            Expression<Func<CustomerOrderInfo, bool>> filter = o => true;
            Expression<Func<WayBillInfo, bool>> filterWayBill = o => true;
            filter = filter.AndIf(o => o.IsHold == param.IsHold.Value, param.IsHold.HasValue)
                  .AndIf(o => o.CustomerCode == param.CustomerCode, !param.CustomerCode.IsNullOrWhiteSpace())
                  .AndIf(o => o.Status == param.Status.Value, param.Status.HasValue)
                  .AndIf(o => o.Status != deleteSatus, !param.Status.HasValue)
                  .AndIf(o => o.CustomerOrderID <= maxCustomerOrderId, maxCustomerOrderId > 0)
                  ;
            //Expression<Func<InStorageInfo, bool>> filterInStorage = o => true;
            //Expression<Func<OutStorageInfo, bool>> filterOutStorage = o => true;
            //Func<IQueryable<WayBillInfo>, IOrderedQueryable<WayBillInfo>> orderFunc;
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
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.WayBillNumber));
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
            else if (!param.CustomerOrderNumber.IsNullOrWhiteSpace())
            {
                var numberList =
                    param.CustomerOrderNumber.Split(Environment.NewLine.ToCharArray(),
                                                    StringSplitOptions.RemoveEmptyEntries)
                         .ToList();

                if (numberList.Count > 0)
                {
                    string number = numberList[0];
                    filter = numberList.Count == 1
                                 ? filter.And(o => o.CustomerOrderNumber == number)
                                 : filter.And(o => numberList.Contains(o.CustomerOrderNumber));
                }
            }

            if (param.IsReceived)
            {
                filterWayBill = filterWayBill
                                .AndIf(o => o.InStorageCreatedOn >= param.CreatedOnFrom.Value, param.CreatedOnFrom.HasValue)
                                .AndIf(o => o.InStorageCreatedOn <= param.CreatedOnTo.Value, param.CreatedOnTo.HasValue);
            }
            else if (param.IsDeliver)
            {
                filterWayBill = filterWayBill
                                .AndIf(o => o.OutStorageCreatedOn >= param.CreatedOnFrom.Value, param.CreatedOnFrom.HasValue)
                                .AndIf(o => o.OutStorageCreatedOn <= param.CreatedOnTo.Value, param.CreatedOnTo.HasValue);
            }
            else
            {
                filter = filter
                        .AndIf(o => o.CreatedOn >= param.CreatedOnFrom.Value, param.CreatedOnFrom.HasValue)
                        .AndIf(o => o.CreatedOn <= param.CreatedOnTo.Value, param.CreatedOnTo.HasValue);
            }

            filter = filter
                .AndIf(o => o.ShippingMethodId == param.ShippingMethodId.Value, param.ShippingMethodId.HasValue)
                .AndIf(o => o.ShippingInfo.CountryCode == param.CountryCode, !param.CountryCode.IsNullOrWhiteSpace())
                .AndIf(o => o.IsPrinted == param.IsPrinted.Value, param.IsPrinted.HasValue);
            //}



            if (param.SearchWhere.HasValue && !string.IsNullOrWhiteSpace(param.SearchContext))
            {
                if (param.IsDeliver)
                {
                    list = from w in ctx.WayBillInfos.Where(filterWayBill)
                           join o in ctx.CustomerOrderInfos.Where(filter) on w.CustomerOrderID equals o.CustomerOrderID
                           orderby w.OutStorageCreatedOn descending
                           select new CustomerOrderInfoExt
                           {
                               CustomerCode = o.CustomerCode,
                               CustomerOrderID = o.CustomerOrderID,
                               CustomerOrderNumber = o.CustomerOrderNumber,
                               WayBillNumber = w.WayBillNumber,
                               Status = o.Status,
                               TrackingNumber = o.TrackingNumber,
                               RawTrackingNumber = w.RawTrackingNumber,
                               CountryCode = w.CountryCode,
                               Weight = w.Weight ?? 0,
                               SettleWeight = w.SettleWeight,
                               Width = w.Width ?? 0,
                               Height = w.Height ?? 0,
                               Length = w.Length ?? 0,
                               ShippingMethodId = w.InShippingMethodID,
                               ShippingMethodName = w.InShippingMethodName,
                               CreatedOn = w.OutStorageCreatedOn ?? DateTime.Now,
                               TransferOrderDate = w.TransferOrderDate,
                           };
                }
                else if (param.IsReceived)
                {
                    list = from w in ctx.WayBillInfos.Where(filterWayBill)
                           //join i in ctx.InStorageInfos.Where(filterInStorage) on w.InStorageID equals i.InStorageID
                           join o in ctx.CustomerOrderInfos.Where(filter) on w.CustomerOrderID equals o.CustomerOrderID
                           orderby w.InStorageCreatedOn descending
                           select new CustomerOrderInfoExt
                           {
                               CustomerCode = o.CustomerCode,
                               CustomerOrderID = o.CustomerOrderID,
                               CustomerOrderNumber = o.CustomerOrderNumber,
                               WayBillNumber = w.WayBillNumber,
                               Status = o.Status,
                               TrackingNumber = o.TrackingNumber,
                               RawTrackingNumber = w.RawTrackingNumber,
                               CountryCode = w.CountryCode,
                               Weight = w.Weight ?? 0,
                               SettleWeight = w.SettleWeight,
                               Width = w.Width ?? 0,
                               Height = w.Height ?? 0,
                               Length = w.Length ?? 0,
                               ShippingMethodId = w.InShippingMethodID,
                               ShippingMethodName = w.InShippingMethodName,
                               CreatedOn = w.InStorageCreatedOn ?? DateTime.Now,
                               TransferOrderDate = w.TransferOrderDate,
                           };
                }
                else
                {
                    list = from w in ctx.WayBillInfos.Where(filterWayBill)
                           join o in ctx.CustomerOrderInfos.Where(filter) on w.CustomerOrderID equals o.CustomerOrderID
                           orderby o.CreatedOn descending
                           select new CustomerOrderInfoExt
                           {
                               CustomerCode = o.CustomerCode,
                               CustomerOrderID = o.CustomerOrderID,
                               CustomerOrderNumber = o.CustomerOrderNumber,
                               WayBillNumber = w.WayBillNumber,
                               Status = o.Status,
                               TrackingNumber = o.TrackingNumber,
                               RawTrackingNumber = w.RawTrackingNumber,
                               CountryCode = w.CountryCode,
                               Weight = w.Weight ?? 0,
                               SettleWeight = w.SettleWeight,
                               Width = w.Width ?? 0,
                               Height = w.Height ?? 0,
                               Length = w.Length ?? 0,
                               ShippingMethodId = w.InShippingMethodID,
                               ShippingMethodName = w.InShippingMethodName,
                               CreatedOn = o.CreatedOn,
                               TransferOrderDate = w.TransferOrderDate,
                               Remark = o.Remark,
                           };
                }

            }
            else
            {
                //if (param.IsDeliver)
                //{
                //    list = from o in ctx.CustomerOrderInfos.Where(filter)
                //           join w in ctx.WayBillInfos.Where(filterWayBill) on o.CustomerOrderID equals w.CustomerOrderID
                //           into Grp
                //           from grp in Grp.DefaultIfEmpty()
                //           orderby grp.TransferOrderDate descending, grp.CreatedOn descending
                //           select new CustomerOrderInfoExt
                //           {
                //               CustomerCode = o.CustomerCode,
                //               CustomerOrderID = o.CustomerOrderID,
                //               CustomerOrderNumber = o.CustomerOrderNumber,
                //               WayBillNumber = grp.WayBillNumber,
                //               Status = o.Status,
                //               TrackingNumber = o.TrackingNumber,
                //               RawTrackingNumber = grp.RawTrackingNumber,
                //               CountryCode = o.ShippingInfo.CountryCode,
                //               Weight = grp.Weight ?? 0,
                //               SettleWeight = grp.SettleWeight,
                //               Width = grp.Width ?? 0,
                //               Height = grp.Height ?? 0,
                //               Length = grp.Length ?? 0,
                //               ShippingMethodId = o.ShippingMethodId,
                //               ShippingMethodName = o.ShippingMethodName,
                //               CreatedOn = grp.CreatedOn,
                //               TransferOrderDate = grp.TransferOrderDate,
                //           };
                //}else if (param.IsReceived)
                //{
                //    list = from o in ctx.CustomerOrderInfos.Where(filter)
                //           join w in ctx.WayBillInfos.Where(filterWayBill) on o.CustomerOrderID equals w.CustomerOrderID 
                //           join i in ctx.InStorageInfos.Where(filterInStorage) on w.InStorageID equals i.InStorageID into Grp
                //           from grp in Grp.DefaultIfEmpty()
                //           orderby w.TransferOrderDate descending, grp.CreatedOn descending
                //           select new CustomerOrderInfoExt
                //           {
                //               CustomerCode = o.CustomerCode,
                //               CustomerOrderID = o.CustomerOrderID,
                //               CustomerOrderNumber = o.CustomerOrderNumber,
                //               WayBillNumber = w.WayBillNumber,
                //               Status = o.Status,
                //               TrackingNumber = o.TrackingNumber,
                //               RawTrackingNumber = w.RawTrackingNumber,
                //               CountryCode = o.ShippingInfo.CountryCode,
                //               Weight = w.Weight ?? 0,
                //               SettleWeight = w.SettleWeight,
                //               Width = w.Width ?? 0,
                //               Height = w.Height ?? 0,
                //               Length = w.Length ?? 0,
                //               ShippingMethodId = o.ShippingMethodId,
                //               ShippingMethodName = o.ShippingMethodName,
                //               CreatedOn = grp.CreatedOn,
                //               TransferOrderDate = w.TransferOrderDate,
                //           };
                //}
                //else
                //{
                if (param.Status.HasValue)
                {
                    switch (CustomerOrder.ParseToStatus(param.Status.Value))
                    {
                        case CustomerOrder.StatusEnum.None:
                        case CustomerOrder.StatusEnum.OK:
                        case CustomerOrder.StatusEnum.SubmitFail:
                            list = from o in ctx.CustomerOrderInfos.Where(filter)
                                   orderby o.CreatedOn descending
                                   select new CustomerOrderInfoExt
                                   {
                                       CustomerCode = o.CustomerCode,
                                       CustomerOrderID = o.CustomerOrderID,
                                       CustomerOrderNumber = o.CustomerOrderNumber,
                                       WayBillNumber = "",
                                       Status = o.Status,
                                       TrackingNumber = o.TrackingNumber,
                                       RawTrackingNumber = "",
                                       CountryCode = "",
                                       Weight = o.Weight,
                                       SettleWeight = 0,
                                       Width = o.Width,
                                       Height = o.Height,
                                       Length = o.Length,
                                       ShippingMethodId = o.ShippingMethodId,
                                       ShippingMethodName = o.ShippingMethodName,
                                       CreatedOn = o.CreatedOn,
                                       TransferOrderDate = null,
                                       Remark = o.Remark,
                                   };
                            break;
                        default:
                            list = from o in ctx.CustomerOrderInfos.Where(filter)
                                   join w in ctx.WayBillInfos.Where(filterWayBill) on o.CustomerOrderID equals w.CustomerOrderID
                                       into Grp
                                   from grp in Grp.DefaultIfEmpty()
                                   orderby grp.TransferOrderDate descending, o.CreatedOn descending,grp.WayBillNumber descending 
                                   select new CustomerOrderInfoExt
                                   {
                                       CustomerCode = o.CustomerCode,
                                       CustomerOrderID = o.CustomerOrderID,
                                       CustomerOrderNumber = o.CustomerOrderNumber,
                                       WayBillNumber = grp.WayBillNumber,
                                       Status = o.Status,
                                       TrackingNumber = o.TrackingNumber,
                                       RawTrackingNumber = grp.RawTrackingNumber,
                                       CountryCode = grp.CountryCode,
                                       Weight = grp.Weight ?? 0,
                                       SettleWeight = grp.SettleWeight,
                                       Width = grp.Width ?? 0,
                                       Height = grp.Height ?? 0,
                                       Length = grp.Length ?? 0,
                                       ShippingMethodId = o.ShippingMethodId,
                                       ShippingMethodName = o.ShippingMethodName,
                                       CreatedOn = o.CreatedOn,
                                       TransferOrderDate = grp.TransferOrderDate,
                                       Remark = o.Remark,
                                   };
                            break;
                    }

                }
                else
                {
                    list = from o in ctx.CustomerOrderInfos.Where(filter)
                           join w in ctx.WayBillInfos.Where(filterWayBill) on o.CustomerOrderID equals w.CustomerOrderID
                               into Grp
                           from grp in Grp.DefaultIfEmpty()
                           orderby grp.TransferOrderDate descending, o.CreatedOn descending, grp.WayBillNumber descending 
                           select new CustomerOrderInfoExt
                               {
                                   CustomerCode = o.CustomerCode,
                                   CustomerOrderID = o.CustomerOrderID,
                                   CustomerOrderNumber = o.CustomerOrderNumber,
                                   WayBillNumber = grp.WayBillNumber,
                                   Status = o.Status,
                                   TrackingNumber = o.TrackingNumber,
                                   RawTrackingNumber = grp.RawTrackingNumber,
                                   CountryCode = grp.CountryCode,
                                   Weight = grp.Weight ?? 0,
                                   SettleWeight = grp.SettleWeight,
                                   Width = grp.Width ?? 0,
                                   Height = grp.Height ?? 0,
                                   Length = grp.Length ?? 0,
                                   ShippingMethodId = o.ShippingMethodId,
                                   ShippingMethodName = o.ShippingMethodName,
                                   CreatedOn = o.CreatedOn,
                                   TransferOrderDate = grp.TransferOrderDate,
                                   Remark = o.Remark,
                               };
                }
                //}

            }

            return list.ToPagedList(param.Page, param.PageSize);
        }

        public void BatchAdd(List<CustomerOrderInfo> orderInfos)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Debug.Assert(ctx != null, "ctx != null");

            ctx.Configuration.AutoDetectChangesEnabled = false;
            ctx.Configuration.ValidateOnSaveEnabled = false;

            foreach (var orderInfo in orderInfos)
            {
                Check.Argument.IsNotNull(orderInfo, "CustomerOrderInfo");
                Check.Argument.IsNotNull(orderInfo.ShippingInfo, "CustomerOrderInfo.ShippingInfo发货地址");
                Check.Argument.IsNotNull(orderInfo.ApplicationInfos, "CustomerOrderInfo.ApplicationInfos申报信息");

                orderInfo.Status = CustomerOrder.StatusEnum.None.GetStatusValue();
                orderInfo.CustomerOrderStatuses.Add(new CustomerOrderStatus { Status = CustomerOrder.StatusEnum.None.GetStatusValue(), CreatedOn = orderInfo.CreatedOn, Remark = "客户创建" });
                ctx.CustomerOrderInfos.Add(orderInfo);

            }
            List<string> customerNumber = new List<string>();
            List<CustomerOrderInfo> customerNumbers = new List<CustomerOrderInfo>();
            foreach (var row in orderInfos)
            {
                customerNumber.Add(row.CustomerOrderNumber.ToUpperInvariant());
            }
            var deleteStatus = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Delete);
            var returnStatus = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Return);
            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                customerNumbers = GetList(p => customerNumber.Contains(p.CustomerOrderNumber) && p.Status != deleteStatus && p.Status != returnStatus);
                foreach (var row in orderInfos)
                {
                    if (customerNumbers.FindAll(o => o.CustomerOrderNumber.ToUpper() == row.CustomerOrderNumber.ToUpper()).Count > 0)
                    {
                        throw new Exception("插入重复订单号:" + row.CustomerOrderNumber);
                    }
                }
                ctx.SaveChanges();
                transaction.Complete();
            }
        }



        public void BatchAdd(List<WayBillInfo> wayBillInfos, bool bTransaction)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;

            Debug.Assert(ctx != null, "ctx != null");

            ctx.Configuration.AutoDetectChangesEnabled = false;
            ctx.Configuration.ValidateOnSaveEnabled = false;

            if (bTransaction)
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required))
                {

                    BatchAddWayBillInfo(ctx, wayBillInfos);
                    transaction.Complete();
                }
            }
            else
            {
                BatchAddWayBillInfo(ctx, wayBillInfos);
            }
        }

        private void BatchAddWayBillInfo(LMS_DbContext ctx, IEnumerable<WayBillInfo> wayBillInfos)
        {
            foreach (var wayBillInfo in wayBillInfos)
            {
                Check.Argument.IsNotNull(wayBillInfo, "WayBillInfo");
                Check.Argument.IsNotNull(wayBillInfo.CustomerOrderInfo, "WayBillInfo.CustomerOrderInfo客户订单");
                Check.Argument.IsNotNull(wayBillInfo.ShippingInfo, "WayBillInfo.ShippingInfo收件人地址");
                Check.Argument.IsNotNull(wayBillInfo.SenderInfo, "WayBillInfo.SenderInfo发件人地址");
                Check.Argument.IsNotNull(wayBillInfo.ApplicationInfos, "WayBillInfo.ApplicationInfos申报信息");

                wayBillInfo.Status = WayBill.StatusEnum.Submitted.GetStatusValue();
                wayBillInfo.CustomerOrderInfo.Status = CustomerOrder.StatusEnum.None.GetStatusValue();

                wayBillInfo.CustomerOrderInfo.CustomerOrderStatuses.Add(new CustomerOrderStatus { Status = CustomerOrder.StatusEnum.None.GetStatusValue(), CreatedOn = wayBillInfo.CustomerOrderInfo.CreatedOn, Remark = "客户创建" });

                ctx.WayBillInfos.Add(wayBillInfo);

            }
            ctx.SaveChanges();
        }


        /// <summary>
        /// 根据订单号获取订单信息
        /// </summary>
        /// <param name="customerOrderNumber">订单数组</param>
        /// <returns></returns>
        public List<CustomerOrderInfoExt> GetCustomerOrderList(string[] customerOrderNumber, string customerCode)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            Check.Argument.IsNotNull(customerOrderNumber, "订单号");

            int deleteSatus = CustomerOrder.StatusEnum.Delete.GetStatusValue();
            int returnSatus = CustomerOrder.StatusEnum.Return.GetStatusValue();
            Expression<Func<CustomerOrderInfo, bool>> filter = o => (o.Status != deleteSatus && o.Status != returnSatus);

            Expression<Func<WayBillInfo, bool>> filterWayBill = o => o.CustomerCode == customerCode;
            filter = filter
                .AndIf(o => customerOrderNumber.Contains(o.CustomerOrderNumber), customerOrderNumber.Length > 0);

            var list = from w in ctx.WayBillInfos.Where(filterWayBill)
                       join o in ctx.CustomerOrderInfos.Where(filter) on w.CustomerOrderID equals o.CustomerOrderID
                       orderby o.CreatedOn descending
                       select new CustomerOrderInfoExt
                       {
                           CustomerCode = o.CustomerCode,
                           CustomerOrderID = o.CustomerOrderID,
                           CustomerOrderNumber = o.CustomerOrderNumber,
                           WayBillNumber = w.WayBillNumber,
                           Status = o.Status,
                           TrackingNumber = w.TrackingNumber,
                           CountryCode = w.CountryCode,
                           ShippingMethodId = w.InShippingMethodID,
                           ShippingMethodName = w.InShippingMethodName,
                           CreatedOn = o.CreatedOn
                       };
            return list.ToList();
        }


        /// <summary>
        /// 根据订单号获取订单信息
        /// </summary>
        /// <param name="wayBillNumbers">运单数组</param>
        /// <param name="customerCode"></param>
        /// <returns></returns>
        public List<CustomerOrderInfo> GetCustomerOrderListByWayBillNumber(string[] wayBillNumbers, string customerCode)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            Check.Argument.IsNotNull(wayBillNumbers, "订单号");

            int deleteSatus = CustomerOrder.StatusEnum.Delete.GetStatusValue();
            Expression<Func<CustomerOrderInfo, bool>> filter = o => o.Status != deleteSatus;
            Expression<Func<WayBillInfo, bool>> filterWayBill = o => o.CustomerCode == customerCode;
            filterWayBill = filterWayBill
                .AndIf(o => wayBillNumbers.Contains(o.WayBillNumber), wayBillNumbers.Length > 0);

            var list = from w in ctx.WayBillInfos.Where(filterWayBill)
                       join o in ctx.CustomerOrderInfos.Where(filter) on w.CustomerOrderID equals o.CustomerOrderID
                       orderby o.CreatedOn descending
                       select o;
            return list.ToList();
        }

        /// <summary>
        /// Add by zhengsong
        /// </summary> 订单导出
        /// <param name="filter">查询条件</param>
        /// <returns></returns>
        public List<CustomerOrderInfoExportExt> GetCustomerOrderInfoList(Expression<Func<CustomerOrderInfo, bool>> filter)
        {
            int status = CustomerOrder.StatusEnum.Send.GetStatusValue();
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var list = from c in ctx.CustomerOrderInfos.Where(filter)
                       join ship in ctx.ShippingInfos on c.ShippingInfoID equals ship.ShippingInfoID
                       into cslist
                       from csl in cslist.DefaultIfEmpty()
                       join send in ctx.SenderInfos on c.SenderInfoID equals send.SenderInfoID
                       into csslist
                       from cssl in csslist.DefaultIfEmpty()
                       join w in ctx.WayBillInfos on c.CustomerOrderID equals w.CustomerOrderID
                       into csswlist
                       from cssw in csswlist.DefaultIfEmpty()
                       join sta in ctx.CustomerOrderStatuses.Where(p => p.Status == status) on c.CustomerOrderID equals
                           sta.CustomerOrderID
                           into lists
                       from l in lists.DefaultIfEmpty()
                       orderby c.CreatedOn
                       select new CustomerOrderInfoExportExt
                           {
                               GoodsTypeID = c.GoodsTypeID,
                               CustomerOrderID = c.CustomerOrderID,
                               CustomerOrderNumber = c.CustomerOrderNumber,
                               WayBillNumber = cssw.WayBillNumber,
                               CustomerCode = c.CustomerCode,
                               CountryCode = csl.CountryCode,
                               ShippingFirstName = csl.ShippingFirstName,
                               ShippingLastName = csl.ShippingLastName,
                               ShippingAddress = csl.ShippingAddress,
                               ShippingCity = csl.ShippingCity,
                               ShippingState = csl.ShippingState,
                               ShippingZip = csl.ShippingZip,
                               ShippingTaxId = csl.ShippingTaxId,
                               ShippingCompany = csl.ShippingCompany,
                               ShippingPhone = csl.ShippingPhone,
                               SenderFirstName = cssl.SenderFirstName,
                               SenderLastName = cssl.SenderLastName,
                               SenderAddress = cssl.SenderAddress,
                               SenderCompany = cssl.SenderCompany,
                               SenderCity = cssl.SenderCity,
                               SenderState = cssl.SenderState,
                               SenderZip = cssl.SenderZip,
                               SenderPhone = cssl.SenderPhone,
                               ShippingMethodName = c.ShippingMethodName,
                               ShippingMethodId = c.ShippingMethodId,
                               Weight = c.Weight,
                               Length = c.Length,
                               Width = c.Width,
                               Height = c.Height,
                               PackageNumber = c.PackageNumber,
                               InsuredID = c.InsuredID,
                               SensitiveTypeID = c.SensitiveTypeID,
                               CreatedOn = c.CreatedOn,
                               DeliveryDate = l.CreatedOn,
                               TrackingNumber = cssw.TrackingNumber,
                               SettleWeight = cssw.SettleWeight,
                               RawTrackingNumber = cssw.RawTrackingNumber,
                               TransferOrderDate = cssw.TransferOrderDate,
                               InsureAmount = c.InsureAmount,
                               AppLicationType = c.AppLicationType,
                           };
            return list.ToList();
        }

        public bool DeleteCustomerOrderList(string customerCode, List<int> selected)
        {
            bool reuslt = false;
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            string customerorderid = "";

            if (selected != null && selected.Count != 0)
            {
                customerorderid = string.Join(",", selected);
            }
            var _customerCode = new SqlParameter { ParameterName = "CustomerCode", Value = customerCode, DbType = DbType.String };
            var _ccustomerorderId = new SqlParameter { ParameterName = "CustomerOrderID", Value = customerorderid, DbType = DbType.String };

            if (ctx != null)
            {
                var obj = ctx.ExecuteStoredProcedureList("P_DeleteCustomerOrderInfo", _customerCode,
                                                         _ccustomerorderId);

                reuslt = obj;
            }
            return reuslt;
        }

        public void ModifCustomerOrderToSubmiting(int customerOrderId, string updatedBy)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            int submitingStatus = (int)CustomerOrder.StatusEnum.Submiting;
            int oKStatus = (int)CustomerOrder.StatusEnum.OK;
            int submitFailStatus = (int)CustomerOrder.StatusEnum.SubmitFail;

            var _submitingStatus = new SqlParameter { ParameterName = "submitingStatus", Value = submitingStatus, DbType = DbType.Int32 };
            var _oKStatus = new SqlParameter { ParameterName = "oKStatus ", Value = oKStatus, DbType = DbType.Int32 };
            var _submitFailStatus = new SqlParameter { ParameterName = "submitFailStatus", Value = submitFailStatus, DbType = DbType.Int32 };
            var _customerOrderId = new SqlParameter { ParameterName = "customerOrderId", Value = customerOrderId, DbType = DbType.Int32 };
            var _lastUpdatedBy = new SqlParameter { ParameterName = "lastUpdatedBy ", Value = updatedBy, DbType = DbType.String };

            ctx.ExecuteCommand(
                "update dbo.CustomerOrderInfos set Status=@submitingStatus,LastUpdatedBy=@lastUpdatedBy where  CustomerOrderID=@customerOrderId and (Status=@oKStatus or Status=@submitFailStatus)",
                _submitingStatus, _customerOrderId, _oKStatus, _submitFailStatus, _lastUpdatedBy);
        }

        public void ModifCustomerOrderToSubmiting(int[] customerOrderId, string updatedBy)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            int submitingStatus = (int)CustomerOrder.StatusEnum.Submiting;
            int oKStatus = (int)CustomerOrder.StatusEnum.OK;
            int submitFailStatus = (int)CustomerOrder.StatusEnum.SubmitFail;

            var _submitingStatus = new SqlParameter { ParameterName = "submitingStatus", Value = submitingStatus, DbType = DbType.Int32 };
            var _oKStatus = new SqlParameter { ParameterName = "oKStatus ", Value = oKStatus, DbType = DbType.Int32 };
            var _submitFailStatus = new SqlParameter { ParameterName = "submitFailStatus", Value = submitFailStatus, DbType = DbType.Int32 };
            var _lastUpdatedBy = new SqlParameter { ParameterName = "lastUpdatedBy ", Value = updatedBy, DbType = DbType.String };

            var customerOrderIds = string.Join(",", customerOrderId.Select(p => p.ToString()).ToArray());

            ctx.ExecuteCommand(
                string.Format("update dbo.CustomerOrderInfos set Status=@submitingStatus,LastUpdatedBy=@lastUpdatedBy,LastUpdatedOn=getdate() where  CustomerOrderID in({0}) and (Status=@oKStatus or Status=@submitFailStatus)", customerOrderIds),
                _submitingStatus, _oKStatus, _submitFailStatus, _lastUpdatedBy);

            //_submitingStatus = new SqlParameter { ParameterName = "submitingStatus", Value = submitingStatus, DbType = DbType.Int32 };

            //var successCustomerOrderIDs = ctx.ExecuteQuery<int>(string.Format("select CustomerOrderID from dbo.CustomerOrderInfos where Status=@submitingStatus and CustomerOrderID in({0})", customerOrderIds),
            //                                                      _submitingStatus).ToList();

            //return successCustomerOrderIDs;
        }

        public List<int> SelectCustomerOrderSubmiting(int[] customerOrderId)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            int submitingStatus = (int)CustomerOrder.StatusEnum.Submiting;
            int submittedStatus = (int)CustomerOrder.StatusEnum.Submitted;

            var _submittedStatus = new SqlParameter { ParameterName = "submittedStatus ", Value = submittedStatus, DbType = DbType.Int32 };
            var _submitingStatus = new SqlParameter { ParameterName = "submitingStatus", Value = submitingStatus, DbType = DbType.Int32 };

            var customerOrderIds = string.Join(",", customerOrderId.Select(p => p.ToString()).ToArray());

            var successCustomerOrderIDs = ctx.ExecuteQuery<int>(string.Format("select CustomerOrderID from dbo.CustomerOrderInfos with(xlock) where (Status=@submitingStatus or Status=@submittedStatus) and CustomerOrderID in({0})", customerOrderIds),
                                                                  _submitingStatus, _submittedStatus).ToList();

            return successCustomerOrderIDs;
        }

        public int GetMaxCustomerOrderID()
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            return ctx.CustomerOrderInfos.Max(p => p.CustomerOrderID);
        }
    }

}
