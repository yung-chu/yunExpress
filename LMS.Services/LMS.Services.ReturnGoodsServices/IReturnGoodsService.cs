using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LighTake.Infrastructure.Common;

namespace LMS.Services.ReturnGoodsServices
{
    public interface IReturnGoodsService
    {
        //
        void Add(ReturnGoods returnGoods);
        /// <summary>
        /// 批量添加退货处理
        /// </summary>
        /// <param name="list"></param>
        void BatchAddReturnGoods(List<ReturnGoodsExt> list);
        IEnumerable<ReturnGoods> GetList();

        /// <summary>
        /// 获取退货信息查询分页列表
        /// Add by zhengsong
        /// Time:2014-05-16
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<ReturnWayBillModelExt> GetPagedList(ReturnWayBillParam param);

        /// <summary>
        /// 获取需要导出的退货信息
        /// Add by zhengsong
        /// Time:2014-05-17
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<ReturnWayBillModelExt> GetExportReturnWayBillList(ReturnWayBillParam param);

        /// <summary>
        /// 批量审核通过
        /// Add By zhengsong
        /// </summary>
        /// <param name="wayBilllist"></param>
        /// <returns></returns>
        bool ReturnAuditList(string[] wayBilllist);

        /// <summary>
        /// 批量修改审核通过
        /// Add By zhengsong
        /// </summary>
        /// <param name="wayBilllist"></param>
        /// <param name="type"></param>
        /// <param name="returnReason"></param>
        /// <param name="isReturnShipping"></param>
        /// <returns></returns>
        bool UpdateReturnAuditList(string[] wayBilllist,int type, string returnReason,bool isReturnShipping);
    }
}
