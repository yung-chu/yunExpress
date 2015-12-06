using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Web.Models;

namespace LMS.Controllers.WayBillController
{
    public class TrackingNumberModel : SearchFilter
    {
        public TrackingNumberModel()
        {
            shippingMethod=new ShippingMethodModel();
            shippingMethods=new List<SelectListItem>();
            uploadTrackingNumberDetailModel=new UploadTrackingNumberDetailModel();
            uploadTrackingNumberDetailModels = new List<UploadTrackingNumberDetailModel>();
            PagedList=new PagedList<UploadTrackingNumberDetailModel>();
        }
        public ShippingMethodModel shippingMethod { get; set; }
        public List<SelectListItem> shippingMethods { get; set; }
        public UploadTrackingNumberDetailModel uploadTrackingNumberDetailModel { get; set; }
        public List<UploadTrackingNumberDetailModel> uploadTrackingNumberDetailModels { get; set; }
        public IPagedList<UploadTrackingNumberDetailModel> PagedList { get; set; }

        public string TrackingNumberID { get; set; }
        public int ShippingMethodID { get; set; }
        public string ShippingMethodName { get; set; }
        public string ApplianceCountry { get; set; }

        public string StartSegment { get; set; }

        public string EndSegment { get; set; }

        public string StartCharacter { get; set; }

        public string EndCharacter { get; set; }

        public short Status { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedNo { get; set; }
        public string LastUpdatedBy { get; set; }
        public System.DateTime LastUpdateOn { get; set; }

        public string filePath { get; set; }
        public string CountryList { get; set; }
        public bool IsEdit { get; set; }
        public string SiteId { get; set; }
        public int Type { get; set; }

    }
    public class UploadTrackingNumberDetailModel
    {
        public string TrackingNumberDetailID { get; set; }
        public string TrackingNumberID { get; set; }
        public string TrackingNumber { get; set; }
        public short Status { get; set; }
        public string WayBillNumber { get; set; }

        public int IsRepeat { get; set; }
    }

    
}