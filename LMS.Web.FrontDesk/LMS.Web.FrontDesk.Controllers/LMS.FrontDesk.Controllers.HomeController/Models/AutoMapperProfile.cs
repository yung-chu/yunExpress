using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using LMS.Data.Entity;


namespace LMS.FrontDesk.Controllers.HomeController.Models
{
    /// <summary>
    ///  映射
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ViewModel"; }
        }

        protected override void Configure()
        {

            CreateMap<Article, ArticleModel>()
                ;
            CreateMap<Category,CategoryModel>()
                ;
            CreateMap<Article, NewsPartialModel>()
                ;

        }
    }
}
