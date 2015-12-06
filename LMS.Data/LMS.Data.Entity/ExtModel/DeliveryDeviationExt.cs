using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
	public class DeliveryDeviationExt
	{
		public  int DeviationID { get; set; }
		public  string VenderName { get; set; }
		public  string VenderCode { get; set; }
		public  int? VenderId { get; set; }
		public  int? ShippingmethodID { get; set; }
		public  string ShippingmethodName { get; set; }
		//public  int? DeviationType { get; set; }
		//public decimal? DeviationValue { get; set; }
		//public decimal? DeviationRate { get; set; }
		public  System.DateTime CreatedOn { get; set; }
		public  string CreatedBy { get; set; }
		public  System.DateTime LastUpdatedOn { get; set; }
		public  string LastUpdatedBy { get; set; }

		public decimal? WaillDeviationValue { get; set; }
		public decimal? WaillDeviationRate { get; set; }
		public decimal? WeightDeviationValue { get; set; }
		public decimal? WeightDeviationRate { get; set; }	
	}

}
