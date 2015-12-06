using LighTake.Infrastructure.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity.Param
{
    public class SettlementSummaryParam : SearchParam
    {
        public string CustomerCode { get; set; }
        public int? Status { get; set; }
    }
}
