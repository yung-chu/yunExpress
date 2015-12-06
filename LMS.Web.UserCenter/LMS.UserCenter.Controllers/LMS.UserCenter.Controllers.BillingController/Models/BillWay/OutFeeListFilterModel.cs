using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LighTake.Infrastructure.Web.Models;

namespace LMS.UserCenter.Controllers.BillingController.Models
{
    public class OutStorageFilterModel : SearchFilter
    {
        public string VenderCode { get; set; }
        public string VenderName { get; set; }
        public int? ShippingMethodId { get; set; }
        public string ShippingName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string CountryCode { get; set; }
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
    }
}