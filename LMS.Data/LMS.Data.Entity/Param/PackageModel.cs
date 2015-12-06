using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class PackageModel
    {
        public decimal Length { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public decimal Weight { get; set; }

        public string CountryCode { get; set; }



        public int ShippingTypeId { get; set; }
    }

    public class VenderPackageModel : PackageModel
    {
        public int ShippingMethodId { get; set; }
        public string Code { get; set; }
    }

    public class CustomerPackageModel : PackageModel
    {
        public int ShippingMethodId { get; set; }
        public int CustomerTypeId { get; set; }
    }

    public class CustomerInfoPackageModel : PackageModel
    {
        public int ShippingMethodId { get; set; }
        public int CustomerTypeId { get; set; }
        public Guid? CustomerId { get; set; }
        public bool EnableTariffPrepay { get; set; }
    }
    public class FreightPackageModel : PackageModel
    {
        public int CustomerTypeId { get; set; }
        public Guid? CustomerId { get; set; }
        public bool EnableTariffPrepay { get; set; }
    }
    //public class CustomerModel
    //{
    //    public Guid CustomerId { get; set; }
    //    public int? CustomerTypeId { get; set; }
    //    public string CustomerCode { get; set; }
    //    public string Name { get; set; }
    //    public string EnName { get; set; }
    //    public string Address { get; set; }
    //    public string QQ { get; set; }
    //    public string MSN { get; set; }
    //    public string Skype { get; set; }
    //    public string Phone { get; set; }
    //    public string Country { get; set; }
    //    public string Province { get; set; }
    //    public string Fax { get; set; }
    //    public string PostCode { get; set; }
    //    public int? CustomerOrigin { get; set; }
    //    public int Status { get; set; }
    //}

    public class CustomerInfoPackageRequest
    {
        public CustomerInfoPackageRequest()
        {
            Packages = new List<PackageRequest>();
            ShippingInfo=new ShippingInfoModel();
        }
        public Guid CustomerId { get; set; }
        public string CountryCode { get; set; }
        public int ShippingMethodId { get; set; }
        public int ShippingTypeId { get; set; }
        public bool EnableTariffPrepay { get; set; }
        public List<PackageRequest> Packages { get; set; }
        public ShippingInfoModel ShippingInfo { get; set; }
    }

    public class ShippingInfoModel
    {
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZip { get; set; }
    }

    public class PackageRequest
    {
        public decimal Length { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public decimal Weight { get; set; }
    }


    public class VenderPackageRequest
    {
        public decimal Length { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public decimal Weight { get; set; }

        public decimal Volume { get; set; }
    }

    public class VenderInfoPackageRequest
    {
        public VenderInfoPackageRequest()
        {
            Packages = new List<VenderPackageRequest>();
        }
        public string VenderCode { get; set; }
        public string CountryCode { get; set; }
        public int ShippingMethodId { get; set; }
        public int ShippingTypeId { get; set; }
        public Guid? CustomerId { get; set; }
        public bool EnableTariffPrepay { get; set; }
        public List<VenderPackageRequest> Packages { get; set; }
    }

}
