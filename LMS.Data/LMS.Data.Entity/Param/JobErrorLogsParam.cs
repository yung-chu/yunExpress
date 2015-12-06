using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{
    public class JobErrorLogsParam: SearchParam
	{
		public string WayBillNumber { get; set; }
		public int? JobType { get; set; }//1-收货，3-发货
		public DateTime? StartTime { get; set; }
		public DateTime? EndTime { get; set; }

	}
}
