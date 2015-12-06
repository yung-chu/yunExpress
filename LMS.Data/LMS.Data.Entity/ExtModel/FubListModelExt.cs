using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class FubListModelExt:LighTake.Infrastructure.Seedwork.Entity
    {
        public string PostBagNumber { get; set; }
        public string FuPostBagNumber { get; set; }
        public string MailTotalPackageNumber { get; set; }
        public int? ShortNumber { get; set; }
        public DateTime? ScanTime { get; set; }
        public string ScanBy { get; set; }
        public DateTime? CenterScanTime { get; set; }
        public string CenterCreatedBy { get; set; }
        public decimal? TotalWeight { get; set; }
        public int? TotalNumber { get; set; }
        public string CountryCode { get; set; }
    }
}
