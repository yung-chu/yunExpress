using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LMS.Services.B2CServices.Model
{
    [XmlRoot("Prealert")]
    public class OkResponses
    {
        public string Result { get; set; }
        public long PreAlertID { get; set; }
        public int NumShipmentsSaved { get; set; }
    }
    [XmlRoot("Error")]
    public class ErrorResponses
    {
        public string Message { get; set; }
        public int Code { get; set; }
        public string Details { get; set; }
    }
}
