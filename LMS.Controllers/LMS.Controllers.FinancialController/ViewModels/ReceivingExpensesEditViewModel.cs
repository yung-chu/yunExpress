using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LighTake.Infrastructure.Common;

namespace LMS.Controllers.FinancialController.ViewModels
{
    public class ReceivingExpensesEditViewModel
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