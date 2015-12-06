using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public	class TariffPrepayFeeShippingMethod
	{
		public string ShippingMethodName { get; set; }
		public int ShippingMethodId { get; set; }
		public string Code { get; set; }
		public bool EnableTariffPrepay { get; set; }
		public string CustomerCode { get; set; }
		public decimal TariffPrepayFee { get; set; }

	}
}
