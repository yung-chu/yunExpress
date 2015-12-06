using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LMS.Services.B2CServices.Model
{
    [XmlRoot("Prealert")]
    public class Prealert
    {
        public Prealert()
        {
            Shipments=new List<Shipment>();
        }
        public string AuthenticationKey { get; set; }
        public string LayoutType { get; set; }
        public string LayoutVersion { get; set; }
        public string LayoutPlatform { get; set; }
        public string PrealertReference { get; set; }
        [XmlElement("Shipment")]
        public List<Shipment> Shipments { get; set; }

        public PrealertValidation PrealertValidation { get; set; }
    }
    public class Shipment
    {
        public Shipment()
        {
            ShipmentAddress=new ShipmentAddress();
            ShipmentPackages = new List<ShipmentPackage>();
            ShipmentContentCustoms=new List<ShipmentContentCustoms>();
            ShipmentContact=new ShipmentContact();
        }
        public string OrderNumber { get; set; }
        public string OrderReference { get; set; }
        public string OrderContent { get; set; }
        public string ShippingMethod { get; set; }
        public string CountryCodeOrigin { get; set; }
        public string PurchaseDate { get; set; }
        public string Currency { get; set; }
        [XmlIgnore]
        public decimal ShippingCosts { get; set; }
        [XmlIgnore]
        public decimal HandlingCosts { get; set; }
        public string CustomsService { get; set; }
        public ShipmentAddress ShipmentAddress { get; set; }
        [XmlElement("ShipmentPackage")]
        public List<ShipmentPackage> ShipmentPackages { get; set; }
        [XmlElement("ShipmentContentCustoms")]
        public List<ShipmentContentCustoms> ShipmentContentCustoms { get; set; }
        public ShipmentContact ShipmentContact { get; set; }
    }
    public class ShipmentAddress
    {
        public string AddressType { get; set; }
        public string ConsigneeName { get; set; }
        public string CompanyName { get; set; }
        public string Street { get; set; }
        public string AdditionalAddressInfo { get; set; }
        public int HouseNumber { get; set; }
        public string HouseNumberExtension { get; set; }
        public string CityOrTown { get; set; }
        public string StateOrProvince { get; set; }
        public string ZIPCode { get; set; }
        public string CountryCode { get; set; }
    }
    public class ShipmentPackage
    {
        public int PackageNumber { get; set; }
        public string PackageBarcode { get; set; }
        public int PackageWeight { get; set; }
        public int DimensionHeight { get; set; }
        public int DimensionWidth { get; set; }
        public int DimensionLength { get; set; }
    }
    public class  ShipmentContentCustoms
    {
        public int PackageNumber { get; set; }
        public string SKUCode { get; set; }
        public string SKUDescription { get; set; }
        public string HSCode { get; set; }
        public string TariCode { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
    }
    public class ShipmentContact
    {
        public string PhoneNumber { get; set; }
        public string SMSNumber { get; set; }
        public string EmailAddress { get; set; }
        public string PersonalNumber { get; set; }
    }
    public class PrealertValidation
    {
        public int TotalShipments { get; set; }
        public string MailAddressConfirmation { get; set; }
        public string MailAddressError { get; set; }
        public string Timezone { get; set; }
    }
}
