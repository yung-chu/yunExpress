using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.WinForm.Client.Models
{
    public class InStorageModel
    {
        public InStorageModel()
        {
            WayBillInfos = new List<WayBillInfoModel>();
        }
        public string InStorageID { get; set; }
        public string ReceivingClerk { get; set; }
        public string CustomerCode { get; set; }
        public Nullable<decimal> TotalWeight { get; set; }
        public Nullable<int> TotalQty { get; set; }
        public Nullable<decimal> MaterialsFee { get; set; }
        public Nullable<decimal> TotalFee { get; set; }
        public int Status { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public string Remark { get; set; }
        public Nullable<decimal> Freight { get; set; }
        public Nullable<decimal> Register { get; set; }
        public Nullable<decimal> FuelCharge { get; set; }
        public Nullable<decimal> Surcharge { get; set; }
        public string ShippingMethodName { get; set; }
        public List<WayBillInfoModel> WayBillInfos { get; set; }
    }
}
