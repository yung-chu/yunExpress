using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Controllers.FinancialController
{
    public class AuditAnomalyViewModel
    {
        public string WayBillNumber { get; set; }
        public string OldFinancialNote { get; set; }
        public string NewFinancialNote { get; set; }
    }
}