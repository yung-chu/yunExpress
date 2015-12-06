using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class ResponseResult
    {
        public bool Result { get; set; }
        public string Message { get; set; }
        public string TrackingNumber { get; set; }
        public string Url { get; set; }
        public string ErrorWayBillNumber { get; set; }
    }

    public class CheckOnWayBillResponseResult
    {
        public bool Result { get; set; }
        public string Message { get; set; }
        public string TrackingNumber { get; set; }
        public string ErrorWayBillNumber { get; set; }
        public ShippingInfoModel ShippingInfo { get; set; }
    }
    public class JsonResponseResult<T> where T : new()
    {
        public string Result { get; set; }
        public string ResultDesc { get; set; }
        public T Item { get; set; }
    }
}
