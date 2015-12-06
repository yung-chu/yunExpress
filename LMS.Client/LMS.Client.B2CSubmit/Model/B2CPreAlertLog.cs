using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Client.B2CSubmit.Model
{
    public class B2CPreAlertLog
    {
        public string WayBillNumber { get; set; }
        public string ShippingMethod { get; set; }
        public int PreAlertID { get; set; }
        public string ErrorMsg { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorDetails { get; set; }
        public int Status { get; set; }
        public string PreAlertBatchNo { get; set; }
    }
}
