using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class ExpressWayBillExt:LighTake.Infrastructure.Seedwork.Entity
    {
        public string WayBillNumber { get; set; }
        public virtual string CustomerOrderNumber { get; set; }
        public string CustomerCode { get; set; }
        public string TrackingNumber { get; set; }
        public int InShippingMethodID { get; set; }
        public string InShippingMethodName { get; set; }
        public int Status { get; set; }
        public decimal WayBillWeight { get; set; }
        public decimal WayBillSettleWeight { get; set; }
        public DateTime? OutStorageTime { get; set; }
        public string CountryCode { get; set; }

        public int PackageDetailID { get; set; }
        public decimal DetailWeight { get; set; }
        public decimal DetailSettleWeight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal AddWeight { get; set; }
    }
}
