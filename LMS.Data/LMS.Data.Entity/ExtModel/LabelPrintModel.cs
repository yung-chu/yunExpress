using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class LabelPrintModel
    {
        //客户订单号
        public string OrderNumber { get; set; }
        /// <summary>
        /// 运输方式ID
        /// </summary>
        public int ShippingMethodId { get; set; }
        /// <summary>
        /// 是否可以打印
        /// </summary>
        public bool IsHavePrint { get; set; }
    }
}
