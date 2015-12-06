using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class WayBillListExportParam
    {
        public string CustomerCode { get; set; }
        public int? ShippingMethodId { get; set; }
        public string CountryCode { get; set; }
        public int? DateWhere { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
        public string Status { get; set; }
        public bool? IsOutHold { get; set; }
    }
}
