using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.WebAPI.Model
{
    public class InStorageWayBillModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string WayBillNumber { get; set; }
        public string TrackingNumber { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public decimal SettleWeight { get; set; }
        public string CustomerOrderNumber { get; set; }
        public decimal Freight { get; set; }
        public decimal Register { get; set; }
        public decimal FuelCharge { get; set; }
        public decimal Surcharge { get; set; }
        public string HtmlString { get; set; }
        public bool EnableTariffPrepay { get; set; }
    }
}