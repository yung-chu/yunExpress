using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Controllers.FinancialController.ViewModels
{
    public class ReceivingBillViewModel
    {

        public ReceivingBillViewModel()
        {
            FilterModel = new ReceivingBillFilterModel();
            PagedList = new PagedList<ReceivingBill>();
        }

        public ReceivingBillFilterModel FilterModel { get; set; }
        public IPagedList<ReceivingBill> PagedList { get; set; }
    }

}