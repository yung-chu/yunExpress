using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Seedwork;
using LMS.Data.Context;
using LMS.Data.Entity;

namespace LMS.Data.Repository
{
	public partial class DeliveryFeeInfoRepository : IDeliveryFeeInfoRepository
	{
		//查询应收应付分析报表
		public IPagedList<ChargePayAnalysesExt> GetChargePayAnalysesList(ChragePayAnalyeseParam param, out int TotalRecord, out int TotalPage)
		{
	        var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
			TotalRecord = 0;
			TotalPage = 0;


			var CustomerCode = new SqlParameter { ParameterName = "CustomerCode", Value = param.CustomerCode, DbType = DbType.String };
			var VenderCode = new SqlParameter {ParameterName = "VenderCode", Value = param.VenderCode, DbType = DbType.String};
			var ShippingMethodId=new SqlParameter{ParameterName = "ShippingMethodId",Value = param.ShippingMethodId,DbType = DbType.Int32};
			var StartTime = new SqlParameter { ParameterName = "StartTime", Value = param.StartTime, DbType = DbType.Time };
			var EndTime = new SqlParameter { ParameterName = "EndTime", Value = param.EndTime, DbType = DbType.Time };
			
			var PageSize = new SqlParameter { ParameterName = "PageSize", Value = param.PageSize, DbType = DbType.Int32 };
			var PageIndex = new SqlParameter { ParameterName = "PageIndex", Value = param.Page, DbType = DbType.Int32 };

			var TotalRecordParam = new SqlParameter { ParameterName = "TotalRecord", Value = 0, DbType = DbType.Int32, Direction = ParameterDirection.Output };
			var TotalPageParam = new SqlParameter { ParameterName = "TotalPage", Value = 0, DbType = DbType.Int32, Direction = ParameterDirection.Output };


			if (ctx != null)
			{
				var obj = ctx.ExecuteStoredProcedureList<ChargePayAnalysesExt>("P_GetAccountsPayableAnalyses", CustomerCode, StartTime,
					EndTime, VenderCode, ShippingMethodId, PageIndex, PageSize,  TotalRecordParam, TotalPageParam);

				if (obj != null && obj.Count > 0)
				{
					TotalRecord = int.Parse( String.IsNullOrEmpty(TotalRecordParam.Value.ToString())?"0":TotalRecordParam.Value.ToString());
					TotalPage = int.Parse(String.IsNullOrEmpty(TotalPageParam.Value.ToString()) ? "0" : TotalPageParam.Value.ToString());

					return new PagedList<ChargePayAnalysesExt>() { InnerList = obj.ToList(), PageIndex = param.Page, PageSize = param.PageSize, TotalCount = Convert.ToInt32(TotalRecordParam.Value.ToString()), TotalPages = Convert.ToInt32(TotalPageParam.Value.ToString()) };

				}
			
			}
			return new PagedList<ChargePayAnalysesExt>() { InnerList = null, PageIndex = param.Page, PageSize = param.PageSize, TotalCount = 0, TotalPages = 0 };
		}

		//导出应收应付分析报表
		public List<ChargePayAnalysesExt> ExportChargePayAnalysesList(ChragePayAnalyeseParam param)
		{

			var ctx = this.UnitOfWork as LMS_DbContext;
			Check.Argument.IsNotNull(ctx, "数据库对象");

			var CustomerCode = new SqlParameter { ParameterName = "CustomerCode", Value = param.CustomerCode, DbType = DbType.String };
			var VenderCode = new SqlParameter { ParameterName = "VenderCode", Value = param.VenderCode, DbType = DbType.String };
			var ShippingMethodId = new SqlParameter { ParameterName = "ShippingMethodId", Value = param.ShippingMethodId, DbType = DbType.Int32 };
			var StartTime = new SqlParameter { ParameterName = "StartTime", Value = param.StartTime, DbType = DbType.Time };
			var EndTime = new SqlParameter { ParameterName = "EndTime", Value = param.EndTime, DbType = DbType.Time };


			if (ctx != null)
			{
				var obj = ctx.ExecuteStoredProcedureList<ChargePayAnalysesExt>("P_ExportAccountsPayableAnalyses @CustomerCode,@VenderCode,@ShippingMethodId,@StartTime,@EndTime", CustomerCode,  VenderCode, ShippingMethodId,StartTime,
				EndTime);

				if (obj != null && obj.Count > 0)
				{
					return obj.ToList();
				}
			}
			return new List<ChargePayAnalysesExt>();
		}

	}
}
