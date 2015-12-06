using System;
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
    public partial class ReturnGoodsRepository
    {
        /// <summary>
        /// 获取退货信息列表
        /// Add by zhengsong
        /// Time:2014-05-16
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IPagedList<ReturnWayBillModelExt> GetPagedListReturnWayBill(ReturnWayBillParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            IQueryable<ReturnWayBillModelExt> list;
            Expression<Func<ReturnGoods, bool>> filter = o => true;
            Expression<Func<WayBillInfo, bool>> filterWayBill = o => true;

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
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.CustomerOrderNumber));
                            break;
                    }
                }
                filter = filter.AndIf(r => r.Status == param.Status, param.Status.HasValue);
            }else
            {
                filter = filter.AndIf(r => r.CreatedOn >= param.ReturnStartTime, param.ReturnStartTime.HasValue)
                      .AndIf(r => r.CreatedOn <= param.ReturnEndTime, param.ReturnEndTime.HasValue)
                      .AndIf(r => r.CreatedBy.Contains(param.CreateBy), !param.CreateBy.IsNullOrWhiteSpace())
                      .AndIf(r => r.Status == param.Status,param.Status.HasValue)
                      .AndIf(r=>r.IsReturnShipping == param.IsReturnShipping,param.IsReturnShipping.HasValue)
                      .AndIf(r=>param.ReturnReason.Contains(r.Reason),!param.ReturnReason.IsNullOrWhiteSpace());

                filterWayBill = filterWayBill.AndIf(w => w.CustomerCode == param.CustomerCode, !param.CustomerCode.IsNullOrWhiteSpace())
                                             .AndIf(w => w.InShippingMethodID == param.InShippingMehtodId, param.InShippingMehtodId != 0);
            }
            list = from r in ctx.ReturnGoods.Where(filter)
                   join w in ctx.WayBillInfos.Where(filterWayBill) on r.WayBillNumber equals w.WayBillNumber
                   join c in ctx.Customers on r.CreatedBy equals c.CustomerCode
                   into rwcList
                   from rwc in rwcList.DefaultIfEmpty()
                   join g in ctx.Countries on w.CountryCode equals g.CountryCode
                   into rwcgList
                   from rList in rwcgList.DefaultIfEmpty()
                   join i in ctx.InStorageInfos on w.InStorageID equals i.InStorageID
                   into ilist
                   from il in ilist.DefaultIfEmpty()
                   join o in ctx.OutStorageInfos on w.OutStorageID equals o.OutStorageID
                   into newList
                   from n in newList.DefaultIfEmpty()
                   orderby r.CreatedOn descending
                   select new ReturnWayBillModelExt
                       {
                           CustomerName = w.CustomerCode,
                           WayBillNumber = r.WayBillNumber,
                           CustomerOrderNumber = w.CustomerOrderNumber,
                           InShippingMehtodName = w.InShippingMethodName,
                           TrackingNumber = w.TrackingNumber,
                           TotalWeight = r.Weight,
                           CountryCode = w.CountryCode,
                           ChineseName = rList.ChineseName,
                           ShippingFee = r.ShippingFee,
                           Type = r.Type,
                           Reason = r.Reason,
                           ReasonRemark = r.ReasonRemark,
                           IsReturnShipping = r.IsReturnShipping,
                           OutCreatedOn = n.CreatedOn,
                           ReturnCreatedOn = r.CreatedOn,
                           CreatedBy = r.CreatedBy,
                           CreatedByEeName = rwc.AccountID,
                           Auditor = r.Auditor,
                           AuditorDate = r.AuditorDate
                       };
            return list.ToPagedList(param.Page,param.PageSize);
        }
        /// <summary>
        /// 获取退货信息集合
        /// Add by zhengsong
        /// Time:2014-05-16
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<ReturnWayBillModelExt> GetReturnWayBillList(ReturnWayBillParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            Expression<Func<ReturnGoods, bool>> filter = o => true;
            Expression<Func<WayBillInfo, bool>> filterWayBill = o => true;

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
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.CustomerOrderNumber));
                            break;
                    }
                }
            }else
            {
                filter = filter.AndIf(r => r.CreatedOn >= param.ReturnStartTime, param.ReturnStartTime.HasValue)
                      .AndIf(r => r.CreatedOn <= param.ReturnEndTime, param.ReturnEndTime.HasValue)
                      .AndIf(r => r.CreatedBy.Contains(param.CreateBy), !param.CreateBy.IsNullOrWhiteSpace());
                filterWayBill = filterWayBill.AndIf(w => w.CustomerCode == param.CustomerCode, !param.CustomerCode.IsNullOrWhiteSpace())
                                             .AndIf(w => w.InShippingMethodID == param.InShippingMehtodId, param.InShippingMehtodId != 0);
            }
            filter = filter.AndIf(r => r.Status == param.Status, param.Status.HasValue);
            var list = from r in ctx.ReturnGoods.Where(filter)
                       join w in ctx.WayBillInfos.Where(filterWayBill) on r.WayBillNumber equals w.WayBillNumber
                       join c in ctx.Customers on r.CreatedBy equals c.CustomerCode
                           into rwcList
                       from rwc in rwcList.DefaultIfEmpty()
                       join g in ctx.Countries on w.CountryCode equals g.CountryCode
                           into rwcgList
                       from rList in rwcgList.DefaultIfEmpty()
                       join i in ctx.InStorageInfos on w.InStorageID equals i.InStorageID
                           into ilist
                       from il in ilist.DefaultIfEmpty()
                       join o in ctx.OutStorageInfos on w.OutStorageID equals o.OutStorageID
                           into newList
                       from n in newList.DefaultIfEmpty()
                       orderby r.CreatedOn descending
                       select new ReturnWayBillModelExt
                           {
                               CustomerName = w.CustomerCode,
                               WayBillNumber = r.WayBillNumber,
                               CustomerOrderNumber = w.CustomerOrderNumber,
                               InShippingMehtodName = w.InShippingMethodName,
                               TrackingNumber = w.TrackingNumber,
                               TotalWeight = r.Weight,
                               CountryCode = w.CountryCode,
                               ChineseName = rList.ChineseName,
                               ShippingFee = r.ShippingFee,
                               Type = r.Type,
                               Reason = r.Reason,
                               ReasonRemark = r.ReasonRemark,
                               IsReturnShipping = r.IsReturnShipping,
                               OutCreatedOn = n.CreatedOn,
                               ReturnCreatedOn = r.CreatedOn,
                               CreatedBy = r.CreatedBy,
                               CreatedByEeName = rwc.AccountID,
                               Auditor = r.Auditor,
                               AuditorDate = r.AuditorDate
                           };
            return list.ToList();
        }
    }
}
