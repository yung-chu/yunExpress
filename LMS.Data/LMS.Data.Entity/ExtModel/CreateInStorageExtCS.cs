using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity.ExtModel
{
    public class CreateInStorageExtCS
    {
        public CreateInStorageExtCS()
        {
            InStorage = new InStorageInfoExtSlim();
            WayBillInfos = new List<WayBillInfoExt>();
        }
        public InStorageInfoExtSlim InStorage { get; set; }
        public List<WayBillInfoExt> WayBillInfos { get; set; }
    }

    public class WayBillInfoExt
    {
        public WayBillInfoExt()
        {
            WaybillPackageDetailList = new List<WaybillPackageDetailExt>();
        }
        public int CustomerType { get; set; }
        public string CustomerCode { get; set; }
        public int GoodsTypeID { get; set; }
        public int ShippingMethodId { get; set; }
        public string WayBillNumber { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string TrackingNumber { get; set; }
        public decimal SettleWeight { get; set; }
        public decimal Weight { get; set; }
        public bool IsBusinessExpress { get; set; }
        public bool IsBattery { get; set; }
        public int? SensitiveType{ get; set; }
        public List<WaybillPackageDetailExt> WaybillPackageDetailList { get; set; }

        public PriceProviderResult PriceResult { get; set; }
    }

    public class WaybillPackageDetailExt
    {
        public string WayBillNumber { get; set; }
        public decimal Weight { get; set; }
        public decimal AddWeight { get; set; }
        public decimal SettleWeight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal LengthFee { get; set; }
        public decimal WeightFee { get; set; }
    }
}