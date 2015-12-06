using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Transactions;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Web.Utities;
using LMS.Services.FreightServices;

namespace LMS.Services.CustomerServices
{
    public class CustomerService : ICustomerService
    {
        private IWorkContext _workContext;
        private ICustomerRepository _customerRepository;
        private ICustomerCreditInfoRepository _customerCreditInfoRepository;
        private IPaymentTypeRepository _paymentTypeRepository;
        private IMoneyChangeTypeInfoRepository _moneyChangeTypeInfoRepository;
        private ICustomerAmountRecordRepository _customerAmountRecordRepository;
        private ICustomerBalanceRepository _customerBalanceRepository;
	    private ICustomerManagerInfoRepository _customerManagerInfoRepository;
        private ICustomerUpdatedLogRepository _customerUpdatedLogRepository;
        private ISettlementInfoRepository _settlementInfoRepository;
        private IFreightService _freightService;
        private IInStorageInfoRepository _inStorageInfoRepository;

        public CustomerService(IWorkContext workContext,
            ICustomerRepository customerRepository,
            ICustomerCreditInfoRepository customerCreditInfoRepository,
            IPaymentTypeRepository paymentTypeRepository,
            IMoneyChangeTypeInfoRepository moneyChangeTypeInfoRepository,
            ICustomerAmountRecordRepository customerAmountRecordRepository,
            IFreightService freightService,
            ICustomerUpdatedLogRepository customerUpdatedLogRepository,
            ISettlementInfoRepository settlementInfoRepository,
			ICustomerBalanceRepository customerBalanceRepository, 
            ICustomerManagerInfoRepository customerManagerInfoRepository,
            IInStorageInfoRepository inStorageInfoRepository)

        {
            _customerRepository = customerRepository;
            _customerCreditInfoRepository = customerCreditInfoRepository;
            _workContext = workContext;
            _paymentTypeRepository = paymentTypeRepository;
            _moneyChangeTypeInfoRepository = moneyChangeTypeInfoRepository;
            _customerAmountRecordRepository = customerAmountRecordRepository;
            _customerBalanceRepository = customerBalanceRepository;
	        _customerManagerInfoRepository = customerManagerInfoRepository;
            _customerUpdatedLogRepository = customerUpdatedLogRepository;
            _settlementInfoRepository = settlementInfoRepository;
            _freightService = freightService;
            _inStorageInfoRepository = inStorageInfoRepository;
        }
        public List<Customer> GetCustomerList(string customerCode, int? status)
        {
            Expression<Func<Customer, bool>> filter = p => true;
			filter = filter.AndIf(p => (p.CustomerCode.Contains(customerCode) || p.Name.Contains(customerCode)), !string.IsNullOrWhiteSpace(customerCode))
                           .AndIf(p => p.Status == status, status.HasValue);
  
			return _customerRepository.GetList(filter).OrderBy(p => p.Status).ThenBy(p => p.CreatedOn).ToList();
        }

        /// <summary>
        /// 查出现结或预付的客户
        /// Add By zhengsong
        /// </summary>
        /// <param name="customers"></param>
        /// <returns></returns>
        public List<Customer> GetCustomerList(List<string> customers)
        {
            Expression<Func<Customer, bool>> filter = p => true;
            filter = filter.And(p => customers.Contains(p.CustomerCode))
                           .And(p => p.Status == 2)
                           .And(p => p.PaymentTypeID == 3 || p.PaymentTypeID == 4);
            return _customerRepository.GetList(filter).ToList();
        }


		//客户列表显示 add by yungchu
	    public IPagedList<CustomerExt> GetCustomerList(SearchCustomerParam param)
	    {
		    return _customerRepository.CustomerList(param);
	    }

		//业务经理
	    public List<CustomerManagerInfo> GetListCustomerManagerInfo(string name)
	    {
			Expression<Func<CustomerManagerInfo, bool>> filter = p => true;

		    if (string.IsNullOrEmpty(name))
		    {
				return _customerManagerInfoRepository.GetList(filter);
		    }
		    else
		    {
				return _customerManagerInfoRepository.GetList(p => p.ID.ToString().Contains(name) || p.Name.Contains(name));
		    }
	    }




	    public List<Customer> GetCustomerList(string keyWord)
        {
            return GetCustomerList(keyWord, true);
        }
        public List<Customer> GetCustomerList(string keyWord, bool? IsAll)
        {
            var enablestatus = Customer.StatusToValue(Customer.StatusEnum.Enable);
            Expression<Func<Customer, bool>> filter = p => true;
            filter = filter.AndIf(p => p.Status == enablestatus, IsAll.HasValue && IsAll.Value)
                           .AndIf(p => (p.CustomerCode.Contains(keyWord) || p.Name.Contains(keyWord)), !string.IsNullOrWhiteSpace(keyWord));
            return _customerRepository.GetList(filter).ToList();
        }

        /// <summary>
        /// 根据客户类型Id获取客户代码
        /// </summary>
        /// <param name="customerTypeId">客户类型Id</param>
        /// <returns></returns>
        public string GetCustomerCode(int customerTypeId)
        {
            var customer = _customerRepository.First(p => p.CustomerTypeID == customerTypeId && p.Status == 1);

            return customer!=null?customer.CustomerCode:"";
        }



        public Customer GetCustomer(string customerCode)
        {
            Check.Argument.IsNullOrWhiteSpace(customerCode, "客户编码");
            return _customerRepository.First(p => p.CustomerCode == customerCode);
        }


	    public  Customer GetCustomerByAccountId(string accountId)
	    {
			Check.Argument.IsNullOrWhiteSpace(accountId, "客户名字");
			return _customerRepository.First(p => p.AccountID == accountId);
	    }


	    public Customer GetCustomerById(string customerId)
        {
            Check.Argument.IsNullOrWhiteSpace(customerId, "客户ID");
            return _customerRepository.Get(Guid.Parse(customerId));
        }

        public CustomerBalance GetCustomerBalance(string customerCode)
        {
            Check.Argument.IsNullOrWhiteSpace(customerCode, "客户编码");
            return _customerBalanceRepository.Single(p=>p.CustomerCode==customerCode);
        }

        public List<Customer> GetCustomerByReceivingExpenseList(string keyword, DateTime? StartTime, DateTime? EndTime)
        {
            return _customerRepository.GetCustomerByReceivingExpenseList(keyword, StartTime, EndTime);
        }

        public void CreateCustomer(Customer customer)
        {
            Check.Argument.IsNotNull(customer, "客户");
            Check.Argument.IsNullOrWhiteSpace(customer.AccountID, "登录账号");
            CustomerBalance customerBalance = new CustomerBalance();
            if (_customerRepository.Count(p => p.AccountID == customer.AccountID) > 0)
            {
                throw new ArgumentException("登录账号\"{0}\"已经存在.".FormatWith(customer.AccountID));
            }
            using (var transaction = new TransactionScope())
            {
                if (string.IsNullOrWhiteSpace(customer.CustomerCode))
                {
                    customer.CustomerCode = GenerateCustomerCode();
                }

                customer.CustomerManager = string.IsNullOrEmpty(customer.CustomerManager) ? GetCustomerManager("谭晓英").Name: customer.CustomerManager;
				customer.PaymentTypeID = 4;//结算类型-现结
                customer.CustomerID = Guid.NewGuid();
                customer.Name = customer.Name;
                customer.AccountPassWord = string.IsNullOrWhiteSpace(customer.AccountPassWord) ? "123456".ToMD5() : customer.AccountPassWord.ToMD5();
                customer.CreatedOn = customer.LastUpdatedOn = DateTime.Now;
                //_workContext.User.UserUame;
                customer.CreatedBy = customer.LastUpdatedBy = customer.AccountID;
                customer.EnableCredit = customer.EnableCredit;
                customer.MaxDelinquentAmounts = customer.MaxDelinquentAmounts;
                _customerRepository.Add(customer);
                _customerRepository.UnitOfWork.Commit();

                customerBalance.CustomerID = customer.CustomerID;
                customerBalance.CustomerCode = customer.CustomerCode.ToUpperInvariant().Trim();
                customerBalance.Balance = 0;
                customerBalance.CreatedOn = customer.CreatedOn;
                customerBalance.LastUpdatedOn = customer.CreatedOn;
                _customerBalanceRepository.Add(customerBalance);
                _customerBalanceRepository.UnitOfWork.Commit();
                transaction.Complete();
            }

        }
        /// <summary>
        /// 获取业务员信息
        /// add by yungchu
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CustomerManagerInfo GetCustomerManager(string name)
        {
            CustomerManagerInfo customerManagerInfo = _customerManagerInfoRepository.First(a => a.Name.Contains(name));
            if (customerManagerInfo != null)
            {
                return customerManagerInfo;
            }
            else//没有则插入该业务员信息
            {
                var customerManager = new CustomerManagerInfo
                {
                    Name = name,
                    Status = 1,
                    CreatedOn = System.DateTime.Now,
                    LastUpdatedOn = System.DateTime.Now
                };
                _customerManagerInfoRepository.Add(customerManager);
                _customerManagerInfoRepository.UnitOfWork.Commit();

                return customerManager;
            }
        }


        public void UpdateCustomer(Customer customer)
        {
            Check.Argument.IsNotNull(customer, "客户");
            Check.Argument.IsNullOrWhiteSpace(customer.CustomerCode, "客户编码");
            Check.Argument.IsNullOrWhiteSpace(customer.AccountID, "登录账号");
           
            Customer _customer = _customerRepository.First(p => p.CustomerID == customer.CustomerID);
            if (_customer != null)
            {
                if (_customer.AccountID != customer.AccountID)
                {
                    if (_customerRepository.Count(p => p.AccountID ==customer .AccountID) > 0)
                    {
                        throw new ArgumentException("登录账号{0}已经存在.".FormatWith(customer.AccountID));
                    }
                }

                //判断客户的结算类型是否是现结或者预付
                if (_customer.PaymentTypeID == 3 || _customer.PaymentTypeID == 4)
                {
                    if (customer.PaymentTypeID != 3 && customer.PaymentTypeID != 4)
                    {
                       if(!_inStorageInfoRepository.InStorageOrSettlementRelational(_customer.CustomerCode))
                       {
                            throw new ArgumentException("该客户还有未结清的结算单，请先结清结算单!");
                        }
                    }
                }

                //没有更新客户代码 就直接更新
                if (_customer.CustomerCode == customer.CustomerCode)
                {
                    UpdateCustomer(_customer, customer);
                }
                else//更新了客户代码
                {
                    var customerBalance = _customerBalanceRepository.First(p => p.CustomerID == customer.CustomerID);
                    if (customerBalance != null)//判断余额表是否存在
                    {
                        if (customerBalance.Balance > 0)
                        {
                            throw new ArgumentException("客户编号{0},余额不为零,不能更新该客户编号.".FormatWith(customer.CustomerCode));
                        }
                        else//余额为零
                        {
                            var bCustomerAmount = _customerAmountRecordRepository.Count(p => p.CustomerCode == customer.CustomerCode) > 0;
                            var bCustomerCreditInfo = _customerCreditInfoRepository.Count(p => p.CustomerCode == customer.CustomerCode) > 0;
                            if (bCustomerAmount || bCustomerCreditInfo)
                            {
                                throw new ArgumentException("客户编号{0},在其它表中存在关联，不能更新该客户编号.".FormatWith(customer.CustomerCode));
                            }
                            else //不存在就直接更新,并更新余额表客户代码
                            {
                                using (var transaction = new TransactionScope())
                                {
                                    UpdateCustomer(_customer, customer);

                                    customerBalance.CustomerID = _customer.CustomerID;
                                    customerBalance.CustomerCode = customer.CustomerCode.ToUpperInvariant().Trim();
                                    customerBalance.LastUpdatedOn =DateTime.Now;
                                    _customerBalanceRepository.Modify(customerBalance);
                                    _customerBalanceRepository.UnitOfWork.Commit();
                                    transaction.Complete();
                                }
                            }
                        }

                    }
                    else//不存在就直接更新
                    {
                        UpdateCustomer(_customer, customer);
                    }
                }
            }

           
        }

        private void UpdateCustomer(Customer _customer, Customer customer)
        {
            _customer.AccountID = customer.AccountID;
            if (!string.IsNullOrWhiteSpace(customer.AccountPassWord))
            {
                _customer.AccountPassWord = customer.AccountPassWord.ToMD5();
            }
            //跟新客户跟新记录表
            if (_customer.PaymentTypeID != customer.PaymentTypeID || _customer.EnableCredit != customer.EnableCredit ||
                _customer.MaxDelinquentAmounts != customer.MaxDelinquentAmounts)
            {
                CustomerUpdatedLog customerUpdatedLog=new CustomerUpdatedLog();
                customerUpdatedLog.CustomerID = _customer.CustomerID;
                customerUpdatedLog.CustomerCode = _customer.CustomerCode;
                customerUpdatedLog.EnableCredit = _customer.EnableCredit;
                customerUpdatedLog.PaymentTypeID = _customer.PaymentTypeID;
                customerUpdatedLog.MaxDelinquentAmounts = _customer.MaxDelinquentAmounts;
                customerUpdatedLog.ChangedBy = _customer.LastUpdatedBy;
                customerUpdatedLog.UpdatedOn = _customer.LastUpdatedOn;
                _customerUpdatedLogRepository.Add(customerUpdatedLog);

            }
            _customer.CustomerManager = customer.CustomerManager;
            _customer.CustomerCode = customer.CustomerCode.ToUpperInvariant().Trim();
            _customer.CustomerTypeID = customer.CustomerTypeID;
            _customer.PaymentTypeID = customer.PaymentTypeID;
            _customer.Name = customer.Name;
            _customer.LinkMan = customer.LinkMan;
            _customer.Tele = customer.Tele;
            _customer.WebSite = customer.WebSite;
            _customer.Address = customer.Address;
            _customer.Status = customer.Status;
            _customer.ApiKey = customer.ApiKey;
            _customer.ApiSecret = customer.ApiSecret;
            _customer.Remark = customer.Remark;
            _customer.EnableCredit = customer.EnableCredit;
            _customer.MaxDelinquentAmounts = customer.MaxDelinquentAmounts;
            _customer.LastUpdatedOn = DateTime.Now;
            _customer.LastUpdatedBy = _workContext.User.UserUame;
            _customerRepository.Modify(_customer);
            _customerRepository.UnitOfWork.Commit();
            _customerUpdatedLogRepository.UnitOfWork.Commit();
        }

        public List<CustomerCreditInfo> GetCustomerCreditInfoList(string customerCode, int? status)
        {
            Expression<Func<CustomerCreditInfo, bool>> filter = p => true;
            filter = filter.AndIf(p => p.CustomerCode.Contains(customerCode), !string.IsNullOrWhiteSpace(customerCode))
                           .AndIf(p => p.Status == status, status.HasValue);
            return _customerCreditInfoRepository.GetList(filter).ToList();
        }

        public IPagedList<CustomerCreditInfo> GetCustomerCreditPagedList(CustomerCreditParam param)
        {
            Expression<Func<CustomerCreditInfo, bool>> filter = p => true;
            filter = filter.AndIf(p => p.CustomerCode.Contains(param.CustomerCode),
                                  !string.IsNullOrWhiteSpace(param.CustomerCode))
                           .AndIf(p => p.Status == param.Status, param.Status.HasValue);

            Func<IQueryable<CustomerCreditInfo>, IOrderedQueryable<CustomerCreditInfo>>
             orderBy = o => o.OrderByDescending(p => p.CreatedOn);
            return _customerCreditInfoRepository.FindPagedList(param.Page, param.PageSize, filter, orderBy);
        }

        public CustomerCreditInfo GetCustomerCreditInfo(int id)
        {
            Check.Argument.IsNotNull(id, "ID");
            return _customerCreditInfoRepository.First(p => p.ID == id);
        }

        public void CreateCustomerCreditInfo(CustomerCreditInfo customerCreditInfo)
        {
            Check.Argument.IsNotNull(customerCreditInfo, "充值记录");
            _customerCreditInfoRepository.Add(customerCreditInfo);
            _customerCreditInfoRepository.UnitOfWork.Commit();
        }

        public void UpdateCustomerCreditInfo(CustomerCreditInfo customerCreditInfo)
        {
            Check.Argument.IsNotNull(customerCreditInfo, "充值记录");
            var nocheckStatus = CustomerCreditInfo.StatusToValue(CustomerCreditInfo.StatusEnum.NoCheck);
            var _customerCreditInfo =
                _customerCreditInfoRepository.First(p => p.ID == customerCreditInfo.ID && p.Status == nocheckStatus);
            if (_customerCreditInfo != null)
            {
                _customerCreditInfo.Amount = customerCreditInfo.Amount;
                _customerCreditInfo.RechargeType = customerCreditInfo.RechargeType;
                _customerCreditInfo.Remark = customerCreditInfo.Remark;
                _customerCreditInfo.TransactionNo = customerCreditInfo.TransactionNo;
                _customerCreditInfo.VoucherPath = customerCreditInfo.VoucherPath;
                _customerCreditInfo.LastUpdatedOn = DateTime.Now;
                _customerCreditInfo.LastUpdatedBy = _workContext.User.UserUame;
                _customerCreditInfoRepository.Modify(_customerCreditInfo);
                _customerCreditInfoRepository.UnitOfWork.Commit();
            }
        }

        public void VerifyCustomerCreditInfo(int id, CustomerCreditInfo.StatusEnum cStatusEnum)
        {
            Check.Argument.IsNotNull(id, "充值记录ID");
            var nocheckStatus = CustomerCreditInfo.StatusToValue(CustomerCreditInfo.StatusEnum.NoCheck);
            var _customerCreditInfo =
                _customerCreditInfoRepository.First(p => p.ID == id && p.Status == nocheckStatus);
            if (_customerCreditInfo != null)
            {
                _customerCreditInfo.Status = CustomerCreditInfo.StatusToValue(cStatusEnum);
                _customerCreditInfo.LastUpdatedOn = DateTime.Now;
                _customerCreditInfo.LastUpdatedBy = _workContext.User.UserUame;
                _customerCreditInfoRepository.Modify(_customerCreditInfo);
                _customerCreditInfoRepository.UnitOfWork.Commit();
            }
        }

        public List<PaymentType> GetPaymentTypeList(int? status)
        {
            Expression<Func<PaymentType, bool>> filter = p => true;
            filter = filter.AndIf(p => p.Status == status, status.HasValue);
            return _paymentTypeRepository.GetList(filter).ToList();
        }

        public List<MoneyChangeTypeInfo> GetMoneyChangeTypeInfo(int? status)
        {
            Expression<Func<MoneyChangeTypeInfo, bool>> filter = p => true;
            filter = filter.AndIf(p => p.Status == status, status.HasValue);
            return _moneyChangeTypeInfoRepository.GetList(filter).ToList();
        }

        public int CreateCustomerAmountRecord(CustomerAmountRecordParam param)
        {
            param.CreatedBy = _workContext.User!=null?_workContext.User.UserUame:"";
            return _customerAmountRecordRepository.CreateCustomerAmountRecord(param);
        }

        public IPagedList<CustomerAmountRecordExt> GetCustomerAmountRecordPagedList(AmountRecordSearchParam param, out decimal totalInFee, out decimal totalOutFee)
        {
            //Check.Argument.IsNullOrWhiteSpace(param.CustomerCode, "客户编码");
            var startTime = param.StartDateTime.HasValue ? param.StartDateTime.Value : new DateTime(2013, 1, 1);
            var endTime = param.EndDateTime.HasValue ? param.EndDateTime.Value : new DateTime(2020, 1, 1);
            param.StartDateTime = startTime;
            param.EndDateTime = endTime;
            return _customerAmountRecordRepository.GetCustomerAmountList(param, out totalInFee, out totalOutFee);
            //Expression<Func<CustomerAmountRecord, bool>> filter = p => true;
            //filter = filter.AndIf(p => p.CustomerCode == param.CustomerCode, !string.IsNullOrWhiteSpace(param.CustomerCode))
            //               .And(p => p.CreatedOn >= startTime && p.CreatedOn <= endTime);
            //Func<IQueryable<CustomerAmountRecord>, IOrderedQueryable<CustomerAmountRecord>>
            // orderBy = o => o.OrderBy(p => p.CreatedOn);
            //return _customerAmountRecordRepository.FindPagedList(param.Page, param.PageSize, filter, orderBy);
        }

		//客户编码开头字字母的生成判断 yungchu
	    public string getCustomerCodeRule;
	    public string CreateCustomerCode(string customerCodeRule)
	    {
			return getCustomerCodeRule = customerCodeRule;
	    }





	    private string GenerateCustomerCode()
        {
            string randomChars = "0123456789";
            int randomNum;
            Random random = new Random();
            string customerCode = string.Empty;
            for (int i = 0; i < 5; i++)
            {
                randomNum = random.Next(randomChars.Length);
                customerCode += randomChars[randomNum];
            }

			//默认客户编码开头为"C"
		    if (string.IsNullOrEmpty(getCustomerCodeRule))
		    {
			    getCustomerCodeRule = "C";
		    }

		    //字母加上5位数字
		    customerCode = getCustomerCodeRule + customerCode;

            return _customerRepository.Exists(l => l.CustomerCode == customerCode) ? GenerateCustomerCode() : customerCode;
        }

        public Customer Login(string accountID, string password)
        {
            string key = password.ToMD5();
            var info = _customerRepository.Single(c => c.AccountID == accountID && c.AccountPassWord == key);
            return info;
        }

        public CustomerStatisticsInfoExt GetCustomerStatisticsInfo(string customerCode, out decimal recharge, out decimal takeOffMoney, out decimal balance, out int unconfirmOrder, out int confirmOrder, out int submitOrder, out int haveOrder, out int sendOrder, out int holdOrder, out int totalOrder, out int submitingOrder, out int submitFailOrder)
        {
            CustomerStatisticsInfoExt model = null;
            recharge = 0; takeOffMoney = 0; balance = 0; unconfirmOrder = 0; confirmOrder = 0; submitOrder = 0;haveOrder = 0;
            sendOrder = 0; holdOrder = 0;
            totalOrder = 0;
            submitingOrder = 0;
            submitFailOrder = 0;
            if (!String.IsNullOrEmpty(customerCode))
            {
                model = _customerRepository.GetCustomerStatisticsInfo(customerCode, out recharge, out takeOffMoney,
                                                                     out balance, out unconfirmOrder, out confirmOrder,
                                                                     out submitOrder,out haveOrder,out sendOrder, out holdOrder,out totalOrder, out submitingOrder, out submitFailOrder);
            }
            return model;
        }

        public bool BoolCustomerAccountId(string AccountId)
        {
            if (_customerRepository.Count(p => p.AccountID == AccountId) > 0)
            {
                return true;
            }
            return false;
        }
    }
}
