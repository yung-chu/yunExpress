using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LighTake.Infrastructure.Common;

namespace LMS.Controllers.FinancialController.ViewModels
{
    public class ReceivingExpenseListViewModel
    {

        public  ReceivingExpenseListViewModel()
        {
            FilterModel = new ReceivingExpenseListFilterModel();
            PagedList = new PagedList<ReceivingExpenseModel>();
            SearchWheres = new List<SelectListItem>();
        }
        
        public ReceivingExpenseListFilterModel FilterModel { get; set; }
        public IList<SelectListItem> SearchWheres { get; set; }
        public PagedList<ReceivingExpenseModel> PagedList { get; set; }

    }

    public class ReceivingExpenseModel
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ShippingMethodName { get; set; }
        public string WayBillNumber { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string TrackingNumber { get; set; }
        public string CountryCode { get; set; }
        public decimal Weight { get; set; }
        public decimal SettleWeight { get; set; }

        public decimal? Freight { get; set; }
        public decimal? Register { get; set; }
        public decimal? FuelCharge { get; set; }
        public decimal? TariffPrepayFee { get; set; }
        public decimal? Surcharge { get; set; }
        public decimal? RemoteAreaFee { get; set; }
        public decimal? TotalFee { get; set; }

        public string FinancialNote { get; set; }
    }

}