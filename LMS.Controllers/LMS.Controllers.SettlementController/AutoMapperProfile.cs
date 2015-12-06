using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using LMS.Data.Entity;

namespace LMS.Controllers.SettlementController
{
    public class AutoMapperProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ViewModel"; }
        }

        protected override void Configure()
        {
            CreateMap<SettlementInfo, SettlementDetailViewModel>()
                .ForMember(p => p.CustomerName, o => o.Ignore());
        }
    }
}