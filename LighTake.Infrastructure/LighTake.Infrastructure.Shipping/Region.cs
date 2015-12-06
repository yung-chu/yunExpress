using System;
using System.Collections.Generic;
using System.Text;

namespace LighTake.Infrastructure.Shipping
{
    public class Region
    {
        public List<Country> ListCountry { get; set;}

        public int ShippingMethodID { get; set; }

        public int RegionID { get; set; }

        public string RegionName { get; set; }

        public decimal FirstWeight { get; set; }

        public decimal AdditionalWeight { get; set; }
    }
}
