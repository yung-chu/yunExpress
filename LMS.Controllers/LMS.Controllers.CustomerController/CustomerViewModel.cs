using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LighTake.Infrastructure.Common;
using LMS.Data.Entity;

namespace LMS.Controllers.CustomerController
{
    public class CustomerViewModel
    {
        public CustomerViewModel()
        {
            CustomerModels = new List<CustomerModel>();
            FilterModel = new CustomerFilterModel();
        }
        public List<CustomerModel> CustomerModels { get; set; }
        public CustomerFilterModel FilterModel { get; set; }
        public bool ShowPaymentType { get; set; }
        public bool OnlyShowCash { get; set; }
    }
    public class SelectListModel
    {
        public string SelectValue { get; set; }
        public string SelectName { get; set; }
    }
    public class CustomerAddViewModel
    {
        public CustomerAddViewModel()
        {
			CustomerManagerList = new List<SelectListItem>();
            CustomerTypeList = new List<SelectListModel>();
            PaymentTypeList = new List<SelectListModel>();
            CustomerStatus=new List<SelectListItem>();
        }

		public List<SelectListItem> CustomerManagerList { get; set; }
	    public List<SelectListModel> CustomerTypeList { get; set; }
        public List<SelectListModel> PaymentTypeList { get; set; }
        public List<SelectListItem> CustomerStatus { get; set; }
		public string CustomerManager { get; set; }
    }
    public class CustomerEditViewModel
    {
        public CustomerEditViewModel()
        {
            CustomerTypeList = new List<SelectListItem>();
            PaymentTypeList = new List<SelectListItem>();
            CustomerModel = new CustomerListModel();
            CustomerStatus=new List<SelectListItem>();
	        CustomerManagerList = new List<SelectListItem>();
        }
        public List<SelectListItem> CustomerTypeList { get; set; }
        public List<SelectListItem> PaymentTypeList { get; set; }
        public List<SelectListItem> CustomerStatus { get; set; }
        public CustomerListModel CustomerModel { get; set; }
		public List<SelectListItem> CustomerManagerList { get; set; }
		public string ReturnUrl { get; set; }
        public string SourcePlatform { get; set; }
    }
    public class CustomerRechargeViewModel
    {
        public CustomerRechargeViewModel()
        {
            MoneyChangeTypeList = new List<SelectListModel>();
            FeeTypeList = new List<SelectListModel>();
        }
        public List<SelectListModel> MoneyChangeTypeList { get; set; }
        public List<SelectListModel> FeeTypeList { get; set; }
    }
    public class CustomerRechargeListViewModel
    {
        public CustomerRechargeListViewModel()
        {
            PagedList = new PagedList<CustomerCreditModel>();
            FilterModel = new CustomerRechargeListFilterModel();
            StatusModels = new List<SelectListItem>();
        }
        public IPagedList<CustomerCreditModel> PagedList { get; set; }
        public CustomerRechargeListFilterModel FilterModel { get; set; }
        public IList<SelectListItem> StatusModels { get; set; }
    }
    public class CustomerAmountRecordListViewModel
    {
        public CustomerAmountRecordListViewModel()
        {
            PagedList = new PagedList<CustomerAmountRecordListModel>();
            FilterModel = new CustomerAmountRecordListFilterModel();
        }
        public IPagedList<CustomerAmountRecordListModel> PagedList { get; set; }
        public CustomerAmountRecordListFilterModel FilterModel { get; set; }
        public decimal TotalInFee { get; set; }
        public decimal TotalOutFee { get; set; }
    }



	public class CustomerListViewModel
	{
		public CustomerListViewModel()
		{
			PagedList = new PagedList<CustomerExt>();
			FilterModel = new CustomerListFilterModel();
			CustomerStatus = new List<SelectListItem>();
		}
		public IPagedList<CustomerExt> PagedList { get; set; }
		public CustomerListFilterModel FilterModel { get; set; }
		public List<SelectListItem> CustomerStatus { get; set; }
	}



}