using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class ExpressWayBillViewExt:LighTake.Infrastructure.Seedwork.Entity
    {
        public ExpressWayBillViewExt()
        {
            wayBillDetails = new List<WayBillDetailExt>();
        }
        public string WayBillNumber { get; set; }
        public virtual string CustomerOrderNumber { get; set; }
        public string CustomerCode { get; set; }
        public string TrackingNumber { get; set; }
        public int InShippingMethodID { get; set; }
        public string InShippingMethodName { get; set; }
        public int Status { get; set; }
        public decimal Weight { get; set; }
        public decimal SettleWeight { get; set; }
        public string OutStorageID { get; set; }
        public DateTime? OutStorageTime { get; set; }
        public string CountryCode { get; set; }

        public List<WayBillDetailExt> wayBillDetails { get; set; }
    }

    public class WayBillDetailExt
    {
        public int PackageDetailID { get; set; }
        public string WayBillNumber { get; set; }
        public decimal Weight { get; set; }
        public decimal SettleWeight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal AddWeight { get; set; }
    }
}
