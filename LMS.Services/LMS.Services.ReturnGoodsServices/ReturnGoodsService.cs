using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LMS.Data.Repository;
using LMS.Services.FreightServices;
using LMS.Services.FinancialServices;
using LMS.Services.SequenceNumber;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Services.ReturnGoodsServices
{
    public class ReturnGoodsService : IReturnGoodsService
    {
        private readonly IReturnGoodsRepository _returnGoodsRepository;
        private readonly IWayBillInfoRepository _wayBillInfoRepository;
        private readonly ICustomerOrderInfoRepository _customerOrderInfoRepository;
        private readonly ICustomerBalanceRepository _customerBalanceRepository;
        private readonly ICustomerAmountRecordRepository _customerAmountRecordRepository;
        private readonly IWorkContext _workContext;
        private readonly IFreightService _freightService;
        private readonly IFinancialService _financialService;
        private readonly IWayBillEventLogRepository _wayBillEventLogRepository;
        private readonly IReceivingExpenseInfoRepository _receivingExpenseInfoRepository;
        private readonly IReceivingExpensRepository _receivingExpensRepository;
        public ReturnGoodsService(IReturnGoodsRepository returnGoodsRepository,
            IWayBillInfoRepository wayBillInfoRepository,
            ICustomerOrderInfoRepository customerOrderInfoRepository,
            ICustomerBalanceRepository customerBalanceRepository,
            ICustomerAmountRecordRepository customerAmountRecordRepository,
            IWorkContext workContext,
            IWayBillEventLogRepository wayBillEventLogRepository,
            IFreightService freightService,
            IFinancialService financialService,
            IReceivingExpenseInfoRepository receivingExpenseInfoRepository,
            IReceivingExpensRepository receivingExpensRepository)
        {
            _returnGoodsRepository = returnGoodsRepository;
            _wayBillInfoRepository = wayBillInfoRepository;
            _customerOrderInfoRepository = customerOrderInfoRepository;
            _customerAmountRecordRepository = customerAmountRecordRepository;
            _customerBalanceRepository = customerBalanceRepository;
            _wayBillEventLogRepository = wayBillEventLogRepository;
            _freightService = freightService;
            _workContext = workContext;
            _financialService = financialService;
            _receivingExpenseInfoRepository = receivingExpenseInfoRepository;
            _receivingExpensRepository = receivingExpensRepository;
        }


        public void Add(ReturnGoods returnGoods)
        {
            if (_returnGoodsRepository.Exists(p => p.WayBillNumber == returnGoods.WayBillNumber)) return;
            _returnGoodsRepository.Add(returnGoods);
            _returnGoodsRepository.UnitOfWork.Commit();
        }

        public void BatchAddReturnGoods(List<ReturnGoodsExt> list)
        {

            var returnList = list.FindAll(p => p.IsDirectReturnGoods).Where(returnGoods =>
                                                                            !_returnGoodsRepository.Exists(p => p.WayBillNumber == returnGoods.WayBillNumber)).ToList();
            if (returnList.Any())
            {
                //直接退货操作
                foreach (var returnGoods in returnList)
                {

                    #region 修改订单、运单状态

                    var wayBillInfo = _wayBillInfoRepository.Get(returnGoods.WayBillNumber);
                    if (wayBillInfo != null)
                    {
                        if (wayBillInfo.Status != (int)WayBill.StatusEnum.Send && wayBillInfo.Status != (int)WayBill.StatusEnum.WaitOrder)
                        {
                            throw new Exception(string.Format("运单{0}的状态不是已发货或待转单", returnGoods.WayBillNumber));
                        }

                        wayBillInfo.Status = WayBill.StatusToValue(WayBill.StatusEnum.ReGoodsInStorage);
                        wayBillInfo.LastUpdatedOn = DateTime.Now;
                        wayBillInfo.LastUpdatedBy = returnGoods.UserName;
                        wayBillInfo.CustomerOrderInfo.Status =
                            CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.ReGoodsInStorage);
                        wayBillInfo.CustomerOrderInfo.LastUpdatedBy = returnGoods.UserName;
                        wayBillInfo.CustomerOrderInfo.LastUpdatedOn = DateTime.Now;
                        _wayBillInfoRepository.Modify(wayBillInfo);
                    }
                    #endregion

                    #region 录入内部信息

                    if (wayBillInfo != null)
                    {
                        //Add By zxq
                        //Time:2014-09-15
                        var wayBillEventLog = new WayBillEventLog()
                        {
                            WayBillNumber = wayBillInfo.WayBillNumber,
                            EventCode = (int)WayBillEvent.EventCodeEnum.OutStorage,
                            Description = WayBillEvent.GetEventCodeDescription((int)WayBillEvent.EventCodeEnum.OutStorage),
                            EventDate = DateTime.Now,
                            LastUpdatedOn = DateTime.Now,
                            Operator = returnGoods.UserName,
                        };

                        _wayBillEventLogRepository.Add(wayBillEventLog);
                        
                    }

                    #endregion

                    //退货详细
                    if (wayBillInfo != null)
                    {
                        ReceivingExpensesEditExt receivingExpensesEditExt = new ReceivingExpensesEditExt();
                        receivingExpensesEditExt =
                            _financialService.GetReceivingExpensesEditExt(wayBillInfo.WayBillNumber);
                        ReturnGoods rgGoods = new ReturnGoods()
                            {
                                WayBillNumber = returnGoods.WayBillNumber,
                                Weight = returnGoods.Weight != 0 ? returnGoods.Weight : wayBillInfo.Weight.Value,
                                CreatedBy = returnGoods.UserName,
                                CreatedOn = DateTime.Now,
                                IsReturnShipping = returnGoods.IsReturnShipping,
                                LastUpdatedBy = returnGoods.UserName,
                                LastUpdatedOn = DateTime.Now,
                                Reason = returnGoods.Reason,
                                ReasonRemark = returnGoods.ReasonRemark,
                                ReGoodsId=SequenceNumberService.GetSequenceNumber(PrefixCode.ReturnGoodsID),
                                Type = returnGoods.Type,
                                Status = (int)ReturnGood.ReturnStatusEnum.UnAudited,
                                ReturnSource = (int)ReturnGood.ReturnSourceStatusEnum.CSReturn
                            };


                        //是否退运费
                        if (returnGoods.IsReturnShipping)
                        {
                            if (receivingExpensesEditExt != null)
                            {
                                if (receivingExpensesEditExt.TotalFeeFinal.HasValue)
                                {
                                    rgGoods.ShippingFee = receivingExpensesEditExt.TotalFeeFinal ?? 0;
                                }
                                else if (receivingExpensesEditExt.TotalFeeOriginal.HasValue)
                                {
                                    rgGoods.ShippingFee = receivingExpensesEditExt.TotalFeeOriginal ?? 0;
                                }
                            }
                            else
                            {
                                rgGoods.ShippingFee = 0;
                            }
                        }
                        else
                        {
                            rgGoods.ShippingFee = 0;
                        }
                        _returnGoodsRepository.Add(rgGoods);
                    }
                    using (var transaction = new TransactionScope())
                    {
                        
                        _returnGoodsRepository.UnitOfWork.Commit();
                        _wayBillInfoRepository.UnitOfWork.Commit();
                        _wayBillEventLogRepository.UnitOfWork.Commit();
                        _receivingExpensRepository.UnitOfWork.Commit();
                        transaction.Complete();
                    }
                }
            }
        }

        public IEnumerable<ReturnGoods> GetList()
        {
            return _returnGoodsRepository.GetAll();
        }

        /// <summary>
        /// 获取退货信息查询分页列表
        /// Add by zhengsong
        /// Time:2014-05-16
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IPagedList<ReturnWayBillModelExt> GetPagedList(ReturnWayBillParam param)
        {
            return _returnGoodsRepository.GetPagedListReturnWayBill(param);
        }

        /// <summary>
        /// 获取需要导出的退货信息
        /// Add by zhengsong
        /// Time:2014-05-17
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<ReturnWayBillModelExt> GetExportReturnWayBillList(ReturnWayBillParam param)
        {
            return _returnGoodsRepository.GetReturnWayBillList(param);
        }

        /// <summary>
        /// 批量审核通过退回
        /// Add By zhengsong
        /// </summary>
        /// <param name="wayBilllist"></param>
        /// <returns></returns>
        public bool ReturnAuditList(string[] wayBilllist)
        {
            bool reuslt = false;
            try
            {
                var returnGoods = _returnGoodsRepository.GetList(p => wayBilllist.Contains(p.WayBillNumber) && p.Status != (int)ReturnGood.ReturnStatusEnum.Audited);
                var waybills = _wayBillInfoRepository.GetList(p => wayBilllist.Contains(p.WayBillNumber)  && p.Status != (int) WayBill.StatusEnum.Return);

                var receivingExpenses = _receivingExpensRepository.GetList(p => wayBilllist.Contains(p.WayBillNumber));
                returnGoods.ForEach(p =>
                    {
                        //修改退货记录表状态
                        p.Status = (int) ReturnGood.ReturnStatusEnum.Audited;
                        p.Auditor = _workContext.User.UserUame;
                        p.AuditorDate = DateTime.Now;
                        p.LastUpdatedBy = _workContext.User.UserUame;
                        p.LastUpdatedOn = DateTime.Now;

                        //修改运单，定单状态
                        var waybill = waybills.FirstOrDefault(z => z.WayBillNumber == p.WayBillNumber);
                        if (waybill != null)
                        {
                            waybill.Status = (int) WayBill.StatusEnum.Return;
                            waybill.LastUpdatedBy = _workContext.User.UserUame;
                            waybill.LastUpdatedOn = DateTime.Now;
                            waybill.IsHold = false;
                            waybill.CustomerOrderInfo.Status = (int) CustomerOrder.StatusEnum.Return;
                            waybill.CustomerOrderInfo.IsHold = false;
                            waybill.CustomerOrderInfo.LastUpdatedBy = _workContext.User.UserUame;
                            waybill.CustomerOrderInfo.LastUpdatedOn = DateTime.Now;
                            _wayBillInfoRepository.Modify(waybill);
                        }
                        var receivingExpense = receivingExpenses.FirstOrDefault(z => z.WayBillNumber == p.WayBillNumber);
                        //是否退运费
                        if (p.IsReturnShipping)
                        {
                            //添加一个收货费用明细退费记录，并且修改收货费用审核表状态
                            ReceivingExpensesEditExt receivingExpensesEditExt =
                                _financialService.GetReceivingExpensesEditExt(p.WayBillNumber);
                            //如果没有费用就不添加费用明细
                            if (receivingExpensesEditExt.TotalFeeFinal.HasValue ||
                                receivingExpensesEditExt.TotalFeeOriginal.HasValue)
                            {
                                #region 在收货费用详细表添加一个退回数据，状态为4
                                if (receivingExpensesEditExt.TotalFeeFinal.HasValue)
                                {
                                    p.ShippingFee = receivingExpensesEditExt.TotalFeeFinal.Value;
                                }
                                else
                                {
                                    p.ShippingFee = receivingExpensesEditExt.TotalFeeOriginal.Value;
                                }
                                _financialService.UpdateReceivingExpenseInfo(p.WayBillNumber, _workContext.User.UserUame);

                                #endregion
                            }
                            else
                            {
                                //要退费用但是还没生成
                                if (receivingExpense !=null) receivingExpense.IsNoGet = true;
                                _receivingExpensRepository.Modify(receivingExpense);
                            }
                        }
                        else
                        {
                            //修改收货费用审核表状态
                            if (receivingExpense != null)
                            {
                                //不退费用测不修改货费用审核表状态跟验收时间
                                //receivingExpense.Status = (int)Financial.ReceivingExpenseStatusEnum.Audited;
                                //receivingExpense.AcceptanceDate = DateTime.Now;
                                receivingExpense.LastUpdatedBy = _workContext.User.UserUame;
                                receivingExpense.LastUpdatedOn = DateTime.Now;
                                _receivingExpensRepository.Modify(receivingExpense);
                            }
                            p.ShippingFee = 0;
                        }
                        _returnGoodsRepository.Modify(p);
                    });
                _returnGoodsRepository.UnitOfWork.Commit();
                _wayBillInfoRepository.UnitOfWork.Commit();
                _receivingExpensRepository.UnitOfWork.Commit();
                reuslt = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return reuslt;
        }

        /// <summary>
        /// 批量修改审核通过
        /// Add By zhengsong
        /// </summary>
        /// <param name="wayBilllist"></param>
        /// <param name="type"></param>
        /// <param name="returnReason"></param>
        /// <param name="isReturnShipping"></param>
        /// <returns></returns>
        public bool UpdateReturnAuditList(string[] wayBilllist,int type, string returnReason,bool isReturnShipping)
        {
            bool reuslt = false;
            try
            {
                var returnGoods = _returnGoodsRepository.GetList(p => wayBilllist.Contains(p.WayBillNumber) && p.Status != (int)ReturnGood.ReturnStatusEnum.Audited);
                var waybills = _wayBillInfoRepository.GetList(p => wayBilllist.Contains(p.WayBillNumber) && p.Status != (int)WayBill.StatusEnum.Return);
                var receivingExpenses = _receivingExpensRepository.GetList(p => wayBilllist.Contains(p.WayBillNumber));
                returnGoods.ForEach(p =>
                {
                    //修改退货记录表状态
                    p.Status = (int)ReturnGood.ReturnStatusEnum.Audited;
                    p.Auditor = _workContext.User.UserUame;
                    p.AuditorDate = DateTime.Now;
                    p.LastUpdatedBy = _workContext.User.UserUame;
                    p.LastUpdatedOn = DateTime.Now;
                    p.Type = type;
                    p.Reason = returnReason;
                    p.IsReturnShipping = isReturnShipping;
                    _returnGoodsRepository.Modify(p);

                    //修改运单，定单状态
                    var waybill = waybills.FirstOrDefault(z => z.WayBillNumber == p.WayBillNumber);
                    if (waybill != null)
                    {
                        waybill.Status = (int)WayBill.StatusEnum.Return;
                        waybill.LastUpdatedBy = _workContext.User.UserUame;
                        waybill.LastUpdatedOn = DateTime.Now;
                        waybill.CustomerOrderInfo.Status = (int)CustomerOrder.StatusEnum.Return;
                        waybill.CustomerOrderInfo.LastUpdatedBy = _workContext.User.UserUame;
                        waybill.CustomerOrderInfo.LastUpdatedOn = DateTime.Now;
                        _wayBillInfoRepository.Modify(waybill);
                    }


                    var receivingExpense = receivingExpenses.FirstOrDefault(z => z.WayBillNumber == p.WayBillNumber);

                    //是否退运费
                    if (p.IsReturnShipping)
                    {
                        //添加一个收货费用明细退费记录，并且修改收货费用审核表状态
                        ReceivingExpensesEditExt receivingExpensesEditExt =
                            _financialService.GetReceivingExpensesEditExt(p.WayBillNumber);
                        //如果没有费用就不添加费用明细
                        if (receivingExpensesEditExt.TotalFeeFinal.HasValue ||
                            receivingExpensesEditExt.TotalFeeOriginal.HasValue)
                        {
                            #region 在收货费用详细表添加一个退回数据，状态为4
                            if (receivingExpensesEditExt.TotalFeeFinal.HasValue)
                            {
                                p.ShippingFee = receivingExpensesEditExt.TotalFeeFinal.Value;
                            }
                            else
                            {
                                p.ShippingFee = receivingExpensesEditExt.TotalFeeOriginal.Value;
                            }
                            _financialService.UpdateReceivingExpenseInfo(p.WayBillNumber, _workContext.User.UserUame);

                            #endregion
                        }
                        else
                        {
                            //要退费用但是还没生成
                            if (receivingExpense != null) receivingExpense.IsNoGet = true;
                            _receivingExpensRepository.Modify(receivingExpense);
                        }
                    }
                    else
                    {
                        //修改收货费用审核表状态
                        if (receivingExpense != null)
                        {
                            p.ShippingFee = 0;
                            //不退费用就不修改验收时间和状态
                            //receivingExpense.Status = (int)Financial.ReceivingExpenseStatusEnum.Audited;
                            //receivingExpense.AcceptanceDate = DateTime.Now;
                            receivingExpense.LastUpdatedBy = _workContext.User.UserUame;
                            receivingExpense.LastUpdatedOn = DateTime.Now;
                            _receivingExpensRepository.Modify(receivingExpense);
                        }
                    }

                });
                _returnGoodsRepository.UnitOfWork.Commit();
                _wayBillInfoRepository.UnitOfWork.Commit();
                _receivingExpensRepository.UnitOfWork.Commit();
                reuslt = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return reuslt;
        }
    }
}
