using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity.ExtModel;
using LMS.Data.Entity.Param;
using LighTake.Infrastructure.Common;
using LMS.Data.Entity;

namespace LMS.Services.SettlementServices
{
    public interface ISettlementService
    {
        /// <summary>
        /// 获取未结清的结算单客户
        /// </summary>
        /// <param name="keyword">查询关键字</param>
        /// <returns></returns>
        List<CustomerSmallExt> GetOutstandingPaymentCustomer(string keyword);
        /// <summary>
        /// 根据客户编码查询结算清单信息列表
        /// </summary>
        /// <param name="customerCode">客户编码</param>
        /// <param name="eStatusEnum">结算清单状态，为空查询所有状态</param>
        /// <returns></returns>
        List<SettlementInfo> GetSettlementByCustomerCode(string customerCode,Settlement.StatusEnum? eStatusEnum);
        /// <summary>
        /// 结清结算单
        /// </summary>
        /// <param name="settlementNumbers"></param>
        void CheckOkSettlement(List<string> settlementNumbers);

        /// <summary>
        /// 通过结算单号获取结算单
        /// </summary>
        /// <param name="settlementNumber"></param>
        /// <returns></returns>
        SettlementInfo GetSettlementInfo(string settlementNumber);

        /// <summary>
        /// 生成结算单
        /// </summary>
        string CreateSettlement(string customerCode, string[] inStorageIDs);


		//结算清单列表 add by yungchu
		IPagedList<SettlementInfoExt> GetSettlementInfoList(SettlementInfoParam param);

        /// <summary>
        /// 现结客户发货情况
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<SettlementSummaryExt> GetSettlementSummaryExtPagedList(SettlementSummaryParam param);
    }
}
