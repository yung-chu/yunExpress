using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Controllers.OrderController
{
    public class ProductDetailModel
    {
        public string ApplicationName { get; set; }
        public string HSCode { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? UnitWeight { get; set; }
        public string PickingName { get; set; }
        public string Remark { get; set; }
        public string ProductUrl { get; set; }
    }
    
}