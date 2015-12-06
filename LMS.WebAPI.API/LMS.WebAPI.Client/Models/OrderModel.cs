using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.WebAPI.Client.Models
{
	public class OrderModel
	{

		public OrderModel()
		{
			OrderTrackingDetails = new List<OrderTrackingDetailModels>();
		}

		public string WayBillNumber { get; set; }
		public int? PackageState { get; set; }
		public string CountryCode { get; set; }
		public string TrackingNumber { get; set; }
		public int? IntervalDays { get; set; }
		public string CreatedBy { get; set; }

		public List<OrderTrackingDetailModels> OrderTrackingDetails { get; set; }


	}


	public class OrderTrackingDetailModels
	{
		public DateTime? ProcessDate { get; set; }
		public string ProcessContent { get; set; }
		public string ProcessLocation { get; set; }
	}
}