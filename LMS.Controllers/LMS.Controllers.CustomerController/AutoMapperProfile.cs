using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using LMS.Controllers.CustomerController;
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

            CreateMap<Customer, CustomerListModel>()
                .ForMember(p => p.Balance, o => o.MapFrom(p => p.CustomerBalance.Balance));
            CreateMap<CustomerListModel, Customer>();
            CreateMap<Customer, CustomerModel>();
            CreateMap<CustomerCreditInfo, CustomerCreditModel>();
            CreateMap<CustomerAmountRecord, CustomerAmountRecordModel>()
                .ForMember(p => p.MoneyChangeTypeShortName,
                           o => o.MapFrom(p => p.MoneyChangeTypeInfo.MoneyChangeTypeShortName))
                           .ForMember(p => p.FeeTypeName, o => o.MapFrom(p => p.FeeType.FeeTypeName));
            CreateMap<CustomerAmountRecordExt, CustomerAmountRecordListModel>();
        }
    }
}
