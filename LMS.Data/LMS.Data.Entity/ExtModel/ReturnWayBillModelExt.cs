using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class ReturnWayBillModelExt : LighTake.Infrastructure.Seedwork.Entity
    {
        public string CustomerName { get; set; }

        public string WayBillNumber { get; set; }

        public string CustomerOrderNumber { get; set; }

        public string TrackingNumber { get; set; }

        public decimal? TotalWeight { get; set; }

        public string CountryCode { get; set; }

        public string ChineseName { get; set; }

        public decimal ShippingFee { get; set; }

        public int Type { get; set; }

        public string Reason { get; set; }

        public string ReasonRemark { get; set; }

        public bool IsReturnShipping { get; set; }

        public DateTime? OutCreatedOn { get; set; }

        public DateTime? ReturnCreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedByEeName { get; set; }

        public string InShippingMehtodName { get; set; }

        public string Auditor { get; set; }

        public DateTime? AuditorDate { get; set; }
    }
}
