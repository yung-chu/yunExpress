using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial interface ICustomerRepository
    {
        /// <summary>
        /// 客户统计信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        CustomerStatisticsInfoExt GetCustomerStatisticsInfo(string customerCode, out decimal recharge, out decimal takeOffMoney, out decimal balance, out int unconfirmOrder, out int confirmOrder, out int submitOrder, out int haveOrder, out int sendOrder, out int holdOrder, out int totalOrder, out int submitingOrder, out int submitFailOrder);
        /// <summary>
        /// 根据收货费用主表验收时间获取用户列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        List<Customer> GetCustomerByReceivingExpenseList(string keyword, DateTime? StartTime, DateTime? EndTime);
		/// <summary>
		/// 客户列表显示 add by yungchu
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		IPagedList<CustomerExt> CustomerList(SearchCustomerParam param);
    }
}
