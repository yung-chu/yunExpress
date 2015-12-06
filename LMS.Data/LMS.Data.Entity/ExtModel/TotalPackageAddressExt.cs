using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LMS.Data.Entity
{
    [Serializable]
    [XmlRoot("Address")]
    public class TotalPackageAddressExt
    {
        [XmlElement("EventCode")]
        public List<TotalPackageEventCode> TraceEventCode { get; set; }
    }
    [Serializable]
    public class TotalPackageEventCode
    {
        [XmlAttribute("Value")] 
        public int EventCode { get; set; }
        [XmlElement("Item")]
        public List<string> Address { get; set; }
    }
}
