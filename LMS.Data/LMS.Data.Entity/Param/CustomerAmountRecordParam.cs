using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{
    public class CustomerAmountRecordParam
    {
        public string CustomerCode { get; set; }
        public string WayBillNumber { get; set; }
        public string TransactionNo { get; set; }
        public int MoneyChangeTypeId { get; set; }
        public int FeeTypeId { get; set; }
        public decimal Amount { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
    }
    public class AmountRecordSearchParam : SearchParam
    {
        public string CustomerCode { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
    }
}
