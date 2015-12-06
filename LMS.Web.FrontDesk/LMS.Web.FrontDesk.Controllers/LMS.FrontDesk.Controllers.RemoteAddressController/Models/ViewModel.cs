using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LighTake.Infrastructure.Common;
using LMS.Data.Entity;

namespace LMS.FrontDesk.Controllers.RemoteAddressController
{
	public class ViewModel
	{
		public ViewModel()
		{
			PagedList = new PagedList<RemoteAreaAddressExt>();
			FilterModel = new RemoteAreaAddressParam();

			ShippingMethodLists = new List<SelectListItem>();
			Countrylists = new List<SelectListItem>();
		    ShowCategoryListModel = new ShowCategoryListModel();

		}

		public IPagedList<RemoteAreaAddressExt> PagedList { get; set; }
		public RemoteAreaAddressParam FilterModel { get; set; }

		public IList<SelectListItem> ShippingMethodLists { get; set; }
		public IList<SelectListItem> Countrylists { get; set; }
		public int GetId { get; set; }
        public ShowCategoryListModel ShowCategoryListModel { get; set; }
	}
}