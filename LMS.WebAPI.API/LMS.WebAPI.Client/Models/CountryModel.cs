using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace LMS.WebAPI.Client.Models
{
    public class CountryModel
    {
        public string CountryCode { get; set; }
        public string EName { get; set; }
        public string CName { get; set; }
    }
}