using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using LMS.Core;
using LMS.Data.Entity;
using LMS.FrontDesk.Controllers.BillingController.Models;
using LMS.Services.CommonServices;
using LMS.Services.CountryServices;
using LMS.Services.CustomerServices;
using LMS.Services.FreightServices;
using LMS.Services.NewServices;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Web.Controllers;

namespace LMS.FrontDesk.Controllers.BillingController
{
    public class BillingController : BaseController
    {
        private readonly IWorkContext _workContext;
        private readonly ICountryService _countryService;
        private readonly IGoodsTypeService _goodsTypeService;
        private readonly IFreightService _freightService;
        private readonly ICustomerService _customerService;
        private readonly INewService _newService;

        public BillingController(IWorkContext workContext, ICountryService countryService,INewService newService,
                                 IGoodsTypeService goodsTypeService,IFreightService freightService,ICustomerService customerService)
        {
            _workContext = workContext;
            _countryService = countryService;
            _goodsTypeService = goodsTypeService;
            _freightService = freightService;
            _customerService = customerService;
            _newService = newService;
        }


        public ActionResult FreightTrial(string countryCode, decimal? weight, string length, string height, string width, int packageType = 0)
        {

			FreightTrialFilterModel filter = new FreightTrialFilterModel { PackageType = packageType, CountryCode = countryCode, Weight = weight, Length = length, Width = width, Height = height};

            return View(BindData(filter));
        }

        [HttpPost]
        public ActionResult FreightTrial(FreightTrialFilterModel filter)
        {
            return View(BindData(filter));
        }

        public FreightTrialViewModels BindData(FreightTrialFilterModel filter)
        {


            ViewBag.CountryList = GetCountryList("");
            ViewBag.GoodsTypeList = GetGoodsTypeList();
            var viewModels = new FreightTrialViewModels { Filter = filter };
         
            try
            {
                viewModels = GetFreightList(filter);
            }
            catch (Exception e)
            {
                ErrorNotification(e.Message);
            }

            viewModels.ShowCategoryListModel = ShowCategoryListModel();

            return viewModels;
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







        private FreightTrialViewModels GetFreightList(FreightTrialFilterModel filter)
        {
            var viewModels = new FreightTrialViewModels {Filter = filter};

            if (!filter.Weight.HasValue &&
                (!string.IsNullOrWhiteSpace(filter.Length) && !string.IsNullOrWhiteSpace(filter.Width) && !string.IsNullOrWhiteSpace(filter.Height)))
            {
                throw new Exception("重量或长宽高必填其中一项");
            }

            if (string.IsNullOrWhiteSpace(filter.CountryCode))
            {
                throw new Exception("请选择发货国家");
            }
            string customerCode = "";
            Guid customerId = Guid.Empty;
            int customerTypeId = 12;//默认12
            if (null != _workContext.User)
            {
                customerCode = _workContext.User.UserUame;
                var customer = _customerService.GetCustomer(customerCode);
                if (customer != null && customer.CustomerTypeID.HasValue)
                {
                    customerTypeId = customer.CustomerTypeID.Value;
                    customerId = customer.CustomerID;
                }
            }
            else
            {
                customerCode = _customerService.GetCustomerCode(customerTypeId);
            }
            var list = _freightService.GetCustomerShippingPrices(new FreightPackageModel()
            {
                Weight = filter.Weight ?? 0,
                Length =  Convert.ToInt32(string.IsNullOrEmpty(filter.Length) ? "0" : filter.Length),
                Width = Convert.ToInt32(string.IsNullOrEmpty(filter.Width) ? "0" : filter.Width),
                Height =Convert.ToInt32(string.IsNullOrEmpty(filter.Height) ? "0" : filter.Height),
                CountryCode = filter.CountryCode,
                ShippingTypeId = filter.PackageType,
                CustomerTypeId = customerTypeId,
                CustomerId = customerId,
                EnableTariffPrepay = false,
            });

            var shippingList = _freightService.GetShippingMethodListByCustomerCode(customerCode, true);
            foreach (var item in list)
            {
                if (!item.CanShipping) continue;
                if (item.ShippingMethodId == null) throw new Exception(string.Format("没有运输方式"));
                var shippingMethod =
                    shippingList.Find(
                        s => s.ShippingMethodId == item.ShippingMethodId.Value);
                if (shippingMethod == null) throw new Exception(string.Format("运输方式【{0}】不存在", item.ShippingMethodId.Value));

                viewModels.FreightList.Add(new FreightModel
                {
                    ShippingMethodName = shippingList.First(s => item.ShippingMethodId != null && s.ShippingMethodId == item.ShippingMethodId.Value).ShippingMethodName,
                    Weight = item.Weight,
                    ShippingFee = item.ShippingFee,
                    RegistrationFee = item.RegistrationFee,
                    RemoteAreaFee = item.RemoteAreaFee,
                    FuelFee = item.FuelFee,
                    OtherFee = item.Value - (item.ShippingFee + item.RegistrationFee + item.RemoteAreaFee + item.FuelFee),
                    DeliveryTime = item.DeliveryTime,
                    Remarks = item.Remark,
                    TariffPrepayFee = item.TariffPrepayFee,
                });
            }
            viewModels.FreightList = viewModels.FreightList.OrderBy(o => o.TotalFee).ToList();
            return viewModels;
        }

        public List<SelectListItem> GetCountryList(string keyword)
        {
            var list = new List<SelectListItem> { new SelectListItem { Value = "", Text = "" } };
            _countryService.GetCountryList(keyword).ForEach(c => list.Add(new SelectListItem
            {
                Value = c.CountryCode,
				Text = c.ChineseName + " " + c.CountryCode + " " + c.Name.Replace(" ", "")
            }));

            return list;
        }

        public List<SelectListItem> GetGoodsTypeList()
        {
            var list = new List<SelectListItem> { new SelectListItem { Value = "", Text = "" } };
            _goodsTypeService.GetList().ForEach(c => list.Add(new SelectListItem
            {
                Value = c.GoodsTypeID.ToString(CultureInfo.InvariantCulture),
                Text = c.GoodsTypeName
            }));

            return list;
        }

		//获取列表信息
		public List<CategoryModel> GetCategoryModel(int flag)
		{
			return _newService.GetCategoryById(flag).ToModelAsCollection<Category, CategoryModel>();
		}

        //获取列表信息
        public List<CategoryModel> GetCategoryList()
        {
            return _newService.GetCategoryById(3).ToModelAsCollection<Category, CategoryModel>();
        }
    }
}
