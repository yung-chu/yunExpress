using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{
    public class B2CPreAlterListParam : SearchParam
    {
        public B2CPreAlterListParam()
        {
            ShippingMethodIds=new List<int>();
        }
        public int ShippingMethodId { get; set; }
        public string ShippingName { get; set; }
        public string CustomerCode { get; set; }
        public string NickName { get; set; }
        public string CountryCode { get; set; }
        public decimal? StartWeight { get; set; }
        public decimal? EndWeight { get; set; }
        public int SearchWhere { get; set; }
        public string SearchContext { get; set; }
        public int SearchTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime OutStartTime { get; set; }
        public int? Status { get; set; }
        public List<int> ShippingMethodIds { get; set; } 
        public bool IsSelectAll { get; set; }
    }
}
