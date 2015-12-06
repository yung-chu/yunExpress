using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{
    public class CountryParam : SearchParam
    {
         public string CountryCode { get; set; }

        public string Name { get; set; }

        public string ChineseName { get; set; }

        public bool? AllowsShipping { get; set; }
    }
}
