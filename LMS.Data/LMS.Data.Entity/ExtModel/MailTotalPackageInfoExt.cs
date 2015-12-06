using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class MailTotalPackageInfoExt:LighTake.Infrastructure.Seedwork.Entity
    {
        public string MailTotalPackageNumber { get; set; }
        public string TotalPackageNumber { get; set; }
        public DateTime ScanTime { get; set; }
        public int? FZFlightType { get; set; }
        public string FZFlightNo { get; set; }
        public DateTime? FuZhouDepartureTime { get; set; }
        public DateTime? TaiWanArrivedTime { get; set; }
        public string TWFlightNo { get; set; }
        public DateTime? TaiWanDepartureTime { get; set; }
        public DateTime? ToArrivedTime { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public int TrackStatus { get; set; }
        public int ShortNumber { get; set; }
        public string CountryCode { get; set; }
    }
    public class MailTotalPackageCountryExt
    {
        public string MailTotalPackageNumber { get; set; }
        public string CountryCode { get; set; }
    }
}
