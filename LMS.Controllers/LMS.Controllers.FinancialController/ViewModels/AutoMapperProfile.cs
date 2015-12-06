using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using LMS.Data.Entity;


namespace LMS.Controllers.FinancialController
{
	public class AutoMapperProfile : Profile
	{
		public override string ProfileName
		{
			get { return "ViewModel"; }
		}

		protected override void Configure()
		{

			CreateMap<ChargePayAnalysesExt, GetChargePayAnayiseModel>();
		}

	}
}