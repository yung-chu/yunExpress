using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Services.CustomerServices
{
    public interface ICustomerService
    {
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Customer Login(string userName, string password);

        /// <summary>
        /// 根据查询条件获取客户信息列表
        /// </summary>
        /// <param name="customerCode"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        List<Customer> GetCustomerList(string customerCode, int? status);
		IPagedList<CustomerExt> GetCustomerList(SearchCustomerParam param);

        List<Customer> GetCustomerList(string keyWord);

		/// <summary>
		/// 业务经理列表add by yungchu
		/// </summary>
		/// <returns></returns>
	    List<CustomerManagerInfo> GetListCustomerManagerInfo(string name);


	    /// <summary>
        /// 按关键字（客户编码，客户名称）搜索启用的客户信息列表
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        List<Customer> GetCustomerList(string keyWord, bool? IsAll);
        /// <summary>
        /// 根据客户编码获取客户信息
        /// </summary>
        /// <param name="customerCode"></param>
        /// <returns></returns>
        Customer GetCustomer(string customerCode);

        /// <summary>
        /// 根据客户ID获取客户信息
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        Customer GetCustomerById(string customerId);


		/// <summary>
		/// 根据客户accountId获取客户信息
		/// </summary>
		/// <param name="customerID"></param>
		/// <returns></returns>
		Customer GetCustomerByAccountId(string accountId);


        /// <summary>
        /// 根据客户类型Id获取客户代码
        /// </summary>
        /// <param name="customerTypeId">客户类型Id</param>
        /// <returns></returns>
        string GetCustomerCode(int customerTypeId);
        /// <summary>
        /// 新建客户
        /// </summary>
        /// <param name="customer"></param>
        void CreateCustomer(Customer customer);

		/// <summary>
		/// 这里主要生成customer开头为Y 的优速达客户编码
		/// yungchu
		/// </summary>
		/// <param name="customerCodeRule"></param>
		string CreateCustomerCode(string customerCodeRule);

		

	    /// <summary>
        /// 修改客户
        /// </summary>
        /// <param name="customer"></param>
        void UpdateCustomer(Customer customer);
        /// <summary>
        /// 查询客户充值记录列表
        /// </summary>
        /// <param name="customerCode"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        List<CustomerCreditInfo> GetCustomerCreditInfoList(string customerCode, int? status);
        /// <summary>
        /// 分页获取客户充值记录列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<CustomerCreditInfo> GetCustomerCreditPagedList(CustomerCreditParam param);
        /// <summary>
        /// 获取客户充值记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CustomerCreditInfo GetCustomerCreditInfo(int id);
        /// <summary>
        /// 创建客户充值
        /// </summary>
        /// <param name="customerCreditInfo"></param>
        void CreateCustomerCreditInfo(CustomerCreditInfo customerCreditInfo);
        /// <summary>
        /// 修改客户充值
        /// </summary>
        /// <param name="customerCreditInfo"></param>
        void UpdateCustomerCreditInfo(CustomerCreditInfo customerCreditInfo);
        /// <summary>
        /// 审核客户充值
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cStatusEnum"></param>
        void VerifyCustomerCreditInfo(int id, CustomerCreditInfo.StatusEnum cStatusEnum);
        /// <summary>
        /// 获取客户结算方式
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        List<PaymentType> GetPaymentTypeList(int? status);
        /// <summary>
        /// 获取客户资金变动类型
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        List<MoneyChangeTypeInfo> GetMoneyChangeTypeInfo(int? status);
        /// <summary>
        /// 创建资金记录
        /// </summary>
        /// <param name="param"></param>
        /// <returns>0-失败，1-成功</returns>
        int CreateCustomerAmountRecord(CustomerAmountRecordParam param);
        /// <summary>
        /// 获取用户资金记录
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<CustomerAmountRecordExt> GetCustomerAmountRecordPagedList(AmountRecordSearchParam param, out decimal totalInFee, out decimal totalOutFee);

        /// <summary>
        /// 获取用户统计信息
        /// </summary>
        /// <param name="customerCode"></param>
        /// <param name="recharge"></param>
        /// <param name="takeOffMoney"></param>
        /// <param name="balance"></param>
        /// <param name="unconfirmOrder"></param>
        /// <param name="confirmOrder"></param>
        /// <param name="submitOrder"></param>
        /// <param name="holdOrder"></param>
        /// <returns></returns>
        CustomerStatisticsInfoExt GetCustomerStatisticsInfo(string customerCode, out decimal recharge,
                                                            out decimal takeOffMoney, out decimal balance,
                                                            out int unconfirmOrder, out int confirmOrder,
                                                            out int submitOrder,out int haveOrder,out int sendOrder, out int holdOrder,out int totalOrder, out int submitingOrder, out int submitFailOrder);

        bool BoolCustomerAccountId(string AccountId);
        CustomerBalance GetCustomerBalance(string customerCode);
        /// <summary>
        /// 根据收货费用表验收时间获取用户列表 --Jess
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        List<Customer> GetCustomerByReceivingExpenseList(string keyword, DateTime? StartTime, DateTime? EndTime);

        /// <summary>
        /// 查出现结或预付的客户
        /// Add By zhengsong
        /// </summary>
        /// <param name="customers"></param>
        /// <returns></returns>
        List<Customer> GetCustomerList(List<string> customers);

    }
}
