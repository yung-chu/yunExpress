using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
	 public class InStorageWeightAbnormalExt: LighTake.Infrastructure.Seedwork.Entity
	{
		public string WayBillNumber { get; set; }
		public string CustomerOrderNumber { get; set; }
		public string TrackingNumber { get; set; }
		public string CustomerCode { get; set; }
		public string CustomerName { get; set; }
		public string InShippingMethodName { get; set; }
		public int? InShippingMethodId { get; set; }
		public string CountryCode { get; set; }
		public decimal Weight { get; set; }
		public decimal ForecastWeight { get; set; }
		public int OperateType { get; set; }
		public string AbnormalTypeName { get; set; }
		public string AbnormalDescription { get; set; }
		public DateTime CreatedOn { get; set; }
		public bool IsHold { get; set; }
	}

	public class ExportInStorageWeightAbnormalExt
	{
		public string CustomerName { get; set; }
		public string CustomerOrderNumber { get; set; }
		public string WayBillNumber { get; set; }
		public string InShippingMethodName { get; set; }
		public string TrackingNumber { get; set; }
		public string CountryCode { get; set; }
		public string ChineseName { get; set; }
		public DateTime CreatedOn { get; set; }
		public int PackageNumber { get; set; }
		public decimal	Length{ get; set; }
		public decimal	Width{ get; set; }
		public decimal Height { get; set; }
		public decimal ForecastWeight { get; set; }
		public decimal Weight { get; set; }
		public decimal Deviation { get; set; }

	}
}
