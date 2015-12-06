using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class ShippingWayBillExt : LighTake.Infrastructure.Seedwork.Entity
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
}