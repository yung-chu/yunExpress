using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using LMS.Data.Entity;
using LMS.FrontDesk.Controllers.UserController.Models;
using LMS.Services.CustomerServices;
using LMS.Services.NewServices;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Web.Controllers;
using LighTake.Infrastructure.Web.Filters;
using System.Configuration;



namespace LMS.FrontDesk.Controllers.UserController
{
    public class UserController:BaseController
    {
        private ICustomerService _customerService;
        private INewService _newService;

        public UserController(ICustomerService customerService,
                              INewService newService)
        {
            _customerService = customerService;
            _newService = newService;
        }

        public ActionResult AddUser()
        {
            AddCustomerModel model = new AddCustomerModel();
            model.ShowCategoryListModel = ShowCategoryListModel();
            return View(model);
        }




        //产品服务, 新闻中心, 关于我们, 帮助中心 顶部列表
        public ShowCategoryListModel ShowCategoryListModel()
        {

            var model = new ShowCategoryListModel();


            //产品与服务
            model.ShippingMethodServices = GetCategoryList();

            //关于我们
            int flag = 2;
            model.CategoryModelAbout = _newService.GetCategoryById(flag).ToModelAsCollection<Category, CategoryModel>();

            //新闻中心
            int fcategoryid = 1;
            model.CategoryModelNews = _newService.GetCategoryById(fcategoryid).ToModelAsCollection<Category, CategoryModel>();

            //帮助中心
            Category getCategory = _newService.GetCategoryInfo("帮助中心");
            model.HelpCenterList = GetCategoryModel(getCategory.CategoryID);


            return model;
        }




        /// <summary>
        /// 注册页
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddUser(AddCustomerModel model)
        {
			//获取配置文件值 ysd
			string getCustomerRule = ConfigurationManager.AppSettings["SetCustomerCode"];


			//客户编码开头取字母 ,没有配置默认开头为C
	        if(string.IsNullOrEmpty(getCustomerRule))
	        {
				_customerService.CreateCustomerCode("C");
	        }
	        else
	        {
				_customerService.CreateCustomerCode(getCustomerRule);
	        }


            if (!ModelState.IsValid)
            {

                model.ShowCategoryListModel = ShowCategoryListModel();
                return View(model);
            }
           
                Customer customer = new Customer();
                customer.AccountID = model.AccountID.Trim();
                customer.Name = model.Name;
                //customer.LinkMan = model.LinkMan.Trim();
                customer.AccountPassWord = model.Password.Trim();
                customer.Mobile = model.Mobile.Trim();
                customer.Tele = customer.Mobile;
                //customer.Email = model.Email.Trim();
                //customer.QQ = model.QQ;

			     //去空格,换行 解决用户列表不显示
				if (!string.IsNullOrEmpty(model.Address))
				{
					customer.Address = Regex.Replace(model.Address, @"[\r\n]", "").Replace(" ", "");
				}
	     

                customer.Status = 1;//未审核
                customer.CustomerTypeID = 12;
                customer.EnableCredit = true;
                customer.MaxDelinquentAmounts = 2000;
            
                _customerService.CreateCustomer(customer);


			   //取用户信息
	            Customer getCustomer  = _customerService.GetCustomerByAccountId(model.AccountID);
				return RedirectToAction("AddSuccess", "User", new { name = model.AccountID, code = getCustomer.CustomerCode});
        }

        public ActionResult AddSuccess(string name,string code)
        {
            AddCustomerModel model=new AddCustomerModel();

            model.ShowCategoryListModel = ShowCategoryListModel();

            model.Name = name;
	        model.CustomerCode = code;
            return View(model);
        }

        public JsonResult SelectAccountId(string AccountId)
        {
            if (_customerService.BoolCustomerAccountId(AccountId))
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        public List<CategoryModel> GetCategoryList()
        {
            return _newService.GetCategoryById(3).ToModelAsCollection<Category,CategoryModel>();
        }
        //获取列表信息
        public List<CategoryModel> GetCategoryModel(int flag)
        {
            return _newService.GetCategoryById(flag).ToModelAsCollection<Category,CategoryModel>();
        }
    }
}
