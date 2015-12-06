using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
	public class WaybillInfoUpdateExt: LighTake.Infrastructure.Seedwork.Entity
	{
		public string WayBillNumber { get; set; }
		public string RawWayBillNumber { get; set; }//原运单号
		public string CustomerOrderNumber { get; set; }
		public string TrackingNumber { get; set; }
		public string CustomerCode { get; set; }
		public string CustomerName { get; set; }
		public string InShippingMethodName { get; set; }
		public int? InShippingMethodID { get; set; }
		public string CountryCode { get; set; }
		public int Status { get; set; }
		public DateTime CreatedOn { get; set; }
		public bool IsHold { get; set; }
	}
    /// <summary>
    /// 运单汇总报表类
    /// </summary>
    public class WaybillSummary
    {
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public int? InShippingMethodID { get; set; }
        public string InShippingMethodName { get; set; }
        public int? OutShippingMethodID { get; set; }
        public string OutShippingMethodName { get; set; }
        public string VenderName { get; set; }
        public string VenderCode { get; set; }
        public int TotalCount { get; set; }
        public decimal SumWeight { get; set; }
        public int InCount { get; set; }
        public int OutCount { get; set; }
        public int ReturnCount { get; set; }
        public int IsHoldCount { get; set; }
    }
}
