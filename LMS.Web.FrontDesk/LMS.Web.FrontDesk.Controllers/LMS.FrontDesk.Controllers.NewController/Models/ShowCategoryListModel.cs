using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.FrontDesk.Controllers.NewController.Models
{
    public class ShowCategoryListModel
    {
        public ShowCategoryListModel()
        {
            ShippingMethodServices = new List<CategoryModel>();
            CategoryModelNews = new List<CategoryModel>();
            HelpCenterList = new List<CategoryModel>();
            CategoryModelAbout = new List<CategoryModel>();
        }
        //获取产品与服务列表
        public List<CategoryModel> ShippingMethodServices { get; set; }
        //新闻中心
        public List<CategoryModel> CategoryModelNews { get; set; }
        //帮助中心列表
        public List<CategoryModel> HelpCenterList { get; set; }
        //关于我们
        public List<CategoryModel> CategoryModelAbout { get; set; }
    }
}
