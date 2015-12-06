using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LighTake.Infrastructure.Common;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;

namespace LMS.Controllers.FinancialController
{
 	public class DeliveryDeviationModel
	{
		public DeliveryDeviationModel()
 		{
			FilterModel=new DeliveryDeviationFilterModel();
			PagedList = new PagedList<DeliveryDeviationExt>();
 		}
 		public DeliveryDeviationFilterModel FilterModel { get; set; }
		public IPagedList<DeliveryDeviationExt> PagedList { get; set; }
 		public int GetId { get; set; }
		public int GetTypeItem { get; set; }
		public string GetVenderName { get; set; }
		public string GetVenderCode { get; set; }
		public int? GetShippingmethodId { get; set; }
		public string GetShippingmethodName { get; set; }
		public decimal? GetWaillDeviationValue { get; set; }
		public decimal? GetWaillDeviationRate { get; set; }
		public decimal? GetWeightDeviationValue { get; set; }
		public decimal? GetWeightDeviationRate { get; set; }

	}
}