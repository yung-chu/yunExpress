using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.UserCenter.Controllers.BillingController.Models
{
    public class BillingExecModel
    {
        public string SerialNumber { get; set; }
        public string CustomerCode { get; set; }
        public string MoneyChangeTypeShortName { get; set; }
        public decimal InCash { get; set; }
        public decimal OutCash { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string Remark { get; set; }
    }
}