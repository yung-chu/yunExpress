using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
	public class OrderTrackingModel : LighTake.Infrastructure.Seedwork.Entity
    {


        public string TrackingNumber { get; set; }
        public int ShipmentID { get; set; }
        public string CustomerCode { get; set; }
        public int? PackageState { get; set; }
        public int? InfoState { get; set; }
        public int? IntervalDays { get; set; }
        public DateTime? LastEventDate { get; set; }
        public string LastEventContent { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public string Remarks { get; set; }

        public List<OrderTrackingDetailModel> OrderTrackingDetails { get; set; }
    }

	public class OrderTrackingDetailModel
    {
        public string TrackingNumber { get; set; }
        public bool? IsDisplay { get; set; }
        public DateTime? ProcessDate { get; set; }
        public string ProcessContent { get; set; }
        public string ProcessLocation { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
