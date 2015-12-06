using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class NoForecastAbnormalExt : NoForecastAbnormal
    {
        public int Status { get; set; }
        public string Description { get; set; }
        public string CustomerName { get; set; }
        public string ShippingMethodName { get; set; }
        public IQueryable<WayBillInfo> WayBillInfos { get; set; }
        public string StatusStr { get { return WayBill.GetNoForecastAbnormalDescription(Status); }}
    }
}
