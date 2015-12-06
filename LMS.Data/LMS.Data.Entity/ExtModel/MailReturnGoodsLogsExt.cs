using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class MailReturnGoodsLogsExt
    {
        public  string PostBagNumber { get; set; }
        public  string TrackNumber { get; set; }
        public  int ReasonType { get; set; }
        public  string ReturnReason { get; set; }
        public  string ReturnBy { get; set; }
        public  System.DateTime ReturnOn { get; set; }
        public  System.DateTime CreatedOn { get; set; }
        public  string CreatedBy { get; set; }
        public  System.DateTime LastUpdatedOn { get; set; }
        public  string LastUpdatedBy { get; set; }


        public int? InShippingMethodID{ get; set; }
        public string InShippingMethodName{ get; set; }
        public decimal? Weight { get; set; }
        public string CountryCode { get; set; }

    }

    public class ReturnGoodsModel
    {
        public string TrackNumber { get; set; }
        public int ReasonType { get; set; }
    }
}
