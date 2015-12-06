using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity.ExtModel
{
    public class PrintInStorageInvoiceExt:LighTake.Infrastructure.Seedwork.Entity
    {
        public string InStorageID { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public string PaymentName { get; set; }
        public int TotalQty { get; set; }
        public int Pieces { get; set; }
        public decimal? TotalWeight { get; set; }
        public decimal PhysicalTotalWeight { get; set; }
        public string InShippingMethodName { get; set; }
        public string CountryCode { get; set; }
        public string ChineseName { get; set; }
        public decimal? SettleWeight { get; set; }
        public decimal? Weight { get; set; }
        public string ReceivingClerk { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CustomerOrderNumber { get; set; }
    }
}
