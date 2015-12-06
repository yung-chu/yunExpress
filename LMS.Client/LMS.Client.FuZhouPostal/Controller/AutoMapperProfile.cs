using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;

namespace LMS.Client.FuZhouPostal.Controller
{
    public class AutoMapperProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ViewModel"; }
        }

        protected override void Configure()
        {
            CreateMap<FZWayBillInfoExt, PostalController.WayBillInfoModel>();
            CreateMap<ApplicationInfo, PostalController.ApplicationInfoModel>();
            CreateMap<SenderInfoModelExt, PostalController.SenderInfoModel>();
            CreateMap<ShippingInfoModelExt, PostalController.ShippingInfoModel>();
        }
    }
}
