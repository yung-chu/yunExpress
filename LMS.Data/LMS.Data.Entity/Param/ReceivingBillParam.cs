using LighTake.Infrastructure.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity.Param
{
    public class ReceivingBillParam : SearchParam
    {
        public string CustomerCode { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public string ReceivingBillAuditor { get; set; }
        public string ReceivingBillID { get; set; }

        public int? Status { get; set; }
    }
}
