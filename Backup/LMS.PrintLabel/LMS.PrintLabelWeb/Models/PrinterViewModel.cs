using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Data.Entity;

namespace LMS.PrintLabelWeb.Models
{
    public class PrinterViewModel
    {
        public PrinterViewModel()
        {
            SelectList = new List<SelectListItem>();
            CustomerOrderInfoModels = new List<CustomerOrderInfoModel>();
            WayBillTemplates = new List<WayBillTemplateExt>();
        }

        public int Type { get; set; }
        public string Ids { get; set; }
        public string TypeId { get; set; }
        public string TemplateName { get; set; }
        public List<SelectListItem> SelectList { get; set; }
        public List<CustomerOrderInfoModel> CustomerOrderInfoModels { get; set; }
        public IEnumerable<WayBillTemplateExt> WayBillTemplates { get; set; }
    }
}