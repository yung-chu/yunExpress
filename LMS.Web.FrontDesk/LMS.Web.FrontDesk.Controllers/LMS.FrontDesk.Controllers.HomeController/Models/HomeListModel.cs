using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LMS.Core;

namespace LMS.FrontDesk.Controllers.HomeController.Models
{
    /// <summary>
    /// 新闻中心
    /// </summary>
    public class HomeListModel
    {
        public HomeListModel()
        {
            ArticleModels=new List<ArticleModel>();
            CategoryModels=new List<CategoryModel>();
            NewsPartialModels=new List<NewsPartialModel>();
            ArticleModel=new ArticleModel();
            FreightTrialFilter = new FreightTrialFilterModel();
            UserInfoFilter = new UserInfoFilterModel();
            CountryList=new List<CountryExt>();
            CategoryModel=new CategoryModel();
            CategoryImg=new CategoryModel();
	        CategoryInfo = new CategoryModel();
            //CategoryModelAbout = new List<CategoryModel>();
            //ShippingMethodServices = new List<CategoryModel>();
            //CategoryModelNews = new List<CategoryModel>();
            //HelpCenterList = new List<CategoryModel>();
            ShowCategoryListModel = new ShowCategoryListModel();
        }

	
	    public int GetId { get; set; }

	    public List<ArticleModel> ArticleModels { get; set; }
        public List<CategoryModel> CategoryModels { get; set; }
        //public List<CategoryModel> CategoryModelAbout { get; set; }
        //public List<CategoryModel> ShippingMethodServices { get; set; }
        //public List<CategoryModel> CategoryModelNews { get; set; }
        //public List<CategoryModel> HelpCenterList { get; set; }
        //产品服务, 新闻中心, 关于我们, 帮助中心 顶部列表
        public ShowCategoryListModel ShowCategoryListModel { get; set; }

        public List<NewsPartialModel> NewsPartialModels { get; set; }

        public ArticleModel ArticleModel { get; set; }
        public CategoryModel CategoryModel { get; set; }
        public CategoryModel CategoryImg { get; set; }
		public CategoryModel CategoryInfo { get; set; }
        public FreightTrialFilterModel FreightTrialFilter { get; set; }
        public string TrackingNumber { get; set; }

        public UserInfoFilterModel UserInfoFilter { get; set; }

        public List<CountryExt> CountryList { get; set; }

        public int CategoryId { get; set; }

        public string UserCenterPath {
            get { return sysConfig.UserCenterPath; }
        }
        public string PicPath {
            get { return sysConfig.UploadWebPath; }
        }
    }

    public class FreightTrialFilterModel
    {
        public FreightTrialFilterModel()
        {
            PackageType = 1;
            PackageTypeName = "包裹";
        }

        public string Length { get; set; }

        public string Width { get; set; }

        public string Height { get; set; }

        public decimal? Weight { get; set; }

        public string CountryCode { get; set; }

        public string ChineseName { get; set; }

        public int PackageType { get; set; }

        public string PackageTypeName { get; set; }
    }

    public class UserInfoFilterModel
    {
        public string UserName { get; set; }

        public string Pwd { get; set; }

    }
}
