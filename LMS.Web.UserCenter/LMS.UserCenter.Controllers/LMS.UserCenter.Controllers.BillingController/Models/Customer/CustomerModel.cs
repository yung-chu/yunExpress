using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.UserCenter.Controllers.BillingController.Models
{
    public class CustomerModel
    {
        public string CustomerCode { get; set; }
        public string AccountID { get; set; }
        public string AccountPassWord { get; set; }
        public string NewPassWord { get; set; }
        public string SureNewPassWord { get; set; }
    }
}