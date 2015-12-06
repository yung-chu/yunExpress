using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{
    public class ShippingWayBillParam : SearchParam
    {
        public string VenderCode { get; set; }
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
        public string OutCreateBy { get; set; }
        public int? InShippingMehtodId { get; set; }
        public int? OutShippingMehtodId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
