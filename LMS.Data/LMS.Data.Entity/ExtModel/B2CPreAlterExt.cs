using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class B2CPreAlterExt : LighTake.Infrastructure.Seedwork.Entity
    {
        public string WayBillNumber { get; set; }
        public string CustomerCode { get; set; }
        public string TrackingNumber { get; set; }
        public decimal SettleWeight { get; set; }
        public string CountryCode { get; set; }
        public string VenderName { get; set; }
        public string OutShippingMethodName { get; set; }
        public DateTime? InStorageCreatedOn { get; set; }
        public string ErrorMsg { get; set; }
        public int Status { get; set; }
    }
}
