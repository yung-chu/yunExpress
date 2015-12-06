using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LighTake.Infrastructure.Common;
using LMS.Data.Entity;

namespace LMS.Services.BillingServices
{
    public interface IBillingService
    {
        /// <summary>
        /// 获取运单列表信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<CustomerAmountRecordExt> GetCustomerAmountRecordPagedList(AmountRecordSearchParam param, out decimal totalInFee, out decimal totalOutFee);

        /// <summary>
        /// 账户充值
        /// </summary>
        /// <param name="param"></param>
        /// <returns>0-失败，1-成功</returns>
        void CreateRechargeRecord(CustomerCreditInfo creditinfo);

        /// <summary>
        /// 账户余额
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        CustomerBalance GetCustomerBalance(string customerCode);

        /// <summary>
        /// 充值类型表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<RechargeType> GetRechargeTypeList(int? status);

        /// <summary>
        /// 用户实体
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Customer GetCustomer(string customerCode);

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        string UpdateCustomer(Customer customer);
        /// <summary>
        /// 判断跟踪号是否存在
        /// </summary>
        /// <param name="transactionNo"></param>
        /// <returns></returns>
        bool CheckTransactionNo(string transactionNo);
    }
}
