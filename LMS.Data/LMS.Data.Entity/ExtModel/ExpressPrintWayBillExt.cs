using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class ExpressPrintWayBillExt : LighTake.Infrastructure.Seedwork.Entity
    {
        public string WayBillNumber { get; set; }
        public Nullable<int> CustomerOrderID { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string CustomerCode { get; set; }
        public string TrackingNumber { get; set; }
        public Nullable<decimal> SettleWeight { get; set; }
        public int Status { get; set; }
        public string CountryCode { get; set; }
        public Nullable<int> InShippingMethodID { get; set; }
        public Nullable<int> OutShippingMethodID { get; set; }
        public string InShippingMethodName { get; set; }
        public string OutShippingMethodName { get; set; }
        public string VenderName { get; set; }
        public string VenderCode { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsPrinted { get; set; }
        public bool IsPrinter { get; set; }
    }
}
