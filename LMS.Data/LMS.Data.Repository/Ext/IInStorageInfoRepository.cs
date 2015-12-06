using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial interface IInStorageInfoRepository
    {
        /// <summary>
        /// 物流快递打单列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        PagedList<ExpressPrintWayBillExt> GetExpressPrintWayBillList(ExpressPrintWayBillParam param);

        /// <summary>
        /// 获取入仓列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        PagedList<InStorageInfoExt> GetInStorageInfoExtPagedList(InStorageListSearchParam param);

        InStorageInfo GetInStorageInfo(string InStorageId);

        string GetShippingMethodName(string InStorageId);

        List<InStorageTotalModel> GetInStorageTotals(string InStorageId);
        bool InStorageOrSettlementRelational(string customerCode);
    }
}
