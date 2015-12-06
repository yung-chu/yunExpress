using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.UserCenter.Controllers.BillingController.Models
{
    public class RechargeModel
    {
        public string CustomerCode { get; set; }
        public int RechargeType { get; set; }
        public decimal Amount { get; set; }
        public string TransactionNo { get; set; }
        public string VoucherPath { get; set; }
        public string Remark { get; set; }
    }
}