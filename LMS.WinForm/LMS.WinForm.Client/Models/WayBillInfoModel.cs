using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;

namespace LMS.WinForm.Client.Models
{
    public class WayBillInfoModel : LighTake.Infrastructure.Seedwork.Entity
    {
        public WayBillInfoModel()
        {
            WaybillPackageDetaillList = new List<WaybillPackageDetailModel>();
        }
        public string WayBillNumber { get; set; }
        public Nullable<int> CustomerOrderID { get; set; }
        public string CustomerOrderNumber { get; set; }//客户订单号 add by huhaiyou 
        public virtual int Pieces { get; set; }//件数 add by huhaiyou 2014-4-24
        public string CustomerCode { get; set; }
        public string TrackingNumber { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public Nullable<decimal> SettleWeight { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Height { get; set; }
        public Nullable<int> GoodsTypeID { get; set; }
        public bool IsReturn { get; set; }
        public bool IsHold { get; set; }
        public bool IsBattery { get; set; }
        public Nullable<int> SensitiveType { get; set; }
        public int Status { get; set; }
        public string OutStorageID { get; set; }
        public string InStorageID { get; set; }
        public Nullable<int> ShippingInfoID { get; set; }
        public string CountryCode { get; set; }
        public Nullable<int> InsuredID { get; set; }
        public Nullable<int> AbnormalID { get; set; }
        public Nullable<int> InShippingMethodID { get; set; }
        public Nullable<int> OutShippingMethodID { get; set; }
        public string InShippingMethodName { get; set; }
        public string OutShippingMethodName { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public int ShippingMethodTypeId { get; set; }
        public PriceProviderResult PriceResult { get; set; }

        //public decimal Freight { get; set; }
        //public decimal FuelCharge { get; set; }
        //public decimal Register { get; set; }
        //public decimal Surcharge { get; set; }
        //public decimal TariffPrepay { get; set; }

        public List<WaybillPackageDetailModel> WaybillPackageDetaillList { get; set; }
    }
}
