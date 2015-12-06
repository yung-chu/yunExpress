using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Client.CreateOutBill.Model
{
    public class ReceivingBillInfo
    {
        public string WayBillNumber { get; set; }
        public string ReceivingBillID { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public DateTime ReceivingDate { get; set; }
        public string TrackingNumber { get; set; }
        public string CountryCode { get; set; }
        public string ShippingMethodName { get; set; }
        public decimal SettleWeight { get; set; }
        public decimal Weight { get; set; }
        public int PackageNumber { get; set; }
        public string FeeDetail { get; set; }
        public decimal TotalFee { get; set; }
    }
    public class ReceivingBill
    {
        public string ReceivingBillID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ReceivingBillDate { get; set; }
        public string ReceivingBillAuditor { get; set; }
        public string BillStartTime { get; set; }
        public string BillEndTime { get; set; }
        public int Search { get; set; }
    }
}
