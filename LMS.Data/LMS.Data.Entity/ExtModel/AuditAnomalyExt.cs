using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class AuditAnomalyExt
    {
        public string WayBillNumber { get; set; }
        public string OldFinancialNote { get; set; }
        public string NewFinancialNote { get; set; }
    }
}
