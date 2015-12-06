using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class CustomerAmountRecordExt : LighTake.Infrastructure.Seedwork.Entity
    {
        public string SerialNumber { get; set; }
        public string CustomerCode { get; set; }
        public string TransactionNo { get; set; }
        public Nullable<int> MoneyChangeTypeID { get; set; }
        public Nullable<int> FeeTypeID { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string Remark { get; set; }
        public string MoneyChangeTypeShortName { get; set; }
    }
}
