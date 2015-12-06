using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LighTake.Infrastructure.Common;

namespace LMS.UserCenter.Controllers.OrderController.Models
{
    public class EubWayBillViewModel
    {
        public EubWayBillViewModel()
        {
            CountryList = new List<SelectListItem>();
            ShippingMethods = new List<SelectListItem>();
            StatusList = new List<SelectListItem>();
            PrintFormatList = new List<SelectListItem>();
            PagedList=new PagedList<EubWayBillApplicationInfoModel>();
            QueryNumberList=new List<SelectListItem>();
            TimeTypeList=new List<SelectListItem>();
            Filter=new EubWayBillFilter();
        }

        public List<SelectListItem> CountryList { get; set; }

        public List<SelectListItem> ShippingMethods { get; set; }

        public List<SelectListItem> StatusList { get; set; }

        public List<SelectListItem> PrintFormatList { get; set; }

        public List<SelectListItem> QueryNumberList { get; set; }

        public List<SelectListItem> TimeTypeList { get; set; } 

        public PagedList<EubWayBillApplicationInfoModel> PagedList { get; set; }

        public EubWayBillFilter Filter { get; set; }
    }
}