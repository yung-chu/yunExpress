using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Context;
using LMS.Data.Entity;

namespace LMS.Data.Repository
{
    public partial class ArticleRepository
    {

        /// <summary>
        /// 新闻中心
        /// CategoryID==13新闻类别
        /// </summary>
        /// <returns></returns>
        public List<ArticleExt> GetArticleList()
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            var list = (from s  in ctx.Articles 
                       join z in ctx.Categories on s.CategoryID equals z.CategoryID
                       where z.ParentID == 1 && z.Status==1 && s.Status==1 && z.CategoryID==13
                       select new ArticleExt
                           {
                               CategoryID = s.CategoryID,
                               ArticleID = s.ArticleID,
                               Title = s.Title,
                               CreatedOn = s.CreatedOn,
                               Sort = s.Sort,
                               Detail = s.Detail
                           });
            return list.ToList();
        }
    }
}
