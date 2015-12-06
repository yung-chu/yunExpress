using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using LighTake.Infrastructure.Common.BizLogging;
using LighTake.Infrastructure.Common.BizLogging.Enums;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Services.HomeServices;
using LMS.Services.NewServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Web.Controllers;
using LMS.Services.OperateLogServices;

namespace LMS.Controllers.NewsController
{
    public class NewsController : BaseController
    {
        private readonly INewService _newService;
        private readonly IHomeService _homeService;
        private readonly IWorkContext _workContext;
		private IOperateLogServices _operateLogServices;


		public NewsController(INewService newService, IHomeService homeService, IWorkContext workContext,IOperateLogServices operateLogServices)
        {
            _newService=newService;
            _homeService = homeService;
            _workContext = workContext;
			_operateLogServices = operateLogServices;
        }

        public ActionResult News()
        {
            return View();
        }

        public ActionResult NewsAdd()
        {
            return View();
        }

        public ActionResult NewsEdit(string articleId)
        {
            if (articleId.IsNullOrEmpty())
                return View();

            var model = _newService.GetArticle(int.Parse(articleId));

            ArticleModel viewModel =null;
            if (model != null)
            {
                viewModel = new ArticleModel();
                model.CopyTo(viewModel);
                var category = _newService.GetCategory(model.CategoryID);
                viewModel.CategoryName = category != null ? category.Name : "";

            }
            return View(viewModel);
        }

        //新闻列表
        public string GetNewsList(FormCollection from)
        {
            var param = JsonHelper.JsonDeserialize<ArticleFilterModel>(from["params"]);
            int count = 0;
            var list = _newService.GetNewsPageList(new ArticleParam
            {
                CategoryID = param.CategoryID,
                Title = param.Title,
                PublishedTime = param.PublishedTime,
                PublishedEndTime = param.PublishedEndTime,
                Page = param.Page,
                PageSize = param.PageSize
            },out count);
            return JsonHelper.CreateJsonParameters(list.InnerList, true, count);
        }

        //添加
        [ValidateInput(false)]
        public string AddNews(ArticleModel model)
        {
            model.CreatedBy =_workContext.User==null?"": _workContext.User.UserUame;
            model.CreatedOn = DateTime.Now;
            return _newService.AddNews(model.ToEntity<Article>());
        }

        //如果该父节点下有子节点，而且父节点ParentID为0就不允许添加问题
        public string IsParent(string helpCategoryID)
        {
            var entity = _homeService.GetCategory(int.Parse(helpCategoryID));
            var list = _homeService.GetCategoryList(int.Parse(helpCategoryID));
            return list.Count > 0 && entity.ParentID == 0 ? "0" : "1";
        }

        //编辑新闻
        [ValidateInput(false)]
        [HttpPost]
        public string NewsEdit(ArticleModel model)
        {

			//#region 操作日志
			////yungchu
			////敏感字-无
			//BizLog bizlog = new BizLog()
			//{
			//	Summary = "新闻编辑",
			//	KeywordType = KeywordType.ArticleId,
			//	Keyword = model.ArticleID.ToString(),
			//	UserCode = _workContext.User.UserUame,
			//	UserRealName = _workContext.User.UserUame,
			//	UserType = UserType.LMS_User,
			//	SystemCode = SystemType.LMS,
			//	ModuleName = "新闻管理"
			//};

			//_operateLogServices.WriteLog(bizlog, model);
			//#endregion

            model.LastUpdatedBy = _workContext.User == null ? "" : _workContext.User.UserUame;
            model.LastUpdatedOn = DateTime.Now;
            return _newService.UpdateNews(model.ToEntity<Article>());
        }

        //删除新闻
        public string DelNews(string articleId)
        {
			//#region 操作日志
			////yungchu
			////敏感字-无
			//BizLog bizlog = new BizLog()
			//{
			//	Summary = "新闻删除",
			//	KeywordType = KeywordType.ArticleId,
			//	Keyword = articleId,
			//	UserCode = _workContext.User.UserUame,
			//	UserRealName = _workContext.User.UserUame,
			//	UserType = UserType.LMS_User,
			//	SystemCode = SystemType.LMS,
			//	ModuleName = "新闻管理"
			//};

			//_operateLogServices.WriteLog(bizlog, _newService.Get(int.Parse(articleId)));
			//#endregion

            return _newService.DelNews(int.Parse(articleId));
        }

    }
}
