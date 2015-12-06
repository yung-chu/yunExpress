using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LighTake.Infrastructure.Common;
using System.Linq.Dynamic;

namespace LMS.Services.NewServices
{
    public class NewService : INewService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICustomerRepository _customerRepository;
        

        public NewService(IArticleRepository articleRepository,
                          ICategoryRepository categoryRepository,
                          ICustomerRepository customerRepository)
        {
            _articleRepository = articleRepository;
            _categoryRepository = categoryRepository;
            _customerRepository = customerRepository;
        }

        public IPagedList<Article> GetArticleList(ArticleParam param)
        {
            if (param == null)
            {
                param = new ArticleParam();
            }
            Func<IQueryable<Article>, IOrderedQueryable<Article>> orderBy = x => x.OrderByDescending(x2 => x2.ArticleID);

            Expression<Func<Article, bool>> filter = x => true;
            filter = filter.AndIf(x => x.Title.Contains(param.Title), !string.IsNullOrWhiteSpace(param.Title))
                           .AndIf(x => x.CategoryID == param.CategoryID, param.CategoryID != null)
                           .AndIf(x => x.Author == param.Author, !string.IsNullOrWhiteSpace(param.Author))
                           .AndIf(x => x.Source.Contains(param.Source), !string.IsNullOrWhiteSpace(param.Source))
                           .AndIf(x => x.PublishedTime >= param.StartPublishedTime, param.StartPublishedTime != null)
                           .AndIf(x => x.PublishedTime <= param.EndPublishedTime, param.EndPublishedTime != null)
                           .AndIf(x => x.Status == param.Status, param.Status != null);

            IPagedList<Article> articles = _articleRepository.FindPagedList(param, filter, orderBy);

            return articles;
        }

        public IPagedList<Article> GetArticleList(WebsiteArticleParam param)
        {
            if (param == null)
            {
                param = new WebsiteArticleParam();
            }
            Func<IQueryable<Article>, IOrderedQueryable<Article>> orderBy = x => x.OrderByDescending(x2 => x2.ArticleID);

            Expression<Func<Article, bool>> filter = x => true;
            filter = filter.AndIf(x => x.CategoryID == param.CategoryID, param.CategoryID != 0)
                           .AndIf(x => x.Status == param.Status, param.Status != null);

            IPagedList<Article> articles = _articleRepository.FindPagedList(param, filter, orderBy);

            return articles;
        }


        public Article Get(int id)
        {
            return _articleRepository.Get(id);
        }

        /// <summary>
        /// 根据类别名称获取类别描述
        /// </summary>
        /// <param name="categoryName">类别名称</param>
        /// <returns>类别描述</returns>
        public string GetCategoryDesc(string categoryName)
        {
            var category = _categoryRepository.Single(p => p.Name == categoryName && p.Status == 1);
            return category!=null?category.Description:"";
            
        }



		public Category GetCategoryInfo(string categoryName)
	    {
			var category = _categoryRepository.First(p => p.Name.Contains(categoryName)&&p.ParentID==0);
			return category;
	    }



	    public Article GetArticle(int articleId)
        {
            return _articleRepository.Get(articleId);
        }

        public Article GetArticleByCategoryId(int categoryId)
        {
            return _articleRepository.GetList(p=>p.CategoryID==categoryId && p.Status==1).OrderBy(p=>p.Sort).FirstOrDefault();
        }

        /// <summary>
        /// 首次加载的时候显示一个默认的类表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int CategoryId(int id)
        {
            var category = _categoryRepository.Get(id);
            if (null == category)
            {
                return 0;
            }
            if (category.ParentID == 0)
            {
                var categoryFirst = _categoryRepository.First(p => p.ParentID == id && p.Status == 1,p => p.OrderBy(x => x.Sort).ThenBy(x => x.CategoryID));
                if(categoryFirst !=null)
                {
                    return categoryFirst.CategoryID;
                }
                else
                {
                    return 0;
                }
            }
            return _categoryRepository.Get(id).ParentID;
        }

        /// <summary>
        /// 判断是否是从一级分类进入的
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public bool Category(int categoryId)
        {
            var category = _categoryRepository.Get(categoryId);
            if (null != category && category.ParentID != 0)
            {
                return false;
            }
            return true;
        }

        public IEnumerable<Category> GetCategoryById(int id)
        {
            return _categoryRepository.GetList(P => P.ParentID == id && P.Status == 1).OrderBy(p=>p.Sort).ThenBy(p=>p.CategoryID);
        }

        public Category GetCategory(int id)
        {
            return _categoryRepository.Single(P => P.CategoryID == id && P.Status == 1);
        }
		//OrderBy(p=>p.Sort)
        public IEnumerable<Article> GetArticleById(int id)
        {
            return _articleRepository.GetList(p => p.CategoryID == id && p.Status == 1).OrderByDescending(p=>p.CreatedOn).Take(4);
        }

        public IEnumerable<Article> GetArticleByBigCategory()
        {
            return _articleRepository.GetArticleList().OrderBy(p=>p.Sort).ThenByDescending(p=>p.CreatedOn).Take(4);
        }

        public List<Article> GetArticleListByCategoryId(int categoryID)
        {
            return _articleRepository.GetList(p => p.CategoryID == categoryID && p.Status == 1).OrderByDescending(p => p.CreatedOn).ThenByDescending(p=>p.ArticleID).ToList();
        }
        

        public bool AddArticle(Article article)
        {
            _articleRepository.Add(article); 
            _articleRepository.UnitOfWork.Commit();
            return true;
        }

        public bool UpdateArticle(Article article)
        {
            _articleRepository.Modify(article);
            _articleRepository.UnitOfWork.Commit();
            return true;
        }

        public bool DeleteArticle(int articleId)
        {
            _articleRepository.Remove(x => x.ArticleID == articleId);
            _articleRepository.UnitOfWork.Commit();
            return true;
        }


        //获取新闻列表
        public IPagedList<ArticleExt> GetNewsPageList(ArticleParam param, out int totalCount)
        {
            Expression<Func<Article, bool>> filter = p => true;
            filter = filter.And(p => p.Status == 1);
            filter = filter.AndIf(p => p.Title.Contains(param.Title), !string.IsNullOrWhiteSpace(param.Title));
            
            filter = filter.AndIf(p => p.PublishedTime >= param.PublishedTime, param.PublishedTime.Date.ToString() != "0001/1/1 0:00:00");
            filter = filter.AndIf(p => p.PublishedTime <= param.PublishedEndTime, param.PublishedEndTime.Date.ToString() != "0001/1/1 0:00:00");
            string idList = String.Empty;
            var list = _articleRepository.GetFiltered(filter).OrderBy(p => p.ArticleID);
            if (param.CategoryID != null && param.CategoryID != 0 && !String.IsNullOrEmpty(param.CategoryID.ToString()))
            {
                idList = GetChildCategoryID(param.CategoryID.Value).TrimEnd(',');
                list = list.Where(p => idList.Contains(p.CategoryID.ToString())).OrderBy(p => p.ArticleID);
            }
            totalCount = list.Count();
            PagedList<Article> pagedList = list.ToPagedList(param.Page, param.PageSize);

            IPagedList<ArticleExt> articleExtList = new PagedList<ArticleExt>();
            pagedList.Each(p =>
            {
                ArticleExt extModel = new ArticleExt();
                p.Detail = "";
                p.CopyTo(extModel);
                var category = _categoryRepository.Single(e => e.CategoryID == p.CategoryID);
                var user = _customerRepository.Single(e => e.CustomerCode == p.CreatedBy);
                extModel.CategoryName = category != null ? category.Name : "";
                extModel.UserName = user != null ? user.Name : "";
                articleExtList.InnerList.Add(extModel);
            });
            return articleExtList;
        }


        private string ids = String.Empty;

        private string GetChildCategoryID(int categoryId)
        {
            ids += categoryId + ",";
            var list = _categoryRepository.GetList(p => p.ParentID == categoryId);
            if (list.Count > 0)
            {
                foreach (var item in list)
                {

                    var listChild = _categoryRepository.GetList(p => p.ParentID == item.CategoryID);
                    if (listChild.Count > 0)
                        GetChildCategoryID(item.CategoryID);
                    else
                        ids += item.CategoryID + ",";
                }
            }
            return ids;
        }

        //添加新闻
        public string AddNews(Article entity)
        {
            string result = "1";
            try
            {
                entity.Status = 1;
                entity.Sort = _articleRepository.GetAll().Count()==0?1:_articleRepository.GetAll().Max(p => p.Sort) + 1;
                _articleRepository.Add(entity);
                _articleRepository.UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                result = "0";
            }
            return result;
        }

        // 修改新闻
        public string UpdateNews(Article entity)
        {
            string result = "1";
            try
            {
                Article oldEntity = _articleRepository.First(p => p.ArticleID.Equals(entity.ArticleID));
                if (!String.IsNullOrEmpty(entity.Title))
                    oldEntity.Title = entity.Title;
                if (!String.IsNullOrEmpty(entity.Detail))
                    oldEntity.Detail = entity.Detail;
                if (!String.IsNullOrEmpty(entity.Source))
                    oldEntity.Source = entity.Source;
                oldEntity.CategoryID = entity.CategoryID;
                oldEntity.LastUpdatedBy = entity.LastUpdatedBy;
                oldEntity.LastUpdatedOn = entity.LastUpdatedOn;
                _articleRepository.Modify(oldEntity);
                _articleRepository.UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                result = "0";
            }
            return result;
        }

        //删除新闻
        public string DelNews(int articleId)
        {
            string result = "1";
            try
            {
                var entity = _articleRepository.Single(p => p.ArticleID == articleId);
                if (entity != null)
                {
                    //entity.Status = 2;//禁用
                    _articleRepository.Remove(entity);
                    _articleRepository.UnitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                result = "0";
            }
            return result;
        }

    }
}
