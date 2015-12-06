using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LighTake.Infrastructure.Common;

namespace LMS.Controllers.FinancialController
{
	public class ChargePayAnayiseModel
	{
		public ChargePayAnayiseModel()
		{
			FilterModel = new ChragePayAnalyeseFilterModel();
			PagedList=new PagedList<GetChargePayAnayiseModel>();
			ListGetChargePayAnayis=new List<GetChargePayAnayiseModel>();
		}

		public IPagedList<GetChargePayAnayiseModel> PagedList { get; set; }
		public List<GetChargePayAnayiseModel> ListGetChargePayAnayis { get; set; }
		public ChragePayAnalyeseFilterModel FilterModel { get; set; }

	}

}

public class GetChargePayAnayiseModel	
{
	public string Name { get; set; }
	public string WayBillNumber { get; set; }
	public string VenderName { get; set; }
	public string ShippingmethodName { get; set; }
	public decimal? ReceivingAmount { get; set; }
	public decimal? DeliveryAmount { get; set; }
	public string Rate { get; set; }
}