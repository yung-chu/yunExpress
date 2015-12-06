using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LighTake.Infrastructure.Web.Models;

namespace LMS.Controllers.FinancialController
{
	public class ChragePayAnalyeseFilterModel : SearchFilter
	{
		public string CustomerCode { get; set; }
		public string VenderCode { get; set; }
		public string ShippingMethodId { get; set; }
		public DateTime? StartTime { get; set; }
		public DateTime? EndTime { get; set; }

		public string VenderName { get; set; }
		public string ShippingMethodName { get; set; }

	}
}