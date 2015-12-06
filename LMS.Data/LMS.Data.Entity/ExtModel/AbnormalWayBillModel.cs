using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity.ExtModel
{
    public class AbnormalWayBillModel
    {
        public string WayBillNumber { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string CustomerCode { get; set; }
        public string InShippingMethodName { get; set; }
        public int AbnormalStatus { get; set; }
        public string CountryCode { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime? AbnormalCreateOn { get; set; }
        public string AbnormalCreateBy { get; set; }
        public string AbnormalTypeName { get; set; }
        public int Status { get; set; }
        public string AbnormalDescription { get; set; }
        public int OperateType { get; set; }
        public bool IsHold { get; set; }
    }
}
