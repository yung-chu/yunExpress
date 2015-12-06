using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Context;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial class WaybillPackageDetailRepository 
    {
        public string GetId()
        {
            return string.Empty;
        }

        public decimal GetWayBillListWeight(List<string> wayBillNumbers)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            return
                ctx.WaybillPackageDetails.Where(p => wayBillNumbers.Contains(p.WayBillNumber))
                   .Sum(p=>p.Weight+p.AddWeight)??0;
        }
    }
}
