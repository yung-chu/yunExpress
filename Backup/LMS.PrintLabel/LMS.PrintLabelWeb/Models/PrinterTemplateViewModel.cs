using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.PrintLabelWeb.Models
{
    public class PrinterTemplateViewModel
    {
        public PrinterTemplateViewModel()
        {
            CustomerOrderInfoModels = new List<CustomerOrderInfoModel>();
        }

        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public string TemplateHead { get; set; }
        public string TemplateBodyContent { get; set; }
        public List<CustomerOrderInfoModel> CustomerOrderInfoModels { get; set; }
    }
}