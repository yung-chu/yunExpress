using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace LMS.PrintLabelAPI.Models
{
    [XmlRoot("Response")]
    public class Response<T> where T : new()
    {
        public string ResultCode { get; set; }
        public string ResultDesc { get; set; }
        public T Item { get; set; }
    }
}