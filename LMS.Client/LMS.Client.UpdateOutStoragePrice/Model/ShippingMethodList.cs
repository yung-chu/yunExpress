using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Client.UpdateOutStoragePrice.Model
{
    public class ShippingMethodModel
    {
        public int ShippingMethodId { get; set; }
        public bool CalcVolumeweight { get; set; }
        public string FullName { get; set; }
        public string EnglishName { get; set; }
        public string Code { get; set; }
        public bool Enabled { get; set; }
        public decimal PackageTransformFileWeight { get; set; }
        public bool HaveTrackingNum { get; set; }
        public string DisplayName { get; set; }
        public int ShippingMethodTypeId { get; set; }
        public bool IsSysTrackNumber { get; set; }
        public bool IsHideTrackingNumber { get; set; }
        public string TrackingUrl { get; set; }
    }

    public class ShippingMethodInfo : ShippingMethodModel
    {
        public string TrackingUrl { get; set; }
        public string Description { get; set; }

    }

    public class ShippingMethodList
    {
        public ShippingMethodList()
        {
            List = new List<ShippingMethodInfo>();
        }

        /// <summary>
        /// 运输方式列表
        /// </summary>
        public List<ShippingMethodInfo> List { get; set; }

        /// <summary>
        /// 时间戳（UTC时间-yyyy-MM-ddTHH:mm:ss）
        /// </summary>
        public string TimeStamp { get; set; }
    }
}
