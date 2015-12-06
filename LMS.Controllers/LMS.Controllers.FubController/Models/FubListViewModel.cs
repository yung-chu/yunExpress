using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Web.Models;
using LMS.Data.Entity;

namespace LMS.Controllers.FubController.Models
{
    public class FubListViewModel
    {
        public FubListViewModel()
        {
            SearchWheres = new List<SelectListItem>();
            DateWheres = new List<SelectListItem>();
            FubListFilterModel = new FubListFilterModel();
            CountryList=new List<SelectListItem>();
            PagedList = new PagedList<FubListModel>();
        }


        public IPagedList<FubListModel> PagedList  { get; set; }
        public FubListFilterModel FubListFilterModel { get; set; }
        public List<SelectListItem> SearchWheres { get; set; }
        public List<SelectListItem> DateWheres { get; set; }
        public List<SelectListItem> CountryList { get; set; }

    }

    public class FubListModel
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

    public class FubListFilterModel : SearchFilter
    {
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
        public int DateWhere { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string CreatedBy { get; set; }
    }






    public class ReturnBagLogListModel
    {
        public ReturnBagLogListModel()
        {
            FilterModel = new MailReturnGoodsLogsParam();
            PagedList = new PagedList<MailReturnGoodsLogsExt>();
            SelectListItem = new List<SelectListItem>();
        }

        public MailReturnGoodsLogsParam FilterModel { get; set; }
        public IPagedList<MailReturnGoodsLogsExt> PagedList { get; set; }
        public List<SelectListItem> SelectListItem { get; set; }
    }






    public class ExchangeBagLogModel
    {
        public ExchangeBagLogModel()
        {
            FilterModel = new MailExchangeBagLogsParam();
            PagedList = new PagedList<MailExchangeBagLogsExt>();
        }
        public MailExchangeBagLogsParam FilterModel { get; set; }
        public IPagedList<MailExchangeBagLogsExt> PagedList { get; set; }
    }

    public class HoldLogModel
    {
        public HoldLogModel()
        {
            FilterModel = new HoldLogFilterModel();
            PagedList = new PagedList<MailHoldLogsExt>();
        }

        public HoldLogFilterModel FilterModel { get; set; }
        public IPagedList<MailHoldLogsExt> PagedList { get; set; }
    }

    public class HoldLogFilterModel : SearchFilter
    {
        public string TrackNumbers { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }


}
