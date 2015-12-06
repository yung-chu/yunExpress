using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity.ExtModel
{

    public class WayBillEventLogExt : WayBillEventLog
    {
        public bool IsHold { get; set; }
        public int Status { get; set; }
        public int? ShippingMethodId { get; set; }
        public string CountryCode { get; set; }
        public string OutStorageID { get; set; }
    }
}
