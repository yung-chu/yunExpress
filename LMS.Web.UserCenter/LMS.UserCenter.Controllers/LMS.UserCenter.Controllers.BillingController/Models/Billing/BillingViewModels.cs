using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LighTake.Infrastructure.Common;

namespace LMS.UserCenter.Controllers.BillingController.Models
{
    public class BillingViewModels
    {
        public BillingViewModels()
        {
            BillingList = new PagedList<BillingModel>();
            Filter = new BillingFilterModel();
        }

        public IPagedList<BillingModel> BillingList { get; set; }

        public BillingFilterModel Filter { get; set; }

        public decimal TotalInFee { get; set; }

        public decimal TotalOutFee { get; set; }
    }
}