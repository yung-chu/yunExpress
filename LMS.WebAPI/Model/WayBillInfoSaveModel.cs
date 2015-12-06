using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LMS.Data.Entity;

namespace LMS.WebAPI.Model
{
    public class WayBillInfoSaveModel
    {
        public WayBillInfoSaveModel()
        {
            waybillPackageDetailList = new List<WaybillPackageDetailModel>();
        }

        public List<WaybillPackageDetailModel> waybillPackageDetailList { get; set; }

        public string WayBillNumber { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string TrackingNumber { get; set; }
        public decimal SettleWeight { get; set; }
        public decimal Weight { get; set; }
        public string OldTrackingNumber { get; set; }
        public int ShippingMethodId { get; set; }
        public bool IsBusinessExpress { get; set; }
        public int GoodsTypeID { get; set; }
        public bool IsBattery { get; set; }
        public int? SensitiveType { get; set; }
        public PriceProviderResult PriceResult { get; set; }
    }
}