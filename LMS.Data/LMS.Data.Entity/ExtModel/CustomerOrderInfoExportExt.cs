using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace LMS.Data.Entity
{
    public class CustomerOrderInfoExportExt:LighTake.Infrastructure.Seedwork.Entity
    {
        public int? GoodsTypeID { get; set; }
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

        public int? ShippingMethodId { get; set; }
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

        public int? InsuredID { get; set; }
        public int? SensitiveTypeID { get; set; }

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

        public string CustomerOrderNumberCode { get; set; }

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
        public List<ApplicationInfoList> ApplicationInfoList { get; set; }
        public List<WayBillInfo> WayBillInfos { get; set; }
    }
    public class ApplicationInfoList
    {
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
}
