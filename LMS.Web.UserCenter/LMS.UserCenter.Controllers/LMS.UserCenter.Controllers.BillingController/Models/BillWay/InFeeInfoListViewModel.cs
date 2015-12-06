using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LighTake.Infrastructure.Common;

namespace LMS.UserCenter.Controllers.BillingController.Models
{
    public class InFeeInfoListViewModel
    {
        public InFeeInfoListViewModel()
        {
            FilterModel = new InFeeListFilterModel();
            PagedList = new PagedList<InFeeInfoModel>();
            SearchWheres = new List<SelectListItem>();
        }

        public List<InFeeInfoModel> List { get; set; }
        public InFeeListFilterModel FilterModel { get; set; }
        public IPagedList<InFeeInfoModel> PagedList { get; set; }
        public IList<SelectListItem> SearchWheres { get; set; }
        public decimal AllTotalFee { get; set; }
    }
}