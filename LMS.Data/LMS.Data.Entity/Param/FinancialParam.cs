using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{
    public class FinancialParam : SearchParam
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ShippingMethodName { get; set; }
        public int? ShippingMethodId { get; set; }

        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
        public int? Status { get; set; }
    }
    public class AuditParam
    {
        public int DeliveryFeeId { get; set; }
        public decimal OtherFee { get; set; }
        public string OtherRemark { get; set; }
        public string ErrorRemark { get; set; }
    }
}
