using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;

namespace LMS.WebAPI.Model
{
    public class AutoMapperProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ViewModel"; }
        }

        protected override void Configure()
        {
            CreateMap<User, UserModel>()
                ;

            CreateMap<InFeeTotalInfoExt, InFeeTotalInfoExtModel>()
               ;
            CreateMap<PrintInStorageInvoiceExt, InStorageInfoModel>()
              ;
            CreateMap<InStorageInfo, InStorageModel>()
              ;
            CreateMap<WayBillInfo, WayBillInfoModel>();

            CreateMap<Customer, CustomerModel>();
        }

    }
}