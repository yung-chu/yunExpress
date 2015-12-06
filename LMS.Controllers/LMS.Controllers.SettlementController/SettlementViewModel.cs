using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LighTake.Infrastructure.Common;
using LMS.Data.Entity;

namespace LMS.Controllers.SettlementController
{
	public class SettlementViewModel
	{
		public SettlementViewModel()
		{

		}

	}
	public class SettlementInfoList
	{

		public SettlementInfoList()
		{
			PagedList = new PagedList<SettlementInfoExt>();
			StatusList = new List<SelectListItem>();
		}

		public IPagedList<SettlementInfoExt> PagedList { get; set; }
		public SettlementInfoParam FilterModel { get; set; }
		public IList<SelectListItem> StatusList { get; set; }
	}
}