using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using LMS.Data.Context;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Repository
{
    public partial interface IInTrackingLogInfoRepository
    {
        List<InTrackingLogInfoExt> GetInTrackingLogInfo(List<string> trackingNumberList);

    }

    public partial class InTrackingLogInfoRepository
    {

        public List<InTrackingLogInfoExt> GetInTrackingLogInfo(List<string> trackingNumberList)
        {
            var ctx =new  LMS_DbContext() ;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            //query only
            ctx.Configuration.AutoDetectChangesEnabled = false;
            //ctx.Configuration.LazyLoadingEnabled = false;
            //ctx.Configuration.ValidateOnSaveEnabled = false;


          var result =
                 from b in ctx.WayBillInfos.AsNoTracking().Where(a => trackingNumberList.Contains(a.TrackingNumber))
                 join a in ctx.InTrackingLogInfos.AsNoTracking() on b.WayBillNumber equals a.WayBillNumber

                select new InTrackingLogInfoExt
                {
                    WayBillNumber = a.WayBillNumber,
                    ProcessDate = a.ProcessDate,
                    ProcessContent = a.ProcessContent,
                    ProcessLocation = a.ProcessLocation,
                    CreatedOn = a.CreatedOn,
                    CreatedBy = a.CreatedBy,
                    LastUpdatedOn = a.LastUpdatedOn,
                    LastUpdatedBy = a.LastUpdatedBy,
                    Remarks = a.Remarks,
                    TrackingNumber = b.TrackingNumber

                };

            return result.ToList();
        }

       


    }
}
