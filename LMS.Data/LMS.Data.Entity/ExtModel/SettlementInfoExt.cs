using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
	public class SettlementInfoExt
	{
		public  string SettlementNumber { get; set; }
		public  string CustomerCode { get; set; }
		public string CustomerName { get; set; }
		public  int TotalNumber { get; set; }
		public  decimal TotalWeight { get; set; }
		public  decimal TotalSettleWeight { get; set; }
		public  decimal TotalFee { get; set; }
		public  int Status { get; set; }
		public  string StatusDesc { get; set; }
		public  string SalesMan { get; set; }
		public  string SalesManTel { get; set; }
		public  string SettlementBy { get; set; }
		public  DateTime? SettlementOn { get; set; }
		public  string CreatedBy { get; set; }
		public  System.DateTime CreatedOn { get; set; }
		public  string LastUpdatedBy { get; set; }
		public  System.DateTime LastUpdatedOn { get; set; }
	}
}
