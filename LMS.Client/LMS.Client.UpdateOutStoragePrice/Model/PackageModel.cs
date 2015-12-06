

using System;
using System.Collections.Generic;

namespace LMS.Client.UpdateOutStoragePrice.Model
{
    //public class PackageModel
    //{
    //    public decimal Length { get; set; }

    //    public decimal Width { get; set; }

    //    public decimal Height { get; set; }

    //    public decimal Weight { get; set; }

    //    public string CountryCode { get; set; }



    //    public int ShippingTypeId { get; set; }
    //}

    //public class VenderPackageModel : PackageModel
    //{
    //    public int ShippingMethodId { get; set; }
    //    public string Code { get; set; }
    //    public string WayBillNumber { get; set; }
    //}
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
        public string WayBillNumber { get; set; }
        public Guid CustomerId { get; set; }
        public bool EnableTariffPrepay { get; set; }
        public List<VenderPackageRequest> Packages { get; set; }
    }
}
