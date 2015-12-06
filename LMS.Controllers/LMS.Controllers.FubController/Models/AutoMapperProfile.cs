using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using LMS.Data.Entity;

namespace LMS.Controllers.FubController.Models
{
    public class AutoMapperProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ViewModel"; }
        }

        protected override void Configure()
        {
            CreateMap<FubListModelExt, FubListModel>();
            CreateMap<MailTotalPackageInfoExt, MailTotalPackageInfoModel>();
        }
    }
}