using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Data.Entity;

namespace LMS.Controllers.WayBillController
{
    public class InvoivePrinterViewModel
    {
        public InvoivePrinterViewModel()
        {
            SelectList = new List<SelectListItem>();
            CustomerOrderInfoModels = new List<InvoivePrinterOrderInfoModel>();
            WayBillTemplates=new List<WayBillTemplateExt>();
        }
        public string Ids { get; set; }
        public string TemplateName { get; set; }
        public List<SelectListItem> SelectList { get; set; }
        public List<InvoivePrinterOrderInfoModel> CustomerOrderInfoModels { get; set; }
        public IEnumerable<WayBillTemplateExt> WayBillTemplates { get; set; }
    }
    public class PrinterTemplateViewModel
    {
        public PrinterTemplateViewModel()
        {
            CustomerOrderInfoModels = new List<InvoivePrinterOrderInfoModel>();
        }

        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public string TemplateHead { get; set; }
        public string TemplateBodyContent { get; set; }
        public List<InvoivePrinterOrderInfoModel> CustomerOrderInfoModels { get; set; }
    }

    public class PrinterTemplateViewModelCommon
    {
        public PrinterTemplateViewModelCommon()
        {
            CustomerOrderInfoModels = new List<CustomerOrderInfoModelCommon>();
        }

        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public string TemplateHead { get; set; }
        public string TemplateBodyContent { get; set; }
        public List<CustomerOrderInfoModelCommon> CustomerOrderInfoModels { get; set; }
    }
}