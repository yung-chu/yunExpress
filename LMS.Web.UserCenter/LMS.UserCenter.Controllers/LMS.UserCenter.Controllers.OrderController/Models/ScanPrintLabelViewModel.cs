using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMS.UserCenter.Controllers.OrderController.Models
{
    public class ScanPrintLabelViewModel
    {
        public ScanPrintLabelViewModel()
        {
            PrintTemplate = new List<SelectListItem>();
            Filter=new ScanPrintLabelFilter();
            OrderInfoModel=new CustomerOrderInfoModel();
        }
        public List<SelectListItem> PrintTemplate { get; set; }
        public ScanPrintLabelFilter Filter { get; set; }
        public CustomerOrderInfoModel OrderInfoModel { get; set; }

    }

    public class ScanPrintLabelFilter
    {
        public ScanPrintLabelFilter()
        {
            TemplateTypeId = "DT1308100021";
        }

        public string TemplateTypeId { get; set; }
        public string TemplateName { get; set; }
        public bool IsAutoPrint { get; set; }
        public string OrderNumber { get; set; }
        public string TemplateContent { get; set; }
    }
}