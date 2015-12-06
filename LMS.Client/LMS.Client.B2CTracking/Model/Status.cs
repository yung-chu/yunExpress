using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LMS.Client.B2CTracking.Model
{
    public class Status
    {
        [XmlElement("STATUSID")]
        public int StatusId { get; set; }
        [XmlElement("PODSTATUS")]
        public string PodStatus { get; set; }
        [XmlElement("STATUSDATE")]
        public string StatusDate { get; set; }
        [XmlElement("COMMENT")]
        public string Comment { get; set; }
    }
}
