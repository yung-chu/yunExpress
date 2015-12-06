using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using LMS.Data.Entity;

namespace LMS.FrontDesk.Controllers.RemoteAddressController
{
	/// <summary>
	///  映射
	/// </summary>
	public class AutoMapperProfile : Profile
	{
		public override string ProfileName
		{
			get { return "ViewModel"; }
		}

		protected override void Configure()
		{

			CreateMap<Category, CategoryModel>()
				;
		
		}
	}
}