using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using LMS.Core;
using LMS.Data.Entity.ExtModel;
using LMS.Data.Entity.Param;
using LighTake.Infrastructure.Common;
using System.Transactions;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LMS.Services.SequenceNumber;
using LighTake.Infrastructure.Common;

namespace LMS.Services.SettlementServices
{
    public class SettlementService : ISettlementService
    {
        private readonly ISettlementInfoRepository _settlementInfoRepository;
        private readonly IInStorageOrSettlementRelationalRepository _inStorageOrSettlementRelationalRepository;
        private readonly IInStorageInfoRepository _inStorageInfoRepository;
        private readonly IReceivingExpensRepository _receivingExpensRepository;
        private readonly ISettlementDetailsInfoRepository _settlementDetailsInfoRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerManagerInfoRepository _customerManagerInfoRepository;
        private IWorkContext _workContext;

        public SettlementService(ISettlementInfoRepository settlementInfoRepository,
                                 IInStorageOrSettlementRelationalRepository inStorageOrSettlementRelationalRepository,
                                 IInStorageInfoRepository inStorageInfoRepository,
                                 IReceivingExpensRepository receivingExpensRepository,
                                 ISettlementDetailsInfoRepository settlementDetailsInfoRepository,
                                 ICustomerRepository customerRepository,
                                 ICustomerManagerInfoRepository customerManagerInfoRepository,
                                 IWorkContext workContext
            )
        {
            _settlementInfoRepository = settlementInfoRepository;
            _inStorageOrSettlementRelationalRepository = inStorageOrSettlementRelationalRepository;
            _inStorageInfoRepository = inStorageInfoRepository;
            _receivingExpensRepository = receivingExpensRepository;
            _settlementDetailsInfoRepository = settlementDetailsInfoRepository;
            _customerRepository = customerRepository;
            _customerManagerInfoRepository = customerManagerInfoRepository;
            _workContext = workContext;
        }

        public List<CustomerSmallExt> GetOutstandingPaymentCustomer(string keyword)
        {
            return _settlementInfoRepository.GetOutstandingPaymentCustomer(keyword);
        }

        public List<SettlementInfo> GetSettlementByCustomerCode(string customerCode, Settlement.StatusEnum? eStatusEnum)
        {
            int? status = eStatusEnum.HasValue ? Settlement.StatusToValue(eStatusEnum.Value) : new int?();
            Expression<Func<SettlementInfo, bool>> filter = p => true;
            filter = filter.AndIf(p => p.CustomerCode == customerCode,!string.IsNullOrWhiteSpace(customerCode))
                           .AndIf(p => p.Status == status.Value, eStatusEnum.HasValue);
            return _settlementInfoRepository.GetList(filter).ToList();
        }

        public void CheckOkSettlement(List<string> settlementNumbers)
        {
            var status = Settlement.StatusToValue(Settlement.StatusEnum.OK);
            _settlementInfoRepository.Modify(m=>new SettlementInfo()
            {
                Status = status,
                LastUpdatedBy = _workContext.User.UserUame,
                LastUpdatedOn = DateTime.Now,
                SettlementBy = _workContext.User.UserUame,
                SettlementOn = DateTime.Now
            },t=>settlementNumbers.Contains(t.SettlementNumber)
            );
            _settlementInfoRepository.UnitOfWork.Commit();
        }

        public SettlementInfo GetSettlementInfo(string settlementNumber)
        {
            return _settlementInfoRepository.Get(settlementNumber);
        }

        public string CreateSettlement(string customerCode, string[] inStorageIDs)
        {

            var listInStorageInfo = _inStorageInfoRepository.GetList(p => inStorageIDs.Contains(p.InStorageID) && p.CustomerCode == customerCode && !p.InStorageOrSettlementRelationals.Any() && (p.PaymentTypeID == 3 || p.PaymentTypeID == 4));

            if (!listInStorageInfo.Any()) throw new BusinessLogicException("该客户下没有未生成结算单的入仓单");

            //获取结算单号
            string settlementNumber = SequenceNumberService.GetSequenceNumber(PrefixCode.SettlementID);

            List<ReceivingExpensesEditExt> listReceivingExpenses = new List<ReceivingExpensesEditExt>();
            List<WayBillInfo> listWayBillInfo = new List<WayBillInfo>();

            foreach (InStorageInfo inStorageInfo in listInStorageInfo)
            {
                _inStorageOrSettlementRelationalRepository.Add(new InStorageOrSettlementRelational()
                    {
                        InStorageID = inStorageInfo.InStorageID,
                        SettlementNumber = settlementNumber,
                    });


                foreach (WayBillInfo wayBillInfo in inStorageInfo.WayBillInfos)
                {
                    listWayBillInfo.Add(wayBillInfo);
                    listReceivingExpenses.Add(_receivingExpensRepository.GetReceivingExpensesEditEx(wayBillInfo.WayBillNumber));
                }
            }

            var customer = _customerRepository.GetFiltered(c=>c.CustomerCode==customerCode).FirstOrDefault();
            var customerManagerInfos = _customerManagerInfoRepository.GetFiltered(c=>c.Name==customer.CustomerManager).FirstOrDefault();

            SettlementInfo settlementInfo=new SettlementInfo()
                {
                    SettlementNumber = settlementNumber,
                    CustomerCode = customerCode,
                    TotalNumber = listInStorageInfo.Select(s=>s.TotalQty.Value).Sum(),
                    TotalWeight = listInStorageInfo.Select(s => s.PhysicalTotalWeight).Sum(),
                    TotalSettleWeight = listInStorageInfo.Select(s => s.TotalWeight.Value).Sum(),
                    TotalFee = listReceivingExpenses.Select(s => s.TotalFeeOriginal.Value).Sum(),
                    Status = 1,
                    SalesMan = customer.CustomerManager,
                    SalesManTel = customerManagerInfos==null?"":(customerManagerInfos.Tel??customerManagerInfos.Mobile),
                    CreatedBy = _workContext.User.UserUame,
                    CreatedOn = DateTime.Now,
                    LastUpdatedBy = _workContext.User.UserUame,
                    LastUpdatedOn = DateTime.Now,
                };

            _settlementInfoRepository.Add(settlementInfo);

            var query =
                from w in listWayBillInfo
                join r in listReceivingExpenses on w.WayBillNumber equals r.WayBillNumber
                group w by w.InShippingMethodID;

            var listSettlementDetailsInfo = from q in query
                                            select
                                                new SettlementDetailsInfo
                                                    {
                                                        SettlementNumber=settlementNumber,
                                                        ShippingMethodID = q.Key,
                                                        ShippingMethodName = q.Select(w => w.InShippingMethodName).FirstOrDefault(),
                                                        TotalNumber = q.Count(),
                                                        TotalWeight = q.Select(w => w.Weight.Value).Sum(),
                                                        TotalSettleWeight = q.Select(w => w.SettleWeight.Value).Sum(),
                                                        TotalFee = listReceivingExpenses.Where(r => q.Select(w => w.WayBillNumber).Contains(r.WayBillNumber)).Select(r => r.TotalFeeOriginal.Value).Sum()
                                                    };

            foreach (SettlementDetailsInfo settlementDetailsInfo in listSettlementDetailsInfo)
            {
                _settlementDetailsInfoRepository.Add(settlementDetailsInfo);
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
            {
                _settlementInfoRepository.UnitOfWork.Commit();
                _inStorageOrSettlementRelationalRepository.UnitOfWork.Commit();
                _settlementDetailsInfoRepository.UnitOfWork.Commit();

                transaction.Complete();

                return settlementNumber;
            }
        }

	    public IPagedList<SettlementInfoExt> GetSettlementInfoList(SettlementInfoParam param)
	    {
		   return _settlementInfoRepository.GetSettlementInfoList(param);
	    }

        public IPagedList<SettlementSummaryExt> GetSettlementSummaryExtPagedList(SettlementSummaryParam param)
        {
            return _settlementInfoRepository.GetSettlementSummaryExtPagedList(param);
        }
    }
}
