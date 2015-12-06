using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.Controllers.FinancialController;
using LMS.Controllers.FinancialController.ViewModels;
using LMS.Data.Entity;
using LMS.Data.Entity.Param;
using LMS.Data.Entity.ExtModel;
using LighTake.Infrastructure.Common;

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
            CreateMap<ReceivingExpenseExt, ReceivingExpenseModel>();
            CreateMap<InFeeInfoAuditListExt, InFeeInfoAuditListModel>();
            CreateMap<AuditAnomalyViewModel, AuditAnomalyExt>();

            CreateMap<DeliveryCostDetailsFilterModel, DeliveryReviewParam>().
                ForMember(s => s.ShippingMethodIds,
                m=>m.MapFrom(q=>q.ShippingMethodId.Split(new[]{','},StringSplitOptions.RemoveEmptyEntries).Select(p=>Convert.ToInt32(p)).ToArray()));

            CreateMap<DeliveryReviewParam, DeliveryCostDetailsFilterModel>();

            CreateMap<DeliveryFeeExt, DeliveryFeeModel>()
                .ForMember(s => s.VenderData,
                m => m.MapFrom(
                    q => Mapper.Map<DeliveryImportAccountCheck, DeliveryImportDataModel>(q.VenderData)
                    ));
            CreateMap<DeliveryFeeExt, ExpressDeliveryFeeModel>();

            CreateMap<DeliveryFeeExpressExt, ExpressDeliveryFeeModel>();
            CreateMap<ExpressDeliveryImportAccountCheck, ExpressDeliveryImportDataModel>();

            CreateMap<PagedList<DeliveryFeeExt>, PagedList<DeliveryFeeModel>>()
                .ForMember(s => s.InnerList,
                m => m.MapFrom(
                    q => Mapper.Map<List<DeliveryFeeExt>, List<DeliveryFeeModel>>(q.InnerList)
                    ));

            CreateMap<ReceivingExpensesEditExt, ReceivingExpensesEditViewModel>();

            CreateMap<ChargePayAnalysesExt, GetChargePayAnayiseModel>();
            CreateMap<DeliveryFeeAnomalyEditExt, DeliveryFeeAnomalyEditViewModel>();
            CreateMap<DeliveryImportAccountCheck, DeliveryImportDataModel>();

            CreateMap<DeliveryImportDataModel, DeliveryImportAccountCheck>();

			CreateMap<JobErrorLogExt, GetJobErrorLogs>();

	        //CreateMap<DeliveryImportDataModel, DeliveryImportAccountCheck>().ForMember("ReceivingDateStr",
	        //    m => m.Ignore());

        }
    }
}
