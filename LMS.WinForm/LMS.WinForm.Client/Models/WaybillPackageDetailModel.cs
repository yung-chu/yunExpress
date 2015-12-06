using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.WinForm.Client.Models
{
    public class WaybillPackageDetailModel
    {
       // public string CustomersId { get; set; }

        public string PackageDetailID { get; set; }
        public string TrackingNumber { get; set; }
        public string WayBillNumber { get; set; }//打印标签属性
        public string Country { get; set; }//打印标签属性
        public string CountryCode { get; set; }//国家编码
        public string ShippingMethod { get; set; }//打印标签属性
        public  int Pieces { get; set; }
        public  Nullable<decimal> Weight { get; set; }
        public Nullable<decimal> AddWeight { get; set; }
        public  Nullable<decimal> SettleWeight { get; set; }
        public  Nullable<decimal> Length { get; set; }
        public  Nullable<decimal> Width { get; set; }
        public  Nullable<decimal> Height { get; set; }
        public Nullable<decimal> LengthFee { get; set; }
        public Nullable<decimal> WeightFee { get; set; }
        public  System.DateTime CreatedOn { get; set; }
        public  string CreatedBy { get; set; }
        public  System.DateTime LastUpdatedOn { get; set; }
        public  string LastUpdatedBy { get; set; }

       // public virtual WayBillInfo WayBillInfo { get; set; }
    }
}
