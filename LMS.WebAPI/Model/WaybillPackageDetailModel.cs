using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.WebAPI.Model
{
    public class WaybillPackageDetailModel
    {
        public string WayBillNumber { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public Nullable<decimal> AddWeight { get; set; }
        public Nullable<decimal> SettleWeight { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Height { get; set; }
        public virtual Nullable<decimal> LengthFee { get; set; }
        public virtual Nullable<decimal> WeightFee { get; set; }
    
    }
}