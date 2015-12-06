using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LighTake.Infrastructure.Web.Models;

namespace LMS.Controllers.FinancialController
{

	public class ReceivingExpenseListFilterModel : FinancialSearchFilter
    {
        public string CustomerCode { get; set; }
        public int? ShippingMethodId { get; set; }
        public string ShippingMethodName { get; set; }
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
    }

	public class ChragePayAnalyeseFilterModel : FinancialSearchFilter
	{
		public string CustomerCode { get; set; }
		public string VenderCode { get; set; }
		public int? ShippingMethodId { get; set; }
		public DateTime? StartTime { get; set; }
		public DateTime? EndTime { get; set; }

		public string VenderName { get; set; }
		public string ShippingMethodName { get; set; }
		//是否第一次进入
		public bool IsFirstIn { get; set; }
	}



	public class JobErrorLogFilterModel : SearchFilter
	{
		public  string WayBillNumber { get; set; }
		public  int? JobType { get; set; }//1-收货，3-发货
		public DateTime? StartTime { get; set; }
		public DateTime? EndTime { get; set; }
	}


	public class ReceivingBillFilterModel : FinancialSearchFilter
    {
        public string CustomerCode { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public string ReceivingBillAuditor { get; set; }
        public string ReceivingBillID { get; set; }

        public bool? IsFirstIn { get; set; }
    }

	public class DeliveryDeviationFilterModel : SearchFilter
	{
		public string VenderName { get; set; }
		public string VenderCode { get; set; }
		public int? ShippingmethodID { get; set; }
		public string ShippingmethodName { get; set; }


		public int Type { get; set; }//1新增 2编辑
		public int DeliveryId { get; set; }
		public string DeviationType { get; set; }//偏差类型
		public decimal? DeviationValue { get; set; }//编辑偏差值
		public string WeightString { get; set; }
		public decimal? WeightDeviations { get; set; }
		public string WayBillFeeString { get; set; }
		public decimal? WayBillFeeDeviations { get; set; }
	}
}