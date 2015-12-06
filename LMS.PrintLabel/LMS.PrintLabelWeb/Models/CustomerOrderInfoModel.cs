using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Data.Entity;

namespace LMS.PrintLabelWeb.Models
{
    public class CustomerOrderInfoModel
    {
        public CustomerOrderInfoModel()
        {
            this.ApplicationInfoList = new List<ApplicationInfoModel>();
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
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
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
        public DateTime? TransferOrderDate { get; set; }
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
        public List<ApplicationInfoModel> ApplicationInfoList { get; set; }
        public List<WayBillInfo> WayBillInfos { get; set; }

        public bool EnableTariffPrepay { get; set; }

        public string Remark { get; set; }

        //提交失败订单编辑时可保存
        public int? SubmitFailFlag { get; set; }

        /// <summary>
        /// 格口号
        /// </summary>
        public int MouthNumber { get; set; }

        //广州小包专用发货地址
        public string Address { get; set; }

        //广州小包专用发货人
        public string Name { get; set; }

        //分拣标识
        public string SortingIdentity { get; set; }

        //带电标识
        public string BatteryIdentity { get; set; }

    }
}