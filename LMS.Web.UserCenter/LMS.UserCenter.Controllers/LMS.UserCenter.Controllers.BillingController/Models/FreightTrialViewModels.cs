using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LighTake.Infrastructure.Common;

namespace LMS.UserCenter.Controllers.BillingController.Models
{
    public class FreightTrialViewModels
    {
        public FreightTrialViewModels()
        {
            Filter = new FreightTrialFilterModel();

            FreightList = new List<FreightModel>();
        }

        public List<FreightModel> FreightList { get; set; }
        public FreightTrialFilterModel Filter { get; set; }
    }

    public class FreightTrialFilterModel
    {
        public decimal? Length { get; set; }

        public decimal? Width { get; set; }

        public decimal? Height { get; set; }

        public decimal? Weight { get; set; }

        public string CountryCode { get; set; }

        public int PackageType { get; set; }
    }

    public class FreightModel
    {
        public int ShippingMethodId { get; set; }

        public string ShippingMethodName { get; set; }

        public decimal Weight { get; set; }

        public decimal ShippingFee { get; set; }

        public decimal RemoteAreaFee { get; set; }

        public decimal RegistrationFee { get; set; }

        public decimal SundryFee {
            get { return (OtherFee + RemoteAreaFee).NewRound(2); }
        }

        public decimal FuelFee { get; set; }

        public decimal OtherFee { get; set; }

        public decimal TotalFee {
            get
            {
                return
                    (ShippingFee + RegistrationFee + FuelFee + OtherFee + RemoteAreaFee)
                        .NewRound(2);
            }
        }

        public string Remarks { get; set; }

        public string DeliveryTime { get; set; }
    }
}