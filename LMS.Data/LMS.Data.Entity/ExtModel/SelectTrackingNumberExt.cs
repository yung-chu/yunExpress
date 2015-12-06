using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class SelectTrackingNumberExt : TrackingNumberInfo
    {
        public SelectTrackingNumberExt()
        {
            TrackingNumberIds=new List<TrackingNumberID>();
            TrackingNumberDetaileds=new List<TrackingNumber>();
        }

        public List<TrackingNumberID> TrackingNumberIds { get; set; }
        public List<TrackingNumber> TrackingNumberDetaileds { get; set; }
    }

    public class TrackingNumberID
    {
        public string TrackingNumberId { get; set; }
    }

    public class TrackingNumber
    {
        public string TrackingNumberId { get; set; }
        public string ShippingMethodName { get; set; }
        public int Status { get; set; }
        public int Used { get; set; }
        public int NotUsed { get; set; }
        public DateTime CreateNo { get; set; }
        public string ApplicableCountry { get; set; }
    }


    public class TrackingNumberExt : TrackingNumberInfo
    {
        public string ShippingMethodName { get; set; }
        public int Used { get; set; }
        public int NotUsed { get; set; }
    }
}
