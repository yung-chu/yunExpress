using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class MailPostBagInfoExt
    {
        public  string PostBagNumber { get; set; }
        public  string OutStorageID { get; set; }
        public  string CountryCode { get; set; }
        public  bool IsBattery { get; set; }
        public  string FuPostBagNumber { get; set; }
        public  DateTime? ScanTime { get; set; }
        public string ScanBy { get; set; }
        public  DateTime CreatedOn { get; set; }
        public  string CreatedBy { get; set; }
        public  DateTime LastUpdatedOn { get; set; }
        public  string LastUpdatedBy { get; set; }
        public  int TrackStatus { get; set; }
        public  decimal TotalWeight { get; set; }
    }
}
