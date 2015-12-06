using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.UserCenter.Controllers.BillingController.Models
{
    public class CustomerBalances
    {
        public string CustomerCode { get; set; }
        public decimal Balance { get; set; }
    }
}