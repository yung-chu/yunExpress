using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity.Param
{
	public class DeliveryDeviationParam : SearchParam
	{
		public  string VenderName { get; set; }
		public  string VenderCode { get; set; }
		public  int? ShippingmethodID { get; set; }
		public  string ShippingmethodName { get; set; }

	}
}
