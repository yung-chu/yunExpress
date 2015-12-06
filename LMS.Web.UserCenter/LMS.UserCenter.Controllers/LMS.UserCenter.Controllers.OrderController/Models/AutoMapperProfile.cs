using System;
using System.Linq;
using AutoMapper;
using LMS.Data.Entity;

namespace LMS.UserCenter.Controllers.OrderController.Models
{
    public class AutoMapperProfile : Profile
    {
        public override string ProfileName
        {
            get { return "OrderViewModel"; }
        }

        protected override void Configure()
        {
            CreateMap<CustomerOrderInfoModel, CustomerOrderInfo>()
                .ForMember(r => r.CreatedBy, m => m.Ignore())
                .ForMember(r => r.CreatedOn, m => m.Ignore())
                .ForMember(r => r.LastUpdatedBy, m => m.Ignore())
                .ForMember(r => r.LastUpdatedOn, m => m.Ignore())
                .ForMember(r => r.Status, m => m.Ignore())
                //.ForMember(r => r.InsureAmount, m => m.Ignore())
                .ForMember(r => r.CustomerCode, m => m.Ignore())
                .ForMember(r=>r.WayBillInfos,m=>m.Ignore())
               // .ForMember(r=>r.PackageNumber,m=>m.Ignore())
                //.ForMember(r=>r.InsureAmount,m=>m.Ignore())
               // .ForMember(r=>r.AppLicationType,m=>m.Ignore())
                //.ForMember(r => r.ApplicationInfos, m =>m.MapFrom(p=>p.ApplicationInfoList))
                ;

            CreateMap<CustomerOrderInfo, CustomerOrderInfoModel>()

                .ForMember(p => p.WayBillNumber, o => o.MapFrom(p => (p.WayBillInfos.Any(w => w.Status != WayBill.StatusEnum.Delete.GetStatusValue()) ? p.WayBillInfos.First(w => w.Status != WayBill.StatusEnum.Delete.GetStatusValue()).WayBillNumber : "")))
                .ForMember(p => p.CountryCode, o => o.MapFrom(p => (p.ShippingInfo != null ? p.ShippingInfo.CountryCode : "")))
                .ForMember(p => p.ApplicationInfoList, o => o.MapFrom(p => p.ApplicationInfos))
                .ForMember(p => p.InsuredCalculationId, o => o.Ignore())
                .ForMember(p => p.AppLicationTypeId, o => o.Ignore())
                .ForMember(p => p.InsuredValue, o => o.Ignore())
                .ForMember(p => p.InsureAmountValue, o => o.Ignore())
                .ForMember(p => p.InsuredValue, o => o.Ignore())
                .ForMember(p => p.PackageNumberValue, o => o.Ignore())
                .ForMember(p => p.ReturnUrl, o => o.Ignore())
                .ForMember(p => p.InsuredName, o => o.Ignore())
                .ForMember(p => p.SensitiveTypeName, o => o.Ignore())
                //.ForMember(p => p.DeliveryDate, o => o.Ignore())
                .ForMember(p => p.RawTrackingNumber, o => o.MapFrom(p => (p.WayBillInfos.Any(w => w.Status != WayBill.StatusEnum.Delete.GetStatusValue()) ? p.WayBillInfos.First(w => w.Status != WayBill.StatusEnum.Delete.GetStatusValue()).RawTrackingNumber : "")))
                .ForMember(p => p.TransferOrderDate, o => o.MapFrom(p => (p.WayBillInfos.Any(w => w.Status != WayBill.StatusEnum.Delete.GetStatusValue()) ? p.WayBillInfos.First(w => w.Status != WayBill.StatusEnum.Delete.GetStatusValue()).TransferOrderDate : null)))
                .ForMember(p => p.ShippingFirstName, o => o.MapFrom(p => p.ShippingInfo.ShippingFirstName))
                .ForMember(p => p.ShippingLastName, o => o.MapFrom(p => p.ShippingInfo.ShippingLastName))
                .ForMember(p => p.CountryCode, o => o.MapFrom(p => p.ShippingInfo.CountryCode))
                .ForMember(p => p.ShippingCompany, o => o.MapFrom(p => p.ShippingInfo.ShippingCompany))
                .ForMember(p => p.ShippingAddress, o => o.MapFrom(p => p.ShippingInfo.ShippingAddress))
                .ForMember(p => p.ShippingCity, o => o.MapFrom(p => p.ShippingInfo.ShippingCity))
                .ForMember(p => p.ShippingState, o => o.MapFrom(p => p.ShippingInfo.ShippingState))
                .ForMember(p => p.ShippingZip, o => o.MapFrom(p => p.ShippingInfo.ShippingZip))
                .ForMember(p => p.ShippingPhone, o => o.MapFrom(p => p.ShippingInfo.ShippingPhone))
                .ForMember(p => p.ShippingTaxId, o => o.MapFrom(p => p.ShippingInfo.ShippingTaxId))
                .ForMember(p => p.SenderFirstName, o => o.MapFrom(p => p.SenderInfo.SenderFirstName))
                .ForMember(p => p.SenderFirstName, o => o.MapFrom(p => p.SenderInfo.SenderFirstName))
                .ForMember(p => p.SenderLastName, o => o.MapFrom(p => p.SenderInfo.SenderLastName))
                .ForMember(p => p.SenderCompany, o => o.MapFrom(p => p.SenderInfo.SenderCompany))
                .ForMember(p => p.SenderAddress, o => o.MapFrom(p => p.SenderInfo.SenderAddress))
                .ForMember(p => p.SenderCity, o => o.MapFrom(p => p.SenderInfo.SenderCity))
                .ForMember(p => p.SenderState, o => o.MapFrom(p => p.SenderInfo.SenderState))
                .ForMember(p => p.SenderZip, o => o.MapFrom(p => p.SenderInfo.SenderZip))
                .ForMember(p => p.SenderPhone, o => o.MapFrom(p => p.SenderInfo.SenderPhone))
                .ForMember(p => p.DeliveryDate, o => o.MapFrom(p => (p.CustomerOrderStatuses.Any(c => c.Status == CustomerOrder.StatusEnum.Send.GetStatusValue()) ? p.CustomerOrderStatuses.First(c => c.Status == CustomerOrder.StatusEnum.Send.GetStatusValue()).CreatedOn.ToString() : null)))
                //.ForMember(p => p.DeliveryDate, o => o.MapFrom(p => p.CustomerOrderStatuses.First(c => c.Status == CustomerOrder.StatusEnum.Send.GetStatusValue()).CreatedOn))
                .ForMember(p => p.WayBillInfos, o => o.MapFrom(p => p.WayBillInfos))
                ;

            CreateMap<CustomerOrderInfoExportExt, CustomerOrderInfoModel>();

            CreateMap<OrderModel, CustomerOrderInfo>()
                .ForMember(p=>p.AppLicationType,o=>o.MapFrom(p=>p.AppLicationTypeId))
                .ForMember(p => p.InsureAmount, o => o.MapFrom(p => p.InsureAmount))
                .ForMember(p => p.PackageNumber, o => o.MapFrom(p => p.PackageNumber))
                .ForMember(p=>p.CustomerOrderID,o=>o.Ignore());

            CreateMap<ProductModel, ApplicationInfo>();
            CreateMap<OrderModel, ShippingInfo>()
                ;
            CreateMap<OrderModel, SenderInfo>()
                .ForMember(p=>p.CountryCode,m=>m.Ignore())
                ;
            CreateMap<ShippingInfo, CustomerOrderInfoModel>()
                .ForMember(p=>p.WayBillInfos,o=>o.Ignore())
                .ForMember(p => p.ApplicationInfoList,o=>o.Ignore())
                .ForMember(p=>p.InsuredCalculationsTypes,o=>o.Ignore())
                .ForMember(p=>p.AppLicationTypes,o=>o.Ignore())
                ;
            CreateMap<SenderInfo, CustomerOrderInfoModel>()
                .ForMember(p => p.WayBillInfos, o => o.Ignore())
                .ForMember(p => p.ApplicationInfoList, o => o.Ignore())
                .ForMember(p => p.InsuredCalculationsTypes, o => o.Ignore())
                .ForMember(p => p.AppLicationTypes, o => o.Ignore())
                ;
            CreateMap<CustomerOrderInfoExt, CustomerOrderInfoModel>()
                ;
            CreateMap<CustomerOrderInfoModel, ShippingInfo>()
                .ForMember(p => p.WayBillInfos, o => o.Ignore())
                ;
            CreateMap<CustomerOrderInfoModel, SenderInfo>()
                .ForMember(p => p.WayBillInfos, o => o.Ignore())
                .ForMember(p=>p.CountryCode,m=>m.Ignore())
                ;
            CreateMap<ApplicationInfoModel, ApplicationInfo>()
                .ForMember(r => r.CreatedBy, m => m.Ignore())
                .ForMember(r => r.CreatedOn, m => m.Ignore())
                .ForMember(r => r.LastUpdatedBy, m => m.Ignore())
                .ForMember(r => r.LastUpdatedOn, m => m.Ignore())
            ;

            CreateMap<ApplicationInfo, ApplicationInfoModel>();

            CreateMap<WayBillInfo, WayBillInfoModel>()
                .ForMember(p => p.CustomerOrderNumber,
                           o => o.MapFrom(p => p.CustomerOrderInfo.CustomerOrderNumber))
                .ForMember(p => p.VenderName, o => o.MapFrom(p => p.OutStorageInfo.VenderName))
                .ForMember(p => p.InShippingName, o => o.MapFrom(p => p.InShippingMethodName))
                .ForMember(p => p.OutShippingName, o => o.MapFrom(p => p.OutShippingMethodName))
                .ForMember(p => p.AbnormalCreateOn, o => o.MapFrom(p => p.AbnormalWayBillLog.CreatedOn))
                .ForMember(p => p.AbnormalDescription, o => o.MapFrom(p => p.AbnormalWayBillLog.AbnormalDescription))
                .ForMember(p => p.AbnormalTypeName, o => o.Ignore())
                .ForMember(p => p.InStorageTime, o => o.Ignore())
                .ForMember(p => p.OutStorageTime, o => o.Ignore());
            CreateMap<ShippingInfo, ShippingInfoModel>();

            CreateMap<EubWayBillApplicationInfoExt, EubWayBillApplicationInfoModel>();

        }
    }
}