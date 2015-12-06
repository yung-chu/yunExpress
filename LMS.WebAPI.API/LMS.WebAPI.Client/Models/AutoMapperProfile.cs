using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using LMS.Data.Entity;

namespace LMS.WebAPI.Client.Models
{
    public class AutoMapperProfile : Profile
    {
         public override string ProfileName
        {
            get { return "ViewModel"; }
        }

        protected override void Configure()
        {
            CreateMap<GoodsTypeInfo, GoodsTypeModel>()
                ;

            CreateMap<WayBillInfo, WayBillModel>().ForMember(m => m.PackageVolume, o => o.Ignore())
                .ForMember(m => m.ShippingMethodCode,o=>o.Ignore())
                .ForMember(m => m.TotalFee, o => o.MapFrom(p => p.InStorageInfo.TotalFee))
                .ForMember(m => m.TotalQty, o => o.MapFrom(p => p.InStorageInfo.TotalQty))

                .ForMember(m => m.OrderNumber, o => o.MapFrom(p => p.CustomerOrderInfo.CustomerOrderNumber))
                .ForMember(m => m.SensitiveTypeID, o => o.MapFrom(p => p.CustomerOrderInfo.SensitiveTypeID))
                ;
            CreateMap<ShippingInfo, ShippingInfoModel>();
            CreateMap<SenderInfo, SenderInfoModel>();
            CreateMap<ApplicationInfo, ApplicationInfoModel>();
        }
    }
}