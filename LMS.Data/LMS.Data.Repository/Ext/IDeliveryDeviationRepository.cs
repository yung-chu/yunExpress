using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using LighTake.Infrastructure.Common;
using LMS.Data.Context;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LMS.Data.Entity.Param;

namespace LMS.Data.Repository
{
	public partial interface IDeliveryDeviationRepository
	{
		IPagedList<DeliveryDeviationExt> GetDeliveryDeviationPagedList(DeliveryDeviationParam param);
		List<DeliveryDeviationExt> GetDeliveryDeviationListInfo(DeliveryDeviationParam param);
	}
	/// <summary>
	/// 发货审核偏差
	/// yungchu
	/// </summary>
	public partial class DeliveryDeviationRepository
	{
		public IPagedList<DeliveryDeviationExt> GetDeliveryDeviationPagedList(DeliveryDeviationParam param)
		{
			var ctx = this.UnitOfWork as LMS_DbContext;
			Check.Argument.IsNotNull(ctx, "数据库对象");

			Expression<Func<DeliveryDeviation, bool>> filter = p => true;
			filter=filter.AndIf(a => a.VenderCode.Contains(param.VenderCode), !string.IsNullOrEmpty(param.VenderCode))
				  .AndIf(a => a.ShippingmethodID == param.ShippingmethodID, param.ShippingmethodID.HasValue);

			var result = from a in ctx.DeliveryDeviations.Where(a => a.DeviationType == 1)
						 join b in ctx.DeliveryDeviations.Where(b => b.DeviationType == 2).Where(filter)
					     on new {a.VenderCode, a.ShippingmethodID} equals new {b.VenderCode, b.ShippingmethodID}
						 orderby a.CreatedBy descending 
						 select new DeliveryDeviationExt
						{
							DeviationID = a.DeviationID,
							VenderCode = a.VenderCode,
							VenderId = a.VenderId,
							VenderName = a.VenderName,
							ShippingmethodID = a.ShippingmethodID,
							ShippingmethodName = a.ShippingmethodName,

							WaillDeviationValue = a.DeviationValue,
							WaillDeviationRate=a.DeviationRate,
							WeightDeviationValue=b.DeviationValue,
							WeightDeviationRate=b.DeviationRate

						};

			return result.ToPagedList(param.Page, param.PageSize);

		}


		public List<DeliveryDeviationExt> GetDeliveryDeviationListInfo(DeliveryDeviationParam param)
		{
			var ctx = this.UnitOfWork as LMS_DbContext;
			Check.Argument.IsNotNull(ctx, "数据库对象");

			Expression<Func<DeliveryDeviation, bool>> filter = p => true;
			filter = filter.AndIf(a => a.VenderCode.Contains(param.VenderCode), !string.IsNullOrEmpty(param.VenderCode))
				  .AndIf(a => a.ShippingmethodID == param.ShippingmethodID, param.ShippingmethodID.HasValue);

			var result = from a in ctx.DeliveryDeviations.Where(a => a.DeviationType == 1)
						 join b in ctx.DeliveryDeviations.Where(b => b.DeviationType == 2).Where(filter)
						 on new { a.VenderCode, a.ShippingmethodID } equals new { b.VenderCode, b.ShippingmethodID }
						 orderby a.CreatedBy descending
						 select new DeliveryDeviationExt
						 {
							 DeviationID = a.DeviationID,
							 VenderCode = a.VenderCode,
							 VenderId = a.VenderId,
							 VenderName = a.VenderName,
							 ShippingmethodID = a.ShippingmethodID,
							 ShippingmethodName = a.ShippingmethodName,

							 WaillDeviationValue = a.DeviationValue,
							 WaillDeviationRate = a.DeviationRate,
							 WeightDeviationValue = b.DeviationValue,
							 WeightDeviationRate = b.DeviationRate

						 };

			return result.ToList();

		}
	}
}
