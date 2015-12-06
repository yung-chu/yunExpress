using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{
	public class SettlementInfoParam: SearchParam
	{
		public string CustomerCode { get; set; }
		public string NikcName { get; set; }
		public DateTime? StartTime { get; set; }
		public DateTime? EndTime { get; set; }
		public string CreatedBy { get; set; }
		public int? Status { get; set; }
		public string SettlementBy { get; set; }
		public string SettlementNumber { get; set; }
	}
}
