using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using LMS.Controllers.OrderController;
using LMS.Data.Entity;


namespace LMS.Models
{
    public class AutoMapperProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ViewModel"; }
        }

        protected override void Configure()
        {
            CreateMap<InsuredCalculation, InsuredModel>();
            CreateMap<SensitiveTypeInfo, SensitiveTypeInfoModel>();
        }
    }
}
