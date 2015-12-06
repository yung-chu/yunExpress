using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{
    public class ReturnWayBillParam : SearchParam
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public DateTime? ReturnStartTime { get; set; }
        public DateTime? ReturnEndTime { get; set; }
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
        public string CreateBy { get; set; }
        public int InShippingMehtodId { get; set; }
        public string ReturnReason { get; set; }
        public bool? IsReturnShipping { get; set; }
        public int? Status { get; set; }
    }
}
