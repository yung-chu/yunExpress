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
    public partial class OutStorageInfoRepository
    {
        public List<string> GetTotalPackageNumberList(string venderCode)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var dt = DateTime.Now.AddHours(2);
            //TraceEventCode等于4是代表到香港的事件时间
            return (from t in ctx.TotalPackageInfos.Where(p => p.VenderCode == venderCode)
                    where
                        !(from j in
                              ctx.TotalPackageTraceInfos.Where(
                                  p => p.TraceEventCode == 4 && (p.IsJob || p.TraceEventTime < dt))
                          select j.TotalPackageNumber).Contains(t.TotalPackageNumber)
                    select t.TotalPackageNumber).ToList();

        }

        public IPagedList<OutStorageInfoExt> GetOutStoragePagedList(OutStorageListSearchParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            var startTime = param.OutStartDate.HasValue ? param.OutStartDate.Value : new DateTime(2013, 1, 1);
            var endTime = param.OutEndDate.HasValue ? param.OutEndDate.Value : new DateTime(2020, 1, 1);
            Expression<Func<OutStorageInfo, bool>> filter = p => true;
            filter = filter.AndIf(p => p.VenderCode == param.VenderCode, !string.IsNullOrWhiteSpace(param.VenderCode))
                           .AndIf(p => p.OutStorageID.Contains(param.OutStorageID), !string.IsNullOrWhiteSpace(param.OutStorageID))
                           .And(p => p.CreatedOn >= startTime && p.CreatedOn <= endTime);


            var query = ctx.OutStorageInfos.Where(filter);

            if (!string.IsNullOrWhiteSpace(param.PostBagNumber))
            {
                query = query.Where(p => p.MailPostBagInfos.Any(e => e.PostBagNumber == param.PostBagNumber));
            }

            var qq = from o in query
                     join p in ctx.MailPostBagInfos
                     on o.OutStorageID equals p.OutStorageID
                     into x
                     from y in x.DefaultIfEmpty()
                     select new OutStorageInfoExt()
                     {
                         OutStorageID = o.OutStorageID,
                         Remark = o.Remark,
                         Surcharge = o.Surcharge,
                         Register = o.Register,
                         Status = o.Status,
                         TotalFee = o.TotalFee,
                         TotalQty = o.TotalQty,
                         TotalWeight = o.TotalWeight,
                         VenderCode = o.VenderCode,
                         CreatedBy = o.CreatedBy,
                         CreatedOn = o.CreatedOn,
                         DeliveryStaff = o.DeliveryStaff,
                         Freight = o.Freight,
                         FuelCharge = o.FuelCharge,
                         LastUpdatedBy = o.LastUpdatedBy,
                         LastUpdatedOn = o.LastUpdatedOn,
                         PostBagNumber = y.PostBagNumber                         
                     };
            qq = qq.OrderByDescending(p => p.CreatedOn);
            return qq.ToPagedList(param.Page, param.PageSize);
        }
    }
}
