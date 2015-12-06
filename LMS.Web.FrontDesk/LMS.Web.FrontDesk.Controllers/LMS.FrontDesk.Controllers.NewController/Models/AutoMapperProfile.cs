using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using LMS.Data.Entity;


namespace LMS.FrontDesk.Controllers.NewController.Models
{
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

        }
    }
}
