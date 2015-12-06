using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.UserCenter.Controllers.OrderController.Models
{
    public class EubWayBillApplicationInfoModel
    {
        public int? EubOrderId { get; set; }
        public string WayBillNumber { get; set; }
        public int? ShippingMethodID { get; set; }
        public string BatchNumber { get; set; }
        public int? PrintFormat { get; set; }
        public string LocalDownLoad { get; set; }
        public string EubDownLoad { get; set; }
        public int? Status { get; set; }
        public DateTime? ApplyDate { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public string TrackingNumber { get; set; }

        public string CountryCode { get; set; }

        public string ShippingMethodName { get; set; }

        public string CustomerOrderNumber { get; set; }
    }

    
}