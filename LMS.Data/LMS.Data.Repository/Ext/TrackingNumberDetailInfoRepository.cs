using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using LMS.Data.Context;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;
using System.Transactions;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace LMS.Data.Repository
{
    public partial class TrackingNumberDetailInfoRepository
    {

        //public TrackingNumberDetailInfo GetTrackingNumberDetailInfo(int shippingMethodId, string countryCode)
        //{
        //    var ctx = this.UnitOfWork as LMS_DbContext;
        //    Check.Argument.IsNotNull(ctx, "数据库对象");
        //    var list = (from tnInfo in ctx.TrackingNumberInfos
        //                join tndInfo in ctx.TrackingNumberDetailInfos on tnInfo.TrackingNumberID equals tndInfo.TrackingNumberID
        //                where tnInfo.ShippingMethodID == shippingMethodId && tnInfo.ApplianceCountry.Contains(countryCode) && tnInfo.Status == (short)TrackingNumberInfo.StatusEnum.Enable
        //                    && tndInfo.Status == (short)TrackingNumberDetailInfo.StatusEnum.NotUsed
        //                orderby tndInfo.TrackingNumber
        //                select tndInfo);
        //    return list.FirstOrDefault();
        //}

        /// <summary>
        /// 无人使用
        /// </summary>
        /// <returns></returns>
        //[Obsolete("弃用")]
        //public IList<TrackingNumberDetailInfo> GetTrackingNumberDetailList()
        //{
        //    var ctx = this.UnitOfWork as LMS_DbContext;
        //    Check.Argument.IsNotNull(ctx, "数据库对象");
        //    var list = (from tnInfo in ctx.TrackingNumberInfos
        //                join tndInfo in ctx.TrackingNumberDetailInfos on tnInfo.TrackingNumberID equals tndInfo.TrackingNumberID
        //                where tnInfo.Status == (short)TrackingNumberInfo.StatusEnum.Enable && tndInfo.Status == (short)TrackingNumberDetailInfo.StatusEnum.NotUsed
        //                select tndInfo);
        //    return list.ToList();
        //}

        /// <summary>
        /// 根据运输方式Id和国家代码获取未使用的跟踪号
        /// </summary>
        /// <param name="shippingMethodId">运输方式Id</param>
        /// <param name="countryCode">国家代码</param>
        /// <param name="detailIds">要排除的TrackingNumberDetailID</param>
        /// <returns></returns>
        //public TrackingNumberDetailInfo GetTrackingNumberDetailInfo(int shippingMethodId, string countryCode, List<int> detailIds)
        //{
        //    var ctx = this.UnitOfWork as LMS_DbContext;
        //    Check.Argument.IsNotNull(ctx, "数据库对象");

        //    Expression<Func<TrackingNumberDetailInfo, bool>> filterWayBill = o => true;
        //    filterWayBill = filterWayBill.AndIf(o => !detailIds.Contains(o.TrackingNumberDetailID), detailIds.Count > 0);

        //    var query = from info in ctx.TrackingNumberInfos.Where(p => p.ApplianceCountry.Contains(countryCode))
        //                join detail in ctx.TrackingNumberDetailInfos.Where(filterWayBill) on info.TrackingNumberID equals detail.TrackingNumberID
        //                where info.ShippingMethodID == shippingMethodId && info.Status == (short)TrackingNumberInfo.StatusEnum.Enable
        //                    && detail.Status == (short)TrackingNumberDetailInfo.StatusEnum.NotUsed
        //                orderby detail.TrackingNumber ascending
        //                select detail;

        //    return query.FirstOrDefault();
        //}

        /// <summary>
        /// 按运输方式获取跟踪号 (上传跟踪号时使用)
        /// </summary>
        /// <param name="shippingMethodId"></param>
        /// <returns></returns>
        //public IList<string> GetListByShippingMethodId(int shippingMethodId)
        //{
        //    var ctx = this.UnitOfWork as LMS_DbContext;
        //    Check.Argument.IsNotNull(ctx, "数据库对象");

        //    var list = (from tnInfo in ctx.TrackingNumberInfos.AsNoTracking()
        //                    join tndInfo in ctx.TrackingNumberDetailInfos.AsNoTracking()
        //                on tnInfo.TrackingNumberID equals tndInfo.TrackingNumberID
        //                    where tnInfo.ShippingMethodID == shippingMethodId
        //                    select tndInfo.TrackingNumber);

        //    return list.ToList();
        //}


        #region 分配跟踪号标准接口

        static object _locker = new object();

        /// <summary>
        /// 分配跟踪号[标准底层接口]【只供LMS.WCF.TrackNumber项目调用】
        /// 其它地方需要分配跟踪号,请使用LMS.Services.TrackingNumberServices.TrackNumberAssignStandard方法
        /// 2014-10-29 by daniel
        /// </summary>
        /// <param name="shippingMethodId">运输方式</param>
        /// <param name="count">数量(0表示取全部)</param>
        /// <param name="countryCode">国家代码(可空)</param>
        /// <returns></returns>
        public IList<string> TrackNumberAssignStandard(int shippingMethodId, int count, string countryCode = "")
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            if (shippingMethodId <= 0)
            {
                throw new BusinessLogicException("运输方式ID不能为空.");
            }
            if (count <= 0 || count > 100)
            {
                throw new BusinessLogicException("每次只能获取1~100个跟踪号.");
            }

            lock (_locker)
            {
                ConcurrentBag<string> result = new ConcurrentBag<string>();

                do
                {
                    var list = GetTrackNumber(ctx, shippingMethodId, count - result.Count, countryCode);

                    if (list == null || list.Count == 0)
                    {
                        throw new BusinessLogicException(string.Format("申请跟踪号失败,运输方式[{0}],国家[{1}],不够申请个数或者没有跟踪号了.", shippingMethodId, countryCode));
                    }

                    //找到运单表中已经存在的跟踪号
                    var usedTrackNumbers = GetWayBillInfoExistTrackNumber(ctx, list);
                    if (usedTrackNumbers.Count > 0)
                    {
                        //修改为已使用
                        UpdateTrackNumberToUsed(ctx, usedTrackNumbers);
                    }

                    //将未使用的跟踪号放入结果集
                    Parallel.ForEach(list, s =>
                    //foreach (var s in list)
                    {
                        if (!usedTrackNumbers.Contains(s))
                        {
                            result.Add(s);
                        }
                    });
                } while (result.Count < count);

                //将所有结果集状态改为已使用
                UpdateTrackNumberToUsed(ctx, result.ToList());

                return result.ToList();
            }
        }

        /// <summary>
        /// (内部方法)获取可用的跟踪号
        /// 2014-10-29 by daniel
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="shippingMethodId"></param>
        /// <param name="count"></param>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        private List<string> GetTrackNumber(LMS_DbContext ctx, int shippingMethodId, int count, string countryCode = "")
        {
            ushort notUsed = (ushort)TrackingNumberDetailInfo.StatusEnum.NotUsed;
            ushort enable = (ushort)TrackingNumberInfo.StatusEnum.Enable;

            Expression<Func<TrackingNumberInfo, bool>> filter = o => true;
            filter = filter
                .And(p => p.Status == enable)
                .AndIf(t => t.ApplianceCountry.Contains(countryCode), !string.IsNullOrWhiteSpace(countryCode));

            var query = (from info in ctx.TrackingNumberInfos.AsNoTracking().Where(filter)
                         join detail in ctx.TrackingNumberDetailInfos.AsNoTracking().Where(p => p.Status == notUsed)
                         on info.TrackingNumberID equals detail.TrackingNumberID
                         where info.ShippingMethodID == shippingMethodId
                         select detail.TrackingNumber);

            return count != 0 ? query.Take(count).ToList() : query.ToList();
        }

        /// <summary>
        /// 查找订单表存在的跟踪号
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="trackNumbers"></param>
        /// <returns></returns>
        private List<string> GetWayBillInfoExistTrackNumber(LMS_DbContext ctx, List<string> trackNumbers)
        {           
            var query = from w in ctx.CustomerOrderInfos.AsNoTracking()
                        where trackNumbers.Contains(w.TrackingNumber)
                        select w.TrackingNumber;

            return query.ToList();
        }
               
        /// <summary>
        /// 修改为已使用
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="trackNumbers"></param>
        private void UpdateTrackNumberToUsed(LMS_DbContext ctx, List<string> trackNumbers)
        {
            short notUsed = (short)TrackingNumberDetailInfo.StatusEnum.NotUsed;
            short used = (short)TrackingNumberDetailInfo.StatusEnum.Used;

            using (var tran = new TransactionScope())
            {
                Modify(
                    p => new TrackingNumberDetailInfo()
                    {
                        Status = used
                    },
                    t => trackNumbers.Contains(t.TrackingNumber) && t.Status == notUsed);
                ctx.Commit();
                tran.Complete();
            }
        }

        #endregion

        /// <summary>
        /// 判断跟踪号是否重复
        /// </summary>
        /// <param name="trackNumbers"></param>
        /// <returns></returns>
        public List<string> CheckRepeatedTrackNumbers(IEnumerable<string> trackNumbers)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            
            var q = from d in ctx.TrackingNumberDetailInfos.AsNoTracking()
                    where trackNumbers.Contains(d.TrackingNumber)
                    select d.TrackingNumber;
                              
            return q.Distinct().ToList();
        }

        /// <summary>
        /// 跟踪号管理页面查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public SelectTrackingNumberExt GetTrackingNumberDetails(TrackingNumberParam param)
        {
            SelectTrackingNumberExt model = new SelectTrackingNumberExt();
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var list = (from tnInfo in ctx.TrackingNumberInfos
                                          .WhereIf(p => p.ShippingMethodID == param.shippingMehtodId,
                                                   param.shippingMehtodId != null)
                                          .WhereIf(p => p.CreatedNo >= param.StartTime.Value, param.StartTime.HasValue)
                                          .WhereIf(p => p.CreatedNo <= param.EndTime.Value, param.EndTime.HasValue)
                        select new TrackingNumberID()
                            {
                                TrackingNumberId = tnInfo.TrackingNumberID
                            });
            model.TrackingNumberIds = list.ToList();
            return model;
        }

    }
}
