using System.Collections.Concurrent;
using System.Threading.Tasks;
using LighTake.Infrastructure.Common.BizLogging;
using LighTake.Infrastructure.Common.BizLogging.Enums;
using LMS.Controllers.FinancialController.ViewModels;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LMS.Data.Entity.Param;
using LMS.Services.CountryServices;
using LMS.Services.FinancialServices;
using LMS.Services.FreightServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Excel;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using LMS.Data.Entity;
using LighTake.Infrastructure.Web.Filters;
using LMS.Core;
using LighTake.Infrastructure.Web.Validation;
using LMS.Services.OperateLogServices;
using NPOI.HSSF.UserModel;

namespace LMS.Controllers.FinancialController
{
    public partial class FinancialController : BaseController
    {
        private IFinancialService _financialService;
        private ICountryService _countryService;
        private IWorkContext _workContext;
        private IFreightService _freightService;
		private IOperateLogServices _operateLogServices;

        public FinancialController(IFinancialService financialService, ICountryService countryService,
			IWorkContext workContext, IFreightService freightService, IOperateLogServices operateLogServices)
        {
            _financialService = financialService;
            _countryService = countryService;
            _workContext = workContext;
            _freightService = freightService;
	        _operateLogServices = operateLogServices;
        }

        /// <summary>
        /// 市场部收货费用列表
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public ActionResult ReceivingExpensesList(ReceivingExpenseListFilterModel filterModel)
        {
            return View(ReceivingExpensesListDataBind(filterModel));
        }

        private ReceivingExpenseListViewModel ReceivingExpensesListDataBind(ReceivingExpenseListFilterModel filterModel)
        {
            var model = new ReceivingExpenseListViewModel {FilterModel = filterModel};

            FinancialParam financialParam = null;

            if (!string.IsNullOrWhiteSpace(filterModel.SearchContext))
            {
                financialParam = new FinancialParam()
                {
                    SearchContext = filterModel.SearchContext,
                    SearchWhere = filterModel.SearchWhere,
                    Page = filterModel.Page,
                    PageSize = filterModel.PageSize,
                    Status = (int) Financial.ReceivingExpenseStatusEnum.AuditAnomaly
                };
            }
            else
            {
                financialParam = new FinancialParam()
                {
                    CustomerCode = filterModel.CustomerCode,
                    ShippingMethodName = filterModel.ShippingMethodName,
                    ShippingMethodId = filterModel.ShippingMethodId,
                    Page = filterModel.Page,
                    PageSize = filterModel.PageSize,
                    Status = (int) Financial.ReceivingExpenseStatusEnum.AuditAnomaly
                };
            }
            model.PagedList =
                _financialService.GetReceivingExpensePagedList(financialParam)
                    .ToModelAsPageCollection<ReceivingExpenseExt, ReceivingExpenseModel>();

            WayBill.GetSearchFilterList().Where(p => p.TextField != "入仓号").ToList().ForEach(p =>
            {
                model.SearchWheres.Add(new SelectListItem()
                {
                    Text = p.TextField,
                    Value = p.ValueField,
                    Selected =
                        filterModel.SearchWhere.HasValue && p.ValueField == filterModel.SearchWhere.Value.ToString()
                });
            });
            return model;
        }

        public ActionResult ReceivingExpensesEdit(string wayBillNumber)
        {
            var model =
                _financialService.GetReceivingExpensesEditExt(wayBillNumber).ToModel<ReceivingExpensesEditViewModel>();

            return View(model);
        }

        [HttpPost]
        public JsonResult ReceivingExpensesEditSave(ReceivingExpensesEditViewModel model)
        {
            try
            {
                var receivingExpensesEditExt = new ReceivingExpensesEditExt();
                model.CopyTo(receivingExpensesEditExt);
                _financialService.EditReceivingExpensesEditExt(receivingExpensesEditExt);

                var result = new
                {
                    Success = true,
                    Message = ""
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                var result = new
                {
                    Success = false,
                    Message = ex.Message
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 发货费用异常
        /// </summary>
        /// <returns></returns>
        public ActionResult DeliveryFeeAnomalyList(DeliveryCostDetailsFilterModel filter)
        {
            if (string.IsNullOrWhiteSpace(filter.ShippingMethodId))
            {
                //获取小包的运输方式
                int[] smallPacketShippingMethodIds = _freightService.GetShippingMethodListByVenderCode(filter.VenderCode, 1, true).Select(p => p.ShippingMethodId).ToArray();

                filter.ShippingMethodId = string.Join(",", smallPacketShippingMethodIds);
            }
            return View(DeliveryFeeAnomalyListDataBind(filter));
        }

        /// <summary>
        /// 发货费用异常(商业快递)
        /// </summary>
        /// <returns></returns>
        public ActionResult DeliveryFeeExpressAnomalyList(DeliveryCostDetailsFilterModel filter)
        {
            bool isExpress = true;

            if (string.IsNullOrWhiteSpace(filter.ShippingMethodId))
            {
                //获取商业快递的运输方式
                int[] smallPacketShippingMethodIds = _freightService.GetShippingMethodListByVenderCode(filter.VenderCode,2, true).Select(p => p.ShippingMethodId).ToArray();

                filter.ShippingMethodId = string.Join(",", smallPacketShippingMethodIds);
            }

            return View(DeliveryFeeAnomalyListDataBind(filter, isExpress));
        }

        private DeliveryCostDetailsModel DeliveryFeeAnomalyListDataBind(DeliveryCostDetailsFilterModel filterModel, bool isExpress=false)
        {
            if (!filterModel.IsFirstIn.HasValue)
            {
                filterModel.IsFirstIn = true;
            }

            if (filterModel.IsFirstIn.Value)
            {
                filterModel.StartTime = DateTime.Now.Date.AddDays(-7);
                filterModel.EndTime = DateTime.Now;
            }

            if (filterModel.EndTime.HasValue)
            {
                filterModel.EndTime = filterModel.EndTime.Value.Date.AddDays(1).AddMinutes(-1);
            }

            filterModel.IsFirstIn = false;

            var model = new DeliveryCostDetailsModel {Filter = filterModel};

            DeliveryReviewParam para = new DeliveryReviewParam();

            if (!string.IsNullOrWhiteSpace(filterModel.SearchContext))
            {
                para = new DeliveryReviewParam()
                {
                    SearchContext = filterModel.SearchContext,
                    SearchWhere = filterModel.SearchWhere,
                    Page = filterModel.Page,
                    PageSize = filterModel.PageSize,
                };
            }
            else
            {
                filterModel.CopyTo(para);
            }

            if (!string.IsNullOrWhiteSpace(filterModel.ShippingMethodId))
            {
                para.ShippingMethodIds = filterModel.ShippingMethodId.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => Convert.ToInt32(p)).ToArray();
            }

            if (para.ShippingMethodIds.Length > 0)
            {
                if (!isExpress)
                {
                    model.PagedList =
                        _financialService.GetDeliveryFeeAnomaly(para).ToModelAsPageCollection<DeliveryFeeExt, DeliveryFeeModel>();
                }
                else
                {
                    model.PagedList =
                        _financialService.GetDeliveryFeeExpressAnomaly(para).ToModelAsPageCollection<DeliveryFeeExt, DeliveryFeeModel>();
                }
            }

            WayBill.GetSearchFilterList().Where(p => p.TextField != "入仓号").ToList().ForEach(p =>
            {
                model.SearchWhereTypes.Add(new SelectListItem()
                {
                    Text = p.TextField,
                    Value = p.ValueField,
                    Selected =
                        filterModel.SearchWhere.HasValue && p.ValueField == filterModel.SearchWhere.Value.ToString()
                });
            });

            return model;
        }


        public ActionResult DeliveryFeeAnomalyEdit(string wayBillNumber)
        {
            var model =
                _financialService.GetDeliveryFeeAnomalyEditExt(wayBillNumber).ToModel<DeliveryFeeAnomalyEditViewModel>();

            return View(model);
        }

        public ActionResult DeliveryFeeExpressAnomalyEdit(string wayBillNumber)
        {
            var model =
                _financialService.GetDeliveryFeeExpressAnomalyEditExt(wayBillNumber).ToModel<DeliveryFeeAnomalyEditViewModel>();

            return View(model);
        }

        [HttpPost]
        public JsonResult DeliveryFeeAnomalyEditSave(DeliveryFeeAnomalyEditViewModel model)
        {
            try
            {
                var deliveryFeeAnomalyEditExt = new DeliveryFeeAnomalyEditExt();
                model.CopyTo(deliveryFeeAnomalyEditExt);
                _financialService.EditDeliveryFeeAnomalyEditExt(deliveryFeeAnomalyEditExt);

                var result = new
                {
                    Success = true,
                    Message = ""
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                var result = new
                {
                    Success = false,
                    Message = ex.Message
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }


        [FormValueRequired("btnExport")]
        [ActionName("DeliveryFeeAnomalyList")]
        public ActionResult ExprotDeliveryFeeAnomaly(DeliveryCostDetailsFilterModel filter)
        {
            var filterNew = new DeliveryCostDetailsFilterModel();

            filter.CopyTo(filterNew);

            filterNew.Page = 1;
            filterNew.PageSize = int.MaxValue;

            var model = DeliveryFeeAnomalyListDataBind(filterNew);

            if (model.PagedList.InnerList.Count > 0)
            {
                model.PagedList.InnerList.ForEach(n =>
                {
                    if (!string.IsNullOrWhiteSpace(n.Remark))
                    {
                        n.Remark = n.Remark.Replace("<hr/>", "\r\n");
                    }

                });

                var titleList = new List<string>
                {
                    "WayBillNumber-运单号",
                    "CustomerOrderNumber-客户订单号",
                    "Trackingnumber-跟踪号",
                    "VenderName-服务商",
                    "OutStorageCreatedOn-发货时间",
                    "CountryCode-发货国家",
                    "ShippingmethodName-运输方式",
                    "AprroveWeight-货物重量Kg",
                    "SetWeight-计费重量kg",
                    "Freight-运费",
                    "Register-挂号费",
                    "FuelCharge-燃油费",
                    "Surcharge-附加费",
                    "TariffPrepayFee-关税预付服务费",
                    "TotalFee-系统总费用",
                    "TotalFeeFinal-最终总费用",
                    "Remark-备注"
                };
                string fileName = "发货费用异常";
                ExportExcelByWeb.ListExcel(fileName, model.PagedList.InnerList, titleList);
            }

            return View(DeliveryFeeAnomalyListDataBind(filter));
        }


        [FormValueRequired("btnExport")]
        [ActionName("DeliveryFeeExpressAnomalyList")]
        public ActionResult ExprotDeliveryFeeExpressAnomaly(DeliveryCostDetailsFilterModel filter)
        {
            var filterNew = new DeliveryCostDetailsFilterModel();

            filter.CopyTo(filterNew);

            filterNew.Page = 1;
            filterNew.PageSize = int.MaxValue;

            var model = DeliveryFeeAnomalyListDataBind(filterNew);

            if (model.PagedList.InnerList.Count > 0)
            {
                model.PagedList.InnerList.ForEach(n =>
                {
                    if (!string.IsNullOrWhiteSpace(n.Remark))
                    {
                        n.Remark = n.Remark.Replace("<hr/>", "\r\n");
                    }

                });

                var titleList = new List<string>
                {
                    "WayBillNumber-运单号",
                    "CustomerOrderNumber-客户订单号",
                    "Trackingnumber-跟踪号",
                    "VenderName-服务商",
                    "OutStorageCreatedOn-发货时间",
                    "CountryCode-发货国家",
                    "ShippingmethodName-运输方式",
                    "AprroveWeight-货物重量Kg",
                    "SetWeight-计费重量kg",
                    "Freight-运费",
                    "Register-挂号费",
                    "FuelCharge-燃油费",
                    "Surcharge-附加费",
                    "TariffPrepayFee-关税预付服务费",
                    "OverWeightLengthGirthFee-超长超重超周围长费",
                    "SecurityAppendFee-安全附加费",
                    "AddedTaxFee-增值税率费",
                    "OtherFee-杂费",
                    "TotalFee-系统总费用",
                    "TotalFeeFinal-最终总费用",
                    "Remark-备注"
                };
                string fileName = "发货费用异常";
                ExportExcelByWeb.ListExcel(fileName, model.PagedList.InnerList, titleList);
            }

            return View(DeliveryFeeAnomalyListDataBind(filter,true));
        }

        /// <summary>
        /// 计算客户费用
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetWayBillPrice(string wayBillNumber)
        {
            try
            {
                var receivingExpenseExt = _financialService.GetWayBillPrice(wayBillNumber);

                var result = new
                {
                    Success = true,
                    ReceivingExpenseExt = receivingExpenseExt,
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());

                var result = new
                {
                    Success = false,
                    Message = ex.Message,
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 计算服务商成本
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetWayBillVenderPrice(string wayBillNumber)
        {
            try
            {
                var deliveryFeeExt = _financialService.GetWayBillVenderPrice(wayBillNumber);

                var result = new
                {
                    Success = true,
                    DeliveryFeeExt = deliveryFeeExt,
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);

                var result = new
                {
                    Success = false,
                    Message=ex.Message,
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult WayBillCancelAnomaly(string wayBillNumbers)
        {
            try
            {
                var wayBillNumber = wayBillNumbers.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

                _financialService.WayBillCancelAnomaly(wayBillNumber);

                var result = new
                {
                    Success = true,
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());

                var result = new
                {
                    Success = false,
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        #region 收货费用审核

        /// <summary>
        /// 收货费用审核列表
        /// </summary>
        /// <returns></returns>
        public ActionResult InFeeInfoAuditList(InFeeInfoAuditFilterModel model)
        {
            if (!model.IsFistIn)
            {
                if (model.StartTime == null)
                {
                    model.StartTime = DateTime.Now.AddMonths(-1);
                }
                if (model.EndTime == null)
                {
                    model.EndTime = DateTime.Now;
                }
                model.PageSize = 300;
                model.IsFistIn = true;
            }
            return View(BindAuditList(model));
        }

        [HttpPost]
        [FormValueRequired("Select")]
        public ActionResult InFeeInfoAuditList(InFeeInfoAuditListViewModel model)
        {
            return View(BindAuditList(model.FilterModel));
        }

        /// <summary>
        /// 出账单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [FormValueRequired("BatchOutBilled")]
        [ActionName("InFeeInfoAuditList")]
        public ActionResult InFeeInfoAuditOutBilled(InFeeInfoAuditListViewModel model)
        {
            InFeeInfoAuditParam param = new InFeeInfoAuditParam();
            List<string> wayBillNumberList =new List<string>();

            param.CountryCode = model.FilterModel.CountryCode;
            param.CustomerCode = model.FilterModel.CustomerCode;
            param.ShippingMethodId = model.FilterModel.ShippingMethodId;
            param.StartTime = model.FilterModel.StartTime;
            param.EndTime = model.FilterModel.EndTime;
            param.Status = model.FilterModel.Status;
            model.InFeeInfoAuditList =_financialService.GetAuditList(param).ToModelAsCollection<InFeeInfoAuditListExt, InFeeInfoAuditListModel>();
            if (model.InFeeInfoAuditList.Count > 0)
            {
                model.InFeeInfoAuditList.ForEach(p =>
                    {
                        if (!wayBillNumberList.Contains(p.WayBillNumber))
                        {
                            wayBillNumberList.Add(p.WayBillNumber);
                        }
                    });
            }
            try
            {
                if (wayBillNumberList.Count > 0)
                {
                    ReceivingBillExt modelExt = new ReceivingBillExt();
                    modelExt.CustomerCode = model.FilterModel.CustomerCode; ;
                    modelExt.CustomerName = model.FilterModel.CustomerName; ;
                    modelExt.StartTime = model.FilterModel.StartTime;
                    modelExt.EndTim = model.FilterModel.EndTime;
                    modelExt.Status = model.FilterModel.Status;
                    modelExt.ShippingMethodId = model.FilterModel.ShippingMethodId;
                    modelExt.CountryCode = model.FilterModel.CountryCode;
                    var bol = _financialService.UpdateOutBilled(wayBillNumberList, modelExt);
                    if (bol)
                    {
                        SetViewMessage(ShowMessageType.Success, "操作成功", true);
                    }
                    else
                    {
                        SetViewMessage(ShowMessageType.Error, "操作失败", true);
                    }
                }else
                {
                    SetViewMessage(ShowMessageType.Error, "请选择要出的账单", true);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                SetViewMessage(ShowMessageType.Error, ex.Message, true);
            }
            return View(BindAuditList(model.FilterModel));
        }

        [HttpPost]
        [FormValueRequired("Exprot")]
        [ActionName("InFeeInfoAuditList")]
        public ActionResult ExprotInFeeInfoAudit(InFeeInfoAuditListViewModel model)
        {
            Log.Info("开始导出收货费用");

            Log.Info("开始查询收货费用列表");
            var list = new List<InFeeInfoAuditListModel>();
            if (string.IsNullOrWhiteSpace(model.FilterModel.SearchContext))
            {
                _financialService.GetInFeeInfoExport(new InFeeInfoAuditParam()
                            {
                                CountryCode = model.FilterModel.CountryCode,
                                CustomerCode = model.FilterModel.CustomerCode,
                                ShippingMethodId = model.FilterModel.ShippingMethodId,
                                StartTime = model.FilterModel.StartTime,
                                EndTime = model.FilterModel.EndTime,
                                Status = model.FilterModel.Status,
                                SearchWhere = 0
                            }).AsParallel().ToList().ForEach(p =>
                {
                    if (
                        !list.Any(
                            s =>
                            s.WayBillNumber == p.WayBillNumber && s.OperationType == p.OperationType))
                    {
                        list.Add(new InFeeInfoAuditListModel()
                        {
                            CustomerCode = p.CustomerCode,
                            CustomerName = p.CustomerName,
                            Auditor = p.Auditor,
                            AuditorDate = p.AuditorDate,
                            StatusName = Financial.GetReceivingExpenseStatusDescription(p.Status),
                            CountryCode = p.CountryCode,
                            FuelCharge = p.FuelCharge ?? 0,
                            Freight = p.Freight ?? 0,
                            SpecialFee = p.SpecialFee ?? 0,
                            TariffPrepayFee = p.TariffPrepayFee ?? 0,
                            Register = p.Register ?? 0,
                            RemoteAreaFee = p.RemoteAreaFee??0,
                            TotalFee = (p.Freight + p.FuelCharge + p.SpecialFee + p.TariffPrepayFee + p.Surcharge +
                                           p.Register+p.RemoteAreaFee),
                            Surcharge = p.Surcharge + p.SpecialFee,
                            WayBillNumber = p.WayBillNumber,
                            CustomerOrderNumber = p.CustomerOrderNumber,
                            OutDateTime = p.OutDateTime,
                            InStorageCreatedOn = p.InStorageCreatedOn,
                            TrackingNumber = p.TrackingNumber,
                            InShippingMethodName = p.InShippingMethodName,
                            SettleWeight = p.SettleWeight??0,
                            Weight = p.Weight??0,
                            OperationType = p.OperationType
                        });
                    }
                });


                #region 并行查询
                //int pagesize = 1500;
                //var totalcount = _financialService.GetInFeeInfoExportTotalCount(new InFeeInfoAuditParam()
                //    {
                //        CountryCode = model.FilterModel.CountryCode,
                //        CustomerCode = model.FilterModel.CustomerCode,
                //        ShippingMethodId = model.FilterModel.ShippingMethodId,
                //        StartTime = model.FilterModel.StartTime,
                //        EndTime = model.FilterModel.EndTime,
                //        Status = model.FilterModel.Status
                //    });
                //if (totalcount > 0)
                //{
                //    var size = totalcount / pagesize;
                //    if (totalcount % pagesize > 0)
                //    {
                //        size++;
                //    }
                //    var data = new ConcurrentStack<InFeeInfoAuditListModel>();
                //    Parallel.For(0, size, i =>
                //        {
                //            Log.Info("开始查询第" + i.ToString() + "页");
                //            var param = new InFeeInfoAuditParam()
                //            {
                //                CountryCode = model.FilterModel.CountryCode,
                //                CustomerCode = model.FilterModel.CustomerCode,
                //                ShippingMethodId = model.FilterModel.ShippingMethodId,
                //                StartTime = model.FilterModel.StartTime,
                //                EndTime = model.FilterModel.EndTime,
                //                Status = model.FilterModel.Status,
                //                PageSize = pagesize,
                //                Page = i + 1
                //            };
                //            _financialService.GetInFeeInfoExport(param).AsParallel().ToList().ForEach(p =>
                //                {
                //                    if (
                //                        !data.Any(
                //                            s =>
                //                            s.WayBillNumber == p.WayBillNumber && s.OperationType == p.OperationType))
                //                    {
                //                        data.Push(new InFeeInfoAuditListModel()
                //                            {
                //                                CustomerCode = p.CustomerCode,
                //                                CustomerName = p.CustomerName,
                //                                Auditor = p.Auditor,
                //                                AuditorDate = p.AuditorDate,
                //                                StatusName = Financial.GetReceivingExpenseStatusDescription(p.Status),
                //                                CountryCode = p.CountryCode,
                //                                FuelCharge = p.FuelCharge ?? 0,
                //                                Freight = p.Freight ?? 0,
                //                                SpecialFee = p.SpecialFee ?? 0,
                //                                TariffPrepayFee = p.TariffPrepayFee ?? 0,
                //                                Register = p.Register ?? 0,
                //                                TotalFee = (p.Freight + p.FuelCharge + p.SpecialFee + p.TariffPrepayFee + p.Surcharge +
                //                                               p.Register),
                //                                Surcharge = p.Surcharge + p.SpecialFee,
                //                                WayBillNumber = p.WayBillNumber,
                //                                CustomerOrderNumber = p.CustomerOrderNumber,
                //                                OutDateTime = p.OutDateTime,
                //                                InStorageCreatedOn = p.InStorageCreatedOn,
                //                                TrackingNumber = p.TrackingNumber,
                //                                InShippingMethodName = p.InShippingMethodName,
                //                                SettleWeight = p.SettleWeight,
                //                                Weight = p.Weight,
                //                                OperationType = p.OperationType
                //                            });
                //                    }
                //                });
                //            Log.Info("完成查询第" + i.ToString() + "页");
                //        });
                //    list = data.AsParallel().ToList();
                //} 
                #endregion
            }
            else
            {
                var param = new InFeeInfoAuditParam()
                   {
                       CountryCode = model.FilterModel.CountryCode,
                       CustomerCode = model.FilterModel.CustomerCode,
                       ShippingMethodId = model.FilterModel.ShippingMethodId,
                       StartTime = model.FilterModel.StartTime,
                       EndTime = model.FilterModel.EndTime,
                       Status = model.FilterModel.Status,
                       SearchWhere = model.FilterModel.SearchWhere,
                       SearchContext = model.FilterModel.SearchContext
                   };
                _financialService.GetInFeeInfoExport(param).AsParallel().ToList().ForEach(p => list.Add(new InFeeInfoAuditListModel()
                    {
                        CustomerCode = p.CustomerCode,
                        CustomerName = p.CustomerName,
                        Auditor = p.Auditor,
                        AuditorDate = p.AuditorDate,
                        StatusName = Financial.GetReceivingExpenseStatusDescription(p.Status),
                        CountryCode = p.CountryCode,
                        FuelCharge = p.FuelCharge ?? 0,
                        Freight = p.Freight ?? 0,
                        SpecialFee = p.SpecialFee ?? 0,
                        TariffPrepayFee = p.TariffPrepayFee ?? 0,
                        RemoteAreaFee = p.RemoteAreaFee??0,
                        Register = p.Register ?? 0,
                        TotalFee = (p.Freight + p.FuelCharge + p.SpecialFee + p.TariffPrepayFee + p.Surcharge +
                                       p.Register+p.RemoteAreaFee),
                        Surcharge = p.Surcharge + p.SpecialFee,
                        WayBillNumber = p.WayBillNumber,
                        CustomerOrderNumber = p.CustomerOrderNumber,
                        OutDateTime = p.OutDateTime,
                        InStorageCreatedOn = p.InStorageCreatedOn,
                        TrackingNumber = p.TrackingNumber,
                        InShippingMethodName = p.InShippingMethodName,
                        SettleWeight = p.SettleWeight??0,
                        Weight = p.Weight??0,
                        OperationType = p.OperationType
                    }));
            }
            Log.Info("完成查询收货费用列表");
            #region 老方法
            //InFeeInfoAuditParam param = new InFeeInfoAuditParam();
            //if (string.IsNullOrWhiteSpace(model.FilterModel.SearchContext))
            //{
            //    param.CountryCode = model.FilterModel.CountryCode;
            //    param.CustomerCode = model.FilterModel.CustomerCode;
            //    param.ShippingMethodId = model.FilterModel.ShippingMethodId;
            //    param.StartTime = model.FilterModel.StartTime;
            //    param.EndTime = model.FilterModel.EndTime;
            //    param.Status = model.FilterModel.Status;
            //    //param.DateWhere = model.FilterModel.DateWhere;
            //}
            //else
            //{
            //    param.SearchWhere = model.FilterModel.SearchWhere;
            //    param.SearchContext = model.FilterModel.SearchContext;
            //}
            //Log.Info("开始查询收货费用列表");
            //model.InFeeInfoAuditList =
            //    _financialService.GetAuditList(param)
            //        .ToModelAsCollection<InFeeInfoAuditListExt, InFeeInfoAuditListModel>();
            //Log.Info("完成查询收货费用列表");
            //Log.Info("开始转化为导出规格的列表");
            //var Countrys = _countryService.GetCountryList(string.Empty);
            //if (model.InFeeInfoAuditList.Count > 0)
            //{
            //    model.InFeeInfoAuditList.ForEach(p =>
            //        {
            //            if (!string.IsNullOrWhiteSpace(model.FilterModel.SearchContext))
            //            {
            //                var customerOrderInfo = _financialService.GetCustomerOrderInfoByNumber(p.CustomerOrderNumber);
            //                if (customerOrderInfo != null)
            //                {
            //                    var customer = _financialService.GetCustomerByCode(customerOrderInfo.CustomerCode);
            //                    if (customer != null)
            //                    {
            //                        p.CustomerCode = customer.CustomerCode;

            //                        p.CustomerName = customer.Name;
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                p.CustomerCode = model.FilterModel.CustomerCode;
            //                p.CustomerName = model.FilterModel.CustomerName;
            //            }
            //            p.StatusName = Financial.GetReceivingExpenseStatusDescription(p.Status);
            //            var country = Countrys.FirstOrDefault(c => c.CountryCode == p.CountryCode);
            //            if (country != null)
            //            {
            //                p.CountryCode = country.ChineseName;
            //            }
            //            p.FuelCharge = p.FuelCharge ?? 0;
            //            p.Freight = p.Freight ?? 0;
            //            p.SpecialFee = p.SpecialFee ?? 0;
            //            p.TariffPrepayFee = p.TariffPrepayFee ?? 0;
            //            p.Surcharge = p.Surcharge ?? 0;
            //            p.Register = p.Register ?? 0;
            //            p.TotalFee = (p.Freight + p.FuelCharge + p.SpecialFee + p.TariffPrepayFee + p.Surcharge +
            //                          p.Register);
            //            p.Surcharge = p.Surcharge + p.SpecialFee;
            //        });
            //    Log.Info("完成转化为导出规格的列表");
            //    Log.Info("开始生成收货execl");
            //    var titleList = new List<string>
            //    {
            //        "WayBillNumber-运单号",
            //        "CustomerOrderNumber-客户订单号",
            //        "CustomerCode-客户代码",
            //        "CustomerName-客户名称",
            //        "OutDateTime-创建时间",
            //        "InStorageCreatedOn-收货时间",
            //        "TrackingNumber-跟踪号",
            //        "CountryCode-发货国家",
            //        "InShippingMethodName-运输方式",
            //        "SettleWeight-计费重量(kg)",
            //        "Weight-货物重量(kg)",
            //        "Freight-运费",
            //        "Register-挂号费",
            //        "FuelCharge-燃油费",
            //        "Surcharge-附加费",
            //        "TariffPrepayFee-关税预付费",
            //        "TotalFee-总费用",
            //        "StatusName-状态",
            //        "Auditor-审核人",
            //        "AuditorDate-审核时间"
            //    };
            //    string fileName = "收货费用审核";
            //    ExportExcelByWeb.ListExcel(fileName, model.InFeeInfoAuditList, titleList);
            //    Log.Info("完成生成收货execl");
            //} 
            #endregion
            Log.Info("开始生成收货execl");
            var titleList = new List<string>
                {
                    "WayBillNumber-运单号",
                    "CustomerOrderNumber-客户订单号",
                    "CustomerCode-客户代码",
                    "CustomerName-客户名称",
                    "OutDateTime-验收时间",
                    "InStorageCreatedOn-收货时间",
                    "TrackingNumber-跟踪号",
                    "CountryCode-发货国家",
                    "InShippingMethodName-运输方式",
                    "SettleWeight-计费重量(kg)",
                    "Weight-货物重量(kg)",
                    "Freight-运费",
                    "Register-挂号费",
                    "FuelCharge-燃油费",
                    "Surcharge-附加费",
                    "TariffPrepayFee-关税预付费",
                    "RemoteAreaFee-偏远附加费",
                    "TotalFee-总费用",
                    "StatusName-状态",
                    "Auditor-审核人",
                    "AuditorDate-审核时间"
                };
            ExportExcelByWeb.ListExcel("收货费用审核" + DateTime.Now.ToString("yyyyMMddhhmmss"), list, titleList);
            Log.Info("完成生成收货execl");
            return View(BindAuditList(model.FilterModel));
        }

        public InFeeInfoAuditListViewModel BindAuditList(InFeeInfoAuditFilterModel filter)
        {
            InFeeInfoAuditListViewModel model = new InFeeInfoAuditListViewModel();
            model.FilterModel = filter;
            WayBill.GetSearchFilterList().ForEach(p =>
            {
                if (p.ValueField != "4")
                {
                    model.SearchWheres.Add(new SelectListItem()
                    {
                        Text = p.TextField,
                        Value = p.ValueField,
                        Selected = filter.Status.HasValue && p.ValueField == filter.Status.Value.ToString()
                    });
                }
            });
            model.StatusList.Add(new SelectListItem() {Text = "请选择", Value = "", Selected = !filter.Status.HasValue});
            Financial.GetReceivingExpenseStatusList()
                .ForEach(
                    p =>
                        model.StatusList.Add(new SelectListItem()
                        {
                            Text = p.TextField,
                            Value = p.ValueField,
                            Selected = filter.Status.HasValue && p.ValueField == filter.Status.Value.ToString()
                        }));
            //WayBill.GetDateFilterList().ForEach(p =>
            //{
            //    if (p.ValueField != "3")
            //    {
            //        model.DateWheres.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = p.ValueField == filter.DateWhere.ToString() });
            //    }
            //});
            InFeeInfoAuditParam param = new InFeeInfoAuditParam();
            if (string.IsNullOrWhiteSpace(filter.SearchContext))
            {
                param.CountryCode = filter.CountryCode;
                param.CustomerCode = filter.CustomerCode;
                param.ShippingMethodId = filter.ShippingMethodId;
                param.Status = filter.Status;
                //param.DateWhere = filter.DateWhere;
                param.StartTime = filter.StartTime;
                param.EndTime = filter.EndTime;
            }
            else
            {
                param.SearchWhere = filter.SearchWhere;
                param.SearchContext = filter.SearchContext;
            }
            param.Page = filter.Page;
            param.PageSize = filter.PageSize;
            model.PagedList =
                _financialService.GetAuditPagedList(param)
                    .ToModelAsPageCollection<InFeeInfoAuditListExt, InFeeInfoAuditListModel>();
            return model;
        }

        /// <summary>
        /// 批量审核
        /// Add By zhengsong
        /// Time:2014-07-01
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <returns></returns>
        public JsonResult BatchAudited(string wayBillNumbers)
        {
            var model = new ResponseResult();
            try
            {
                if (!string.IsNullOrWhiteSpace(wayBillNumbers))
                {
                    var arr = wayBillNumbers.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
                    var wayBillNumberList = arr.ToList();
                    _financialService.BatchAudited(wayBillNumberList);
                    model.Result = true;
                }
                else
                {
                    model.Result = false;
                    model.Message = "请选择需要审核的账单";
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                model.Result = false;
                model.Message = ex.Message;
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 反审核
        /// Add By zhengsong
        /// Time:2014-07-01
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <returns></returns>
        public JsonResult BatchAntiAudit(string wayBillNumbers)
        {
            var model = new ResponseResult();
            try
            {
                if (!string.IsNullOrWhiteSpace(wayBillNumbers))
                {
                    var arr = wayBillNumbers.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
                    var wayBillNumberList = arr.ToList();
                    _financialService.BatchAntiAudit(wayBillNumberList);
                    model.Result = true;
                }
                else
                {
                    model.Result = false;
                    model.Message = "请选择需要反审核的账单";
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                model.Result = false;
                model.Message = ex.Message;
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 审核异常
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        public ActionResult AuditAnomaly(string wayBillNumber)
        {
            AuditAnomalyViewModel model = new AuditAnomalyViewModel();
            if (!string.IsNullOrWhiteSpace(wayBillNumber))
            {
                var receivingExpenses = _financialService.GetReceivingExpensesEditExt(wayBillNumber);
                if (receivingExpenses != null)
                {
                    model.OldFinancialNote = receivingExpenses.FinancialNote;
                }
                model.WayBillNumber = wayBillNumber;
            }
            return View(model);
        }

        public JsonResult SaveAuditAnomaly(AuditAnomalyViewModel model)
        {
            var result = new ResponseResult();
            if (model != null && model.WayBillNumber != null)
            {
                try
                {
                    List<AuditAnomalyExt> AuditAnomalyList = new List<AuditAnomalyExt>();
                    AuditAnomalyList.Add(new AuditAnomalyExt
                    {
                        WayBillNumber = model.WayBillNumber,
                        OldFinancialNote = model.OldFinancialNote,
                        NewFinancialNote = model.NewFinancialNote
                    });
                    var bol = _financialService.UpdateAuditAnomaly(AuditAnomalyList);
                    if (bol)
                    {
                        result.Result = true;
                        result.Message = "Success";
                    }
                    else
                    {
                        result.Result = false;
                        result.Message = "保存失败";
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    result.Result = false;
                    result.Message = ex.Message;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 批量审核异常
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        public JsonResult BatchAnomaly(string wayBillNumbers, string remarks)
        {

            var result = new ResponseResult();
            try
            {
                if (string.IsNullOrWhiteSpace(wayBillNumbers))
                {
                    result.Result = false;
                    result.Message = "请选择需要审核的账单";
                }
                else if (string.IsNullOrWhiteSpace(remarks))
                {
                    result.Result = false;
                    result.Message = "请输入审核异常备注";
                }
                else
                {
                    var arr = wayBillNumbers.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
                    var wayBillNumberList = arr.ToList();
                    List<AuditAnomalyExt> AuditAnomalyList = new List<AuditAnomalyExt>();
                    wayBillNumberList.ForEach(p => AuditAnomalyList.Add(new AuditAnomalyExt
                    {
                        WayBillNumber = p,
                        NewFinancialNote = remarks
                    }));
                    bool bol = _financialService.UpdateAuditAnomaly(AuditAnomalyList);
                    if (bol)
                    {
                        result.Result = true;
                        result.Message = "Success";
                    }
                    else
                    {
                        result.Result = false;
                        result.Message = "保存失败";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                result.Result = false;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 出账单
        /// Add By zhengsong
        /// Time:2014-07-01
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <param name="customerCode"></param>
        /// <param name="customerName"></param>
        /// <param name="startTime"></param>
        /// <param name="endTim"></param>
        /// <param name="status"></param>
        /// <param name="shippingMethodId"></param>
        /// <param name="countryCode"></param>
        /// <param name="searchWhere"></param>
        /// <param name="searchContext"></param>
        /// <returns></returns>
        //public JsonResult BatchOutBilled(string wayBillNumbers, string customerCode, string customerName,
        //    DateTime startTime, DateTime endTim, int status, int? shippingMethodId,
        //    string countryCode, int searchWhere, string searchContext)
        //{

        //    var result = new ResponseResult();
        //    ReceivingBillExt model = new ReceivingBillExt();

        //    if (string.IsNullOrWhiteSpace(wayBillNumbers))
        //    {
        //        result.Result = false;
        //        result.Message = "请选择需要审核的账单";
        //    }
        //    else
        //    {
        //        var arr = wayBillNumbers.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
        //        var wayBillNumberList = arr.ToList();
        //        model.CustomerCode = customerCode;
        //        model.CustomerName = customerName;
        //        model.StartTime = startTime;
        //        model.EndTim = endTim;
        //        model.Status = status;
        //        model.ShippingMethodId = shippingMethodId;
        //        model.CountryCode = countryCode;
        //        model.SearchWhere = searchWhere;
        //        model.SearchContext = searchContext;
        //        var bol = _financialService.UpdateOutBilled(wayBillNumberList, model);
        //        if (bol)
        //        {
        //            result.Result = true;
        //            result.Message = "出账单成功";
        //        }
        //        else
        //        {
        //            result.Result = false;
        //            result.Message = "出账单失败";
        //        }
        //    }

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        #endregion




        /// <summary>
        /// 应收应付利润分析报表
        ///  add by yungchu
        /// </summary>
        /// <returns></returns>
        public ActionResult ChargePayAnalyseList(ChragePayAnalyeseFilterModel filter)
        {
			ChargePayAnayiseModel model = new ChargePayAnayiseModel();

			//第一次进入不显示数据
			if (!filter.IsFirstIn)
			{
				model.FilterModel = new ChragePayAnalyeseFilterModel();
				model.FilterModel.IsFirstIn = true;//不是第一次
				model.FilterModel.StartTime = DateTime.Parse(DateTime.Now.AddDays(1).AddMonths(-1).ToString("yyyy-MM-dd"));
				model.FilterModel.EndTime = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
				return View(model);
			}
			else
			{
				return View(ChargePayDataBind(filter));
			}
        }

        /// <summary>
        /// 应收应付利润分析报表--查询
        /// add by yungchu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [FormValueRequired("Search")]
        [ActionName("ChargePayAnalyseList")]
        public ActionResult SearchChargePayAnalyseList(ChargePayAnayiseModel model)
        {
            return View(ChargePayDataBind(model.FilterModel));
        }

        /// <summary>
        /// 按客户导出利润表
        /// add by yungchu
        /// 2014/7/1
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ChargePayAnalyseList")]
        [FormValueRequired("ExportByCustomer")]
        public ActionResult ExportProfitByCustomer(ChargePayAnayiseModel model)
        {
			//保存赋值
			ChargePayAnayiseModel getModel = new ChargePayAnayiseModel();
			getModel = model;

			//搜索结果
			List<GetChargePayAnayiseModel> getChargePayAnayiseModel =
				GetExportChargePayAnalysesList(model.FilterModel).ListGetChargePayAnayis;

			//客户,服务商，渠道分组
			var listCustomer = from a in getChargePayAnayiseModel
							   group a by new { name = a.Name, venderName = a.VenderName, shippingmethodName = a.ShippingmethodName }
								   into g
								   select new GetChargePayAnayiseModel
								   {
									   Name = g.Key.name,
									   VenderName = g.Key.venderName,
									   ShippingmethodName = g.Key.shippingmethodName,
									   ReceivingAmount = g.Sum(p => p.ReceivingAmount.Value),
									   DeliveryAmount = g.Sum(p => p.DeliveryAmount.Value),
									   Rate = ((g.Sum(p => p.ReceivingAmount.Value) - g.Sum(p => p.DeliveryAmount.Value)) / g.Sum(p => p.DeliveryAmount.Value) * 100).ToString("#0.0000")+"%"

								   };




			//客户名列表
			List<string> listList=new List<string>();
	        foreach (var listItem in listCustomer)
	        {
				if (!listList.Contains(listItem.Name))
				{
					listList.Add(listItem.Name);
				}
	        }


			var titleList = new List<string>
            {
                "VenderName-服务商",
                "ShippingmethodName-渠道",
                "ReceivingAmount-应收",
                "DeliveryAmount-应付",
                "Rate-利率"
            };

			string filename = "对账表按客户汇总";
			ExportExcelByWeb.InitExcel(filename);
			HSSFWorkbook workbook = ExportExcelByWeb.InitializeWorkbook();

			decimal receivingAmount = 0; //应收合计
			decimal deliveryAmount = 0; //应付合计


		    //客户数据源
			List<GetChargePayAnayiseModel> listPayAnayiseModel = new List<GetChargePayAnayiseModel>();
			foreach (var item in listCustomer)
			{
				listPayAnayiseModel.Add(new GetChargePayAnayiseModel
				{
					Name = item.Name,
					VenderName = item.VenderName,
					ShippingmethodName = item.ShippingmethodName,
					ReceivingAmount = item.ReceivingAmount.Value,
					DeliveryAmount = item.DeliveryAmount.Value,
					Rate = item.Rate
				});

			}




			//按客户分组导出excel
			foreach (var customerName in listList)
			{
				//当前客户数据
				var getData = listPayAnayiseModel.Where(a => a.Name == customerName).ToList();

				foreach (var chargePayAnayiseModel in getData)
				{
					//应收，应付合计
					receivingAmount += chargePayAnayiseModel.ReceivingAmount.Value;
					deliveryAmount += chargePayAnayiseModel.DeliveryAmount.Value;
				}


				//导出excel            
				ExportExcelByWeb.WriteToDownLoad(workbook, getData, titleList, null, 1,
							customerName, model.FilterModel.StartTime.Value, model.FilterModel.EndTime.Value, receivingAmount,
							deliveryAmount, customerName);

				//清空
				receivingAmount = 0;
				deliveryAmount = 0;
			}



			ExportExcelByWeb.WriteToStream(workbook);

			return View(getModel);
        }




        /// <summary>
        /// 按服务商导出利润表 
        /// add bu yungchu
        /// 2014/7/2
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ChargePayAnalyseList")]
        [FormValueRequired("ExportByVender")]
        public ActionResult ExportProfitByVender(ChargePayAnayiseModel model)
        {
            //保存赋值
            ChargePayAnayiseModel getModel = new ChargePayAnayiseModel();
            getModel = model;

            //搜索结果
            List<GetChargePayAnayiseModel> getChargePayAnayiseModel =
                GetExportChargePayAnalysesList(model.FilterModel).ListGetChargePayAnayis;

            //服务商,渠道，客户分组
	        var listVender = from a in getChargePayAnayiseModel
		        group a by new {venderName = a.VenderName, shippingmethodName = a.ShippingmethodName, name = a.Name}
		        into g
		        select new GetChargePayAnayiseModel
		        {
			        Name = g.Key.name,
			        VenderName = g.Key.venderName,
			        ShippingmethodName = g.Key.shippingmethodName,
			        ReceivingAmount = g.Sum(p => p.ReceivingAmount.Value),
			        DeliveryAmount = g.Sum(p => p.DeliveryAmount.Value),
			        Rate =((g.Sum(p => p.ReceivingAmount.Value) - g.Sum(p => p.DeliveryAmount.Value))/g.Sum(p => p.DeliveryAmount.Value)*100).ToString("#0.0000")+"%"
		        };





			//服务商列表
			List<string> listList = new List<string>();
			foreach (var listItem in listVender)
			{
				if (!listList.Contains(listItem.VenderName))
				{
					listList.Add(listItem.VenderName);
				}
			}



			//服务商数据源
			List<GetChargePayAnayiseModel> listPayAnayiseModel = new List<GetChargePayAnayiseModel>();
			foreach (var item in listVender)
			{
				listPayAnayiseModel.Add(new GetChargePayAnayiseModel
				{
					Name = item.Name,
					VenderName = item.VenderName,
					ShippingmethodName = item.ShippingmethodName,
					ReceivingAmount = item.ReceivingAmount.Value,
					DeliveryAmount = item.DeliveryAmount.Value,
					Rate = item.Rate
				});

			}





            var titleList = new List<string>
            {
                "ShippingmethodName-渠道",
                "Name-客户名称",
                "ReceivingAmount-应收",
                "DeliveryAmount-应付",
                "Rate-利率"
            };

            string filename = "对账表按服务商汇总";
            ExportExcelByWeb.InitExcel(filename);
            HSSFWorkbook workbook = ExportExcelByWeb.InitializeWorkbook();



            decimal receivingAmount = 0; //应收合计
            decimal deliveryAmount = 0; //应付合计

            //服务商分组导出excel
			foreach (var vender in listList)
			{
				//当前数据源
				var getData = listPayAnayiseModel.Where(a => a.VenderName == vender).ToList();

				foreach (var venderItem in getData)
				{
					receivingAmount += venderItem.ReceivingAmount.Value;
					deliveryAmount += venderItem.DeliveryAmount.Value;
				}

				//导出excel
				ExportExcelByWeb.WriteToDownLoad(workbook, getData, titleList,
					null, 2, vender, model.FilterModel.StartTime.Value, model.FilterModel.EndTime.Value,
					receivingAmount, deliveryAmount, vender);


				//清空(避免累加)
				receivingAmount = 0;
				deliveryAmount = 0;
			}


            ExportExcelByWeb.WriteToStream(workbook);

            return View(getModel);
        }


        #region  应收应付报表数据绑定

        //查询应收应付报表数据
        public ChargePayAnayiseModel ChargePayDataBind(ChragePayAnalyeseFilterModel filter)
        {
            if (filter.StartTime == null)
            {
                filter.StartTime = DateTime.Parse(DateTime.Now.AddDays(1).AddMonths(-1).ToString("yyyy-MM-dd"));
            }
            if (filter.EndTime == null)
            {
                filter.EndTime = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
            }

            int TotalRecord = 0;
            int TotalPage = 0;
            var model = new ChargePayAnayiseModel();

            model = new ChargePayAnayiseModel
            {
                FilterModel = filter,
                PagedList = _financialService.GetChragePayAnayeseRecordPagedList(new ChragePayAnalyeseParam()
                {
                    CustomerCode = filter.CustomerCode,
                    VenderCode = filter.VenderCode,
                    StartTime = filter.StartTime,
                    EndTime = filter.EndTime,
                    ShippingMethodId = filter.ShippingMethodId,
                    Page = filter.Page,
                    PageSize = filter.PageSize
                }, out TotalRecord, out TotalPage)
                    .ToModelAsPageCollection<ChargePayAnalysesExt, GetChargePayAnayiseModel>()

            };


            return model;
        }

        //导出应收应付报表数据excel
        public ChargePayAnayiseModel GetExportChargePayAnalysesList(ChragePayAnalyeseFilterModel filter)
        {

            if (filter.StartTime == null)
            {
                filter.StartTime = DateTime.Parse(DateTime.Now.AddDays(1).AddMonths(-1).ToString("yyyy-MM-dd"));
            }
            if (filter.EndTime == null)
            {
                filter.EndTime = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
            }

            var model = new ChargePayAnayiseModel
            {
                FilterModel = filter,
                ListGetChargePayAnayis = _financialService.GetExportChargePayAnalysesList(new ChragePayAnalyeseParam()
                {
                    CustomerCode = filter.CustomerCode,
                    VenderCode = filter.VenderCode,
                    StartTime = filter.StartTime,
                    EndTime = filter.EndTime,
                    ShippingMethodId = filter.ShippingMethodId
                }).ToModelAsCollection<ChargePayAnalysesExt, GetChargePayAnayiseModel>()

            };

            return model;
        }

        #endregion

		/// <summary>
		/// 错误日志查询列表
		/// yungchu
		/// </summary>
		/// <returns></returns>
		public ActionResult JobErrorLogInfo(JobErrorLogFilterModel filterModel)
	    {
			return View(JobErrorLogsDataBind(filterModel));
	    }

		[HttpPost]
		[ActionName("JobErrorLogInfo")]
		[FormValueRequired("Search")]
		public ActionResult SearchJobErrorLogInfo(JobErrorLogs model)
		{
			return View(JobErrorLogsDataBind(model.FilterModel));
		}

		public JobErrorLogs JobErrorLogsDataBind(JobErrorLogFilterModel filterModel)
		{

			if (filterModel.StartTime == null)
			{
				filterModel.StartTime = DateTime.Parse(DateTime.Now.AddDays(1).AddMonths(-1).ToString("yyyy-MM-dd"));
			}
			if (filterModel.EndTime == null)
			{
				filterModel.EndTime = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
			}

			var model = new JobErrorLogs
			{
				FilterModel = filterModel,
				PagedList = _financialService.GetJobErrorLogsPagedList(new JobErrorLogsParam
				{
					JobType = filterModel.JobType,
					WayBillNumber = filterModel.WayBillNumber,
					StartTime = filterModel.StartTime.Value,
					EndTime = filterModel.EndTime.Value,
					Page = filterModel.Page,
					PageSize = filterModel.PageSize
				}).ToModelAsPageCollection<JobErrorLogExt, GetJobErrorLogs>()
			};

	        //jobType 日志类别
			var listStatus = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "收货",
					TextField_EN = "收货"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "发货",
					TextField_EN = "发货"
				}
			};

			model.StatusList.Add(new SelectListItem() { Text = "请选择", Value = "", Selected = !filterModel.JobType.HasValue });
			listStatus.ForEach(a=>
				model.StatusList.Add(new SelectListItem(){Text = a.TextField,Value = a.ValueField,Selected = filterModel.JobType.HasValue&&a.ValueField==filterModel.JobType.Value.ToString()})
				);
			
			

			return model;
	
		}





	    public ActionResult ReceivingBillList([ModelBinder(typeof(StringTrimModelBinder))] ReceivingBillFilterModel filterModel)
        {
            return View(ReceivingBillListDataBind(filterModel));
        }

        private ReceivingBillViewModel ReceivingBillListDataBind(ReceivingBillFilterModel filterModel)
        {
            ReceivingBillParam param = new ReceivingBillParam();

            //param.Status = 2;

            if (!filterModel.IsFirstIn.HasValue)
            {
                filterModel.IsFirstIn = true;
            }

            if (filterModel.IsFirstIn.Value)
            {
                filterModel.StartTime = DateTime.Now.Date.AddMonths(-3);
                filterModel.EndTime = DateTime.Now;
            }

            if (filterModel.EndTime.HasValue)
            {
                filterModel.EndTime = filterModel.EndTime.Value.Date.AddDays(1).AddMinutes(-1);
            }

            filterModel.IsFirstIn = false;

            filterModel.CopyTo(param);

            var model = new ReceivingBillViewModel()
            {
                FilterModel = filterModel,
                PagedList = _financialService.GetReceivingBillPagedList(param),
            };

            return model;
        }
		/// <summary>
		/// 发货审核偏差表
		///  yungchu
		/// </summary>
		/// <returns></returns>
		public ActionResult DeliveryDeviation(DeliveryDeviationFilterModel filterModel)
		{
			return View(DeliveryDeviationDataBind(filterModel));
		}

		[HttpPost]
		[FormValueRequired("Search")]
		public ActionResult DeliveryDeviation(DeliveryDeviationModel model)
		{
			return View(DeliveryDeviationDataBind(model.FilterModel));
		}

		public DeliveryDeviationModel DeliveryDeviationDataBind(DeliveryDeviationFilterModel filterModel)
	    {
		    var model = new DeliveryDeviationModel
		    {
			    FilterModel = filterModel,
			    PagedList = _financialService.GetDeliveryDeviationPagedList(new DeliveryDeviationParam()
			    {
				    VenderCode = filterModel.VenderCode,
					VenderName = filterModel.VenderName,
				    ShippingmethodID = filterModel.ShippingmethodID,
					ShippingmethodName = filterModel.ShippingmethodName,
					Page = filterModel.Page,
					PageSize = filterModel.PageSize
				})
		    };
		    return model;
	    }
		//删除
		public ActionResult DeleteDeliveryDeviations(int id)
	    {
			try
			{
				var isSuccess = _financialService.DeleteDeliveryDeviations(id);
				if (isSuccess) SuccessNotification("删除成功");
				else ErrorNotification("删除失败");

			}
			catch (Exception e)
			{
				Log.Exception(e);
				ErrorNotification("删除失败，原因为：" + e.Message);
			}
			return RedirectToAction("DeliveryDeviation");
	    }

		//编辑或新增
		public ActionResult AddOrEditDeliveryDeviation(int type,int id)
		{
			//type=1 新增 type=2 编辑
			DeliveryDeviationModel model=new DeliveryDeviationModel();

			if (type == 2)
			{
				DeliveryDeviationExt getModel = _financialService.GetDeliveryDeviationInfo(id);
				model.GetVenderCode = getModel.VenderCode;
				model.GetVenderName = getModel.VenderName;
				model.GetShippingmethodId = getModel.ShippingmethodID;
				model.GetShippingmethodName = getModel.ShippingmethodName;

				model.GetWaillDeviationValue = getModel.WaillDeviationValue;
				model.GetWaillDeviationRate = getModel.WaillDeviationRate*100;
				model.GetWeightDeviationValue = getModel.WeightDeviationValue;
				model.GetWeightDeviationRate = getModel.WeightDeviationRate*100;
			}


			model.GetTypeItem = type;
			model.GetId = id;
			return View(model);
		}
		//编辑，新增数据
		public JsonResult AddOrEditDeliveryDeviationInfo(DeliveryDeviationFilterModel filterModel)
	    {
			var getResult = new ResponseResult();

			 //新增
			if (filterModel.Type == 1)
			{
			 	List<DeliveryDeviation> listEntity=GetAddOrEditInfo(filterModel);
				try
				{
					var isSuccess = _financialService.AddDeliveryDeviations(listEntity);
					if (isSuccess)
					{
						getResult.Result = true;
					}
					else
					{
						getResult.Result = false;
						getResult.Message = "保存失败";
					}

				}
				catch (Exception ex)
				{
					getResult.Result = false;
					getResult.Message = ex.Message;
				}

			}//编辑
			else if (filterModel.Type == 2)
			{
				try
				{
					List<DeliveryDeviation> listEntity = GetAddOrEditInfo(filterModel);


					//#region 操作日志
					//DeliveryDeviationExt getModel = _financialService.GetDeliveryDeviationInfo(filterModel.DeliveryId);
					//string waillDeviation = getModel.WaillDeviationValue.HasValue ? getModel.WaillDeviationValue.ToString() : getModel.WaillDeviationRate.HasValue ? getModel.WaillDeviationRate.Value.ToString() : "0";
					//string weightDeviation = getModel.WeightDeviationValue.HasValue ? getModel.WeightDeviationValue.ToString() : getModel.WeightDeviationRate.HasValue ? getModel.WeightDeviationRate.Value.ToString() : "0";


					//StringBuilder sb = new StringBuilder();
					//sb.Append("");

					//foreach (var item in listEntity)
					//{
					//	if (filterModel.WayBillFeeDeviations != null && waillDeviation!=filterModel.WayBillFeeDeviations.Value.ToString())
					//	{
					//		sb.AppendFormat(" 服务商{0},运输方式{1}的运费偏差从{2}更改为{3}", filterModel.VenderName, filterModel.ShippingmethodName, waillDeviation, filterModel.WayBillFeeDeviations);
					//	}

					//	if (filterModel.WeightDeviations != null && weightDeviation != filterModel.WeightDeviations.Value.ToString())
					//	{
					//		sb.AppendFormat(" 服务商{0},运输方式{1}的重量偏差从{2}更改为{3}", filterModel.VenderName, filterModel.ShippingmethodName, weightDeviation, filterModel.WeightDeviations);
					//	}


					//	//yungchu
					//	//敏感字-偏差值
					//	BizLog bizlog = new BizLog()
					//	{
					//		Summary = sb.ToString() != "" ? "[发货费用偏差率设置]" + sb : "发货费用偏差率设置",
					//		KeywordType = KeywordType.ShippingMethodId,
					//		Keyword =item.ShippingmethodName ,
					//		UserCode = _workContext.User.UserUame,
					//		UserRealName = _workContext.User.UserUame,
					//		UserType = UserType.LMS_User,
					//		SystemCode = SystemType.LMS,
					//		ModuleName = "发货费用偏差率"
					//	};
					//	_operateLogServices.WriteLog(bizlog, item);

					//}
					//#endregion

					var isSuccess = _financialService.UpdateDeliveryDeviations(listEntity, filterModel.DeliveryId);
					if (isSuccess)
					{
						getResult.Result = true;
					}
					else
					{
						getResult.Result = false;
						getResult.Message = "更新失败";
					}
				}
				catch (Exception ex)
				{
					getResult.Result = false;
					getResult.Message = ex.Message;
				}

			}

			return Json(getResult, JsonRequestBehavior.AllowGet);
	    }


		public List<DeliveryDeviation> GetAddOrEditInfo(DeliveryDeviationFilterModel filterModel)
		{
			DeliveryDeviation ddDeviation = new DeliveryDeviation();
			DeliveryDeviation ddDeliveryDeviation = new DeliveryDeviation();


			if (filterModel.WayBillFeeString == "偏差值")
			{
				ddDeliveryDeviation.DeviationValue = filterModel.WayBillFeeDeviations.HasValue?filterModel.WayBillFeeDeviations.Value:0;
			}
			else
			{
				ddDeliveryDeviation.DeviationRate = filterModel.WayBillFeeDeviations.HasValue? filterModel.WayBillFeeDeviations.Value:0;
			}

			if (filterModel.WeightString == "偏差值")
			{
				ddDeviation.DeviationValue = filterModel.WeightDeviations.HasValue?filterModel.WeightDeviations.Value:0;
			}
			else
			{
				ddDeviation.DeviationRate = filterModel.WeightDeviations.HasValue?filterModel.WeightDeviations.Value:0;
			}

	
			List<DeliveryDeviation> listEntity = new List<DeliveryDeviation>()
			   {
				new DeliveryDeviation()
				{
					   VenderName=filterModel.VenderName,
					   VenderCode=filterModel.VenderCode,
					   ShippingmethodID = filterModel.ShippingmethodID,
					   ShippingmethodName  =filterModel.ShippingmethodName,
					   DeviationType  =1,//'1-运费，2-重量',   
					   DeviationValue  =  ddDeliveryDeviation.DeviationValue, 
					   DeviationRate   =  ddDeliveryDeviation.DeviationRate/100

				},
				new DeliveryDeviation()
				{
					   VenderName=filterModel.VenderName,
					   VenderCode=filterModel.VenderCode,
					   ShippingmethodID = filterModel.ShippingmethodID,
					   ShippingmethodName  =filterModel.ShippingmethodName,
					   DeviationType  =2,//'1-运费，2-重量',   
					   DeviationValue  =  ddDeviation.DeviationValue ,
					   DeviationRate   = ddDeviation.DeviationRate/100 
				}
			};

			return listEntity;

		}

    }



}
