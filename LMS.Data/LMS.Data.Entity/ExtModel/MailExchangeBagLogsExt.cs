using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class MailExchangeBagLogsExt
    {

        public  string TrackNumber { get; set; }
        public  string PostBagNumber { get; set; }
        public  string NewPostBagNumber { get; set; }
        public  System.DateTime ExchangeTime { get; set; }
        public  string RecordBy { get; set; }
        public  System.DateTime CreatedOn { get; set; }
        public  string CreatedBy { get; set; }
        public  System.DateTime LastUpdatedOn { get; set; }
        public  string LastUpdatedBy { get; set; }

        public string CountryCode { get; set; }
    }

    public class MailHoldLogsExt
    {
        public string TrackNumber { get; set; }
        public string PostBagNumber { get; set; }
        public System.DateTime? HoldOn { get; set; }
        public string HoldBy { get; set; }
        public int? Status { get; set; }
        public decimal? Weight { get; set; }
        public string CountryName{ get; set; }
    }
}
