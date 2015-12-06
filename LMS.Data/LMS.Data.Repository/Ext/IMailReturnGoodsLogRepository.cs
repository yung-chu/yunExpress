using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using LighTake.Infrastructure.Common;
using LMS.Data.Context;
using LMS.Data.Entity;


namespace LMS.Data.Repository
{
    public partial interface IMailReturnGoodsLogRepository
    {
        IPagedList<MailReturnGoodsLogsExt> GetMailReturnGoodsLogsList(MailReturnGoodsLogsParam param);
    }

    public partial class MailReturnGoodsLogRepository : IMailReturnGoodsLogRepository
    {
        public IPagedList<MailReturnGoodsLogsExt> GetMailReturnGoodsLogsList(MailReturnGoodsLogsParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            //query only
            ctx.Configuration.AutoDetectChangesEnabled = false;

            Expression<Func<MailReturnGoodsLog, bool>> filter = o => true;


            var numberList = new List<string>();
            if (!string.IsNullOrWhiteSpace(param.TrackNumber))
            {
                numberList =
                    param.TrackNumber
                    .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
            }

            if (numberList.Any())
            {
                filter = filter.And(a => numberList.Contains(a.TrackNumber));
            }
            else
            {
                filter = filter.AndIf(a => param.StartTime <= a.ReturnOn, param.StartTime.HasValue)
                    .AndIf(a => a.ReturnOn <= param.EndTime, param.EndTime.HasValue)
                    .AndIf(a => a.ReasonType == param.ReasonType, param.ReasonType.HasValue)
                    .AndIf(a => a.ReturnBy.Contains(param.ReturnBy), !string.IsNullOrEmpty((param.ReturnBy)));
            }


            var result = from a in ctx.MailReturnGoodsLogs.AsNoTracking().Where(filter)
                join b in ctx.WayBillInfos.AsNoTracking() on a.TrackNumber equals b.TrackingNumber
                orderby a.ReturnOn descending
                select new MailReturnGoodsLogsExt
                {
                    TrackNumber = a.TrackNumber,
                    ReasonType = a.ReasonType,
                    ReturnBy = a.ReturnBy,
                    ReturnOn = a.ReturnOn,
                    InShippingMethodID = b.InShippingMethodID.Value,
                    InShippingMethodName = b.InShippingMethodName,
                    Weight = b.Weight.HasValue ? b.Weight : 0,
                    CountryCode = b.CountryCode
                };



            return result.ToPagedList(param.Page,param.PageSize);
        }
    }
}
