using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Services.NewServices
{
    public interface INewService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns>文章分页列表</returns>
        IPagedList<Article> GetArticleList(ArticleParam param);

        IPagedList<Article> GetArticleList(WebsiteArticleParam param);
        

        Article GetArticle(int articleId);
        IEnumerable<Category> GetCategoryById(int id);


		//根据类别名获取信息 yungchu
		Category GetCategoryInfo(string categoryName);


        IEnumerable<Article> GetArticleById(int id);

        List<Article> GetArticleListByCategoryId(int categoryID);

        /// <summary>
        /// 根据类别名称获取类别描述
        /// </summary>
        /// <param name="categoryName">类别名称</param>
        /// <returns>类别描述</returns>
        string GetCategoryDesc(string categoryName);

        /// <summary>
        /// 首次加载的时候显示一个默认的类表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int CategoryId(int id);

        /// <summary>
        /// 判断是否是从一级分类进入的
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        bool Category(int categoryId);

        bool AddArticle(Article article);
        bool UpdateArticle(Article article);
        bool DeleteArticle(int articleId);

        string AddNews(Article entity);

        string UpdateNews(Article entity);

        string DelNews(int articleId);

        IPagedList<ArticleExt> GetNewsPageList(ArticleParam param, out int totalCount);

        Category GetCategory(int id);
        

        Article GetArticleByCategoryId(int categoryId);
        IEnumerable<Article> GetArticleByBigCategory();
        Article Get(int id);
    }
}
