using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Controllers.FinancialController.ViewModels;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Web.Models;

namespace LMS.Controllers.FinancialController
{
    public class ExpressDeliveryFeeListModel
    {
        public ExpressDeliveryFeeListModel()
        {
            Filter = new ExpressDeliveryFeeListFilterModel();
            PagedList = new PagedList<ExpressDeliveryFeeModel>();
            SearchWhereTypes = new List<SelectListItem>();
            StatusList = new List<SelectListItem>();
        }
        public IList<SelectListItem> SearchWhereTypes { get; set; }
        public IList<SelectListItem> StatusList { get; set; } 
        public ExpressDeliveryFeeListFilterModel Filter { get; set; }
        public IPagedList<ExpressDeliveryFeeModel> PagedList { get; set; }
    }
    /// <summary>
    /// 快递发货费用查询
    /// </summary>
    public class ExpressDeliveryFeeListFilterModel : FinancialSearchFilter
    {
        public string VenderCode { get; set; }
        public string VenderName { get; set; }
        public string ShippingMethodId { get; set; }
        public string ShippingName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Status { get; set; }
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
        public string UserName { get; set; }
        public int ShippingType { get; set; }
    }
    /// <summary>
    /// 快递发货费用
    /// </summary>
    public class ExpressDeliveryFeeModel
    {
        public int DeliveryFeeID { get; set; }
        public string WayBillNumber { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string Trackingnumber { get; set; }
        public string VenderName { get; set; }
        public string VenderCode { get; set; }
        public int? VenderId { get; set; }
        public int? ShippingmethodID { get; set; }
        public string ShippingmethodName { get; set; }
        public decimal? SetWeight { get; set; }
        public decimal? AprroveWeight { get; set; }
        public int Status { get; set; }
        public string StatusStr { get; set; }
        public string Remark { get; set; }
        public string Auditor { get; set; }
        public DateTime? AuditorDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }

        /// <summary>
        /// 发货时间（出仓时间）
        /// </summary>
        public virtual DateTime? OutStorageCreatedOn { get; set; }
        public virtual string CountryCode { get; set; }
        public virtual string CountryChineseName { get; set; }
        /// <summary>
        /// 称重质量
        /// </summary>
        public virtual decimal? Weight { get; set; }
        //运费
        public virtual decimal? Freight { get; set; }
        //挂号费
        public virtual decimal? Register { get; set; }
        //燃油费
        public virtual decimal? FuelCharge { get; set; }
        //关税预付服务费
        public virtual decimal? TariffPrepayFee { get; set; }
        //附加费
        public virtual decimal? Surcharge { get; set; }
        //超长超重超周长费
        public virtual decimal? OverWeightLengthGirthFee { get; set; }
        //安全附加费
        public virtual decimal? SecurityAppendFee { get; set; }
        //增值税费
        public virtual decimal? AddedTaxFee { get; set; }
        //杂费
        public virtual decimal? OtherFee { get; set; }
        //总费用
        public virtual decimal? TotalFee { get; set; }

        //最终总费用
        public virtual decimal? TotalFeeFinal { get; set; }
        public virtual ExpressDeliveryImportDataModel VenderData { get; set; }
        //杂费备注
        public virtual string OtherFeeRemark { get; set; }

        public string CustomerName { get; set; }
    }
    /// <summary>
    /// 快递导入数据
    /// </summary>
    public class ExpressDeliveryImportDataModel
    {
        public virtual int ExpressDeliveryImportAccountCheckFinalID { get; set; }
        public virtual string OrderNumber { get; set; }
        public virtual string UserName { get; set; }
        public virtual string WayBillNumber { get; set; }
        public virtual DateTime? ReceivingDate { get; set; }
        public virtual string VenderName { get; set; }
        public virtual string ShippingMethodName { get; set; }
        public virtual string CountryName { get; set; }
        public virtual decimal? Weight { get; set; }
        public virtual decimal? SettleWeight { get; set; }
        public virtual decimal Freight { get; set; }
        public virtual decimal FuelCharge { get; set; }
        public virtual decimal Register { get; set; }
        public virtual decimal Surcharge { get; set; }
        public virtual decimal TariffPrepayFee { get; set; }
        public virtual decimal OverWeightLengthGirthFee { get; set; }
        public virtual decimal SecurityAppendFee { get; set; }
        public virtual decimal AddedTaxFee { get; set; }
        public virtual decimal Incidentals { get; set; }
        public virtual string IncidentalRemark { get; set; }
        public virtual decimal TotalFee { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual int Status { get; set; }
        public virtual string ErrorReason { get; set; }
    }
}