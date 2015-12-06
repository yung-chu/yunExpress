using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LMS.Core;
using LighTake.Infrastructure.Common;


namespace LMS.FrontDesk.Controllers.NewController.Models
{
    public class NewListModel
    {
        public NewListModel()
        {
            ArticleModels=new List<ArticleModel>();
            CategoryModels=new List<CategoryModel>();
            ArticleModel=new ArticleModel();
            CategoryModel=new CategoryModel();
	        CategoryInfo = new CategoryModel();
            PagedList = new PagedList<ArticleModel>();
            Param = new WebsiteArticleParam();
            GetArticleList = new List<ArticleModel>();
           // ShippingMethodServices=new List<CategoryModel>();
			//CategoryModelAbout=new List<CategoryModel>();
			//CategoryModelNews=new List<CategoryModel>();
			//HelpCenterList=new List<CategoryModel>();
            ShowCategoryListModel = new ShowCategoryListModel();
        }
        //news details
	    public int GetId { get; set; }
	    public int ArticleId { get; set; }
        public int CategoryChakedId { get; set; }
        public List<ArticleModel> ArticleModels { get; set; }
        public List<CategoryModel> CategoryModels { get; set; }
       // public List<CategoryModel> ShippingMethodServices { get; set; }
		//public List<CategoryModel> CategoryModelAbout { get; set; }
		//public List<CategoryModel> CategoryModelNews { get; set; }
	   // public List<CategoryModel> HelpCenterList { get; set; }
	    public IPagedList<ArticleModel> PagedList { get; set; }
        public ArticleModel ArticleModel { get; set; }
        public List<ArticleModel> GetArticleList { get; set; }

        public CategoryModel CategoryModel { get; set; }
		public CategoryModel CategoryInfo { get; set; }
        public WebsiteArticleParam Param { get; set; }

        public int? ArticleUpPageId { get; set; }
        public int? ArticleDownPageId { get; set; }

        public string PicPath {
            get { return sysConfig.UploadWebPath; }
        }

        public int CategoryId { get; set; }

        public ShowCategoryListModel ShowCategoryListModel { get; set; }

    }
}
