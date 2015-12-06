using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Web.Models;
using System.Web.Mvc;

namespace LMS.Controllers.FinancialController.ViewModels
{
    /// <summary>
    /// Delivery Cost Details View Model
    /// </summary>
    public class DeliveryCostDetailsModel
    {
        public DeliveryCostDetailsModel()
        {
            Filter = new DeliveryCostDetailsFilterModel();
            PagedList = new PagedList<DeliveryFeeModel>();
            SearchWhereTypes = new List<SelectListItem>();
        }
        public IList<SelectListItem> SearchWhereTypes { get; set; }
        public DeliveryCostDetailsFilterModel Filter { get; set; }
        public IPagedList<DeliveryFeeModel> PagedList { get; set; }
        
    }

    public partial class DeliveryImportDataModel 
    {
        public virtual int DeliveryImportAccountCheckID { get; set; }
        public virtual string WayBillNumber { get; set; }
        public virtual string OrderNumber { get; set; }
        public virtual string ReceivingDateStr { get; set; }
        public virtual Nullable<System.DateTime> ReceivingDate { get; set; }
        public virtual string VenderName { get; set; }
        public virtual string ShippingMethodName { get; set; }
        public virtual string CountryName { get; set; }
        public virtual Nullable<decimal> Weight { get; set; }
        public virtual Nullable<decimal> SettleWeight { get; set; }
        public virtual Nullable<decimal> TotalFee { get; set; }
        public virtual System.DateTime CreatedOn { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual Nullable<int> Status { get; set; }
        public virtual string ErrorReason { get; set; }
    }

    /// <summary>
    /// Data View Model
    /// </summary>
    public class DeliveryFeeModel
    {
        public int DeliveryFeeID { get; set; }
        public string WayBillNumber { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string Trackingnumber { get; set; }
        public string VenderName { get; set; }
        public string VenderCode { get; set; }
        public Nullable<int> VenderId { get; set; }
        public Nullable<int> ShippingmethodID { get; set; }
        public string ShippingmethodName { get; set; }
        public Nullable<decimal> SetWeight { get; set; }
        public Nullable<decimal> AprroveWeight { get; set; }
        public int Status { get; set; }
        public string Remark { get; set; }
        public string Auditor { get; set; }
        public Nullable<System.DateTime> AuditorDate { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }

        /// <summary>
        /// 发货时间（出仓时间）
        /// </summary>
        public virtual System.DateTime? OutStorageCreatedOn { get; set; }
        public virtual string CountryCode { get; set; }
        public virtual string CountryChineseName { get; set; }
        /// <summary>
        /// 称重质量
        /// </summary>
        public virtual Nullable<decimal> Weight { get; set; }
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
        //增值税率费
        public virtual decimal? AddedTaxFee { get; set; }
        //杂费
        public virtual decimal? OtherFee { get; set; }
        //总费用
        public virtual decimal? TotalFee { get; set; }

        //最终总费用
        public virtual decimal? TotalFeeFinal { get; set; }

        public virtual DeliveryImportDataModel VenderData { get; set; }

        //杂费备注
        public virtual string OtherFeeRemark { get; set; }

        public string CustomerName { get; set; }
    }

    /// <summary>
    /// SearchFilter
    /// </summary>
    public class DeliveryCostDetailsFilterModel : FinancialSearchFilter
    {
        public string VenderCode { get; set; }
        public string VenderName { get; set; }
        public string ShippingMethodId { get; set; }
        public string ShippingName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Status { get; set; }
        //public string CountryCode { get; set; }
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
        public string UserName { get; set; }
        public bool? IsFirstIn { get; set; }
        public bool? IsExportExcel { get; set; }
        public int ShippingType { get; set; }
    }

}