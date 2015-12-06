using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LighTake.Infrastructure.Common;

namespace LMS.UserCenter.Controllers.BillingController.Models
{
    public class OutFeeInfoListViewModel
    {
        public OutFeeInfoListViewModel()
        {
            FilterModel = new OutStorageFilterModel();
            PagedList = new PagedList<OutFeeInfoModel>();
            SearchWheres = new List<SelectListItem>();
        }
        public OutStorageFilterModel FilterModel { get; set; }
        public IPagedList<OutFeeInfoModel> PagedList { get; set; }
        public IList<SelectListItem> SearchWheres { get; set; }
        public decimal AllTotalFee { get; set; }
    }
}