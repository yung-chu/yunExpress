using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using LMS.Data.Entity;

namespace LMS.FrontDesk.Controllers.UserController.Models
{
    public class AutoMapperProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ViewModel"; }
        }
        protected override void Configure()
        {
            CreateMap<AddCustomerModel,Customer>()
                .ForMember(p=>p.CustomerCode,dto=>dto.Ignore())
                .ForMember(p=>p.AccountPassWord,dto=>dto.MapFrom(p=>p.NextPassword))
                .ForMember(p=>p.Name,dto=>dto.MapFrom(p=>p.Name))
                .ForMember(p => p.Mobile, dto => dto.MapFrom(p => p.Mobile))
                .ForMember(p => p.Address, dto => dto.MapFrom(p => p.Address))
                .ForMember(p => p.AccountID,dto=>dto.MapFrom(p=>p.AccountID))
                .ForMember(p => p.CustomerTypeID, dto => dto.Ignore())
                .ForMember(p => p.EnName, dto => dto.Ignore())
                .ForMember(p => p.LinkMan, dto => dto.Ignore())
                .ForMember(p => p.WebSite, dto => dto.Ignore())
                .ForMember(p => p.PaymentTypeID, dto => dto.Ignore())
                .ForMember(p => p.EnAddress, dto => dto.Ignore())
                .ForMember(p => p.Country, dto => dto.Ignore())
                .ForMember(p => p.City, dto => dto.Ignore())
                .ForMember(p => p.Province, dto => dto.Ignore())
                .ForMember(p => p.Email, dto => dto.Ignore())
                .ForMember(p => p.Fax, dto => dto.Ignore())
                .ForMember(p => p.PostCode, dto => dto.Ignore())
                .ForMember(p => p.MSN, dto => dto.Ignore())
                .ForMember(p => p.QQ, dto => dto.Ignore())
                .ForMember(p => p.Skype, dto => dto.Ignore())
                .ForMember(p => p.Status, dto => dto.Ignore())
                .ForMember(p => p.LastLoginIP, dto => dto.Ignore())
                .ForMember(p => p.Remark, dto => dto.Ignore())
                .ForMember(p => p.CreatedBy, dto => dto.Ignore())
                .ForMember(p => p.CreatedOn, dto => dto.Ignore())
                .ForMember(p => p.LastUpdatedBy, dto => dto.Ignore())
                .ForMember(p => p.LastUpdatedOn, dto => dto.Ignore())
                .ForMember(p => p.LoginCount, dto => dto.Ignore())
                ;

            CreateMap<Category, CategoryModel>();

        }
    }
}
