using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Context;
using LMS.Data.Entity;
using LMS.Data.Repository.Extensions;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Repository
{
   public partial class FuzhouPostLogRepository
    {

        /// <summary>
        /// 更申请记录
        ///  Add By zhengsong
        /// Time"2014-11-04
        /// </summary>
       public void bulkInsertFuzhouPostLog(List<FuzhouPostLog> addfuzhouPostLogs)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Bulk.BulkInsert(ctx, "FuzhouPostLogs", addfuzhouPostLogs);
            ctx.SaveChanges();
        }
    }
}
