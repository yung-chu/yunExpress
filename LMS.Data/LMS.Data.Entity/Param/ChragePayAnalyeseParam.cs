using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{
	public class ChragePayAnalyeseParam : SearchParam
	{
	    public string CustomerCode { get; set; }
	    public string VenderCode { get; set; }
	    public int? ShippingMethodId { get; set; }
	    public DateTime? StartTime { get; set; }
	    public DateTime? EndTime { get; set; }
	}
}
