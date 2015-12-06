using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Web.Models;

namespace LMS.Controllers.FubController.Models
{
    public class MailTotalPackageInfoModel
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
    public class LogFlightNumberListViewModel
    {
        public LogFlightNumberListViewModel()
        {
            FilterModel = new LogFlightNumberListFilter();
            CountryList=new List<SelectListItem>();
            PagedList=new PagedList<MailTotalPackageInfoModel>();
        }
        public IPagedList<MailTotalPackageInfoModel> PagedList { get; set; }
        public LogFlightNumberListFilter FilterModel { get; set; }
        public List<SelectListItem> CountryList { get; set; }
    }
    public class LogFlightNumberListFilter : SearchFilter
    {
        public string MailTotalPackageNumber { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}