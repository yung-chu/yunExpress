using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using LMS.Data.Entity;

namespace LMS.FrontDesk.Controllers.TrackController.Models
{
	public class AutoMapperProfile : Profile
	{
		public override string ProfileName
		{
			get { return "ViewModel"; }
		}

		protected override void Configure()
		{

			CreateMap<OrderTrackingModel, ContactTable>()
				.ForMember(p => p.WaybillNumber, dto => dto.Ignore())
				.ForMember(p => p.Destination, dto => dto.Ignore())
				.ForMember(p => p.EnumStingStatus, dto => dto.Ignore())
				.ForMember(p => p.CurrentLocation, dto => dto.Ignore())
				.ForMember(p => p.OrderTrackingDetails, dto => dto.Ignore())
				.ForMember(p => p.TrackNumber, dto => dto.Ignore())
				.ForMember(p => p.WaybillReurnStatus,dto=>dto.Ignore())
				.ForMember(p => p.WaybillStatus, dto => dto.Ignore())
				
				.ForMember(p => p.CreatedBy, dto => dto.MapFrom(p => p.CreatedBy))
				.ForMember(p => p.LastUpdatedOn, dto => dto.MapFrom(p => p.LastUpdatedOn))
				.ForMember(p => p.LastUpdatedBy, dto => dto.MapFrom(p => p.LastUpdatedBy))
				.ForMember(p => p.Remarks, dto => dto.MapFrom(p => p.Remarks))
				.ForMember(p => p.PackageState, dto => dto.MapFrom(p => p.PackageState))
				.ForMember(p => p.InfoState, dto => dto.MapFrom(p => p.InfoState))
				.ForMember(p => p.IntervalDays, dto => dto.MapFrom(p => p.IntervalDays))
				.ForMember(p => p.LastEventDate, dto => dto.MapFrom(p => p.LastEventDate))
				.ForMember(p => p.LastEventContent, dto => dto.MapFrom(p => p.LastEventContent));

		}
	}
}
