using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Client.B2CTracking.Model
{
    public class B2CForeCastLogs
    {
        public string WayBillNumber { get; set; }
        public int EventCode { get; set; }
        public DateTime EventDate { get; set; }
        public string EventContent { get; set; }
        public string EventLocation { get; set; }
    }
}
