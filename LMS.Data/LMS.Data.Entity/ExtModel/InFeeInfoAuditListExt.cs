using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class InFeeInfoAuditListExt:LighTake.Infrastructure.Seedwork.Entity
    {
        //收货
        public string WayBillNumber { get; set; }
        //public string ReceivingBillID { get; set; }
        public string CustomerOrderNumber { get; set; }
        public int Status { get; set; }

        //[NotMapped]
        //public string StatusName { get; set; }

        public string Auditor { get; set; }
        public DateTime? AuditorDate { get; set; }
        public DateTime OutDateTime { get; set; }
        public int? OperationType { get; set; }

        //运单信息
        public string TrackingNumber { get; set; }
        public DateTime? InStorageCreatedOn { get; set; }
        public int? InShippingMethodID { get; set; }
        public string InShippingMethodName { get; set; }
        public string CountryCode { get; set; }
        public decimal? Weight { get; set; }
        public decimal? SettleWeight { get; set; }

        public string CustomerName { get; set; }

        public string CustomerCode { get; set; }

        //费用
        public decimal? Freight { get; set; }
        public decimal? Register { get; set; }
        public decimal? FuelCharge { get; set; }
        public decimal? Surcharge { get; set; }
        public decimal? TariffPrepayFee { get; set; }

        public decimal? RemoteAreaFee { get; set; }
        //[NotMapped]
        //public decimal TotalFee { get; set; }

        public decimal? SpecialFee { get; set; }
    }
}
