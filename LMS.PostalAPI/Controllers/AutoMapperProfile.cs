using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;

namespace LMS.PostalAPI.Controllers
{
    public class AutoMapperProfile : Profile
    {
         public override string ProfileName
        {
            get { return "ViewModel"; }
        }

        protected override void Configure()
        {
            CreateMap<FZWayBillInfoExt, WayBillInfoModel>();

            CreateMap<ApplicationInfo, ApplicationInfoModel>();
            CreateMap<SenderInfo, SenderInfoModel>();
            CreateMap<ShippingInfo, ShippingInfoModel>();
        }
    }
}