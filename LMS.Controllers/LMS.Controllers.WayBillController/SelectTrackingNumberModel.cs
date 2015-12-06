using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Web.Models;

namespace LMS.Controllers.WayBillController
{
    public class SelectTrackingNumberModel
    {
        public SelectTrackingNumberModel()
        {
            ShippingMethods = new List<SelectListItem>();
            PagedList=new List<TrackingNumberDetailed>();
            FilterModel=new TrackingNumberFilterModel();
        }

        public List<SelectListItem> ShippingMethods { get; set; }
        public List<TrackingNumberDetailed> PagedList { get; set; }
        public int? ShippingMethodID { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TrackingNumberFilterModel FilterModel { get; set; }
    }

    public class TrackingNumberDetailed
    {
        public string TrackingNumberId { get; set; }
        public int ShippingMethodId { get; set; }
        public string ShippingMethodName { get; set; }
        public int Status { get; set; }
        public int Used { get; set; }
        public int NotUsed { get; set; }
        public DateTime CreatedNo { get; set; }
        public string ApplianceCountry { get; set; }
    }

    public class TrackingNumberFilterModel: SearchFilter
    {
        public int? ShippingMethodId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

    }
}