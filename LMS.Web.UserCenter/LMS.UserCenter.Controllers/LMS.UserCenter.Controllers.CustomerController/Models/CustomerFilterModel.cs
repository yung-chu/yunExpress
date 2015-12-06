using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LighTake.Infrastructure.Web.Models;

namespace LMS.UserCenter.Controllers.CustomerController.Models
{
    public class CustomerFilterModel
    {
        public string CustomerCode { get; set; }
        public bool IsAll { get; set; }
    }
    public class CustomerListFilterModel
    {
        public string CustomerCode { get; set; }
        public int? Status { get; set; }
    }
    public class CustomerRechargeListFilterModel : SearchFilter
    {
        public string CustomerCode { get; set; }
        public int? Status { get; set; }
    }
    public class CustomerAmountRecordListFilterModel : SearchFilter
    {
        public string CustomerCode { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
    }
}