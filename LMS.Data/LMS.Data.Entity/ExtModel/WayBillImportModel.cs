using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity.ExtModel
{
    public class WayBillImportModel
    {
        public string WayBillNumber { get; set; }
        public string CustomerCode { get; set; }
        public string TrackingNumber { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public int? GoodsTypeID { get; set; }
        public bool IsReturn { get; set; }
        public string CountryCode { get; set; }
        public int? InsuredID { get; set; }
        public int? InShippingMethodID { get; set; }
        public string InShippingMethodName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public string CustomerOrderNumber { get; set; }
        public bool EnableTariffPrepay { get; set; }
        public int Status { get; set; }
        public ShippingInfoImportModel ShippingInfo { get; set; }
        public SenderInfoImportModel SenderInfo { get; set; }
        private List<ApplicationInfoImportModel> _applicationInfos;

        public List<ApplicationInfoImportModel> ApplicationInfos
        {
            get { return _applicationInfos ?? (_applicationInfos = new List<ApplicationInfoImportModel>()); }
            set { _applicationInfos = value; }
        }

        public CustomerOrderInfoImportModel CustomerOrderInfo { get; set; }
    }
    public class ShippingInfoImportModel
    {
        /// <summary>
        /// 收件人税号
        /// </summary>

        public string ShippingTaxId { get; set; }

        /// <summary>
        /// 收件人国家简码
        /// </summary>
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
    }
    public class SenderInfoImportModel
    {
        public string CountryCode { get; set; }

        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderCompany { get; set; }
        public string SenderAddress { get; set; }
        public string SenderCity { get; set; }
        public string SenderState { get; set; }
        public string SenderZip { get; set; }
        public string SenderPhone { get; set; }
    }
    public class ApplicationInfoImportModel
    {
        public string ApplicationName { get; set; }
        public string HSCode { get; set; }
        public int? Qty { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? UnitWeight { get; set; }

        /// <summary>
        /// 配货名称
        /// </summary>
        public string PickingName { get; set; }

        public string Remark { get; set; }

        public string ProductUrl { get; set; }
    }
    public class CustomerOrderInfoImportModel
    {
        public string CustomerOrderNumber { get; set; }
        public string CustomerCode { get; set; }
        public string TrackingNumber { get; set; }
        public int? ShippingMethodId { get; set; }
        public string ShippingMethodName { get; set; }
        public int? GoodsTypeID { get; set; }
        public bool IsReturn { get; set; }
        public bool IsInsured { get; set; }
        public int? InsuredID { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public int? SensitiveTypeID { get; set; }
        public int? PackageNumber { get; set; }
        public decimal? InsureAmount { get; set; }
        public int AppLicationType { get; set; }
        public decimal Weight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public bool EnableTariffPrepay { get; set; }
        public string Remark { get; set; }
        //客户订单状态表备注
        public string StatusRemark { get; set; }
    }
}
