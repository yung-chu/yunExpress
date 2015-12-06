using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial interface ICustomerAmountRecordRepository
    {
        /// <summary>
        ///  生成资金变动记录 
        /// </summary>
        /// <param name="param"></param>
        /// <returns>1-成功，0-失败,2-已存在交易号</returns>
        int CreateCustomerAmountRecord(CustomerAmountRecordParam param);
        /// <summary>
        /// 查询客户资金记录汇总
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<CustomerAmountRecordExt> GetCustomerAmountList(AmountRecordSearchParam param, out decimal TotalInFee, out decimal TotalOutFee);
    }
}
