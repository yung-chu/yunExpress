using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.UserCenter.Controllers.BillingController.Models
{
    public class CountryModel
    {
        public string CountryCode { get; set; }
        public string Name { get; set; }
        public bool AllowsShipping { get; set; }
        public string ThreeLetterISOCode { get; set; }
        public int NumericISOCode { get; set; }
        public byte Status { get; set; }
        public int DisplayOrder { get; set; }
        public string ChineseName { get; set; }
    }
}
