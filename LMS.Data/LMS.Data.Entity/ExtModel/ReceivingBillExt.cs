using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class ReceivingBillExt
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTim { get; set; }
        public int? Status { get; set; }
        public int? ShippingMethodId { get; set; }
        public string CountryCode { get; set; }
        public int SearchWhere { get; set; }
        public string SearchContext { get; set; }
    }
}
