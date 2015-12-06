using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using LMS.Data.Entity;
using LMS.FrontDesk.Controllers.NewController.Models;
using LMS.Services.NewServices;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Web.Controllers;

namespace LMS.FrontDesk.Controllers.NewController
{
    public class NewController:BaseController
    {
        private INewService _newService;


        public NewController(INewService newService)
        {
            _newService = newService;
        }

       
        /// <summary>
        /// 新闻中心
        /// Add by zhengsong
        /// Update By xiaofan
        /// </summary>
        /// <returns></returns>
        public ActionResult NewList(int categoryId)
        {

            if (_newService.GetArticleListByCategoryId(categoryId) == null)
            {
                throw new Exception("输入的参数有误");
            }

            NewListModel model = new NewListModel();
            model.ShowCategoryListModel = ShowCategoryListModel();
           //获取当前类别列表信息
            model.CategoryInfo = _newService.GetCategory(categoryId).ToModel<CategoryModel>();
            model.GetArticleList = _newService.GetArticleListByCategoryId(categoryId).ToModelAsCollection<Article, ArticleModel>();
            model.GetId = categoryId;


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
            model.HelpCenterList = _newService.GetCategoryById(getCategory.CategoryID).ToModelAsCollection<Category, CategoryModel>();
         
            return model;
        }





        /// <summary>
        /// 新闻详情页
        /// Add by ZhengSong 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult NewDetailed(int id)
        {
            if (_newService.Get(id) == null)
            {
                throw new Exception("输入参数有误！");
            }
            NewListModel model = new NewListModel();
            model.ShowCategoryListModel = ShowCategoryListModel();
            model.ArticleModel = _newService.Get(id).ToModel<ArticleModel>();
          
            return View(model);
        }







		public List<CategoryModel> GetCategoryList()
		{
			return _newService.GetCategoryById(3).ToModelAsCollection<Category, CategoryModel>();
		}
    }


	public class CheckHtml
	{
		/// <summary>
		////过滤HTML代码(取新闻时)
		/// add by yungchu
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		public  string ParseTags(string HTMLStr)
		{
			return System.Text.RegularExpressions.Regex.Replace(HTMLStr, "<[^>]*>", "").Replace("&nbsp","").Replace(";","")
                .Replace(",","").Replace("。","");
	    }

	}
}
