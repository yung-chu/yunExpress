using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using LMS.Data.Entity;


namespace LMS.UserCenter.Controllers.BillingController.Models
{
    public class AutoMapperProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ViewModel"; }
        }

        protected override void Configure()
        {
            CreateMap<CustomerAmountRecordExt, BillingModel>();//读数据《数据仓库=》页面实体》
            CreateMap<InFeeInfoExt, InFeeInfoModel>()
                .ForMember(p => p.InShippingName, o => o.MapFrom(p => p.ShippingMethodName));//从数据仓库字段映射给Controller实体类
            CreateMap<Country, CountryModel>();
            CreateMap<CustomerBalance, CustomerBalances>();
        }
    }
}
