using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Transactions;
using LighTake.Infrastructure.Http;
using LighTake.Infrastructure.Http.Infrastructure;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LMS.Services.SequenceNumber;
using LighTake.Infrastructure.Common;
using LMS.Data.Entity.ExtModel;
using LMS.Data.Entity.Param;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Common.Caching;


namespace LMS.Services.FinancialServices
{

    public class FinancialService : IFinancialService
    {
        private IReceivingExpensRepository _receivingExpensRepository;
        private IDeliveryFeeRepository _deliveryFeeRepository;
        private IReceivingExpenseInfoRepository _receivingExpenseInfoRepository;
        private IReceivingBillRepository _receivingBillRepository;
		private IWorkContext _workContext;
        private ICustomerRepository _customerRepository;
        private IWayBillInfoRepository _wayBillInfoRepository;
        private IWaybillPackageDetailRepository _waybillPackageDetailRepository;
        private IDeliveryFeeInfoRepository _deliveryFeeInfoRepository;
        private ICountryRepository _countryRepository;
	    private IJobErrorLogRepository _iJobErrorLogRepository;
	    private IDeliveryDeviationRepository _deliveryDeviationRepository;
        private ICustomerOrderInfoRepository _customerOrderInfoRepository;
        private IDeliveryImportAccountChecksTempRepository _deliveryImportAccountChecksTempRepository;
        public FinancialService(IWorkContext workContext, IReceivingExpensRepository receivingExpensRepository,
                                IReceivingBillRepository receivingBillRepository,
                                IDeliveryFeeRepository deliveryFeeRepository,
                                IReceivingExpenseInfoRepository receivingExpenseInfoRepository,
                                ICustomerRepository customerRepository,
                                IWayBillInfoRepository wayBillInfoRepository,
                                IWaybillPackageDetailRepository waybillPackageDetailRepository,
                                IDeliveryFeeInfoRepository deliveryFeeInfoRepository,
                                ICountryRepository countryRepository,
								IJobErrorLogRepository jobErrorLogRepository,
			                    IDeliveryDeviationRepository deliveryDeviationRepository,
                                ICustomerOrderInfoRepository customerOrderInfoRepository,
                                IDeliveryImportAccountChecksTempRepository deliveryImportAccountChecksTempRepository
            )
        {
            _receivingExpensRepository = receivingExpensRepository;
            _receivingExpenseInfoRepository = receivingExpenseInfoRepository;
            _receivingBillRepository = receivingBillRepository;
            _workContext = workContext;
            this._deliveryFeeRepository = deliveryFeeRepository;
            _customerRepository = customerRepository;
            _wayBillInfoRepository = wayBillInfoRepository;
            _waybillPackageDetailRepository = waybillPackageDetailRepository;
            _deliveryFeeInfoRepository = deliveryFeeInfoRepository;
            _countryRepository = countryRepository;
	        _iJobErrorLogRepository = jobErrorLogRepository;
	        _deliveryDeviationRepository = deliveryDeviationRepository;
            _customerOrderInfoRepository = customerOrderInfoRepository;
            _deliveryImportAccountChecksTempRepository = deliveryImportAccountChecksTempRepository;
        }

        public IPagedList<ReceivingExpenseExt> GetReceivingExpensePagedList(FinancialParam financialParam)
        {
            return _receivingExpensRepository.GetInFeeInfoExtPagedList(financialParam);
        }

        public ReceivingExpensesEditExt GetReceivingExpensesEditExt(string wayBillNumber)
        {
            return _receivingExpensRepository.GetReceivingExpensesEditEx(wayBillNumber);
        }


		/// <summary>
		/// 获取财务错误日志列表
		/// yungchu
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		public IPagedList<JobErrorLogExt> GetJobErrorLogsPagedList(JobErrorLogsParam param)
		{
			return _iJobErrorLogRepository.GetPagedList(param);
		}



	    /// <summary>
        /// 修改收费费用审核详细表
        /// Add By zhengsong
        /// Time:2014-07-14
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <param name="userUame"></param>
        /// <param name="returnFee"></param>
        public void UpdateReceivingExpenseInfo(string wayBillNumber, string userUame, bool returnFee = true)
        {
            if (returnFee)
            {
                //插入收费费用审核详细表
                //Add By zhengsong
                //Time:2014-07-12
                ReceivingExpensesEditExt receivingExpensesEditExt = new ReceivingExpensesEditExt();
                receivingExpensesEditExt = GetReceivingExpensesEditExt(wayBillNumber);
                var receivingExpenseInfoList = new List<ReceivingExpenseInfo>();
                if (receivingExpensesEditExt != null)
                {
                    var receivingExpenseID = GetReceivingExpenseID(wayBillNumber);
                    if (receivingExpenseID != 0)
                    {
                        receivingExpensesEditExt.SurchargeFinal = receivingExpensesEditExt.SurchargeFinal ?? 0;
                        receivingExpensesEditExt.TariffPrepayFeeFinal =
                            receivingExpensesEditExt.TariffPrepayFeeFinal ?? 0;
                        receivingExpensesEditExt.FreightFinal = receivingExpensesEditExt.FreightFinal ?? 0;
                        receivingExpensesEditExt.FuelChargeFinal = receivingExpensesEditExt.FuelChargeFinal ?? 0;
                        receivingExpensesEditExt.RegisterFinal = receivingExpensesEditExt.RegisterFinal ?? 0;

                        receivingExpensesEditExt.FreightOriginal = receivingExpensesEditExt.FreightOriginal ?? 0;
                        receivingExpensesEditExt.FuelChargeOriginal = receivingExpensesEditExt.FuelChargeOriginal ?? 0;
                        receivingExpensesEditExt.TariffPrepayFeeOriginal = receivingExpensesEditExt.TariffPrepayFeeOriginal ?? 0;
                        receivingExpensesEditExt.SurchargeOriginal= receivingExpensesEditExt.SurchargeOriginal ?? 0;
                        receivingExpensesEditExt.RegisterOriginal = receivingExpensesEditExt.RegisterOriginal ?? 0;
                        if (receivingExpensesEditExt.TotalFeeFinal.HasValue)
                        {
                            //退回运费
                            if (receivingExpensesEditExt.FreightFinal > 0)
                            {
                                receivingExpenseInfoList.Add(new ReceivingExpenseInfo
                                {
                                    ReceivingExpenseID = receivingExpenseID,
                                    Amount = receivingExpensesEditExt.FreightFinal * -1,
                                    CreatedBy = userUame,
                                    CreatedOn = DateTime.Now,
                                    FeeTypeID = (int)CustomerOrder.FeeTypeEnum.Freight,
                                    LastUpdatedBy = userUame,
                                    LastUpdatedOn = DateTime.Now,
                                    OperationType = 4
                                });
                            }
                            //退回燃油费
                            if (receivingExpensesEditExt.FuelChargeFinal > 0)
                            {
                                receivingExpenseInfoList.Add(new ReceivingExpenseInfo
                                    {
                                        ReceivingExpenseID = receivingExpenseID,
                                        Amount = receivingExpensesEditExt.FuelChargeFinal*-1,
                                        CreatedBy = userUame,
                                        CreatedOn = DateTime.Now,
                                        FeeTypeID = (int) CustomerOrder.FeeTypeEnum.FuelCharge,
                                        LastUpdatedBy = userUame,
                                        LastUpdatedOn = DateTime.Now,
                                        OperationType = 4
                                    });
                            }
                            //退回挂号费
                            if (receivingExpensesEditExt.RegisterFinal > 0)
                            {
                                receivingExpenseInfoList.Add(new ReceivingExpenseInfo
                                    {
                                        ReceivingExpenseID = receivingExpenseID,
                                        Amount = receivingExpensesEditExt.RegisterFinal*-1,
                                        CreatedBy = userUame,
                                        CreatedOn = DateTime.Now,
                                        FeeTypeID = (int) CustomerOrder.FeeTypeEnum.Register,
                                        LastUpdatedBy = userUame,
                                        LastUpdatedOn = DateTime.Now,
                                        OperationType = 4
                                    });
                            }

                            //退回附加费
                            if (receivingExpensesEditExt.SurchargeFinal > 0)
                            {
                                receivingExpenseInfoList.Add(new ReceivingExpenseInfo
                                    {
                                        ReceivingExpenseID = receivingExpenseID,
                                        Amount = receivingExpensesEditExt.SurchargeFinal*-1,
                                        CreatedBy = userUame,
                                        CreatedOn = DateTime.Now,
                                        FeeTypeID = (int) CustomerOrder.FeeTypeEnum.Surcharge,
                                        LastUpdatedBy = userUame,
                                        LastUpdatedOn = DateTime.Now,
                                        OperationType = 4
                                    });
                            }
                            //退回关税预付服务费
                            if (receivingExpensesEditExt.TariffPrepayFeeFinal > 0)
                            {
                                receivingExpenseInfoList.Add(new ReceivingExpenseInfo
                                    {
                                        ReceivingExpenseID = receivingExpenseID,
                                        Amount = receivingExpensesEditExt.TariffPrepayFeeFinal*-1,
                                        CreatedBy = userUame,
                                        CreatedOn = DateTime.Now,
                                        FeeTypeID = (int) CustomerOrder.FeeTypeEnum.TariffPrepayFee,
                                        LastUpdatedBy = userUame,
                                        LastUpdatedOn = DateTime.Now,
                                        OperationType = 4
                                    });
                            }
                            //退回特殊费
                            if ((receivingExpensesEditExt.TotalFeeFinal - receivingExpensesEditExt.TariffPrepayFeeFinal -
                                 receivingExpensesEditExt.SurchargeFinal - receivingExpensesEditExt.RegisterFinal -
                                 receivingExpensesEditExt.FuelChargeFinal - receivingExpensesEditExt.FreightFinal) > 0)
                            {
                                receivingExpenseInfoList.Add(new ReceivingExpenseInfo
                                    {
                                        ReceivingExpenseID = receivingExpenseID,
                                        Amount =
                                            (receivingExpensesEditExt.TotalFeeFinal -
                                             receivingExpensesEditExt.TariffPrepayFeeFinal -
                                             receivingExpensesEditExt.SurchargeFinal -
                                             receivingExpensesEditExt.RegisterFinal -
                                             receivingExpensesEditExt.FuelChargeFinal -
                                             receivingExpensesEditExt.FreightFinal)*-1,
                                        CreatedBy = userUame,
                                        CreatedOn = DateTime.Now,
                                        FeeTypeID = (int) CustomerOrder.FeeTypeEnum.SpecialFee,
                                        LastUpdatedBy = userUame,
                                        LastUpdatedOn = DateTime.Now,
                                        OperationType = 4
                                    });
                            }
                            receivingExpenseInfoList.ForEach(p => _receivingExpenseInfoRepository.Add(p));
                        }
                        else if (receivingExpensesEditExt.TotalFeeOriginal.HasValue)
                        {
                            //退回运费
                            if (receivingExpensesEditExt.FreightOriginal > 0)
                            {
                                receivingExpenseInfoList.Add(new ReceivingExpenseInfo
                                    {
                                        ReceivingExpenseID = receivingExpenseID,
                                        Amount = receivingExpensesEditExt.FreightOriginal*-1,
                                        CreatedBy = userUame,
                                        CreatedOn = DateTime.Now,
                                        FeeTypeID = (int) CustomerOrder.FeeTypeEnum.Freight,
                                        LastUpdatedBy = userUame,
                                        LastUpdatedOn = DateTime.Now,
                                        OperationType = 4
                                    });
                            }
                            //退回燃油费
                            if (receivingExpensesEditExt.FuelChargeOriginal > 0)
                            {
                                receivingExpenseInfoList.Add(new ReceivingExpenseInfo
                                    {
                                        ReceivingExpenseID = receivingExpenseID,
                                        Amount = receivingExpensesEditExt.FuelChargeOriginal*-1,
                                        CreatedBy = userUame,
                                        CreatedOn = DateTime.Now,
                                        FeeTypeID = (int) CustomerOrder.FeeTypeEnum.FuelCharge,
                                        LastUpdatedBy = userUame,
                                        LastUpdatedOn = DateTime.Now,
                                        OperationType = 4
                                    });
                            }
                            //退回挂号费
                            if (receivingExpensesEditExt.RegisterOriginal > 0)
                            {
                                receivingExpenseInfoList.Add(new ReceivingExpenseInfo
                                    {
                                        ReceivingExpenseID = receivingExpenseID,
                                        Amount = receivingExpensesEditExt.RegisterOriginal*-1,
                                        CreatedBy = userUame,
                                        CreatedOn = DateTime.Now,
                                        FeeTypeID = (int) CustomerOrder.FeeTypeEnum.Register,
                                        LastUpdatedBy = userUame,
                                        LastUpdatedOn = DateTime.Now,
                                        OperationType = 4
                                    });
                            }

                            //退回附加费
                            if (receivingExpensesEditExt.SurchargeOriginal > 0)
                            {
                                receivingExpenseInfoList.Add(new ReceivingExpenseInfo
                                    {
                                        ReceivingExpenseID = receivingExpenseID,
                                        Amount = receivingExpensesEditExt.SurchargeOriginal*-1,
                                        CreatedBy = userUame,
                                        CreatedOn = DateTime.Now,
                                        FeeTypeID = (int) CustomerOrder.FeeTypeEnum.Surcharge,
                                        LastUpdatedBy = userUame,
                                        LastUpdatedOn = DateTime.Now,
                                        OperationType = 4
                                    });
                            }
                            //退回关税预付服务费
                            if (receivingExpensesEditExt.TariffPrepayFeeOriginal > 0)
                            {
                                receivingExpenseInfoList.Add(new ReceivingExpenseInfo
                                    {
                                        ReceivingExpenseID = receivingExpenseID,
                                        Amount = receivingExpensesEditExt.TariffPrepayFeeOriginal*-1,
                                        CreatedBy = userUame,
                                        CreatedOn = DateTime.Now,
                                        FeeTypeID = (int) CustomerOrder.FeeTypeEnum.TariffPrepayFee,
                                        LastUpdatedBy = userUame,
                                        LastUpdatedOn = DateTime.Now,
                                        OperationType = 4
                                    });
                            }
                            receivingExpenseInfoList.ForEach(p => _receivingExpenseInfoRepository.Add(p));
                        }
                        var receivingExpen = _receivingExpensRepository.GetFiltered(p => p.WayBillNumber == wayBillNumber).FirstOrDefault();
                        if (receivingExpen != null)
                        {
                            receivingExpen.AcceptanceDate = DateTime.Now;
                            _receivingExpensRepository.Modify(receivingExpen);
                        }

                    }
                }


            }

            //更改费用表状态

            var receivingExpens =
                _receivingExpensRepository.First(p => p.WayBillNumber == wayBillNumber);
            if (receivingExpens != null)
            {
                try
                {
                    receivingExpens.Status = Financial.DeliveryFeeStatusEnum.Audited.GetDeliveryFeeStatusValue();
                    receivingExpens.AcceptanceDate = DateTime.Now;
                    receivingExpens.IsNoGet = true;
                    receivingExpens.LastUpdatedBy = userUame;
                    receivingExpens.LastUpdatedOn = DateTime.Now;
                    _receivingExpensRepository.Modify(receivingExpens);
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    throw;
                }
            }
            _receivingExpensRepository.UnitOfWork.Commit();
            _receivingExpenseInfoRepository.UnitOfWork.Commit();

        }


        /// <summary>
        /// 修改收货费用
        /// </summary>
        /// <param name="receivingExpensesEditExt"></param>
        public void EditReceivingExpensesEditExt(ReceivingExpensesEditExt receivingExpensesEditExt)
        {
            var receivingExpens = _receivingExpensRepository.GetFiltered(p => p.WayBillNumber == receivingExpensesEditExt.WayBillNumber).First();

            //如果最终费用没有填写，不修改费用信息
            if (!receivingExpensesEditExt.FreightFinal.HasValue &&
                !receivingExpensesEditExt.FuelChargeFinal.HasValue &&
                !receivingExpensesEditExt.RegisterFinal.HasValue &&
                !receivingExpensesEditExt.SurchargeFinal.HasValue &&
                !receivingExpensesEditExt.TariffPrepayFeeFinal.HasValue &&
                !receivingExpensesEditExt.RemoteAreaFeeFinal.HasValue &&
                !receivingExpensesEditExt.TotalFeeFinal.HasValue
                )
            {
                //return;
            }
            else
            {

                var receivingExpenseeditex = GetReceivingExpensesEditExt(receivingExpensesEditExt.WayBillNumber);

                receivingExpensesEditExt.FreightFinal = receivingExpensesEditExt.FreightFinal.HasValue ? receivingExpensesEditExt.FreightFinal : receivingExpenseeditex.FreightOriginal;
                receivingExpensesEditExt.FuelChargeFinal = receivingExpensesEditExt.FuelChargeFinal.HasValue ? receivingExpensesEditExt.FuelChargeFinal : receivingExpenseeditex.FuelChargeOriginal;
                receivingExpensesEditExt.RegisterFinal = receivingExpensesEditExt.RegisterFinal.HasValue ? receivingExpensesEditExt.RegisterFinal : receivingExpenseeditex.RegisterOriginal;
                receivingExpensesEditExt.SurchargeFinal = receivingExpensesEditExt.SurchargeFinal.HasValue ? receivingExpensesEditExt.SurchargeFinal : receivingExpenseeditex.SurchargeOriginal;
                receivingExpensesEditExt.TariffPrepayFeeFinal = receivingExpensesEditExt.TariffPrepayFeeFinal.HasValue ? receivingExpensesEditExt.TariffPrepayFeeFinal : receivingExpenseeditex.TariffPrepayFeeOriginal;
                receivingExpensesEditExt.RemoteAreaFeeFinal = receivingExpensesEditExt.RemoteAreaFeeFinal.HasValue ? receivingExpensesEditExt.RemoteAreaFeeFinal : receivingExpenseeditex.RemoteAreaFeeOriginal;

                //如果没有冲红数据
                if (_receivingExpenseInfoRepository.GetFiltered(p => p.OperationType == 2
                                                                     && p.ReceivingExpenseID == receivingExpens.ReceivingExpenseID).FirstOrDefault() == null)
                {
                    #region 判断费用是否改变

                    if (receivingExpensesEditExt.FreightFinal == receivingExpenseeditex.FreightOriginal &&
                        receivingExpensesEditExt.FuelChargeFinal == receivingExpenseeditex.FuelChargeOriginal &&
                        receivingExpensesEditExt.RegisterFinal == receivingExpenseeditex.RegisterOriginal &&
                        receivingExpensesEditExt.SurchargeFinal == receivingExpenseeditex.SurchargeOriginal &&
                        receivingExpensesEditExt.TariffPrepayFeeFinal == receivingExpenseeditex.TariffPrepayFeeOriginal &&
                        receivingExpensesEditExt.RemoteAreaFeeFinal == receivingExpenseeditex.RemoteAreaFeeOriginal &&
                        receivingExpensesEditExt.TotalFeeFinal == receivingExpenseeditex.TotalFeeOriginal)
                    {
                        return;
                    }

                    #endregion

                    #region 生成冲红数据

                    var listReceivingExpenseInfoChongHong = new List<ReceivingExpenseInfo>();

                    //运费冲红
                    listReceivingExpenseInfoChongHong.Add(new ReceivingExpenseInfo
                        {
                            ReceivingExpenseID = receivingExpens.ReceivingExpenseID,
                            Amount = receivingExpenseeditex.FreightOriginal*-1,
                            CreatedBy = _workContext.User.UserUame,
                            CreatedOn = DateTime.Now,
                            FeeTypeID = (int) CustomerOrder.FeeTypeEnum.Freight,
                            LastUpdatedBy = _workContext.User.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            OperationType = 2
                        });

                    //燃油费冲红
                    listReceivingExpenseInfoChongHong.Add(new ReceivingExpenseInfo
                        {
                            ReceivingExpenseID = receivingExpens.ReceivingExpenseID,
                            Amount = receivingExpenseeditex.FuelChargeOriginal*-1,
                            CreatedBy = _workContext.User.UserUame,
                            CreatedOn = DateTime.Now,
                            FeeTypeID = (int) CustomerOrder.FeeTypeEnum.FuelCharge,
                            LastUpdatedBy = _workContext.User.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            OperationType = 2
                        });

                    //挂号费冲红
                    listReceivingExpenseInfoChongHong.Add(new ReceivingExpenseInfo
                        {
                            ReceivingExpenseID = receivingExpens.ReceivingExpenseID,
                            Amount = receivingExpenseeditex.RegisterOriginal*-1,
                            CreatedBy = _workContext.User.UserUame,
                            CreatedOn = DateTime.Now,
                            FeeTypeID = (int) CustomerOrder.FeeTypeEnum.Register,
                            LastUpdatedBy = _workContext.User.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            OperationType = 2
                        });


                    //附加费冲红
                    listReceivingExpenseInfoChongHong.Add(new ReceivingExpenseInfo
                        {
                            ReceivingExpenseID = receivingExpens.ReceivingExpenseID,
                            Amount = receivingExpenseeditex.SurchargeOriginal*-1,
                            CreatedBy = _workContext.User.UserUame,
                            CreatedOn = DateTime.Now,
                            FeeTypeID = (int) CustomerOrder.FeeTypeEnum.Surcharge,
                            LastUpdatedBy = _workContext.User.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            OperationType = 2
                        });

                    //关税预付服务费冲红
                    listReceivingExpenseInfoChongHong.Add(new ReceivingExpenseInfo
                        {
                            ReceivingExpenseID = receivingExpens.ReceivingExpenseID,
                            Amount = receivingExpenseeditex.TariffPrepayFeeOriginal*-1,
                            CreatedBy = _workContext.User.UserUame,
                            CreatedOn = DateTime.Now,
                            FeeTypeID = (int) CustomerOrder.FeeTypeEnum.TariffPrepayFee,
                            LastUpdatedBy = _workContext.User.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            OperationType = 2
                        });

                    //偏远附加费冲红
                    listReceivingExpenseInfoChongHong.Add(new ReceivingExpenseInfo
                        {
                            ReceivingExpenseID = receivingExpens.ReceivingExpenseID,
                            Amount = receivingExpenseeditex.RemoteAreaFeeOriginal*-1,
                            CreatedBy = _workContext.User.UserUame,
                            CreatedOn = DateTime.Now,
                            FeeTypeID = (int) CustomerOrder.FeeTypeEnum.RemoteAreaFee,
                            LastUpdatedBy = _workContext.User.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            OperationType = 2
                        });

                    listReceivingExpenseInfoChongHong.ForEach(_receivingExpenseInfoRepository.Add);

                    #endregion
                }

                //删除旧的最终数据
                _receivingExpenseInfoRepository.Remove(d => d.OperationType == 3 && d.ReceivingExpenseID == receivingExpens.ReceivingExpenseID);

                #region 生成新最终数据

                var listReceivingExpenseInfoZuiZhong = new List<ReceivingExpenseInfo>();

                //如果
                if (!receivingExpensesEditExt.TotalFeeFinal.HasValue)
                {
                    //运费最终
                    listReceivingExpenseInfoZuiZhong.Add(new ReceivingExpenseInfo
                        {
                            ReceivingExpenseID = receivingExpens.ReceivingExpenseID,
                            Amount = receivingExpensesEditExt.FreightFinal,
                            CreatedBy = _workContext.User.UserUame,
                            CreatedOn = DateTime.Now,
                            FeeTypeID = (int) CustomerOrder.FeeTypeEnum.Freight,
                            LastUpdatedBy = _workContext.User.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            OperationType = 3
                        });

                    //燃油费最终
                    listReceivingExpenseInfoZuiZhong.Add(new ReceivingExpenseInfo
                        {
                            ReceivingExpenseID = receivingExpens.ReceivingExpenseID,
                            Amount = receivingExpensesEditExt.FuelChargeFinal,
                            CreatedBy = _workContext.User.UserUame,
                            CreatedOn = DateTime.Now,
                            FeeTypeID = (int) CustomerOrder.FeeTypeEnum.FuelCharge,
                            LastUpdatedBy = _workContext.User.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            OperationType = 3
                        });

                    //挂号费最终
                    listReceivingExpenseInfoZuiZhong.Add(new ReceivingExpenseInfo
                        {
                            ReceivingExpenseID = receivingExpens.ReceivingExpenseID,
                            Amount = receivingExpensesEditExt.RegisterFinal,
                            CreatedBy = _workContext.User.UserUame,
                            CreatedOn = DateTime.Now,
                            FeeTypeID = (int) CustomerOrder.FeeTypeEnum.Register,
                            LastUpdatedBy = _workContext.User.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            OperationType = 3
                        });


                    //附加费最终
                    listReceivingExpenseInfoZuiZhong.Add(new ReceivingExpenseInfo
                        {
                            ReceivingExpenseID = receivingExpens.ReceivingExpenseID,
                            Amount = receivingExpensesEditExt.SurchargeFinal,
                            CreatedBy = _workContext.User.UserUame,
                            CreatedOn = DateTime.Now,
                            FeeTypeID = (int) CustomerOrder.FeeTypeEnum.Surcharge,
                            LastUpdatedBy = _workContext.User.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            OperationType = 3
                        });

                    //关税预付服务费最终
                    listReceivingExpenseInfoZuiZhong.Add(new ReceivingExpenseInfo
                        {
                            ReceivingExpenseID = receivingExpens.ReceivingExpenseID,
                            Amount = receivingExpensesEditExt.TariffPrepayFeeFinal,
                            CreatedBy = _workContext.User.UserUame,
                            CreatedOn = DateTime.Now,
                            FeeTypeID = (int) CustomerOrder.FeeTypeEnum.TariffPrepayFee,
                            LastUpdatedBy = _workContext.User.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            OperationType = 3
                        });

                    //偏远附加费最终
                    listReceivingExpenseInfoZuiZhong.Add(new ReceivingExpenseInfo
                        {
                            ReceivingExpenseID = receivingExpens.ReceivingExpenseID,
                            Amount = receivingExpensesEditExt.RemoteAreaFeeFinal,
                            CreatedBy = _workContext.User.UserUame,
                            CreatedOn = DateTime.Now,
                            FeeTypeID = (int) CustomerOrder.FeeTypeEnum.RemoteAreaFee,
                            LastUpdatedBy = _workContext.User.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            OperationType = 3
                        });
                }
                else
                {
                    //特殊费最终
                    listReceivingExpenseInfoZuiZhong.Add(new ReceivingExpenseInfo
                        {
                            ReceivingExpenseID = receivingExpens.ReceivingExpenseID,
                            Amount = receivingExpensesEditExt.TotalFeeFinal,
                            CreatedBy = _workContext.User.UserUame,
                            CreatedOn = DateTime.Now,
                            FeeTypeID = (int) CustomerOrder.FeeTypeEnum.SpecialFee,
                            LastUpdatedBy = _workContext.User.UserUame,
                            LastUpdatedOn = DateTime.Now,
                            OperationType = 3
                        });

                }

                listReceivingExpenseInfoZuiZhong.ForEach(_receivingExpenseInfoRepository.Add);

                #endregion

            }

            receivingExpens.Status =(int) Financial.ReceivingExpenseStatusEnum.UnAudited;
            receivingExpens.LastUpdatedBy = _workContext.User.UserUame;
            receivingExpens.LastUpdatedOn = DateTime.Now;

            _receivingExpensRepository.Modify(receivingExpens);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                _receivingExpensRepository.UnitOfWork.Commit();
                _receivingExpenseInfoRepository.UnitOfWork.Commit();

                transaction.Complete();
            }
        }

        /// <summary>
        /// 修改发货费用
        /// </summary>
        /// <param name="deliveryFeeAnomalyEditExt"></param>
        public void EditDeliveryFeeAnomalyEditExt(DeliveryFeeAnomalyEditExt deliveryFeeAnomalyEditExt)
        {
            var deliveryFee = _deliveryFeeRepository.GetFiltered(d => d.WayBillNumber == deliveryFeeAnomalyEditExt.WayBillNumber).First();

            deliveryFee.Status = (int) Financial.DeliveryFeeStatusEnum.Audited;
            deliveryFee.LastUpdatedBy = _workContext.User.UserUame;
            deliveryFee.LastUpdatedOn = DateTime.Now;

            _deliveryFeeInfoRepository.Remove(d => d.OperationType == 3 && d.DeliveryFeeID == deliveryFee.DeliveryFeeID);

            var listDeliveryFeeInfos = new List<DeliveryFeeInfo>();

            //运费最终数据
            if (deliveryFeeAnomalyEditExt.FreightFinal.HasValue && deliveryFeeAnomalyEditExt.FreightFinal>0)
            {
                listDeliveryFeeInfos.Add(new DeliveryFeeInfo()
                    {
                        DeliveryFeeID = deliveryFee.DeliveryFeeID,
                        MoneyChangeTypeID=2,
                        Amount = deliveryFeeAnomalyEditExt.FreightFinal,
                        FeeTypeID = (int)CustomerOrder.FeeTypeEnum.Freight,
                        OperationType=3,
                        CreatedOn=DateTime.Now,
                        CreatedBy = _workContext.User.UserUame,
                        LastUpdatedBy = _workContext.User.UserUame,
                        LastUpdatedOn = DateTime.Now,
                    });
            }

            //燃油费最终
            if (deliveryFeeAnomalyEditExt.FuelChargeFinal.HasValue && deliveryFeeAnomalyEditExt.FuelChargeFinal>0)
            {
                listDeliveryFeeInfos.Add(new DeliveryFeeInfo()
                {
                    DeliveryFeeID = deliveryFee.DeliveryFeeID,
                    MoneyChangeTypeID = 2,
                    Amount = deliveryFeeAnomalyEditExt.FuelChargeFinal,
                    FeeTypeID = (int)CustomerOrder.FeeTypeEnum.FuelCharge,
                    OperationType = 3,
                    CreatedOn = DateTime.Now,
                    CreatedBy = _workContext.User.UserUame,
                    LastUpdatedBy = _workContext.User.UserUame,
                    LastUpdatedOn = DateTime.Now,
                });
            }

            //挂号费最终
            if (deliveryFeeAnomalyEditExt.RegisterFinal.HasValue && deliveryFeeAnomalyEditExt.RegisterFinal>0)
            {
                listDeliveryFeeInfos.Add(new DeliveryFeeInfo()
                {
                    DeliveryFeeID = deliveryFee.DeliveryFeeID,
                    MoneyChangeTypeID = 2,
                    Amount = deliveryFeeAnomalyEditExt.RegisterFinal,
                    FeeTypeID = (int)CustomerOrder.FeeTypeEnum.Register,
                    OperationType = 3,
                    CreatedOn = DateTime.Now,
                    CreatedBy = _workContext.User.UserUame,
                    LastUpdatedBy = _workContext.User.UserUame,
                    LastUpdatedOn = DateTime.Now,
                });
            }

            //关税预付服务费最终
            if (deliveryFeeAnomalyEditExt.TariffPrepayFeeFinal.HasValue && deliveryFeeAnomalyEditExt.TariffPrepayFeeFinal>0)
            {
                listDeliveryFeeInfos.Add(new DeliveryFeeInfo()
                {
                    DeliveryFeeID = deliveryFee.DeliveryFeeID,
                    MoneyChangeTypeID = 2,
                    Amount = deliveryFeeAnomalyEditExt.TariffPrepayFeeFinal,
                    FeeTypeID = (int)CustomerOrder.FeeTypeEnum.TariffPrepayFee,
                    OperationType = 3,
                    CreatedOn = DateTime.Now,
                    CreatedBy = _workContext.User.UserUame,
                    LastUpdatedBy = _workContext.User.UserUame,
                    LastUpdatedOn = DateTime.Now,
                });
            }


            //附加费最终
            if (deliveryFeeAnomalyEditExt.SurchargeFinal.HasValue && deliveryFeeAnomalyEditExt.SurchargeFinal>0)
            {
                listDeliveryFeeInfos.Add(new DeliveryFeeInfo()
                {
                    DeliveryFeeID = deliveryFee.DeliveryFeeID,
                    MoneyChangeTypeID = 2,
                    Amount = deliveryFeeAnomalyEditExt.SurchargeFinal,
                    FeeTypeID = (int)CustomerOrder.FeeTypeEnum.Surcharge,
                    OperationType = 3,
                    CreatedOn = DateTime.Now,
                    CreatedBy = _workContext.User.UserUame,
                    LastUpdatedBy = _workContext.User.UserUame,
                    LastUpdatedOn = DateTime.Now,
                });
            }

            //超长超重超周长费
            if (deliveryFeeAnomalyEditExt.OverWeightLengthGirthFeeFinal.HasValue && deliveryFeeAnomalyEditExt.OverWeightLengthGirthFeeFinal>0)
            {
                listDeliveryFeeInfos.Add(new DeliveryFeeInfo()
                {
                    DeliveryFeeID = deliveryFee.DeliveryFeeID,
                    MoneyChangeTypeID = 2,
                    Amount = deliveryFeeAnomalyEditExt.OverWeightLengthGirthFeeFinal,
                    FeeTypeID = (int)CustomerOrder.FeeTypeEnum.OverWeightLengthGirthFee,
                    OperationType = 3,
                    CreatedOn = DateTime.Now,
                    CreatedBy = _workContext.User.UserUame,
                    LastUpdatedBy = _workContext.User.UserUame,
                    LastUpdatedOn = DateTime.Now,
                });
            }

            //安全附加费
            if (deliveryFeeAnomalyEditExt.SecurityAppendFeeFinal.HasValue && deliveryFeeAnomalyEditExt.SecurityAppendFeeFinal>0)
            {
                listDeliveryFeeInfos.Add(new DeliveryFeeInfo()
                {
                    DeliveryFeeID = deliveryFee.DeliveryFeeID,
                    MoneyChangeTypeID = 2,
                    Amount = deliveryFeeAnomalyEditExt.SecurityAppendFeeFinal,
                    FeeTypeID = (int)CustomerOrder.FeeTypeEnum.SecurityAppendFee,
                    OperationType = 3,
                    CreatedOn = DateTime.Now,
                    CreatedBy = _workContext.User.UserUame,
                    LastUpdatedBy = _workContext.User.UserUame,
                    LastUpdatedOn = DateTime.Now,
                });
            }


            //增值税费
            if (deliveryFeeAnomalyEditExt.AddedTaxFeeFinal.HasValue && deliveryFeeAnomalyEditExt.AddedTaxFeeFinal>0)
            {
                listDeliveryFeeInfos.Add(new DeliveryFeeInfo()
                {
                    DeliveryFeeID = deliveryFee.DeliveryFeeID,
                    MoneyChangeTypeID = 2,
                    Amount = deliveryFeeAnomalyEditExt.AddedTaxFeeFinal,
                    FeeTypeID = (int)CustomerOrder.FeeTypeEnum.AddedTaxFee,
                    OperationType = 3,
                    CreatedOn = DateTime.Now,
                    CreatedBy = _workContext.User.UserUame,
                    LastUpdatedBy = _workContext.User.UserUame,
                    LastUpdatedOn = DateTime.Now,
                });
            }


            //杂费
            if (deliveryFeeAnomalyEditExt.OtherFeeFinal.HasValue && deliveryFeeAnomalyEditExt.OtherFeeFinal>0)
            {
                listDeliveryFeeInfos.Add(new DeliveryFeeInfo()
                {
                    DeliveryFeeID = deliveryFee.DeliveryFeeID,
                    MoneyChangeTypeID = 2,
                    Amount = deliveryFeeAnomalyEditExt.OtherFeeFinal,
                    FeeTypeID = (int)CustomerOrder.FeeTypeEnum.OtherFee,
                    OperationType = 3,
                    CreatedOn = DateTime.Now,
                    CreatedBy = _workContext.User.UserUame,
                    LastUpdatedBy = _workContext.User.UserUame,
                    LastUpdatedOn = DateTime.Now,
                });
            }

            //判断是否需要特殊费
            var totalFee =
                (
                    (!deliveryFeeAnomalyEditExt.FreightFinal.HasValue ? 0 : deliveryFeeAnomalyEditExt.FreightFinal)
                    + (!deliveryFeeAnomalyEditExt.FuelChargeFinal.HasValue ? 0 : deliveryFeeAnomalyEditExt.FuelChargeFinal)
                    + (!deliveryFeeAnomalyEditExt.RegisterFinal.HasValue ? 0 : deliveryFeeAnomalyEditExt.RegisterFinal)
                    + (!deliveryFeeAnomalyEditExt.SurchargeFinal.HasValue ? 0 : deliveryFeeAnomalyEditExt.SurchargeFinal)
                    + (!deliveryFeeAnomalyEditExt.TariffPrepayFeeFinal.HasValue ? 0 : deliveryFeeAnomalyEditExt.TariffPrepayFeeFinal)
                    + (!deliveryFeeAnomalyEditExt.OverWeightLengthGirthFeeFinal.HasValue ? 0 : deliveryFeeAnomalyEditExt.OverWeightLengthGirthFeeFinal)
                    + (!deliveryFeeAnomalyEditExt.SecurityAppendFeeFinal.HasValue ? 0 : deliveryFeeAnomalyEditExt.SecurityAppendFeeFinal)
                    + (!deliveryFeeAnomalyEditExt.AddedTaxFeeFinal.HasValue ? 0 : deliveryFeeAnomalyEditExt.AddedTaxFeeFinal)
                    + (!deliveryFeeAnomalyEditExt.OtherFeeFinal.HasValue ? 0 : deliveryFeeAnomalyEditExt.OtherFeeFinal)
                );


            if (totalFee==0)
            {
                if (!deliveryFeeAnomalyEditExt.TotalFeeFinal.HasValue)
                {
                    throw new BusinessLogicException("总费用与明细必须有填一项");
                }

                //特殊费最终
                listDeliveryFeeInfos.Add(new DeliveryFeeInfo()
                    {

                        DeliveryFeeID = deliveryFee.DeliveryFeeID,
                        MoneyChangeTypeID = 2,
                        Amount = deliveryFeeAnomalyEditExt.TotalFeeFinal-totalFee,
                        FeeTypeID = (int) CustomerOrder.FeeTypeEnum.SpecialFee,
                        OperationType = 3,
                        CreatedOn = DateTime.Now,
                        CreatedBy = _workContext.User.UserUame,
                        LastUpdatedBy = _workContext.User.UserUame,
                        LastUpdatedOn = DateTime.Now,
                    });
            }

            listDeliveryFeeInfos.ForEach(_deliveryFeeInfoRepository.Add);

            _deliveryFeeRepository.Modify(deliveryFee);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                _deliveryFeeInfoRepository.UnitOfWork.Commit();
                _deliveryFeeRepository.UnitOfWork.Commit();

                transaction.Complete();
            }
        }

        public ReceivingExpenseExt GetWayBillPrice(string wayBillNumber)
        {
            var priceProviderResult = PostCustomerPrice(GetCustomerInfoPackageRequest(wayBillNumber));

            if (!priceProviderResult.CanShipping)
                throw new Exception(priceProviderResult.Message);

            return new ReceivingExpenseExt()
            {
                Freight = priceProviderResult.ShippingFee,
                Register = priceProviderResult.RegistrationFee,
                FuelCharge = priceProviderResult.FuelFee,
                TariffPrepayFee = priceProviderResult.TariffPrepayFee,
                RemoteAreaFee = priceProviderResult.RemoteAreaFee,
                Surcharge = priceProviderResult.Value - (priceProviderResult.ShippingFee + priceProviderResult.RegistrationFee + priceProviderResult.FuelFee + priceProviderResult.TariffPrepayFee + priceProviderResult.RemoteAreaFee),
                TotalFee = priceProviderResult.Value,
                WayBillNumber = wayBillNumber,
            };
        }

        public List<PriceProviderResult> GetPriceProviderResult(string[] wayBillNumbers)
        {
            List<PriceProviderResult> listPriceProviderResult = new List<PriceProviderResult>();
            wayBillNumbers.ToList().ForEach(p =>
                {
                    listPriceProviderResult.Add(PostCustomerPrice(GetCustomerInfoPackageRequest(p)));
                });

            return listPriceProviderResult;
        }

        public PriceProviderExtResult GetPriceProviderResult(string wayBillNumber)
        {
            return PostCustomerPrice(GetCustomerInfoPackageRequest(wayBillNumber));
        }

        /// <summary>
        /// 计算服务商价格
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        public DeliveryFeeExt GetWayBillVenderPrice(string wayBillNumber)
        {
            var priceProviderResult = PostVenderPriceAuto(GetVenderPackageRequest(wayBillNumber));

            if (priceProviderResult == null)
            {
                throw new Exception("系统异常，请重试");
            }

            if (!priceProviderResult.CanShipping)
            {
                throw new Exception(priceProviderResult.Message);
            }

            return new DeliveryFeeExt()
                {
                    Freight = priceProviderResult.ShippingFee,
                    Register = priceProviderResult.RegistrationFee,
                    FuelCharge = priceProviderResult.FuelFee,
                    TariffPrepayFee = priceProviderResult.TariffPrepayFee,
                    OverWeightLengthGirthFee = priceProviderResult.OverWeightOrLengthFee + priceProviderResult.OverGirthFee,
                    SecurityAppendFee = priceProviderResult.SecurityAppendFee,
                    AddedTaxFee = priceProviderResult.AddedTaxFee,
                    Surcharge = priceProviderResult.Value - (priceProviderResult.ShippingFee
                                                             + priceProviderResult.RegistrationFee
                                                             + priceProviderResult.FuelFee
                                                             + priceProviderResult.TariffPrepayFee
                                                             + priceProviderResult.OverWeightOrLengthFee + priceProviderResult.OverGirthFee
                                                             + priceProviderResult.SecurityAppendFee
                                                             + priceProviderResult.AddedTaxFee),
                    TotalFee = priceProviderResult.Value,
                    WayBillNumber = wayBillNumber,
                    SetWeight = priceProviderResult.Weight,
                };
        }

        private CustomerInfoPackageRequest GetCustomerInfoPackageRequest(string wayBillNumber)
        {
            var wayBillInfo = _wayBillInfoRepository.Get(wayBillNumber);
            var customer = _customerRepository.GetFiltered(c => c.CustomerCode == wayBillInfo.CustomerCode).First();
            var listWaybillPackageDetail =
                _waybillPackageDetailRepository.GetFiltered(w => w.WayBillNumber == wayBillNumber).ToList();

            var packages = new List<PackageRequest>();

            listWaybillPackageDetail.ForEach(w =>
                {
                    packages.Add(new PackageRequest()
                        {
                            Weight = w.Weight.Value,
                            Length = w.Length.Value,
                            Width = w.Width.Value,
                            Height = w.Height.Value,
                        });
                });

            var packageRequest = new CustomerInfoPackageRequest()
                {
                    CustomerId = customer.CustomerID,
                    CountryCode = wayBillInfo.CountryCode,
                    ShippingTypeId = wayBillInfo.GoodsTypeID.Value,
                    ShippingMethodId = wayBillInfo.InShippingMethodID.Value,
                    EnableTariffPrepay = wayBillInfo.EnableTariffPrepay,
                    Packages = packages,
                };

            packageRequest.ShippingInfo.ShippingCity = wayBillInfo.ShippingInfo.ShippingCity;
            packageRequest.ShippingInfo.ShippingState = wayBillInfo.ShippingInfo.ShippingState;
            packageRequest.ShippingInfo.ShippingZip = wayBillInfo.ShippingInfo.ShippingZip;

            return packageRequest;
        }

        private VenderInfoPackageRequest GetVenderPackageRequest(string wayBillNumber)
        {
            var wayBillInfo = _wayBillInfoRepository.Get(wayBillNumber);

            var listWaybillPackageDetail = _waybillPackageDetailRepository.GetFiltered(p => p.WayBillNumber == wayBillNumber).ToList();

            if (listWaybillPackageDetail.Count == 0)
            {
                throw new Exception("没有找到该运单的包裹明细");
            }

            var venderPackageRequest = new VenderInfoPackageRequest()
            {
                CountryCode = wayBillInfo.CountryCode,
                ShippingTypeId = wayBillInfo.GoodsTypeID.Value,
                ShippingMethodId = wayBillInfo.OutShippingMethodID.Value,
                VenderCode = wayBillInfo.OutStorageInfo.VenderCode,
                CustomerId = _customerRepository.GetFiltered(p => p.CustomerCode == wayBillInfo.CustomerCode).FirstOrDefault().CustomerID,
                EnableTariffPrepay = wayBillInfo.EnableTariffPrepay,
            };

            listWaybillPackageDetail.ForEach(p =>
            {
                venderPackageRequest.Packages.Add(new VenderPackageRequest()
                {
                    Length = p.Length.Value,
                    Width = p.Width.Value,
                    Height = p.Height.Value,
                    Weight = p.Weight.Value
                });
            });

            return venderPackageRequest;
        }

        private VenderPackageModel GetVenderPackageModel(string wayBillNumber)
        {
            var wayBillInfo = _wayBillInfoRepository.Get(wayBillNumber);

            var listWaybillPackageDetail = _waybillPackageDetailRepository.GetFiltered(p => p.WayBillNumber == wayBillNumber).ToList();

            if (listWaybillPackageDetail.Count == 0)
            {
                throw new Exception("没有找到该运单的包裹明细");
            }

            if (listWaybillPackageDetail.Count > 1)
            {
                throw new Exception("运单含有多个包裹，目前不支持计算成本");
            }

            var waybillPackageDetail = listWaybillPackageDetail.FirstOrDefault();


            var venderPackageModel = new VenderPackageModel()
            {

                CountryCode = wayBillInfo.CountryCode,
                ShippingTypeId = wayBillInfo.GoodsTypeID.Value,
                ShippingMethodId = wayBillInfo.OutShippingMethodID.Value,

                Code = wayBillInfo.OutStorageInfo.VenderCode,
                Weight = waybillPackageDetail.Weight.Value,
                Length = waybillPackageDetail.Length.Value,
                Width = waybillPackageDetail.Width.Value,
                Height = waybillPackageDetail.Height.Value,

            };
            return venderPackageModel;
        }

        private PriceProviderExtResult PostCustomerPrice(CustomerInfoPackageRequest packageRequest)
        {
            try
            {
                var resultModel = HttpHelper.DoRequest<PriceProviderExtResult>(sysConfig.LISAPIPath + "API/LIS/PostCustomerPriceAuto", EnumHttpMethod.POST, EnumContentType.Json, packageRequest);
                Log.Info(resultModel.RawValue);
                return resultModel.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LISAPIPath + "/API/LIS/PostCustomerPriceAuto");
                Log.Exception(ex);
            }
            return null;
        }

        private PriceProviderResult PostVenderPrice(VenderPackageModel venderPackageModel)
        {
            try
            {
                var resultModel = HttpHelper.DoRequest<PriceProviderResult>(sysConfig.LISAPIPath + "API/LIS/PostVenderPrice?canBusinessExpress=false", EnumHttpMethod.POST, EnumContentType.Json, venderPackageModel);
                Log.Info(resultModel.RawValue);
                return resultModel.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LISAPIPath + "/API/LIS/PostVenderPrice");
                Log.Exception(ex);
            }
            return null;
        }

        private PriceProviderExtResult PostVenderPriceAuto(VenderInfoPackageRequest venderPackageRequst)
        {
            string url = sysConfig.LISAPIPath + "API/LIS/PostVenderPriceAuto";

            try
            {

                var resultModel = HttpHelper.DoRequest<PriceProviderExtResult>(url, EnumHttpMethod.POST, EnumContentType.Json, venderPackageRequst);
                Log.Info(resultModel.RawValue);
                return resultModel.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + url);
                Log.Exception(ex);
            }
            return null;
        }

        public DeliveryFeeAnomalyEditExt GetDeliveryFeeAnomalyEditExt(string wayBillNumber)
        {
            return _deliveryFeeRepository.GetDeliveryFeeAnomalyEditExt(wayBillNumber);
        }


        public DeliveryFeeAnomalyEditExt GetDeliveryFeeExpressAnomalyEditExt(string wayBillNumber)
        {
            return _deliveryFeeRepository.GetDeliveryFeeExpressAnomalyEditExt(wayBillNumber);
        }

        public void WayBillCancelAnomaly(string[] wayBillNumbers)
        {
            wayBillNumbers.ToList().ForEach(w =>
                {
                    var deliveryFee = _deliveryFeeRepository.GetFiltered(p => p.WayBillNumber == w).First();
                    deliveryFee.Status = (int) Financial.DeliveryFeeStatusEnum.UnAudited;
                    _deliveryFeeRepository.Modify(deliveryFee);
                });

          _deliveryFeeRepository.UnitOfWork.Commit();
        }


        public CustomerOrderInfo GetCustomerOrderInfoByNumber(string customerOrderInfoNumber)
        {
            return
                _customerOrderInfoRepository.GetFiltered(p => p.CustomerOrderNumber == customerOrderInfoNumber)
                                            .FirstOrDefault();
        }

        public Customer GetCustomerByCode(string customerCode)
        {
            return
                _customerRepository.GetFiltered(p => p.CustomerCode == customerCode)
                                            .FirstOrDefault();
        }

        /// <summary>
        /// 财务收货费用
        /// Add By zhengsong
        /// Time:2014-06-30
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IPagedList<InFeeInfoAuditListExt> GetAuditPagedList(InFeeInfoAuditParam param)
        {
            param.StartTime = param.StartTime.HasValue ? param.StartTime.Value : DateTime.Parse(DateTime.Now.AddDays(-29).ToString("yyyy-MM-dd") + " " + "00:00");
            param.EndTime = param.EndTime.HasValue ? param.EndTime.Value : DateTime.Parse(DateTime.Now.AddDays(+1).ToString("yyyy-MM-dd") + " " + "00:00");
            return _receivingExpensRepository.GetAuditPagedList(param);
        }

        public IList<InFeeInfoAuditListExt> GetAuditList(InFeeInfoAuditParam param)
        {
            param.StartTime = param.StartTime.HasValue ? param.StartTime.Value : new DateTime(2010, 1, 1);
            param.EndTime = param.EndTime.HasValue ? param.EndTime.Value : new DateTime(2020, 1, 1);
            return _receivingExpensRepository.GetAuditList(param);
        }

        public IList<InFeeInfoAuditListExt> GetInFeeInfoExport(InFeeInfoAuditParam param)
        {
            param.StartTime = param.StartTime.HasValue ? param.StartTime.Value : new DateTime(2010, 1, 1);
            param.EndTime = param.EndTime.HasValue ? param.EndTime.Value : new DateTime(2020, 1, 1);
            return _receivingExpensRepository.GetInFeeInfoExport(param);
        }
        public int GetInFeeInfoExportTotalCount(InFeeInfoAuditParam param)
        {
            param.StartTime = param.StartTime.HasValue ? param.StartTime.Value : new DateTime(2010, 1, 1);
            param.EndTime = param.EndTime.HasValue ? param.EndTime.Value : new DateTime(2020, 1, 1);
            return _receivingExpensRepository.GetInFeeInfoExportTotalCount(param);
        }

        /// <summary>
        /// 批量审核
        /// Add By zhengsong
        /// Time:2014-07-01
        /// </summary>
        /// <param name="wayBillNumberList"></param>
        public void BatchAudited(List<string> wayBillNumberList)
        {
            using (var transaction = new TransactionScope())
            {
                foreach (var wayBillNumber in wayBillNumberList)
                {
                    UpdateAuditStatus(wayBillNumber);
                }
                _receivingExpensRepository.UnitOfWork.Commit();
                transaction.Complete();
            }
        }

        public void UpdateAuditStatus(string wayBillNumber)
        {
            Check.Argument.IsNullOrWhiteSpace(wayBillNumber, "运单号");
            var status = Financial.ReceivingExpenseStatusToValue(Financial.ReceivingExpenseStatusEnum.UnAudited);
            var audited = Financial.ReceivingExpenseStatusToValue(Financial.ReceivingExpenseStatusEnum.Audited);
            var model = _receivingExpensRepository.First(p => p.WayBillNumber == wayBillNumber && p.Status == status);
            if (model == null)
            {
                throw new ArgumentException("该运单号\"{0}\"不存在，或不是未审核状态！".FormatWith(wayBillNumber));
            }else
            {
                model.Status = audited;
                model.Auditor = _workContext.User.UserUame;
                model.AuditorDate = DateTime.Now;
                model.LastUpdatedBy = _workContext.User.UserUame;
                model.LastUpdatedOn = DateTime.Now;
                _receivingExpensRepository.Modify(model);
            }
    }

        /// <summary>
        /// 反审核
        /// Add By zhengsong
        /// Time:2014-07-01
        /// </summary>
        /// <param name="wayBillNumberList"></param>
        public void BatchAntiAudit(List<string> wayBillNumberList)
        {
            using (var transaction = new TransactionScope())
            {
                foreach (var wayBillNumber in wayBillNumberList)
                {
                    UpdateAntiAuditStatus(wayBillNumber);
                }
                _receivingExpensRepository.UnitOfWork.Commit();
                transaction.Complete();
            }
        }

        public void UpdateAntiAuditStatus(string wayBillNumber)
        {
            Check.Argument.IsNullOrWhiteSpace(wayBillNumber, "运单号");
            InFeeInfoAuditParam param=new InFeeInfoAuditParam();
            var status = Financial.ReceivingExpenseStatusToValue(Financial.ReceivingExpenseStatusEnum.UnAudited);
            var audited = Financial.ReceivingExpenseStatusToValue(Financial.ReceivingExpenseStatusEnum.Audited);
            var model = _receivingExpensRepository.First(p => p.WayBillNumber == wayBillNumber && p.Status == audited);

            param.SearchWhere = 1;
            param.SearchContext = wayBillNumber;
            param.StartTime = param.StartTime.HasValue ? param.StartTime.Value : new DateTime(2010, 1, 1);
            param.EndTime = param.EndTime.HasValue ? param.EndTime.Value : new DateTime(2020, 1, 1);
            var list = GetAuditList(param);

            if (model == null)
            {
                throw new ArgumentException("该运单号\"{0}\"不存在，或不是已审核状态！".FormatWith(wayBillNumber));
            }else if (list.FirstOrDefault(p=>p.OperationType == 4) != null)
            {
                throw new ArgumentException("该运单\"{0}\"已退回不能反审核！".FormatWith(wayBillNumber));
            }else
            {
                model.Status = status;
                model.LastUpdatedBy = _workContext.User.UserUame;
                model.LastUpdatedOn = DateTime.Now;
                _receivingExpensRepository.Modify(model);
            }
        }

        /// <summary>
        /// 审核异常
        /// Add By zhengsong
        /// Time:2014-07-01
        /// </summary>
        /// <param name="auditAnomalyList"></param>
        /// <returns></returns>
        public bool UpdateAuditAnomaly(List<AuditAnomalyExt> auditAnomalyList)
        {
            bool Result = false;
            try
            {
                foreach (var auditAnomaly in auditAnomalyList)
                {
                    UpdateAnomaly(auditAnomaly);
                }
                _receivingExpensRepository.UnitOfWork.Commit();
                Result = true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                Result = false;
            }
            return Result;
        }

        public void UpdateAnomaly(AuditAnomalyExt auditAnomaly)
        {
            if (auditAnomaly.WayBillNumber != null)
            {
                var status = Financial.ReceivingExpenseStatusToValue(Financial.ReceivingExpenseStatusEnum.UnAudited);
                var anomaly = Financial.ReceivingExpenseStatusToValue(Financial.ReceivingExpenseStatusEnum.AuditAnomaly);
                var model = _receivingExpensRepository.First(p => p.WayBillNumber == auditAnomaly.WayBillNumber && p.Status == status);
                if (model == null)
                {
                    throw new ArgumentException("该运单号\"{0}\"不存在，或不是未审核状态！".FormatWith(auditAnomaly.WayBillNumber));
                }else
                {
                    model.Status = anomaly;
                    model.FinancialNote += _workContext.User.UserUame + ":" + auditAnomaly.NewFinancialNote +"." + DateTime.Now.ToString("yyyy-MM-dd")+Environment.NewLine;
                    model.LastUpdatedBy = _workContext.User.UserUame;
                    model.LastUpdatedOn = DateTime.Now;
                    model.Auditor = _workContext.User.UserUame;
                    model.AuditorDate = DateTime.Now;
                    _receivingExpensRepository.Modify(model);
                }
            }
        }

        /// <summary>
        /// 出账单
        /// Add By zhengsong
        /// Time:2014-07-01
        /// </summary>
        /// <param name="wayBillNumberList"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateOutBilled(List<string> wayBillNumberList, ReceivingBillExt model)
        {
            bool Result = false;
            try
            {
                using (var transaction = new TransactionScope())
                {
                    var receivingBillID = SequenceNumberService.GetSequenceNumber(PrefixCode.ReceivingBillID);
                    ReceivingBill receivingBill = new ReceivingBill();
                    receivingBill.ReceivingBillID = receivingBillID;
                    receivingBill.CustomerCode = model.CustomerCode;
                    receivingBill.CustomerName = model.CustomerName;
                    receivingBill.ReceivingBillDate = DateTime.Now;
                    receivingBill.ReceivingBillAuditor = _workContext.User.UserUame;
                    receivingBill.BillStartTime = model.StartTime;
                    receivingBill.BillEndTime = model.EndTim;
                    receivingBill.ShippingMethodID = model.ShippingMethodId;
                    receivingBill.CountryCode = model.CountryCode;
                    receivingBill.Search = model.SearchWhere;
                    receivingBill.SearchValue = model.SearchContext;
                    //1代表未生成状态
                    receivingBill.Status = 1;
                    _receivingBillRepository.Add(receivingBill);
                    foreach (var wayBillNumber in wayBillNumberList)
                    {
                        UpdateOutBill(wayBillNumber, receivingBill.ReceivingBillID);
                    }
                    //更改费用明细
                    UpdateReceivingExpenseInfo(wayBillNumberList, receivingBillID);
                    _receivingBillRepository.UnitOfWork.Commit();
                    _receivingExpensRepository.UnitOfWork.Commit();
                    _receivingExpenseInfoRepository.UnitOfWork.Commit();
                    transaction.Complete();
                }
                Result = true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                throw;
            }
            return Result;
        }

        public void UpdateOutBill(string wayBillNumber, string receivingBillID)
        {
            var status = Financial.ReceivingExpenseStatusToValue(Financial.ReceivingExpenseStatusEnum.OutBilled);
            var audited = Financial.ReceivingExpenseStatusToValue(Financial.ReceivingExpenseStatusEnum.Audited);
            var receivingExpens = _receivingExpensRepository.First(p => p.WayBillNumber == wayBillNumber && p.Status == audited);
            if (receivingExpens == null)
            {
                throw new ArgumentException("该运单号\"{0}\"不存在，或不是已审核状态！".FormatWith(wayBillNumber));
            }else
            {
                receivingExpens.Status = status;
                receivingExpens.LastUpdatedBy = _workContext.User.UserUame;
                receivingExpens.LastUpdatedOn = DateTime.Now;
                _receivingExpensRepository.Modify(receivingExpens);
            }
        }

        //更改费用明细
        public void UpdateReceivingExpenseInfo(List<string> wayBillNumberList,string receivingBillID)
        {
            List<int> receivingExpenseIDs=new List<int>();
            if (wayBillNumberList.Count > 0)
            {
                var receivingExpenslist =
                    _receivingExpensRepository.GetList(p =>  wayBillNumberList.Contains(p.WayBillNumber));
                if (receivingExpenslist.Count > 0)
                {
                    receivingExpenslist.ForEach(p => receivingExpenseIDs.Add(p.ReceivingExpenseID));
                    var list =_receivingExpenseInfoRepository.GetList(p => receivingExpenseIDs.Contains(p.ReceivingExpenseID.Value) && p.ReceivingBillID ==null);
                    list.ForEach(p =>
                        {
                            p.ReceivingBillID = receivingBillID;
                            _receivingExpenseInfoRepository.Modify(p);
                        });
                }
            }
            
        }

        #region 发货明细审核


        public PagedList<DeliveryFeeExt> DeliveryFeeSearch(DeliveryReviewParam param)
        {

            var pagedList = this._deliveryFeeRepository.Search(param);

            var countrys = GetAllCountry();

            foreach (DeliveryFeeExt deliveryFeeExt in pagedList)
            {
                deliveryFeeExt.CountryChineseName =
                    countrys
                    .First(c => c.CountryCode == deliveryFeeExt.CountryCode)
                    .ChineseName;
            }

            return pagedList;
        }

        public PagedList<DeliveryFeeExt> GetDeliveryFeeAnomaly(DeliveryReviewParam param)
        {
            param.Status = (int)Financial.DeliveryFeeStatusEnum.AuditAnomaly;

            var pagedList = this._deliveryFeeRepository.Search(param,true);

            var countrys = GetAllCountry();

            foreach (DeliveryFeeExt deliveryFeeExt in pagedList)
            {
                deliveryFeeExt.CountryChineseName =
                    countrys
                    .First(c => c.CountryCode == deliveryFeeExt.CountryCode)
                    .ChineseName;
            }

            return pagedList;
        }
        //快递发货审核查询
        public PagedList<DeliveryFeeExt> ExpressDeliveryFeeSearch(DeliveryReviewParam param)
        {

            var pagedList = _deliveryFeeRepository.Search(param,false,true);

            var countrys = GetAllCountry();

            foreach (DeliveryFeeExt deliveryFeeExt in pagedList)
            {
                deliveryFeeExt.CountryChineseName =
                    countrys
                    .First(c => c.CountryCode == deliveryFeeExt.CountryCode)
                    .ChineseName;
            }

            return pagedList;
        }

        public PagedList<DeliveryFeeExt> GetDeliveryFeeExpressAnomaly(DeliveryReviewParam param)
        {
            param.Status = (int)Financial.DeliveryFeeStatusEnum.AuditAnomaly;

            var pagedList = this._deliveryFeeRepository.Search(param, true, true);

            var countrys = GetAllCountry();

            foreach (DeliveryFeeExt deliveryFeeExt in pagedList)
            {
                deliveryFeeExt.CountryChineseName =
                    countrys
                    .First(c => c.CountryCode == deliveryFeeExt.CountryCode)
                    .ChineseName;
            }

            return pagedList;
        }

        IEnumerable<Country> GetAllCountry()
        {
            //添加缓存机制 周建春
            string cacheKey = "/Services/FinancialService/GetAllCountry";
            if (Cache.Exists(cacheKey))
            {
                return Cache.Get<IEnumerable<Country>>(cacheKey);
            }
            else
            {
                var countryList = _countryRepository.GetAll().ToList();
                Cache.Add(cacheKey, countryList, DateTime.Now.AddHours(1));
                return countryList;
            }
        }
        


        public List<DeliveryFeeExt> ExportExcel(DeliveryReviewParam param)
        {
            return _deliveryFeeRepository.Export(param);
        }
        public List<DeliveryFeeExt> ExportExcel(DeliveryReviewParam param, bool enableStatusFilter, bool isExpress)
        {
            return _deliveryFeeRepository.Export(param,enableStatusFilter,isExpress);
        }
        public string GetRemarkHistory(int id)
        {
            return _deliveryFeeRepository.GetRemarkHistory(id);
        }


        public List<DeliveryDeviation> GetVenderDeliveryDeviation(string venderName)
        {
            return _deliveryFeeRepository.GetVenderDeliveryDeviation(venderName);
        }
        public List<WayBillNumberExtSimple> GetLocalOrderInfo(List<string> orderOrTrackNumbers)
        {
            return _deliveryFeeRepository.GetLocalOrderInfo(orderOrTrackNumbers);
        }
        public bool SaveDeliveryImportAccountChecks(List<DeliveryImportAccountCheck> importData, string venderCode, int selectOrderNo)
        {
            //批量插入临时表

            var userName = importData.FirstOrDefault().CreatedBy;
            _deliveryFeeRepository.DeleteDeliveryImportAccountChecksTemp(userName);
            Log.Info("开始批量插入DeliveryImportAccountChecksTemp");
            var list = new List<DeliveryImportAccountChecksTemp>();
            importData.ForEach(p => list.Add(new DeliveryImportAccountChecksTemp()
                {
                    CountryName = p.CountryName,
                    CreatedBy = p.CreatedBy,
                    FeeDeviation = 0,
                    OrderNumber = p.OrderNumber,
                    OriginalSettleWeight = 0,
                    OriginalTotalFee = 0,
                    ReceivingDate = p.ReceivingDate,
                    SettleWeight = p.SettleWeight,
                    ShippingMethodName = p.ShippingMethodName,
                    TotalFee = p.TotalFee,
                    VenderName = p.VenderName,
                    WayBillNumber = "",
                    Weight = p.Weight,
                    WeightDeviation = 0
                }));
            _wayBillInfoRepository.BulkInsert("DeliveryImportAccountChecksTemp",list);
            _wayBillInfoRepository.UnitOfWork.Commit();
            Log.Info("完成批量插入DeliveryImportAccountChecksTemp");
            return _deliveryFeeRepository.SaveDeliveryImportAccountChecks(importData,venderCode,selectOrderNo);
        }
        public bool SaveDeliveryImportAccountChecks(List<ExpressDeliveryImportAccountCheck> importData, string venderCode, int selectOrderNo)
        {
            var userName = importData.FirstOrDefault().CreatedBy;
            _deliveryFeeRepository.DeleteExpressDeliveryImportAccountChecksTemp(userName);
            Log.Info("开始批量插入ExpressDeliveryImportAccountChecksTemp");
            var list = new List<ExpressDeliveryImportAccountChecksTemp>();
            importData.ForEach(p=>list.Add(new ExpressDeliveryImportAccountChecksTemp()
                {
                    CountryName = p.CountryName,
                    CreatedBy = p.CreatedBy,
                    FeeDeviation = 0,
                    OrderNumber = p.OrderNumber,
                    OriginalSettleWeight = 0,
                    OriginalTotalFee = 0,
                    ReceivingDate = p.ReceivingDate,
                    SettleWeight = p.SettleWeight,
                    ShippingMethodName = p.ShippingMethodName,
                    TotalFee = p.TotalFee,
                    VenderName = p.VenderName,
                    WayBillNumber = "",
                    Weight = p.Weight,
                    WeightDeviation = 0,
                    Freight = p.Freight,
                    FuelCharge = p.FuelCharge,
                    Register = p.Register,
                    Surcharge = p.Surcharge,
                    TariffPrepayFee = p.TariffPrepayFee,
                    OverWeightLengthGirthFee = p.OverWeightLengthGirthFee,
                    SecurityAppendFee = p.SecurityAppendFee,
                    AddedTaxFee = p.AddedTaxFee,
                    Incidentals = p.Incidentals,
                    IncidentalRemark = p.IncidentalRemark
                }));
            _wayBillInfoRepository.BulkInsert("ExpressDeliveryImportAccountChecksTemp", list);
            _wayBillInfoRepository.UnitOfWork.Commit();
            Log.Info("完成批量插入ExpressDeliveryImportAccountChecksTemp");
            return _deliveryFeeRepository.SaveDeliveryImportAccountChecks(importData, venderCode, selectOrderNo);
        }
        public PagedList<DeliveryFeeExt> ImportExcelWait2Audit(DeliveryReviewParam param)
        {
            return _deliveryFeeRepository.ImportExcelWait2Audit(param);
        }
        public PagedList<DeliveryFeeExpressExt> ExpressImportWait2Audit(DeliveryReviewParam param)
        {
            return _deliveryFeeRepository.ExpressImportWait2Audit(param);
        }

        public bool DeliveryFeeAuditPass(List<int> ids, string userName, string remark, DateTime dt)
        {
            return _deliveryFeeRepository.DeliveryFeeAuditPass(ids, userName, remark, dt);
        }
        public bool ReverseAudit(List<int> ids, string userName, string remark, DateTime dt)
        {
            return _deliveryFeeRepository.ReverseAudit(ids, userName, remark, dt);
        }
        public bool DeliveryFeeAuditError(List<int> ids, string userName, string error, DateTime dt)
        {
            return _deliveryFeeRepository.DeliveryFeeAuditError(ids, userName, error, dt);
        }

        public bool ExpressDeliveryFeeAuditError(List<AuditParam> dataParams,string userName)
        {
            return _deliveryFeeRepository.ExpressDeliveryFeeAuditError(dataParams,userName);
        }
        public bool ExpressDeliveryFeeAuditPass(List<AuditParam> dataParams, string userName)
        {
            return _deliveryFeeRepository.ExpressDeliveryFeeAuditPass(dataParams, userName);
        }
     
        #endregion


      
		/// <summary>
		/// 应收应付查询
		/// Add By yungchu
		/// Time:2014-07-02
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		public IPagedList<ChargePayAnalysesExt> GetChragePayAnayeseRecordPagedList(ChragePayAnalyeseParam param, out int TotalRecord, out int TotalPage)
		{
			return _deliveryFeeInfoRepository.GetChargePayAnalysesList(param, out TotalRecord, out TotalPage);
		}

		/// <summary>
		/// 应收应付导出excel
		/// Add By yungchu
		/// Time:2014-07-02
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		public List<ChargePayAnalysesExt> GetExportChargePayAnalysesList(ChragePayAnalyeseParam param)
		{
			return _deliveryFeeInfoRepository.ExportChargePayAnalysesList(param);
		}
        public decimal DeliveryFeeGetTotalFinalSum(DeliveryReviewParam para)
        {
            return _deliveryFeeRepository.DeliveryFeeGetTotalFinalSum(para);
        }
      
        public IPagedList<ReceivingBill> GetReceivingBillPagedList(ReceivingBillParam param)
        {
            return _receivingBillRepository.GetPagedList(param);
        }


        /// <summary>
        /// Add By zhengsong
        /// 判断是否生成运费
        /// Time:2014-0712
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        public decimal GetCountFeel(string wayBillNumber)
        {
            decimal fee = 0;
            var receivingExpens = _receivingExpensRepository.First(p=>p.WayBillNumber==wayBillNumber);
            if (receivingExpens != null)
            {
              var list= _receivingExpenseInfoRepository.GetList(p => p.ReceivingExpenseID == receivingExpens.ReceivingExpenseID);
                if (list.Count > 0)
                {
                    list.ForEach(p =>
                        {
                            if (p.OperationType != 4)
                            {
                                fee += p.Amount ?? 0;
                            }
                        });
                }
            }
            return fee;
        }


        /// <summary>
        /// 获取ReceivingExpenseID
        /// Add By zhengsong
        /// Time:2014-07-12
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        public int GetReceivingExpenseID(string wayBillNumber)
        {
            int receivingExpenseID=0;
            var receivingExpens = _receivingExpensRepository.First(p => p.WayBillNumber == wayBillNumber);
            if (receivingExpens != null)
            {
                receivingExpenseID = receivingExpens.ReceivingExpenseID;
            }
            return receivingExpenseID;
        }


        public InFeeTotalInfoExt GetInFeeTotalInfo(string number)
        {
            var wayBillInfo = _wayBillInfoRepository.GetWayBillInfo(number,"");

            if (wayBillInfo == null)
            {
                return null;
            }

            var receivingExpens = GetReceivingExpensesEditExt(wayBillInfo.WayBillNumber);

            var inFeeTotalInfoExt = new InFeeTotalInfoExt();

            var country= _countryRepository.GetFiltered(c => c.CountryCode == wayBillInfo.CountryCode).FirstOrDefault();

            var customer = _customerRepository.GetFiltered(c => c.CustomerCode == wayBillInfo.CustomerCode).FirstOrDefault();

            inFeeTotalInfoExt.ChineseName = country.ChineseName;

            inFeeTotalInfoExt.CountryCode = wayBillInfo.CountryCode;

            inFeeTotalInfoExt.CustomerCode = wayBillInfo.CustomerCode;

            inFeeTotalInfoExt.CustomerID = customer.CustomerID;

            inFeeTotalInfoExt.CustomerName = customer.Name;

            inFeeTotalInfoExt.CustomerOrderNumber = wayBillInfo.CustomerOrderNumber;

            inFeeTotalInfoExt.InStorageID = wayBillInfo.InStorageID;

            inFeeTotalInfoExt.PackageNumber = wayBillInfo.CustomerOrderInfo.PackageNumber.HasValue?wayBillInfo.CustomerOrderInfo.PackageNumber.Value:0;

            inFeeTotalInfoExt.ShippingMethodID = wayBillInfo.InShippingMethodID.Value;

            inFeeTotalInfoExt.ShippingMethodName = wayBillInfo.InShippingMethodName;

            inFeeTotalInfoExt.TrackingNumber = wayBillInfo.TrackingNumber;

            inFeeTotalInfoExt.WayBillNumber = wayBillInfo.WayBillNumber;

            inFeeTotalInfoExt.WayBillStatus = wayBillInfo.Status;


            //费用

            inFeeTotalInfoExt.TotalFee = receivingExpens != null ? 
                (receivingExpens.TotalFeeFinal.HasValue ? receivingExpens.TotalFeeFinal.Value :
                (receivingExpens.TotalFeeOriginal.HasValue?receivingExpens.TotalFeeOriginal.Value:0)) : 0;


            return inFeeTotalInfoExt;
        }


		/// <summary>
		/// 增删修查发货审核偏差率
		/// yungchu
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		#region 增删修查
		public bool AddDeliveryDeviations(List<DeliveryDeviation> listEntity)
	    {

			 Expression<Func<DeliveryDeviation, bool>> filter = p => true;
			var venderCode = listEntity[0].VenderCode;
			var shippingmethodId = listEntity[0].ShippingmethodID;
			filter = filter.AndIf(a => a.VenderCode.Contains(venderCode), !string.IsNullOrEmpty(venderCode))
				.AndIf(a => a.ShippingmethodID == shippingmethodId, shippingmethodId.HasValue);
			 //是否存在记录
			 IEnumerable<DeliveryDeviation> getDeliveryDeviationsdata = _deliveryDeviationRepository.GetList(filter);

			if (getDeliveryDeviationsdata.Any())
			{
				throw new ArgumentException(string.Format("已经存在服务商{0}对应渠道{1}！", listEntity[0].VenderName, listEntity[0].ShippingmethodName));
			}

			try
			{
				foreach (var item in listEntity)
				{
					item.CreatedOn = System.DateTime.Now;
					item.CreatedBy = _workContext.User.UserUame;
					item.LastUpdatedOn = System.DateTime.Now;
					_deliveryDeviationRepository.Add(item);
					_deliveryDeviationRepository.UnitOfWork.Commit();
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}

	    }

		public bool DeleteDeliveryDeviations(int id)
	    {
		    try
		    {

			     DeliveryDeviation entity = _deliveryDeviationRepository.First(a => a.DeviationID == id);

				 Expression<Func<DeliveryDeviation, bool>> filter = p => true;
			      filter = filter.AndIf(a => a.VenderCode == entity.VenderCode, !string.IsNullOrEmpty(entity.VenderCode))
				    .AndIf(a => a.ShippingmethodID == entity.ShippingmethodID, entity.ShippingmethodID.HasValue);


			    foreach (var item in  _deliveryDeviationRepository.GetList(filter))
			    {
				    _deliveryDeviationRepository.Remove(item);
				    _deliveryDeviationRepository.UnitOfWork.Commit();
			    }

			
			    return true;
		    }
		    catch (Exception)
		    {
			    return false;
		    }
	    }

		//更新发货审核偏差率
		public bool UpdateDeliveryDeviations(List<DeliveryDeviation> listEntity,int id)
	    {
		    try
		    {
				Expression<Func<DeliveryDeviation, bool>> filter = p => true;
	
			    DeliveryDeviation getModel = _deliveryDeviationRepository.Get(id);

				filter = filter.AndIf(a => a.VenderCode.Contains(getModel.VenderCode), !string.IsNullOrEmpty(getModel.VenderCode))
					.AndIf(a => a.ShippingmethodID == getModel.ShippingmethodID, getModel.ShippingmethodID.HasValue);


			     List<DeliveryDeviation> getCurrentData = _deliveryDeviationRepository.GetList(filter);


					if (getCurrentData.Count() == 1)//如果只有一条
				    {
						getCurrentData[0] = listEntity[0];
			
						_deliveryDeviationRepository.Modify(getCurrentData[0]);
						_deliveryDeviationRepository.UnitOfWork.Commit();

				    }
					else 
					{

						for (int i = 0; i < 2; i++)
						{
							getCurrentData[i].VenderCode = listEntity[i].VenderCode;
							getCurrentData[i].VenderName = listEntity[i].VenderName;
							getCurrentData[i].ShippingmethodID = listEntity[i].ShippingmethodID;
							getCurrentData[i].ShippingmethodName = listEntity[i].ShippingmethodName;
							getCurrentData[i].DeviationType = i+1;//'1-运费，2-重量',   
							getCurrentData[i].DeviationValue = listEntity[i].DeviationValue;
							getCurrentData[i].DeviationRate = listEntity[i].DeviationRate;
							getCurrentData[i].CreatedBy = _workContext.User.UserUame;
							getCurrentData[i].CreatedOn = System.DateTime.Now;
							getCurrentData[i].LastUpdatedOn = System.DateTime.Now;
						}

						foreach (var deliveryDeviation in getCurrentData.Take(2))
						{
							_deliveryDeviationRepository.Modify(deliveryDeviation);
							_deliveryDeviationRepository.UnitOfWork.Commit();
						}

					}


				return true;
		    }
		    catch (Exception)
		    {
			    return false;
		    }
	    }

		//同一个服务商，渠道信息发货偏差率
	    public DeliveryDeviationExt GetDeliveryDeviationInfo(int id)
	    {
			DeliveryDeviationExt model=new DeliveryDeviationExt();
		    DeliveryDeviation getDeviation=  _deliveryDeviationRepository.Get(id);

		    model.VenderCode = getDeviation.VenderCode;
		    model.VenderName = getDeviation.VenderName;
		    model.ShippingmethodID=getDeviation.ShippingmethodID;
		    model.ShippingmethodName = getDeviation.ShippingmethodName;

		    List<DeliveryDeviationExt> listDeliveryDeviationExt= _deliveryDeviationRepository.GetDeliveryDeviationListInfo(new DeliveryDeviationParam()
		    {
			    VenderCode = getDeviation.VenderCode,
			    ShippingmethodID = getDeviation.ShippingmethodID
		    });


			foreach (var item in listDeliveryDeviationExt)
			{
				model.WaillDeviationValue = item.WaillDeviationValue;
				model.WaillDeviationRate = item.WaillDeviationRate;
				model.WeightDeviationValue = item.WeightDeviationValue;
				model.WeightDeviationRate = item.WeightDeviationRate;

			}

		    return model;
	    }


	    public IPagedList<DeliveryDeviationExt> GetDeliveryDeviationPagedList(DeliveryDeviationParam param)
		{
			return _deliveryDeviationRepository.GetDeliveryDeviationPagedList(param);

		}
       #endregion
    }
}
