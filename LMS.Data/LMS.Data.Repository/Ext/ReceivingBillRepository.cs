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
using LMS.Data.Entity.ExtModel;
using LMS.Data.Entity.Param;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial class ReceivingBillRepository
    {

        public IPagedList<ReceivingBill> GetPagedList(ReceivingBillParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            Expression<Func<ReceivingBill, bool>> filter = o => true;

            filter = filter.AndIf(r => r.Status == param.Status, param.Status.HasValue)
                           .AndIf(r => r.ReceivingBillAuditor.Contains(param.ReceivingBillAuditor), !string.IsNullOrWhiteSpace(param.ReceivingBillAuditor))
                           .AndIf(r => r.CustomerCode == param.CustomerCode, !string.IsNullOrWhiteSpace(param.CustomerCode))
                           .AndIf(r => r.ReceivingBillDate >= param.StartTime, param.StartTime.HasValue)
                           .AndIf(r => r.ReceivingBillDate <= param.EndTime, param.EndTime.HasValue)
                           .AndIf(r => r.ReceivingBillID == param.ReceivingBillID, !string.IsNullOrWhiteSpace(param.ReceivingBillID));
            
            Func<IQueryable<ReceivingBill>, IOrderedQueryable<ReceivingBill>> orderBy = x => x.OrderByDescending(x2 => x2.ReceivingBillDate);
            return FindPagedList(param, filter, orderBy);
        }

    }
}
