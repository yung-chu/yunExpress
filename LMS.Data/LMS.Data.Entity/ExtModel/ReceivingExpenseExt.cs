using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class ReceivingExpenseExt : LighTake.Infrastructure.Seedwork.Entity
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ShippingMethodName { get; set; }
        public string WayBillNumber { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string TrackingNumber { get; set; }
        public string CountryCode { get; set; }
        public decimal Weight { get; set; }
        public decimal SettleWeight { get; set; }

        public decimal? Freight { get; set; }
        public decimal? Register { get; set; }
        public decimal? FuelCharge { get; set; }
        public decimal? TariffPrepayFee { get; set; }
        public decimal? Surcharge { get; set; }
        public decimal? RemoteAreaFee { get; set; }
        public decimal? TotalFee{ get; set; }

        public string FinancialNote { get; set; }
    }

    public class ReceivingExpensesEditExt : LighTake.Infrastructure.Seedwork.Entity
    {
        public string CustomerOrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string FinancialNote { get; set; }
        public string WayBillNumber { get; set; }

        public decimal? FreightOriginal { get; set; }
        public decimal? RegisterOriginal { get; set; }
        public decimal? FuelChargeOriginal { get; set; }
        public decimal? TariffPrepayFeeOriginal { get; set; }
        public decimal? SurchargeOriginal { get; set; }
        public decimal? RemoteAreaFeeOriginal { get; set; }
        public decimal? TotalFeeOriginal { get; set; }


        public decimal? FreightFinal { get; set; }
        public decimal? RegisterFinal { get; set; }
        public decimal? FuelChargeFinal { get; set; }
        public decimal? TariffPrepayFeeFinal { get; set; }
        public decimal? SurchargeFinal { get; set; }
        public decimal? RemoteAreaFeeFinal { get; set; }
        public decimal? TotalFeeFinal { get; set; }
    }
}
