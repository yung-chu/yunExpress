using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Web.Models;

namespace LMS.Controllers.WayBillController
{
    public class B2CPreAlterViewModel
    {
        public B2CPreAlterViewModel()
        {
            Param = new B2CPreAlterListFilte();
            PagedList=new PagedList<B2CPreAlterExt>();
            ShippingMethods=new List<SelectListItem>();
            SearchWheres=new List<SelectListItem>();
            DateTimeWheres=new List<SelectListItem>();
            StatusList=new List<SelectListItem>();
        }
        public B2CPreAlterListFilte Param { get; set; }
        public IPagedList<B2CPreAlterExt> PagedList { get; set; }
        public List<SelectListItem> ShippingMethods { get; set; }
        public List<SelectListItem> SearchWheres { get; set; }
        public List<SelectListItem> DateTimeWheres { get; set; }
        public List<SelectListItem> StatusList { get; set; }
    }

    public class B2CPreAlterListFilte : SearchFilter
    {
        public int ShippingMethodId { get; set; }
        public string ShippingName { get; set; }
        public string CustomerCode { get; set; }
        public string NickName { get; set; }
        public string CountryCode { get; set; }
        public decimal? StartWeight { get; set; }
        public decimal? EndWeight { get; set; }
        public int SearchWhere { get; set; }
        public string SearchContext { get; set; }
        public int SearchTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime OutStartTime { get; set; }
        public bool IsSelectAll { get; set; }
        public int? Status { get; set; }
        public string SelectWayBillNumber { get; set; }
    }
}