using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using LMS.Controllers.WayBillController;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using ShippingInfoModel = LMS.Controllers.WayBillController.ShippingInfoModel;

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
            CreateMap<WayBillInfo, WayBillInfoModel>()
                .ForMember(p => p.CustomerOrderNumber,
                           o => o.MapFrom(p => p.CustomerOrderInfo.CustomerOrderNumber))
                .ForMember(p => p.VenderName, o => o.MapFrom(p => p.OutStorageInfo.VenderName))
                .ForMember(p => p.InShippingName, o => o.MapFrom(p => p.InShippingMethodName))
                .ForMember(p => p.OutShippingName, o => o.MapFrom(p => p.OutShippingMethodName))
                .ForMember(p => p.InCreatedBy, o => o.MapFrom(p => p.InStorageInfo.CreatedBy))
                .ForMember(p => p.OutCreatedBy, o => o.MapFrom(p => p.OutStorageInfo.CreatedBy))
                //.ForMember(p => p.AbnormalCreateOn, o => o.MapFrom(p => p.AbnormalWayBillLog.CreatedOn))
                //.ForMember(p => p.AbnormalCreateBy, o => o.MapFrom(p => p.AbnormalWayBillLog.CreatedBy))
                //.ForMember(p => p.AbnormalDescription, o => o.MapFrom(p => p.AbnormalWayBillLog.AbnormalDescription))
                .ForMember(p=>p.InStorageTime,o=>o.MapFrom(p=>p.InStorageCreatedOn))
                .ForMember(p => p.OutStorageTime, o => o.MapFrom(p => p.OutStorageCreatedOn))
                //.ForMember(p => p.AbnormalTypeName, o => o.Ignore())
                .ForMember(p=>p.VenderName,o=>o.Ignore());

            CreateMap<WayBillInfo, OutWayBillModel>();

            CreateMap<WayBillInfo, WayBillExcelExport>()
                .ForMember(p => p.InShippingName, o => o.MapFrom(p => p.InShippingMethodName))
                .ForMember(p => p.OutShippingName, o => o.MapFrom(p => p.OutShippingMethodName))
                .ForMember(p => p.InCreatedBy, o => o.MapFrom(p => p.InStorageInfo.CreatedBy))
                .ForMember(p => p.OutCreatedBy, o => o.MapFrom(p => p.OutStorageInfo.CreatedBy));

            CreateMap<WayBillInfo, EUBWayBill>()
               .ForMember(p => p.SensitiveTypeID, o => o.MapFrom(p => p.CustomerOrderInfo.SensitiveTypeID))
               .ForMember(p => p.Number, o => o.Ignore())
               .ForMember(p => p.IsCharged, o => o.Ignore())
               .ForMember(p => p.TrackingNumberCode, o => o.Ignore())
               .ForMember(p => p.Online, o => o.Ignore());
            
            CreateMap<WayBillInfo, AbnormalWayBillModel>()
                .ForMember(p => p.AbnormalCreateOn, o => o.MapFrom(p => p.AbnormalWayBillLog.CreatedOn))
                .ForMember(p => p.AbnormalCreateBy, o => o.MapFrom(p => p.AbnormalWayBillLog.CreatedBy))
                .ForMember(p => p.AbnormalDescription, o => o.MapFrom(p => p.AbnormalWayBillLog.AbnormalDescription))
                .ForMember(p => p.OperateType, o => o.MapFrom(p => p.AbnormalWayBillLog.OperateType))
                .ForMember(p => p.AbnormalStatus, o => o.MapFrom(p => p.AbnormalWayBillLog.AbnormalStatus))
                .ForMember(p => p.AbnormalTypeName, o => o.Ignore());

   
            CreateMap<InStorageInfo, InStorageInfoModel>()
                .ForMember(p => p.ShippingMethodName, o => o.Ignore());
            CreateMap<OutStorageInfo, OutStorageInfoModel>()
                .ForMember(p => p.isUpdate, o => o.Ignore())
                .ForMember(p => p.PostBagNumber, o => o.MapFrom(p => p.MailPostBagInfos.FirstOrDefault().PostBagNumber));
                //.ForMember(p => p.WayBillInfos, o => o.Ignore());
            CreateMap<Customer, CustomerInStorageModel>()
                .ForMember(p => p.PaymentTypeName, o => o.MapFrom(p => p.PaymentType.PaymentName))
                .ForMember(p => p.Balance, o => o.Ignore());
            CreateMap<ShippingInfo, ShippingInfoModel>();
            CreateMap<SenderInfo, SenderInfoModel>();
            CreateMap<ApplicationInfo, ApplicationInfoModel>();
            CreateMap<ApplicationInfo, ApplicationInfoModels>();
            CreateMap<AbnormalWayBillLog, AbnormalWayBillLogModel>();
            CreateMap<InFeeInfoExt, InFeeInfoModel>()
                .ForMember(p => p.InShippingName, o => o.MapFrom(p => p.ShippingMethodName))
                .ForMember(p => p.SettleWeight, o => o.MapFrom(p => p.SettleWeight??0))
                .ForMember(p => p.Freight, o => o.MapFrom(p => p.Freight ?? 0))
                .ForMember(p => p.FuelCharge, o => o.MapFrom(p => p.FuelCharge ?? 0))
                .ForMember(p => p.Register, o => o.MapFrom(p => p.Register ?? 0))
                .ForMember(p => p.TariffPrepayFee, o => o.MapFrom(p => p.TariffPrepayFee ?? 0))
                .ForMember(p => p.Surcharge, o => o.MapFrom(p => p.Surcharge ?? 0))
                .ForMember(p => p.TotalFee, o => o.MapFrom(p => p.TotalFee ?? 0));
            CreateMap<OutFeeInfoExt, OutFeeInfoModel>()
                .ForMember(p => p.OutShippingName, o => o.MapFrom(p => p.ShippingMethodName));
            CreateMap<SensitiveTypeInfo, SensitiveTypeInfoModel>();
            CreateMap<InsuredCalculation, InsuredCalculationModel>();
            CreateMap<GoodsTypeInfo, GoodsTypeInfoModel>();
            CreateMap<CustomerOrderInfo, CustomerOrderInfoModel>();
            CreateMap<TrackingNumberDetailInfo,UploadTrackingNumberDetailModel>()
                .ForMember(p=>p.IsRepeat,o=>o.Ignore())
            ;
            CreateMap<UploadTrackingNumberDetailModel, TrackingNumberDetailInfo>()
            ;
            CreateMap<TrackingNumberInfo,TrackingNumberModel>()
                .ForMember(p => p.filePath, o => o.Ignore())
                .ForMember(p => p.shippingMethod, o => o.Ignore())
                .ForMember(p => p.shippingMethods, o => o.Ignore())
                .ForMember(P=>P.SiteId,o=>o.Ignore())
                .ForMember(p=>p.CountryList,o=>o.Ignore())
                .ForMember(p=>p.SiteId,o=>o.Ignore())
                .ForMember(p => p.uploadTrackingNumberDetailModel, o => o.Ignore())
                .ForMember(p=>p.uploadTrackingNumberDetailModels,o=>o.MapFrom(p=>p.TrackingNumberDetailInfos))
            ;
            CreateMap<TrackingNumberModel, TrackingNumberInfo>()
                .ForMember(p => p.TrackingNumberDetailInfos, o => o.MapFrom(p=>p.uploadTrackingNumberDetailModels))
            ;

            CreateMap<CustomerOrderInfosModel, CustomerOrderInfo>()
                .ForMember(r => r.CreatedBy, m => m.Ignore())
                .ForMember(r => r.CreatedOn, m => m.Ignore())
                .ForMember(r => r.LastUpdatedBy, m => m.Ignore())
                .ForMember(r => r.LastUpdatedOn, m => m.Ignore())
                .ForMember(r => r.Status, m => m.Ignore())
                .ForMember(r => r.CustomerCode, m => m.Ignore())
                .ForMember(r => r.ApplicationInfos, m => m.MapFrom(p => p.ApplicationInfoList));

            CreateMap<CustomerOrderInfo, CustomerOrderInfosModel>()

                .ForMember(p => p.WayBillNumber, o => o.MapFrom(p => (p.WayBillInfos.Any(w => w.Status != WayBill.StatusEnum.Delete.GetStatusValue()) ? p.WayBillInfos.First(w => w.Status != WayBill.StatusEnum.Delete.GetStatusValue()).WayBillNumber : "")))
                .ForMember(p => p.CountryCode, o => o.MapFrom(p => (p.ShippingInfo != null ? p.ShippingInfo.CountryCode : "")))
                .ForMember(p => p.ApplicationInfoList, o => o.MapFrom(p => p.ApplicationInfos))
                ;

            CreateMap<ShippingInfo, CustomerOrderInfosModel>()
              ;
            CreateMap<CountryModel,CountryExt>()
                .ForMember(p=>p.IsCommonCountry,o=>o.Ignore())
                .ForMember(p => p.CountryPinyin, o => o.Ignore())
                ;

            CreateMap<WayBillTemplateModel, WayBillTemplate>()
                //.ForMember(p => p.WayBillTemplateType.TypeName, m => m.MapFrom(p => p.TemplateTypeName))
                //.ForMember(p => p.WayBillTemplateSpecification.SpecificationName,
                //           m => m.MapFrom(p => p.SpecificationName))
                ;

            CreateMap<WayBillTemplate, WayBillTemplateModel>()
                .ForMember(p => p.TemplateTypeName, o => o.Ignore())
                .ForMember(p => p.SpecificationName, o => o.Ignore())
                .ForMember(p => p.ShippingMethodName, o => o.Ignore())
                ;
            CreateMap<WayBillTemplateInfo, WayBillTemplateInfoModel>()
                ;
            CreateMap<SelectTrackingNumberExt, SelectTrackingNumberModel>()
                .ForMember(p=>p.ShippingMethods,o=>o.Ignore())
                .ForMember(p => p.ShippingMethodID, o => o.Ignore())
                .ForMember(p => p.StartTime, o => o.Ignore())
                .ForMember(p => p.EndTime, o => o.Ignore())
                ;
            CreateMap<SelectTrackingNumberModel, SelectTrackingNumberExt>()
                .ForMember(p=>p.TrackingNumberIds,o=>o.Ignore())
                ;
            CreateMap<TrackingNumberDetailed, TrackingNumber>()
                ;
            CreateMap<TrackingNumber, TrackingNumberDetailed>()
               ;

            CreateMap<TrackingNumberExt, TrackingNumberDetailed>()
             ;
            CreateMap<TrackingNumberDetailed, TrackingNumberExt>()
            ;
            CreateMap<ExpressPrintWayBillExt, ExpressPrintWayBill>();

            CreateMap<ReturnWayBillModelExt, ReturnWayBillModel>()
                .ForMember(p=>p.TypeName,o=>o.Ignore())
                .ForMember(p => p.IsReturnShippingName,o=>o.Ignore());
			CreateMap<WayBillPrintLog, WayBillPrintLogModel>();
            CreateMap<ExpressWayBillViewExt, ExpressWayBillInfoModel>();
            CreateMap<WayBillDetailExt, WayBillDetailModel>();
            CreateMap<DeliveryChannelConfiguration, OutStorageConfigureModel>();

	        CreateMap<InStorageWeightDeviation, InStorageWeightDeviationInfoModel>();

		    CreateMap<WaybillInfoUpdateExt, WaybillInfoUpdateModel>();
		

            CreateMap<ShippingWayBillExt, ShippingWayBillListModel>()
                .ForMember(p=>p.IsUpdate,o=>o.Ignore());

	        CreateMap<InStorageWeightAbnormalExt, InStorageWeightAbnormal>();


        }
    }
}
