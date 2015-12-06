using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LighTake.Infrastructure.Common;

namespace LMS.Controllers.FinancialController.ViewModels
{
    public class DeliveryFeeAnomalyEditViewModel
    {
        public string CustomerOrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string Remark { get; set; }
        public string WayBillNumber { get; set; }

        public decimal? SetWeightOriginal { get; set; }
        public decimal? FreightOriginal { get; set; }
        public decimal? RegisterOriginal { get; set; }
        public decimal? FuelChargeOriginal { get; set; }
        public decimal? TariffPrepayFeeOriginal { get; set; }
        public decimal? SurchargeOriginal { get; set; }
        public decimal? OverWeightLengthGirthFeeOriginal { get; set; }
        public decimal? SecurityAppendFeeOriginal { get; set; }
        public decimal? AddedTaxFeeOriginal { get; set; }
        public decimal? OtherFeeOriginal { get; set; }
        public decimal? TotalFeeOriginal { get; set; }
        public string OtherFeeRemark { get; set; }

        public decimal? FreightFinal { get; set; }
        public decimal? RegisterFinal { get; set; }
        public decimal? FuelChargeFinal { get; set; }
        public decimal? TariffPrepayFeeFinal { get; set; }
        public decimal? SurchargeFinal { get; set; }
        public decimal? OverWeightLengthGirthFeeFinal { get; set; }
        public decimal? SecurityAppendFeeFinal { get; set; }
        public decimal? AddedTaxFeeFinal { get; set; }
        public decimal? OtherFeeFinal { get; set; }
        public decimal? TotalFeeFinal { get; set; }

        public decimal? SetWeightVender { get; set; }
        public decimal? OtherFeeVender { get; set; }
        public decimal? FreightVender { get; set; }
        public decimal? RegisterVender { get; set; }
        public decimal? FuelChargeVender { get; set; }
        public decimal? TariffPrepayFeeVender { get; set; }
        public decimal? SurchargeVender { get; set; }
        public decimal? OverWeightLengthGirthFeeVender { get; set; }
        public decimal? SecurityAppendFeeVender { get; set; }
        public decimal? AddedTaxFeeVender { get; set; }
        public string OtherFeeRemarkVender { get; set; }
        public decimal? TotalFeeVender { get; set; }
    }


}