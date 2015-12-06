using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class WayBillListExportModel:LighTake.Infrastructure.Seedwork.Entity
    {
        //运单信息表
        public string CustomerOrderNumber { get; set; }
        public string CustomerCode { get; set; }
        public string Name { get; set; }
        public string WayBillNumber { get; set; }
        public bool IsReturn { get; set; }
        public string TrackingNumber { get; set; }
        public string TrueTrackingNumber { get; set; }
        public string InShippingMethodName { get; set; }
        public string InsuredName { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? InStorageCreatedOn { get; set; }
        public DateTime? OutStorageCreatedOn { get; set; }
        public int Status { get; set; }
        public bool EnableTariffPrepay { get; set; }
        public decimal? Weight { get; set; }
        public decimal? SettleWeight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        //客户订单信息
        public decimal? InsureAmount { get; set; }
        public int? PackageNumber { get; set; }
        public int AppLicationType { get; set; }
        //收件人信息
        public string CountryCode { get; set; }
        public string ChineseName { get; set; }
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
        //发件人信息
        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderCompany { get; set; }
        public string SenderAddress { get; set; }
        public string SenderCity { get; set; }
        public string SenderState { get; set; }
        public string SenderZip { get; set; }
        public string SenderPhone { get; set; }
        //敏感货物类型表
        public string SensitiveTypeName { get; set; }
    }
    public class ApplicationInfoExportModel:LighTake.Infrastructure.Seedwork.Entity
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
}
