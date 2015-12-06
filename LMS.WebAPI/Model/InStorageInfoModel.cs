using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.WebAPI.Model
{
    public class InStorageInfoModel
    {
        public string InStorageID { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public string PaymentName { get; set; }
        public decimal TotalFee { get; set; }
        public decimal Freight { get; set; }
        public decimal Register { get; set; }
        public decimal FuelCharge { get; set; }
        public decimal TariffPrepayFee { get; set; }
        public decimal Surcharge { get; set; }
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
        public decimal WaybillNumberTotalFee { get; set; }
        public string CustomerOrderNumber { get; set; }
    }
}