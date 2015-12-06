using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using LMS.Data.Context;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial class InStorageInfoRepository
    {
        /// <summary>
        /// 物流快递打单列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>                                
        public PagedList<ExpressPrintWayBillExt> GetExpressPrintWayBillList(ExpressPrintWayBillParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            IQueryable<ExpressPrintWayBillExt> list;
            int satusHave = WayBill.StatusEnum.Have.GetStatusValue();
            int satusWaitOrder = WayBill.StatusEnum.WaitOrder.GetStatusValue();
            int satusSend = WayBill.StatusEnum.Send.GetStatusValue();
            Expression<Func<CustomerOrderInfo, bool>> filter = o => true;
            Expression<Func<WayBillInfo, bool>> filterWayBill = o => o.Status == satusHave || o.Status == satusWaitOrder || o.Status == satusSend;
            Expression<Func<InStorageInfo, bool>> filterInStorage = o => true;
            filter = filter.AndIf(p => p.CustomerCode == param.CustomerCode, param.CustomerCode != null);


            if (param.ShippingMethodType == 1)
            {
                filterWayBill = filterWayBill.AndIf(p => p.InShippingMethodID == param.ShippingMethodId,
                                                    param.ShippingMethodId != 0);
            }
            else if (param.ShippingMethodType == 2)
            {
                filterWayBill = filterWayBill.AndIf(p => p.OutShippingMethodID == param.ShippingMethodId,
                                                    param.ShippingMethodId != 0);
            }
            if (param.IsVender == 2)
            {
                filterWayBill = filterWayBill.And(p => p.VenderCode != null);
            }
            else if (param.IsVender == 3)
            {
                filterWayBill = filterWayBill.And(p => p.VenderCode == null);
            }
            filterWayBill = filterWayBill.AndIf(p => p.VenderCode == param.VenderCode, param.VenderCode != null)
                                         .AndIf(p => p.SettleWeight >= param.StartWeight.Value, param.StartWeight.HasValue)
                                         .AndIf(p => p.SettleWeight <= param.EndWeight.Value, param.EndWeight.HasValue);
            filterInStorage = filterInStorage
                                 .AndIf(o => o.CreatedOn >= param.StartTime.Value, param.StartTime.HasValue)
                                 .AndIf(o => o.CreatedOn <= param.EndTime.Value, param.EndTime.HasValue);

            if (!string.IsNullOrWhiteSpace(param.CountryCode))
            {
                var codes = param.CountryCode.Split(',').ToList();
                if (codes.Count > 0)
                {
                    filterWayBill = filterWayBill.And(p => codes.Contains(p.CountryCode));
                }
            }

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
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.TrackingNumber));
                            break;
                        case WayBill.SearchFilterEnum.CustomerOrderNumber:
                            filter = filter.And(p => numberList.Contains(p.CustomerOrderNumber));
                            break;
                    }
                }
            }

            IQueryable<WayBillInfo> queryWayBillInfos = from w in ctx.WayBillInfos.Where(filterWayBill) select w;

            //已打印
            if (param.PrintStatus == "1")
            {
                queryWayBillInfos = from w in ctx.WayBillInfos.Where(filterWayBill) where ctx.WayBillPrintLogs.Any(p => p.waybillnumber == w.WayBillNumber) select w;
            }
            //未打印
            if (param.PrintStatus == "2")
            {
                queryWayBillInfos = from w in ctx.WayBillInfos.Where(filterWayBill) where ctx.WayBillPrintLogs.All(p => p.waybillnumber != w.WayBillNumber) select w;
            }

            list = from w in queryWayBillInfos
                   join i in ctx.InStorageInfos.Where(filterInStorage) on w.InStorageID equals i.InStorageID
                   join o in ctx.CustomerOrderInfos.Where(filter) on w.CustomerOrderID equals o.CustomerOrderID
                   orderby i.CreatedOn descending
                   select new ExpressPrintWayBillExt
                   {
                       WayBillNumber = w.WayBillNumber,
                       CustomerOrderID = w.CustomerOrderID,
                       CustomerOrderNumber = o.CustomerOrderNumber,
                       CountryCode = w.CountryCode,
                       TrackingNumber = w.TrackingNumber,
                       SettleWeight = w.SettleWeight,
                       Status = w.Status,
                       CustomerCode = w.CustomerCode,
                       InShippingMethodID = w.InShippingMethodID,
                       OutShippingMethodID = w.OutShippingMethodID,
                       VenderCode = w.VenderCode,
                       VenderName = "",
                       CreatedOn = i.CreatedOn,
                       IsPrinted = ctx.WayBillPrintLogs.Any(p => p.waybillnumber == w.WayBillNumber),
                       IsPrinter = ctx.ExpressResponses.Any(p => p.WayBillNumber == w.WayBillNumber),
                   };
            return list.ToPagedList(param.Page, param.PageSize);
        }

        public PagedList<InStorageInfoExt> GetInStorageInfoExtPagedList(InStorageListSearchParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            var startTime = param.InStartDate.HasValue ? param.InStartDate.Value : new DateTime(2013, 1, 1);
            var endTime = param.InEndDate.HasValue ? param.InEndDate.Value : new DateTime(2020, 1, 1);
            Expression<Func<InStorageInfo, bool>> inStoragefilter = p => true;
            inStoragefilter = inStoragefilter.AndIf(p => p.CustomerCode == param.CustomerCode, !string.IsNullOrWhiteSpace(param.CustomerCode))
                                             .AndIf(p => p.InStorageID.Contains(param.InStorageID), !string.IsNullOrWhiteSpace(param.InStorageID))
                                             .And(p => p.CreatedOn >= startTime && p.CreatedOn <= endTime)
                                             .AndIf(p => p.WayBillInfos.FirstOrDefault().InShippingMethodID == param.ShippingMethodId, param.ShippingMethodId.HasValue);

            //Expression<Func<WayBillInfo, bool>> wayBillfilter = p => true;
            //wayBillfilter = wayBillfilter.AndIf(p => p.InShippingMethodID == param.ShippingMethodId, param.ShippingMethodId.HasValue);

            IQueryable<InStorageInfoExt> list = from w in ctx.InStorageInfos.Where(inStoragefilter)
                                                orderby w.CreatedOn descending
                                                select new InStorageInfoExt
                                                    {
                                                        InStorageID = w.InStorageID,
                                                        ReceivingClerk = w.ReceivingClerk,
                                                        CustomerCode = w.CustomerCode,
                                                        TotalWeight = w.TotalWeight,
                                                        PhysicalTotalWeight = w.PhysicalTotalWeight,
                                                        TotalQty = w.TotalQty,
                                                        MaterialsFee = w.MaterialsFee,
                                                        Status = w.Status,
                                                        TotalFee = w.TotalFee,
                                                        CreatedBy = w.CreatedBy,
                                                        LastUpdatedOn = w.LastUpdatedOn,
                                                        LastUpdatedBy = w.LastUpdatedBy,
                                                        Remark = w.Remark,
                                                        CreatedOn = w.CreatedOn,
                                                        Freight = w.Freight,
                                                        Register = w.Register,
                                                        FuelCharge = w.FuelCharge,
                                                        Surcharge = w.Surcharge,
                                                        InShippingMethodName = w.WayBillInfos.FirstOrDefault().InShippingMethodName
                                                    };
            return list.ToPagedList(param.Page, param.PageSize);
        }

        public InStorageInfo GetInStorageInfo(string InStorageId)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            ctx.Configuration.LazyLoadingEnabled = true;
            ctx.Configuration.AutoDetectChangesEnabled = false;
            return ctx.InStorageInfos.FirstOrDefault(t => t.InStorageID == InStorageId);
        }

        public string GetShippingMethodName(string InStorageId)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            ctx.Configuration.LazyLoadingEnabled = true;
            ctx.Configuration.AutoDetectChangesEnabled = false;
            return ctx.WayBillInfos.FirstOrDefault(t => t.InStorageID == InStorageId).InShippingMethodName;
        }

        public List<InStorageTotalModel> GetInStorageTotals(string InStorageId)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            ctx.Configuration.LazyLoadingEnabled = true;
            ctx.Configuration.AutoDetectChangesEnabled = false;

            var result = ctx.Database.SqlQuery<InStorageTotalModel>(@"SELECT
    CountryCode ,
    COUNT(0) PackCount ,
    SUM(SettleWeight) TotalWeight ,
    SUM(Weight) PhysicalTotalWeight
FROM
    dbo.WayBillInfos
WHERE
    InStorageID = @InStorageID
GROUP BY
    CountryCode", new System.Data.SqlClient.SqlParameter("InStorageID", InStorageId));

            if (result == null) return new List<InStorageTotalModel>();
            return result.ToList();
        }

        //判断是否生成结算清单
        public bool InStorageOrSettlementRelational(string customerCode)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;

            int status = (int) Settlement.StatusEnum.OK;
            //现结跟预付
            int paymentTypeId1 = 3;
            int paymentTypeId2 = 4;

            var inStorageInfos = (from w in ctx.InStorageInfos.Where(p => p.CustomerCode == customerCode && (p.PaymentTypeID == paymentTypeId1 || p.PaymentTypeID == paymentTypeId2))
                     select w.InStorageID);

            var inStorageOrSettlementRelationals =
                (from i in ctx.InStorageOrSettlementRelationals
                 where (from s in ctx.SettlementInfos where s.CustomerCode == customerCode && s.Status == status select s.SettlementNumber).Contains(i.SettlementNumber)
                select i.InStorageID);

            bool ret = true;
                inStorageInfos.ToList().ForEach(p =>
                    {
                        if (inStorageOrSettlementRelationals.FirstOrDefault(z => z == p) == null)
                        {
                            ret= false;
                        }
                    });
            return ret;
        }
    }
}
