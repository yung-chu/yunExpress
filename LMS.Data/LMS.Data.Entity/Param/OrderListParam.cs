using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{
    public class OrderListParam : SearchParam
    {
        public string CustomerCode { get; set; }
        public int? ShippingMethodId { get; set; }
        public int DateWhere { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string CountryCode { get; set; }
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
        public int? Status { get; set; }
		public string GetStatus { get; set; }
	    public bool? IsHold { get; set; }
        public int OperatorType { get; set; }
        public string Operator { get; set; }
        public bool ShowTestWaybill { get; set; }
    }
    public class InFeeListParam : SearchParam
    {
        public string CustomerCode { get; set; }
        public int? ShippingMethodId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string CountryCode { get; set; }
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
    }

    public class InFeeTotalListParam : SearchParam
    {
        public string CustomerCode { get; set; }
        public string Number { get; set; }
    }

    public class OutFeeListParam : SearchParam
    {
        public string VenderCode { get; set; }
        public int? ShippingMethodId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string CountryCode { get; set; }
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
    }

    public class OutStorageListParam : SearchParam
    {
        public string VenderCode { get; set; }
        public string OutStorageID { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        
    }

    public class WayBillTemplateListParam : SearchParam
    {
        public int ShippingMethodId { get; set; }
       
    }

    /// <summary>
    /// 快递入仓打印发票
    /// </summary>
    public class PrintInStorageInvoiceParam
    {
        public string InStorageID { get; set; }
    }

    public class WayBillTemplateInfoParam : SearchParam
    {
        public string TemplateName { get; set; }
        public int TemplateType { get; set; }
        public int Status { get; set; }
    }

    public class DictionaryTypeListParam
    {
        public DictionaryTypeListParam()
        {
            IsEnable = true;
        }
        public string DicTypeId { get; set; }

        public string Name { get; set; }

        public string ParentId { get; set; }

        public bool IsParent { get; set; }

        public bool IsEnable { get; set; }

        public bool IsDelete { get; set; }
    }

    public class EditTotalPackageTimeParam : SearchParam
    {
        public string VenderCode { get; set; }
        public string VenderName { get; set; }
        public string SearchContext { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string CreateBy { get; set; }
    }

    public class NoForecastAbnormalParam : SearchParam
    {
        public string CustomerCode { get; set; }
        public int? ShippingMethodId { get; set; }
        public string SearchContext { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Status { get; set; }
    }
}
