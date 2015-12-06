using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class ErrorWayBillExt
    {
        public string WayBillNumber { get; set; }
        public string ErrorMassge { get; set; }
        public bool result { get; set; }
        public int? outShippingMethodID { get; set; }
    }
}
