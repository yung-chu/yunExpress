using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class CreateOutStorageExt
    {
        public CreateOutStorageExt()
        {
            OutStorage = new OutStorageInfo();
            WayBillInfos = new List<OutWayBillInfoExt>();
        }
        public OutStorageInfo OutStorage { get; set; }
        public List<OutWayBillInfoExt> WayBillInfos { get; set; }
        public string TotalPackageNumber { get; set; }
        //总票数
        public int TotalVotes { get; set; }
        //总件数
        public int TotalQty { get; set; }
        //总重量
        public decimal TotalWeight { get; set; }
        public bool? IsCreateTotalPackageNumber { get; set; }
        public string Remark { get; set; }
    }
    public class OutWayBillInfoExt
    {
        public string WayBillNumber { get; set; }
        public string TrackingNumber { get; set; }
        public decimal SettleWeight { get; set; }
        public decimal Weight { get; set; }
        public decimal Freight { get; set; }
        public decimal FuelCharge { get; set; }
        public decimal Register { get; set; }
        public decimal Surcharge { get; set; }
        public int OutShippingMethodID { get; set; }
        public string OutShippingMethodName { get; set; }
        public int GoodsTypeID { get; set; }
        public string CountryCode { get; set; }
        public bool IsBattery{ get; set; }
        public bool HaveTrackingNum { get; set; }
    }
}
