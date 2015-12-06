using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LighTake.Infrastructure.Web.Models;

namespace LMS.Controllers.OrderController
{
    public class OrderFilterModel
    {
        public string NickName { get; set; }
        public string CustomerCode { get; set; }
        public string FilePath { get; set; }
    }

}