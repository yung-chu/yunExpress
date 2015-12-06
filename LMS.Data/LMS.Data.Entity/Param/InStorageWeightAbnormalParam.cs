using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{
	public class InStorageWeightAbnormalParam : SearchParam
	{
		public string CustomerCode { get; set; }
		public int? ShippingMethodId { get; set; }
		public string ShippingName { get; set; }
		public int? SearchWhere { get; set; }
		public string SearchContext { get; set; }
		public DateTime? StartTime { get; set; }
		public DateTime? EndTime { get; set; }

        public string IsWeightGtWeight { get; set; }
	}
}
