using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LMS.Client.B2CTracking.Model
{
    [Serializable]
    public class Parcel
    {
        [XmlElement("KEYCODE")]
        public string KeyCode { get; set; }
        [XmlElement("ABCCODE")]
        public string AbcCode { get; set; }
        [XmlElement("SUPPLIERCODE")]
        public string SupplierCode { get; set; }
        [XmlElement("PRODUCTNAME")]
        public string ProductName { get; set; }
        [XmlElement("WEIGHT")]
        public int Weight { get; set; }
        [XmlElement("SUPPLIER")]
        public string Supplier { get; set; }
        [XmlElement("EMAIL")]
        public string Email { get; set; }
        [XmlElement("ORDERID")]
        public int OrderId { get; set; }
        [XmlElement("CODAMOUNT")]
        public decimal CodAmount { get; set; }
        [XmlElement("PRICE")]
        public decimal Price { get; set; }
        [XmlElement("TRACKYOURPARCEL")]
        public string TrackYourParcel { get; set; }
        [XmlElement("status")]
        public List<Status> Status { get; set; }
    }
    [Serializable]
    public class TableModel
    {
        [XmlElement("parcel")]
        public List<Parcel> Parcel { get; set; }
    }
    [Serializable, XmlRoot("tracking")]
    public class Tracking
    {
        [XmlElement("table")]
        public TableModel Table { get; set; }
    }
}
