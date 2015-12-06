using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity.ExtModel
{
    public class InStorageInfoExtSlim
    {
        public string InStorageID { get; set; }
        public string ReceivingClerk { get; set; }
        public string CustomerCode { get; set; }
        public Nullable<decimal> TotalWeight { get; set; }
        public Nullable<int> TotalQty { get; set; }
        public int Status { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public string Remark { get; set; }
        public PriceProviderResult PriceResult { get; set; }
        public decimal PhysicalTotalWeight { get; set; }
        public int? PaymentTypeID { get; set; }
        public DateTime ReceivingDate { get; set; }
    }
}
