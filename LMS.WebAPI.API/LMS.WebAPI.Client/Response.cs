using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace LMS.WebAPI.Client
{
    [XmlRoot("Response")]
    public class Response<T> where T : new()
    {
        public string ResultCode { get; set; }
        public string ResultDesc { get; set; }
        public T Item { get; set; }
    }
    public class Item
    {

    }
}