using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Common;
using LMS.Data.Entity;

namespace LMS.Data.Repository
{
	public partial interface IDeliveryFeeInfoRepository
	{
	
		/// <summary>
		/// add by yungchu
		/// 2014/06/28
		/// 查询应收应付分析报表
		/// </summary>
		/// <param name="param"></param>
		/// <param name="TotalRecord"></param>
		/// <param name="TotalPage"></param>
		/// <returns></returns>

		IPagedList<ChargePayAnalysesExt> GetChargePayAnalysesList(ChragePayAnalyeseParam param, out int TotalRecord, out int TotalPage);

		/// <summary>
		/// add by yungchu
		/// 2014/6/30
		/// 导出应收应付分析报表
		/// </summary>
		/// <param name="param"></param>
		/// <param name="TotalRecord"></param>
		/// <param name="TotalPage"></param>
		/// <returns></returns>
		List<ChargePayAnalysesExt> ExportChargePayAnalysesList(ChragePayAnalyeseParam param);


	}
}
