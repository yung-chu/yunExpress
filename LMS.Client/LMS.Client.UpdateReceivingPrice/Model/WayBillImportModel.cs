using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Client.UpdateReceivingPrice.Model
{
    public class WayBillImportModel
    {
        public WayBillImportModel()
        {
            Packages = new List<PackageRequest>();
            ShippingInfo = new ShippingInfoModel();
        }
        //public int CustomerTypeId { get; set; }
        public Guid CustomerId { get; set; }
        public string CountryCode { get; set; }
        public int ShippingMethodId { get; set; }
        public int ShippingTypeId { get; set; }
        public bool EnableTariffPrepay { get; set; }
        public List<PackageRequest> Packages { get; set; }
        public ShippingInfoModel ShippingInfo { get; set; }
    }
    public class PackageRequest
    {
        public decimal Length { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public decimal Weight { get; set; }
    }
    public class WayBillPriceModel:WayBillImportModel
    {
        public string WayBillNumber { get; set; }
        //public int CustomerTypeID { get; set; }
        public int ReceivingExpenseID { get; set; }
    }
    public class ShippingInfoModel
    {
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZip { get; set; }
    }
}
