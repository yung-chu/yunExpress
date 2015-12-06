using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class ShippingMethod
    {
        public string ShippingMethodName { get; set; }
        public int ShippingMethodId { get; set; }
        public string ShippingMethodCode { get; set; }
        public bool WeightOrVolume { get; set; }
        public bool HaveTrackingNum { get; set; }
        public string ShippingMethodEName { get; set; }
        public bool IsHideTrackingNumber { get; set; }
        public int ShippingMethodTypeId { get; set; }
        //是否计算偏远附加费
        public bool FuelRelateRAF { get; set; }

    }
    public class ShippingMethodCountryModel : LighTake.Infrastructure.Seedwork.Entity
    {
        public string CountryCode { get; set; }

        public int ShippingMethodId { get; set; }

        public int AreaId { get; set; }
    }
    public class ShippingMethodModel
    {
        public int ShippingMethodId { get; set; }
        public bool CalcVolumeweight { get; set; }
        public string FullName { get; set; }
        public string EnglishName { get; set; }
        public string Code { get; set; }
        public bool Enabled { get; set; }
        public bool HaveTrackingNum { get; set; }
        public bool IsSysTrackNumber { get; set; }
        public int ShippingMethodTypeId { get; set; }
        public decimal PackageTransformFileWeight { get; set; }
        //Add By zhengsong ;Time:2014-06-05
        //是否影藏跟踪号
        public bool IsHideTrackingNumber { get; set; }
        public string TrackingUrl { get; set; }
        //是否计算偏远附加费
        public bool FuelRelateRAF { get; set; }
    }
}
