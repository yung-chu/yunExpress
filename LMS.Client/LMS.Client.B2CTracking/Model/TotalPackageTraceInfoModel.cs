using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Client.B2CTracking.Model
{
    public class TotalPackageTraceInfoModel
    {
        public int ID { get; set; }
        public string TotalPackageNumber { get; set; }
        public int TraceEventCode { get; set; }
        public DateTime TraceEventTime { get; set; }
        public string TraceEventAddress { get; set; }
        public string CreatedBy { get; set; }
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
        public bool IsHideTrackingNumber { get; set; }
        public string TrackingUrl { get; set; }
    }
    public class ShippingMethodConfig
    {
        public int ShippingMethodId { get; set; }
        public string CountryCode { get; set; }
        public string EventContent { get; set; }
        public string Address { get; set; }
        /// <summary>
        /// 相对北京时间变差几小时
        /// </summary>
        public int OffsetHours { get; set; }
        public int AddHours { get; set; }
    }
}
