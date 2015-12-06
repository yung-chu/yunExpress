using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace LMS.WebAPI.Client.Models
{
    public class ShippingMethodModel
    {
        public string Code { get; set; }
        public string FullName { get; set; }
        public string EnglishName { get; set; }
        public bool HaveTrackingNum { get; set; }
        public string DisplayName { get; set; }
    }
}