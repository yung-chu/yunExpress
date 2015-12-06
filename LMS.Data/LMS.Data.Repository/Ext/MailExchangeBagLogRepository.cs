using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using LMS.Data.Context;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial class MailExchangeBagLogRepository
    {
        /// <summary>
        /// 验证包裹单号
        /// </summary>
        /// <param name="trackNumber"></param>
        /// <returns>0-验证成功,1-不存在，2-已退件</returns>
        public string CheckTrackNumber(string trackNumber)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            string result = "0";

            var sendstatus = WayBill.StatusToValue(WayBill.StatusEnum.Send);

            if (ctx.WayBillInfos.Any(p => p.TrackingNumber == trackNumber && p.Status == sendstatus))
            {
                if (ctx.MailReturnGoodsLogs.Any(p => p.TrackNumber == trackNumber))
                {
                    result = "2";
                }
            }
            else
            {
                result = "1";
            }
            return result;
        }


        /// <summary>
        /// 换袋记录查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IPagedList<MailExchangeBagLogsExt> GetMailExchangeBagLogsList(MailExchangeBagLogsParam param)
        {
            Expression<Func<MailExchangeBagLog, bool>> filter = o => true;
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
                filter = filter.AndIf(a => param.StartTime <= a.ExchangeTime, param.StartTime.HasValue)
                    .AndIf(a => a.ExchangeTime <= param.EndTime, param.EndTime.HasValue);
            }

            using (var ctx = new LMS_DbContext())
            {
                ctx.Configuration.AutoDetectChangesEnabled = false;
                var result = from a in ctx.MailExchangeBagLogs.AsNoTracking().Where(filter)
                             join b in ctx.WayBillInfos.AsNoTracking() on a.TrackNumber equals b.TrackingNumber
                             orderby a.ExchangeTime descending
                             select new MailExchangeBagLogsExt
                             {
                                 TrackNumber = a.TrackNumber,
                                 PostBagNumber = a.PostBagNumber, //旧云途邮袋号
                                 NewPostBagNumber = a.NewPostBagNumber, //新云途邮袋号
                                 ExchangeTime = a.ExchangeTime,
                                 RecordBy = a.RecordBy,
                                 CountryCode = b.CountryCode
                             };
                return result.ToPagedList(param.Page, param.PageSize);
            }
        }
        /// <summary>
        /// 验证目的袋牌
        /// </summary>
        /// <param name="bagNumber">目的袋牌</param>
        /// <param name="trackNumber">包裹单号</param>
        /// <returns>0-包裹单号不存在，1-该单号已退件,2-目的袋牌不存在，
        /// 3-该单号与目标袋牌国家不匹配,4-该目标袋牌重量已超重不能放入,
        /// 100-验证成功，6-该目标袋牌已在中心局扫描过</returns>
        public string CheckBagNumber(string bagNumber, string trackNumber)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var sendstatus = WayBill.StatusToValue(WayBill.StatusEnum.Send);

            var waybillinfo =
                ctx.WayBillInfos.SingleOrDefault(p => p.TrackingNumber == trackNumber && p.Status == sendstatus);
            if (waybillinfo == null)
            {
                //包裹单号不存在
                return "0";
            }
            if (ctx.MailReturnGoodsLogs.Any(p => p.TrackNumber == trackNumber))
            {
                //该单号已退件
                return "1";
            }
            var mailPostBagInfo =
                ctx.MailPostBagInfos.SingleOrDefault(p => p.PostBagNumber == bagNumber);
            if (mailPostBagInfo == null)
            {
                //目的袋牌不存在
                return "2";
            }
            if (waybillinfo.CountryCode != mailPostBagInfo.CountryCode)
            {
                //该单号与目标袋牌国家不匹配
                return "3";
            }
            if (waybillinfo.Weight + mailPostBagInfo.TotalWeight > 30)
            {
                //该目标袋牌重量已超重不能放入
                return "4";
            }
            if (ctx.MailTotalPackageOrPostBagRelationals.Any(p => p.PostBagNumber == mailPostBagInfo.PostBagNumber))
            {
                //该目标袋牌已在中心局扫描过
                return "6";
            }
            return "100";
        }
        /// <summary>
        /// 保存换袋记录
        /// </summary>
        /// <param name="bagNumber">目的袋牌</param>
        /// <param name="trackNumber">包裹单号</param>
        /// <param name="recordBy">操作人</param>
        /// <returns>-1-失败， 0-包裹单号不存在，1-该单号已退件,2-目的袋牌不存在，
        /// 3-该单号与目标袋牌国家不匹配,4-该目标袋牌重量已超重不能放入,5-该单号发货渠道错误，
        /// 100-验证成功，6-该目标袋牌已在中心局扫描过</returns>
        public int SacnPackageExchangeBag(string bagNumber, string trackNumber, string recordBy)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            var sendstatus = WayBill.StatusToValue(WayBill.StatusEnum.Send);

            var trackNumberParam = new SqlParameter{ParameterName = "trackNumber",Value = trackNumber, DbType = DbType.String};
            var bagNumberParam = new SqlParameter{ParameterName = "bagNumber",Value = bagNumber,DbType = DbType.String};
            var statusParam = new SqlParameter{ParameterName = "status",Value = sendstatus,DbType = DbType.Int32};
            var recordByParam = new SqlParameter{ParameterName = "recordBy",Value = recordBy,DbType = DbType.String};
            var result = new SqlParameter{ParameterName = "result",Value = 0,DbType = DbType.Int32,Direction = ParameterDirection.Output};
            int isSuccess = -1;
            if (ctx != null)
            {
                ctx.ExecuteCommand(
                    "Exec P_SacnPackageExchangeBag @trackNumber,@bagNumber,@status,@recordBy,@result output",
                    trackNumberParam, bagNumberParam, statusParam, recordByParam, result);
                Int32.TryParse(result.Value.ToString(), out isSuccess);
            }
            return isSuccess;
        }
    }
}
