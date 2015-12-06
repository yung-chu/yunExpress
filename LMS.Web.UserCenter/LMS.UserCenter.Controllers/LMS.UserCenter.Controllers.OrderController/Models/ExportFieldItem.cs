using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.UserCenter.Controllers.OrderController.Models
{

    public class ExportFieldItem
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public bool Select { get; set; }
        public string GroupName { get; set; }
    }
}