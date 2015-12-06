using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LighTake.Infrastructure.Common;

namespace LMS.Services.FeeManageServices
{
    public interface IFeeManageService
    {

        /// <summary>
        /// 获取所有费用类型
        /// </summary>
        /// <param name="feeTypeName"></param>
        /// <param name="IsEnable"></param>
        /// <returns></returns>
        List<FeeType> GetFeeTypeList(string feeTypeName, bool? isEnable);
        /// <summary>
        /// 新建费用类型
        /// </summary>
        /// <param name="feeType"></param>
        void CreateFeeType(FeeType feeType);
        /// <summary>
        /// 编辑费用类型
        /// </summary>
        /// <param name="feeType"></param>
        void UpdateFeeType(FeeType feeType);
        /// <summary>
        /// 获取单个费用
        /// </summary>
        /// <param name="feeTypeId"></param>
        /// <returns></returns>
        FeeType GetFeeType(int feeTypeId);
        /// <summary>
        /// 收货物流运费明细
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalFee"></param>
        /// <returns></returns>
        IPagedList<InFeeInfoExt> GetInFeeInfoPagedList(InFeeListParam param, out decimal totalFee);
        /// <summary>
        /// 发货物流运费明细
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalFee"></param>
        /// <returns></returns>
        IPagedList<OutFeeInfoExt> GetOutFeeInfoPagedList(OutFeeListParam param, out decimal totalFee);

        /// <summary>
        /// 收货物流运费明细(不分页)
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalFee"></param>
        /// <returns></returns>
        IList<InFeeInfoExt> GetInFeeInfoList(InFeeListParam param, out decimal totalFee);
        /// <summary>
        /// 发货物流运费明细(不分页)
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalFee"></param>
        /// <returns></returns>
        IList<OutFeeInfoExt> GetOutFeeInfoList(OutFeeListParam param, out decimal totalFee);

        /// <summary>
        /// 收货物流运费明细(不分页)
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalFee"></param>
        /// <returns></returns>
        IList<InFeeTotalInfoExt> GetInFeeTotalInfoList(InFeeTotalListParam param, out decimal totalFee);



    }
}
