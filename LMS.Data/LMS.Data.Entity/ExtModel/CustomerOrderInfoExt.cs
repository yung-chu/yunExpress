using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class CustomerOrderInfoExt : CustomerOrderInfo
    {
        public string AbnormalDescription { get; set; }

        public string WayBillNumber { get; set; }

        public string RawTrackingNumber { get; set; }

        public string CountryCode { get; set; }

        public DateTime? TransferOrderDate { get; set; }

        public decimal? SettleWeight { get; set; }


    }
}
