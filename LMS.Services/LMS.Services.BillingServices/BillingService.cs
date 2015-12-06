using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LighTake.Infrastructure.Common;

namespace LMS.Services.BillingServices
{
    public class BillingService : IBillingService
    {

        private ICustomerAmountRecordRepository _amountRecordRepository;
        private ICustomerBalanceRepository _customerBalanceRepository;
        private IRechargeTypeRepository _rechargeTypeRepository;
        private ICustomerCreditInfoRepository _customerCreditInfoRepository;
        private ICustomerRepository _customerRepository;
        private readonly IWorkContext _workContext;

        public BillingService(ICustomerAmountRecordRepository amountRecordRepository,
                             ICustomerBalanceRepository customerBalanceRepository,
                             IRechargeTypeRepository rechargeTypeRepositor,
                             ICustomerCreditInfoRepository customerCreditInfoRepository,
                             ICustomerRepository customerRepository,
                             IWorkContext workContext)
        {
            _amountRecordRepository = amountRecordRepository;
            _customerBalanceRepository = customerBalanceRepository;
            _rechargeTypeRepository = rechargeTypeRepositor;
            _customerCreditInfoRepository = customerCreditInfoRepository;
            _customerRepository = customerRepository;
            _workContext = workContext;
        }

        //账户异动
        public IPagedList<CustomerAmountRecordExt> GetCustomerAmountRecordPagedList(AmountRecordSearchParam param, out decimal totalInFee, out decimal totalOutFee)
        {

            var startTime = param.StartDateTime.HasValue ? param.StartDateTime.Value : new DateTime(2013, 1, 1);
            var endTime = param.EndDateTime.HasValue ? param.EndDateTime.Value : new DateTime(2020, 1, 1);
            param.StartDateTime = startTime;
            param.EndDateTime = endTime;
            return _amountRecordRepository.GetCustomerAmountList(param, out totalInFee, out totalOutFee);
        }

        //根据客户编码获取账户余额实体
        public CustomerBalance GetCustomerBalance(string customerCode)
        {
            Expression<Func<CustomerBalance, bool>> filter = p => true;
            filter = filter.AndIf(p => p.CustomerCode.Contains(customerCode), !string.IsNullOrWhiteSpace(customerCode));
            CustomerBalance model = _customerBalanceRepository.Single(filter);
            if (model == null) //如果用户余额表中没有该用户数据
            {
                model = new CustomerBalance
                {
                    CustomerCode = _workContext.User.UserUame,
                    Balance = 0
                };
            }

            return model;
        }

        //获取充值类型
        public List<RechargeType> GetRechargeTypeList(int? status)
        {
            Expression<Func<RechargeType, bool>> filter = p => true;
            filter = filter.AndIf(p => p.Status == status, status.HasValue);
            return _rechargeTypeRepository.GetList(filter).ToList();
        }

        //账户充值
        public void CreateRechargeRecord(CustomerCreditInfo creditinfo)
        {
            _customerCreditInfoRepository.Add(creditinfo);
            _customerCreditInfoRepository.UnitOfWork.Commit();
        }

        //用户实体
        public Customer GetCustomer(string customerCode)
        {
            Expression<Func<Customer, bool>> filter = p => true;
            filter = filter.AndIf(p => p.CustomerCode.Contains(customerCode), !string.IsNullOrWhiteSpace(customerCode));
            return _customerRepository.Single(filter);
        }

        //修改用户信息实体
        public string UpdateCustomer(Customer customer)
        {
            string isSuccess = "1";
            try
            {
                _customerRepository.Modify(customer);
                _customerRepository.UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                isSuccess = ex.ToString();
            }
            return isSuccess;   
        }

        public bool CheckTransactionNo(string transactionNo)
        {
            return _amountRecordRepository.Exists(p => p.TransactionNo == transactionNo);
        }
    }
}
