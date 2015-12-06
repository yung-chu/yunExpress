using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Web.Models;

namespace LMS.Controllers.WayBillController
{
    public class ShippingMethodModel
    {
        public int ShippingMethodId { get; set; }
        public string ShippingMethodName { get; set; }
        public bool WeightOrVolume { get; set; }
        public bool HaveTrackingNum { get; set; }
        public bool IsHideTrackingNumber { get; set; }
    }
    public class InStorageFormModel
    {
        public int CustomerType { get; set; }
        public string CustomerCode { get; set; }
        public int GoodsTypeID { get; set; }
        public int ShippingMethodId { get; set; }
        public string WayBillNumber { get; set; }
        public string TrackingNumber { get; set; }
        public decimal Weight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public bool ChkPrint { get; set; }
        public string PrintTemplateName { get; set; }

		public bool IsWeightAbnormalWaybill { get; set; }//是否重量异常单
    }
    public class OutStorageFormModel
    {
        public string VenderCode { get; set; }
        public int GoodsTypeID { get; set; }
        public int ShippingMethodId { get; set; }
        public string WayBillNumber { get; set; }
        public string CountryCode { get; set; }
        public bool IsBattery { get; set; }
    }
    public class InStorageSaveModel
    {
        public int CustomerType { get; set; }
        public string CustomerCode { get; set; }
        public int GoodsTypeID { get; set; }
        public int ShippingMethodId { get; set; }

		//判断用户操作
		public string Opereate { get; set; }
        public DateTime? GetBusinessDate { get; set; }//业务日期

    }
    public class WayBillInfoSaveModel
    {
        public string WayBillNumber { get; set; }
        public decimal Freight { get; set; }
        public decimal FuelCharge { get; set; }
        public decimal Register { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string TrackingNumber { get; set; }
        public decimal SettleWeight { get; set; }
        public decimal Weight { get; set; }
        public decimal Surcharge { get; set; }
        public decimal TariffPrepay { get; set; }
        public string OldTrackingNumber { get; set; }

    }
    public class InStorageWayBillModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string WayBillNumber { get; set; }
        public string TrackingNumber { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public decimal SettleWeight { get; set; }
        public string CustomerOrderNumber { get; set; }
        public decimal Freight { get; set; }
        public decimal Register { get; set; }
        public decimal FuelCharge { get; set; }
        public decimal Surcharge { get; set; }
        public decimal TariffPrepay { get; set; }
        public string HtmlString { get; set; }
        public decimal Weight { get; set; }
        public decimal Height { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
    }
    public class OutStorageWayBillModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string WayBillNumber { get; set; }
        public string TrackingNumber { get; set; }
        public string CountryCode { get; set; }
        public decimal SettleWeight { get; set; }
        public decimal Weight { get; set; }
        public string CustomerOrderNumber { get; set; }
        public decimal Freight { get; set; }
        public decimal Register { get; set; }
        public decimal FuelCharge { get; set; }
        public decimal Surcharge { get; set; }
    }
    public class WayBillInfoModel
    {
        public WayBillInfoModel()
        {
            ShippingInfo = new ShippingInfoModel();
            ApplicationInfos = new List<ApplicationInfoModel>();
            //AbnormalWayBillLog = new AbnormalWayBillLogModel();
        }
        public string WayBillNumber { get; set; }
        public string CustomerOrderNumber { get; set; }
        public int CustomerOrderID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCodeName { get; set; }
        public string TrackingNumber { get; set; }
        public string TrueTrackingNumber { get; set; }
        public string RawTrackingNumber { get; set; }

        //入仓出仓操作人

        public string InCreatedBy { get; set; }
        public string OutCreatedBy { get; set; }

        public Nullable<System.DateTime> TransferOrderDate { get; set; }
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
        public string OutStorageCreatedOn { get; set; }
        public string InStorageCreatedOn { get; set; }
        public Nullable<int> ShippingInfoID { get; set; }
        public virtual Nullable<int> SenderInfoID { get; set; }
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
        //public string AbnormalCreateBy { get; set; }
        //public string AbnormalTypeName { get; set; }
        //public string AbnormalDescription { get; set; }
        public string InShippingName { get; set; }
        public string OutShippingName { get; set; }
        public DateTime? InStorageTime { get; set; }
        public DateTime? OutStorageTime { get; set; }
        public ShippingInfoModel ShippingInfo { get; set; }
        public SenderInfoModel SenderInfo { get; set; }
        public InsuredCalculationModel InsuredCalculation { get; set; }
        public GoodsTypeInfoModel GoodsTypeInfo { get; set; }
        //public AbnormalWayBillLogModel AbnormalWayBillLog { get; set; }
        public CustomerOrderInfoModel CustomerOrderInfo { get; set; }
        public List<ApplicationInfoModel> ApplicationInfos { get; set; }
        public bool EnableTariffPrepay { get; set; }

		//日志打印表
	    public string wayNumber { get; set; }
		public string SendGoodsVender { get; set; }
		public string SendGoodsVenderName { get; set; }
		public string SendGoodsChannel { get; set; }
		public string SendGoodsChannelFullName { get; set; }
		public string ReturnUrl { get; set; }
    }


    public class WayBillExcelExport
    {

        public string WayBillNumber { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string CustomerCode { get; set; }
        public string TrackingNumber { get; set; }
        public string InShippingName { get; set; }
        public string OutShippingName { get; set; }
        public decimal? SettleWeight { get; set; }
        //入仓出仓操作人
        public string CountryCode { get; set; }
        public string InCreatedBy { get; set; }
        public string OutCreatedBy { get; set; }
        public bool IsHold { get; set; }
        public int Status { get; set; }
        public bool EnableTariffPrepay { get; set; }
        public DateTime CreatedOn { get; set; }
    }


    public class InStorageInfoModel
    {
        public InStorageInfoModel()
        {
            WayBillInfos = new List<WayBillInfoModel>();
            InStorageTotalModels = new List<InStorageTotalModel>();
        }
        public string InStorageID { get; set; }
        public string ReceivingClerk { get; set; }
        public string CustomerCode { get; set; }
        public Nullable<decimal> TotalWeight { get; set; }
        public Nullable<int> TotalQty { get; set; }
        public Nullable<decimal> MaterialsFee { get; set; }
        public Nullable<decimal> TotalFee { get; set; }
        public int Status { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public string Remark { get; set; }
        public Nullable<decimal> Freight { get; set; }
        public Nullable<decimal> Register { get; set; }
        public Nullable<decimal> FuelCharge { get; set; }
        public Nullable<decimal> Surcharge { get; set; }
        public Nullable<decimal> TariffPrepayFee { get; set; }
        public string ShippingMethodName { get; set; }
        public List<WayBillInfoModel> WayBillInfos { get; set; }
        public decimal PhysicalTotalWeight { get; set; }

        public List<InStorageTotalModel> InStorageTotalModels { get; set; }
    }
    public class OutStorageInfoModel
    {
        public OutStorageInfoModel()
        {
            WayBillInfos = new List<WayBillInfoModel>();
        }
        public string OutStorageID { get; set; }
        public string DeliveryStaff { get; set; }
        public string VenderCode { get; set; }
        public string VenderName { get; set; }
        public Nullable<decimal> TotalWeight { get; set; }
        public Nullable<int> TotalQty { get; set; }
        public Nullable<decimal> TotalFee { get; set; }
        public int Status { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public string Remark { get; set; }
        public Nullable<decimal> Freight { get; set; }
        public Nullable<decimal> Register { get; set; }
        public Nullable<decimal> FuelCharge { get; set; }
        public Nullable<decimal> Surcharge { get; set; }
        public List<WayBillInfoModel> WayBillInfos { get; set; }

        //是否可以修改出仓运输方式
        public bool isUpdate { get; set; }

        public string PostBagNumber { get; set; }
    }
    public class CustomerInStorageModel
    {
        public string CustomerCode { get; set; }
        public string AccountID { get; set; }
        public Nullable<int> CustomerTypeID { get; set; }
        public string Name { get; set; }
        public Nullable<int> PaymentTypeID { get; set; }
        public string PaymentTypeName { get; set; }
        public decimal Balance { get; set; }
    }
   
    public class GoodsTypeInfoModel
    {
        public int GoodsTypeID { get; set; }
        public string GoodsTypeName { get; set; }
    }

    public class VenderModel
    {
        public string VenderCode { get; set; }
        public string VenderName { get; set; }
    }
    public class ShippingInfoModel
    {
        public int ShippingInfoID { get; set; }
        public string CountryCode { get; set; }
        public string ShippingFirstName { get; set; }
        public string ShippingLastName { get; set; }
        public string ShippingCompany { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingAddress1 { get; set; }
        public string ShippingAddress2 { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZip { get; set; }
        public string ShippingPhone { get; set; }
        public string ShippingTaxId { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class SenderInfoModel
    {
        public int SenderInfoID { get; set; }
        public string CountryCode { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderCompany { get; set; }
        public string SenderAddress { get; set; }
        public string SenderCity { get; set; }
        public string SenderState { get; set; }
        public string SenderZip { get; set; }
        public string SenderPhone { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class ApplicationInfoModel
    {
        public int ApplicationID { get; set; }
        public Nullable<int> CustomerOrderID { get; set; }
        public string WayBillNumber { get; set; }
        public string ApplicationName { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<decimal> UnitWeight { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string HSCode { get; set; }
        public bool IsDelete { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public string PickingName { get; set; }
        public string Remark { get; set; }
        public string ProductUrl { get; set; }
    }

    public class ApplicationInfoModelCommon
    {
        public int? CustomerOrderID { get; set; }
        public string WayBillNumber { get; set; }
        public int ApplicationID { get; set; }
        public string ApplicationName { get; set; }
        public int Qty { get; set; }
        public decimal UnitWeight { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public string HSCode { get; set; }
        public string PickingName { get; set; }
        public string Remark { get; set; }
        public string ProductUrl { get; set; }
        public bool IsDelete { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
    }

    /// <summary>
    /// 打印发票的申报信息
    /// </summary>
    public class ApplicationInfoModel1
    {
        public int? CustomerOrderID { get; set; }
        public string WayBillNumber { get; set; }
        public int ApplicationID { get; set; }
        public string ApplicationName { get; set; }
        public int Qty { get; set; }
        public decimal UnitWeight { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public string HSCode { get; set; }
        public string PickingName { get; set; }
        public string Remark { get; set; }
    }


    public class InFeeInfoModel
    {
        public string InStorageID { get; set; }
        public string WayBillNumber { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string CustomerCode { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime InDateTime { get; set; }
        public int ShippingMethodID { get; set; }
        public string InShippingName { get; set; }
        public string CountryCode { get; set; }
        public string ChineseName { get; set; }
        public decimal SettleWeight { get; set; }
        public decimal Freight { get; set; }
        public decimal FuelCharge { get; set; }
        public decimal Register { get; set; }
        public decimal TariffPrepayFee { get; set; }
        public decimal Surcharge { get; set; }
        public decimal TotalFee { get; set; }
        public decimal Weight { get; set; }
    }
    public class OutFeeInfoModel
    {
        public string OutStorageID { get; set; }
        public string WayBillNumber { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string VenderCode { get; set; }
        public string VenderName { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime OutDateTime { get; set; }
        public int ShippingMethodID { get; set; }
        public string OutShippingName { get; set; }
        public string CountryCode { get; set; }
        public string ChineseName { get; set; }
        public decimal SettleWeight { get; set; }
        public decimal Freight { get; set; }
        public decimal FuelCharge { get; set; }
        public decimal Register { get; set; }
        public decimal Surcharge { get; set; }
        public decimal TotalFee { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public decimal Weight { get; set; }
    }
    public class AbnormalWayBillLogModel
    {
        public int AbnormalID { get; set; }
        public int OperateType { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string AbnormalDescription { get; set; }
        public Nullable<int> AbnormalStatus { get; set; }
    }

    public class InsuredCalculationModel
    {
        public int InsuredID { get; set; }
        public string InsuredName { get; set; }
        public string InsuredCalculation { get; set; }
        public int Status { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
    }

    public class SensitiveTypeInfoModel
    {
        public int SensitiveTypeID { get; set; }
        public string SensitiveTypeName { get; set; }
        public string Remark { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public bool IsDelete { get; set; }
    }

    public class CustomerOrderInfoModel
    {
        public int CustomerOrderID { get; set; }
        public string CustomerCode { get; set; }
        public int SensitiveTypeID { get; set; }
        public string CustomerOrderNumber { get; set; }
        public Nullable<int> InsuredID { get; set; }
        public int PackageNumber { get; set; }
        public Nullable<decimal> InsureAmount { get; set; }
        public int AppLicationType { get; set; }
    }

    public class CustomerOrderInfoModelCommon
    {
        public CustomerOrderInfoModelCommon()
        {
            this.ApplicationInfoList = new List<ApplicationInfoModelCommon>();
            PackageNumberValue = "1";

        }

        public int GoodsTypeID { get; set; }
        public int CustomerOrderID { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string WayBillNumber { get; set; }
        public string CountryCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ShippingFirstName { get; set; }
        public string ShippingLastName { get; set; }
        public string ShippingFirstLastName { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZip { get; set; }
        public string ShippingPhone { get; set; }
        public string ShippingCompany { get; set; }
        public string ShippingTaxId { get; set; }

        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderFirstLastName { get; set; }
        public string SenderCompany { get; set; }
        public string SenderAddress { get; set; }
        public string ShippingAddress1 { get; set; }
        public string ShippingAddress2 { get; set; }
        public string SenderCity { get; set; }
        public string SenderState { get; set; }
        public string SenderZip { get; set; }
        public string SenderPhone { get; set; }

        public int ShippingMethodId { get; set; }
        public string ShippingMethodName { get; set; }

        public decimal? Weight { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Height { get; set; }
        public decimal? SettleWeight { get; set; }
        public int? PackageNumber { get; set; }
        public string PackageNumberValue { get; set; }


        public bool IsReturn { get; set; }
        public bool IsInsured { get; set; }
        public bool IsBattery { get; set; }
        public bool IsPrinted { get; set; }
        public bool IsHold { get; set; }

        public int InsuredID { get; set; }
        public int SensitiveTypeID { get; set; }

        public int Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public string ProductDetail { get; set; }
        public string AbnormalDescription { get; set; }
        public string TrackingNumber { get; set; }
        public string RawTrackingNumber { get; set; }
        public Nullable<System.DateTime> TransferOrderDate { get; set; }
        public string CountryName { get; set; }

        public string CountryChineseName { get; set; }

        public string BarCode { get; set; }

        public string BarCode128 { get; set; }

        public string CustomerOrderNumberCode39 { get; set; }
        public string CustomerOrderNumberCode128 { get; set; }
        public string CustomerOrderNumberCode128L { get; set; }
        public string TrackingNumberCode39 { get; set; }
        public string TrackingNumberCode128 { get; set; }
        public string WayBillNumberCode39 { get; set; }
        public string WayBillNumberCode128 { get; set; }
        public string CustomerOrderNumberCode128Lh { get; set; }

        public int? ShippingZone { get; set; }

        public string ReturnUrl { get; set; }
        public string InsureAmountValue { get; set; }
        public string InsuredName { get; set; }
        public string SensitiveTypeName { get; set; }
        public decimal? InsureAmount { get; set; }
        public string AppLicationTypeId { get; set; }
        public int AppLicationType { get; set; }
        public string InsuredValue { get; set; }
        public string InsuredCalculationId { get; set; }
        public List<SelectListItem> AppLicationTypes { get; set; }
        public List<SelectListItem> InsuredCalculationsTypes { get; set; }
        public List<ApplicationInfoModelCommon> ApplicationInfoList { get; set; }
        public List<WayBillInfo> WayBillInfos { get; set; }

        public bool EnableTariffPrepay { get; set; }

        public string Remark { get; set; }

        //提交失败订单编辑时可保存
        public int? SubmitFailFlag { get; set; }

        /// <summary>
        /// 格口号
        /// </summary>
        public int MouthNumber { get; set; }
    }

    public class InvoivePrinterOrderInfoModel
    {
        public InvoivePrinterOrderInfoModel()
        {
            this.ApplicationInfoList = new List<ApplicationInfoModel1>();
            PackageNumberValue = "1";

        }

        public int GoodsTypeID { get; set; }
        public int CustomerOrderID { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string WayBillNumber { get; set; }
        public string CountryCode { get; set; }
        public string CustomerCode { get; set; }
        public string ShippingFirstName { get; set; }
        public string ShippingLastName { get; set; }
        public string ShippingFirstLastName { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZip { get; set; }
        public string ShippingPhone { get; set; }
        public string ShippingCompany { get; set; }
        public string ShippingTaxId { get; set; }

        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderFirstLastName { get; set; }
        public string SenderCompany { get; set; }
        public string SenderAddress { get; set; }
        public string SenderCity { get; set; }
        public string SenderState { get; set; }
        public string SenderZip { get; set; }
        public string SenderPhone { get; set; }

        public int ShippingMethodId { get; set; }
        public string ShippingMethodName { get; set; }

        public decimal? Weight { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Height { get; set; }
        public decimal? SettleWeight { get; set; }
        public int? PackageNumber { get; set; }
        public string PackageNumberValue { get; set; }


        public bool IsReturn { get; set; }
        public bool IsInsured { get; set; }
        public bool IsBattery { get; set; }
        public bool IsPrinted { get; set; }
        public bool IsHold { get; set; }

        public int InsuredID { get; set; }
        public int SensitiveTypeID { get; set; }

        public int Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public string ProductDetail { get; set; }
        public string AbnormalDescription { get; set; }
        public string TrackingNumber { get; set; }
        public string RawTrackingNumber { get; set; }
        public Nullable<System.DateTime> TransferOrderDate { get; set; }
        public string CountryName { get; set; }

        public string CountryChineseName { get; set; }

        public string BarCode { get; set; }

        public int? ShippingZone { get; set; }

        public string ReturnUrl { get; set; }
        public string InsureAmountValue { get; set; }
        public string InsuredName { get; set; }
        public string SensitiveTypeName { get; set; }
        public decimal? InsureAmount { get; set; }
        public string AppLicationTypeId { get; set; }
        public int AppLicationType { get; set; }
        public string InsuredValue { get; set; }
        public string InsuredCalculationId { get; set; }
        public List<SelectListItem> AppLicationTypes { get; set; }
        public List<SelectListItem> InsuredCalculationsTypes { get; set; }
        public List<ApplicationInfoModel1> ApplicationInfoList { get; set; }
        public List<WayBillInfo> WayBillInfos { get; set; }

    }

    public class CustomerOrderInfosModel
    {
        public CustomerOrderInfosModel()
        {
            this.ApplicationInfoList = new List<ApplicationInfoModels>();
          
        }

        public int GoodsTypeID { get; set; }
        public int CustomerOrderID { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string WayBillNumber { get; set; }
        public string CountryCode { get; set; }
        public string CustomerCode { get; set; }
        public string ShippingFirstName { get; set; }
        public string ShippingLastName { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZip { get; set; }
        public string ShippingPhone { get; set; }

        public int ShippingMethodId { get; set; }
        public string ShippingMethodName { get; set; }

        public bool IsReturn { get; set; }
        public bool IsInsured { get; set; }
        public bool IsBattery { get; set; }

        public bool IsHold { get; set; }

        public int InsuredID { get; set; }
        public int SensitiveTypeID { get; set; }

        public int AppLicationType { get; set; }

        public int Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ProductDetail { get; set; }
        public string AbnormalDescription { get; set; }
        public string TrackingNumber { get; set; }

        public string CountryName { get; set; }

        public string CountryChineseName { get; set; }
        public string BarCode { get; set; }

        public int? ShippingZone { get; set; }

        public List<ApplicationInfoModels> ApplicationInfoList { get; set; }
    }

    public class ApplicationInfoModels
    {
        public int ApplicationID { get; set; }
        public int CustomerOrderID { get; set; }
        public string WayBillNumber { get; set; }
        public string ApplicationName { get; set; }
        public int Qty { get; set; }
        public decimal UnitWeight { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public string HSCode { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
    }

    public class WayBillTemplateModel
    {
        public WayBillTemplateModel()
        {
            RowNumber = 1;
            ColumnNumber = 1;
            LinkMode = 1;
        }

        public  int WayBillTemplateId { get; set; }

        [Required(ErrorMessage = "模板名称不能为空")]
        public  string TemplateName { get; set; }

        public  string TemplateTypeId { get; set; }
        public  int ShippingMethodId { get; set; }
        public string ShippingMethodName { get; set; }
        public  int Status { get; set; }

        [Required(ErrorMessage = "模板内容不能为空")]
        public  string TemplateContent { get; set; }
        public  string Remark { get; set; }
        public  DateTime CreatedOn { get; set; }
        public  string CreatedBy { get; set; }
        public  DateTime LastUpdatedOn { get; set; }
        public  string LastUpdatedBy { get; set; }

        [Required(ErrorMessage = "请选择模板头")]
        public int TemplateHeadId { get; set; }

        [Required(ErrorMessage = "请选择模板体")]
        public int TemplateContentId { get; set; }

        public int LinkMode { get; set; }
        public string Countries { get; set; }

        public string TemplateTypeName { get; set; }
        public string SpecificationName { get; set; }
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
    }
    public class WayBillTemplateInfoModel
    {
        public int TemplateModelId { get; set; }
        public int TemplateType { get; set; }

        [Required(ErrorMessage = "模板名称不能为空")]
        public string TemplateName { get; set; }

        [Required(ErrorMessage = "模板类容不能为空")]
        public string TemplateContent { get; set; }

        public string Remarks { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
    }

    public class ReturnWayBillViewModel
    {
        public ReturnWayBillViewModel()
        {
            PagedReturnWayBillList = new PagedList<ReturnWayBillModel>();
            ReturnWayBillList=new List<ReturnWayBillModel>();
            FilterModel=new ReturnWayBillFilterModel();
            SearchWheres =new List<SelectListItem>();
            Customers=new List<SelectListItem>();
        }

        public IPagedList<ReturnWayBillModel> PagedReturnWayBillList { get; set; }
        public List<ReturnWayBillModel> ReturnWayBillList { get; set; }
        public ReturnWayBillFilterModel FilterModel { get; set; }
        public IList<SelectListItem> SearchWheres { get; set; }
        public IList<SelectListItem> Customers { get; set; }
    }

    public class ReturnWayBillModel
    {
        public string CustomerName { get; set; }

        public string WayBillNumber { get; set; }

        public string CustomerOrderNumber { get; set; }

        public string TrackingNumber { get; set; }

        public decimal? TotalWeight { get; set; }

        public string CountryCode { get; set; }

        public string ChineseName { get; set; }

        public decimal ShippingFee { get; set; }

        public int Type { get; set; }

        public string TypeName { get; set; }

        public string Reason { get; set; }

        public string ReasonRemark { get; set; }

        public bool IsReturnShipping { get; set; }

        public string IsReturnShippingName { get; set; }

        public DateTime? OutCreatedOn { get; set; }

        public DateTime ReturnCreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedByEeName { get; set; }

        public string InShippingMehtodName { get; set; }

        public string Auditor { get; set; }

        public DateTime? AuditorDate { get; set; }
    }

    public class ReturnWayBillFilterModel : SearchFilter
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public DateTime? ReturnStartTime { get; set; }
        public DateTime? ReturnEndTime { get; set; }
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
        public string CreateBy { get; set; }
        public string ShippingMehtodName { get; set; }
        public int ShippingMehtodId { get; set; }
    }

    public class ReturnAuditViewModel
    {
        public ReturnAuditViewModel()
        {
            PagedReturnAuditList=new PagedList<ReturnWayBillModel>();
            FilterModel=new ReturnAuditFilterModel();
            SearchWheres=new List<SelectListItem>();
            Customers=new List<SelectListItem>();
            Reasons=new List<SelectListItem>();
        }
        public IPagedList<ReturnWayBillModel> PagedReturnAuditList { get; set; }
        public ReturnAuditFilterModel FilterModel { get; set; }
        public IList<SelectListItem> SearchWheres { get; set; }
        public IList<SelectListItem> Customers { get; set; }
        public IList<SelectListItem> Reasons { get; set; }
    }

    public class ReturnAuditFilterModel : SearchFilter
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public DateTime? ReturnStartTime { get; set; }
        public DateTime? ReturnEndTime { get; set; }
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
        public string CreateBy { get; set; }
        public string ShippingMehtodName { get; set; }
        public int ShippingMehtodId { get; set; }
        public string ReturnReason { get; set; }
        public string IsReturnShipping { get; set; }
        public int DateWhere { get; set; }
    }

    public class UpdateReturnAuditViewModel
    {

        public string Type { get; set; }
        public string NewIsReturnShipping { get; set; }
        public string NewReturnReason { get; set; }
        public string WayBillList { get; set; }
    }


	public class InStorageWeightDeviationInfoModel
	{
		public int InStorageWeightDeviationID { get; set; }
		public string CustomerCode { get; set; }
		public string CustomerName { get; set; }
		public int? ShippingMethodID { get; set; }
		public string ShippingMethodName { get; set; }
		public decimal? DeviationValue { get; set; }
		public int Status { get; set; }
		public DateTime CreatedOn { get; set; }
		public string CreatedBy { get; set; }
		public DateTime LastUpdatedOn { get; set; }
		public string LastUpdatedBy { get; set; }
	}



	public class InStorageWeightDeviationModel
	{
		public InStorageWeightDeviationModel()
		{
            FilterModel=new InStorageWeightDeviationFilterModel();
			PagedList = new PagedList<InStorageWeightDeviationInfoModel>();
		}
		public InStorageWeightDeviationFilterModel FilterModel { get; set; }
		public IPagedList<InStorageWeightDeviationInfoModel> PagedList { get; set; }
		public int Type { get; set; }//区分新增与编辑
		public int InStorageWeightDeviationID { get; set; }
		public string CustomerCode { get; set; }
		public string CustomerName { get; set; }
		public int? ShippingMethodID { get; set; }
		public string ShippingMethodName { get; set; }
		public decimal DeviationValue { get; set; }
		public string ReturnUrl { get; set; }

	}

    public class OutStorageConfigureViewModel
    {
        public OutStorageConfigureViewModel()
        {
            InShippingMethods=new List<SelectListItem>();
            Venders=new List<SelectListItem>();
            OutShippingMethods=new List<SelectListItem>();
            OutStorageConfigureModel=new OutStorageConfigureModel();
            OutStorageConfigureModels=new List<OutStorageConfigureModel>();
        }

        public List<SelectListItem> InShippingMethods {get; set; }

        public List<SelectListItem> Venders { get; set; }

        public List<SelectListItem> OutShippingMethods { get; set; }

        public OutStorageConfigureModel OutStorageConfigureModel { get; set; }

        public List<OutStorageConfigureModel> OutStorageConfigureModels { get; set; }
    }

    public class OutStorageConfigureModel
    {
        public int DeliveryChannelConfigurationId { get; set; }
        public int InShippingMethodId { get; set; }
        public string InShippingMethodName { get; set; }
        public int VenderId { get; set; }
        public string VenderName { get; set; }
        public int OutShippingMethodId { get; set; }
        public string OutShippingMethodName { get; set; }
    }

    public class ShippingWayBillListViewModel
    {
        public ShippingWayBillListViewModel()
        {
            FilterModel=new ShippingWayBillFilterModel();
            SearchWheres=new List<SelectListItem>();
            PagedList=new PagedList<ShippingWayBillListModel>();
            List=new List<ShippingWayBillListModel>();
        }

        public List<ShippingWayBillListModel> List { get; set; }
        public IPagedList<ShippingWayBillListModel> PagedList { get; set; }
        public List<SelectListItem> SearchWheres { get; set; }
        public ShippingWayBillFilterModel FilterModel { get; set; }
    }

    public class ShippingWayBillListModel
    {
        public string WayBillNumber { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string TrackingNumber { get; set; }
        public string OutStorageID { get; set; }
        public string VenderName { get; set; }
        public string InShippingMethodName { get; set; }
        public string OutShippingMethodName { get; set; }
        public string CountryCode { get; set; }
        public DateTime? OutStorageCreatedOn { get; set; }
        public string OutStorageCreatedBy { get; set; }
        public bool IsUpdate { get; set; }
        public string Remark { get; set; }


        public int Status { get; set; }
        public int? InsuredID { get; set; }
        public bool IsReturn { get; set; }
        public int? SenderInfoID { get; set; }
        public int? ShippingInfoID { get; set; }
        public bool EnableTariffPrepay { get; set; }
        public DateTime? InStorageCreatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public decimal? Weight { get; set; }
        public decimal? SettleWeight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public int? CustomerOrderID { get; set; }

    }

    public class ShippingWayBillFilterModel : SearchFilter
    {
        public string VenderCode { get; set; }
        public string VenderName { get; set; }
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
        public string OutCreateBy { get; set; }
        public int? InShippingMehtodId { get; set; }
        public string InShippingMehtodName { get; set; }
        public int? OutShippingMehtodId { get; set; }
        public string OutShippingMehtodName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int DateWhere { get; set; }
        public string Filter { get; set; }
    }


	public class WaybillInfoUpdateViewModel
	{
		public WaybillInfoUpdateViewModel()
		{
			PagedList = new PagedList<WaybillInfoUpdateModel>();
			FilterModel = new WaybillInfoUpdateFilterModel();
			SearchWheres = new List<SelectListItem>();
			StatusList = new List<SelectListItem>();
			ShippingMethodLists = new List<SelectListItem>();
		}
		public IPagedList<WaybillInfoUpdateModel> PagedList { get; set; }
		public WaybillInfoUpdateFilterModel FilterModel { get; set; }
		public IList<SelectListItem> SearchWheres { get; set; }
		public IList<SelectListItem> StatusList { get; set; }
		public IList<SelectListItem> ShippingMethodLists { get; set; }

		/// 是否有批量拦截(取消)权限
		public bool DisplayBatchHold { get; set; }
		public bool DisplayCancelHold { get; set; }
		//修改批量运输方式权限
		public bool DisPlayModifyShippingMethod { get; set; }
	}

	public class WaybillInfoUpdateModel
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



	public class InStorageWeightAbnormalViewModel
	{
		public InStorageWeightAbnormalViewModel()
		{
			PagedList = new PagedList<InStorageWeightAbnormal>();
			FilterModel = new InStorageWeightAbnormalFilterModel();
			SearchWheres= new List<SelectListItem>();
            WeightListItem = new List<SelectListItem>();
		}
		public IPagedList<InStorageWeightAbnormal> PagedList { get; set; }
		public InStorageWeightAbnormalFilterModel FilterModel { get; set; }
		public IList<SelectListItem> SearchWheres { get; set; }
        public IList<SelectListItem> WeightListItem{ get; set; }
		public bool DisplayCancelHold { get; set; }
		public bool DisplayBatchDelete { get; set; }
		public bool IsFastInStorageBut { get; set; }
	}


	public class InStorageWeightAbnormal
	{
		public string WayBillNumber { get; set; }
		public string CustomerOrderNumber { get; set; }
		public string TrackingNumber { get; set; }
		public string CustomerCode { get; set; }
		public string CustomerName { get; set; }
		public string InShippingMethodName { get; set; }
		public int? InShippingMethodId { get; set; }
		public string CountryCode { get; set; }
		public decimal Weight { get; set; }
		public decimal ForecastWeight { get; set; }//称重重量
		public int OperateType { get; set; }
		public string AbnormalTypeName { get; set; }
		public string AbnormalDescription { get; set; }
		public DateTime CreatedOn { get; set; }
		public bool IsHold { get; set; }
	}






	public class InStorageWeightAbnormalParm
	{
        public WayBillInfoExtSilm WayBillInfoExtSilm { get; set; }
		public decimal Weight { get; set; }//称重重量
		public decimal? Length { get; set; }
		public decimal? Width { get; set; }
		public decimal? Height { get; set; }
		public decimal DeviationValue { get; set; }
		public decimal ConfigurationValue { get; set; }


	}






    #region 总包号编辑时间
    public class EditTotalPackageTimeFilterModel : SearchFilter
    {
        public string VenderCode { get; set; }
        public string VenderName { get; set; }
        public string SearchContext { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string CreateBy { get; set; }
    } 

    public class EditTotalPackageTimeListModel
    {
        public EditTotalPackageTimeListModel()
        {
            TraceInfo = new List<TotalPackageTraceInfoModel>();
        }
        public  string TotalPackageNumber { get; set; }
        public  string VenderCode { get; set; }
        public  string VenderName { get; set; }
        public  int TotalQty { get; set; }
        public  decimal TotalWeight { get; set; }
        public  int TotalVotes { get; set; }
        public  string Remark { get; set; }
        public  DateTime CreatedOn { get; set; }
        public  string CreatedBy { get; set; }
        public  DateTime LastUpdatedOn { get; set; }
        public  string LastUpdatedBy { get; set; }
        public List<TotalPackageTraceInfoModel> TraceInfo { get; set; } 
    }

    public class TotalPackageTraceInfoModel
    {
        public int ID { get; set; }
        public string TotalPackageNumber { get; set; }
        public int TraceEventCode { get; set; }
        public DateTime TraceEventTime { get; set; }
        public string TraceEventAddress { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public string TraceEventContent { get; set; }
        public bool IsJob { get; set; }
    }

    public class EditTotalPackageTimeOneModel
    {
        public EditTotalPackageTimeOneModel()
        {
            TraceInfos = new List<TotalPackageTraceInfoModel>();
        }
        public DateTime CreatedTime { get; set; }
        public string TotalPackageNumber { get; set; }
        public TotalPackageAddressExt AddressList { get; set; }
        public List<TotalPackageTraceInfoModel> TraceInfos { get; set; }
    }

    public class EditTotalPackageTimeViewModel
    {
        public EditTotalPackageTimeViewModel()
        {
            FilterModel = new EditTotalPackageTimeFilterModel();
            PagedList = new PagedList<TotalPackageInfoExt>();
        }
        public EditTotalPackageTimeFilterModel FilterModel { get; set; }
        public IPagedList<TotalPackageInfoExt> PagedList { get; set; }
    }
    #endregion

    public class WayBillSummaryViewModel
    {
        public WayBillSummaryViewModel()
        {
            FilterModel = new WaybillSummaryParam();
            List = new List<WaybillSummary>();
            SelectTimeNames = new List<SelectListItem>();
            SelectShippingMethods=new List<SelectListItem>();
            SelectStatus = new List<SelectListItem>();
        }
        public WaybillSummaryParam FilterModel { get; set; }
        public List<WaybillSummary> List { get; set; }
        public IList<SelectListItem> SelectTimeNames { get; set; }
        public IList<SelectListItem> SelectShippingMethods { get; set; }
        public IList<SelectListItem> SelectStatus { get; set; } 
    }
    
}