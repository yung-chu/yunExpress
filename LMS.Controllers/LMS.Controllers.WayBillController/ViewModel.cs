using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Seedwork;
using LMS.Models;

namespace LMS.Controllers.WayBillController
{
    public class InStorageScanViewModel
    {
        public InStorageScanViewModel()
        {
            GoodsTypeModels = new List<SelectListItem>();
            ScanTypeModels = new List<SelectListItem>();
            PrintTemplate = new List<SelectListItem>();
            PrintSpecification = new List<SelectListItem>();
            SensitiveTypes=new List<SelectListItem>();
        }

        public IList<SelectListItem> GoodsTypeModels { get; set; }
        public IList<SelectListItem> ScanTypeModels { get; set; }
        public IList<SelectListItem> PrintTemplate { get; set; }
        public IList<SelectListItem> PrintSpecification { get; set; }
        public IList<SelectListItem> SensitiveTypes { get; set; }
        public DateTime BusinessDate { get; set; }//业务日期
    }

    public class OutStorageScanViewModel
    {
        public OutStorageScanViewModel()
        {
            GoodsTypeModels = new List<SelectListItem>();
        }

        public IList<SelectListItem> GoodsTypeModels { get; set; }
    }

    public class SelectShippingMethodViewModel
    {
        public SelectShippingMethodViewModel()
        {
            ShippingMethodList = new List<ShippingMethodModel>();
        }

        public List<ShippingMethodModel> ShippingMethodList { get; set; }
        public string CustomerId { get; set; }
        public int? CustomerTypeId { get; set; }
        public string VenderCode { get; set; }
        public int SelectType { get; set; }
    }

    public class InStorageInfoModelDetailViewModel
    {
        public InStorageInfoModelDetailViewModel()
        {
            InStorageInfoModel = new InStorageInfoModel();
            Customer = new CustomerInStorageModel();
        }

        public InStorageInfoModel InStorageInfoModel { get; set; }
        public CustomerInStorageModel Customer { get; set; }
    }


    public class OutStorageInfoDetailViewModel
    {
        public OutStorageInfoDetailViewModel()
        {
            OutStorageInfo = new OutStorageInfoModel();
        }

        public OutStorageInfoModel OutStorageInfo { get; set; }
        public string VenderName { get; set; }
        public string ShippingMethodName { get; set; }
    }

    public class InStorageListViewModel
    {
        public InStorageListViewModel()
        {
            FilterModel = new InStorageFilterModel();
            PagedList = new PagedList<InStorageInfoExt>();
        }

        public InStorageFilterModel FilterModel { get; set; }
        public IPagedList<InStorageInfoExt> PagedList { get; set; }
    }

    public class OutStorageListViewModel
    {
        public OutStorageListViewModel()
        {
            FilterModel = new OutStorageFilterModel();
            PagedList = new PagedList<OutStorageInfoModel>();
        }

        public OutStorageFilterModel FilterModel { get; set; }
        public IPagedList<OutStorageInfoModel> PagedList { get; set; }
    }

    public class WayBillListViewModelBase
    {
        public WayBillListViewModelBase()
        {
            FilterModel = new WayBillListFilterModel();
            //PagedList = new PagedList<WayBillInfoModel>();
            SearchWheres = new List<SelectListItem>();
            StatusList = new List<SelectListItem>();
            DateWheres = new List<SelectListItem>();
            WayBillInfoModels = new List<WayBillInfoModel>();
            ShippingMethodLists = new List<SelectListItem>();
            WayBillTrackingNumbers = new List<WayBillTrackingNumber>();
            WayBillTNPagedList = new PagedList<WayBillTrackingNumber>();
            CustomerList = new List<SelectListItem>();
            GetStatusList = new List<DropListStatus>();
            OperatorTypeList = new List<SelectListItem>();
        }

        public WayBillListFilterModel FilterModel { get; set; }
        //public IPagedList<WayBillInfoModel> PagedList { get; set; }
        public IList<SelectListItem> SearchWheres { get; set; }
        public IList<SelectListItem> DateWheres { get; set; }
        public IList<SelectListItem> StatusList { get; set; }
        public List<WayBillInfoModel> WayBillInfoModels { get; set; }
        public IList<SelectListItem> ShippingMethodLists { get; set; }
        public List<WayBillTrackingNumber> WayBillTrackingNumbers { get; set; }
        public IPagedList<WayBillTrackingNumber> WayBillTNPagedList { get; set; }
        public List<SelectListItem> CustomerList { get; set; }

        public List<DropListStatus> GetStatusList { get; set; }

        public List<SelectListItem> OperatorTypeList { get; set; }

        public string ReturnStatusList { get; set; }

        /// <summary>
        /// 是否有直接出仓权限
        /// </summary>
        public bool IsFastOutStorageBut { get; set; }

        /// <summary>
        /// 是否有直接入仓权限
        /// </summary>
        public bool IsFastInStorageBut { get; set; }

        public bool DisplayBatchDelete { get; set; }
        public bool DisplayBatchHold { get; set; }
        public bool DisPlayModifyShippingMethod { get; set; }

        public bool BtnSuccess { get; set; }
        public int SuccessTotal { get; set; }
        public int FailureTotal { get; set; }
        public int Total { get; set; }
        public string UniqueExcelFileName { get; set; }
    }

    public class DropListStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }




    public class WayBillListViewModel : WayBillListViewModelBase
    {
        public WayBillListViewModel()
        {
            ExportPagedList=new PagedList<WayBillExcelExport>();
            PagedList = new PagedList<WayBillInfoModel>();
        }

        public IPagedList<WayBillExcelExport> ExportPagedList { get; set; }
        public IPagedList<WayBillInfoModel> PagedList { get; set; }
    }

    public class WayBillListSilmViewModel : WayBillListViewModelBase
    {
        public WayBillListSilmViewModel()
        {
            PagedList = new PagedList<WayBillInfoListSilm>();
        }

        public IPagedList<WayBillInfoListSilm> PagedList { get; set; }
    }

    public class AbnormalWayBillListViewModel
    {
        public AbnormalWayBillListViewModel()
        {
            FilterModel = new AbnormalWayBillListFilterModel();
            PagedList = new PagedList<AbnormalWayBillModel>();
            List = new List<AbnormalWayBillModel>();
            SearchWheres = new List<SelectListItem>();
            StatusList = new List<SelectListItem>();
            WayBillStatusList = new List<SelectListItem>();
            DateWheres = new List<SelectListItem>();
        }

        public AbnormalWayBillListFilterModel FilterModel { get; set; }
        public IPagedList<AbnormalWayBillModel> PagedList { get; set; }
        public List<AbnormalWayBillModel> List { get; set; }
        public IList<SelectListItem> SearchWheres { get; set; }
        public IList<SelectListItem> DateWheres { get; set; }
        public IList<SelectListItem> StatusList { get; set; }
        public IList<SelectListItem> WayBillStatusList { get; set; }
        public bool DisplayCancelHold { get; set; }
        public bool DisplayBatchDelete { get; set; }
    }



    public class InFeeInfoListViewModel
    {
        public InFeeInfoListViewModel()
        {
            FilterModel = new InFeeListFilterModel();
            PagedList = new PagedList<InFeeInfoModel>();
            SearchWheres = new List<SelectListItem>();
        }

        public List<InFeeInfoModel> List { get; set; }
        public InFeeListFilterModel FilterModel { get; set; }
        public IPagedList<InFeeInfoModel> PagedList { get; set; }
        public IList<SelectListItem> SearchWheres { get; set; }
        public decimal AllTotalFee { get; set; }
    }

    public class OutFeeInfoListViewModel
    {
        public OutFeeInfoListViewModel()
        {
            FilterModel = new OutFeeListFilterModel();
            PagedList = new PagedList<OutFeeInfoModel>();
            SearchWheres = new List<SelectListItem>();
        }

        public List<OutFeeInfoModel> List { get; set; }
        public OutFeeListFilterModel FilterModel { get; set; }
        public IPagedList<OutFeeInfoModel> PagedList { get; set; }
        public IList<SelectListItem> SearchWheres { get; set; }
        public decimal AllTotalFee { get; set; }
    }

    public class FastOutStorageViewModel
    {
        public FastOutStorageViewModel()
        {
            GoodsTypeModels = new List<SelectListItem>();
        }

        public string WayBillNumbers { get; set; }
        public string ReturnUrl { get; set; }
        public IList<SelectListItem> GoodsTypeModels { get; set; }
        public int GoodsTypeId { get; set; }
        public string VenderCode { get; set; }
        public string VenderName { get; set; }
        public int ShippingMethodId { get; set; }
        public string ShippingMethodName { get; set; }
        public string ErrorMessage { get; set; }
        //总票数
        public int TotalVotes { get; set; }
        //总件数
        public int TotalQty { get; set; }
        //总重量
        public decimal TotalWeight { get; set; }
        public int? SelectTotalPackage { get; set; }
        public string TotalPackageNumber { get; set; }
        public string Remark { get; set; }
    }

    public class FastInStorageViewModel
    {
        public FastInStorageViewModel()
        {
            GoodsTypeModels = new List<SelectListItem>();
        }

        public string WayBillNumbers { get; set; }
        public string ReturnUrl { get; set; }
        public IList<SelectListItem> GoodsTypeModels { get; set; }
        public int GoodsTypeId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerNickName { get; set; }
        public int ShippingMethodId { get; set; }
        public string ShippingMethodName { get; set; }
        public string ErrorMessage { get; set; }

        //用于判断用户操作
        public string Opereate { get; set; }

    }

    public class WayBillTemplateListViewModel
    {
        public WayBillTemplateListViewModel()
        {
            ShippingMethods = new List<SelectListItem>();
            PagedList = new PagedList<WayBillTemplateModel>();
            FilterModel = new WayBillTemplateListFilterModel();
        }

        public WayBillTemplateListFilterModel FilterModel { get; set; }
        public string ShippingMethodId { get; set; }
        public IList<SelectListItem> ShippingMethods { get; set; }
        public IPagedList<WayBillTemplateModel> PagedList { get; set; }
    }

    public class WayBillTemplateViewModel
    {
        public WayBillTemplateViewModel()
        {
            ShippingMethods = new List<SelectListItem>();
            WayBillTemplateTypes = new List<SelectListItem>();
            FilterModel = new WayBillTemplateModel();
            WayBillTemplateStatus = new List<SelectListItem>();
            WayBillTemplateHead = new List<SelectListItem>();
            WayBillTemplateBody = new List<SelectListItem>();
            RowNumber = 1;
            ColumnNumber = 1;

        }

        public WayBillTemplateModel FilterModel { get; set; }
        public IList<SelectListItem> ShippingMethods { get; set; }
        public IList<SelectListItem> WayBillTemplateTypes { get; set; }
        public IList<SelectListItem> WayBillTemplateStatus { get; set; }
        public List<SelectListItem> WayBillTemplateHead { get; set; }
        public List<SelectListItem> WayBillTemplateBody { get; set; }
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
    }

    public class WayBillTemplateInfoViewModel
    {
        public WayBillTemplateInfoViewModel()
        {
            WayBillTemplateInfo = new WayBillTemplateInfoModel();
            TemplateInfoParam = new WayBillTemplateInfoParam();
            WayBillTemplateInfoList = new PagedList<WayBillTemplateInfoModel>();
        }

        public WayBillTemplateInfoModel WayBillTemplateInfo { get; set; }
        public WayBillTemplateInfoParam TemplateInfoParam { get; set; }
        public List<SelectListItem> TemplateTypeList { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public PagedList<WayBillTemplateInfoModel> WayBillTemplateInfoList { get; set; }
    }

    public class ExpressWayBillViewModel
    {
        public ExpressWayBillViewModel()
        {
            FilterModel = new ExpressWayBillFilterModel();
            PagedList = new PagedList<ExpressWayBillInfoModel>();
            ExpressWayBillInfoModels = new List<ExpressWayBillInfoModel>();
            SearchWheres = new List<SelectListItem>();
            DateWheres = new List<SelectListItem>();
            StatusList = new List<SelectListItem>();
            Customers = new List<SelectListItem>();
        }

        public IPagedList<ExpressWayBillInfoModel> PagedList { get; set; }
        public List<ExpressWayBillInfoModel> ExpressWayBillInfoModels { get; set; }
        public ExpressWayBillFilterModel FilterModel { get; set; }
        public IList<SelectListItem> SearchWheres { get; set; }
        public IList<SelectListItem> DateWheres { get; set; }
        public IList<SelectListItem> StatusList { get; set; }
        public IList<SelectListItem> Customers { get; set; }

    }

    public class ExpressWayBillInfoModel
    {
        public ExpressWayBillInfoModel()
        {
            wayBillDetails = new List<WayBillDetailModel>();
        }

        public string WayBillNumber { get; set; }
        public virtual string CustomerOrderNumber { get; set; }
        public string CustomerCode { get; set; }
        public string TrackingNumber { get; set; }
        public int InShippingMethodID { get; set; }
        public string InShippingMethodName { get; set; }
        public int Status { get; set; }
        public decimal Weight { get; set; }
        public decimal SettleWeight { get; set; }
        public string OutStorageID { get; set; }
        public DateTime? OutStorageTime { get; set; }
        public string CountryCode { get; set; }

        public List<WayBillDetailModel> wayBillDetails { get; set; }

    }

    public class WayBillDetailModel
    {
        public int PackageDetailID { get; set; }
        public decimal Weight { get; set; }
        public decimal SettleWeight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal AddWeight { get; set; }
    }

    public class WayBillTrackingNumber
    {
        public WayBillTrackingNumber()
        {
            ErrorMsg = new StringBuilder();
        }

        public int execlRow { get; set; }
        public string WayBillNumber { get; set; }
        public string TrackingNumber { get; set; }
        public StringBuilder ErrorMsg { get; set; }
    }

    public class OutShippingMethodViewModel
    {
        public OutShippingMethodViewModel()
        {
            ShippingMethodList = new List<ShippingMethodModel>();
        }

        public List<ShippingMethodModel> ShippingMethodList { get; set; }
        public int VenderId { get; set; }
    }

    public class InStorageSyncErrorListModel
    {
        public LighTake.Infrastructure.Web.Models.SearchFilter SearchParam { get; set; }

        public IPagedList<TaskModel> List { get; set; }
    }


    public class ModifWayBillDetailModel
    {
        public ModifWayBillDetailModel()
        {
            InsuredList = new List<SelectListItem>();
            SensitiveTypeList = new List<SelectListItem>();
            ApplicationTypeList = new List<SelectListItem>();
        }

        public string WayBillNumber { get; set; }
        public int? ChangeType { get; set; }
        public string ChangeReason { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerId { get; set; }
        public int? CustomerTypeId { get; set; }
        public string CustomerName { get; set; }
        public int ShippingMethodId { get; set; }
        public string ShippingName { get; set; }
        public string CountryCode { get; set; }
        public string CountryChineseName { get; set; }
        public string TrackingNumber { get; set; }
		//件数
		public int PackageNumber { get; set; }
		public bool IsSubmitStatus { get; set; }

	    //收件人信息
        public ShippingInfo ShippingInfo { get; set; }

        //发件人信息
        public SenderInfo SenderInfo { get; set; }

        //申报信息
        public int AppLicationType { get; set; }
        public List<ApplicationInfo> ApplicationInfos { get; set; }

        public List<SelectListItem> InsuredList { get; set; }
        public List<SelectListItem> SensitiveTypeList { get; set; }
        public List<SelectListItem> ApplicationTypeList { get; set; }

        public bool IsInsured { get; set; }
        public int? InsuredID { get; set; }
        public decimal? InsureAmount { get; set; }
        public int? SensitiveTypeID { get; set; }
        public bool IsReturn { get; set; }
        public bool EnableTariffPrepay { get; set; }
        public string ReturnUrl { get; set; }

        public DateTime BusinessDate { get; set; }//业务日期
    }

    public class NoForecastAbnormalViewModel
    {
        public NoForecastAbnormalViewModel()
        {
            StatusList=new List<SelectListItem>();
            FilterModel=new NoForecastAbnormalFilterModel();
            PagedList = new PagedList<NoForecastAbnormalExt>();
        }

        public IPagedList<NoForecastAbnormalExt> PagedList { get; set; }
        public List<SelectListItem> StatusList { get; set; }

        public NoForecastAbnormalFilterModel FilterModel { get; set; }
	    public bool DisplayDelete { get; set; }
    }

    public class PrinterViewModel
    {
        public PrinterViewModel()
        {
            SelectList = new List<SelectListItem>();
            CustomerOrderInfoModels = new List<CustomerOrderInfoModel>();
            WayBillTemplates = new List<WayBillTemplateExt>();
        }

        public int Type { get; set; }
        public string Ids { get; set; }
        public string TypeId { get; set; }
        public string TemplateName { get; set; }
        public List<SelectListItem> SelectList { get; set; }
        public List<CustomerOrderInfoModel> CustomerOrderInfoModels { get; set; }
        public IEnumerable<WayBillTemplateExt> WayBillTemplates { get; set; }

    }

    public class PrinterViewModelCommon
    {
        public PrinterViewModelCommon()
        {
            SelectList = new List<SelectListItem>();
            CustomerOrderInfoModels = new List<CustomerOrderInfoModelCommon>();
            WayBillTemplates = new List<WayBillTemplateExt>();
        }

        public int Type { get; set; }
        public string Ids { get; set; }
        public string TypeId { get; set; }
        public string TemplateName { get; set; }
        public List<SelectListItem> SelectList { get; set; }
        public List<CustomerOrderInfoModelCommon> CustomerOrderInfoModels { get; set; }
        public IEnumerable<WayBillTemplateExt> WayBillTemplates { get; set; }

    }
}