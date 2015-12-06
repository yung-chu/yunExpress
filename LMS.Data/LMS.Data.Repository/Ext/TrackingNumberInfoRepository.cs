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
    public partial interface ITrackingNumberInfoRepository
    {
        List<TrackingNumberExt> GetTrackingNumberExtList(TrackingNumberParam param);
    }

    public partial class TrackingNumberInfoRepository : ITrackingNumberInfoRepository
    {


        public List<TrackingNumberExt> GetTrackingNumberExtList(TrackingNumberParam param)
        {
            Expression<Func<TrackingNumberInfo, bool>> filter = o => true;
            filter = filter.AndIf(o => o.ShippingMethodID == param.shippingMehtodId, param.shippingMehtodId.HasValue)
                .AndIf(o=>o.Status== (short)TrackingNumberInfo.StatusEnum.Enable,true)
                .AndIf(o => o.CreatedNo >= param.StartTime, param.StartTime.HasValue)
                .AndIf(o => o.CreatedNo <= param.EndTime, param.EndTime.HasValue);
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var list = (from tnInfo in ctx.TrackingNumberInfos.Where(filter)
                        orderby tnInfo.CreatedNo descending
                        select new TrackingNumberExt
                        {
                            TrackingNumberID = tnInfo.TrackingNumberID,
                            ShippingMethodID = tnInfo.ShippingMethodID,
                            ApplianceCountry = tnInfo.ApplianceCountry,
                            Status = tnInfo.Status,
                            Remarks = tnInfo.Remarks,
                            NotUsed = ctx.TrackingNumberDetailInfos.Count(p => p.TrackingNumberID == tnInfo.TrackingNumberID && p.Status == (short)TrackingNumberDetailInfo.StatusEnum.NotUsed),
                            Used = ctx.TrackingNumberDetailInfos.Count(p => p.TrackingNumberID == tnInfo.TrackingNumberID && p.Status == (short)TrackingNumberDetailInfo.StatusEnum.Used),
                            CreatedNo = tnInfo.CreatedNo
                        });

            return list.ToList();
        }



      





    }
}
