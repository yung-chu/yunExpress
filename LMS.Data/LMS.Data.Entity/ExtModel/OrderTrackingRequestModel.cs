using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class OrderTrackingRequestModel
    {
        public int ShipmentID { get; set; }
        public string CustomerCode { get; set; }
        public string TrackingNumber { get; set; }
    }
}
