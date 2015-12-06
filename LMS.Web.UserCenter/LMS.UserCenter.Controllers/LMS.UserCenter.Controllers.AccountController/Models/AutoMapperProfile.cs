using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using LMS.Data.Entity;


namespace LMS.UserCenter.Controllers.AccountController
{
    public class AutoMapperProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ViewModel"; }
        }

        protected override void Configure()
        {
            CreateMap<CustomerStatisticsInfoExt, CustomerStatisticsModel>();//读数据《数据仓库=》页面实体》
        }
    }
}
