using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{ 
	public class ChargePayAnalysesExt : LighTake.Infrastructure.Seedwork.Entity
	{
	    public string Name { get; set; }
	    public string WayBillNumber { get; set; }
	    public string VenderName { get; set; }
		public string ShippingmethodName { get; set; }
	    public decimal? ReceivingAmount { get; set; }
		public decimal? DeliveryAmount { get; set; }
		public decimal? Rate { get; set; }
	}
}
