using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using LighTake.Infrastructure.Common.BizLogging;
using LighTake.Infrastructure.Common.BizLogging.Enums;
using LMS.Core;
using LMS.Services.HomeServices;
using LMS.Services.NewServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Utities;
using LighTake.Infrastructure.Web.Controllers;
using LighTake.Infrastructure.Seedwork;
using LMS.Data.Entity;
using LMS.Models;
using LighTake.Infrastructure.Web.Filters;
using LMS.Services.OperateLogServices;

namespace LMS.Controllers.WebsiteController
{
    public class WebsiteController : BaseController
    {
        private readonly IHomeService _homeService;
        private readonly INewService _newService;
		private IOperateLogServices _operateLogServices;
	    private IWorkContext _workContext;

		public WebsiteController(IHomeService homeService, INewService newService, IOperateLogServices operateLogServices, IWorkContext workContext)
        {
            _homeService = homeService;
            _newService = newService;
			_operateLogServices = operateLogServices;
			_workContext = workContext;
        }

        #region 分类管理


        public ActionResult CategoryList()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public string GetCategoryList(int? parentid)
        {
            int parentId = parentid == null ? 0 : parentid.Value;
            var cateList = GetChildCategorys(parentId);
            var cates = cateList.ToModelAsCollection<Category, CategoryModel>();
            foreach (var categoryModel in cates)
            {
                categoryModel.SeoTitle = categoryModel.SeoKeywords = categoryModel.SeoDescription = categoryModel.Description = string.Empty;
            }
            cates.ForEach(x => x._parentId = x.ParentID > 0 ? x.ParentID.ToString() : string.Empty);
            int count = cates.Count;
            foreach (var categoryModel in cates)
            {
                var clst = cates.Where(x => x.ParentID == categoryModel.ParentID);
                var minSort = clst.Min(x => x.Sort);
                var maxSort = clst.Max(x => x.Sort);
                categoryModel.IsFirst = categoryModel.Sort == minSort;
                categoryModel.IsLast = categoryModel.Sort == maxSort;
            }
            return JsonHelper.CreateJsonParameters(cates, true, count);
        }

        private List<Category> GetChildCategorys(int parentId)
        {
            var cates = _homeService.GetAllChildCategoryList(parentId);

            return cates;
        }


        public string SetCategorySort(int? categoryid, int? type)
        {
            if (categoryid == null)
                return "0";
            int cateId = categoryid.Value;
            var sortType = type == null || type == 1;
            return _homeService.SetCategorySort(cateId, sortType) ? "1" : "0";
        }

        /// <summary>
        /// 修改分类
        /// </summary>
        /// <param name="categoryid"></param>
        /// <returns></returns>
        public ActionResult EditCategory(int? categoryid)
        {
            Category category = null;
            if (categoryid == null)
            {
                return Redirect(Url.Action("CategoryList"));
            }
            int cateId = categoryid.Value;
            category = _homeService.GetCategory(cateId);
            return View(category);
        }


        /// <summary>
        /// 修改分类
        /// </summary>
        /// <param name="categoryid"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditNewCategory(Category categoryModel)
        {
            var status = UpdateCategory(categoryModel, false);
            return Content(status ? "1" : "0");
        }

        public string HasEnableChild(int? categoryid)
        {
            int catId = categoryid.IsValueNullOrWhiteSpace() ? 0 : categoryid.Value;
            if (catId <= 0)
            {
                return "0";
            }
            return _homeService.HasEnableChild(catId) ? "1" : "0";

        }


        public ActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddNewCategory(Category categoryModel)
        {
            var status = UpdateCategory(categoryModel, true);
            return Content(status ? "1" : "0");
        }

        private bool UpdateCategory(Category categoryModel, bool add)
        {
            if (add)
            {
                categoryModel.CreatedOn = DateTime.Now;
                categoryModel.Status = 1;
            }
            else
            {
                categoryModel.LastUpdatedOn = DateTime.Now;

				//#region 操作日志
				////yungchu
				////敏感字-无
				//BizLog bizlog = new BizLog()
				//{
				//	Summary = "网站类别编辑",
				//	KeywordType = KeywordType.CategoryId,
				//	Keyword = categoryModel.CategoryID.ToString(),
				//	UserCode = _workContext.User.UserUame,
				//	UserRealName = _workContext.User.UserUame,
				//	UserType = UserType.LMS_User,
				//	SystemCode = SystemType.LMS,
				//	ModuleName = "网站类别管理"
				//};

				//_operateLogServices.WriteLog(bizlog, _homeService.GetCategory(categoryModel.CategoryID));
				//#endregion

            }

            string filePath = sysConfig.UploadPath;
            string fileName = SaveFileToService(filePath);
            if (!string.IsNullOrWhiteSpace(fileName))
                categoryModel.Pic = fileName;

            var status = add ? _homeService.AddCategory(categoryModel) : _homeService.UpdateCategory(categoryModel) == 1;
            return status;
        }

        /// <summary>
        /// 保存文件到服务器上
        /// </summary>
        /// <returns>返回当前上传成功后的文件名</returns>
        private string SaveFileToService(string filePath)
        {
            string tempName = string.Empty;
            try
            {
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                HttpFileCollectionBase files = HttpContext.Request.Files;
                for (int iFile = 0; iFile < files.Count; iFile++)
                {
                    HttpPostedFileBase postedFile = files[iFile];
                    tempName = Path.GetFileName(postedFile.FileName);

                    if (String.IsNullOrWhiteSpace(tempName))
                        return String.Empty;
                    string fileExtension = Path.GetExtension(tempName) ?? string.Empty;
                    if (".jpg.jpeg.gif.png.bmp".IndexOf(fileExtension.ToLower()) < 0)
                        throw new Exception("只能上传.jpg .jpeg .gif .png .bmp格式图片！");
                    if (!string.IsNullOrEmpty(tempName))
                    {
                        tempName = DateTime.Now.ToString("yyyyMMddHHmmss") + fileExtension;
                        postedFile.SaveAs(filePath + tempName);
                    }
                }
                return tempName;
            }
            catch (Exception ex)
            {
                return tempName;
            }
        }

        public string GetAllCategorySelect()
        {
            var list = _homeService.GetAllChildCategoryList(0);
            var treelist = new List<TreeNode>();
            treelist.Add(new TreeNode("0", "", "顶级分类"));

            list.ForEach(l => treelist.Add(new TreeNode(l.CategoryID.ToString(), l.ParentID.ToString(), Tools.FilterAllHtml(l.Name).Replace("\"", ""))));
            var treeHelper = new TreeNodeHelper();
            var tree = treeHelper.GenerateTreeRoot(treelist);

            return tree.ToJsonTreeString();
        }

        public string DeleteCategory(int? categoryid)
        {
            string res = "0";

	        if (categoryid == null)
	        {
		        return res;
	        }

			//#region 操作日志
			////yungchu
			////敏感字-无
			//BizLog bizlog = new BizLog()
			//{
			//	Summary = "类别删除",
			//	KeywordType = KeywordType.CategoryId,
			//	Keyword = categoryid.Value.ToString(),
			//	UserCode = _workContext.User.UserUame,
			//	UserRealName = _workContext.User.UserUame,
			//	UserType = UserType.LMS_User,
			//	SystemCode = SystemType.LMS,
			//	ModuleName = "网站类别管理"
			//};

			//_operateLogServices.WriteLog(bizlog, _homeService.GetCategory(categoryid.Value));
			//#endregion


	        int catId = categoryid.Value;
            if (_homeService.DeleteCategory(catId) == 2)
            {
                res = "1";
            }
            return res;
        }


        #endregion

        #region 文章管理

        public ActionResult AddArticle()
        {
            return View();
        }

        [HttpPost]
        [FormValueRequired("btnsave")]
        public string AddArticle(Article article)
        {
            var status = _newService.AddArticle(article);
            return status ? "1" : "0";
        }


        //public ActionResult ArticleList(ArticleParam param)
        //{
        //    var list = _newService.GetArticleList(param);

        //    return View(list);
        //}

        //public ActionResult DeleteArticle()
        //{
        //    return View();
        //}

        #endregion


    }
}
