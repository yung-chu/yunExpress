using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using LighTake.Infrastructure.Common.Caching;
using LMS.Data.Entity;
using LMS.Services.CountryServices;
using LMS.Services.FreightServices;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Web.Controllers;
using LMS.Services.NewServices;


namespace LMS.FrontDesk.Controllers.RemoteAddressController
{
	public class RemoteAddressController : BaseController
    {
		private readonly IFreightService _freightService;
		private readonly INewService _newService;
		private readonly ICountryService _countryService;

		public RemoteAddressController(IFreightService freightService, INewService newService, ICountryService countryService)
		{
			_freightService = freightService;
			_newService = newService;
			_countryService = countryService;
		}


		public ActionResult Search(RemoteAreaAddressParam param)
		{
			return View(DataBind(param));
		}

		[ActionName("Search")]
		[System.Web.Mvc.HttpPost]
		public ActionResult SearchInfo(ViewModel model)
		{
			return View(DataBind(model.FilterModel));
		}


		public ViewModel DataBind(RemoteAreaAddressParam param)
		{

			var list = _freightService.GetPagedListRemoteAreaAddress(new RemoteAreaAddressParam
			{

				Page=param.Page,
				PageSize=param.PageSize,
				ShippingMethodId = param.ShippingMethodId,
				CountryCode = param.CountryCode,
				State =!string.IsNullOrEmpty(param.State)?  param.State.Trim():null,
				City =!string.IsNullOrEmpty(param.City)? param.City.Trim():null,
				Zip = param.Zip
			});

			var pageCount = 0;
			var totalCount = 0;

			if (list!=null&&list.Any())
			{
				var remoteAreaAddressExt = list.LastOrDefault();
				if (remoteAreaAddressExt != null) 
					pageCount = remoteAreaAddressExt.PageInfoModel.PageCount;
				var areaAddressExt = list.LastOrDefault();
				if (areaAddressExt != null) 
					totalCount = areaAddressExt.PageInfoModel.TotalCount;

				list.RemoveAt(list.Count - 1);//去除最后一条
			}

			//构建分页强类型
			var PagedList = new PagedList<RemoteAreaAddressExt>()
			{
				PageIndex = param.Page,
				PageSize=param.PageSize,
				TotalCount = totalCount,
				TotalPages = pageCount,
				InnerList = list
			};

			var model = new ViewModel
			{
				FilterModel = param,
				PagedList = PagedList,
			};



			//运输方式下拉框
			model.ShippingMethodLists = GetShippingMethodSelectList();
			model.Countrylists = GetCountrySelectList();

			//显示国家全称
			List<Country> listCountry = _countryService.GetCountryList("");
			model.PagedList.InnerList.Each(
				p =>
				{
					if (!string.IsNullOrEmpty(p.CountryCode))
					{
						var getCountry = listCountry.Find(a => a.CountryCode.ToUpperInvariant().Contains(p.CountryCode.ToUpperInvariant()));
						if (getCountry!=null)
						{
							p.EName = getCountry.Name;
						}
					}
				});


			//显示运输方式名称
			var listShippingMethodList = GetShippingMethodSelectList();
			model.PagedList.InnerList.Each(
			p =>
			{

				var getShippingMethod = listShippingMethodList.Find(a => a.Value == p.ShippingMethodId.ToString(CultureInfo.InvariantCulture));
				if (getShippingMethod!=null)
				{
					p.ShippingMethodName = getShippingMethod.Text;
				}

			});


		    model.ShowCategoryListModel = ShowCategoryListModel();

			return model;
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
            model.HelpCenterList = _newService.GetCategoryById(getCategory.CategoryID).ToModelAsCollection<Category, CategoryModel>();

            return model;
        }


		//获取列表信息
		public List<CategoryModel> GetCategoryList()
		{
			return _newService.GetCategoryById(3).ToModelAsCollection<Category, CategoryModel>();
		}

		public List<CategoryModel> GetCategoryModel(int flag)
		{
			return _newService.GetCategoryById(flag).ToModelAsCollection<Category, CategoryModel>();
		}
		public List<SelectListItem> GetShippingMethodSelectList()
		{
			List<SelectListItem> selectListItems = new List<SelectListItem>();
			selectListItems.Add(new SelectListItem()
			{
				Text = "请选择",
				Value = ""
			});
			_freightService.GetShippingMethodListByCustomerTypeId(null, false)
						   .ForEach(p => selectListItems.Add(new SelectListItem()
						   {
							   Text = p.ShippingMethodName,
							   Value = p.ShippingMethodId.ToString()
						   }));
			return selectListItems;
		}

		public List<SelectListItem> GetCountrySelectList()
		{
			List<SelectListItem> selectListItems = new List<SelectListItem>();
			selectListItems.Add(new SelectListItem()
			{
				Text = "请选择",
				Value = ""
			});

			_countryService.GetCountryList("").ForEach(p => selectListItems.Add(new SelectListItem()
			{
				Text = p.CountryCode+" |"+p.Name,
				Value = p.CountryCode
			}));
			return selectListItems;
		}


    }
}
