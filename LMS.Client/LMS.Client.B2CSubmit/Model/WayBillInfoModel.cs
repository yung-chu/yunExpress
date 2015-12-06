using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Client.B2CSubmit.Model
{
    public class WayBillInfoModel
    {
        public WayBillInfoModel()
        {
            ApplicationInfos=new List<ApplicationInfoModel>();
        }
        public string WayBillNumber { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string PreAlertBatchNo { get; set; }
        public string ShippingMethod { get; set; }
        public string CompanyName { get; set; }
        public string ConsigneeName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Street { get; set; }
        public string AdditionalAddressInfo { get; set; }
        public int HouseNumber { get; set; }
        public string HouseNumberExtension { get; set; }
        public string CityOrTown { get; set; }
        public string StateOrProvince { get; set; }
        public string ZIPCode { get; set; }
        public string CountryCode { get; set; }
        public decimal Weight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public int ShippingMethodID { get; set; }
        public List<ApplicationInfoModel> ApplicationInfos { get; set; }
    }
    public class ApplicationInfoModel
    {
        public int PackageNumber { get; set; }
        public string SKUCode { get; set; }
        public string SKUDescription { get; set; }
        public string HSCode { get; set; }
        public int PackageWeight { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }
}
