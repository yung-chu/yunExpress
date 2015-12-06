using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.GetLabelPrint.WebAPI.Models
{
    public class LabelPrintRequest
    {
        public LabelPrintRequest()
        {
            OrderNumber = new List<string>();
        }
        public List<string> OrderNumber { get; set; }

        public string TypeId { get; set; }
    }
}