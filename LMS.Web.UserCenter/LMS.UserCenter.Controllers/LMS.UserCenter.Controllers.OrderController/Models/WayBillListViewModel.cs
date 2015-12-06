using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Web.Models;

namespace LMS.UserCenter.Controllers.OrderController.Models
{
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
    }

    public class WayBillListViewModel
    {
        public WayBillListViewModel()
        {
            Filter = new WayBillListFilterModel();
            PagedList = new PagedList<WayBillInfoModel>();
            SearchWheres = new List<SelectListItem>();
            StatusList = new List<SelectListItem>();
            DateWheres = new List<SelectListItem>();
        }
        public WayBillListFilterModel Filter { get; set; }
        public IPagedList<WayBillInfoModel> PagedList { get; set; }
        public IList<SelectListItem> SearchWheres { get; set; }
        public IList<SelectListItem> DateWheres { get; set; }
        public IList<SelectListItem> StatusList { get; set; }
        public bool IsFastOutStorageBut { get; set; }
    }

    public class PrinterViewModel
    {
        public PrinterViewModel()
        {
            SelectList = new List<SelectListItem>();
            CustomerOrderInfoModels=new List<CustomerOrderInfoModel>();
            WayBillTemplates=new List<WayBillTemplateExt>();
        }

        public int Type { get; set; }
        public string Ids { get; set; }
        public string TypeId { get; set; }
        public string TemplateName { get; set; }
        public List<SelectListItem> SelectList { get; set; }
        public List<CustomerOrderInfoModel> CustomerOrderInfoModels { get; set; }
        public IEnumerable<WayBillTemplateExt> WayBillTemplates { get; set; }
        
    }

    public class PrinterTemplateViewModel
    {
        public PrinterTemplateViewModel()
        {
            CustomerOrderInfoModels = new List<CustomerOrderInfoModel>();
        }

        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public string TemplateHead { get; set; }
        public string TemplateBodyContent { get; set; }
        public List<CustomerOrderInfoModel> CustomerOrderInfoModels { get; set; }
    }

    public class WayBillInfoModel
    {
        public WayBillInfoModel()
        {
            ShippingInfo = new ShippingInfoModel();
            ApplicationInfos = new List<ApplicationInfoModel>();

        }
        public string WayBillNumber { get; set; }
        public string CustomerOrderNumber { get; set; }
        public int CustomerOrderID { get; set; }
        public string CustomerCode { get; set; }
        public string TrackingNumber { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public Nullable<decimal> SettleWeight { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Height { get; set; }
        public Nullable<int> GoodsTypeID { get; set; }
        public bool IsReturn { get; set; }
        public bool IsHold { get; set; }
        public bool IsBattery { get; set; }
        public int Status { get; set; }
        public string OutStorageID { get; set; }
        public string InStorageID { get; set; }
        public Nullable<int> ShippingInfoID { get; set; }
        public string CountryCode { get; set; }
        public Nullable<int> InsuredID { get; set; }
        public Nullable<int> AbnormalID { get; set; }
        public Nullable<int> InShippingMethodID { get; set; }
        public Nullable<int> OutShippingMethodID { get; set; }
        public string InShippingMethodName { get; set; }
        public string OutShippingMethodName { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public string VenderName { get; set; }
        public DateTime? AbnormalCreateOn { get; set; }
        public string AbnormalTypeName { get; set; }
        public string AbnormalDescription { get; set; }
        public string InShippingName { get; set; }
        public string OutShippingName { get; set; }
        public DateTime? InStorageTime { get; set; }
        public DateTime? OutStorageTime { get; set; }
        public ShippingInfoModel ShippingInfo { get; set; }

        public List<ApplicationInfoModel> ApplicationInfos { get; set; }

    }

    public class ShippingInfoModel
    {
        public int ShippingInfoID { get; set; }
        public string CountryCode { get; set; }
        public string ShippingFirstName { get; set; }
        public string ShippingLastName { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZip { get; set; }
        public string ShippingPhone { get; set; }
    }
}