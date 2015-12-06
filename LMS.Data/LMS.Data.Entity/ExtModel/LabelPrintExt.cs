using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class LabelPrintExt
    {
        //运单号
        public string WayBillNumber { get; set; }
        //跟踪号
        public string Trackingnumber { get; set; }
        //客户订单号
        public string CustomerOrderNumber { get; set; }
        //客户订单ID
        public int CustomerOrderId { get; set; }
    }
}
