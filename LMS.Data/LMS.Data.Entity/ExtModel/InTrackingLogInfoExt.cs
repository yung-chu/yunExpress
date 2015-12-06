using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class InTrackingLogInfoExt
    {
        public  long ID { get; set; }
        public  string WayBillNumber { get; set; }
        public  System.DateTime? ProcessDate { get; set; }
        public  string ProcessContent { get; set; }
        public  string ProcessLocation { get; set; }
        public  System.DateTime? CreatedOn { get; set; }
        public  string CreatedBy { get; set; }
        public  System.DateTime? LastUpdatedOn { get; set; }
        public  string LastUpdatedBy { get; set; }
        public  string Remarks { get; set; }

        public string TrackingNumber { get; set; }
    }
}
