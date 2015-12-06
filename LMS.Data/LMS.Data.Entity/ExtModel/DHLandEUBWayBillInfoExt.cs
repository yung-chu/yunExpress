using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity.ExtModel
{
    public class DHLandEUBWayBillInfoExt
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
        public int? InShippingMethodId { get; set; }

        public int? SenderInfoID { get; set; }
        public int? ShippingInfoID { get; set; }
        public SenderInfoModelExt SenderInfo { get; set; }
        public ShippingInfoModelExt ShippingInfo { get; set; }
        public List<ApplicationInfoModelExt> ApplicationInfoModels { get; set; }
    }

    public class ApplicationInfoModelExt
    {
        public string WayBillNumber { get; set; }
        public int ApplicationID { get; set; }
        public string ApplicationName { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<decimal> UnitWeight { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public string PickingName { get; set; }
        public string Remark { get; set; }
    }
}
