using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    /// <summary>
    /// 查询运单汇总报表条件参数
    /// </summary>
    public class WaybillSummaryParam
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string VenderCode { get; set; }
        public string VenderName { get; set; }
        public string ShippingName { get; set; }
        public int SelectShippingMethod { get; set; }
        public int? ShippingMethodId { get; set; }
        public string Status { get; set; }
        public int SelectTimeName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
