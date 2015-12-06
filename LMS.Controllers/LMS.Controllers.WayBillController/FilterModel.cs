using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LighTake.Infrastructure.Web.Models;

namespace LMS.Controllers.WayBillController
{
    public class InStorageFilterModel : SearchFilter
    {
        public string CustomerCode { get; set; }
        public string InStorageID { get; set; }
        public DateTime? InStartDate { get; set; }
        public DateTime? InEndDate { get; set; }
        public int? ShippingMethodId { get; set; }
        public string ShippingMethodIdName { get; set; }
    }
    public class OutStorageFilterModel : SearchFilter
    {
        public string VenderCode { get; set; }
        public string OutStorageID { get; set; }
        public DateTime? OutStartDate { get; set; }
        public DateTime? OutEndDate { get; set; }
        public string PostBagNumber { get; set; }
    }
    public class WayBillListFilterModel : SearchFilter
    {
        public string CustomerCode { get; set; }
        public int? ShippingMethodId { get; set; }
        public string ShippingName { get; set; }
        public int DateWhere { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string CountryCode { get; set; }
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
		public int? Status { get; set; }
		public string GetStatus { get; set; } //用户下拉框多选
	    public bool IsHold { get; set; }
        public int OperatorType { get; set; }
        public string Operator { get; set; }
        public bool ShowTestWaybill { get; set; }
    }

    public class AbnormalWayBillListFilterModel : SearchFilter
    {
        public string CustomerCode { get; set; }
        public int? ShippingMethodId { get; set; }
        public string ShippingName { get; set; }
        public int DateWhere { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string CountryCode { get; set; }
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
        public int? Status { get; set; }
		//运单状态
	    public int? WaybillStatus { get; set; }

        public bool? IsFirstIn { get; set; }

    }
    public class InFeeListFilterModel : SearchFilter
    {
        public string CustomerCode { get; set; }
        public int? ShippingMethodId { get; set; }
        public string ShippingName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string CountryCode { get; set; }
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
        public bool IsFistIn { get; set; }
    }
    public class OutFeeListFilterModel : SearchFilter
    {
        public string VenderCode { get; set; }
        public string VenderName { get; set; }
        public int? ShippingMethodId { get; set; }
        public string ShippingName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string CountryCode { get; set; }
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
    }

    public class WayBillTemplateListFilterModel : SearchFilter
    {
        public int ShippingMethodId { get; set; }
    }

    public class ExpressWayBillFilterModel : SearchFilter
    {
        public string CustomerCode { get; set; }
        public int? ShippingMethodId { get; set; }
        public string ShippingName { get; set; }
        public int DateWhere { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
        public int? Status { get; set; }
    }

	public class InStorageWeightDeviationFilterModel : SearchFilter
	{
		public  string CustomerCode { get; set; }
		public  string CustomerName { get; set; }
		public  int? ShippingMethodID { get; set; }
		public  string ShippingMethodName { get; set; }
		public  decimal DeviationValue { get; set; }
		public  int Status { get; set; }

		public int Type { get; set; }
		public int inStorageWeightDeviationId { get; set; }

	}

	public class WaybillInfoUpdateFilterModel : SearchFilter
	{
		public string CustomerCode { get; set; }
		public int? ShippingMethodId { get; set; }
		public string ShippingName { get; set; }
		public int? SearchWhere { get; set; }
		public string SearchContext { get; set; }
		public DateTime? StartTime { get; set; }
		public DateTime? EndTime { get; set; }
		public int? Status { get; set; }
		public bool IsFirstIn { get; set; }//true不是第一次进入

	}

	public class InStorageWeightAbnormalFilterModel : SearchFilter
	{
		public string CustomerCode { get; set; }
		public int? ShippingMethodId { get; set; }
		public string ShippingName { get; set; }
		public int? SearchWhere { get; set; }
		public string SearchContext { get; set; }
		public DateTime? StartTime { get; set; }
		public DateTime? EndTime { get; set; }

        public string IsWeightGtWeight { get; set; }
        public bool IsFirstIn { get; set; }
	}

    public class NoForecastAbnormalFilterModel: SearchFilter
    {
        public NoForecastAbnormalFilterModel()
        {
            IsFirstIn = true;
        }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int? ShippingMethodId { get; set; }
        public string ShippingName { get; set; }
        public string SearchContext { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Status { get; set; }
        public bool IsFirstIn { get; set; }
    }
}