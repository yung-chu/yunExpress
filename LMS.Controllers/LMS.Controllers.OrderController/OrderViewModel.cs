using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Controllers.OrderController
{
    public class OrderViewModel
    {
        public OrderViewModel()
        {
            OrderModels = new List<OrderModel>();
        }

        public OrderFilterModel FilterModel { get; set; }

        public List<OrderModel> OrderModels { get; set; }

        public int ProductColumnsCount { get; set; }

        public bool? Error { get; set; }

       
    }
}