using System.Linq.Dynamic;
using System.Linq.Expressions;
using LMS.Core;
using LMS.Data.Context;
using LMS.Data.Entity.ExtModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Data.Repository
{
    public partial class MailPostBagInfoRepository
    {

        /// <summary>
        /// 国际小包优+ 查询
        /// Add By zhengsong
        /// Time:2014-12-30
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IPagedList<FubListModelExt> GetFubPagedList(FubListParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            Expression<Func<MailPostBagInfo, bool>> filter = p => true;
            //Expression<Func<MailTotalPackageOrPostBagRelational, bool>> filterRelational = p => true;
            Expression<Func<MailTotalPackageInfo, bool>> filterPackage = p => true;
            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                var numberList =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();
                if (param.SearchWhere.HasValue && numberList.Count > 0)
                {
                    switch (param.SearchWhere.Value)
                    {
                        case 1:
                            filter = filter.And(p => numberList.Contains(p.PostBagNumber));
                            break;
                        case 2:
                            filter = filter.And(p => numberList.Contains(p.FuPostBagNumber));
                            break;
                        case 3:
                            List<int> shortNumbers = new List<int>();
                            numberList.ForEach(P =>
                                {
                                    int a = 0;
                                    if (Int32.TryParse(P, out a))
                                    {
                                        shortNumbers.Add(a);
                                    }
                                }
                            );
                            filterPackage = filterPackage.And(p => shortNumbers.Contains(p.ShortNumber));
                            break;
                    }
                }
            }
            switch (param.DateWhere)
            {
                case 0:
                    break;
                case 1:
                    filter = filter.And(r => r.ScanTime >= param.StartTime)
                                     .And(r => r.ScanTime <= param.EndTime)
                                     .AndIf(r => r.ScanBy.Contains(param.CreatedBy), !param.CreatedBy.IsNullOrWhiteSpace());
                    break;
                case 2:
                    filterPackage = filterPackage.And(r => r.ScanTime >= param.StartTime)
                                                       .And(r => r.ScanTime <= param.EndTime)
                                                       .AndIf(r => r.CreatedBy.Contains(param.CreatedBy),
                                                              !param.CreatedBy.IsNullOrWhiteSpace());
                    break;
            }
            if ((param.SearchWhere.HasValue && param.SearchWhere == 3 && !string.IsNullOrWhiteSpace(param.SearchContext)) || param.DateWhere == 2)
            {

                var list = from m in ctx.MailPostBagInfos.Where(filter)
                           join mm in ctx.MailTotalPackageOrPostBagRelationals on m.PostBagNumber equals mm.PostBagNumber
                           into mail
                           from ma in mail.DefaultIfEmpty()
                           join z in ctx.MailTotalPackageInfos.Where(filterPackage) on ma.MailTotalPackageNumber equals z.MailTotalPackageNumber
                           join o in ctx.OutStorageInfos on m.OutStorageID equals o.OutStorageID
                           into mailto
                           from mo in mailto.DefaultIfEmpty()
                           orderby m.CreatedOn
                           select new FubListModelExt
                           {
                               PostBagNumber = m.PostBagNumber,
                               FuPostBagNumber = m.FuPostBagNumber,
                               MailTotalPackageNumber = ma.MailTotalPackageNumber,
                               ShortNumber = z.ShortNumber,
                               ScanTime = m.ScanTime,
                               ScanBy = m.ScanBy,
                               CenterScanTime = ma.ScanTime,
                               CenterCreatedBy = ma.CreatedBy,
                               TotalWeight = m.TotalWeight,
                               TotalNumber = mo.TotalQty,
                               CountryCode = m.CountryCode
                           };

                return list.ToPagedList(param.Page, param.PageSize);
            }
            else
            {
                var list = from m in ctx.MailPostBagInfos.Where(filter)
                           join mm in ctx.MailTotalPackageOrPostBagRelationals on m.PostBagNumber equals mm.PostBagNumber
                           into mail
                           from ma in mail.DefaultIfEmpty()
                           join z in ctx.MailTotalPackageInfos.Where(filterPackage) on ma.MailTotalPackageNumber equals z.MailTotalPackageNumber
                           into mailt
                           from mt in mailt.DefaultIfEmpty()
                           join o in ctx.OutStorageInfos on m.OutStorageID equals o.OutStorageID
                           into mailto
                           from mo in mailto.DefaultIfEmpty()
                           orderby m.CreatedOn
                           select new FubListModelExt
                           {
                               PostBagNumber = m.PostBagNumber,
                               FuPostBagNumber = m.FuPostBagNumber,
                               MailTotalPackageNumber = ma.MailTotalPackageNumber,
                               ShortNumber = mt.ShortNumber,
                               ScanTime = m.ScanTime,
                               ScanBy = m.ScanBy,
                               CenterScanTime = ma.ScanTime,
                               CenterCreatedBy = ma.CreatedBy,
                               TotalWeight = m.TotalWeight,
                               TotalNumber = mo.TotalQty,
                               CountryCode = m.CountryCode
                           };

                return list.ToPagedList(param.Page, param.PageSize);
            }
        }

        /// <summary>
        /// 中心局邮袋扫描查询
        /// Add By zhengsong
        /// Time:2014-12-30
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IPagedList<FubListModelExt> GetFubCenterPagedList(FubListParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            Expression<Func<MailTotalPackageOrPostBagRelational, bool>> filterRelational = p => true;
            switch (param.DateWhere)
            {
                case 0:
                    break;
                case 2:
                    filterRelational = filterRelational.And(r => r.ScanTime >= param.StartTime)
                                                       .And(r => r.ScanTime <= param.EndTime)
                                                       .AndIf(r => r.CreatedBy.Contains(param.CreatedBy),
                                                              !param.CreatedBy.IsNullOrWhiteSpace());
                    break;
            }
            var list = from mm in ctx.MailTotalPackageOrPostBagRelationals.Where(filterRelational)
                       join m in ctx.MailPostBagInfos on mm.PostBagNumber equals m.PostBagNumber
                       into mail
                       from ma in mail.DefaultIfEmpty()
                       join o in ctx.OutStorageInfos on ma.OutStorageID equals o.OutStorageID
                       into mailo
                       from mo in mailo.DefaultIfEmpty()
                       orderby mm.CreatedOn
                       select new FubListModelExt
                       {
                           PostBagNumber = ma.PostBagNumber,
                           FuPostBagNumber = ma.FuPostBagNumber,
                           MailTotalPackageNumber = mm.MailTotalPackageNumber,
                           ShortNumber = mm.MailTotalPackageInfo.ShortNumber,
                           ScanTime = ma.ScanTime,
                           ScanBy = ma.ScanBy,
                           CenterScanTime = mm.ScanTime,
                           CenterCreatedBy = mm.CreatedBy,
                           TotalWeight = ma.TotalWeight,
                           TotalNumber = mo.TotalQty,
                           CountryCode = ma.CountryCode
                       };

            return list.ToPagedList(param.Page, param.PageSize);
        }

        /// <summary>
        /// 获取打印袋牌数据
        /// </summary>
        /// <param name="outStorageId">出仓ID</param>
        /// <returns></returns>
        public BagTagPrintExt GetBagTagPrint(string outStorageId)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;

            var b = ctx.MailPostBagInfos.AsNoTracking().FirstOrDefault(t => t.OutStorageID == outStorageId);

            if (b == null) return null;

            BagTagPrintExt re = new BagTagPrintExt();

            re.BagTagNumber = b.PostBagNumber;
            re.CountryName = b.CountryCode;
            re.HasBattery = b.IsBattery;
            re.Qty = 0;
            re.TotalWeight = b.TotalWeight;

            var qty = ctx.OutStorageInfos.AsNoTracking().Where(t => t.OutStorageID == outStorageId).Select(t => t.TotalQty).FirstOrDefault();

            if (qty != null && qty.HasValue)
            {
                re.Qty = qty.Value;
            }

            return re;
        }


        public ResultInfo IsValidFuPostBagNumber(string fuPostBagNumber)
        {
            ResultInfo re = new ResultInfo();
            var ctx = this.UnitOfWork as LMS_DbContext;

            try
            {
                var postBagNumber = ctx.MailPostBagInfos.AsNoTracking()
                        .Where(t => (t.FuPostBagNumber == fuPostBagNumber || t.PostBagNumber == fuPostBagNumber) && t.FuPostBagNumber != null )
                        .Select(t => t.PostBagNumber).FirstOrDefault();

                if (postBagNumber == null) //不存在
                {
                    re.Status = false;
                    re.Message = "[福邮袋牌号]或者[客户袋牌号]不存在";
                    return re;
                }

                //var yunExpess = postBagNumber; // GetYunExpressBagNumber(fuPostBagNumber);

                //已经扫描
                if (ctx.MailTotalPackageOrPostBagRelationals.AsNoTracking().Any(t => t.PostBagNumber == postBagNumber))
                {
                    re.Status = false;
                    re.Message = "[福邮袋牌号]或者[客户袋牌号]已经扫描";
                    return re;
                }
                re.Status = true;
                return re;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                re.Status = false;
                re.Message = "[福邮袋牌号]或者[客户袋牌号]不存在";
                return re;
            }
        }


        public List<MailPostBagInfo> GetUnTrackingCreated(int count)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;

            var query = from w in ctx.MailPostBagInfos
                        join t in ctx.MailTotalPackageOrPostBagRelationals on w.PostBagNumber equals t.PostBagNumber
                        where w.TrackStatus == 1
                        select w;

            return query.Take(count).ToList();
        }

        public List<WayBillInfo> GetWayBillByMailTotalPackageNumber(string mailTotalPackageNumber)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;

            var query = from w in ctx.WayBillInfos
                        join t in ctx.MailPostBagInfos on w.OutStorageID equals t.OutStorageID
                        join m in ctx.MailTotalPackageOrPostBagRelationals on t.PostBagNumber equals m.PostBagNumber
                        where
                            m.MailTotalPackageNumber == mailTotalPackageNumber
                        select w;

            return query.ToList();
        }

        public string GetCountryCodeByMailTotalPackageNumber(string mailTotalPackageNumber)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;

            var query = from c in ctx.MailPostBagInfos
                        join m in ctx.MailTotalPackageOrPostBagRelationals on c.PostBagNumber equals m.PostBagNumber
                        where m.MailTotalPackageNumber == mailTotalPackageNumber
                        select c.CountryCode;

            return query.First();
        }

        /// <summary>
        /// 获取云途袋牌号
        /// </summary>
        /// <param name="fuPostBagNumber">福邮袋牌号</param>
        /// <returns></returns>
        public MailPostBagInfo GetYunExpressBagNumber(string fuPostBagNumber)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;

            if (ctx.MailPostBagInfos.AsNoTracking()
                .Any(t => (t.FuPostBagNumber == fuPostBagNumber || t.PostBagNumber == fuPostBagNumber) && t.FuPostBagNumber != null))
            {
                return ctx.MailPostBagInfos.AsNoTracking().FirstOrDefault(t => (t.FuPostBagNumber == fuPostBagNumber || t.PostBagNumber == fuPostBagNumber) && t.FuPostBagNumber != null);
            }
            return null;
        }

        public string GetPostBagNumber(string trackNumber)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            var query = from w in ctx.WayBillInfos
                        join m in ctx.MailPostBagInfos on w.OutStorageID equals m.OutStorageID
                        where w.TrackingNumber == trackNumber
                        select m.PostBagNumber;
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 获取总包号发货国家
        /// </summary>
        /// <param name="mailTotalPackageNumbers"></param>
        /// <returns></returns>
        public Dictionary<string, MailTotalPackageCountryExt> GetMailTotalPackageInfoCountry(List<string> mailTotalPackageNumbers)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            var result = new Dictionary<string, MailTotalPackageCountryExt>();
            if (mailTotalPackageNumbers.Any())
            {
                var sbWhere =
                    @"WITH a AS
                    (
	                    SELECT MailTotalPackageNumber,PostBagNumber,ROW_NUMBER() OVER(PARTITION BY MailTotalPackageNumber ORDER BY ID) row_number
	                    FROM dbo.MailTotalPackageOrPostBagRelationals
                        Where MailTotalPackageNumber in ('{0}')
                    ),
                    b AS
                    (
	                    SELECT a.MailTotalPackageNumber,a.PostBagNumber
	                    FROM a 
	                    WHERE a.row_number=1
                    )
                    SELECT b.MailTotalPackageNumber,m.CountryCode
                    FROM b LEFT JOIN dbo.MailPostBagInfos m ON b.PostBagNumber=m.PostBagNumber".FormatWith(string.Join("','", mailTotalPackageNumbers));
                result = ctx.Database.SqlQuery<MailTotalPackageCountryExt>(sbWhere).ToDictionary(p => p.MailTotalPackageNumber);

            }
            return result;
        }


        public IPagedList<MailHoldLogsExt> GetMailHoldLogsList(MailHoldLogsParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            Expression<Func<MailHoldLog, bool>> filter = p => true;

            IQueryable<MailHoldLogsExt> query = null;

            if (param.TrackNumbers!=null&&param.TrackNumbers.Any())
            {
                 query = from w in ctx.WayBillInfos
                            join c in ctx.Countries on w.CountryCode equals c.CountryCode
                            join t in ctx.MailPostBagInfos on w.OutStorageID equals t.OutStorageID
                            join m in ctx.MailHoldLogs on w.TrackingNumber equals m.TrackingNumber
                                into wt
                            from e in wt.DefaultIfEmpty()
                            where
                                param.TrackNumbers.Contains(w.TrackingNumber)
                            select new MailHoldLogsExt
                                {
                                    TrackNumber = w.TrackingNumber,
                                    PostBagNumber = t.PostBagNumber,
                                    HoldOn = e.HoldOn,
                                    HoldBy = e.HoldBy,
                                    Weight = w.Weight,
                                    CountryName = c.ChineseName,
                                };
            }
            else
            {
                filter = filter.AndIf(r => r.HoldOn >= param.StartTime, param.StartTime.HasValue)
                               .AndIf(r => r.HoldOn < param.EndTime, param.EndTime.HasValue);

                query = from w in ctx.WayBillInfos
                        join c in ctx.Countries on w.CountryCode equals c.CountryCode
                        join t in ctx.MailPostBagInfos on w.OutStorageID equals t.OutStorageID
                        join m in ctx.MailHoldLogs.Where(filter) on w.TrackingNumber equals m.TrackingNumber
                        select new MailHoldLogsExt
                        {
                            TrackNumber = w.TrackingNumber,
                            PostBagNumber = t.PostBagNumber,
                            HoldOn = m.HoldOn,
                            HoldBy = m.HoldBy,
                            Weight = w.Weight,
                            CountryName = c.ChineseName,
                        };
            }

            return query.OrderByDescending(p=>p.HoldOn).ToPagedList(param.Page, param.PageSize);
        }
    }
}
