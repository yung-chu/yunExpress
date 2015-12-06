using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using LighTake.Infrastructure.Common;
using Newtonsoft.Json;

namespace LMS.WebAPI.Client.Models
{
    public class PriceModel
    {
        public PriceModel()
        {
            FreightList=new List<FreightModel>();
            Filter=new FreightTrialFilterModel();
        }

        public List<FreightModel> FreightList { get; set; }
        public FreightTrialFilterModel Filter { get; set; }

       
    }

    public class FreightTrialFilterModel
    {
        public int? Length { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public decimal Weight { get; set; }

        public string CountryCode { get; set; }

        public int PackageType { get; set; }

        public bool EnableTariffPrepay { get; set; }
    }

    public class FilterModel
    {
        public string Length { get; set; }

        public string Width { get; set; }

        public string Height { get; set; }

        public string Weight { get; set; }

        public string CountryCode { get; set; }

        public string PackageType { get; set; }
    }

    public class QuotationModel
    {

        public string Code { get; set; }
        public string ShippingMethodName { get; set; }
        public string ShippingMethodEName { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal RegistrationFee { get; set; }
        public decimal FuelFee { get; set; }
        public decimal TariffPrepayFee { get; set; }
        public decimal SundryFee { get; set; }
        public decimal TotalFee { get; set; }
        public string Remarks { get; set; }
        public string DeliveryTime { get; set; }
    }

    public class FreightModel
    {
        public string ShippingMethodName { get; set; }

        public string ShippingMethodEName { get; set; }


        public string Code { get; set; }
        [JsonIgnore]
        [XmlIgnore]
        public decimal Weight { get; set; }

        public decimal ShippingFee { get; set; }

        public decimal RemoteAreaFee { get; set; }

        public decimal RegistrationFee { get; set; }

        public decimal SundryFee
        {
            get { return (OtherFee + RemoteAreaFee).NewRound(2); }
        }

        public decimal FuelFee { get; set; }

        public decimal OtherFee { get; set; }

        public decimal TariffPrepayFee{ get; set; }

        public decimal TotalFee
        {
            get
            {return
                    (ShippingFee + RegistrationFee + FuelFee + OtherFee + RemoteAreaFee+TariffPrepayFee)
                        .NewRound(2);}
        }

        public string Remarks { get; set; }

        public string DeliveryTime { get; set; }
    }

    public class ParamModel
    {
        public decimal? Length { get; set; }

        public decimal? Width { get; set; }

        public decimal? Height { get; set; }

        public decimal? Weight { get; set; }

        public string CountryCode { get; set; }

        public int ShippingTypeId { get; set; }
    }
}