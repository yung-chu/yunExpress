using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Data.Entity;
using LMS.Services.CustomerServices;
using LMS.Services.FeeManageServices;
using LMS.Services.FreightServices;
using LMS.UserCenter.Controllers.CustomerController.Models;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Web.Controllers;
using LighTake.Infrastructure.Seedwork;

namespace LMS.UserCenter.Controllers.CustomerController
{
    public class CustomerController : BaseController
    {
    
        private ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public string GetCustomerList(FormCollection from)
        {
            var param =
                JsonHelper.JsonToEntity(from["params"], new CustomerListFilterModel()) as CustomerListFilterModel;
            var list =
                _customerService.GetCustomerList(param.CustomerCode, param.Status)
                                .ToModelAsCollection<Customer, CustomerListModel>();
            return JsonHelper.CreateJsonParameters(list, true, list.Count);
        }

        public ActionResult SelectList(bool? IsAll)
        {
            IsAll = IsAll ?? false;
            return View(CustomerDataBind(new CustomerFilterModel() {IsAll = IsAll.Value}));
        }

        [HttpPost]
        public ActionResult SelectList(CustomerViewModel paramModel)
        {
            return View(CustomerDataBind(paramModel.FilterModel));
        }

        private CustomerViewModel CustomerDataBind(CustomerFilterModel filterModel)
        {
            CustomerViewModel viewModel = new CustomerViewModel();
            CustomerParam param = new CustomerParam();
            viewModel.FilterModel = filterModel;
            if (!string.IsNullOrWhiteSpace(filterModel.CustomerCode))
                param.CustomerCode = filterModel.CustomerCode;
            viewModel.CustomerModels =
                _customerService.GetCustomerList(param.CustomerCode, filterModel.IsAll)
                                .ToModelAsCollection<Customer, CustomerModel>();
            return viewModel;
        }



        public JsonResult GetSelectCustomerByParam(string keyword, bool IsAll)
        {
            CustomerParam param = new CustomerParam();
            if (!string.IsNullOrWhiteSpace(keyword))
                param.CustomerCode = keyword;
            var list =
                _customerService.GetCustomerList(param.CustomerCode, IsAll)
                                .ToModelAsCollection<Customer, CustomerModel>();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

    }
}
