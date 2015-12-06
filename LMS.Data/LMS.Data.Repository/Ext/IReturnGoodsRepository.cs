using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial interface IReturnGoodsRepository
    {
        /// <summary>
        /// 获取退货信息列表
        /// Add by zhengsong
        /// Time:2014-05-16
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<ReturnWayBillModelExt> GetPagedListReturnWayBill(ReturnWayBillParam param);

        /// <summary>
        /// 获取退货信息集合
        /// Add by zhengsong
        /// Time:2014-05-16
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<ReturnWayBillModelExt> GetReturnWayBillList(ReturnWayBillParam param);
    }
}
