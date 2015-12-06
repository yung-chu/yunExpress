using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity.ExtModel
{
    public class FZWayBillInfoExt : LighTake.Infrastructure.Seedwork.Entity
    {
        public string WayBillNumber { get; set; }
        public string CustomerOrderNumber { get; set; }
        public int? CustomerOrderID { get; set; }
        public string TrackingNumber { get; set; }

        public decimal? Weight { get; set; }
        public decimal? SettleWeight { get; set; }
        public bool IsReturn { get; set; }
        public bool IsBattery { get; set; }
        public int Status { get; set; }
        public string OutStorageID { get; set; }
        public DateTime? OutStorageCreatedOn { get; set; }
        public string CountryCode { get; set; }
        public int? InsuredID { get; set; }
        public int? OutShippingMethodID { get; set; }

        public int? SenderInfoID { get; set; }
        public int? ShippingInfoID { get; set; }
        public SenderInfoModelExt SenderInfo { get; set; }
        public ShippingInfoModelExt ShippingInfo { get; set; }
    }

    public class FZWayBillNumbers 
    {
        public string WayBillNumber { get; set; } 
    }

    public class ShippingInfoModelExt
    {
        public int? ShippingInfoID { get; set; }
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
    }

    public class SenderInfoModelExt
    {
        public int? SenderInfoID { get; set; }
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
}
