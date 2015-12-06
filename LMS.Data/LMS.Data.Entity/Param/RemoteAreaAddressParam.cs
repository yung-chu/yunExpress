using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{
	public class RemoteAreaAddressParam : SearchParam
	{
		public int? ShippingMethodId { get; set; }
		public string CountryCode { get; set; }
		public string State { get; set; }
		public string StateCode { get; set; }
		public string City { get; set; }
		public string Zip { get; set; }
	}
}
