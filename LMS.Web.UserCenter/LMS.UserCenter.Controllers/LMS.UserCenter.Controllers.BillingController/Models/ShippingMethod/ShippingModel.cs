using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.UserCenter.Controllers.BillingController.Models
{
    public class ShippingModel
    {
        public int ShippingMethodId { get; set; }
        public string ShippingMethodName { get; set; }
        public bool WeightOrVolume { get; set; }
    }
}