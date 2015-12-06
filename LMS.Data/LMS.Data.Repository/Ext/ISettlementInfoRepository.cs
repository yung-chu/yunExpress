using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity.ExtModel;
using LMS.Data.Entity.Param;
using LighTake.Infrastructure.Common;
using LMS.Data.Entity;

namespace LMS.Data.Repository
{
    public partial interface ISettlementInfoRepository
    {
        /// <summary>
        /// 获取未结清的结算单客户
        /// </summary>
        /// <param name="keyword">查询关键字</param>
        /// <returns></returns>
        List<CustomerSmallExt> GetOutstandingPaymentCustomer(string keyword);

		/// <summary>
		/// 结算清单查询列表
		/// add by yungchu
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
	    IPagedList<SettlementInfoExt> GetSettlementInfoList(SettlementInfoParam param);

        /// <summary>
        /// 现结客户发货情况
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<SettlementSummaryExt> GetSettlementSummaryExtPagedList(SettlementSummaryParam param);
    }
}
