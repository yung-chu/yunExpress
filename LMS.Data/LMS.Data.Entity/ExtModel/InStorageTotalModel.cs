using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class InStorageTotalModel
    {
        //public string ShippingMethodName { get; set; }
        public string CountryCode { get; set; }
        public decimal? PhysicalTotalWeight { get; set; }
        public decimal? TotalWeight { get; set; }
        public int PackCount { get; set; }
    }
}
