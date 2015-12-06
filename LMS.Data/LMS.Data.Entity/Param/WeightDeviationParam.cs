using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{
	public class WeightDeviationParam : SearchParam
	{
		public virtual string CustomerCode { get; set; }
		public virtual int? ShippingMethodID { get; set; }
		public virtual int Status { get; set; }
	}
}
