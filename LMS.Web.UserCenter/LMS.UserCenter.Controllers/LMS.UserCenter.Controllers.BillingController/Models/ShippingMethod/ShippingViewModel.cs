using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LighTake.Infrastructure.Common;

namespace LMS.UserCenter.Controllers.BillingController.Models
{
    public class ShippingViewModel
    {
        public ShippingViewModel()
        {
            ShippingMethodList = new List<ShippingModel>();
        }
        public List<ShippingModel> ShippingMethodList { get; set; }
        public int? CustomerTypeId { get; set; }
        public string VenderCode { get; set; }
        public int SelectType { get; set; }
    }
}