using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using LighTake.Infrastructure.Web.Filters;
using LMS.Core;
using LMS.Data.Entity;
using LMS.FrontDesk.Controllers.HomeController.Models;
using LMS.Services.CommonServices;
using LMS.Services.CountryServices;
using LMS.Services.CustomerServices;
using LMS.Services.NewServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Caching;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Web;
using LighTake.Infrastructure.Web.Controllers;
using LighTake.Infrastructure.Web.Utities;

namespace LMS.FrontDesk.Controllers.HomeController
{
    public class HomeController : BaseController
    {
        private INewService _newService;
        private readonly IWorkContext _workContext;
        private readonly ICountryService _countryService;
        private readonly IGoodsTypeService _goodsTypeService;
        private readonly ICustomerService _customerService;
        private readonly IAuthenticationService _authenticationService;

        public HomeController(INewService newService, IWorkContext workContext, ICountryService countryService, IGoodsTypeService goodsTypeService,ICustomerService customerService,IAuthenticationService authenticationService)
        {
            _newService = newService;
            _workContext = workContext;
            _countryService = countryService;
            _goodsTypeService = goodsTypeService;
            _customerService = customerService;
            _authenticationService = authenticationService;
        }


	    /// <summary>
	    /// 首页
	    /// </summary>
	    /// <returns></returns>
	    public ActionResult Index()
	    {
		    int NewArticleId = 5, CompanyId = 6;
		    var model = new HomeListModel();
		
            //最新通知 4,  //公司新闻 3
            model.ArticleModels.AddRange(_newService.GetArticleById(NewArticleId).ToModelAsCollection<Article, ArticleModel>());
	        model.ArticleModels.AddRange(_newService.GetArticleById(CompanyId).Take(3).ToModelAsCollection<Article, ArticleModel>());


            model.CountryList = GetCommonCountryList();
            ViewBag.CountryList = GetCountryList("");
            ViewBag.GoodsTypeList = GetGoodsTypeList();

		    //主页顶级信息
		    model.CategoryInfo = _newService.GetCategoryInfo("主页").ToModel<CategoryModel>();
			if (model.CategoryInfo!=null)
		    {
				ViewBag.Description = model.CategoryInfo.SeoDescription;
				ViewBag.SeoKey = model.CategoryInfo.SeoKeywords;
		    }

	        model.ShowCategoryListModel = ShowCategoryListModel();

		    return View(model);
        }



        //产品服务, 新闻中心, 关于我们, 帮助中心 顶部列表GetCategoryList
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






        [HttpPost]
		public ActionResult Index(HomeListModel viewModel)
        {

           string country = viewModel.FreightTrialFilter.CountryCode;

           string length = viewModel.FreightTrialFilter.Length.Replace("cm", "");
           string height = viewModel.FreightTrialFilter.Height.Replace("cm", "");
           string width = viewModel.FreightTrialFilter.Width.Replace("cm", "");

		   return RedirectToAction("FreightTrial", "Billing", new { countryCode = country, weight = viewModel.FreightTrialFilter.Weight, length = length, height = height, width = width,packageType = viewModel.FreightTrialFilter.PackageType });
			//return RedirectToAction("FreightTrial", "Billing", new { countryCode = viewModel.FreightTrialFilter.CountryCode, weight = viewModel.FreightTrialFilter.Weight, length = viewModel.FreightTrialFilter.Length, height = viewModel.FreightTrialFilter.Height, width = viewModel.FreightTrialFilter.Width, packageType = viewModel.FreightTrialFilter.PackageType });
        }

     

        /// <summary>
        ///  关于我们
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult About(int id)
        {
            if (_newService.GetCategory(id)==null)
            {
                throw new Exception("输入的参数有误");
            }

	        HomeListModel model=new HomeListModel();
            model.ShowCategoryListModel = ShowCategoryListModel();
            model.CategoryInfo = _newService.GetCategory(id).ToModel<CategoryModel>();
            model.GetId = id;

	        return View(model);
        }

        /// <summary>
        ///  联系我们
        /// </summary>
        /// <returns></returns>
        public ActionResult ContactUs(int? id)
        {
            if (id.Value != 4)
            {
                if (_newService.CategoryId(id.Value) != 4)
                {
                    throw new Exception("输入的参数有误");
                }
            }
            else if (id.Value == 0)
            {
                throw new Exception("输入的参数有误");
            }
            HomeListModel model = new HomeListModel();
            if (_newService.Category(id.Value))
            {
                //根据一级标题查询一个二级标题Id
                int categoryId = _newService.CategoryId(id.Value);
                model.CategoryId = categoryId;
                model.CategoryModels = _newService.GetCategoryById(id.Value).ToModelAsCollection<Category, CategoryModel>();
                model.CategoryModel = _newService.GetCategory(categoryId).ToModel<CategoryModel>();
            }
            else
            {
                model.CategoryId = id.Value;
                //根据二级标题去查询一级标题的Id
                model.CategoryModels = _newService.GetCategoryById(_newService.CategoryId(id.Value)).ToModelAsCollection<Category, CategoryModel>();
                model.CategoryModel = _newService.GetCategory(id.Value).ToModel<CategoryModel>();
            }
            return View(model);
        }

        /// <summary>
        /// 产品与服务
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductServices(int id)
        {
   
            if (_newService.GetCategory(id) == null)
            {
                throw new Exception("输入的参数有误!");
            }

            HomeListModel model = new HomeListModel();
            //获取当前类别信息
            model.CategoryInfo = _newService.GetCategory(id).ToModel<CategoryModel>();
            model.GetId = model.CategoryInfo.CategoryID;
            model.ShowCategoryListModel = ShowCategoryListModel();

            return View(model);
        }



		/// <summary>
		/// 帮助中心
		/// add by yungchu
		/// </summary>
		/// <returns></returns>
	    public ActionResult HelpCenter(int id)
	    {

            //获取帮助中心信息
            if (_newService.GetCategory(id) == null)
            {
                throw new Exception("输入的参数有误!");
            }

			HomeListModel model = new HomeListModel();
		    model.ShowCategoryListModel = ShowCategoryListModel();

		    model.CategoryInfo = _newService.GetCategory(id).ToModel<CategoryModel>();
            model.GetId = model.CategoryInfo.CategoryID;

			return View(model);
	    }




        public List<SelectListItem> GetCountryList(string keyword)
        {
            if (Cache.Get("CountryList") != null)
            {
                return Cache.Get("CountryList") as List<SelectListItem>;
            }
            else
            {
                var list = new List<SelectListItem> { new SelectListItem { Value = "", Text = "" } };
                _countryService.GetCountryList(keyword).ForEach(c => list.Add(new SelectListItem
                {
                    Value = c.CountryCode,
                    Text = string.Format("{0}|{1}", c.CountryCode, c.ChineseName)
                }));
                Cache.Add("CountryList",list);
                return list;
            }
        }
        

        public string GetCategoryDesc(string categoryDesc)
        {
            if (!string.IsNullOrWhiteSpace(categoryDesc))
            {
                int length = categoryDesc.Length < 100 ? categoryDesc.Length : 100;
                string str = WebTools.NoHTML(categoryDesc).Substring(0, length);
                return str.Length < 100 ? str : str + "...";
            }
            return "暂无内容";
        }

       
        /// <summary>
        /// 验证用户登录信息
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="pwd">密码</param>
        /// <param name="remember">是否记住用户信息</param>
        /// <returns>JSON</returns>
        [HttpPost]
        public JsonResult ValidateLogOn(string userName, string pwd, bool remember)
        {
           
            var result = new ResponseResult();
            if (string.IsNullOrWhiteSpace(userName))
            {
                result.Result = false;
                result.Message = "登录用户名不能为空";
            }
            if (string.IsNullOrWhiteSpace(pwd))
            {
                result.Result = false;
                result.Message = "登录密码不能为空";
            }

            try
            {
                var info = _customerService.Login(userName.Trim(), pwd.Trim());
                if (null != info)
                {
                    if (info.Status == (int) Customer.StatusEnum.Disable)
                    {
                        result.Result = false;
                        result.Message = "您的帐户已被禁用";
                    }
                    else if (info.Status == (int)Customer.StatusEnum.Unaudited)
                    {
                        result.Result = false;
                        result.Message = "您的帐户还在审核中";
                    }
                    else
                    {
                        _authenticationService.SignIn(new UserData
                        {
                            UserName = info.CustomerCode
                        }, remember);
                        result.Result = true;

	                    if (remember)
	                    {
		                    //记住用户名和密码
							result.Message = userName.Trim()+ "," + pwd.Trim();
	                    }

                    }
                   
                }
                else
                {
                    result.Result = false;
                    result.Message = "用户名和密码错误";
                }
                
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                result.Result = false;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Logout()
        {
            _authenticationService.SignOut();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 获取国家列表包含常用国家
        /// </summary>
        /// <returns></returns>
        public List<CountryExt> GetCommonCountryList()
        {
            if (Cache.Get("CommonCountryList") != null)
                return Cache.Get("CommonCountryList") as List<CountryExt>;
            else
            {
                var list = _countryService.GetCommonCountryList();
                if (list.Count > 0)
                {
                    list.ForEach(p=>p.CountryPinyin=PinyinHelper.GetShortPinyin(p.ChineseName));
                }
                Cache.Add("CommonCountryList", list);
                return list;
            }
            
        }


        public List<SelectListItem> GetGoodsTypeList()
        {
            if (Cache.Get("GoodsTypeList") != null)
            {
                return Cache.Get("GoodsTypeList") as List<SelectListItem>;
            }
            else
            {
                var list = new List<SelectListItem>(){ };
        
                _goodsTypeService.GetList().ForEach(c => list.Add(new SelectListItem
                {
                    Value = c.GoodsTypeID.ToString(CultureInfo.InvariantCulture),
                    Text = c.GoodsTypeName
                }));
                Cache.Add("GoodsTypeList", list);
                return list;
            }
        }

        public string GetUserName()
        {
            return _workContext.User != null ? _customerService.GetCustomer(_workContext.User.UserUame).AccountID : "";
        }

        public List<CategoryModel> GetCategoryList()
        {
            return _newService.GetCategoryById(3).ToModelAsCollection<Category, CategoryModel>();
        }
        //获取列表信息
        public List<CategoryModel> GetCategoryModel(int flag)
        {
            return _newService.GetCategoryById(flag).ToModelAsCollection<Category, CategoryModel>();
        }
    }


}
