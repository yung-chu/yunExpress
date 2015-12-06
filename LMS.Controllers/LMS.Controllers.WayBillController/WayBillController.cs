using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Xml;
using Aspose.BarCode;
using LighTake.Infrastructure.Common.BizLogging.Enums;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LMS.Data.Express.DHL.Request;
using LMS.Data.Repository;
using LMS.Services.CommonServices;
using LMS.Services.CountryServices;
using LMS.Services.CustomerOrderServices;
using LMS.Models;
using LMS.Services.CustomerServices;
using LMS.Services.DictionaryTypeServices;
using LMS.Services.EubWayBillServices;
using LMS.Services.ExpressServices;
using LMS.Services.FeeManageServices;
using LMS.Services.FreightServices;
using LMS.Services.InStorageServices;
using LMS.Services.OrderServices;
using LMS.Services.OutStorageServices;
using LMS.Services.ReturnGoodsServices;
using LMS.Services.SequenceNumber;
using LMS.Services.TrackingNumberServices;
using LMS.Services.UserServices;
using LMS.Services.WayBillTemplateServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Caching;
using LighTake.Infrastructure.Common.Excel;
using LighTake.Infrastructure.Common.InversionOfControl;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Web;
using LighTake.Infrastructure.Web.Controllers;
using LighTake.Infrastructure.Web.Filters;
using LighTake.Infrastructure.Web.Utities;
using RazorEngine;
using Customer = LMS.Data.Entity.Customer;
using System.Threading;
using LMS.Services.OperateLogServices;
using LighTake.Infrastructure.Common.BizLogging;
using WayBillInfoExt = LMS.Data.Entity.WayBillInfoExt;
using LMS.Services.SF;

namespace LMS.Controllers.WayBillController
{
    public partial class WayBillController : BaseController
    {
        private IInStorageService _inStorageService;
        private ICustomerService _customerService;
        private ICustomerOrderService _customerOrderService;
        private IOutStorageService _outStorageService;
        private IOrderService _orderService;
        private IFeeManageService _feeManageService;
        private IFreightService _freightService;
        private IWorkContext _workContext;
        private IUserService _userService;
        private ITrackingNumberService _trackingNumberService;
        private ICountryService _countryService;
        private IWayBillTemplateService _wayBillTemplateService;
        private IExpressService _expressService;
        private IReturnGoodsService _returnGoodsService;
        private IEubWayBillService _eubWayBillService;
        private IOperateLogServices _operateLogServices;
        private readonly IWayBillInfoRepository _wayBillInfoRepository;
        private readonly IDictionaryTypeService _dictionaryTypeService;
        private readonly IWayBillTemplateInfoRepository _wayBillTemplateInfoRepository;
        private IInsuredCalculationService _insuredCalculationService;
        private ISensitiveTypeInfoService _sensitiveTypeInfoService;
        private IGoodsTypeService _goodsTypeService;
        private IAbnormalWayBillLogRepository _abnormalWayBillLogRepository;
	    private IApplicationInfoRepository _applicationInfoRepository;

        private readonly string _tempPathForExcel;


        public WayBillController(IInStorageService inStorageService,
                                ICustomerService customerService,
                                IOutStorageService outStorageService,
                                IOrderService orderService,
                                IFeeManageService feeManageService,
                                IFreightService freightService,
                                IWorkContext workContext,
                                IUserService userService,
                                ITrackingNumberService trackingNumberService,
                                ICustomerOrderService customerOrderService,
                                ICountryService countryService,
                                IWayBillTemplateService wayBillTemplateService,
                                IExpressService expressService,
                                IDictionaryTypeService dictionaryTypeService,
                                IWayBillInfoRepository wayBillInfoRepository,
                                IWayBillTemplateInfoRepository wayBillTemplateInfoRepository,
                                IReturnGoodsService returnGoodsService,
                                IEubWayBillService eubWayBillService,
                                IInsuredCalculationService insuredCalculationService,
                                ISensitiveTypeInfoService sensitiveTypeInfoService,
								IGoodsTypeService goodsTypeService, IAbnormalWayBillLogRepository abnormalWayBillLogRepository, IOperateLogServices operateLogServices, IApplicationInfoRepository applicationInfoRepository)
        {
            _inStorageService = inStorageService;
            _customerService = customerService;
            _outStorageService = outStorageService;
            _orderService = orderService;
            _feeManageService = feeManageService;
            _freightService = freightService;
            _workContext = workContext;
            _userService = userService;
            _trackingNumberService = trackingNumberService;
            _customerOrderService = customerOrderService;
            _countryService = countryService;
            _wayBillTemplateService = wayBillTemplateService;
            _dictionaryTypeService = dictionaryTypeService;
            _wayBillInfoRepository = wayBillInfoRepository;
            _expressService = expressService;
            _returnGoodsService = returnGoodsService;
            _wayBillTemplateInfoRepository = wayBillTemplateInfoRepository;
            _eubWayBillService = eubWayBillService;
            _insuredCalculationService = insuredCalculationService;
            _sensitiveTypeInfoService = sensitiveTypeInfoService;
            _goodsTypeService = goodsTypeService;
            _abnormalWayBillLogRepository = abnormalWayBillLogRepository;
            _operateLogServices = operateLogServices;
	        _applicationInfoRepository = applicationInfoRepository;

            try
            {
                _tempPathForExcel = sysConfig.TemporaryPath ?? string.Empty;

                if (!Directory.Exists(_tempPathForExcel))
                {
                    Directory.CreateDirectory(_tempPathForExcel);
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("失败创建文件夹[{0}]", _tempPathForExcel));
            }
        }


        public ActionResult FristInStorage()
        {
            // var model = new InStorageScanViewModel();
            var model = new InStorageScanViewModel()
                {
                    PrintSpecification = _dictionaryTypeService.GetSelectList(DictionaryTypeInfo.WayBillTemplateSpecification)

                };
            _inStorageService.GetGoodsTypeList(false).ForEach(g => model.GoodsTypeModels.Add(new SelectListItem()
            {
                Text = g.GoodsTypeName,
                Value = g.GoodsTypeID.ToString()
            }));
            WayBill.GetScanTypesList().ForEach(p => model.ScanTypeModels.Add(new SelectListItem()
                {
                    Text = p.TextField,
                    Value = p.ValueField
                }));

            model.BusinessDate = System.DateTime.Now;

            //model.SensitiveTypes = GetSensitiveTypeList();
            //model.SensitiveTypes.FirstOrDefault().Selected = true;
            return View(model);
        }

        public List<SelectListItem> GetSensitiveTypeList()
        {
            var list = new List<SelectListItem>();
            _sensitiveTypeInfoService.GetList().ForEach(c => list.Add(new SelectListItem
            {
                Value = c.SensitiveTypeID.ToString(CultureInfo.InvariantCulture),
                Text = c.SensitiveTypeName
            }));

            return list;
        }

        public ActionResult FristOutStorage()
        {
            var model = new OutStorageScanViewModel();
            _inStorageService.GetGoodsTypeList(false).ForEach(g => model.GoodsTypeModels.Add(new SelectListItem()
            {
                Text = g.GoodsTypeName,
                Value = g.GoodsTypeID.ToString()
            }));
            return View(model);
        }

        public ActionResult SelectVender()
        {
            return View();
        }

        public ActionResult NewSelectVender()
        {
            return View();
        }

        public JsonResult GetSelectVender(string keyWord, bool IsAll)
        {
            //添加缓存机制 周建春 过期时间5Minutes
            string cacheKey = string.Format("/LMS/SelectVender/{0}_{1}", keyWord.Trim().ToLower(), IsAll ? 1 : 0);

            if (Cache.Exists(cacheKey))
            {
                return Json(Cache.Get<IEnumerable<Vender>>(cacheKey), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var list = _freightService.GetVenderList(IsAll).WhereIf(p =>
                                                                        p.VenderCode.Contains(keyWord.ToUpper()) ||
                                                                        p.VenderName.Contains(keyWord),
                                                                        !string.IsNullOrWhiteSpace(keyWord));

                if (list != null && list.Any())
                {
                    Cache.Add(cacheKey, list, DateTime.Now.AddMinutes(5));
                }

                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        // 避免多次弹出层相同服务商,数据读不出 ???
        public ActionResult SelectVenderInfo()
        {
            return View();
        }

        public JsonResult GetSelectVenderInfo(string keyWord, bool IsAll)
        {
            //添加缓存机制 周建春 过期时间5Minutes
            string cacheKey = string.Format("/LMS/SelectVenderInfo/{0}_{1}", keyWord.Trim().ToLower(), IsAll ? 1 : 0);


            if (Cache.Exists(cacheKey))
            {
                return Json(Cache.Get<IEnumerable<Vender>>(cacheKey), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var list = _freightService.GetVenderList(IsAll).WhereIf(p =>
                    p.VenderCode.Contains(keyWord.ToUpper()) ||
                    p.VenderName.Contains(keyWord),
                    !string.IsNullOrWhiteSpace(keyWord));
                if (list.Count() > 0)
                    Cache.Add(cacheKey, list, DateTime.Now.AddMinutes(5));

                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        //// 避免多次弹出层相同服务商,数据读不出
        //public ActionResult SelectVenderInfo()
        //{
        //    return View();
        //}
        //public JsonResult GetSelectVenderInfo(string keyWord, bool IsAll)
        //{
        //    //添加缓存机制 周建春 过期时间5Minutes
        //    string cacheKey = string.Format("/SelectVenderInfo/{0}_{1}", keyWord.Trim().ToLower(), IsAll ? 1 : 0);

        //    if (Cache.Exists(cacheKey))
        //    {
        //        return Json(Cache.Get<IEnumerable<Vender>>(cacheKey), JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        var list = _freightService.GetVenderList(IsAll).WhereIf(p =>
        //            p.VenderCode.Contains(keyWord.ToUpper()) ||
        //            p.VenderName.Contains(keyWord),
        //            !string.IsNullOrWhiteSpace(keyWord));

        //        Cache.Add(cacheKey, list, DateTime.Now.AddMinutes(5));


        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //}


        /// 直接出仓 跳转post请求
        /// add by yungchu
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        [ButtonPermissionValidator(PermissionRecords.FastOutStorageCode)]
        [System.Web.Mvc.ActionName("List")]
        [FormValueRequired("btnOutStorage")]
        public ActionResult FirstStorage(WayBillListFilterModel param)
        {
            string ReturnUrl = Request.Form["ReturnUrl"];
            string WayBillNumbers = Request.Form["OrderNumber"];
            var model = new FastOutStorageViewModel();
            if (!string.IsNullOrWhiteSpace(WayBillNumbers))
            {
                model.WayBillNumbers = WayBillNumbers;
            }
            else
            {
                model.ErrorMessage = "没有直接出仓的运单号";
            }
            model.ReturnUrl = !string.IsNullOrWhiteSpace(ReturnUrl) ? ReturnUrl : "";
            _inStorageService.GetGoodsTypeList(false).ForEach(g => model.GoodsTypeModels.Add(new SelectListItem()
            {
                Text = g.GoodsTypeName,
                Value = g.GoodsTypeID.ToString()
            }));
            return View("FastOutStorage", model);


        }

        #region 总包号
        public string CreateTotalPackageNumber()
        {
            return _outStorageService.CreateTotalPackageNumber();
        }

        public string GetTotalPackageNumberLis(string venderCode)
        {
            var list = _outStorageService.GetTotalPackageNumberList(venderCode);
            if (list != null && list.Count > 0)
            {
                return string.Join("|", list);
            }
            return "";
        }

        public ActionResult EditTotalPackageTime(EditTotalPackageTimeFilterModel param)
        {
            if (param.StartTime == null)
            {
                param.StartTime = DateTime.Parse(DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd") + " " + "00:00");
            }
            if (param.EndTime == null)
            {
                param.EndTime = DateTime.Parse(DateTime.Now.AddDays(+1).ToString("yyyy-MM-dd") + " " + "00:00");
            }
            return View(TotalPackageListDataBind(param));
        }

        public JsonResult PostEditTime()
        {
            var result = new ResponseResult();
            if (Request.Form.HasKeys() && (!Request.Form["TotalPackageNumber"].IsNullOrWhiteSpace() || Request.Form["ishasValue"] == "1"))
            {
                var config = TotalPackageConfig.GetShowTimeList();
                var list = new List<TotalPackageTraceInfo>();
                var dlist = new List<TotalPackageTraceInfo>();
                foreach (var d in config)
                {
                    if (!Request.Form["Trace_" + d.ValueField].IsNullOrWhiteSpace())
                    {
                        list.Add(new TotalPackageTraceInfo()
                            {
                                TraceEventAddress = Request.Form["Address_" + d.ValueField],
                                TraceEventCode = Int32.Parse(d.ValueField),
                                TraceEventTime = DateTime.Parse(Request.Form["Trace_" + d.ValueField]),
                                TotalPackageNumber = Request.Form["TotalPackageNumber"]
                            });
                    }
                    else
                    {
                        dlist.Add(new TotalPackageTraceInfo()
                        {
                            TraceEventCode = Int32.Parse(d.ValueField),
                            TotalPackageNumber = Request.Form["TotalPackageNumber"]
                        });
                    }
                }
                try
                {
                    if (list.Any())
                    {
                        _outStorageService.EditTotalPackageTraceTime(list);
                    }
                    if (dlist.Any())
                    {
                        _outStorageService.DeleteTotalPackageTraceTime(dlist);
                    }
                    result.Result = true;
                    result.Message = "保存成功！";
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    result.Result = false;
                    result.Message = "保存失败！";
                }
            }
            else
            {
                result.Result = false;
                result.Message = "提交数据为空！";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("EditTotalPackageTime")]
        [FormValueRequired("btnSearch")]
        public ActionResult SearchEditTotalPackageTime(EditTotalPackageTimeViewModel filter)
        {
            filter.PagedList.PageIndex = 1;
            return View(TotalPackageListDataBind(filter.FilterModel));
        }

        public ActionResult EditTime(string totalPackageNumber, DateTime createdTime)
        {
            var model = new EditTotalPackageTimeOneModel
                {
                    CreatedTime = _outStorageService.GetTotalPackageTraceLastTime(totalPackageNumber),
                    TotalPackageNumber = totalPackageNumber,
                    AddressList = _outStorageService.GetTotalPackageAddress()
                };
            _outStorageService.GetTotalPackageTraceInfo(totalPackageNumber).ForEach(p => model.TraceInfos.Add(new TotalPackageTraceInfoModel()
                {
                    ID = p.ID,
                    TraceEventCode = p.TraceEventCode,
                    TraceEventTime = p.TraceEventTime,
                    TraceEventAddress = p.TraceEventAddress,
                    IsJob = p.IsJob
                }));
            return View(model);
        }

        public EditTotalPackageTimeViewModel TotalPackageListDataBind(EditTotalPackageTimeFilterModel param)
        {
            var model = new EditTotalPackageTimeViewModel
                {
                    FilterModel = param,
                    PagedList = _outStorageService.GetTotalPackageList(new EditTotalPackageTimeParam()
                        {
                            CreateBy = param.CreateBy,
                            EndTime = param.EndTime,
                            Page = param.Page,
                            PageSize = param.PageSize,
                            SearchContext = param.SearchContext,
                            StartTime = param.StartTime,
                            VenderCode = param.VenderCode
                        })
                };
            return model;
        }

        #endregion

        /// <summary>
        /// 保存需要直接出仓的单号到Session
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="wayBillNumbers"></param>
        /// <returns></returns>
        [ButtonPermissionValidator(PermissionRecords.FastOutStorageCode)]
        public JsonResult SaveFastOutStorageTemp(string returnUrl, string wayBillNumbers)
        {
            string r = Regex.Match(returnUrl, "(SearchContext=.*?)&").Value;
            string[] waybill_list_SearchContext =
                HttpUtility.UrlDecode(Regex.Match(returnUrl, "SearchContext=(.*?)&").Groups[1].Value)
                           .Split(new string[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

            Session["ReturnUrl"] = string.IsNullOrWhiteSpace(r) ? returnUrl : returnUrl.Replace(r, "");
            Session["WayBillNumbers"] = wayBillNumbers;
            Session["waybill_list_SearchContext"] = waybill_list_SearchContext;

            var result = new ResponseResult()
                {
                    Result = Session["ReturnUrl"] != null && Session["WayBillNumbers"] != null,
                    Message = Url.Action("FastOutStorageFromTemp")
                };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 从Session读取运单
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="wayBillNumbers"></param>
        /// <returns></returns>
        [ButtonPermissionValidator(PermissionRecords.FastOutStorageCode)]
        public ActionResult FastOutStorageFromTemp()
        {
            string ReturnUrl = Session["ReturnUrl"] as string;
            string WayBillNumbers = Session["WayBillNumbers"] as string;

            if (string.IsNullOrWhiteSpace(WayBillNumbers))
            {
                return RedirectToAction("List");
            }

            //释放内存
            Session.Remove("ReturnUrl");
            Session.Remove("WayBillNumbers");

            var model = new FastOutStorageViewModel();
            if (!string.IsNullOrWhiteSpace(WayBillNumbers))
            {
                model.WayBillNumbers = WayBillNumbers;
                var list = WayBillNumbers.Split(',').ToList();
                model.TotalVotes = list.Count;
                int totalqty = 0;
                decimal totalweight = 0;
                _outStorageService.GetWayBillTotalQty(list, out totalqty);
                _outStorageService.GetWayBillTotalWeight(list, out totalweight);
                model.TotalQty = totalqty;
                model.TotalWeight = totalweight;
            }
            else
            {
                model.ErrorMessage = "没有直接出仓的运单号";
            }
            model.ReturnUrl = !string.IsNullOrWhiteSpace(ReturnUrl) ? ReturnUrl : "";
            _inStorageService.GetGoodsTypeList(false).ForEach(g => model.GoodsTypeModels.Add(new SelectListItem()
            {
                Text = g.GoodsTypeName,
                Value = g.GoodsTypeID.ToString()
            }));
            return View("FastOutStorage", model);
        }

        /// <summary>
        /// 直接入仓按钮
        /// by zxq
        /// </summary>
        /// <param name="param"></param>
        /// <param name="shippingId">运输方式id</param>
        /// <param name="customerCode">客户代码</param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("List")]
        [FormValueRequired("btnInStorage")]
        [ButtonPermissionValidator(PermissionRecords.FastInStorageCode)]
        public ActionResult FirstInStorage(int shippingId, string customerCode)
        {
            string ReturnUrl = Request.Form["ReturnUrl"];
            string WayBillNumbers = Request.Form["OrderNumber"];

            var model = GetFastInStorageViewModel(ReturnUrl, WayBillNumbers, "", shippingId, customerCode);
            return View("FastInStorage", model);
        }





        #region 重量异常单直接入仓

        //缓存数据
        //yungchu
        [ButtonPermissionValidator(PermissionRecords.FastInStorageCode)]
        public JsonResult WeightAbnormalFirstInStorageTemp(WeightAbnormalParam param)
        {

            var result = new ResponseResult();
            result.Result = false;

            Session["ReturnUrl"] = param.ReturnUrl;
            Session["WayBillNumbers"] = param.WayBillNumbers;
            Session["getOpereate"] = param.GetOpereate;
            Session["shippingId"] = param.ShippingId;
            Session["customerCode"] = param.CustomerCode;


            bool isNull = !string.IsNullOrEmpty(param.WayBillNumbers) && !string.IsNullOrEmpty(param.ReturnUrl) &&
                          !string.IsNullOrEmpty(param.ShippingId) && !string.IsNullOrEmpty(param.CustomerCode);
            if (isNull)
            {
                result.Result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InStorageSyncErrorList(LighTake.Infrastructure.Web.Models.SearchFilter param)
        {
            var pagelist = _inStorageService.GetTaskList(1, -1, string.Empty, param.Page, param.PageSize <= 0 ? 50 : param.PageSize);
            InStorageSyncErrorListModel model = new InStorageSyncErrorListModel();
            model.List = pagelist;
            model.SearchParam = param;

            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult InStorageSyncRetry(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
            {
                var empty = new
                {
                    Status = 0,
                    Info = "无效任务"
                };
                return Json(empty);
            }
            try
            {
                List<long> list = new List<long>();
                foreach (var s in ids.Split(','))
                {
                    if (string.IsNullOrWhiteSpace(s)) continue;
                    long t = 0;
                    if (long.TryParse(s.Trim(), out t))
                    {
                        list.Add(t);
                    }
                }

                if (list.Count == 0)
                {
                    var resultEmpty = new
                    {
                        Status = 0,
                        Info = "无效任务"
                    };
                    return Json(resultEmpty);
                }

                if (_inStorageService.Retry(list.ToArray()))
                {
                    var result = new
                    {
                        Status = 1,
                        Info = ""
                    };
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex.InnerException == null ? ex : ex.InnerException);
            }
            var resultFailure = new
            {
                Status = 0,
                Info = "保存失败"
            };
            return Json(resultFailure);
        }

        public class WeightAbnormalParam
        {
            public string ReturnUrl { get; set; }
            public string WayBillNumbers { get; set; }
            public string GetOpereate { get; set; }
            public string ShippingId { get; set; }
            public string CustomerCode { get; set; }
        }


        /// <summary>
        /// 读取session数据
        /// add by yungchu
        /// </summary>
        /// <param name="shippingId"></param>
        /// <param name="customerCode"></param>
        /// <returns></returns>
        [ButtonPermissionValidator(PermissionRecords.FastInStorageCode)]
        public ActionResult WeightAbnormalFastInStorage()
        {
            string ReturnUrl = Session["ReturnUrl"] as string;
            string WayBillNumbers = Session["WayBillNumbers"] as string;
            string getOpereate = Session["getOpereate"] as string;
            string shippingId = Session["shippingId"] as string;
            string customerCode = Session["customerCode"] as string;


            if (string.IsNullOrWhiteSpace(WayBillNumbers))
            {
                return RedirectToAction("InStorageWeightAbnormal", "WayBill");
            }

            var model = GetFastInStorageViewModel(ReturnUrl, WayBillNumbers, getOpereate, int.Parse(shippingId), customerCode);

            //释放内存
            Session.Remove("ReturnUrl");
            Session.Remove("WayBillNumbers");
            Session.Remove("getOpereate");
            Session.Remove("shippingId");
            Session.Remove("customerCode");

            return View("FastInStorage", model);
        }

        //直接入仓之后解除运单hold
        public void CancelWayBillHold(List<string> wayBillNumbers)
        {
            var result = new ResponseResult();
            try
            {
                if (wayBillNumbers != null && wayBillNumbers.Count != 0)
                {
                    _orderService.BatchCancelHoldWayBillInfo(wayBillNumbers);
                    result.Result = true;
                }
            }
            catch (Exception ex)
            {
                result.Result = false;
                Log.Exception(ex);
            }

        }


        #endregion


        //返回直接入仓model 
        public FastInStorageViewModel GetFastInStorageViewModel(string ReturnUrl, string WayBillNumbers, string getOpereate, int shippingId, string customerCode)
        {
            var model = new FastInStorageViewModel();

            if (!string.IsNullOrEmpty(getOpereate))//入仓重量异常单入仓
            {
                model.Opereate = getOpereate;
            }

            if (!string.IsNullOrWhiteSpace(WayBillNumbers))
            {
                model.WayBillNumbers = WayBillNumbers;
            }
            else
            {
                model.ErrorMessage = "没有直接入仓的运单号";
            }

            model.CustomerCode = customerCode;
            model.ShippingMethodId = shippingId;

            var customer = _customerService.GetCustomer(customerCode);

            if (customer != null)
            {
                model.CustomerNickName = customer.Name;
            }
            else
            {
                model.ErrorMessage = "没有找到该客户";
            }

            var shippingMethods = _freightService.GetShippingMethodsByIds(new List<int>() { shippingId });

            if (shippingMethods != null && shippingMethods.Count > 0)
            {
                model.ShippingMethodName = shippingMethods[0].FullName;
            }
            else
            {
                model.ErrorMessage = "没有找到该运输方式";
            }

            model.ReturnUrl = !string.IsNullOrWhiteSpace(ReturnUrl) ? ReturnUrl : "";
            _inStorageService.GetGoodsTypeList(false).ForEach(g => model.GoodsTypeModels.Add(new SelectListItem()
            {
                Text = g.GoodsTypeName,
                Value = g.GoodsTypeID.ToString()
            }));

            return model;
        }



        public ActionResult List(WayBillListFilterModel param)
        {
            if (param.DateWhere == 0)
            {
                if (param.StartTime == null)
                {
                    param.StartTime = DateTime.Parse(DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd") + " " + "00:00");
                }
                if (param.EndTime == null)
                {
                    param.EndTime = DateTime.Parse(DateTime.Now.AddDays(+1).ToString("yyyy-MM-dd") + " " + "00:00");
                }
                param.DateWhere = 1;
            }


            if (string.IsNullOrWhiteSpace(param.SearchContext))
            {
                string[] searchContextlines = Session["waybill_list_SearchContext"] as string[];

                if (searchContextlines != null && searchContextlines.Any())
                {
                    Session["waybill_list_SearchContext"] = null;
                    param.SearchContext = string.Join(Environment.NewLine, searchContextlines);
                }
            }

            return View(ListDataBindSilm(param));
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult SaveWaybillListSearchContext(string searchContext)
        {
            Session["waybill_list_SearchContext"] = searchContext.Split(new string[] {Environment.NewLine},                                                    StringSplitOptions.RemoveEmptyEntries);
            var result = new ResponseResult()
            {
                Result = true,
            };
            return Json(result);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("List")]
        [FormValueRequired("btnSearch")]
        public ActionResult SearachList(WayBillListViewModel param)
        {
            param.FilterModel.Page = 1;

            int oldPageSize = param.FilterModel.PageSize;

            if (!string.IsNullOrWhiteSpace(param.FilterModel.SearchContext))
            {
                param.FilterModel.PageSize = 2000;
            }

            var model = ListDataBindSilm(param.FilterModel);

            param.FilterModel.PageSize = oldPageSize;

            return View("List", model);
        }

        public ActionResult TrackList(WayBillListFilterModel param)
        {
            if (param.DateWhere == 0)
            {
                if (param.StartTime == null)
                {
                    param.StartTime = DateTime.Parse(DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd") + " " + "00:00");
                }
                if (param.EndTime == null)
                {
                    param.EndTime = DateTime.Parse(DateTime.Now.AddDays(+1).ToString("yyyy-MM-dd") + " " + "00:00");
                }
                param.DateWhere = 1;
            }
            return View(ListDataBind(param));
        }

        [System.Web.Mvc.HttpPost]
        [FormValueRequired("btnSearch")]
        public ActionResult TrackList(WayBillListViewModel param)
        {
            param.FilterModel.Page = 1;
            return View(ListDataBind(param.FilterModel));
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("TrackList")]
        [FormValueRequired("btnExport")]
        public ActionResult ExportTrackList(WayBillListViewModel param)
        {

            List<ExportWayBillModel> models = new List<ExportWayBillModel>();
            List<string> wayBillNumbers = new List<string>();
            var countryList = _countryService.GetCountryList("");
            var customerList = _customerService.GetCustomerList("", false);
            ExportWayBillModel model = new ExportWayBillModel();
            //得到运单号
            int MaxSubColum = 0;
            SelectWayBillList(param.FilterModel).WayBillInfoModels.ForEach(WayBillInfoModel =>
            {
                model = new ExportWayBillModel();
                CustomerOrderInfoModel customerOrderInfos = _orderService.GetCustomerOrderInfoById(WayBillInfoModel.CustomerOrderID).ToModel<CustomerOrderInfoModel>();
                if (customerOrderInfos != null)
                {
                    model.CustomerOrderNumber = customerOrderInfos.CustomerOrderNumber;
                    model.CustomerCode = customerOrderInfos.CustomerCode;
                    if (customerList.FirstOrDefault(p => p.CustomerCode == model.CustomerCode) != null)
                    {
                        model.Name = customerList.First(p => p.CustomerCode == model.CustomerCode).Name;
                    }
                    else
                    {
                        model.Name = "";
                    }
                    model.InsureAmount = customerOrderInfos.InsureAmount;
                    model.PackageNumber = customerOrderInfos.PackageNumber;
                    model.AppLicationType = CustomerOrder.GetApplicationTypeDescription(customerOrderInfos.AppLicationType);
                }
                else
                {
                    model.CustomerOrderNumber = "";
                }
                model.WayBillNumber = WayBillInfoModel.WayBillNumber;
                model.InShippingMethodName = WayBillInfoModel.InShippingMethodName;
                model.TrackingNumber = WayBillInfoModel.TrackingNumber;
                model.TrueTrackingNumber = WayBillInfoModel.TrueTrackingNumber;
                model.Weight = WayBillInfoModel.Weight;
                model.SettleWeight = WayBillInfoModel.SettleWeight;
                model.Length = WayBillInfoModel.Length;
                model.Width = WayBillInfoModel.Width;
                model.Height = WayBillInfoModel.Height;
                model.WayCreatedOn = WayBillInfoModel.CreatedOn.ToString();
                model.ShiCreatedOn = WayBillInfoModel.InStorageTime.ToString();
                model.SenCreatedOn = WayBillInfoModel.OutStorageTime.ToString();
                model.Status = WayBill.GetStatusDescription(WayBillInfoModel.Status);
                ShippingInfoModel shippingInfo = _orderService.GetshippingInfoById(WayBillInfoModel.ShippingInfoID).ToModel<ShippingInfoModel>();
                SenderInfoModel senderInfo = _orderService.GetSenderInfoById(WayBillInfoModel.SenderInfoID).ToModel<SenderInfoModel>();
                if (shippingInfo != null)
                {
                    model.CountryCode = shippingInfo.CountryCode;
                    if (countryList.First(p => p.CountryCode == model.CountryCode) != null)
                    {
                        model.ChineseName = countryList.First(p => p.CountryCode == model.CountryCode).ChineseName;
                    }
                    else
                    {
                        model.ChineseName = "";
                    }
                    model.ShippingFirstName = shippingInfo.ShippingFirstName;
                    model.ShippingLastName = shippingInfo.ShippingLastName;
                    model.ShippingAddress = shippingInfo.ShippingAddress;
                    model.ShippingCity = shippingInfo.ShippingCity;
                    model.ShippingState = shippingInfo.ShippingState;
                    model.ShippingZip = shippingInfo.ShippingZip;
                    model.ShippingPhone = shippingInfo.ShippingPhone;
                    model.ShippingCompany = shippingInfo.ShippingCompany;
                    model.ShippingTaxId = shippingInfo.ShippingTaxId;
                }
                else
                {
                    model.CountryCode = "";
                    model.ChineseName = "";
                    model.ShippingFirstName = "";
                    model.ShippingLastName = "";
                    model.ShippingAddress = "";
                    model.ShippingCity = "";
                    model.ShippingState = "";
                    model.ShippingZip = "";
                    model.ShippingPhone = "";
                    model.ShippingCompany = "";
                    model.ShippingTaxId = "";
                }
                if (senderInfo != null)
                {
                    model.SenderFirstName = senderInfo.SenderFirstName;
                    model.SenderLastName = senderInfo.SenderLastName;
                    model.SenderCompany = senderInfo.SenderCompany;
                    model.SenderAddress = senderInfo.SenderAddress;
                    model.SenderCity = senderInfo.SenderCity;
                    model.SenderState = senderInfo.SenderState;
                    model.SenderZip = senderInfo.SenderZip;
                    model.SenderPhone = senderInfo.SenderPhone;
                }
                else
                {
                    model.SenderFirstName = "";
                    model.SenderLastName = "";
                    model.SenderCompany = "";
                    model.SenderAddress = "";
                    model.SenderCity = "";
                    model.SenderState = "";
                    model.SenderZip = "";
                    model.SenderPhone = "";
                }
                model.IsReturn = WayBillInfoModel.IsReturn;
                InsuredCalculationModel insuredCalculation = _orderService.GetInsuredCalculationById(WayBillInfoModel.InsuredID).ToModel<InsuredCalculationModel>();
                if (insuredCalculation != null)
                {
                    model.InsuredName = insuredCalculation.InsuredName;
                }
                else
                {
                    model.InsuredName = "";
                }
                CustomerOrderInfoModel customerOrderInfo = _orderService.GetCustomerOrderInfoById(WayBillInfoModel.CustomerOrderID).ToModel<CustomerOrderInfoModel>();
                SensitiveTypeInfoModel sensitiveTypeInfo = _orderService.GetSensitiveTypeInfoById(customerOrderInfo.SensitiveTypeID).ToModel<SensitiveTypeInfoModel>();
                if (customerOrderInfos != null && sensitiveTypeInfo != null)
                {
                    model.SensitiveTypeName = sensitiveTypeInfo.SensitiveTypeName;
                }
                else
                {
                    model.SensitiveTypeName = "";
                }


                List<ApplicationInfoModel> applicationInfoModels =
                    _orderService.GetApplicationInfoByWayBillNumber(WayBillInfoModel.WayBillNumber)
                                 .ToModelAsCollection<ApplicationInfo, ApplicationInfoModel>();
                model.ApplicationInfoModels = applicationInfoModels;
                if (applicationInfoModels != null)
                {
                    if (applicationInfoModels.Count >= MaxSubColum)
                    {
                        MaxSubColum = applicationInfoModels.Count;
                    }
                }
                models.Add(model);
                wayBillNumbers.Add(WayBillInfoModel.WayBillNumber.ToString());
            });
            List<string> lstName = new List<string>
                {
                    "客户订单号",
                    "客户代码",
                    "客户名称",
                    "运单号",
                    "入仓运输方式",
                    "系统跟踪号",
                    "真实跟踪号",
                    "国家简码",
                    "国家中文名",
                    "收件人姓",
                    "收件人名字",
                    "收件人公司",
                    "收货地址",
                    "城市",
                    "省/州",
                    "邮编",
                    "电话",
                    "创建时间",
                    "收货时间",
                    "发货时间",
                    "状态",
                    "收件人税号",
                    "发件人姓",
                    "发件人名",
                    "发件人公司",
                    "发件人地址",
                    "城市",
                    "省/州",
                    "发件人邮编",
                    "发件人电话",
                    "是否退回",
                    "保险类型",
                    "保险价值RMB",
                    "敏感货物",
                    "申报类型",
                    "件数",
                    "长cm",
                    "宽cm",
                    "高cm",
                    "称重重量kg",
                    "结算重量kg",
                    "是否关税预付"
                };
            for (int i = 1; i <= MaxSubColum; i++)
            {
                lstName.Add("申报名称" + i);
                lstName.Add("申报中文名称" + i);
                lstName.Add("海关编码" + i);
                lstName.Add("数量" + i);
                lstName.Add("单价" + i + "(usd)");
                lstName.Add("净重量" + i + "(kg)");
                lstName.Add("销售链接" + i);
                lstName.Add("备注" + i);
            }
            string fileName = sysConfig.ExcelTemplateWebPath + sysConfig.ExportWayBill;
            ExportExcelByWeb.TrackWayBillExcel(fileName, models, lstName);
            return View(ListDataBind(param.FilterModel));
        }

        #region 跟踪号上传
        /// <summary>
        /// 跟踪号上传
        /// Add by zhengsong
        /// </summary>
        /// <returns></returns>
        public ActionResult TrackingNumberList(string id = null)
        {
            TrackingNumberModel model = new TrackingNumberModel();
            var freightService = EngineContext.Current.Resolve<IFreightService>();
            var shippingMethodeList = freightService.GetShippingMethods(null, true);
            shippingMethodeList.ForEach(p => model.shippingMethods.Add(new SelectListItem()
            {
                Text = p.FullName,
                Value = p.ShippingMethodId.ToString()
            }));
            model.uploadTrackingNumberDetailModels =
                _trackingNumberService.GetTrackingNumberDetailById(id).ToModelAsCollection<TrackingNumberDetailInfo, UploadTrackingNumberDetailModel>();
            return View(model);
        }
        [System.Web.Mvc.HttpPost]
        [FormValueRequired("btnSave")]
        public ActionResult TrackingNumberList(HttpPostedFileBase file, TrackingNumberModel model)
        {
            if (model.Type == 1)
            {
                if (file == null)
                {
                    return RedirectToAction("TrackingNumberList");
                }
                model.filePath = SaveFile(file);
                //从Excel中获取跟踪号
                try
                {
                    model.uploadTrackingNumberDetailModels = GetTrackingNumberExcelList(model.filePath);
                    if (model.uploadTrackingNumberDetailModels.Count < 1)
                    {
                        return RedirectToAction("TrackingNumberList");
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    SetViewMessage(ShowMessageType.Error, "跟踪号存在为空", false);
                    return TrackingNumberList();
                }
            }
            else if (model.Type == 2)
            {
                int startSegment = 0;
                int endSegment = 0;
                int characterMaxLength = 4;

                #region 验证
                if (string.IsNullOrWhiteSpace(model.StartCharacter))
                {
                    ModelState.AddModelError("StartCharacter", "开始字符不能为空");
                }
                else if (model.StartCharacter.Length > characterMaxLength)
                {
                    ModelState.AddModelError("StartCharacter", "开始字符长度应该是0到4位.");
                }


                if (string.IsNullOrWhiteSpace(model.StartSegment))
                {
                    ModelState.AddModelError("StartSegment", "开始段号不能为空");
                }
                else if (!Regex.IsMatch(model.StartSegment, @"^\d{8}"))
                {
                    ModelState.AddModelError("StartSegment", "开始段号为8位数字组成");
                }

                if (string.IsNullOrWhiteSpace(model.EndSegment))
                {
                    ModelState.AddModelError("EndSegment", "结尾段号不能为空");
                }
                else if (!Regex.IsMatch(model.EndSegment, @"^\d{8}"))
                {
                    ModelState.AddModelError("EndSegment", "结尾段号为8位数字组成");
                }

                if (int.TryParse(model.StartSegment, out startSegment) && int.TryParse(model.EndSegment, out endSegment))
                {
                    if (startSegment > endSegment)
                    {
                        ModelState.AddModelError("StartSegment", "[开始段号]应该小于或者等于[结尾段号]");
                    }
                    int max = 10000;
                    if ((endSegment - startSegment) < 0 || (endSegment - startSegment) > max) //小于0，或者大于2k
                    {
                        ModelState.AddModelError("StartSegment", string.Format("[结尾段号]减去[开始段号]应该大于【0】并且小于等于【{0}】", max));
                        ModelState.AddModelError("EndSegment", string.Format("[结尾段号]减去[开始段号]应该大于【0】并且小于等于【{0}】", max));
                    }
                }

                if (string.IsNullOrWhiteSpace(model.EndCharacter))
                {
                    ModelState.AddModelError("EndCharacter", "结尾字符不能为空");
                }
                else if (model.EndCharacter.Length > characterMaxLength)
                {
                    ModelState.AddModelError("EndCharacter", "结尾字符长度应该是0到4位.");
                }

                if (!ModelState.IsValid)
                {
                    var freightService = EngineContext.Current.Resolve<IFreightService>();
                    var shippingMethodeList = freightService.GetShippingMethods(null, true);
                    shippingMethodeList.ForEach(p => model.shippingMethods.Add(new SelectListItem()
                    {
                        Text = p.FullName,
                        Value = p.ShippingMethodId.ToString()
                    }));
                    return View(model);
                }
                #endregion

                System.Collections.Concurrent.ConcurrentBag<UploadTrackingNumberDetailModel> tns = new System.Collections.Concurrent.ConcurrentBag<UploadTrackingNumberDetailModel>();
                int length = model.EndSegment.Length;
                int startNumber = Int32.Parse(model.StartSegment);
                int endNumber = Int32.Parse(model.EndSegment);
                System.Threading.Tasks.Parallel.For(startNumber, endNumber, i =>
                // for (int i = startNumber; i <= endNumber; i++)
                {
                    string tnumber = string.Empty;
                    if (startNumber.ToString().Length < length)
                    {
                        int num = length - startNumber.ToString().Length;
                        string zerro = "";
                        for (int j = 0; j < num; j++)
                        {
                            zerro += "0";
                        }
                        tnumber = string.Concat(model.StartCharacter, zerro, i.ToString(), GetCode(zerro + startNumber.ToString()), model.EndCharacter);
                    }
                    else
                    {
                        tnumber = string.Concat(model.StartCharacter, i.ToString(), GetCode(startNumber.ToString()), model.EndCharacter);
                    }
                    tns.Add(new UploadTrackingNumberDetailModel
                    {
                        TrackingNumber = tnumber
                    });
                    //startNumber++;
                });

                model.uploadTrackingNumberDetailModels.AddRange(tns);

            }
            //判断是否有相同的跟踪号
            if (model.uploadTrackingNumberDetailModels.Any(p => p.IsRepeat == 1))
            {
                string errorTrackingNumber = "";
                foreach (var row in model.uploadTrackingNumberDetailModels)
                {
                    if (row.IsRepeat == 1 && !errorTrackingNumber.Contains(row.TrackingNumber))
                    {
                        errorTrackingNumber += "[" + row.TrackingNumber + "]";
                    }
                }
                SetViewMessage(ShowMessageType.Error, "跟踪号" + errorTrackingNumber + "存在重复", false, false);
                return TrackingNumberList();
            }

            //生成跟踪号ID
            model.TrackingNumberID = SequenceNumberService.GetSequenceNumber(PrefixCode.TrackNumberID);
            model.ApplianceCountry = model.CountryList;
            model.Status = (short)TrackingNumberInfo.StatusEnum.Enable;
            model.CreatedBy = _workContext.User.UserUame;
            model.CreatedNo = DateTime.Now;
            model.LastUpdatedBy = _workContext.User.UserUame;
            model.LastUpdateOn = DateTime.Now;

            //上传跟踪号
            var trackingNumberList = _trackingNumberService.UploadTrackingNumberList(model.ToEntity<TrackingNumberInfo>());
            if (trackingNumberList.Count > 0)
            {
                if (trackingNumberList[0].Status == 0)
                {
                    //XXXX跟踪号已存在,给提示或者跳转
                    string errorTrackingNumber = "";
                    foreach (var row in trackingNumberList)
                    {
                        errorTrackingNumber += "[" + row.TrackingNumber + "]";
                    }
                    SetViewMessage(ShowMessageType.Error, "跟踪号" + errorTrackingNumber + "已存在", false, false);
                    return TrackingNumberList();
                }
                else
                {
                    SetViewMessage(ShowMessageType.Success, "保存成功", true);
                    string TrackingNumberID = model.TrackingNumberID;
                    return RedirectToAction("TrackingNumberDetail", "WayBill", new { id = TrackingNumberID });
                }
            }
            else
            {
                SetViewMessage(ShowMessageType.Error, "未找到数据", false);
                return TrackingNumberList();
            }
        }

        private string GetCode(string number)
        {
            int a = Int32.Parse(number.Substring(0, 1)) * 8 + Int32.Parse(number.Substring(1, 1)) * 6 +
                    Int32.Parse(number.Substring(2, 1)) * 4 + Int32.Parse(number.Substring(3, 1)) * 2 +
                    Int32.Parse(number.Substring(4, 1)) * 3 + Int32.Parse(number.Substring(5, 1)) * 5 +
                    Int32.Parse(number.Substring(6, 1)) * 9 + Int32.Parse(number.Substring(7, 1)) * 7;
            string b = (11 - (a % 11)).ToString();
            if (b == "10")
            {
                b = "0";
            }
            else if (b == "11")
            {
                b = "5";
            }
            return b;
        }

        #endregion

        #region 运单跟踪号上传

        public ActionResult NewTrackingNumber()
        {
            WayBillListViewModel model = new WayBillListViewModel();
            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        [FormValueRequired("btnUpload")]
        public ActionResult NewTrackingNumber(HttpPostedFileBase excelfile)
        {
            if (excelfile == null)
            {
                return RedirectToAction("NewTrackingNumber");
            }
            string strUniqueExcelFileName = Guid.NewGuid().ToString() + Path.GetExtension(excelfile.FileName);
            excelfile.SaveAs(_tempPathForExcel + strUniqueExcelFileName);
            WayBillListViewModel model = new WayBillListViewModel();
            List<WayBillInfoModel> wayBillInfo = new List<WayBillInfoModel>();
            //OrderListParam param=new OrderListParam();
            model.BtnSuccess = true;
            //wayBillInfo = _orderService.GetWayBillInfo(param).ToModelAsCollection<WayBillInfo,WayBillInfoModel>();
            try
            {
                model.WayBillTrackingNumbers = GetWayBillTrackingNumber(_tempPathForExcel + strUniqueExcelFileName);
            }
            catch (Exception ex)
            {
                SetViewMessage(ShowMessageType.Error, ex.Message, true);
                //出现Excel文本格式错误，导致不能正确读取Excel数据
                if (ex.Message == "Cannot get a text value from a numeric cell")
                {
                    SetViewMessage(ShowMessageType.Error, "Excel文本格式错误!", true);
                }
                Log.Exception(ex);
                return RedirectToAction("NewTrackingNumber");
            }

            List<string> list = new List<string>();
            model.WayBillTrackingNumbers.ForEach(p => list.Add(!string.IsNullOrWhiteSpace(p.WayBillNumber) ? p.WayBillNumber : p.TrackingNumber));

            wayBillInfo = _orderService.GetWayBillInfos(list.ToArray()).ToList().ToModelAsCollection<WayBillInfo, WayBillInfoModel>();
            if (model.WayBillTrackingNumbers.Count < 1)
            {
                return RedirectToAction("NewTrackingNumber");
            }

            model.UniqueExcelFileName = strUniqueExcelFileName;
            int total = 0;
            int successTotal = 0;
            model.WayBillTrackingNumbers.ForEach(p =>
                {
                    bool success = true;
                    int n = 0;
                    if (string.IsNullOrWhiteSpace(p.TrackingNumber))
                    {
                        p.ErrorMsg.Append("跟踪单号不能为空<br/>");
                        success = false;
                    }
                    foreach (var row in wayBillInfo)
                    {
                        if (row.WayBillNumber.ToUpperInvariant() == p.WayBillNumber.ToUpperInvariant() || row.CustomerOrderNumber.ToUpperInvariant() == p.WayBillNumber.ToUpperInvariant())
                        {
                            n++;
                        }
                        if (row.TrackingNumber != null)
                        {
                            if (row.TrackingNumber.ToUpperInvariant() == p.WayBillNumber.ToUpperInvariant())
                            {
                                n++;
                            }
                        }
                    }
                    if (_orderService.IsExitTrackingNumber(p.TrackingNumber, ""))
                    {
                        p.ErrorMsg.Append("跟踪单号已存在<br/>");
                        success = false;
                    }
                    if (model.WayBillTrackingNumbers.FindAll(z => z.TrackingNumber == p.TrackingNumber).Count > 1)
                    {
                        p.ErrorMsg.Append("跟踪单号存在重复<br/>");
                        success = false;
                    }
                    if (string.IsNullOrWhiteSpace(p.WayBillNumber))
                    {
                        p.ErrorMsg.Append("运单号/订单号/原跟踪号不能为空<br/>");
                        success = false;
                    }
                    else if (n == 0)
                    {
                        p.ErrorMsg.Append("运单号/订单号/原跟踪号不存在<br/>");
                        success = false;
                    }
                    else if (model.WayBillTrackingNumbers.FindAll(z => z.WayBillNumber.ToUpperInvariant() == p.WayBillNumber.ToUpperInvariant()).Count > 1)
                    {
                        p.ErrorMsg.Append("运单号/订单号/原跟踪号存在重复<br/>");
                        success = false;
                    }
                    if (success)
                    {
                        successTotal++;
                    }
                    total++;
                });
            model.Total = total;
            model.SuccessTotal = successTotal;
            model.FailureTotal = model.Total - model.SuccessTotal;
            if (model.WayBillTrackingNumbers.Any(p => !string.IsNullOrWhiteSpace(p.ErrorMsg.ToString())))
            {
                model.BtnSuccess = false;
            }
            return View(model);
        }
        [System.Web.Mvc.HttpPost]
        [FormValueRequired("btnSave")]
        public ActionResult NewTrackingNumber(WayBillListViewModel model)
        {
            var result = new ResponseResult();

            string filePath = _tempPathForExcel + model.UniqueExcelFileName;
            model.WayBillTrackingNumbers = GetWayBillTrackingNumber(filePath);
            List<WayBillInfo> wayBillInfos = new List<WayBillInfo>();
            if (model.WayBillTrackingNumbers.Count > 0)
            {
                model.WayBillTrackingNumbers.ForEach(p => wayBillInfos.Add(new WayBillInfo()
                            {
                                WayBillNumber = p.WayBillNumber.ToUpperInvariant(),
                                TrackingNumber = p.TrackingNumber.ToUpperInvariant(),
                            })
                    );
            }
            System.IO.File.Delete(filePath);
            try
            {

                //#region 操作日志记录
                ////yungchu
                ////敏感字段--跟踪号

                //if (wayBillInfos.Count > 0)
                //{
                //	var sbBuilder = new StringBuilder();
                //	sbBuilder.Append("");

                //	string wayBillNumber = string.Empty;
                //	string trackingNumber = string.Empty;
                //	foreach (var item in wayBillInfos)
                //	{
                //		if (_inStorageService.GetWayBillInfo(item.WayBillNumber) != null)
                //		{
                //			wayBillNumber = _inStorageService.GetWayBillInfo(item.WayBillNumber).WayBillNumber;
                //			trackingNumber = _inStorageService.GetWayBillInfo(item.WayBillNumber).TrackingNumber;
                //		}

                //		sbBuilder.AppendFormat(" 跟踪号从{0}更改为{1}", trackingNumber, item.TrackingNumber);

                //		string sa = _workContext.User.CustomerId.ToString();


                //		BizLog bizlog = new BizLog()
                //		{
                //			Summary = sbBuilder.ToString() != "" ? "[运单转单]" + sbBuilder : "运单转单",
                //			KeywordType = KeywordType.WayBillNumber,
                //			Keyword = wayBillNumber,//跟踪号、订单号都 统一显示运单号
                //			UserCode = _workContext.User.UserUame,
                //			UserRealName = _workContext.User.UserUame,
                //			UserType = UserType.LMS_User,
                //			SystemCode = SystemType.LMS,
                //			ModuleName = "运单转单"
                //		};

                //		_operateLogServices.WriteLog(bizlog, model);

                //	}

                //}

                //#endregion

                _orderService.ChangeWayBillTrackingNumber(wayBillInfos);

                result.Result = true;

                //SetViewMessage(ShowMessageType.Success, "保存成功", true);
            }
            catch (EntityCommandExecutionException)
            {
                result.Result = false;
                result.Message = "系统繁忙，请稍后重试";

                //SetViewMessage(ShowMessageType.Success, "系统繁忙，请稍后重试", true);
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.Message = "保存失败";

                //SetViewMessage(ShowMessageType.Error, "保存失败", true);
            }

            //return RedirectToAction("NewTrackingNumber");
            return Json(result);
        }

        [System.Web.Mvc.HttpPost]
        [FormValueRequired("btnCancel")]
        [System.Web.Mvc.ActionName("NewTrackingNumber")]
        public ActionResult CancelTrackingNumber()
        {
            return RedirectToAction("NewTrackingNumber");
        }

        #endregion

        #region 跟踪号管理

        //public ActionResult SelectTrackingNumber()
        //{
        //    SelectTrackingNumberModel model=new SelectTrackingNumberModel();
        //    var freightService = EngineContext.Current.Resolve<IFreightService>();
        //    var shippingMethodeList = freightService.GetShippingMethods(null, true);
        //    shippingMethodeList.ForEach(p => model.ShippingMethods.Add(new SelectListItem()
        //    {
        //        Text = p.FullName,
        //        Value = p.ShippingMethodId.ToString()
        //    }));
        //    return View(model);
        //}

        public ActionResult SelectTrackingNumber(TrackingNumberFilterModel filterModel)
        {
            return View(BindTrackingNumberModel(filterModel));
        }

        //[System.Web.Mvc.HttpPost]
        //[FormValueRequired("btnSelect")]
        //public ActionResult SelectTrackingNumber(SelectTrackingNumberModel model)
        //{
        //    var freightService = EngineContext.Current.Resolve<IFreightService>();
        //    var shippingMethodeList = freightService.GetShippingMethods(null, true);
        //    TrackingNumberParam param = new TrackingNumberParam();
        //    param.shippingMehtodId = model.ShippingMethodID;
        //    param.StartTime = model.StartTime;
        //    param.EndTime = model.EndTime;
        //    if (_orderService.GetTrackingNumberDetails(param) != null)
        //    {
        //        model.TrackingNumberDetaileds = _orderService.GetTrackingNumberDetails(param).ToModel<SelectTrackingNumberModel>().TrackingNumberDetaileds;
        //    }
        //    shippingMethodeList.ForEach(p => model.ShippingMethods.Add(new SelectListItem()
        //    {
        //        Text = p.FullName,
        //        Value = p.ShippingMethodId.ToString()
        //    }));

        //    return View(model);
        //}

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("SelectTrackingNumber")]
        [FormValueRequired("btnSelect")]
        public ActionResult SelectTrackingNumberPost(TrackingNumberFilterModel filterModel)
        {

            return View(BindTrackingNumberModel(filterModel));
        }

        private SelectTrackingNumberModel BindTrackingNumberModel(TrackingNumberFilterModel filterModel)
        {
            SelectTrackingNumberModel model = new SelectTrackingNumberModel()
                {
                    ShippingMethodID = filterModel.ShippingMethodId,
                    StartTime = filterModel.StartTime,
                    EndTime = filterModel.EndTime,
                    FilterModel = filterModel
                };
            var shippingMethodeList = _freightService.GetShippingMethods(null, true);
            TrackingNumberParam param = new TrackingNumberParam();
            param.shippingMehtodId = filterModel.ShippingMethodId;
            param.StartTime = filterModel.StartTime;
            param.EndTime = filterModel.EndTime;
            //if (_orderService.GetTrackingNumberDetails(param) != null)
            //{
            //    model.PagedList = _orderService.GetTrackingNumberExtList(param).ToModelAsCollection<TrackingNumberExt, TrackingNumberDetailed>(); 
            //    //model.TrackingNumberDetaileds = _orderService.GetTrackingNumberDetails(param).ToModel<SelectTrackingNumberModel>().TrackingNumberDetaileds;
            //}
            model.PagedList = _orderService.GetTrackingNumberExtList(param).ToModelAsCollection<TrackingNumberExt, TrackingNumberDetailed>();
            model.PagedList.ForEach(p =>
                {
                    var shippingMethodModel = shippingMethodeList.FirstOrDefault(s => s.ShippingMethodId == p.ShippingMethodId);
                    if (shippingMethodModel != null)
                        p.ShippingMethodName = shippingMethodModel.FullName;
                });
            shippingMethodeList.ForEach(p => model.ShippingMethods.Add(new SelectListItem()
            {
                Text = p.FullName,
                Value = p.ShippingMethodId.ToString()
            }));
            return model;
        }

        //[System.Web.Mvc.HttpPost]
        //[FormValueRequired("btnDisable")]
        //[System.Web.Mvc.ActionName("SelectTrackingNumber")]
        //public ActionResult DisableTrackingNumber(SelectTrackingNumberModel model)
        //{
        //    var freightService = EngineContext.Current.Resolve<IFreightService>();
        //    var shippingMethodeList = freightService.GetShippingMethods(null, true);
        //    TrackingNumberParam param = new TrackingNumberParam();
        //    int? shippingMethodID=null;
        //    DateTime? startTime = null;
        //    DateTime? endTime = null;
        //    string trackingNumberId = Request.Form["row.TrackingNumberId"];
        //    if (!string.IsNullOrWhiteSpace(Request.Form["ShippingMethodID"]))
        //    {
        //        shippingMethodID = Int32.Parse(Request.Form["ShippingMethodID"]);
        //    }
        //    if (!string.IsNullOrWhiteSpace(Request.Form["StartTime"]))
        //    {
        //        startTime = DateTime.Parse(Request.Form["StartTime"]);
        //    }
        //    if (!string.IsNullOrWhiteSpace(Request.Form["EndTime"]))
        //    {
        //        endTime = DateTime.Parse(Request.Form["EndTime"]);
        //    }
        //    if (_trackingNumberService.DisableTrackingNumber(trackingNumberId))
        //    {
        //        param.shippingMehtodId = shippingMethodID;
        //        param.StartTime = startTime;
        //        param.EndTime = endTime;
        //        //model.TrackingNumberDetaileds = _orderService.GetTrackingNumberDetails(param).ToModel<SelectTrackingNumberModel>().TrackingNumberDetaileds;

        //       SetViewMessage(ShowMessageType.Success, "禁用成功", true);
        //    }
        //    else
        //    {

        //        SetViewMessage(ShowMessageType.Success, "禁用失败", false);
        //    }
        //    shippingMethodeList.ForEach(p => model.ShippingMethods.Add(new SelectListItem()
        //    {
        //        Text = p.FullName,
        //        Value = p.ShippingMethodId.ToString()
        //    }));
        //    return View(model);
        //}

        [System.Web.Mvc.HttpPost]
        [FormValueRequired("btnDisable")]
        [System.Web.Mvc.ActionName("SelectTrackingNumber")]
        public ActionResult DisableTrackingNumber(TrackingNumberFilterModel filterModel)
        {
            SelectTrackingNumberModel model = new SelectTrackingNumberModel()
            {
                ShippingMethodID = filterModel.ShippingMethodId,
                StartTime = filterModel.StartTime,
                EndTime = filterModel.EndTime,
                FilterModel = filterModel
            };
            var shippingMethodeList = _freightService.GetShippingMethods(null, true);
            TrackingNumberParam param = new TrackingNumberParam();
            int? shippingMethodID = null;
            DateTime? startTime = null;
            DateTime? endTime = null;
            string trackingNumberId = Request.Form["row.TrackingNumberId"];

            shippingMethodID = filterModel.ShippingMethodId ?? 0;
            startTime = filterModel.StartTime;
            endTime = filterModel.EndTime;

            if (_trackingNumberService.DisableTrackingNumber(trackingNumberId))
            {
                param.shippingMehtodId = shippingMethodID;
                param.StartTime = startTime;
                param.EndTime = endTime;
                model.PagedList = _orderService.GetTrackingNumberExtList(param).ToModelAsCollection<TrackingNumberExt, TrackingNumberDetailed>();
                model.PagedList.ForEach(p =>
                {
                    var shippingMethodModel = shippingMethodeList.FirstOrDefault(s => s.ShippingMethodId == p.ShippingMethodId);
                    if (shippingMethodModel != null)
                        p.ShippingMethodName = shippingMethodModel.FullName;
                });
                SetViewMessage(ShowMessageType.Success, "禁用成功", true);
            }
            else
            {

                SetViewMessage(ShowMessageType.Success, "禁用失败", false);
            }
            shippingMethodeList.ForEach(p => model.ShippingMethods.Add(new SelectListItem()
            {
                Text = p.FullName,
                Value = p.ShippingMethodId.ToString()
            }));
            return View(model);
        }

        /// <summary>
        /// 跟踪号查看
        /// </summary>
        /// <param name="trackingNumberId"></param>
        /// <returns></returns>
        public ActionResult TrackingNumberDetail(string id, TrackingNumberModel model = null)
        {
            //TrackingNumberModel model=new TrackingNumberModel();
            int shippingMethodId = 0;
            if (id == null)
            {
                return View(model);
            }
            if (_trackingNumberService.GetTrackingNumberInfo(id) != null)
            {
                var trackingNumberInfo = _trackingNumberService.GetTrackingNumberInfo(id);
                model.ApplianceCountry = trackingNumberInfo.ApplianceCountry.Substring(0, trackingNumberInfo.ApplianceCountry.Length - 1);
                shippingMethodId = trackingNumberInfo.ShippingMethodID;
                model.TrackingNumberID = id;
                //List<ShippingMethodModel> shippingMethodModels = new List<ShippingMethodModel>();
                var freightService = EngineContext.Current.Resolve<IFreightService>();
                var shippingMethodModels = freightService.GetShippingMethods(null, true);
                model.ShippingMethodName = shippingMethodModels.Find(x => x.ShippingMethodId == shippingMethodId).FullName;
            }
            //model.uploadTrackingNumberDetailModels =
            //   _trackingNumberService.GetTrackingNumberDetailById(id).ToModelAsCollection<TrackingNumberDetailInfo, UploadTrackingNumberDetailModel>();
            model.PagedList = _trackingNumberService.GetTrackingNumberPagedList(model.Page, model.PageSize, model.TrackingNumberID).ToModelAsPageCollection<TrackingNumberDetailInfo, UploadTrackingNumberDetailModel>();
            return View(model);
        }
        #endregion

        public ActionResult CountryList(CountryFilterModel filterModel)
        {
            switch (filterModel.SelectedField)
            {
                case "CountryCode":
                    filterModel.CountryCode = filterModel.SeekValue;
                    break;
                case "ChineseName":
                    filterModel.ChineseName = filterModel.SeekValue;
                    break;
            }
            var pageData = _trackingNumberService.GetPagedList(new CountryParam
            {
                CountryCode = filterModel.CountryCode,
                ChineseName = filterModel.ChineseName,
                Page = filterModel.Page,
                PageSize = filterModel.PageSize
            });
            var model = new CountryListModel
            {
                CountryModels = pageData.ToModelAsPageCollection<CountryExt, CountryModel>(),
                FilterModel = filterModel
            };
            string chineseNames = string.Empty;
            if (null != filterModel.Codes)
            {
                var codelist = filterModel.Codes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries); //选中 
                foreach (var country in model.CountryModels.InnerList)
                {
                    if (codelist.Contains(country.CountryCode))
                    {
                        country.TypeCHK = 1;
                        chineseNames += country.ChineseName + ",";
                    }
                }
            }
            else
            {
                var trackingNumber = _trackingNumberService.GetTrackingNumberInfo(filterModel.TrackingNumberID);
                var countryList = trackingNumber != null ? trackingNumber.ApplianceCountry : "";
                var countryCodelArr = countryList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var countryDic =
                    countryCodelArr.Select(
                        t =>
                        t.Replace("[", "")
                         .Replace("]", "")
                         .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                   .Where(arr => arr.Length >= 2)
                                   .ToDictionary(arr => arr[0], arr => arr[1]);

                foreach (var country in model.CountryModels.InnerList)
                {
                    if (countryDic.ContainsKey(country.CountryCode))
                    {
                        country.TypeCHK = 1;
                        chineseNames += country.ChineseName + ",";
                    }
                }
            }
            filterModel.ChineseNameList = filterModel.ChineseNameList ?? chineseNames;
            return View(model);
        }

        public string SaveFile(HttpPostedFileBase file)
        {
            if (file != null)
            {
                try
                {
                    // 文件上传后的保存路径
                    string filePath = sysConfig.UploadPath;

                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    string datePath = DateTime.Now.ToString("yyyyMM");
                    string path = Path.Combine(filePath, datePath);

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fileName = Path.GetFileName(file.FileName);// 原始文件名称
                    string fileExtension = Path.GetExtension(fileName); // 文件扩展名
                    string saveName = Path.Combine(datePath, DateTime.Now.ToString("yyyyMMddHHmmssfff") + fileExtension); // 保存文件名称
                    string savePath = Path.Combine(filePath, saveName);


                    try
                    {
                        file.SaveAs(savePath);
                        return savePath;
                    }
                    catch (Exception e)
                    {
                        if (System.IO.File.Exists(savePath))
                        {
                            System.IO.File.Delete(savePath);
                        }
                        Log.Exception(e);
                        // return Json(new { Success = false, Message = e.Message }, JsonRequestBehavior.AllowGet);
                    }

                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
            }
            return "";
        }
        //Jess 2014-10-27 Npoi支持xlsx
        public List<UploadTrackingNumberDetailModel> GetTrackingNumberExcelList(string filePath)
        {
            DataTable dataTable = ExcelHelper.ReadToDataTable(filePath);
            return GetTrackingNumberExcel(dataTable);
        }
        private List<UploadTrackingNumberDetailModel> GetTrackingNumberExcel(DataTable dataTable)
        {
            List<UploadTrackingNumberDetailModel> lstReslut = new List<UploadTrackingNumberDetailModel>();
            if (dataTable == null || dataTable.Columns.Count < 1 || dataTable.Rows.Count < 1)
            {
                return lstReslut;
            }
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                var row = dataTable.Rows[i];
                string tracknb = row[0].ToString().Trim();
                string filterExp = dataTable.Columns[0].ColumnName.Trim() + "=" + "'" + tracknb + "'";
                int isRepeat = 0;
                if (dataTable.Select(filterExp).Length > 1)
                {
                    isRepeat = 1;
                }
                lstReslut.Add(new UploadTrackingNumberDetailModel
                    {
                        TrackingNumber = row[0].ToString().Trim(),
                        IsRepeat = isRepeat
                    });
            }
            return lstReslut;
        }
        //Jess 2014-10-27 先保存到服务器，然后再读起。这个支持xlsx
        //public List<WayBillTrackingNumber> GetWayBillTrackingNumber(HttpPostedFileBase file)
        //{
        //    DataTable dataTable = ExcelHelper.ReadToDataTable(file.InputStream);
        //    return GetWayBillTrackingNumber(dataTable);
        //}
        public List<WayBillTrackingNumber> GetWayBillTrackingNumber(string file)
        {
            DataTable dataTable = ExcelHelper.ReadToDataTable(file);
            return GetWayBillTrackingNumber(dataTable);
        }

        private List<WayBillTrackingNumber> GetWayBillTrackingNumber(DataTable dataTable)
        {
            List<WayBillTrackingNumber> lstReslut = new List<WayBillTrackingNumber>();
            if (dataTable == null || dataTable.Columns.Count < 1 || dataTable.Rows.Count < 1)
            {
                return lstReslut;
            }
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                var row = dataTable.Rows[i];
                if (!string.IsNullOrWhiteSpace(row[0].ToString()) && !string.IsNullOrWhiteSpace(row[0].ToString()))
                {
                    lstReslut.Add(new WayBillTrackingNumber
                        {
                            execlRow = i + 1,
                            WayBillNumber = row[0].ToString().Trim(),
                            TrackingNumber = row[1].ToString().Trim(),
                        });
                }
            }

            return lstReslut;
        }


        // 运单导出
        #region ExportWayBill
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("List")]
        [FormValueRequired("btnExport")]
        public ActionResult ExportWayBill(WayBillListViewModel param)
        {
            int MaxSubColum = 0;
            var models = new List<ExportWayBillModel>();
            var pm = new WayBillListExportParam()
                {
                    CountryCode = param.FilterModel.CountryCode,
                    CustomerCode = param.FilterModel.CustomerCode,
                    DateWhere = param.FilterModel.DateWhere,
                    EndTime = param.FilterModel.EndTime,
                    IsOutHold = param.FilterModel.IsHold,
                    SearchContext = param.FilterModel.SearchContext,
                    SearchWhere = param.FilterModel.SearchWhere,
                    ShippingMethodId = param.FilterModel.ShippingMethodId,
                    StartTime = param.FilterModel.StartTime,
                    Status = param.FilterModel.GetStatus
                };
            var applications = _orderService.GetApplicationInfoExport(pm);
            MaxSubColum = applications.GroupBy(i => i.WayBillNumber).Select(i => i.Count()).ToList().Max();

            //是否排除测试账号
            List<string> excludeCustomerCode=new List<string>();
            if (!param.FilterModel.ShowTestWaybill)
            {
                excludeCustomerCode = sysConfig.TestCustomerCode.Split(',').ToList();
            }

            _orderService.GetWayBillListExport(pm).AsParallel().ToList().ForEach(p =>
                    {
                        if(excludeCustomerCode.Contains(p.CustomerCode)) return;

                        var model = new ExportWayBillModel
                            {
                                CustomerOrderNumber = p.CustomerOrderNumber,
                                CustomerCode = p.CustomerCode,
                                Name = p.Name,
                                InsureAmount = p.InsureAmount,
                                PackageNumber = p.PackageNumber,
                                AppLicationType = CustomerOrder.GetApplicationTypeDescription(p.AppLicationType),
                                WayBillNumber = p.WayBillNumber,
                                InShippingMethodName = p.InShippingMethodName,
                                TrackingNumber = p.TrackingNumber,
                                Weight = p.Weight,
                                SettleWeight = p.SettleWeight,
                                Length = p.Length,
                                Width = p.Width,
                                Height = p.Height,
                                WayCreatedOn = p.CreatedOn.ToString(),
                                ShiCreatedOn = p.InStorageCreatedOn.ToString(),
                                SenCreatedOn = p.OutStorageCreatedOn.ToString(),
                                Status = WayBill.GetStatusDescription(p.Status),
                                EnableTariffPrepay = p.EnableTariffPrepay,
                                CountryCode = p.CountryCode,
                                ChineseName = p.ChineseName,
                                ShippingFirstName = p.ShippingFirstName,
                                ShippingLastName = p.ShippingLastName,
                                ShippingAddress = p.ShippingAddress + " " + p.ShippingAddress1 + " " + p.ShippingAddress2,
                                ShippingCity = p.ShippingCity,
                                ShippingState = p.ShippingState,
                                ShippingZip = p.ShippingZip,
                                ShippingPhone = p.ShippingPhone,
                                ShippingCompany = p.ShippingCompany,
                                ShippingTaxId = p.ShippingTaxId,
                                SenderFirstName = p.SenderFirstName,
                                SenderLastName = p.SenderLastName,
                                SenderCompany = p.SenderCompany,
                                SenderAddress = p.SenderAddress,
                                SenderCity = p.SenderCity,
                                SenderState = p.SenderState,
                                SenderZip = p.SenderZip,
                                SenderPhone = p.SenderPhone,
                                InsuredName = p.InsuredName,
                                SensitiveTypeName = p.SensitiveTypeName
                            };
                        applications.Where(a => a.WayBillNumber == p.WayBillNumber).AsParallel().ToList().ForEach(ap => model.ApplicationInfoModels.Add(new ApplicationInfoModel
                            {
                                ApplicationID = ap.ApplicationID,
                                ApplicationName = ap.ApplicationName,
                                CreatedBy = ap.CreatedBy,
                                CreatedOn = ap.CreatedOn,
                                CustomerOrderID = ap.CustomerOrderID,
                                HSCode = ap.HSCode,
                                IsDelete = ap.IsDelete,
                                LastUpdatedBy = ap.LastUpdatedBy,
                                LastUpdatedOn = ap.LastUpdatedOn,
                                PickingName = ap.PickingName,
                                ProductUrl = ap.ProductUrl,
                                Qty = ap.Qty,
                                Remark = ap.Remark,
                                Total = ap.Total,
                                UnitPrice = ap.UnitPrice,
                                UnitWeight = ap.UnitWeight,
                                WayBillNumber = ap.WayBillNumber
                            }));
                        models.Add(model);
                    });
            #region 老方法
            //List<string> wayBillNumbers = new List<string>();
            //var countryList = _countryService.GetCountryList("");
            //var customerList = _customerService.GetCustomerList("", false);
            //ExportWayBillModel model = new ExportWayBillModel();
            ////得到运单号

            //SelectWayBillList(param.FilterModel).WayBillInfoModels.ForEach(WayBillInfoModel =>
            //     {
            //         model = new ExportWayBillModel();
            //         CustomerOrderInfoModel customerOrderInfos = _orderService.GetCustomerOrderInfoById(WayBillInfoModel.CustomerOrderID).ToModel<CustomerOrderInfoModel>();
            //         if (customerOrderInfos != null)
            //         {
            //             model.CustomerOrderNumber = customerOrderInfos.CustomerOrderNumber;
            //             model.CustomerCode = customerOrderInfos.CustomerCode;
            //             if (customerList.FirstOrDefault(p => p.CustomerCode == model.CustomerCode) != null)
            //             {
            //                 model.Name = customerList.First(p => p.CustomerCode == model.CustomerCode).Name;
            //             }
            //             else
            //             {
            //                 model.Name = "";
            //             }
            //             model.InsureAmount = customerOrderInfos.InsureAmount;
            //             model.PackageNumber = customerOrderInfos.PackageNumber;
            //             model.AppLicationType = CustomerOrder.GetApplicationTypeDescription(customerOrderInfos.AppLicationType);
            //         }
            //         else
            //         {
            //             model.CustomerOrderNumber = "";
            //         }
            //         model.WayBillNumber = WayBillInfoModel.WayBillNumber;
            //         model.InShippingMethodName = WayBillInfoModel.InShippingMethodName;
            //         model.TrackingNumber = WayBillInfoModel.TrackingNumber;
            //         model.Weight = WayBillInfoModel.Weight;
            //         model.SettleWeight = WayBillInfoModel.SettleWeight;
            //         model.Length = WayBillInfoModel.Length;
            //         model.Width = WayBillInfoModel.Width;
            //         model.Height = WayBillInfoModel.Height;
            //         model.WayCreatedOn = WayBillInfoModel.CreatedOn.ToString();
            //         model.ShiCreatedOn = WayBillInfoModel.InStorageTime.ToString();
            //         model.SenCreatedOn = WayBillInfoModel.OutStorageTime.ToString();
            //         model.Status = WayBill.GetStatusDescription(WayBillInfoModel.Status);
            //         //是否关税预付
            //         model.EnableTariffPrepay = WayBillInfoModel.EnableTariffPrepay;

            //         ShippingInfoModel shippingInfo = _orderService.GetshippingInfoById(WayBillInfoModel.ShippingInfoID).ToModel<ShippingInfoModel>();
            //         SenderInfoModel senderInfo = _orderService.GetSenderInfoById(WayBillInfoModel.SenderInfoID).ToModel<SenderInfoModel>();
            //         if (shippingInfo != null)
            //         {
            //             model.CountryCode = shippingInfo.CountryCode;
            //             if (countryList.First(p => p.CountryCode == model.CountryCode) != null)
            //             {
            //                 model.ChineseName = countryList.First(p => p.CountryCode == model.CountryCode).ChineseName;
            //             }
            //             else
            //             {
            //                 model.ChineseName = "";
            //             }
            //             model.ShippingFirstName = shippingInfo.ShippingFirstName;
            //             model.ShippingLastName = shippingInfo.ShippingLastName;
            //             model.ShippingAddress = shippingInfo.ShippingAddress + " " + shippingInfo.ShippingAddress1 + " " + shippingInfo.ShippingAddress2;
            //             model.ShippingCity = shippingInfo.ShippingCity;
            //             model.ShippingState = shippingInfo.ShippingState;
            //             model.ShippingZip = shippingInfo.ShippingZip;
            //             model.ShippingPhone = shippingInfo.ShippingPhone;
            //             model.ShippingCompany = shippingInfo.ShippingCompany;
            //             model.ShippingTaxId = shippingInfo.ShippingTaxId;
            //         }
            //         else
            //         {
            //             model.CountryCode = "";
            //             model.ChineseName = "";
            //             model.ShippingFirstName = "";
            //             model.ShippingLastName = "";
            //             model.ShippingAddress = "";
            //             model.ShippingCity = "";
            //             model.ShippingState = "";
            //             model.ShippingZip = "";
            //             model.ShippingPhone = "";
            //             model.ShippingCompany = "";
            //             model.ShippingTaxId = "";
            //         }
            //         if (senderInfo != null)
            //         {
            //             model.SenderFirstName = senderInfo.SenderFirstName;
            //             model.SenderLastName = senderInfo.SenderLastName;
            //             model.SenderCompany = senderInfo.SenderCompany;
            //             model.SenderAddress = senderInfo.SenderAddress;
            //             model.SenderCity = senderInfo.SenderCity;
            //             model.SenderState = senderInfo.SenderState;
            //             model.SenderZip = senderInfo.SenderZip;
            //             model.SenderPhone = senderInfo.SenderPhone;
            //         }
            //         else
            //         {
            //             model.SenderFirstName = "";
            //             model.SenderLastName = "";
            //             model.SenderCompany = "";
            //             model.SenderAddress = "";
            //             model.SenderCity = "";
            //             model.SenderState = "";
            //             model.SenderZip = "";
            //             model.SenderPhone = "";
            //         }
            //         model.IsReturn = WayBillInfoModel.IsReturn;
            //         InsuredCalculationModel insuredCalculation = _orderService.GetInsuredCalculationById(WayBillInfoModel.InsuredID).ToModel<InsuredCalculationModel>();
            //         if (insuredCalculation != null)
            //         {
            //             model.InsuredName = insuredCalculation.InsuredName;
            //         }
            //         else
            //         {
            //             model.InsuredName = "";
            //         }
            //         CustomerOrderInfoModel customerOrderInfo = _orderService.GetCustomerOrderInfoById(WayBillInfoModel.CustomerOrderID).ToModel<CustomerOrderInfoModel>();
            //         SensitiveTypeInfoModel sensitiveTypeInfo = _orderService.GetSensitiveTypeInfoById(customerOrderInfo.SensitiveTypeID).ToModel<SensitiveTypeInfoModel>();
            //         if (customerOrderInfos != null && sensitiveTypeInfo != null)
            //         {
            //             model.SensitiveTypeName = sensitiveTypeInfo.SensitiveTypeName;
            //         }
            //         else
            //         {
            //             model.SensitiveTypeName = "";
            //         }


            //         List<ApplicationInfoModel> applicationInfoModels =
            //             _orderService.GetApplicationInfoByWayBillNumber(WayBillInfoModel.WayBillNumber)
            //                          .ToModelAsCollection<ApplicationInfo, ApplicationInfoModel>();
            //         model.ApplicationInfoModels = applicationInfoModels;
            //         if (applicationInfoModels != null)
            //         {
            //             if (applicationInfoModels.Count >= MaxSubColum)
            //             {
            //                 MaxSubColum = applicationInfoModels.Count;
            //             }
            //         }
            //         models.Add(model);
            //         wayBillNumbers.Add(WayBillInfoModel.WayBillNumber.ToString());
            //     }); 
            #endregion
            var lstName = new List<string>
                {
                    "客户订单号",
                    "客户代码",
                    "客户名称",
                    "运单号",
                    "入仓运输方式",
                    "跟踪号",
                    "国家简码",
                    "国家中文名",
                    "收件人姓",
                    "收件人名字",
                    "收件人公司",
                    "收货地址",
                    "城市",
                    "省/州",
                    "邮编",
                    "电话",
                    "创建时间",
                    "收货时间",
                    "发货时间",
                    "状态",
                    "收件人税号",
                    "发件人姓",
                    "发件人名",
                    "发件人公司",
                    "发件人地址",
                    "城市",
                    "省/州",
                    "发件人邮编",
                    "发件人电话",
                    "是否退回",
                    "保险类型",
                    "保险价值RMB",
                    "敏感货物",
                    "申报类型",
                    "件数",
                    "长cm",
                    "宽cm",
                    "高cm",
                    "称重重量kg",
                    "结算重量kg",
					"是否关税预付"
                };
            for (int i = 1; i <= MaxSubColum; i++)
            {
                lstName.Add("申报名称" + i);
                lstName.Add("申报中文名称" + i);
                lstName.Add("海关编码" + i);
                lstName.Add("数量" + i);
                lstName.Add("单价" + i + "(usd)");
                lstName.Add("净重量" + i + "(kg)");
                lstName.Add("销售链接" + i);
                lstName.Add("备注" + i);
            }
            //string fileName = sysConfig.ExcelTemplateWebPath + sysConfig.ExportWayBill;
            string fileName = Path.Combine(sysConfig.TemporaryPath, string.Format("WayBillInfo_{0}{1}.{2}", _workContext.User.UserUame, DateTime.Now.Ticks.ToString(),"xlsx"));
            ExportExcelByWeb.ListWayBillExcel(fileName, models, lstName,2);
            return View(ListDataBindSilm(param.FilterModel));
        }
        #endregion

        public ActionResult AbnormalWayBillList(AbnormalWayBillListFilterModel param)
        {
            if (!param.IsFirstIn.HasValue)
            {
                param.Status = WayBill.AbnormalStatusToValue(WayBill.AbnormalStatusEnum.NO);
            }

            param.IsFirstIn = false;

            return View(AbnormalWayBillListDataBind(param));
        }
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("AbnormalWayBillList")]
        [FormValueRequired("btnSearch")]

        public ActionResult SearachAbnormalWayBillList(AbnormalWayBillListViewModel param)
        {
            param.FilterModel.Page = 1;
            return View(AbnormalWayBillListDataBind(param.FilterModel));
        }
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("AbnormalWayBillList")]
        [FormValueRequired("btnToExcel")]
        public ActionResult AbnormalWayBillToExecl(AbnormalWayBillListViewModel filter)
        {
            var model = AbnormalWayBillListDataBind(filter.FilterModel);
            AbnormalWayBillParam param = new AbnormalWayBillParam();
            param.CountryCode = filter.FilterModel.CountryCode;
            param.CustomerCode = filter.FilterModel.CustomerCode;
            param.DateWhere = 1;
            param.EndTime = filter.FilterModel.EndTime;
            param.SearchWhere = filter.FilterModel.SearchWhere;
            param.SearchContext = filter.FilterModel.SearchContext;
            param.ShippingMethodId = filter.FilterModel.ShippingMethodId;
            param.Status = filter.FilterModel.Status;
            param.StartTime = filter.FilterModel.StartTime;
            model.List = _orderService.GetAbnormalWayBillList(param);
            model.List.ForEach(p =>
            {
                p.AbnormalTypeName = WayBill.GetAbnormalTypeDescription(p.OperateType);
            });
            var titleList = new List<string> { "WayBillNumber-运单号", "CustomerOrderNumber-客户订单号", "CustomerCode-客户代码", "AbnormalCreateOn-创建时间","AbnormalCreateBy-创建人",
                "TrackingNumber-跟踪号", "CountryCode-发货国家", "InShippingMethodName-运输方式", "AbnormalTypeName-异常类型","AbnormalDescription-异常说明"};
            ExportExcelByWeb.WriteToDownLoad(model.List, titleList, null);
            return View(model);
        }

        public ActionResult Detail(string WayBillNumber)
        {
            var model = new WayBillInfoModel();
            var wayBillInfo = _inStorageService.GetWayBillInfo(WayBillNumber);
            if (wayBillInfo != null)
            {
                model = wayBillInfo.ToModel<WayBillInfoModel>();
                model.CustomerCodeName = _customerService.GetCustomer(model.CustomerCode).Name;
                if (wayBillInfo.InStorageInfo != null)
                {
                    model.InStorageTime = wayBillInfo.InStorageInfo.CreatedOn;
                }
                if (wayBillInfo.OutStorageInfo != null)
                {
                    model.OutStorageTime = wayBillInfo.OutStorageInfo.CreatedOn;

                    model.VenderName = _freightService.GetVenderList(false).FirstOrDefault(s => s.VenderCode == wayBillInfo.OutStorageInfo.VenderCode).VenderName;
                }
            }
            return View(model);
        }


        public ActionResult InStorageList(InStorageFilterModel param)
        {
            return View(InStorageListDataBind(param));
        }
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("InStorageList")]
        [FormValueRequired("btnSearch")]
        public ActionResult SearchInStorageList(InStorageListViewModel param)
        {
            param.FilterModel.Page = 1;
            return View(InStorageListDataBind(param.FilterModel));
        }

        public ActionResult InStorageDetail(string InStorageId, string ErrorWayBillNumber = "")
        {
            HttpContext.Server.ScriptTimeout = 200 * 60;
            var model = new InStorageInfoModelDetailViewModel();
            if (!string.IsNullOrWhiteSpace(InStorageId))
            {
                var info = _inStorageService.GetInStorageInfo(InStorageId);
                model.InStorageInfoModel = new InStorageInfoModel()
                {
                    CreatedBy = info.CreatedBy,
                    CreatedOn = info.CreatedOn,
                    CustomerCode = info.CustomerCode,
                    Freight = info.Freight,
                    FuelCharge = info.FuelCharge,
                    InStorageID = info.InStorageID,
                    LastUpdatedBy = info.LastUpdatedBy,
                    LastUpdatedOn = info.LastUpdatedOn,
                    MaterialsFee = info.MaterialsFee,
                    PhysicalTotalWeight = info.PhysicalTotalWeight,
                    ReceivingClerk = info.ReceivingClerk,
                    Register = info.Register,
                    Remark = info.Remark,
                    //ShippingMethodName
                    Status = info.Status,
                    Surcharge = info.Surcharge,
                    TariffPrepayFee = info.TariffPrepayFee,
                    TotalFee = info.TotalFee,
                    TotalQty = info.TotalQty,
                    TotalWeight = info.TotalWeight,
                    WayBillInfos = new List<WayBillInfoModel>()
                };

                if (model.InStorageInfoModel != null && !string.IsNullOrWhiteSpace(model.InStorageInfoModel.CustomerCode))
                {
                    var customer = _customerService.GetCustomer(model.InStorageInfoModel.CustomerCode);
                    if (customer != null)
                    {
                        model.Customer.CustomerCode = customer.CustomerCode;
                        model.Customer.Balance = customer.CustomerBalance.Balance ?? 0;
                        model.Customer.Name = customer.Name;
                        model.Customer.PaymentTypeName = customer.PaymentType.PaymentName;
                        model.Customer.PaymentTypeID = customer.PaymentType.PaymentTypeID;
                    }
                }

                model.InStorageInfoModel.ShippingMethodName = _inStorageService.GetShippingMethodName(InStorageId);
                model.InStorageInfoModel.InStorageTotalModels = _inStorageService.GetInStorageTotals(InStorageId);

            }
            if (string.IsNullOrWhiteSpace(ErrorWayBillNumber))
            {
                SuccessNotification(ErrorWayBillNumber + "运单重量为零");
            }
            return View(model);
        }

        public ActionResult ExportInStorage(string InStorageId)
        {
            var model = new InStorageInfoModelDetailViewModel();
            if (!string.IsNullOrWhiteSpace(InStorageId))
            {
                model.InStorageInfoModel = _inStorageService.GetInStorageInfo(InStorageId).ToModel<InStorageInfoModel>();
            }
            if (model.InStorageInfoModel != null)
            {
                if (model.InStorageInfoModel.WayBillInfos.Count > 0)
                {
                    model.InStorageInfoModel.ShippingMethodName = model.InStorageInfoModel.WayBillInfos.First().InShippingName;
                    var countryList = _countryService.GetCountryList("");
                    var list = from w in model.InStorageInfoModel.WayBillInfos
                               group w by w.CountryCode
                                   into g
                                   select new
                                       {
                                           CountryCode = g.Key,
                                           PackCount = g.Count(),
                                           TotalWeight = g.Sum(w => w.SettleWeight)
                                       };
                    List<ExportList> exportLists = new List<ExportList>();
                    list.ToList().ForEach(p => exportLists.Add(new ExportList
                    {
                        Area = _freightService.GetCountryArea(model.InStorageInfoModel.WayBillInfos[0].InShippingMethodID.Value, p.CountryCode).First().AreaId,
                        CountryCode = p.CountryCode,
                        PackCount = p.PackCount,
                        TotalWeight = p.TotalWeight.Value,
                        CountryName = countryList.Find(c => c.CountryCode == p.CountryCode).ChineseName
                    }));
                    int n = 1;
                    foreach (var row in exportLists)
                    {
                        row.Index = n;
                        n++;
                    }
                    var name = new List<string>
                        {
                            "Index-序号",
                            "Area-分区",
                            "CountryCode-国家简码",
                            "CountryName-国家名称",
                            "TotalWeight-重量Kg",
                            "PackCount-件数"
                        };
                    string fileName = sysConfig.ExcelTemplateWebPath + sysConfig.ExportInStorageInfo;
                    ExportExcelByWeb.InStorageInfExcel(fileName, exportLists, name, model.InStorageInfoModel.InStorageID, model.InStorageInfoModel.ShippingMethodName, model.InStorageInfoModel.TotalWeight.Value, model.InStorageInfoModel.TotalQty.Value);
                }
            }
            return null;
        }

        public ActionResult OutStorageList(OutStorageFilterModel param)
        {
            return View(OutStorageListDataBind(param));
        }
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("OutStorageList")]
        [FormValueRequired("btnSearch")]
        public ActionResult SearchOutStorageList(OutStorageListViewModel param)
        {
            param.FilterModel.Page = 1;
            return View(OutStorageListDataBind(param.FilterModel));
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("OutStorageList")]
        [FormValueRequired("btnExport")]
        public ActionResult ExportOutStorageList(OutStorageListViewModel param)
        {
            int count = 0;
            param.FilterModel.Page = 1;
            OutStorageListParam outStorageListParam = new OutStorageListParam
                {
                    VenderCode = param.FilterModel.VenderCode,
                    OutStorageID = param.FilterModel.OutStorageID,
                    StartTime = param.FilterModel.OutStartDate,
                    EndTime = param.FilterModel.OutEndDate
                };
            if (null == _outStorageService.GetExportOutStorageInfo(outStorageListParam))
            {
                SetViewMessage(ShowMessageType.Error, "该查询条件没有对应的运单数据", true, false);
                return View(OutStorageListDataBind(param.FilterModel));
            }
            var modelList = _outStorageService.GetExportOutStorageInfo(outStorageListParam).ToList();

            modelList.ForEach(p =>
                {
                    var list = _orderService.GetApplicationInfoByWayBillNumber(p.WayBillNumber).ToList();
                    if (list.Count > count)
                    {
                        count = list.Count;
                    }
                    p.AppLicationTypeValue = CustomerOrder.GetApplicationTypeDescription(p.AppLicationType);
                    p.ApplicationInfos = list;
                });

            //var modelList = _outStorageService.GetExportOutStorageInfo(outStorageListParam).ToList();

            var titleList = new List<string>
                {
                    "CustomerOrderNumber-客户订单号",
                    "WayBillNumber-运单号",
                    "InShippingMethodName-入仓运输方式",
                    "OutShippingMethodName-出仓运输方式",
                    "VenderName-物流商",
                    "TrackingNumber-跟踪号",
                    "CountryName-国家（英文）",
                    "CountryChineseName-国家（中文）",
                    "ShippingFirstName-收件人姓",
                    "ShippingLastName-收件人名",
                    "ShippingCompany-收件人公司",
                    "ShippingAddress-收货地址",
                    "ShippingCity-城市",
                    "ShippingState-省/州",
                    "ShippingZip-邮编",
                    "ShippingPhone-电话",
                    "ShippingTaxId-收件人税号",
                    "SenderFirstName-发件人姓",
                    "SenderLastName-发件人名",
                    "SenderCompany-发件人公司",
                    "SenderAddress-发货地址",
                    "SenderCity-城市",
                    "SenderState-省/州",
                    "SenderZip-发件人邮编",
                    "SenderPhone-发件人电话",
                    "IsReturn-是否退回",
                    "InsuredName-保险类型",
                    "InsureAmount-保险价值RMB",
                    "SensitiveTypeName-敏感货品",
                    "AppLicationTypeValue-申报类型",
                    "PackageNumber-件数",
                    "Length-长cm",
                    "Width-宽cm",
                    "Height-高cm",
                    "Weight-称重重量kg",
                    "SettleWeight-结算重量kg"
                };
            for (int i = 1; i <= count; i++)
            {
                titleList.Add(string.Format("ApplicationName{0}-申报名称{0}", i));
                titleList.Add(string.Format("HSCode{0}-海关编码{0}", i));
                titleList.Add(string.Format("Qty{0}-数量{0}", i));
                titleList.Add(string.Format("UnitPrice{0}-单价{0}usd", i));
                titleList.Add(string.Format("UnitWeight{0}-净重量{0}kg", i));
                titleList.Add(string.Format("PickingName{0}-配货信息{0}", i));
                titleList.Add(string.Format("Remark{0}-备注{0}", i));
            }
            string fileName = sysConfig.ExcelTemplateWebPath + sysConfig.ExportWayBill;

            ExportExcelByWeb.ListExcel(fileName, modelList, titleList);

            return View(OutStorageListDataBind(param.FilterModel));
        }

        public ActionResult OutStorageDetail(string OutStorageId)
        {
            var model = new OutStorageInfoDetailViewModel();
            if (!string.IsNullOrWhiteSpace(OutStorageId))
            {
                model.OutStorageInfo = _outStorageService.GetOutStorageInfo(OutStorageId).ToModel<OutStorageInfoModel>();
                if (model.OutStorageInfo != null && !string.IsNullOrWhiteSpace(model.OutStorageInfo.VenderCode))
                {
                    if (model.OutStorageInfo.WayBillInfos.Count > 0)
                    {
                        model.ShippingMethodName = model.OutStorageInfo.WayBillInfos.First().OutShippingName;
                    }
                }
            }
            return View(model);
        }

        public ActionResult FilterOutShippingMethod(int venderId)
        {
            OutShippingMethodViewModel model = new OutShippingMethodViewModel();
            var outShippingMethods = _outStorageService.GetOutStorageShippingMethods(venderId);
            outShippingMethods.ForEach(p =>
                {
                    if (model.ShippingMethodList.FirstOrDefault(z => z.ShippingMethodId == p.OutShippingMethodId) == null)
                    {
                        ShippingMethodModel shippingMethodModel = new ShippingMethodModel();
                        shippingMethodModel.ShippingMethodName = p.OutShippingMethodName;
                        shippingMethodModel.ShippingMethodId = p.OutShippingMethodId;
                        model.ShippingMethodList.Add(shippingMethodModel);
                    }
                });
            return View(model);
        }

        public ActionResult InFeeInfoList(InFeeListFilterModel filter)
        {
            if (!filter.IsFistIn)
            {
                if (filter.StartTime == null)
                {
                    filter.StartTime = DateTime.Parse(DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd") + " " + "00:00");
                }
                if (filter.EndTime == null)
                {
                    filter.EndTime = DateTime.Parse(DateTime.Now.AddDays(+1).ToString("yyyy-MM-dd") + " " + "00:00");
                }
                filter.IsFistIn = true;
            }
            return View(InFeeInfoListDataBind(filter));
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("InFeeInfoList")]
        [FormValueRequired("btnSearch")]
        public ActionResult SearchInFeeInfoList(InFeeInfoListViewModel filter)
        {
            filter.PagedList.PageIndex = 1;
            return View(InFeeInfoListDataBind(filter.FilterModel));
        }
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("InFeeInfoList")]
        [FormValueRequired("btnToExcel")]
        public ActionResult InFeeInfoToExecl(InFeeInfoListViewModel filter)
        {
            var model = InFeeInfoListExport(filter.FilterModel);
            var titleList = new List<string> { "WayBillNumber-运单号", "CustomerOrderNumber-客户订单号", "CustomerCode-客户代码", "InDateTime-收货时间",
                "TrackingNumber-跟踪号", "ChineseName-发货国家", "InShippingName-运输方式", "SettleWeight-计费重量(kg)" ,"Weight-货物重量(kg)" ,
            "Freight-运费","Register-挂号费","FuelCharge-燃油费","Surcharge-附加费","TotalFee-总费用"};
            ExportExcelByWeb.WriteToDownLoad(model.List, titleList, null);
            return View(model);
        }

        public ActionResult OutFeeInfoList(OutFeeListFilterModel filter)
        {
            return View(OutFeeInfoListDataBind(filter));
        }
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("OutFeeInfoList")]
        [FormValueRequired("btnSearch")]
        public ActionResult SearchOutFeeInfoList(OutFeeInfoListViewModel filter)
        {
            filter.PagedList.PageIndex = 1;
            return View(OutFeeInfoListDataBind(filter.FilterModel));
        }
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("OutFeeInfoList")]
        [FormValueRequired("btnToExcel")]
        public ActionResult OutFeeInfoToExecl(OutFeeInfoListViewModel filter)
        {
            var model = OutFeeInfoListExport(filter.FilterModel);
            var titleList = new List<string> { "WayBillNumber-运单号", "CustomerOrderNumber-销售订单号", "VenderName-服务商", "OutDateTime-收货时间",
                "TrackingNumber-跟踪号", "ChineseName-发货国家", "OutShippingName-运输方式", "SettleWeight-计费重量(kg)" ,"Weight-货物重量(kg)" ,
            "Freight-运费","Register-挂号费","FuelCharge-燃油费","Surcharge-附加费","TotalFee-总费用","Remark-备注"};
            ExportExcelByWeb.WriteToDownLoad(model.List, titleList, null);
            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("OutFeeInfoList")]
        [FormValueRequired("btnUpdatePrice")]
        public ActionResult OutFeeInfoToUpdatePrice(OutFeeInfoListViewModel filter, List<string> WayBillNumbers)
        {
            if (WayBillNumbers != null && WayBillNumbers.Count > 0)
            {
                WayBillNumbers.ForEach(p =>
                    {
                        var result = _freightService.GetVenderShippingPrice(_outStorageService.GetVenderPriceModel(p));
                        if (result.CanShipping)
                        {
                            _outStorageService.UpdateVenderPrice(p, result);
                        }
                        else
                        {
                            _outStorageService.UpdateErrorRemark(p, result.Message);
                        }
                    });
                _outStorageService.UpdateOutStoragePrice(WayBillNumbers);
            }
            return View(OutFeeInfoListDataBind(filter.FilterModel));
        }

        /// <summary>
        /// 直接出仓
        /// </summary>
        /// <param name="WayBillNumbers"></param>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        [ButtonPermissionValidator(PermissionRecords.FastOutStorageCode)]
        public ActionResult FastOutStorage(string WayBillNumbers, string ReturnUrl)
        {


            var model = new FastOutStorageViewModel();
            if (!string.IsNullOrWhiteSpace(WayBillNumbers))
            {
                model.WayBillNumbers = WayBillNumbers;
            }
            else
            {
                model.ErrorMessage = "没有直接出仓的运单号";
            }
            model.ReturnUrl = !string.IsNullOrWhiteSpace(ReturnUrl) ? ReturnUrl : "";
            _inStorageService.GetGoodsTypeList(false).ForEach(g => model.GoodsTypeModels.Add(new SelectListItem()
            {
                Text = g.GoodsTypeName,
                Value = g.GoodsTypeID.ToString()
            }));
            return View(model);
        }


        #region 运单模板
        public ActionResult WayBillTemplateList(WayBillTemplateListFilterModel filter)
        {
            return View(WayBillTemplateListDataBind(filter));
        }
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("WayBillTemplateList")]
        [FormValueRequired("btnSearch")]
        public ActionResult SearchWayBillTemplateList(WayBillTemplateListFilterModel filter)
        {
            return View(WayBillTemplateListDataBind(filter));
        }



        private WayBillTemplateListViewModel WayBillTemplateListDataBind(WayBillTemplateListFilterModel filter)
        {
            WayBillTemplateListParam param = new WayBillTemplateListParam()
            {
                Page = filter.Page,
                PageSize = filter.PageSize,
                ShippingMethodId = filter.ShippingMethodId,
            };
            var viewModel = new WayBillTemplateListViewModel();
            viewModel.FilterModel = filter;
            viewModel.ShippingMethods = GetShippingMethodSelectList(filter.ShippingMethodId);
            viewModel.PagedList = _wayBillTemplateService.GetWayBillTemplatePagedList(param).ToModelAsPageCollection<WayBillTemplate, WayBillTemplateModel>();

            viewModel.PagedList.InnerList.ForEach(p =>
                {
                    var itme =
                        viewModel.ShippingMethods.FirstOrDefault(s => s.Value.Equals(p.ShippingMethodId.ToString()));
                    p.ShippingMethodName =
                        itme != null ? itme.Text : "";
                    p.TemplateTypeName = _dictionaryTypeService.GetName(p.TemplateTypeId);
                });

            return viewModel;
        }



        public ActionResult WayBillTemplatePreview(int wayBillTemplateId)
        {
            var model = _wayBillTemplateService.GetWayBillTemplate(wayBillTemplateId).ToModel<WayBillTemplateModel>();
            return View(model);
        }


        public ActionResult WayBillTemplateSave(int wayBillTemplateId = 0)
        {
            var viewModel = new WayBillTemplateViewModel();
            if (wayBillTemplateId > 0)
            {
                viewModel.FilterModel = _wayBillTemplateService.GetWayBillTemplate(wayBillTemplateId).ToModel<WayBillTemplateModel>();
            }
            viewModel.FilterModel.WayBillTemplateId = wayBillTemplateId;
            return View(BindWayBillTemplateSave(viewModel.FilterModel));
        }

        public JsonResult DisableWayBillTemplate(int wayBillTemplateId)
        {
            var model = new ResponseResult();
            try
            {
                var wayBillTemplate = _wayBillTemplateService.GetWayBillTemplate(wayBillTemplateId);
                if (wayBillTemplate.Status == (int)WayBillTemplateInfo.StatusEnum.Enable)
                {
                    wayBillTemplate.Status = (int)WayBillTemplateInfo.StatusEnum.Disable;
                    model.Message = "禁用";
                }
                else
                {
                    wayBillTemplate.Status = (int)WayBillTemplateInfo.StatusEnum.Enable;
                    model.Message = "启用";
                }


                _wayBillTemplateService.UpdateWayBillTemplate(wayBillTemplate);
                model.Result = true;

                //#region 操作日志
                ////yungchu
                ////敏感字-无
                //BizLog bizlog = new BizLog()
                //{
                //	Summary = "运单模板启用",
                //	KeywordType = KeywordType.Lms_WayBillTemplateId,
                //	Keyword = wayBillTemplateId.ToString(),
                //	UserCode = _workContext.User.UserUame,
                //	UserRealName = _workContext.User.UserUame,
                //	UserType = UserType.LMS_User,
                //	SystemCode = SystemType.LMS,
                //	ModuleName = "运单模板管理"
                //};

                //_operateLogServices.WriteLog(bizlog, wayBillTemplate);
                //#endregion

            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                model.Result = false;
                model.Message = ex.Message;
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpPost]
        [ValidateInput(false)]
        public ActionResult WayBillTemplateSave(WayBillTemplateModel filterModel)
        {
            if (!ModelState.IsValid)
            {
                return View(BindWayBillTemplateSave(filterModel));
            }
            try
            {
                if (filterModel.WayBillTemplateId > 0)
                {

                    _wayBillTemplateService.UpdateWayBillTemplate(filterModel.ToEntity<WayBillTemplate>());

                    //#region  操作日志
                    ////yungchu
                    ////敏感字--无
                    //var bizlog = new BizLog()
                    //{
                    //	Summary = "运单模板编辑",
                    //	KeywordType = KeywordType.Lms_WayBillTemplateId,
                    //	Keyword = filterModel.WayBillTemplateId.ToString(),
                    //	UserCode = _workContext.User.UserUame,
                    //	UserRealName = _workContext.User.UserUame,
                    //	UserType = UserType.LMS_User,
                    //	SystemCode = SystemType.LMS,
                    //	ModuleName = "运单模板管理"
                    //};

                    //_operateLogServices.WriteLog(bizlog, filterModel);

                    //#endregion

                }
                else
                {
                    _wayBillTemplateService.AddWayBillTemplate(filterModel.ToEntity<WayBillTemplate>());
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                SetViewMessage(ShowMessageType.Error, ex.Message);
                return View(BindWayBillTemplateSave(filterModel));

            }
            SetViewMessage(ShowMessageType.Success, "提交成功");
            return RedirectToAction("WayBillTemplateList");
        }

        private WayBillTemplateViewModel BindWayBillTemplateSave(WayBillTemplateModel filterModel)
        {
            int status = WayBillTemplateInfo.StatusToValue(WayBillTemplateInfo.StatusEnum.Enable);
            var viewModel = new WayBillTemplateViewModel()
                {
                    FilterModel = filterModel,
                    ShippingMethods = GetShippingMethodSelectList(filterModel.ShippingMethodId),
                    WayBillTemplateTypes = GetDictionaryTypeList(DictionaryTypeInfo.WayBillTemplateType, filterModel.TemplateTypeId),

                };
            WayBillTemplateInfo.GetStatusList()
                               .ForEach(
                                   p =>
                                   viewModel.WayBillTemplateStatus.Add(new SelectListItem()
                                       {
                                           Text = p.TextField,
                                           Value = p.ValueField,
                                           Selected = p.ValueField == filterModel.Status.ToString()
                                       }));
            _wayBillTemplateInfoRepository.GetList(p => p.Status == status).ForEach(p =>
                {
                    if (p.TemplateType ==
                        WayBillTemplateInfo.TemplateTypeToValue(WayBillTemplateInfo.TemplateTypeEnum.Head))
                    {
                        viewModel.WayBillTemplateHead.Add(new SelectListItem()
                            {
                                Text = p.TemplateName,
                                Value = p.TemplateModelId.ToString()
                            });
                    }
                    else if (p.TemplateType ==
                             WayBillTemplateInfo.TemplateTypeToValue(WayBillTemplateInfo.TemplateTypeEnum.Body))
                    {
                        viewModel.WayBillTemplateBody.Add(new SelectListItem()
                            {
                                Text = p.TemplateName,
                                Value = p.TemplateModelId.ToString()
                            });
                    }
                });
            viewModel.WayBillTemplateHead.Insert(0, new SelectListItem { Text = "请选择", Value = "" });
            viewModel.WayBillTemplateBody.Insert(0, new SelectListItem { Text = "请选择", Value = "" });
            return viewModel;
        }

        /// <summary>
        /// 新增模板列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult WayBillTemplateInfoList(WayBillTemplateInfoParam param)
        {
            WayBillTemplateInfoViewModel model = new WayBillTemplateInfoViewModel();
            model.StatusList = GetStatusList();
            model.TemplateTypeList = GetTemplateTypeList();
            model.WayBillTemplateInfoList = _wayBillTemplateService.GetWayBillTemplateInfoList(param).ToModelAsPageCollection<WayBillTemplateInfo, WayBillTemplateInfoModel>();
            model.TemplateInfoParam = param;
            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        [FormValueRequired("SelectTempla")]
        public ActionResult WayBillTemplateInfoList(WayBillTemplateInfoViewModel model)
        {
            model.StatusList = GetStatusList();
            model.TemplateTypeList = GetTemplateTypeList();
            model.WayBillTemplateInfoList = _wayBillTemplateService.GetWayBillTemplateInfoList(model.TemplateInfoParam).ToModelAsPageCollection<WayBillTemplateInfo, WayBillTemplateInfoModel>();
            return View(model);
        }

        /// <summary> 添加新模板
        /// 添加新模板
        /// Add bu zhengsong
        /// </summary>
        /// <returns></returns>
        public ActionResult AddWayBillTemplate(int templateModelId)
        {
            WayBillTemplateInfoViewModel model = new WayBillTemplateInfoViewModel();
            List<SelectListItem> typeList = new List<SelectListItem>();
            WayBillTemplateInfo.GetTemplateTypeList().Each(p => typeList.Add(new SelectListItem()
            {
                Text = p.TextField,
                Value = p.ValueField
            }));
            model.TemplateTypeList = typeList;
            List<SelectListItem> statusList = new List<SelectListItem>();
            WayBillTemplateInfo.GetStatusList().Each(p => statusList.Add(new SelectListItem()
            {
                Text = p.TextField,
                Value = p.ValueField
            }));
            model.StatusList = statusList;
            if (templateModelId != 0)
            {
                model.WayBillTemplateInfo = _wayBillTemplateService.GetWayBillTemplateInfoByID(templateModelId).ToModel<WayBillTemplateInfoModel>();
            }
            return View(model);
        }


        /// <summary> 新增模板
        /// Add by zhengsong
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        [FormValueRequired("SaveTemplate")]
        [ValidateInput(false)]
        public ActionResult AddWayBillTemplate(WayBillTemplateInfoViewModel model)
        {
            WayBillTemplateInfoViewModel viewModel = new WayBillTemplateInfoViewModel();
            List<SelectListItem> typeList = new List<SelectListItem>();
            WayBillTemplateInfo.GetTemplateTypeList().Each(p => typeList.Add(new SelectListItem()
            {
                Text = p.TextField,
                Value = p.ValueField
            }));
            viewModel.TemplateTypeList = typeList;

            List<SelectListItem> statusList = new List<SelectListItem>();
            WayBillTemplateInfo.GetStatusList().Each(p => statusList.Add(new SelectListItem()
            {
                Text = p.TextField,
                Value = p.ValueField
            }));
            viewModel.StatusList = statusList;
            if (!ModelState.IsValid)
            {
                viewModel.WayBillTemplateInfo = model.WayBillTemplateInfo;
                return View(viewModel);
            }
            else
            {
                int status = WayBillTemplateInfo.StatusToValue(WayBillTemplateInfo.StatusEnum.Enable);
                if (model.WayBillTemplateInfo.Status == status &&
                    _wayBillTemplateInfoRepository.GetList(
                        p =>
                        p.TemplateName == model.WayBillTemplateInfo.TemplateName.Trim() &&
                        p.Status == status &&
                        p.TemplateType == model.WayBillTemplateInfo.TemplateType &&
                        p.TemplateModelId != model.WayBillTemplateInfo.TemplateModelId).Count > 0)
                {
                    SetViewMessage(ShowMessageType.Error, "模板名称重复", true);
                    viewModel.WayBillTemplateInfo = model.WayBillTemplateInfo;
                    return View(viewModel);
                }
                WayBillTemplateInfo wayBillTemplateInfo = new WayBillTemplateInfo();
                wayBillTemplateInfo.TemplateModelId = model.WayBillTemplateInfo.TemplateModelId;
                wayBillTemplateInfo.TemplateType = model.WayBillTemplateInfo.TemplateType;
                wayBillTemplateInfo.TemplateName = model.WayBillTemplateInfo.TemplateName.Trim();
                wayBillTemplateInfo.TemplateContent = model.WayBillTemplateInfo.TemplateContent;
                wayBillTemplateInfo.Status = model.WayBillTemplateInfo.Status;
                wayBillTemplateInfo.Remarks = model.WayBillTemplateInfo.Remarks;
                wayBillTemplateInfo.CreatedBy = _workContext.User.UserUame;
                wayBillTemplateInfo.CreatedOn = DateTime.Now;
                wayBillTemplateInfo.LastUpdatedBy = _workContext.User.UserUame;
                wayBillTemplateInfo.LastUpdatedOn = DateTime.Now;
                if (_wayBillTemplateService.AddWayBillTemplateInfo(wayBillTemplateInfo))
                {
                    SetViewMessage(ShowMessageType.Success, "保存成功", false);
                    return View(viewModel);
                    //return RedirectToAction("AddWayBillTemplate", new { templateModelId = 0 });
                }
                else
                {
                    SetViewMessage(ShowMessageType.Error, "保存失败", true);
                    viewModel.WayBillTemplateInfo = model.WayBillTemplateInfo;
                    return View(viewModel);
                }
            }
        }

        /// <summary> 启用禁用模板
        ///  Add by zhengsong
        /// </summary>
        /// <param name="templateModelId"></param>
        /// <returns></returns>
        public JsonResult EdidWayBillTemplateStatus(int templateModelId)
        {
            var model = new ResponseResult();
            model.Message = "操作失败";
            model.Result = false;
            int status = WayBillTemplateInfo.StatusToValue(WayBillTemplateInfo.StatusEnum.Enable);
            WayBillTemplateInfo wayBillTemplateInfo = new WayBillTemplateInfo();
            wayBillTemplateInfo = _wayBillTemplateInfoRepository.Get(templateModelId);
            if (wayBillTemplateInfo.Status == WayBillTemplateInfo.StatusToValue(WayBillTemplateInfo.StatusEnum.Enable))
            {
                wayBillTemplateInfo.Status = WayBillTemplateInfo.StatusToValue(WayBillTemplateInfo.StatusEnum.Disable);
                model.Message = "禁用";
            }
            else
            {
                if (_wayBillTemplateInfoRepository.GetList(
                        p =>
                        p.TemplateName == wayBillTemplateInfo.TemplateName &&
                        p.Status == status &&
                        p.TemplateType == wayBillTemplateInfo.TemplateType &&
                        p.TemplateModelId != wayBillTemplateInfo.TemplateModelId).Count > 0)
                {
                    model.Message = "存在相同的模板已启用";
                    model.Result = false;
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
                wayBillTemplateInfo.Status = WayBillTemplateInfo.StatusToValue(WayBillTemplateInfo.StatusEnum.Enable);
                model.Message = "启用";
            }
            if (_wayBillTemplateService.AddWayBillTemplateInfo(wayBillTemplateInfo))
            {
                RedirectToAction("AddWayBillTemplate", new { templateModelId = 0 });
                model.Result = true;
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary> 模板组成
        /// Add by zhengsong
        /// </summary>
        /// <param name="templateModelId"></param>
        /// <returns></returns>
        public JsonResult SelectWayBillTemplateModel(int templateModelId)
        {
            var model = new ResponseResult();
            model.Result = false;
            try
            {
                if (_wayBillTemplateInfoRepository.Get(templateModelId) != null
                    && _wayBillTemplateInfoRepository.Get(templateModelId).Status == WayBillTemplateInfo.StatusToValue(WayBillTemplateInfo.StatusEnum.Enable))
                {
                    model.Message = _wayBillTemplateInfoRepository.Get(templateModelId).TemplateContent;
                    model.Result = true;
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
                model.Result = false;
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult WayBillTemplateInfoPreview(int templateModelId)
        {
            WayBillTemplateInfoModel model = new WayBillTemplateInfoModel();
            model = _wayBillTemplateInfoRepository.Get(templateModelId).ToModel<WayBillTemplateInfoModel>();
            return View(model);
        }

        #endregion

        [System.Web.Mvc.HttpPost]
        public ActionResult SaveExpressPrintWayBillListSearchContext(string searchContext)
        {
            Session["ExpressPrintWayBill_list_SearchContext"] = searchContext.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var result = new ResponseResult()
            {
                Result = true,
            };
            return Json(result);
        }

        /// <summary>
        /// 物流快递打单
        /// Add by zhengsong
        /// </summary>
        /// <returns></returns>
        public ActionResult ExpressPrintWayBillList(ExpressPrintWayBillParam param = null)
        {
            if (param != null&&string.IsNullOrWhiteSpace(param.SearchContext))
            {
                string[] searchContextlines = Session["ExpressPrintWayBill_list_SearchContext"] as string[];

                if (searchContextlines != null && searchContextlines.Any())
                {
                    Session["ExpressPrintWayBill_list_SearchContext"] = null;
                    param.SearchContext = string.Join(Environment.NewLine, searchContextlines);
                }
            }
            return View(BindList(param));
        }

        /// <summary>
        /// 快递打单--绑定线下发货渠道跟踪号--详细页
        /// add by yungchu
        /// </summary>
        /// <param name="WayBillNumber"></param>
        /// <returns></returns>
        public ActionResult WayBillInfosDetail(string WayBillNumber, string ReturnUrl)
        {

            string searchWayBillNumber = string.Empty;

            try
            {   //搜索值
                searchWayBillNumber = Request.Form["searchNoTxt"].ToString();
            }
            catch (Exception)
            {
                searchWayBillNumber = WayBillNumber;
            }

            if (!searchWayBillNumber.IsNullOrEmpty())
            {
                WayBillNumber = searchWayBillNumber;
            }

            var model = new WayBillInfoModel();

            var wayBillInfo = _inStorageService.GetWayBillInfo(WayBillNumber);
            if (wayBillInfo != null)
            {
                model = wayBillInfo.ToModel<WayBillInfoModel>();
                model.CustomerCodeName = _customerService.GetCustomer(model.CustomerCode).Name;
                if (wayBillInfo.InStorageInfo != null)
                {
                    model.InStorageTime = wayBillInfo.InStorageInfo.CreatedOn;
                }
                if (wayBillInfo.OutStorageInfo != null)
                {
                    model.OutStorageTime = wayBillInfo.OutStorageInfo.CreatedOn;

                    model.VenderName = _freightService.GetVenderList(false).FirstOrDefault(s => s.VenderCode == wayBillInfo.OutStorageInfo.VenderCode).VenderName;
                }
            }

            if (!string.IsNullOrEmpty(WayBillNumber))
            {
                model.wayNumber = WayBillNumber;

            }
            //取发货渠道名
            if (!string.IsNullOrEmpty(wayBillInfo.OutShippingMethodID.ToString()))
            {
                model.SendGoodsChannel = wayBillInfo.OutShippingMethodID.ToString();

                List<Data.Entity.ShippingMethodModel> listShippingMethod = _freightService.GetShippingMethodsByIds(new List<int> { wayBillInfo.OutShippingMethodID.Value });

                foreach (var shippingMethodModel in listShippingMethod)
                {
                    model.SendGoodsChannelFullName = shippingMethodModel.FullName;
                }
            }


            if (!string.IsNullOrEmpty(wayBillInfo.VenderCode))
            {
                model.SendGoodsVender = wayBillInfo.VenderCode;
                model.SendGoodsVenderName = _freightService.GetVenderList(false).FirstOrDefault(s => s.VenderCode.Contains(wayBillInfo.VenderCode)).VenderName;
            }

            //返回路径
            model.ReturnUrl = ReturnUrl;

            return View(model);
        }


        /// <summary>
        /// 快递打单--现下发货渠道详细页--检测搜索是否为空
        /// add by yungchu
        /// </summary>
        /// <param name="WayBillNumber"></param>
        /// <returns></returns>
        public JsonResult CheckSearchResult(string wayBillNumber)
        {
            var responseResult = new ResponseResult();
            var wayBillInfo = _inStorageService.GetWayBillInfo(wayBillNumber);
            if (wayBillInfo == null)
            {
                responseResult.Result = false;
                responseResult.Message = "查询不存在,请重新输入！";
            }
            else
            {
                responseResult.Result = true;
            }
            return Json(responseResult, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 保存运单记录，修改其信息
        /// add by yungchu
        /// </summary>
        /// <param name="WayBillNumber"></param>
        /// <param name="ShippingMethodId"></param>
        /// <param name="TrackingNumber"></param>
        /// <param name="ShippingMethodName"></param>
        /// <returns></returns>
        /// 
        public JsonResult SaveUpdateWayBillInfo(string WayBillNumber, int ShippingMethodId, string TrackingNumber, string VenderName, string RawTrackingNumber, string CustomerCodeName, string CustomerOrderNumber)
        {

            int result = 3;//默认正确
            var responseResult = new ResponseResult();
            ExpressPrintWayBillViewModel model = new ExpressPrintWayBillViewModel();
            Data.Entity.ShippingMethodModel shippingMethodModel = new Data.Entity.ShippingMethodModel();

            WayBillInfo getWayBillInfo = _inStorageService.GetWayBillInfo(WayBillNumber);

            //判断隐藏跟踪单号--保存不显示跟踪号
            shippingMethodModel = _freightService.GetShippingMethod(getWayBillInfo.InShippingMethodID.Value);
            if (shippingMethodModel.IsHideTrackingNumber)
            {
                responseResult.TrackingNumber = "1";
            }


            try
            {
                //日志打印
                model.WayBillPrintLogLists =
                    _inStorageService.GetWayBillPrintLogList("").ToModelAsCollection<WayBillPrintLog, WayBillPrintLogModel>();


                if (_orderService.IsExitTrackingNumber(TrackingNumber, WayBillNumber))
                {
                    responseResult.Message = "已存在相同的跟踪号，请重新输入！";
                    responseResult.Result = false;
                    result = 1;
                }


                if (model.WayBillPrintLogLists.Any(a => a.NewTrackNumber.Contains(TrackingNumber)) &&
                    model.WayBillPrintLogLists.Any(a => a.SendGoodsVender.Contains(VenderName.ToString())) &&
                    model.WayBillPrintLogLists.Any(a => a.SendGoodsChannel.Contains(ShippingMethodId.ToString())) && getWayBillInfo.OutShippingMethodID == ShippingMethodId)
                {
                    responseResult.Message = "未作任何修改，请重新输入！";
                    responseResult.Result = false;
                    result = 2;
                }


                else if (result == 3)
                {
                    //修改运单信息
                    _orderService.UpdateWayBillInfos(new WayBillInfo()
                    {
                        WayBillNumber = WayBillNumber,
                        TrackingNumber = TrackingNumber,
                        VenderCode = VenderName.ToString(),
                        OutShippingMethodID = ShippingMethodId,
                        RawTrackingNumber = RawTrackingNumber
                    });


                    //增加打印日志
                    bool isAdd = _inStorageService.AddWayBillPrintLog(
                        new WayBillPrintLog()
                        {
                            waybillnumber = WayBillNumber,
                            sendGoodsVender = VenderName.ToString(),
                            sendGoodsChannel = ShippingMethodId.ToString(),
                            newTrackNumber = TrackingNumber,
                            printPerson = CustomerCodeName,
                            printDate = DateTime.Now

                        });
                    if (isAdd)
                    {
                        responseResult.Message = "保存成功！";
                        responseResult.Result = true;
                    }
                    else
                    {
                        responseResult.Message = "保存失败！";
                        responseResult.Result = false;
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                responseResult.Result = false;
                responseResult.Message = "保存失败！";
                responseResult.Message = ex.Message;
            }

            return Json(responseResult, JsonRequestBehavior.AllowGet);
        }




        /// <summary>
        /// 检查DHL运单是否有不能打印的
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        public JsonResult CheckDHLWayBill(string wayBillNumber)
        {
            var model = new ResponseResult();
            string[] wayBillNumbers = wayBillNumber.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var list = _inStorageService.GetWayBillByWayBillNumbers(wayBillNumbers).ToList();
            list.ForEach(p =>
            {
                if (p.ExpressRespons == null)
                {
                    model.Message += "{0},".FormatWith(p.WayBillNumber);
                }
            });
            if (string.IsNullOrWhiteSpace(model.Message))
            {
                model.Result = true;
            }
            else
            {
                model.Result = false;
                model.Message = "运单号为{0}没有对应的数据,不能打印".FormatWith(model.Message);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DHLPrintPreview(string wayBillNumber)
        {
            string[] wayBillNumbers = wayBillNumber.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            DHLPrintViewModel viewModel = new DHLPrintViewModel()
            {
                ExpressAccountInfos = _expressService.GetExpressAccountInfos().ToList(),
                WayBillInfos = _inStorageService.GetWayBillByWayBillNumbers(wayBillNumbers).ToList().FindAll(p => p.ExpressRespons != null).OrderBy(p => p.CustomerOrderID).ToList()
            };
            //viewModel.WayBillInfos.ForEach(p =>
            //    {
            //        var country = _countryService.GetCountryList("").Single(c => c.CountryCode == p.ShippingInfo.CountryCode);
            //        p.ShippingInfo.ShippingState = country.Name;
            //    });



            //foreach (var item in viewModel.WayBillInfos)
            //{
            //	#region 操作日志
            //	//yungchu
            //	//敏感字-无
            //	BizLog bizlog = new BizLog()
            //	{
            //		Summary = "打印运单",
            //		KeywordType = KeywordType.WayBillNumber,
            //		Keyword = item.WayBillNumber,
            //		UserCode = _workContext.User.UserUame,
            //		UserRealName = _workContext.User.UserUame,
            //		UserType = UserType.LMS_User,
            //		SystemCode = SystemType.LMS,
            //		ModuleName = "打印运单"
            //	};

            //	_operateLogServices.WriteLog(bizlog, viewModel.ExpressAccountInfos);
            //	#endregion
            //}

            var shippingMethods = new List<int>();
            foreach (var wayBillInfo in viewModel.WayBillInfos)
            {
                if (!shippingMethods.Contains(wayBillInfo.OutShippingMethodID.Value))
                {
                    shippingMethods.Add(wayBillInfo.OutShippingMethodID.Value);
                }
            }

            viewModel.ShippingMethodCodes = _freightService.GetShippingMethodsByIds(shippingMethods).ToDictionary(p => p.ShippingMethodId, p => p.Code);


            //打印运单记录到打印日志表
            var getWaybillInfo = _orderService.GetWayBillInfos(wayBillNumbers).ToList().FindAll(p => p.WayBillNumber != null);
            foreach (var wayBillInfo in getWaybillInfo)
            {

                if (!string.IsNullOrWhiteSpace(wayBillInfo.VenderCode) && wayBillInfo.OutShippingMethodID != null &&
                    !string.IsNullOrWhiteSpace(wayBillInfo.TrackingNumber))
                {
                    try
                    {
                        //增加打印日志
                        bool isAdd = _inStorageService.AddWayBillPrintLog(
                        new WayBillPrintLog()
                        {
                            waybillnumber = wayBillInfo.WayBillNumber,
                            sendGoodsVender = wayBillInfo.VenderCode,
                            sendGoodsChannel = wayBillInfo.OutShippingMethodID.ToString(),
                            newTrackNumber = wayBillInfo.TrackingNumber,
                            printPerson = _customerService.GetCustomer(wayBillInfo.CustomerCode).Name,
                            printDate = DateTime.Now

                        });
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                    }

                }

            }



            return View(viewModel);
        }


        public ActionResult DHLPrintPreview2(string wayBillNumber)
        {
            DHLPrintViewModel viewModel = ((DHLPrintPreview(wayBillNumber) as ViewResult).Model) as DHLPrintViewModel;
            return View(viewModel);
        }

        [System.Web.Mvc.HttpPost]
        [FormValueRequired("Selectbtn")]
        [System.Web.Mvc.ActionName("ExpressPrintWayBillList")]
        public ActionResult SelectWayBillList(ExpressPrintWayBillParam param)
        {
            return View(BindList(param));
        }
        public ExpressPrintWayBillViewModel BindList(ExpressPrintWayBillParam param = null)
        {


            ExpressPrintWayBillViewModel model = new ExpressPrintWayBillViewModel();
            WayBill.GetSearchFilterList().ForEach(p =>
            {
                model.SearchWheres.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = model.Param.SearchWhere.HasValue && p.ValueField == model.Param.SearchWhere.Value.ToString() });
            });
            model.ShippingMethods = GetShippingMethodListByType();
            if (param != null)
            {
                model.Param = param;
                model.Param.PageSize = param.PageSize;
            }


            List<SelectListItem> vList = new List<SelectListItem>();
            var list = _freightService.GetVenderList(true);
            list.ForEach(p => vList.Add(new SelectListItem() { Value = p.VenderCode, Text = p.VenderName }));
            vList.Insert(0, new SelectListItem() { Value = "", Text = "请选择" });
            model.VenderList = vList;

            model.ExpressPrintWayBills = _inStorageService.GetExpressPrintWayBillList(model.Param);
            model.ExpressPrintWayBills.Each(p =>
            {
                if (!string.IsNullOrWhiteSpace(p.VenderCode))
                {
                    p.VenderName = vList.Find(v => v.Value == p.VenderCode).Text;
                }
                if (p.OutShippingMethodID.HasValue)
                {
                    if (model.ShippingMethods.Find(v => v.Value == p.OutShippingMethodID.ToString()) != null)
                    {
                        p.OutShippingMethodName = model.ShippingMethods.Find(v => v.Value == p.OutShippingMethodID.ToString()).Text;
                    }
                }
                if (p.InShippingMethodID.HasValue)
                {
                    if (model.ShippingMethods.Find(v => v.Value == p.InShippingMethodID.ToString()) != null)
                    {
                        p.InShippingMethodName = model.ShippingMethods.Find(v => v.Value == p.InShippingMethodID.ToString()).Text;
                    }
                }
            });


            return model;
        }

        /// <summary>
        /// 获取字典类型
        /// </summary>
        /// <param name="dictionaryTypeParentId">父节点ID</param>
        /// <param name="dictionaryTypeId">字典类型ID</param>
        /// <returns></returns> 
        public List<SelectListItem> GetDictionaryTypeList(string dictionaryTypeParentId, string dictionaryTypeId)
        {
            DictionaryTypeListParam param = new DictionaryTypeListParam()
                {
                    ParentId = dictionaryTypeParentId,
                    DicTypeId = dictionaryTypeId
                };
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            _dictionaryTypeService.GetList(param).ToList()
                           .ForEach(p => selectListItems.Add(new SelectListItem()
                           {
                               Text = p.Name,
                               Value = p.DicTypeId,
                               Selected = p.DicTypeId == dictionaryTypeId
                           }));
            return selectListItems;
        }

        [System.Web.Mvc.HttpPost]
        [ButtonPermissionValidator(PermissionRecords.FastOutStorageCode)]
        public JsonResult CheckOnFastOutStorage(FastOutStorageViewModel model)
        {
            var result = new ResponseResult();
            if (model == null)
            {
                result.Result = false;
                result.Message = "提交数据为空！";
            }
            else
            {
                
                if (model.SelectTotalPackage.HasValue && model.SelectTotalPackage.Value == 2)
                {
                    var oldtotalPackagelist = _outStorageService.GetTotalPackageNumberList(model.VenderCode);
                    if (oldtotalPackagelist == null)
                    {
                        result.Result = false;
                        result.Message = "该服务商不存在符合要求的总包号！";
                    }
                    else
                    {
                        if (!oldtotalPackagelist.Contains(model.TotalPackageNumber))
                        {
                            result.Result = false;
                            result.Message = "该总包号不存在或者已经离港！";
                        }
                    }
                    if (!result.Message.IsNullOrWhiteSpace())
                    {
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }

                if (model.ShippingMethodName == "国际小包优+")
                {
                    result.Result = false;
                    result.Message = "国际小包优+运输方式不能直接出仓！";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                var wayBillNumbers = model.WayBillNumbers.Split(',').ToList();
                var list = _outStorageService.GetWayBillInfoListByWayBillNumber(wayBillNumbers);

                #region 判断是否存在现结客户有货款未结清
                //List<string> customers=new List<string>();
                //list.ForEach(p =>
                //    {
                //        if (!customers.Contains(p.CustomerCode))
                //        {
                //            customers.Add(p.CustomerCode);
                //        }
                //    });
                //var customerList = _customerService.GetCustomerList(customers);
                //customerList.ForEach(p =>
                //    {
                //        var customerBalance = _customerService.GetCustomerBalance(p.CustomerCode);
                //        decimal money = 0;
                //        if (p.EnableCredit)
                //        {
                //            money = (customerBalance.Balance ?? 0) + p.MaxDelinquentAmounts;
                //        }
                //        else
                //        {
                //            money = customerBalance.Balance ?? 0;
                //        }
                //        if (money < 0)
                //        {
                //            result.Result = false;
                //            result.Message = p.Name+ "现结客户货款未结清，不允许出库！";
                //        }
                //    });
                //if (!result.Result && !string.IsNullOrWhiteSpace(result.Message))
                //{
                //    return Json(result, JsonRequestBehavior.AllowGet);
                //}
                #endregion
                var getShippinglist = _freightService.GetShippingMethodListByVenderCode(model.VenderCode, true);
                bool isHaveTrackingNum = getShippinglist.FirstOrDefault(a => a.ShippingMethodId == model.ShippingMethodId).HaveTrackingNum;
                //Add By zhengsong
                //判断是否是指定的运输渠道
                if (list != null && list[0].InShippingMethodID != null)
                {
                    var vender = _freightService.GetVender(model.VenderCode);
                    int venderId = 0;
                    if (vender != null)
                    {
                        venderId = vender.VenderId;
                    }
                    var outStorageShippinMethod = _outStorageService.GetDeliveryChannelConfiguration(list[0].InShippingMethodID ?? 0, venderId, model.ShippingMethodId);
                    if (outStorageShippinMethod == null)
                    {
                        result.Result = false;
                        result.Message = "[" + list[0].InShippingMethodName + "]运输方式，与该出仓渠道不匹配，不允许出仓！";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }

                if (list != null && list.Count > 0)
                {
                    var outStorage = new CreateOutStorageExt
                        {
                            OutStorage =
                                {
                                    VenderCode = model.VenderCode,
                                    VenderName = model.VenderName,
                                    Freight = 0,
                                    FuelCharge = 0,
                                    OutStorageID = SequenceNumberService.GetSequenceNumber(PrefixCode.OutStorageID),
                                    Register = 0,
                                    TotalFee = 0,
                                    TotalQty = 0,
                                    TotalWeight = 0,
                                    Surcharge = 0
                                },
                            TotalQty = 0,
                            TotalVotes = 0,
                            TotalWeight = 0
                        };
                    var waybillList = new List<string>();
                    list.ForEach(w =>
                        {
                            if (!string.IsNullOrWhiteSpace(w.WayBillNumber) && w.Status == WayBill.StatusToValue(WayBill.StatusEnum.Have) && !w.IsHold)
                            {
                                var extmodel = new OutWayBillInfoExt
                                    {
                                        TrackingNumber = w.TrackingNumber,
                                        SettleWeight = w.SettleWeight ?? 0,
                                        WayBillNumber = w.WayBillNumber,
                                        OutShippingMethodID = model.ShippingMethodId,
                                        OutShippingMethodName = model.ShippingMethodName,
                                        Surcharge = 0,
                                        GoodsTypeID = model.GoodsTypeId,
                                        HaveTrackingNum = isHaveTrackingNum

                                    };
                                outStorage.WayBillInfos.Add(extmodel);
                                outStorage.OutStorage.TotalQty++;
                                outStorage.OutStorage.TotalWeight += extmodel.SettleWeight;
                                outStorage.TotalVotes++;
                                outStorage.TotalWeight += w.Weight ?? 0;
                                waybillList.Add(w.WayBillNumber);
                            }
                        });
                    if (waybillList.Any())
                    {
                        if (model.SelectTotalPackage.HasValue)
                        {
                            outStorage.IsCreateTotalPackageNumber = model.SelectTotalPackage.Value == 1;
                        }
                        outStorage.Remark = model.Remark;
                        outStorage.TotalPackageNumber = model.TotalPackageNumber;
                        decimal totalWeight = 0;
                        _outStorageService.GetWayBillTotalWeight(waybillList, out totalWeight);
                        int totalQty = 0;
                        _outStorageService.GetWayBillTotalQty(waybillList, out totalQty);
                        outStorage.TotalQty = totalQty;
                        outStorage.TotalWeight = totalWeight;
                    }
                    try
                    {
                        _outStorageService.CreateOutStorage(outStorage);

                        result.Result = true;
                        result.Message = outStorage.OutStorage.OutStorageID;
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                        result.Result = false;
                        result.Message = ex.Message;
                    }
                }
                else
                {
                    result.Result = false;
                    result.Message = "提交数据为空！";
                }

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpPost]
        [FormValueRequired("btnOutStorage")]
        public ActionResult OutStorage(List<string> WayBillNumbers)
        {
            return RedirectToAction("FristOutStorage", "WayBill", WayBillNumbers);
        }

        public List<SelectListItem> GetShippingMethodSelectList(int shippingMethodId)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            _freightService.GetShippingMethodListByCustomerTypeId(null, false)
                           .ForEach(p => selectListItems.Add(new SelectListItem()
                           {
                               Text = p.ShippingMethodName,
                               Value = p.ShippingMethodId.ToString(),
                               Selected = p.ShippingMethodId == shippingMethodId
                           }));
            return selectListItems;
        }


        public List<SelectListItem> GetShippingMethodListByType()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            _freightService.GetShippingMethodList(true)
                           .ForEach(p => selectListItems.Add(new SelectListItem()
                           {
                               Text = p.FullName,
                               Value = p.ShippingMethodId.ToString()
                           }));
            selectListItems.Insert(0, new SelectListItem() { Value = "0", Text = "请选择" });
            return selectListItems;
        }

        public List<SelectListItem> GetShippingMethodSelectList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            _freightService.GetShippingMethodListByCustomerTypeId(null, false)
                           .ForEach(p => selectListItems.Add(new SelectListItem()
                               {
                                   Text = p.ShippingMethodName,
                                   Value = p.ShippingMethodId.ToString()
                               }));
            return selectListItems;
        }
        public ActionResult FilterShippingMethod(string customerId, int? customerTypeId, string venderCode, int type)
        {
            return View(BindShippingMethod(customerId, customerTypeId, venderCode, type, false));
        }

        public ActionResult SelectShippingMethod(string customerId, int? customerTypeId, string venderCode, int type, int? shippingMethodType, bool? isAll)
        {
            isAll = isAll ?? false;
            _freightService.GetVenderList(true);
            return View(BindShippingMethod(customerId, customerTypeId, venderCode, type, shippingMethodType, isAll));
        }

        public ActionResult SelectOutShippingMethod(string customerId, int? customerTypeId, string venderCode, int type)
        {
            return View(BindShippingMethod(customerId, customerTypeId, venderCode, type, false));
        }

        // 避免多次弹出层相同运输方式,数据读不出
        public ActionResult SelectShippingMethodInfo(string customerId, int? customerTypeId, string venderCode, int type, bool? isAll)
        {
            isAll = isAll ?? false;
            _freightService.GetVenderList(true);
            return View(BindShippingMethod(customerId, customerTypeId, venderCode, type, isAll));
        }

        public JsonResult GetShippingMethodByVenderCode(string venderCode)
        {
            ShippingMethodJsonReuslt jsonReuslt = new ShippingMethodJsonReuslt();
            if (!string.IsNullOrWhiteSpace(venderCode))
            {
                _freightService.GetShippingMethodListByVenderCode(venderCode, true).ForEach(p => jsonReuslt.Items.Add(new ShippingMethodItem() { ShippingMethodId = p.ShippingMethodId, ShippingMethodName = p.ShippingMethodName }));
            }
            return Json(jsonReuslt, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 更新运单的供应商和发货运输方式，并请求DHL接口
        /// </summary>
        /// <param name="venderCode">供应商代码</param>
        /// <param name="outShippingMethodId">发货运输方式ID</param>
        /// <param name="selectWayBillNumber">已选择的运单</param>
        /// <returns></returns>
        public JsonResult UpdateWayBillByVenderCode(string venderCode, int outShippingMethodId,
                                                    string selectWayBillNumber)
        {
            var model = new ResponseResult();
            if (!string.IsNullOrWhiteSpace(selectWayBillNumber))
            {
                var wayBillNumbers = selectWayBillNumber.Split(',').ToList();
                //list.Each(p => { p.VenderCode = venderCode;
                //                   p.OutShippingMethodID = outShippingMethodId;
                //});
                //_inStorageService.BatchUpdateWayBillByVenderCode(list);
                //荷兰小包
                if (sysConfig.NLPOSTShippingMethodID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Contains(outShippingMethodId.ToString()))
                {
                    model = NLPOST(venderCode, outShippingMethodId, wayBillNumbers);
                    if (string.IsNullOrWhiteSpace(model.Message))
                    {
                        model.Result = true;
                        model.Message = "更新成功";
                    }
                    else
                    {
                        model.Result = false;
                    }
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
                var expressAccount = _expressService.GetExpressAccountInfo(venderCode.Trim(), outShippingMethodId);
                if (expressAccount == null)
                {
                    model.Result = false;
                    model.Message = "DHL帐号不存在，请求DHL接口失败";
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
                var applicationName = string.Empty;

                var list = _inStorageService.GetWayBillByWayBillNumbers(wayBillNumbers);
                foreach (var wayBillInfo in list)
                {
                    applicationName = wayBillInfo.ApplicationInfos.First().ApplicationName.Cutstring(70);
                    string[] shippingAddress = wayBillInfo.ShippingInfo.ShippingAddress.StringSplitLengthWords(35).ToArray();
                    wayBillInfo.VenderCode = venderCode;
                    wayBillInfo.OutShippingMethodID = outShippingMethodId;
                    if (null != wayBillInfo.ExpressRespons)
                    {
                        model.Message += "运单号为{0},已经调用DHL接口,无需重复请求".FormatWith(wayBillInfo.WayBillNumber);
                        break;
                    }
                    LMS.Data.Express.DHL.Request.ShipmentValidateRequestAP ap = new LMS.Data.Express.DHL.Request.
                        ShipmentValidateRequestAP()
                        {

                            Billing = new LMS.Data.Express.DHL.Request.Billing()
                                {
                                    DutyPaymentType = wayBillInfo.EnableTariffPrepay ? LMS.Data.Express.DHL.Request.DutyTaxPaymentType.S : LMS.Data.Express.DHL.Request.DutyTaxPaymentType.R,
                                    ShipperAccountNumber = expressAccount.ShipperAccountNumber,
                                    ShippingPaymentType = LMS.Data.Express.DHL.Request.ShipmentPaymentType.S
                                },
                            Commodity = new[]
                                {
                                    new LMS.Data.Express.DHL.Request.Commodity()
                                        {
                                            CommodityCode = "1111" //商品代码
                                        }
                                },
                            Consignee = new LMS.Data.Express.DHL.Request.Consignee() //收货人
                                {
                                    AddressLine = new[]
                                        {
                                            shippingAddress[0],
											  string.IsNullOrWhiteSpace(wayBillInfo.ShippingInfo.ShippingAddress1)&&shippingAddress.Length>1?shippingAddress[1]:wayBillInfo.ShippingInfo.ShippingAddress1,//多地址 yungchu
											   string.IsNullOrWhiteSpace(wayBillInfo.ShippingInfo.ShippingAddress2)&&shippingAddress.Length>2?shippingAddress[2]:wayBillInfo.ShippingInfo.ShippingAddress2
                                        },
                                    City = wayBillInfo.ShippingInfo.ShippingCity,
                                    CompanyName = string.IsNullOrWhiteSpace(wayBillInfo.ShippingInfo.ShippingCompany) ? (wayBillInfo.ShippingInfo.ShippingFirstName + " " +
                                                wayBillInfo.ShippingInfo.ShippingLastName) : wayBillInfo.ShippingInfo.ShippingCompany,
                                    Contact = new LMS.Data.Express.DHL.Request.Contact()
                                        {
                                            //Email = new Email()
                                            //{
                                            //    Body = "dsa@us.dhl.com",
                                            //    cc = new[] { "String", "String" },
                                            //    From = "dsa@us.dhl.com",
                                            //    ReplyTo = "String",
                                            //    Subject = "String",
                                            //    To = "dsa@us.dhl.com"
                                            //},
                                            PersonName =
                                                wayBillInfo.ShippingInfo.ShippingFirstName + " " +
                                                wayBillInfo.ShippingInfo.ShippingLastName,
                                            PhoneNumber = wayBillInfo.ShippingInfo.ShippingPhone,
                                            PhoneExtension = 455,
                                        },
                                    CountryCode = wayBillInfo.ShippingInfo.CountryCode,
                                    PostalCode = wayBillInfo.ShippingInfo.ShippingZip
                                },
                            Dutiable = new LMS.Data.Express.DHL.Request.Dutiable()
                                {
                                    DeclaredCurrency = "USD",
                                    DeclaredValue = wayBillInfo.ApplicationInfos.Sum(p => (p.UnitPrice ?? 1) * (p.Qty ?? 1)).ToString("F2"),
                                    ShipperEIN = "Text"
                                },
                            LanguageCode = "en",
                            PiecesEnabled = LMS.Data.Express.DHL.Request.PiecesEnabled.Y,
                            Reference = new[]
                                {
                                    new LMS.Data.Express.DHL.Request.Reference()
                                        {
                                            ReferenceID = wayBillInfo.WayBillNumber
                                        }
                                },
                            Request = new LMS.Data.Express.DHL.Request.Request()
                                {
                                    ServiceHeader = new LMS.Data.Express.DHL.Request.ServiceHeader()
                                        {
                                            //MessageReference = "1234567890123456789012345678901",
                                            //MessageTime = DateTime.Parse("2011-07-11T11:25:56.000-08:00"),
                                            Password = expressAccount.Password,
                                            SiteID = expressAccount.Account
                                        }
                                },
                            ShipmentDetails = new LMS.Data.Express.DHL.Request.ShipmentDetails() //出货详情
                                {
                                    Contents = applicationName,
                                    CurrencyCode = "USD",
                                    Date = DateTime.Now,
                                    DimensionUnit = LMS.Data.Express.DHL.Request.DimensionUnit.C,
                                    DoorTo = LMS.Data.Express.DHL.Request.DoorTo.DD,
                                    GlobalProductCode = "P",
                                    LocalProductCode = "P",
                                    NumberOfPieces = 1,
                                    PackageType = LMS.Data.Express.DHL.Request.PackageType.EE,
                                    //Weight = wayBillInfo.ApplicationInfos.Sum(p => (p.UnitWeight ?? 0.001M) * (p.Qty ?? 1)),
                                    Weight = wayBillInfo.Weight ?? 0,
                                    WeightUnit = LMS.Data.Express.DHL.Request.WeightUnit.K,
                                    //保险费
                                    //InsuredAmount =
                                    //    wayBillInfo.CustomerOrderInfo.InsureAmount != null
                                    //        ? wayBillInfo.CustomerOrderInfo.InsureAmount.ToString()
                                    //        : "",
                                    Pieces = new[]
                                        {
                                            new LMS.Data.Express.DHL.Request.Piece
                                                {
                                                    // PieceID = "String",
                                                    Depth = Math.Ceiling(wayBillInfo.Length ?? 0).ConvertTo<uint>(),
                                                    // DimWeight = (decimal)2.111,
                                                    Height = Math.Ceiling(wayBillInfo.Height ?? 0).ConvertTo<uint>(),
                                                    //PackageType = PackageType.EE,
                                                    Weight = Math.Ceiling(wayBillInfo.Weight ?? 0),
                                                    Width = Math.Ceiling(wayBillInfo.Width ?? 0).ConvertTo<uint>()
                                                }
                                        }
                                },
                            Shipper = new LMS.Data.Express.DHL.Request.Shipper() //发货人
                                {
                                    ShipperID = expressAccount.ShipperAccountNumber,
                                    AddressLine = new[] { expressAccount.Address },
                                    City = expressAccount.City,
                                    CompanyName = expressAccount.CompanyName,
                                    Contact = new LMS.Data.Express.DHL.Request.Contact()
                                        {
                                            //Email = new Email()
                                            //{
                                            //    Body = "djogi@dhl.com",
                                            //    cc = new[] { "String" },
                                            //    From = "djogi@dhl.com",
                                            //    ReplyTo = "djogi@163.com",
                                            //    Subject = "String",
                                            //    To = "djogi@163.com"
                                            //},
                                            PersonName = expressAccount.PersonName,
                                            FaxNumber = expressAccount.FaxNumber,
                                            PhoneExtension = expressAccount.PhoneExtension.ConvertTo<uint>(),
                                            PhoneNumber = expressAccount.PhoneNumber,
                                            Telex = expressAccount.Telex
                                        },
                                    CountryCode = expressAccount.CountryCode,
                                    CountryName = expressAccount.CountryName,
                                    DivisionCode = expressAccount.DivisionCode,
                                    PostalCode = expressAccount.PostalCode
                                }
                        };

                    ap.Consignee.AddressLine = ap.Consignee.AddressLine.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

                    try
                    {
                        var country = _countryService.GetCountryList("").Single(c => c.CountryCode == wayBillInfo.ShippingInfo.CountryCode.ToUpperInvariant());
                        ap.Consignee.CountryName = country.Name;
                        var responseResult = _expressService.PostDHLShipment(ap, expressAccount.ServerUrl);
                        if (null != responseResult)
                        {
                            string fileExtension = ".jpg";
                            ExpressRespons response = new ExpressRespons()
                            {
                                DHLRoutingBarCode = responseResult.DHLRoutingCode,
                                DataIdentifier = responseResult.Pieces[0].DataIdentifier,
                                DHLRoutingBarCodeImg =
                                    Tools.Base64StringToImage(sysConfig.DHLBarCodePath,
                                                              "DHLRouting" + Guid.NewGuid().ToString(""),
                                                              responseResult.Barcodes.DHLRoutingBarCode,
                                                              fileExtension),
                                DHLRoutingDataId = responseResult.DHLRoutingDataId,
                                LicensePlate = responseResult.Pieces[0].LicensePlate,
                                LicensePlateBarCodeImg =
                                    Tools.Base64StringToImage(sysConfig.DHLBarCodePath,
                                                              "LicensePlate" +
                                                              Guid.NewGuid().ToString(""),
                                                              responseResult.Pieces[0].LicensePlateBarCode,
                                                              fileExtension),
                                ShipmentDetailTime = responseResult.ShipmentDate,
                                ServiceAreaCode = responseResult.DestinationServiceArea.ServiceAreaCode,
                                FacilityCode = responseResult.DestinationServiceArea.FacilityCode,
                                WayBillNumber = wayBillInfo.WayBillNumber,
                                AirwayBillNumber = responseResult.AirwayBillNumber,
                                AirwayBillNumberBarCodeImg =
                                    Tools.Base64StringToImage(sysConfig.DHLBarCodePath,
                                                              "WayBillNumber" +
                                                              responseResult.AirwayBillNumber,
                                                              responseResult.Barcodes.AWBBarCode, fileExtension),
                            };

                            wayBillInfo.TrackingNumber = responseResult.AirwayBillNumber;
                            wayBillInfo.CustomerOrderInfo.TrackingNumber = responseResult.AirwayBillNumber;
                            try
                            {
                                _expressService.AddExpressResponse(response, wayBillInfo);
                            }
                            catch (Exception ex)
                            {
                                Log.Exception(ex);
                                model.Message += "运单号为{0}：错误信息：{1}".FormatWith(wayBillInfo.WayBillNumber, ex.Message);
                            }
                        }
                        else
                        {
                            model.Message += "运单号为{0}：请求DHL接口失败".FormatWith(wayBillInfo.WayBillNumber);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                        model.Message += "运单号为{0}：错误信息：{1}".FormatWith(wayBillInfo.WayBillNumber, ex.Message);
                    }
                }

            }
            if (string.IsNullOrWhiteSpace(model.Message))
            {
                model.Result = true;
                model.Message = "更新成功";
            }
            else
            {
                model.Result = false;
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private ResponseResult NLPOST(string venderCode, int outShippingMethodId, List<string> wayBillNumbers)
        {
            var model = new ResponseResult();
            var list = _inStorageService.GetWayBillByWayBillNumbers(wayBillNumbers);
            //获取所到国家
            var targetCountryCodeList = _freightService.GetCountryArea(outShippingMethodId);
            //var targetCountryCodeList = new List<string>()
            //             {
            //                 "US","SK","SI","SE","RO","PT","PL","PL","LV","LU","LT","IT","IS","IE","IE","GR","GB","FI","ES","EE","DK","DE","CZ","CH","CA","BE","AT"
            //             };
            foreach (var wayBillInfo in list)
            {
                if (wayBillInfo.CountryCode.IsNullOrWhiteSpace())
                {
                    model.Message += "运单号为{0} 发货国家为空！".FormatWith(wayBillInfo.WayBillNumber);
                    continue;
                }
                if (!targetCountryCodeList.Exists(p => p.CountryCode.ToUpperInvariant() == wayBillInfo.CountryCode.ToUpperInvariant()))
                {
                    model.Message += "运单号为{0} 发货国家为{1} 荷兰邮政小包挂号暂时不支持此国家！".FormatWith(wayBillInfo.WayBillNumber, wayBillInfo.CountryCode);
                    continue;
                }
                if (_expressService.IsExistNLPOST(wayBillInfo.WayBillNumber))
                {
                    model.Message += "运单号为{0},已经申请荷兰邮政小包挂号,无需重复请求".FormatWith(wayBillInfo.WayBillNumber);
                    continue;
                }
                //(wayBillInfo.ShippingInfo.ShippingPhone ?? "-")
                
                //var countryName = _countryService.GetCountryByCode(wayBillInfo.CountryCode.Trim());
                string strXML = @"<Request service='OrderService' lang='en'>
                                         <Head>{0}</Head>
                                         <Body>
                                           <Order orderid='" + wayBillInfo.WayBillNumber.WayBillNumberReplace() + @"' 
                                                             express_type='A1' 
                                                             j_company='SHENZHEN ZONGTENG' 
                                                             j_contact='Summer'
                                                             j_tel='15818739473'
                                                             j_mobile='15818739473' 
                                                             j_address='2FL,Block C,Longjing Second industrial Park,Taoyuan Village,Nanshan District,Shenzhen,China'
                                                             d_company='" + (wayBillInfo.ShippingInfo.ShippingCompany ?? "-").Trim().ToDBC().StripXML() + @"'  
                                                             d_contact='" + (wayBillInfo.ShippingInfo.ShippingFirstName ?? "").Trim().StripXML() + " " + (wayBillInfo.ShippingInfo.ShippingLastName ?? "").Trim().ToDBC().StripXML() + @"'
                                                             d_tel='" + (wayBillInfo.ShippingInfo.ShippingPhone.GetNumber() == "" ? "-" : wayBillInfo.ShippingInfo.ShippingPhone.GetNumber()) + @"'
                                                             d_mobile='" + (wayBillInfo.ShippingInfo.ShippingPhone.GetNumber() == "" ? "-" : wayBillInfo.ShippingInfo.ShippingPhone.GetNumber()) + @"'
                                                             d_address='" + string.Format("{0} {1} {2}", (wayBillInfo.ShippingInfo.ShippingAddress ?? "").Trim(), (wayBillInfo.ShippingInfo.ShippingAddress1 ?? "").Trim(), (wayBillInfo.ShippingInfo.ShippingAddress2 ?? "").Trim()).ToDBC().StripXML() + @"'
                                                             parcel_quantity='" + (wayBillInfo.CustomerOrderInfo.PackageNumber ?? 1) + @"' 
                                                             j_province='Guangdong province' 
                                                             j_city='Shenzhen' 
                                                             j_post_code='518055' 
                                                             j_country='CN'
                                                             d_country='" + wayBillInfo.CountryCode.Trim() + @"'      
                                                             d_post_code='" + wayBillInfo.ShippingInfo.ShippingZip.GetNumber() + @"'    
                                                             d_province='" + (wayBillInfo.ShippingInfo.ShippingState ?? "").Trim().ToDBC().StripXML() + @"'
                                                             d_city='" + (wayBillInfo.ShippingInfo.ShippingCity ?? "-").Trim().ToDBC().StripXML() + @"'
                                                             returnsign='" + (wayBillInfo.IsReturn ? "Y" : "N") + @"'       
                                                             cargo_total_weight='" + wayBillInfo.ApplicationInfos.Sum(p => p.Qty * p.UnitWeight).Value.ToString("F3") + @"'    >                                     
                                           {1}
                                        </Order>
                                        </Body>
                                        </Request>";
                string application = string.Empty;
                foreach (var app in wayBillInfo.ApplicationInfos)
                {
                    if(app.IsDelete)continue;
                    application += "<Cargo ename='" + app.ApplicationName.Trim().ToDBC().StripXML();
                    if (app.HSCode.GetNumber() != "")
                    {
                        application += "' hscode='" + app.HSCode.GetNumber();
                    }
                    application += "' count='" + (app.Qty ?? 1) + "' unit='PCE' weight='" + app.UnitWeight + "'  amount='" +
                     app.UnitPrice.Value.ToString("F2") + @"'> </Cargo> ";
                }
                string responseResult = LMSSFService.SfExpressService(strXML.FormatWith(sysConfig.ClientCode, application),
                                              (strXML.FormatWith(sysConfig.ClientCode, application) +
                                               sysConfig.SFCheckWord).ToMD5());
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(responseResult);
                XmlNode root = xdoc.SelectSingleNode("/Response/Head");
                if (root != null && root.InnerText == "OK")
                {
                    var parcel = new NetherlandsParcelRespons();
                    XmlNode o = xdoc.SelectSingleNode("/Response/Body/OrderResponse");
                    if (o != null && o.Attributes != null)
                    {
                        if (o.Attributes["mailno"] != null)
                        {
                            parcel.MailNo = o.Attributes["mailno"].Value.Trim();
                        }
                        if (o.Attributes["agent_mailno"] != null)
                        {
                            parcel.AgentMailNo = o.Attributes["agent_mailno"].Value.Trim();
                            wayBillInfo.TrackingNumber = o.Attributes["agent_mailno"].Value.Trim();
                            wayBillInfo.CustomerOrderInfo.TrackingNumber = o.Attributes["agent_mailno"].Value.Trim();
                        }
                        if (o.Attributes["origincode"] != null)
                        {
                            parcel.OriginCode = o.Attributes["origincode"].Value;
                        }
                        if (o.Attributes["destcode"] != null)
                        {
                            parcel.DestCode = o.Attributes["destcode"].Value;
                        }
                        //if (o.Attributes["orderid"] != null)
                        //{
                        //    parcel.WayBillNumber = o.Attributes["orderid"].Value.Trim();
                        //}
                        parcel.WayBillNumber = wayBillInfo.WayBillNumber;
                        parcel.Status = 1;
                        wayBillInfo.VenderCode = venderCode;
                        wayBillInfo.OutShippingMethodID = outShippingMethodId;
                        _expressService.AddNLPOST(parcel, wayBillInfo);
                        model.Message += NLPOSTConfirm(wayBillInfo.WayBillNumber, parcel.MailNo).Message;

                    }
                    else
                    {
                        model.Message += "运单号为：{0} 申请顺丰失败！".FormatWith(wayBillInfo.WayBillNumber);
                    }
                }
                else if (root != null && root.InnerText == "ERR")
                {
                    XmlNode err = xdoc.SelectSingleNode("/Response/ERROR");
                    if (err != null && err.Attributes != null && err.Attributes["code"] != null)
                    {
                        //LMS.Data.Express.NLPOST.ErrorCode.ResourceManager.GetString();
                        if (!err.Attributes["code"].Value.IsNullOrWhiteSpace())
                        {
                            var regex = new Regex(@"^[0-9,]*$");
                            if (regex.IsMatch(err.Attributes["code"].Value))
                            {
                                var errmsg = new List<string>();
                                err.Attributes["code"].Value.Split(',')
                                                      .ToList()
                                                      .ForEach(
                                                          p =>
                                                          errmsg.Add(
                                                              LMS.Data.Express.NLPOST.ErrorCode.ResourceManager
                                                                 .GetString(p) ?? p));
                                model.Message +=
                                    "运单号为：{2}提交顺丰API错误代码为:{0},错误信息：{1}".FormatWith(err.Attributes["code"].Value,
                                                                                   string.Join(",", errmsg),
                                                                                   wayBillInfo.WayBillNumber);
                                Log.Error("运单号为：{2}提交顺丰API错误代码为:{0},错误信息：{1}".FormatWith(err.Attributes["code"].Value,
                                                                                         string.Join(",", errmsg),
                                                                                         wayBillInfo.WayBillNumber));
                            }
                            else
                            {
                                var parcel = SearchNLPOST(wayBillInfo.WayBillNumber);
                                //订单状态代码
                                //A 新建订单
                                //C 已出货
                                //D 已删除运单
                                //M 平邮件
                                //P 已申请投递
                                //V 已收货
                                if (!parcel.AgentMailNo.IsNullOrWhiteSpace() && !parcel.Remark.IsNullOrWhiteSpace() &&
                                    parcel.Remark == "A")
                                {
                                    parcel.Status = 1;
                                    wayBillInfo.VenderCode = venderCode;
                                    wayBillInfo.OutShippingMethodID = outShippingMethodId;
                                    _expressService.AddNLPOST(parcel, wayBillInfo);
                                    model.Message += NLPOSTConfirm(wayBillInfo.WayBillNumber, parcel.MailNo).Message;
                                }
                            }
                        } 
                        
                    }

                }
                else
                {
                    model.Message += "运单号为：{0} 申请顺丰失败！".FormatWith(wayBillInfo.WayBillNumber);
                }
            }
            return model;
        }

        private ResponseResult NLPOSTConfirm(string wayBillNumber, string mailno, int dealtype = 1)
        {
            var model = new ResponseResult();
            string confirmXML = @"<Request service='OrderConfirmService' lang='en'>
                                                                 <Head>" + sysConfig.ClientCode + @"</Head>
                                                                 <Body>
                                                                    <OrderConfirm orderid ='" + wayBillNumber.WayBillNumberReplace() + @"' 
                                                                                  mailno='" + mailno + @"'
                                                                                  dealtype='" + dealtype.ToString() + @"'>
                                                                    </OrderConfirm>
                                                                </Body>
                                                            </Request>";
            string responseResult = LMSSFService.SfExpressService(confirmXML, (confirmXML + sysConfig.SFCheckWord).ToMD5());
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(responseResult);
            XmlNode root = xdoc.SelectSingleNode("/Response/Head");
            if (root != null && root.InnerText == "OK")
            {
                var parcel = new NetherlandsParcelRespons();
                XmlNode o = xdoc.SelectSingleNode("/Response/Body/OrderConfirmResponse");
                if (o != null && o.Attributes != null)
                {
                    if (o.Attributes["mailno"] != null)
                    {
                        parcel.MailNo = o.Attributes["mailno"].Value;
                    }
                    //if (o.Attributes["orderid"] != null)
                    //{
                    //    parcel.WayBillNumber = o.Attributes["orderid"].Value;
                    //}
                    parcel.WayBillNumber = wayBillNumber;
                    parcel.Status = 2;
                    _expressService.UpdateNLPOST(parcel);
                    return model;
                }
            }
            else if (root != null && root.InnerText == "ERR")
            {
                XmlNode err = xdoc.SelectSingleNode("/Response/ERROR");
                if (err != null && err.Attributes != null && err.Attributes["code"] != null)
                {
                    model.Message += "运单号为：{2}订单发货确定顺丰API错误代码为:{0},错误信息：{1}".FormatWith(err.Attributes["code"].Value,
                                                                                    err.InnerText,
                                                                                    wayBillNumber);
                    Log.Error("运单号为：{2}订单发货确定顺丰API错误代码为:{0},错误信息：{1}".FormatWith(err.Attributes["code"].Value, err.InnerText,
                                                                             wayBillNumber));
                    return model;
                }
            }
            model.Message += "运单号为：{0} 订单发货确定顺丰API失败！".FormatWith(wayBillNumber);
            return model;
        }

        private NetherlandsParcelRespons SearchNLPOST(string wayBillNumber)
        {
            var model = new NetherlandsParcelRespons(){WayBillNumber = wayBillNumber};
            string searchXML = @"<Request service='OrderSearchService' lang='en'>
                                <Head>" + sysConfig.ClientCode + @"</Head>
                                <Body>
                                <OrderSearch orderid ='" + wayBillNumber.WayBillNumberReplace() + @"' />
                                </Body>
                                </Request>
                                ";
            string responseResult = LMSSFService.SfExpressService(searchXML, (searchXML + sysConfig.SFCheckWord).ToMD5());
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(responseResult);
            XmlNode root = xdoc.SelectSingleNode("/Response/Head");
            if (root != null && root.InnerText == "OK")
            {
                XmlNode o = xdoc.SelectSingleNode("/Response/Body/OrderSearchResponse");
                if (o != null && o.Attributes != null)
                {
                    if (o.Attributes["mailno"] != null)
                    {
                        model.MailNo = o.Attributes["mailno"].Value;
                    }
                    if (o.Attributes["origincode"] != null)
                    {
                        model.OriginCode = o.Attributes["origincode"].Value;
                    }
                    if (o.Attributes["destcode"] != null)
                    {
                        model.DestCode = o.Attributes["destcode"].Value;
                    }
                    if (o.Attributes["coservehawbcode"] != null)
                    {
                        model.AgentMailNo = o.Attributes["coservehawbcode"].Value;
                    }
                    if (o.Attributes["oscode"] != null)
                    {
                        model.Remark = o.Attributes["oscode"].Value;
                    }
                    model.Status = 1;
                }
            }
            else if (root != null && root.InnerText == "ERR")
            {
                XmlNode err = xdoc.SelectSingleNode("/Response/ERROR");
                if (err != null && err.Attributes != null && err.Attributes["code"] != null)
                {
                    Log.Error("运单号为：{2}订单查询顺丰API错误代码为:{0},错误信息：{1}".FormatWith(err.Attributes["code"].Value, err.InnerText,
                                                                             wayBillNumber));
                }
            }
            return model;
        }

        private SelectShippingMethodViewModel BindShippingMethod(string customerId, int? customerTypeId, string venderCode, int type, bool? IsAll)
        {
            return BindShippingMethod(customerId, customerTypeId, venderCode, type, 0, IsAll);
        }

        private SelectShippingMethodViewModel BindShippingMethod(string customerId, int? customerTypeId, string venderCode, int type, int? shippingMethodType, bool? IsAll)
        {
            var model = new SelectShippingMethodViewModel();
            if (type == 1)
            {
                _freightService.GetShippingMethodListByCustomerTypeId(customerId, customerTypeId, IsAll.Value).ForEach(p => model.ShippingMethodList.Add(new ShippingMethodModel()
                {
                    ShippingMethodId = p.ShippingMethodId,
                    ShippingMethodName = p.ShippingMethodName,
                    WeightOrVolume = p.WeightOrVolume,
                    HaveTrackingNum = p.HaveTrackingNum,
                    IsHideTrackingNumber = p.IsHideTrackingNumber
                }));
            }
            else if (type == 2)
            {
                _freightService.GetShippingMethodListByVenderCode(venderCode, shippingMethodType ?? 0, IsAll.Value).ForEach(p => model.ShippingMethodList.Add(new ShippingMethodModel()
                {
                    ShippingMethodId = p.ShippingMethodId,
                    ShippingMethodName = p.ShippingMethodName,
                    WeightOrVolume = p.WeightOrVolume,
                    HaveTrackingNum = p.HaveTrackingNum,
                    IsHideTrackingNumber = p.IsHideTrackingNumber
                }));
            }
            model.CustomerTypeId = customerTypeId ?? 0;
            model.VenderCode = venderCode;
            model.SelectType = type;
            model.CustomerId = customerId;
            return model;
        }

        public JsonResult GetSelectShippingMethod(string keyWord, int CustomerTypeId, string VenderCode, int type, bool? IsAll, string customerId = "")
        {
            IsAll = IsAll ?? false;
            var shippingMethodModels = new List<ShippingMethodModel>();
            if (type == 1)
            {
                _freightService.GetShippingMethodListByCustomerTypeId(customerId, CustomerTypeId, IsAll.Value).ForEach(p => shippingMethodModels.Add(new ShippingMethodModel()
                {
                    ShippingMethodId = p.ShippingMethodId,
                    ShippingMethodName = p.ShippingMethodName,
                    WeightOrVolume = p.WeightOrVolume,
                    HaveTrackingNum = p.HaveTrackingNum,
                    IsHideTrackingNumber = p.IsHideTrackingNumber
                }));
            }
            else if (type == 2)
            {
                _freightService.GetShippingMethodListByVenderCode(VenderCode, IsAll.Value).ForEach(p => shippingMethodModels.Add(new ShippingMethodModel()
                {
                    ShippingMethodId = p.ShippingMethodId,
                    ShippingMethodName = p.ShippingMethodName,
                    WeightOrVolume = p.WeightOrVolume,
                    HaveTrackingNum = p.HaveTrackingNum,
                    IsHideTrackingNumber = p.IsHideTrackingNumber
                }));
            }
            var list = shippingMethodModels.WhereIf(p => p.ShippingMethodName.ToUpperInvariant().Contains(keyWord.ToUpperInvariant()), !string.IsNullOrWhiteSpace(keyWord));
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// CS版  获取客户运输方式 add by huhaiyou 2014-4-23 
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="CustomerTypeId"></param>
        /// <param name="VenderCode"></param>
        /// <param name="type"></param>
        /// <param name="IsAll"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public List<ShippingMethodModel> GetSelectShippingMethods(string keyWord, int CustomerTypeId, string VenderCode, int type, bool? IsAll, string customerId = "")
        {
            IsAll = IsAll ?? false;
            var shippingMethodModels = new List<ShippingMethodModel>();
            if (type == 1)
            {
                _freightService.GetShippingMethodListByCustomerTypeId(customerId, CustomerTypeId, IsAll.Value).ForEach(p => shippingMethodModels.Add(new ShippingMethodModel()
                {
                    ShippingMethodId = p.ShippingMethodId,
                    ShippingMethodName = p.ShippingMethodName,
                    WeightOrVolume = p.WeightOrVolume,
                    HaveTrackingNum = p.HaveTrackingNum
                }));
            }
            else if (type == 2)
            {
                _freightService.GetShippingMethodListByVenderCode(VenderCode, IsAll.Value).ForEach(p => shippingMethodModels.Add(new ShippingMethodModel()
                {
                    ShippingMethodId = p.ShippingMethodId,
                    ShippingMethodName = p.ShippingMethodName,
                    WeightOrVolume = p.WeightOrVolume,
                    HaveTrackingNum = p.HaveTrackingNum
                }));
            }
            var list = shippingMethodModels.WhereIf(p => p.ShippingMethodName.ToUpperInvariant().Contains(keyWord.ToUpperInvariant()), !string.IsNullOrWhiteSpace(keyWord));
            return list.ToList();
        }

        /// <summary>
        /// 从缓存中获取运输方式支持的国家
        /// </summary>
        private List<ShippingMethodCountryModel> GetShippingMethodCountriesFromCache(int shippingMethodId)
        {
            object inCache = Cache.Get(shippingMethodId.ToString());

            if (inCache != null)
            {
                var listShippingMethodCountryModel = inCache as List<ShippingMethodCountryModel>;

                if (listShippingMethodCountryModel != null) return listShippingMethodCountryModel;
            }
            var listShippingMethodCountryModelNewest = _freightService.GetCountryArea(shippingMethodId);

            if (listShippingMethodCountryModelNewest != null)
            {
                Cache.Add(shippingMethodId.ToString(), listShippingMethodCountryModelNewest, 5);
            }

            return listShippingMethodCountryModelNewest;
        }

        /// <summary>
        /// 从缓存中国家列表
        /// </summary>
        private List<Country> GetGetCountryListFromCache()
        {
            const string key = "List_Country";

            object inCache = Cache.Get(key);

            if (inCache != null)
            {
                var listCountry = inCache as List<Country>;

                if (listCountry != null) return listCountry;
            }

            var listCountryNewest = _freightService.GetCountryList();

            if (listCountryNewest != null)
            {
                Cache.Add(key, listCountryNewest, 60);
            }

            return listCountryNewest;
        }

        /// <summary>
        /// 从缓存用户列表
        /// </summary>
        private List<Customer> GetGetCustomerListFromCache()
        {
            const string key = "List_Customer";

            object inCache = Cache.Get(key);

            if (inCache != null)
            {
                var listCustomer = inCache as List<Customer>;

                if (listCustomer != null) return listCustomer;
            }

            var listCustomerNewest = _customerService.GetCustomerList("", false);

            if (listCustomerNewest != null)
            {
                Cache.Add(key, listCustomerNewest, 60);
            }

            return listCustomerNewest;
        }

        public JsonResult CheckOnWayBill(InStorageFormModel filter)
        {
            var model = new ResponseResult();
            var error = new StringBuilder();

            if (string.IsNullOrWhiteSpace(filter.CustomerCode))
            {
                model.Result = false;
                model.Message = "客户为空！";
            }
            else
            {
                var NumberStr = string.Empty;
                if (!string.IsNullOrWhiteSpace(filter.WayBillNumber))
                {
                    NumberStr = filter.WayBillNumber.Trim();
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(filter.TrackingNumber))
                    {
                        NumberStr = filter.TrackingNumber.Trim();
                    }
                }
                if (string.IsNullOrWhiteSpace(NumberStr))
                {
                    model.Result = false;
                    model.Message = "单号为空！";
                }
                else
                {
                    Stopwatch stopwatch = new Stopwatch();//检测时间
                    stopwatch.Start();//检测时间

                    var waybillinfo = _wayBillInfoRepository.GetWayBillInfoExtSilm(NumberStr);

                    stopwatch.Stop();//检测时间
                    Log.Info(string.Format("入仓扫描单号查找运单耗时:{0}", stopwatch.ElapsedMilliseconds));//检测时间

                    if (waybillinfo != null)
                    {
                        if (WayBill.StatusToValue(WayBill.StatusEnum.Submitted) != waybillinfo.Status)
                        {
                            error.AppendLine(string.Format("运单:{0}状态为{1}！<br/>", NumberStr, WayBill.GetStatusDescription(waybillinfo.Status)));
                        }
                        if (error.Length == 0 && waybillinfo.GoodsTypeID != filter.GoodsTypeID)
                        {
                            error.AppendLine(string.Format("运单：{0}货物类型不是一致", NumberStr));
                        }
                        if (error.Length == 0 && !string.IsNullOrWhiteSpace(filter.TrackingNumber))
                        {
                            var w = _wayBillInfoRepository.GetWayBillByTrackingNumber(filter.TrackingNumber.Trim());
                            if (w != null && w.WayBillNumber != waybillinfo.WayBillNumber)
                            {
                                error.AppendLine(string.Format("跟踪号：{0}系统已经存在", filter.TrackingNumber));
                            }
                        }
                        if (error.Length == 0 && waybillinfo.IsHold)
                        {
                            error.AppendLine(string.Format("运单:{0}已经Hold！<br/>", NumberStr));
                        }
                        if (error.Length == 0 && waybillinfo.InShippingMethodID.HasValue &&
                            waybillinfo.InShippingMethodID.Value != filter.ShippingMethodId)
                        {
                            error.AppendLine(string.Format("入仓运输方式与运单运输方式：{0}不一致！<br/>",
                                                           waybillinfo.InShippingMethodName));
                            //_orderService.AddAbnormalWayBill(waybillinfo.WayBillNumber,
                            //WayBill.AbnormalTypeEnum.InAbnormal,
                            //string.Format("运单号：{0}国家或运输方式异常", waybillinfo.WayBillNumber));
                        }
                        if (error.Length == 0 && waybillinfo.CustomerCode.ToUpper() != filter.CustomerCode.ToUpper())
                        {
                            error.AppendLine(string.Format("入仓客户与运单客户:{0}不一致！<br/>", waybillinfo.CustomerCode));
                            //_orderService.AddAbnormalWayBill(waybillinfo.WayBillNumber,
                            //                                 WayBill.AbnormalTypeEnum.InAbnormal,
                            //                                 string.Format("入仓客户与运单客户:{0}不一致", waybillinfo.CustomerCode));
                        }
                        if (error.Length > 0)
                        {
                            model.Result = false;
                            model.Message = error.ToString();
                        }
                        else
                        {
                            stopwatch.Restart();//检测时间

                            var chineseName = GetGetCountryListFromCache().Find(p => p.CountryCode == waybillinfo.CountryCode).ChineseName;

                            stopwatch.Stop();//检测时间
                            Log.Info(string.Format("入仓扫描单号获取国家中文名耗时:{0}", stopwatch.ElapsedMilliseconds));//检测时间

                            //var areaIdList =
                            //    _freightService.GetCountryArea(filter.ShippingMethodId, waybillinfo.CountryCode)
                            //                   .FirstOrDefault();

                            stopwatch.Restart();//检测时间

                            var listShippingMethodCountryModel = GetShippingMethodCountriesFromCache(filter.ShippingMethodId);
                            var areaIdList = listShippingMethodCountryModel.Find(p => p.CountryCode == waybillinfo.CountryCode);

                            stopwatch.Stop();//检测时间
                            Log.Info(string.Format("入仓扫描单号获取运输方式国家耗时:{0}", stopwatch.ElapsedMilliseconds));//检测时间

                            if (areaIdList != null)
                            {
                                model.Result = true;
                                model.Message = chineseName + "(" + areaIdList.AreaId + "区)";
                            }
                            else
                            {
                                //_orderService.AddAbnormalWayBill(waybillinfo.WayBillNumber,
                                //                                 WayBill.AbnormalTypeEnum.InAbnormal,
                                //                                 string.Format("该运输方式无法送达该目的地国家{0} ", chineseName));
                                model.Result = false;
                                model.Message = string.Format("该运输方式无法送达该目的地国家{0} ", chineseName);
                            }
                        }
                        model.TrackingNumber = waybillinfo.TrackingNumber;

                        //返回运单的重量weight
                        model.ErrorWayBillNumber = waybillinfo.Weight.HasValue ? waybillinfo.Weight.ToString() : "0";

                    }
                    else
                    {
                        NoForecastAbnormal noForecastAbnormal = new NoForecastAbnormal()
                            {
                                CustomerCode = filter.CustomerCode,
                                Number = NumberStr,
                                ShippingMethodId = filter.ShippingMethodId,
                                Weight = filter.Weight,
                                CreatedOn = DateTime.Now,
                                CreatedBy = _workContext.User.UserUame,
                                LastUpdatedOn = DateTime.Now,
                                LastUpdatedBy = _workContext.User.UserUame,
                            };
                        _orderService.UpdateNoForecastAbnormal(noForecastAbnormal);

                        model.Result = false;
                        model.Message = string.Format("单号：{0}无预报，不能扫描入仓！", NumberStr);
                    }
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }



        private CustomerOrderInfosModel GetPrinterInfo(int id)
        {

            try
            {
                var entity = _customerOrderService.Print(id);

                if (entity == null) return null;
                var model = entity.ToModel<CustomerOrderInfosModel>();
                if (string.IsNullOrWhiteSpace(model.TrackingNumber))
                {
                    var firstOrDefault = entity.WayBillInfos.FirstOrDefault();
                    if (firstOrDefault != null)
                        model.TrackingNumber = firstOrDefault.WayBillNumber;
                }
                model.BarCode = "<img id=\"img\" src=\"/barcode.ashx?m=0&h=35&vCode=" + model.TrackingNumber + "\" alt=\"" + model.TrackingNumber + "\" style=\"width:200px;height:35px;\" />";

                entity.ShippingInfo.ToModel(model);

                //if (entity.ShippingInfoID != null && entity.ShippingMethodId.HasValue)
                //{
                //    List<ShippingMethodCountryModel> shippingMethod = _freightService.GetCountryArea(entity.ShippingMethodId.Value, entity.ShippingInfo.CountryCode);
                //    if (shippingMethod != null && shippingMethod.Count > 0)
                //        model.ShippingZone = shippingMethod.First().AreaId;
                //}
                var country = _countryService.GetCountryList("").Single(c => c.CountryCode == entity.ShippingInfo.CountryCode);

                model.CountryName = country.Name;
                model.CountryChineseName = country.ChineseName;
                return model;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return null;
            }
        }

        /// <summary>
        /// 拦截（运单管理）
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        public JsonResult InterceptWayBill(string wayBillNumber)
        {
            var model = new ResponseResult();
            try
            {
                _orderService.HoldWayBillInfo(wayBillNumber);

                //#region 操作日志
                ////yungchu
                ////敏感字-无
                //BizLog bizlog = new BizLog()
                //{
                //	Summary = "运单拦截",
                //	KeywordType = KeywordType.WayBillNumber,
                //	Keyword = wayBillNumber,
                //	UserCode = _workContext.User.UserUame,
                //	UserRealName = _workContext.User.UserUame,
                //	UserType = UserType.LMS_User,
                //	SystemCode = SystemType.LMS,
                //	ModuleName = "运单修改"
                //};
                // WayBillInfoExtSilm wayBillInfoExtSilm=new WayBillInfoExtSilm();
                //_wayBillInfoRepository.First(p => p.WayBillNumber == wayBillNumber).CopyTo(wayBillInfoExtSilm);

                //_operateLogServices.WriteLog(bizlog, wayBillInfoExtSilm);
                //#endregion

                model.Result = true;
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
        /// 批量拦截(运单管理)
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <returns></returns>
        [ButtonPermissionValidator(PermissionRecords.BatchHoldOn)]
        public JsonResult BatchInterceptWayBill(string wayBillNumbers)
        {
            var model = new ResponseResult();
            try
            {
                if (!string.IsNullOrWhiteSpace(wayBillNumbers))
                {
                    var arr = wayBillNumbers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var wayBillNumberList = arr.ToList();
                    _orderService.BatchHoldWayBillInfo(wayBillNumberList);
                    model.Result = true;
                }
                else
                {
                    model.Result = false;
                    model.Message = "请选择需要的拦截的运单";
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


        public JsonResult DeleteAbnormalWayBill(string wayBillNumber)
        {
            var model = new ResponseResult();
            try
            {
                _orderService.DeleteWayBillInfo(wayBillNumber);
                model.Result = true;
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
        /// 批量删除异常运单
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <returns></returns>
        [ButtonPermissionValidator(PermissionRecords.BatchDeleteAbnormalWayBill)]
        public JsonResult BatchDeleteAbnormalWayBill(string wayBillNumbers)
        {
            return BateDelete(wayBillNumbers);
        }

        private JsonResult BateDelete(string wayBillNumbers)
        {
            var model = new ResponseResult();
            try
            {
                if (!string.IsNullOrWhiteSpace(wayBillNumbers))
                {
                    var arr = wayBillNumbers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var wayBillNumberList = arr.ToList();
                    _orderService.BatchDeleteWayBillInfo(wayBillNumberList);
                    model.Result = true;
                }
                else
                {
                    model.Result = false;
                    model.Message = "请选择需要的删除的运单";
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
        /// 批量删除(运单管理)
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <returns></returns>
        [ButtonPermissionValidator(PermissionRecords.BatchDelete)]
        public JsonResult BatchDeleteWayBill(string wayBillNumbers)
        {
            return BateDelete(wayBillNumbers);
        }


        public JsonResult CancelAbnormalWayBill(string wayBillNumber)
        {
            var model = new ResponseResult();
            try
            {
                _orderService.CancelHoldWayBillInfo(wayBillNumber);
                model.Result = true;
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
        /// 批量取消异常运单
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <returns></returns>
        [ButtonPermissionValidator(PermissionRecords.BatchCancelAbnormalWayBill)]
        public JsonResult BatchCancelAbnormalWayBill(string wayBillNumbers)
        {

            var model = new ResponseResult();
            try
            {
                if (!string.IsNullOrWhiteSpace(wayBillNumbers))
                {
                    var arr = wayBillNumbers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var wayBillNumberList = arr.ToList();
                    _orderService.BatchCancelHoldWayBillInfo(wayBillNumberList);
                    model.Result = true;
                }
                else
                {
                    model.Result = true;
                    model.Message = "请选择需要的取消异常运单";
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
        /// 批量退回异常运单
        /// yungchu
        /// </summary>
        /// <returns></returns>
        [ButtonPermissionValidator(PermissionRecords.BatchDeleteAbnormalWayBill)]
        public JsonResult BatchReturnAbnormalWayBill(string wayBillNumbers)
        {
            var model = new ResponseResult();
            try
            {
                if (!string.IsNullOrWhiteSpace(wayBillNumbers))
                {
                    var arr = wayBillNumbers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var wayBillNumberList = arr.ToList();
                    _orderService.BatchReturnWayBillInfo(wayBillNumberList);
                    model.Result = true;
                }
                else
                {
                    model.Result = false;
                    model.Message = "请选择需要退回的异常运单";
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


        public JsonResult SaveWayBill(string WayBillNumber, int ShippingMethodId, string TrackingNumber, string ShippingMethodName)
        {
            var model = new ResponseResult();

            try
            {

                //#region 操作日志
                ////yungchu
                ////敏感字-无
                //StringBuilder sb = new StringBuilder();
                //sb.Append("");
                //WayBillInfo waybillinfo = _wayBillInfoRepository.First(a => a.WayBillNumber == WayBillNumber);
                //if (waybillinfo.InShippingMethodID != ShippingMethodId)
                //{
                //	sb.AppendFormat(" 运输方式从{0}更改为{1}", waybillinfo.InShippingMethodName, ShippingMethodName);
                //}
                //if (!string.IsNullOrEmpty(TrackingNumber)&& waybillinfo.TrackingNumber != TrackingNumber)
                //{
                //	sb.AppendFormat(" 跟踪号从{0}更改为{1}", waybillinfo.TrackingNumber, TrackingNumber);
                //}


                //BizLog bizlog = new BizLog()
                //{
                //	Summary = sb.ToString() != "" ? "[运单修改]" + sb : "运单修改",
                //	KeywordType = KeywordType.WayBillNumber,
                //	Keyword = WayBillNumber,
                //	UserCode = _workContext.User.UserUame,
                //	UserRealName = _workContext.User.UserUame,
                //	UserType = UserType.LMS_User,
                //	SystemCode = SystemType.LMS,
                //	ModuleName = "运单修改"
                //};

                //WayBillInfoExtSilm wayBillInfoExtSilm = new WayBillInfoExtSilm();
                //waybillinfo.CopyTo(wayBillInfoExtSilm);

                //_operateLogServices.WriteLog(bizlog, wayBillInfoExtSilm);
                //#endregion

                _orderService.UpdateWayBillInfo(new WayBillInfo()
                    {
                        WayBillNumber = WayBillNumber,
                        InShippingMethodID = ShippingMethodId,
                        InShippingMethodName = ShippingMethodName,
                        TrackingNumber = TrackingNumber,
                    });
                model.Result = true;
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
        /// 批量修改运输方式
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <param name="shippingMethodId">运输方式名ID</param>
        /// <param name="shippingMethodName">运输方式名称</param>
        /// <returns></returns>
        [ButtonPermissionValidator(PermissionRecords.BatchModifyShippingMethod)]
        public JsonResult BatchModifyShippingMethod(string wayBillNumbers, int? shippingMethodId, string shippingMethodName)
        {

            var model = new ResponseResult();
            try
            {
                if (!string.IsNullOrWhiteSpace(wayBillNumbers))
                {
                    var arr = wayBillNumbers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var wayBillNumberList = arr.ToList();
                    _orderService.BatchUpdateWayBillInfo(wayBillNumberList, shippingMethodId, shippingMethodName);
                    model.Result = true;
                }
                else
                {
                    model.Result = true;
                    model.Message = "请选择需要修改运输方式的运单";
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

        public JsonResult CheckOnInStorage(InStorageFormModel filter)
        {
            return Json(CheckOnInStorageWork(filter), JsonRequestBehavior.AllowGet);
        }

        public InStorageWayBillModel CheckOnInStorageWork(InStorageFormModel filter)
        {
            var model = new InStorageWayBillModel();
            var error = new StringBuilder();
            var NumberStr = string.Empty;
            if (!string.IsNullOrWhiteSpace(filter.WayBillNumber))
            {
                NumberStr = filter.WayBillNumber;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(filter.TrackingNumber))
                {
                    NumberStr = filter.TrackingNumber;
                }
            }
            var waybillinfo = _wayBillInfoRepository.GetWayBillInfoExtSilm(NumberStr);
            if (!string.IsNullOrWhiteSpace(NumberStr) && waybillinfo != null)
            {
                //if (!string.IsNullOrWhiteSpace(filter.TrackingNumber) && waybillinfo.TrackingNumber != filter.TrackingNumber)
                //{
                //    error.AppendLine("跟踪号与输入的跟踪号不一样！<br/>");
                //}
                if (error.Length == 0 && WayBill.StatusToValue(WayBill.StatusEnum.Submitted) != waybillinfo.Status)
                {
                    error.AppendLine(string.Format("运单:{0}状态不是已提交！<br/>", NumberStr));
                }
                if (error.Length == 0 && waybillinfo.GoodsTypeID != filter.GoodsTypeID)
                {
                    error.AppendLine(string.Format("运单：{0}货物类型不是一致", NumberStr));
                }

                if (!filter.IsWeightAbnormalWaybill)//重量异常单不显示提示
                {
                    if (error.Length == 0 && waybillinfo.IsHold)
                    {
                        error.AppendLine(string.Format("运单:{0}已经Hold！<br/>", NumberStr));
                    }
                }

                if (error.Length == 0 && waybillinfo.InShippingMethodID.HasValue &&
                    waybillinfo.InShippingMethodID.Value != filter.ShippingMethodId)
                {
                    error.AppendLine(string.Format("入仓运输方式与运单运输方式：{0}不一致！<br/>", waybillinfo.InShippingMethodName));
                    //_orderService.AddAbnormalWayBill(waybillinfo.WayBillNumber, WayBill.AbnormalTypeEnum.InAbnormal, string.Format("运单号：{0}国家或运输方式异常", waybillinfo.WayBillNumber));
                }
                if (error.Length == 0 && waybillinfo.CustomerCode.ToUpper().Trim() != filter.CustomerCode.ToUpper().Trim())
                {
                    error.AppendLine(string.Format("入仓客户与运单客户:{0}不一致！<br/>", waybillinfo.CustomerCode));
                    //_orderService.AddAbnormalWayBill(waybillinfo.WayBillNumber, WayBill.AbnormalTypeEnum.InAbnormal, string.Format("入仓客户与运单客户:{0}不一致", waybillinfo.CustomerCode.ToUpper()));
                }
                if (error.Length == 0&&!filter.IsWeightAbnormalWaybill)
                {
                    //获取配置的重量相差值
                    var configurationValue = GetWeightDeviations(filter.CustomerCode, filter.ShippingMethodId);
                    //实际相差值
                    var deviationValue = Math.Abs((waybillinfo.Weight ?? 0) - filter.Weight)*1000;
                    //比较
                    if (configurationValue != 0 && deviationValue > configurationValue)
                    {
                        error.AppendLine(string.Format("称重重量与预报重量相差{0}g大于配置的{1}g不能入仓！<br/>", deviationValue,
                                                       configurationValue));
                        try
                        {
                            AddInStorageWeightAbnormal(new InStorageWeightAbnormalParm
                            {
                                WayBillInfoExtSilm = waybillinfo,
                                Weight = filter.Weight,
                                Length = filter.Length,
                                Width = filter.Width,
                                Height = filter.Height,
                                DeviationValue = deviationValue,
                                ConfigurationValue = configurationValue,
                            });
                        }
                        catch (Exception ex)
                        {
                            error.AppendLine(ex.Message);
                            Log.Exception(ex);
                        }
                    }
                }
                if (error.Length > 0)
                {
                    model.IsSuccess = false;
                    model.Message = error.ToString();
                }
                else
                {
                    // 根据转换重量转换包裹类型 Add by zhengsong 
                    #region
                    var shippingMethod = _freightService.GetShippingMethod(filter.ShippingMethodId);
                    if (shippingMethod != null)
                    {
                        if (shippingMethod.Enabled && shippingMethod.ShippingMethodTypeId == 4)
                        {
                            if (filter.Weight <= shippingMethod.PackageTransformFileWeight)
                            {
                                filter.GoodsTypeID = 2;
                            }
                        }
                    }
                    #endregion

                    Guid customerId = Guid.Empty;
                    Customer customer = GetGetCustomerListFromCache().Find(p => p.CustomerCode == waybillinfo.CustomerCode);
                    if (customer != null)
                        customerId = customer.CustomerID;
                    var result = _freightService.GetCustomerShippingPrice(new CustomerInfoPackageModel()
                    {
                        CountryCode = waybillinfo.CountryCode,
                        CustomerTypeId = filter.CustomerType,
                        Height = filter.Height ?? 0,
                        Length = filter.Length ?? 0,
                        ShippingMethodId = filter.ShippingMethodId,
                        Weight = filter.Weight,
                        Width = filter.Width ?? 0,
                        ShippingTypeId = filter.GoodsTypeID,
                        CustomerId = customerId,
                        EnableTariffPrepay = waybillinfo.EnableTariffPrepay,
                    });


                    if (result.CanShipping)
                    {
                        model.IsSuccess = true;
                        model.Message = "";
                        model.WayBillNumber = waybillinfo.WayBillNumber;
                        model.CountryCode = waybillinfo.CountryCode;
                        model.CountryName = _freightService.GetChineseName(waybillinfo.CountryCode);
                        model.CustomerOrderNumber = waybillinfo.CustomerOrderNumber;
                        model.Freight = result.ShippingFee; //运费
                        model.FuelCharge = result.FuelFee; //燃油费
                        model.Register = result.RegistrationFee; //挂号费
                        model.Surcharge = result.Value - (result.ShippingFee + result.FuelFee + result.RegistrationFee + result.TariffPrepayFee); //附加费
                        model.TariffPrepay = result.TariffPrepayFee;//关税预付服务费
                        model.SettleWeight = result.Weight; //结算重量
                        model.Weight = filter.Weight;
                        model.Width = filter.Width ?? 0;
                        model.Height = filter.Height ?? 0;
                        model.Length = filter.Length ?? 0;
                        //if (!string.IsNullOrWhiteSpace(filter.TrackingNumber) &&
                        //    filter.TrackingNumber != waybillinfo.TrackingNumber)
                        //{
                        //    model.TrackingNumber = filter.TrackingNumber;
                        //}
                        //else
                        //{
                        //    model.TrackingNumber = !string.IsNullOrWhiteSpace(waybillinfo.TrackingNumber) ? waybillinfo.TrackingNumber : "";
                        //}
                        model.TrackingNumber = !string.IsNullOrWhiteSpace(waybillinfo.TrackingNumber) ? waybillinfo.TrackingNumber : "";

                        if (filter.ChkPrint)
                        {
                            var customerOrder = GetPrinterInfo(waybillinfo.CustomerOrderID);
                            if (customerOrder != null)
                            {
                                var wayBillTemplateModel = _wayBillTemplateService.GetWayBillTemplateByNameAndShippingMethod(filter.PrintTemplateName, filter.ShippingMethodId).FirstOrDefault();
                                if (wayBillTemplateModel != null)
                                {
                                    //model.HtmlString = this.RenderPartialViewToString("_PrintTNTOrder", customerOrderInfosModels);
                                    if (!string.IsNullOrWhiteSpace(customerOrder.TrackingNumber))
                                    {
                                        model.HtmlString =
                                            Razor.Parse(HttpUtility.HtmlDecode(wayBillTemplateModel.TemplateContent),
                                                        customerOrder);
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        // _orderService.AddAbnormalWayBill(waybillinfo.WayBillNumber, WayBill.AbnormalTypeEnum.InAbnormal, result.Message);  //入仓扫描时取消因重量价格不对自动拦截运单  by:bookers  date:2013.10.25
                        model.IsSuccess = false;
                        model.Message = result.Message;
                    }
                }
            }
            else
            {
                model.IsSuccess = false;
                model.Message = string.Format("单号：{0}系统找不到！", NumberStr);
            }
            return model;
        }

        //获取入仓重量配置值 yungchu
        public decimal GetWeightDeviations(string customerCode, int shippingMethodId)
        {
            InStorageWeightDeviation getWeightDeviation = _inStorageService.GetInStorageWeightCompareDeviationValue(customerCode, shippingMethodId);
            return getWeightDeviation != null ? (getWeightDeviation.DeviationValue.HasValue?getWeightDeviation.DeviationValue.Value:0) : 0;
        }

        public JsonResult CheckOnOutStorage(OutStorageFormModel filter)
        {
            var model = new OutStorageWayBillModel();
            var error = new StringBuilder();
            //var waybillinfo = _wayBillInfoRepository.GetWayBill(filter.WayBillNumber.Trim()).ToModel<OutWayBillModel>();
            int status = WayBill.StatusToValue(WayBill.StatusEnum.Have);
            var waybillinfo =
                _wayBillInfoRepository.Single(
                    w =>
                    (w.WayBillNumber == filter.WayBillNumber.Trim() || w.TrackingNumber == filter.WayBillNumber.Trim() ||
                     w.CustomerOrderNumber == filter.WayBillNumber.Trim()) &&
                    w.Status == status).ToModel<OutWayBillModel>();
            if (!string.IsNullOrWhiteSpace(filter.WayBillNumber) && waybillinfo != null)
            {
                if (WayBill.StatusToValue(WayBill.StatusEnum.Have) != waybillinfo.Status)
                {
                    error.AppendLine(string.Format("输入:{0}状态不是已收货！<br/>", filter.WayBillNumber));
                }
                if (error.Length == 0 && waybillinfo.IsHold)
                {
                    error.AppendLine(string.Format("输入：{0}已经Hold！<br/>", filter.WayBillNumber));
                }

                if (error.Length == 0 && !string.IsNullOrWhiteSpace(filter.CountryCode) && filter.CountryCode != waybillinfo.CountryCode)
                {
                    error.AppendLine(string.Format("输入：{0}目的国家为{1}！<br/>",filter.WayBillNumber, waybillinfo.CountryCode));
                }

                //Add By zhengsong
                //判断是否是指定的运输渠道
                var vender = _freightService.GetVender(filter.VenderCode);
                int venderId = 0;
                if (vender != null)
                {
                    venderId = vender.VenderId;
                }
                var outStorageShippinMethod = _outStorageService.GetDeliveryChannelConfiguration(waybillinfo.InShippingMethodID ?? 0, venderId, filter.ShippingMethodId);
                if (outStorageShippinMethod == null)
                {
                    model.IsSuccess = false;
                    model.Message = string.Format("[{0}]运输方式，与该出仓渠道不匹配，不允许出仓！", waybillinfo.InShippingMethodName);
                    return Json(model, JsonRequestBehavior.AllowGet);
                }

                //现结客户货款未结清，不允许出库
                //var customer = _customerService.GetCustomer(waybillinfo.CustomerCode);
                //if (customer.PaymentTypeID == 3 || customer.PaymentTypeID == 4)
                //{
                //    var customerBalance = _customerService.GetCustomerBalance(waybillinfo.CustomerCode);
                //    decimal money = 0;
                //    if (customer.EnableCredit)
                //    {
                //        money = (customerBalance.Balance ?? 0) + customer.MaxDelinquentAmounts;
                //    }
                //    else
                //    {
                //        money = customerBalance.Balance ?? 0;
                //    }
                //    if (money < 0)
                //    {
                //        model.IsSuccess = false;
                //        model.Message = string.Format("[{0}]现结客户货款未结清，不允许出库!", customer.Name);
                //        return Json(model, JsonRequestBehavior.AllowGet);
                //    }
                //}

                //if (waybillinfo.GoodsTypeID != filter.GoodsTypeID)
                //{
                //    error.AppendLine("该运单货物类型与系统不一样！<br/>");
                //}
                if (error.Length == 0)
                {
                    var areaIdList =
                        _freightService.GetCountryArea(filter.ShippingMethodId, waybillinfo.CountryCode)
                                       .FirstOrDefault();
                    var chineseName = _freightService.GetChineseName(waybillinfo.CountryCode);
                    if (areaIdList != null)
                    {
                        model.Message = chineseName + "(" + areaIdList.AreaId + "区)";
                    }
                    else
                    {
                        _orderService.AddAbnormalWayBill(waybillinfo.WayBillNumber, WayBill.AbnormalTypeEnum.OutAbnormal,
                                                         string.Format("该运输方式无法送达该目的地国家{0} ", chineseName));

                        error.AppendLine(string.Format("该运输方式无法送达该目的地国家{0}<br/> ", chineseName));
                    }
                }
                if (error.Length > 0)
                {
                    model.IsSuccess = false;
                    model.Message = error.ToString();
                }
                else
                {
                    var result = _freightService.GetVenderShippingPrice(new VenderPackageModel()
                    {
                        CountryCode = waybillinfo.CountryCode,
                        Code = filter.VenderCode,
                        Height = waybillinfo.Height ?? 0,
                        Length = waybillinfo.Length ?? 0,
                        ShippingMethodId = filter.ShippingMethodId,
                        Weight = waybillinfo.Weight ?? 0,
                        Width = waybillinfo.Width ?? 0,
                        ShippingTypeId = filter.GoodsTypeID
                    });
                    if (result.CanShipping)
                    {
                        model.IsSuccess = true;
                        model.WayBillNumber = waybillinfo.WayBillNumber;
                        model.CountryCode = waybillinfo.CountryCode;
                        model.CustomerOrderNumber = waybillinfo.CustomerOrderNumber;
                        model.Freight = result.ShippingFee; //运费
                        model.FuelCharge = result.FuelFee; //燃油费
                        model.Register = result.RegistrationFee; //挂号费
                        model.Surcharge = result.RemoteAreaFee + result.OtherFee; //附加费
                        model.SettleWeight = result.Weight; //结算重
                        model.Weight = waybillinfo.Weight.Value; //结算重量
                        model.TrackingNumber = waybillinfo.TrackingNumber;
                    }
                    else
                    {
                        //_orderService.AddAbnormalWayBill(waybillinfo.WayBillNumber, WayBill.AbnormalTypeEnum.OutAbnormal, result.Message);  //出仓扫描时取消因重量价格不对自动拦截运单  by:bookers  date:2013.10.25
                        model.IsSuccess = false;
                        model.Message = result.Message;
                    }
                }
            }
            else
            {
                model.IsSuccess = false;
                model.Message = string.Format("单号:{0}系统找不到！", filter.WayBillNumber);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        //单号扫描判断是否存在
        public JsonResult CheckOnTrackingNumber(string TrackingNumber)
        {
            var model = new ResponseResult();
            var waybillinfo = _inStorageService.GetWayBillInfo(TrackingNumber).ToModel<WayBillInfoModel>();
            model.Result = waybillinfo == null;
            if (!model.Result)
            {
                model.Message = waybillinfo.WayBillNumber + "|" + waybillinfo.TrackingNumber + "|" + waybillinfo.TrueTrackingNumber + "|" +
                                waybillinfo.CustomerOrderNumber;
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveEnableCredit(string customerCode, int shippingMethodId)
        {
            Customer model = new Customer();
            model = _customerService.GetCustomer(customerCode);
            CustomerBalance balance = new CustomerBalance();
            balance = _customerService.GetCustomerBalance(customerCode);
            List<WayBillTemplate> wayBillTemplateModels = _wayBillTemplateService.GetWayBillTemplateByShippingMethod(shippingMethodId);
            string templateName = string.Empty;
            if (wayBillTemplateModels.Count > 0)
            {
                wayBillTemplateModels.ForEach(p => templateName += p.TemplateName + ",");
                templateName = templateName.Remove(templateName.Length - 1);
            }


            DataResult data = new DataResult() { Type = 0, TemplateName = templateName };
            if (model != null)
            {
                if (model.EnableCredit)
                {
                    if (balance != null)
                    {
                        if (-model.MaxDelinquentAmounts >= balance.Balance)
                        {
                            data.Type = 1;
                            return Json(data, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(data, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        data.Type = 2;
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Type = 2;
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        #region 快递打单打印发票

        public ActionResult InvoicePrinter(string ids)
        {
            List<string> wayBillNUmber = new List<string>();
            string[] arr = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            arr.Each(v => wayBillNUmber.Add(v.Trim()));
            var list = _customerOrderService.GetCustomerOrderIdByWayBillNumber(wayBillNUmber);



            string customerOrderIds = "";
            list.ForEach(p =>
                {
                    customerOrderIds += p + ",";
                });
            return View(BindInvoicePrinter(customerOrderIds));
        }

        public InvoivePrinterViewModel BindInvoicePrinter(string ids, string templateName = "")
        {
            InvoivePrinterViewModel viewModel = new InvoivePrinterViewModel
                {
                    Ids = ids
                };
            string[] arr = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length == 0)
            {
                return viewModel;
            }
            List<int> customerOrderIds = new List<int>();
            arr.Each(v => customerOrderIds.Add(v.ConvertTo<int>()));
            var selectList = new List<SelectListItem>();
            var shippingMethodIds = new List<int>();
            var list = GetPrinterList(customerOrderIds).OrderBy(p => p.CustomerOrderID).ToList();
            if (list == null)
            {
                return viewModel;
            }
            //过滤相同的运输方式
            list.ForEach(p =>
            {
                if (shippingMethodIds.Contains(p.ShippingMethodId)) return;
                shippingMethodIds.Add(p.ShippingMethodId);
            });
            IEnumerable<WayBillTemplateExt> wayBillTemplateModelList = new List<WayBillTemplateExt>();
            IEnumerable<WayBillTemplateExt> wayBillTemplateModels = _wayBillTemplateService.GetWayBillTemplateList(shippingMethodIds, "DT1308100023");
            var billTemplateModelList = wayBillTemplateModels as WayBillTemplateExt[] ?? wayBillTemplateModels.ToArray();
            if (billTemplateModelList.Any())
            {
                wayBillTemplateModelList = billTemplateModelList;
                string filter = string.Empty;
                foreach (var wayBillTemplate in billTemplateModelList)
                {
                    string val = wayBillTemplate.TemplateName;
                    var listItem = new SelectListItem()
                    {
                        Value = wayBillTemplate.TemplateName,
                        Text = wayBillTemplate.TemplateName,
                        Selected = val.Equals(templateName)
                    };
                    if (filter.Contains(val)) continue;
                    filter += val + ",";
                    selectList.Add(listItem);
                }
            }

            if (selectList.Count > 0)
            {
                //没有选中模板就默认第一个被选中
                SelectListItem item = selectList.FirstOrDefault(p => p.Selected);
                if (item == null)
                {
                    item = selectList.First();
                }
                templateName = item.Value;
            }

            if (!string.IsNullOrWhiteSpace(templateName))
            {
                wayBillTemplateModelList = _wayBillTemplateService.GetGetWayBillTemplateExtByName(templateName);
            }
            viewModel.SelectList = selectList;
            viewModel.CustomerOrderInfoModels = list;
            viewModel.WayBillTemplates = wayBillTemplateModelList;
            return viewModel;
        }

        /// <summary>
        /// 获取要打印的数据
        /// Add by zhengsong
        /// </summary>
        /// <param name="customerOrderIds"></param>
        /// <returns></returns>
        private List<InvoivePrinterOrderInfoModel> GetPrinterList(IEnumerable<int> customerOrderIds)
        {
            //var cacheList = Cache.Get("cache_customerOrderIds") as int[];
            var orderIds = customerOrderIds as int[] ?? customerOrderIds.ToArray();
            //if (orderIds.Length <= 0) return null;
            //if (null == cacheList)
            //{
            //    Cache.Add("cache_customerOrderIds", orderIds);


            //    var cacheOrderList = GetCustomerOrderListModel(orderIds);

            //    if (cacheOrderList != null)
            //        Cache.Add("cache_customerOrders", cacheOrderList);
            //    return cacheOrderList;
            //}
            //else
            //{
            //    if (Tools.CompareArrContent(orderIds, cacheList))
            //    {
            //        var cacheOrderList = Cache.Get("cache_customerOrders") as List<InvoivePrinterOrderInfoModel>;
            //        if (cacheOrderList == null || cacheOrderList.Count==0)
            //        {
            //            cacheOrderList = GetCustomerOrderListModel(orderIds);
            //            if (cacheOrderList != null)
            //                Cache.Add("cache_customerOrders", cacheOrderList);
            //        }
            //        return cacheOrderList;
            //    }
            //    else
            //    {
            //        var cacheOrderList = GetCustomerOrderListModel(orderIds);
            //        Cache.Add("cache_customerOrderIds", orderIds);
            //        if (cacheOrderList != null)
            //            Cache.Add("cache_customerOrders", cacheOrderList);
            //        return cacheOrderList;
            //    }
            //}

            return GetCustomerOrderListModel(orderIds);
        }

        private List<InvoivePrinterOrderInfoModel> GetCustomerOrderListModel(int[] orderIds)
        {

            try
            {
                //System.Diagnostics.Stopwatch sw = new Stopwatch();
                //sw.Start();
                var list = _customerOrderService.PrintByCustomerOrderIds(orderIds);
                var listModel = new List<InvoivePrinterOrderInfoModel>();
                list.ForEach(p =>
                    {
                        var wayBillInfo = p.WayBillInfos.FirstOrDefault();
                        var ApplicationInfos = new List<ApplicationInfoModel1>();
                        p.ApplicationInfos.ToList().ForEach(m => ApplicationInfos.Add(new ApplicationInfoModel1()
                            {
                                ApplicationID = m.ApplicationID,
                                ApplicationName = m.ApplicationName,
                                HSCode = m.HSCode,
                                PickingName = m.PickingName,
                                Qty = m.Qty ?? 0,
                                Remark = m.Remark,
                                Total = m.Total ?? 0,
                                UnitPrice = m.UnitPrice ?? 0,
                                UnitWeight = m.UnitWeight ?? 0,
                                WayBillNumber = m.WayBillNumber
                            }));
                        listModel.Add(new InvoivePrinterOrderInfoModel()
                            {
                                CustomerOrderID = p.CustomerOrderID,
                                CustomerOrderNumber = p.CustomerOrderNumber,
                                CustomerCode = p.CustomerCode,
                                TrackingNumber = p.TrackingNumber,
                                ShippingMethodId = wayBillInfo.OutShippingMethodID ?? 0,
                                ShippingMethodName = wayBillInfo.OutShippingMethodName,
                                GoodsTypeID = p.GoodsTypeID ?? 0,
                                InsuredID = p.InsuredID ?? 0,
                                IsReturn = p.IsReturn,
                                IsInsured = p.IsInsured,
                                IsBattery = p.IsBattery,
                                IsPrinted = p.IsPrinted,
                                IsHold = p.IsHold,
                                Status = p.Status,
                                CreatedOn = p.CreatedOn,
                                SensitiveTypeID = p.SensitiveTypeID ?? 0,
                                PackageNumber = p.PackageNumber,
                                AppLicationType = p.AppLicationType,
                                Weight = p.Weight,
                                Length = p.Length,
                                Width = p.Width,
                                Height = p.Height,
                                ApplicationInfoList = ApplicationInfos,
                                WayBillInfos = p.WayBillInfos.ToList(),
                                ShippingAddress = p.ShippingInfo.ShippingAddress,
                                ShippingCity = p.ShippingInfo.ShippingCity,
                                ShippingCompany = p.ShippingInfo.ShippingCompany,
                                ShippingFirstLastName =
                                    p.ShippingInfo.ShippingFirstName + " " + p.ShippingInfo.ShippingLastName,
                                ShippingFirstName = p.ShippingInfo.ShippingFirstName,
                                ShippingLastName = p.ShippingInfo.ShippingLastName,
                                ShippingPhone = p.ShippingInfo.ShippingPhone,
                                ShippingState = p.ShippingInfo.ShippingState,
                                ShippingZip = p.ShippingInfo.ShippingZip,
                                ShippingTaxId = p.ShippingInfo.ShippingTaxId,
                                CountryCode = p.ShippingInfo.CountryCode,

                                ShippingZone =
                                    GetShippingZone(p.ShippingMethodId ?? 0, p.ShippingInfo.ShippingZip,
                                                    p.ShippingInfo.CountryCode)

                            });

                        //#region 操作日志
                        ////yungchu
                        ////敏感字-无
                        //BizLog bizlog = new BizLog()
                        //{
                        //	Summary = "打印发票",
                        //	KeywordType = KeywordType.CustomerOrderNumber,
                        //	Keyword = p.CustomerOrderNumber,
                        //	UserCode = _workContext.User.UserUame,
                        //	UserRealName = _workContext.User.UserUame,
                        //	UserType = UserType.LMS_User,
                        //	SystemCode = SystemType.LMS,
                        //	ModuleName = "打印发票"
                        //};

                        //_operateLogServices.WriteLog(bizlog, listModel);
                        //#endregion

                    }
                    );
                listModel.ForEach(p =>
                    {

                        if (string.IsNullOrWhiteSpace(p.TrackingNumber))
                        {
                            var firstOrDefault = p.WayBillInfos.FirstOrDefault();
                            if (firstOrDefault != null)
                                p.TrackingNumber = firstOrDefault.WayBillNumber;
                        }
                        p.BarCode = "<img id=\"img\" src=\"/barcode.ashx?m=0&h=35&vCode=" + p.TrackingNumber +
                                    "\" alt=\"" +
                                    p.TrackingNumber + "\" style=\"width:200px;height:35px;\" />";
                        //var entity = list.Find(m => m.CustomerOrderID.Equals(p.CustomerOrderID));
                        //entity.ShippingInfo.ToModel(p);
                        var country = _countryService.GetCountryByCode(p.CountryCode);
                        p.CountryName = country.Name;
                        p.CountryChineseName = country.ChineseName;

                    });
                //sw.Stop();
                //double time = sw.Elapsed.TotalMilliseconds;
                return listModel;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return null;
            }
        }

        /// <summary>
        /// 获取收件人的所在区号
        /// </summary>
        /// <param name="shippingMethodId">运输方式ID</param>
        /// <param name="postCode">邮政编号</param>
        /// <returns></returns>
        private int GetShippingZone(int shippingMethodId, string postCode, string countryCode)
        {
            int zone = 0;
            if (string.IsNullOrWhiteSpace(postCode))
                return zone;
            //非俄速通小包专线挂号时就返回0
            if (shippingMethodId != sysConfig.SpecialShippingMethodId)
            {
                List<ShippingMethodCountryModel> shippingMethod = _freightService.GetCountryArea(shippingMethodId, countryCode);
                if (shippingMethod != null && shippingMethod.Count > 0)
                    zone = shippingMethod.First().AreaId;
                return zone;
            }
            var firstStr = postCode.Substring(0, 1);
            if (firstStr == "1" || firstStr == "2" || firstStr == "3" || firstStr == "4")
            {
                switch (firstStr)
                {
                    case "1":
                        zone = 1;
                        break;
                    case "2":
                        zone = 2;
                        break;
                    case "3":
                        zone = 3;
                        break;
                    case "4":
                        zone = 4;
                        break;
                    default:
                        zone = 0;
                        break;
                }
            }
            else
            {
                var twoStr = postCode.Substring(0, 2);
                if (twoStr == "60" || twoStr == "61" || twoStr == "62")
                {
                    switch (twoStr)
                    {
                        case "60":
                        case "61":
                        case "62":
                            zone = 4;
                            break;
                        default:
                            zone = 6;
                            break;
                    }
                }
                else
                {
                    var threeStr = postCode.Substring(0, 3);
                    if (threeStr == "640" || threeStr == "641")
                    {
                        switch (threeStr)
                        {
                            case "640":
                            case "641":
                                zone = 4;
                                break;
                            default:
                                zone = 6;
                                break;
                        }
                    }
                    else
                    {
                        zone = 6;
                    }
                }
            }

            return zone;
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult LoadPrintData(InvoivePrinterViewModel viewModel)
        {
            InvoivePrinterViewModel model;
            List<string> wayBillNUmber = new List<string>();
            string[] arr = viewModel.Ids.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);

            arr.Each(v => wayBillNUmber.Add(v.Trim()));
            var list = _customerOrderService.GetCustomerOrderIdByWayBillNumber(wayBillNUmber);

            string customerOrderIds = "";
            list.ForEach(p =>
                {
                    customerOrderIds += p + ",";
                });
            model = BindInvoicePrinter(customerOrderIds, viewModel.TemplateName);
            TempData["InvoivePrinterViewModel"] = model;
            return PartialView("_InvoicePrinter", model);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult PrintPreview(InvoivePrinterViewModel viewModel)
        {
            InvoivePrinterViewModel model;
            //var list = CacheHelper.Get("cache_countryList") as List<Country>;
            if (TempData["PrinterViewModel"] != null)
            {
                model = TempData["PrinterViewModel"] as InvoivePrinterViewModel;
            }
            else
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                model = BindInvoicePrinter(viewModel.Ids, viewModel.TemplateName);
                sw.Stop();
                string aa = sw.ElapsedMilliseconds.ToString();
            }
            return View(model);
        }

        public ActionResult QuickPrint(string number)
        {
            return View();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult CanQuickPrint(string number)
        {
            CanQuickPrintResult responseResult = new CanQuickPrintResult();
            var wayBillInfo = _wayBillInfoRepository.GetWayBill(number);

            if (wayBillInfo == null)
            {
                responseResult.Message = "单号不存在";
                return Json(responseResult);
            }

            string wayBillNumber = wayBillInfo.WayBillNumber;


            //DHL快递
            //if (_expressService.IsExistExpressResponse(wayBillNumber) &&
            if (wayBillInfo.ExpressRespons != null && _expressService.GetExpressAccountInfos().ToList().Find(p => p.VenderCode == wayBillInfo.VenderCode && p.ShippingMethodId == wayBillInfo.OutShippingMethodID) != null)
            {
                responseResult.Success = true;
                responseResult.Urls = new[]
                    {
                        @Url.Action("DHLPrintPreview", "Print", new {wayBillNumber}),
                        @Url.Action("InvoicePrinter", "Print", new {ids = wayBillNumber, Printer = 2}),
                        @Url.Action("DHLPrintPreview_1", "Print", new {wayBillNumber, Printer = 3}),
                        @Url.Action("InvoicePrinter", "Print", new {ids = wayBillNumber, Printer = 4})
                    };
                return Json(responseResult);
            }

            if (_expressService.GetNetherlandsParcelRespons(wayBillNumber, 2) != null)
            {
                responseResult.Success = true;
                responseResult.Urls =new[]{ @Url.Action("NetherlandsParcelPreview", "Print", new { wayBillNumber })};
                //responseResult.UrlMain = @Url.Action("Printer", "Print", new { ids=wayBillInfo.CustomerOrderID, typeId = "DT1308100021" });
                return Json(responseResult);
            }


            //立陶宛打单
            List<int> shippingMethodIds=new List<int>();
            var StrshippingMethodId = sysConfig.LithuaniaShippingMethodCodeId.Split(',').ToList();
            StrshippingMethodId.ForEach(p =>
                {
                    int shippingMethodId;
                    if (Int32.TryParse(p, out shippingMethodId))
                    {
                        shippingMethodIds.Add(shippingMethodId);
                    }
                });
            if (wayBillInfo.OutShippingMethodID != null && shippingMethodIds.Contains(wayBillInfo.OutShippingMethodID.Value))
            {
                    responseResult.Success = true;
                    responseResult.Urls = new[] { @Url.Action("LithuaniaPrintView", "Print", new { wayBillNumber }) };
                    //responseResult.UrlMain = @Url.Action("Printer", "Print", new { ids=wayBillInfo.CustomerOrderID, typeId = "DT1308100021" });
                    return Json(responseResult);
            }

            //判断是否为EUB
            var eubWayBillApplicationInfo = _eubWayBillService.GetEubWayBillApplicationInfo(wayBillNumber);

            if (eubWayBillApplicationInfo == null)
            {
                responseResult.Message = "运单发货方式未在线上申请DHL快递或EUB面单！";
                return Json(responseResult);
            }

            //判断是否已申请
            if (eubWayBillApplicationInfo.Status >= (int)EubWayBillApplicationInfo.StatusEnum.Apply)
            {
                //判断是否下载
                if (System.IO.File.Exists(sysConfig.PdfTemplatePath + wayBillNumber + ".pdf"))
                {
                    if (IsFileInUse(sysConfig.PdfTemplatePath + wayBillNumber + ".pdf"))
                    {
                        responseResult.Message = "Pdf文件被占用";
                        Log.Error("{0}文件已被占用!".FormatWith(sysConfig.PdfTemplatePath + wayBillNumber + ".pdf"));
                    }
                    else
                    {
                        responseResult.Success = true;
                        responseResult.Urls = new[] {sysConfig.PdfUserUploadWebPath + wayBillNumber + ".pdf"};
                    }
                }
                else
                {
                    responseResult.Message = "EUB运单文件尚未下载，请稍后再试";
                }
            }
            else
            {
                responseResult.Message = "EUB未申请运单,请先申请";
            }

            return Json(responseResult);
        }

        /// <summary>
        /// 判断文件是否被占用
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns>返回值</returns>
        public static bool IsFileInUse(string fileName)
        {
            bool inUse = true;
            FileStream fs = null;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read,
                FileShare.None);
                inUse = false;
            }
            catch
            {
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            //True表示正在使用,False表示没有使用
            return inUse;
        }

        #endregion

        [System.Web.Mvc.HttpPost]
        public JsonResult CreateInStorage(InStorageSaveModel model, List<WayBillInfoSaveModel> wayBilllist)
        {
            var responseResult = new ResponseResult();
            if (model != null && wayBilllist != null && wayBilllist.Count > 0)
            {

                var inStorage = new CreateInStorageExt
                {
                    InStorage =
                    {
                        CustomerCode = model.CustomerCode,
                        Freight = 0,
                        FuelCharge = 0,
                        InStorageID = SequenceNumberService.GetSequenceNumber(PrefixCode.InStorageID),
                        Register = 0,
                        TotalFee = 0,
                        TotalQty = 0,
                        Surcharge = 0,
                        TotalWeight = 0,
                        TariffPrepayFee = 0,
                    } ,
                    BusinessDate = model.GetBusinessDate
                };
                var shippingMethod = _freightService.GetShippingMethod(model.ShippingMethodId);
                List<string> errorWayBillNumber = new List<string>();
                try
                {
                    foreach (var w in wayBilllist)
                    {
                        if (!string.IsNullOrWhiteSpace(w.WayBillNumber))
                        {

                            if (w.Weight <= 0 || w.SettleWeight <= 0)
                            {
                                errorWayBillNumber.Add(w.WayBillNumber);
                                continue;
                            }
                            //根据转换重量转换成文件类型 Add by zhengsong

                            #region

                            if (shippingMethod != null)
                            {
                                if (shippingMethod.Enabled && shippingMethod.ShippingMethodTypeId == 4) //4-代表EMS
                                {
                                    if (w.Weight <= shippingMethod.PackageTransformFileWeight)
                                    {
                                        model.GoodsTypeID = 2; //2-代表文件类型
                                    }
                                }
                            }

                            #endregion

                            WayBillInfoExt extmodel = new WayBillInfoExt();
                            extmodel.CustomerCode = model.CustomerCode.Trim();
                            extmodel.CustomerType = model.CustomerType;
                            extmodel.GoodsTypeID = model.GoodsTypeID;
                            extmodel.Length = w.Length;
                            extmodel.Height = w.Height;
                            extmodel.Width = w.Width;
                            extmodel.Weight = w.Weight;
                            extmodel.ShippingMethodId = model.ShippingMethodId;
                            extmodel.TrackingNumber = w.TrackingNumber;
                            extmodel.SettleWeight = w.SettleWeight;
                            extmodel.Freight = w.Freight;
                            extmodel.FuelCharge = w.FuelCharge;
                            extmodel.Register = w.Register;
                            extmodel.TariffPrepay = w.TariffPrepay;
                            extmodel.Surcharge = w.Surcharge;
                            extmodel.WayBillNumber = w.WayBillNumber.Trim();
                            inStorage.WayBillInfos.Add(extmodel);
                            inStorage.InStorage.Freight += w.Freight;
                            inStorage.InStorage.FuelCharge += w.FuelCharge;
                            inStorage.InStorage.Register += w.Register;
                            inStorage.InStorage.Surcharge += w.Surcharge;
                            inStorage.InStorage.TariffPrepayFee += w.TariffPrepay;
                            inStorage.InStorage.TotalWeight += w.SettleWeight;
                            inStorage.InStorage.TotalQty++;
                            inStorage.InStorage.TotalFee = inStorage.InStorage.Freight +
                                                           inStorage.InStorage.FuelCharge +
                                                           inStorage.InStorage.Register +
                                                           inStorage.InStorage.Surcharge +
                                                           inStorage.InStorage.TariffPrepayFee;

                        }

                    }

                    if (inStorage.InStorage.TotalQty > 0)
                    {
                        _inStorageService.CreateInStorage(inStorage);
                        responseResult.Result = true;
                        responseResult.Message = inStorage.InStorage.InStorageID;
                        errorWayBillNumber.ForEach(p =>
                            {
                                responseResult.ErrorWayBillNumber += "[" + p + "]";
                            });
                        if (responseResult.ErrorWayBillNumber != null)
                        {
                            Log.Error(responseResult.ErrorWayBillNumber);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    responseResult.Result = false;
                    responseResult.Message = ex.Message.Contains("更新条目时出错") ? "系统繁忙，请重试!" : ex.Message;
                }
            }
            return Json(responseResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 直接入仓提交
        /// by zxq
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        [ButtonPermissionValidator(PermissionRecords.FastInStorageCode)]
        public JsonResult CreateFastInStorage(FastInStorageViewModel model)
        {
            var responseResult = new ResponseResult();

            List<string> wayBilllist = model.WayBillNumbers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var errorDictionaryList = new List<Dictionary<string, string>>();

            if (wayBilllist.Count > 0)
            {
                //创建一个入仓
                var inStorage = new CreateInStorageExt
                {

                    InStorage =
                    {
                        CustomerCode = model.CustomerCode,
                        Freight = 0,
                        FuelCharge = 0,
                        InStorageID = SequenceNumberService.GetSequenceNumber(PrefixCode.InStorageID),
                        Register = 0,
                        TotalFee = 0,
                        TotalQty = 0,
                        Surcharge = 0,
                        TotalWeight = 0,
                        TariffPrepayFee = 0,
                    }
                };

                //var shippingMethod = _freightService.GetShippingMethod(model.ShippingMethodId);
                var shippingMethod = _freightService.GetShippingMethodListByCustomerCode(model.CustomerCode, true).Find(m => m.ShippingMethodId == model.ShippingMethodId);
                var customer = _customerService.GetCustomer(model.CustomerCode);

                foreach (var w in wayBilllist)
                {
                    var wayBillInfoExt = new WayBillInfoExt();

                    var wayBillInfo = _wayBillInfoRepository.GetWayBillInfo(w, customer.CustomerCode);

                    if (wayBillInfo == null)
                    {
                        var errorDic = new Dictionary<string, string>();
                        errorDic.Add(w, "未找到或不属于该客户！");
                        errorDictionaryList.Add(errorDic);

                        continue;
                    }



                    wayBillInfo.CopyTo(wayBillInfoExt); //复制到Ext

                    //入仓重量异常单入仓(按称重重量入仓) yungchu
                    if (model.Opereate == "InStorageWeightAbnormal")
                    {
                        wayBillInfoExt.Weight = _wayBillInfoRepository.GetWayBillWeight(w);
                    }



                    wayBillInfoExt.CustomerType = customer.CustomerTypeID.Value;
                    wayBillInfoExt.GoodsTypeID = model.GoodsTypeId;
                    wayBillInfoExt.ShippingMethodId = shippingMethod.ShippingMethodId;

                    //检查重量
                    if (wayBillInfoExt.Weight <= 0)
                    {
                        var errorDic = new Dictionary<string, string>();
                        errorDic.Add(wayBillInfoExt.WayBillNumber, "未输入重量,入仓失败！");
                        errorDictionaryList.Add(errorDic);

                        continue;
                    }


                    //检查体积,如果该运输方式需要输入体积！
                    if (shippingMethod.WeightOrVolume && (wayBillInfoExt.Height * wayBillInfoExt.Length * wayBillInfoExt.Width <= 1))
                    {
                        var errorDic = new Dictionary<string, string>();
                        errorDic.Add(wayBillInfoExt.WayBillNumber, "未输入体积,入仓失败！");
                        errorDictionaryList.Add(errorDic);

                        continue;
                    }

                    #region 跑一般入仓检查逻辑

                    //new一个model以调用那个方法
                    var inStorageFormModel = new InStorageFormModel()
                        {
                            CustomerType = customer.CustomerTypeID.Value,
                            CustomerCode = customer.CustomerCode,
                            GoodsTypeID = model.GoodsTypeId,
                            ShippingMethodId = shippingMethod.ShippingMethodId,
                            WayBillNumber = w,
                            ChkPrint = false,
                            Weight = wayBillInfoExt.Weight,
                            IsWeightAbnormalWaybill = model.Opereate == "InStorageWeightAbnormal"  //是否重量异常单
                        };

                    var inStorageWayBillModel = CheckOnInStorageWork(inStorageFormModel);
                    if (!inStorageWayBillModel.IsSuccess)
                    {
                        var errorDic = new Dictionary<string, string>();
                        errorDic.Add(wayBillInfoExt.WayBillNumber, inStorageWayBillModel.Message);
                        errorDictionaryList.Add(errorDic);
                        continue;
                    }
                    wayBillInfoExt.Freight = inStorageWayBillModel.Freight; //运费
                    wayBillInfoExt.FuelCharge = inStorageWayBillModel.FuelCharge; //燃油费
                    wayBillInfoExt.Register = inStorageWayBillModel.Register; //挂号费
                    wayBillInfoExt.Surcharge = inStorageWayBillModel.Surcharge; //附加费
                    wayBillInfoExt.SettleWeight = inStorageWayBillModel.SettleWeight; //结算重量
                    wayBillInfoExt.TariffPrepay = inStorageWayBillModel.TariffPrepay;//关税预付服务费

                    if (wayBillInfoExt.SettleWeight <= 0)
                    {
                        var errorDic = new Dictionary<string, string>();
                        errorDic.Add(wayBillInfoExt.WayBillNumber, "结算重量为零,入仓失败！");
                        errorDictionaryList.Add(errorDic);

                        continue;
                    }

                    #endregion

                    //判断是否存在该订单的错误信息，无错误信息则继续提交
                    if (errorDictionaryList.FindIndex(m => m.ContainsKey(wayBillInfoExt.WayBillNumber)) == -1)
                    {
                        inStorage.WayBillInfos.Add(wayBillInfoExt);

                        inStorage.InStorage.Freight += wayBillInfoExt.Freight;
                        inStorage.InStorage.FuelCharge += wayBillInfoExt.FuelCharge;
                        inStorage.InStorage.Register += wayBillInfoExt.Register;
                        inStorage.InStorage.TariffPrepayFee += wayBillInfoExt.TariffPrepay;
                        inStorage.InStorage.Surcharge += wayBillInfoExt.Surcharge;
                        inStorage.InStorage.TotalWeight += wayBillInfoExt.SettleWeight;
                        inStorage.InStorage.TotalQty++;
                        inStorage.InStorage.TotalFee = inStorage.InStorage.Freight + inStorage.InStorage.FuelCharge +
                                                       inStorage.InStorage.Register + inStorage.InStorage.Surcharge +
                                                       inStorage.InStorage.TariffPrepayFee;
                    }
                }

                try
                {
                    if (inStorage.WayBillInfos.Count > 0)
                    {
                        //如果是入仓异常单, 直接入仓之后解除运单hold yungchu
                        if (model.Opereate == "InStorageWeightAbnormal")
                        {
                            List<string> listString = new List<string>();
                            inStorage.WayBillInfos.ForEach(a => listString.Add(a.WayBillNumber));
                            CancelWayBillHold(listString);
                        }
                        _inStorageService.CreateInStorage(inStorage);
                        responseResult.Result = true;
                        responseResult.Url = Url.Action("InStorageDetail", new { InStorageId = inStorage.InStorage.InStorageID });
                        responseResult.Message = string.Format("<a href='{0}'>全部入仓成功，入仓单号：{1}</a><br/>", responseResult.Url, inStorage.InStorage.InStorageID);
                    }

                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    responseResult.Result = false;
                    responseResult.Message = ex.Message.Contains("更新条目时出错") ? "系统繁忙，请重试!" : ex.Message;
                }
                finally
                {
                    if (responseResult.Result && errorDictionaryList.Count > 0 && inStorage.WayBillInfos.Count > 0)
                    {

                        //如果是入仓异常单, 直接入仓之后解除运单hold yungchu
                        if (model.Opereate == "InStorageWeightAbnormal")
                        {
                            List<string> getErrorWaybillnumber = (from item in errorDictionaryList
                                                                  from item1 in item
                                                                  select item1.Key).ToList();

                            ThreadPool.QueueUserWorkItem(a => CancelWayBillHold(wayBilllist.Except(getErrorWaybillnumber).ToList()));
                        }

                        responseResult.Message += string.Format("<a href='{0}'>部分入仓成功，入仓单号：{1}</a><br/>", responseResult.Url, inStorage.InStorage.InStorageID);
                    }

                    if (errorDictionaryList.Count > 0)
                    {
                        responseResult.Result = false;
                    }

                    errorDictionaryList.ForEach(e =>
                        {
                            foreach (var keyValuePair in e)
                            {
                                responseResult.Message += "运单:" + keyValuePair.Key + keyValuePair.Value + "<br/>";
                            }
                        });

                }
            }
            return Json(responseResult, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult CreateOutStorage(OutStorageFormModel model, List<WayBillInfoSaveModel> wayBilllist)
        {
            
            var responseResult = new ResponseResult();
            if (model != null && wayBilllist != null && wayBilllist.Count > 0)
            {
                #region 判断是否存在现结客户有货款未结清
                //List<string> wayBillNumbers =new List<string>();
                //wayBilllist.ForEach(p => wayBillNumbers.Add(p.WayBillNumber));
                //var wayBills = _outStorageService.GetWayBillInfoListByWayBillNumber(wayBillNumbers);
                //List<string> customers = new List<string>();
                //wayBills.ForEach(p =>
                //{
                //    if (!customers.Contains(p.CustomerCode))
                //    {
                //        customers.Add(p.CustomerCode);
                //    }
                //});
                //var customerList = _customerService.GetCustomerList(customers);
                //customerList.ForEach(p =>
                //{
                //    var customerBalance = _customerService.GetCustomerBalance(p.CustomerCode);
                //    decimal money = 0;
                //    if (p.EnableCredit)
                //    {
                //        money = (customerBalance.Balance ?? 0) + p.MaxDelinquentAmounts;
                //    }
                //    else
                //    {
                //        money = customerBalance.Balance ?? 0;
                //    }
                //    if (money < 0)
                //    {
                //        responseResult.Result = false;
                //        responseResult.Message = p.Name + "现结客户货款未结清，不允许出库！";
                //    }
                //});
                //if (!responseResult.Result && !string.IsNullOrWhiteSpace(responseResult.Message))
                //{
                //    return Json(responseResult, JsonRequestBehavior.AllowGet);
                //}
                #endregion

                var venderName =
                    _freightService.GetVenderList(true).FirstOrDefault(p => p.VenderCode == model.VenderCode).VenderName;
                var outStorage = new CreateOutStorageExt
                {
                    OutStorage =
                    {
                        VenderCode = model.VenderCode,
                        VenderName = venderName,
                        GoodsTypeID = model.GoodsTypeID,
                        Freight = 0,
                        FuelCharge = 0,
                        OutStorageID = SequenceNumberService.GetSequenceNumber(PrefixCode.OutStorageID),
                        Register = 0,
                        TotalFee = 0,
                        TotalQty = 0,
                        TotalWeight = 0,
                        Surcharge = 0
                    }
                };

                var outShippingMethodName = string.Empty;
                bool haveTrackingNum = false;
                var list = _freightService.GetShippingMethodListByVenderCode(model.VenderCode, true);
                if (list != null && list.Count > 0 && list.Exists(p => p.ShippingMethodId == model.ShippingMethodId))
                {
                    var shippingMehtod = list.FirstOrDefault(p => p.ShippingMethodId == model.ShippingMethodId);
                    outShippingMethodName = shippingMehtod != null ? shippingMehtod.ShippingMethodName : "";
                    haveTrackingNum = shippingMehtod != null && shippingMehtod.HaveTrackingNum;
                    wayBilllist.ForEach(w =>
                    {  //TrackingNumber is not null
                        if (!string.IsNullOrWhiteSpace(w.WayBillNumber))
                        {
                            var extmodel = new OutWayBillInfoExt
                            {
                                TrackingNumber = w.TrackingNumber,
                                SettleWeight = w.SettleWeight,
                                Weight=w.Weight,
                                WayBillNumber = w.WayBillNumber,
                                OutShippingMethodID = model.ShippingMethodId,
                                OutShippingMethodName = outShippingMethodName,
                                Surcharge = w.Surcharge,
                                GoodsTypeID = model.GoodsTypeID,
                                HaveTrackingNum = haveTrackingNum,
                                CountryCode = model.CountryCode,
                                IsBattery = model.IsBattery,
                            };
                            outStorage.WayBillInfos.Add(extmodel);
                            outStorage.OutStorage.Freight += 0;
                            outStorage.OutStorage.FuelCharge += 0;
                            outStorage.OutStorage.Register += 0;
                            outStorage.OutStorage.Surcharge += 0;
                            outStorage.OutStorage.TotalWeight += w.SettleWeight;
                            outStorage.OutStorage.TotalQty++;
                            outStorage.OutStorage.TotalFee = outStorage.OutStorage.Freight + outStorage.OutStorage.FuelCharge +
                                                           outStorage.OutStorage.Register + outStorage.OutStorage.Surcharge;
                        }
                    });
                    try
                    {
                        _outStorageService.CreateOutStorage(outStorage);
                        responseResult.Result = true;
                        responseResult.Message = outStorage.OutStorage.OutStorageID;
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                        responseResult.Result = false;
                        responseResult.Message = ex.Message;
                    }
                }
                else
                {
                    responseResult.Result = false;
                    responseResult.Message = "运输方式不存在！";
                }
            }
            return Json(responseResult, JsonRequestBehavior.AllowGet);
        }

        #region 退货记录查询导出功能
        /// <summary>
        /// 退货记录查询列表
        /// Add by zhengsong
        /// Time:2014-05-16
        /// </summary>
        /// <returns></returns>
        public ActionResult ReturnWayBillList(ReturnWayBillFilterModel filter = null)
        {
            return View(BindReturnWayBill(filter));
        }

        /// <summary>
        /// 查询退货记录
        /// Add by zhengsong
        /// Time:2014-05-16
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        [FormValueRequired("btnSelect")]
        public ActionResult ReturnWayBillList(ReturnWayBillViewModel model)
        {
            return View(BindReturnWayBill(model.FilterModel));
        }

        /// <summary>
        /// 退货信息导出
        /// Add by zhengsong
        /// Time:2014-05-17
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        [FormValueRequired("btnExport")]
        [System.Web.Mvc.ActionName("ReturnWayBillList")]
        public ActionResult ExportReturnWayBill(ReturnWayBillViewModel model)
        {
            ReturnWayBillParam param = new ReturnWayBillParam();
            param.CustomerCode = model.FilterModel.CustomerCode;
            param.ReturnStartTime = model.FilterModel.ReturnStartTime;
            param.ReturnEndTime = model.FilterModel.ReturnEndTime;
            param.SearchWhere = model.FilterModel.SearchWhere;
            param.SearchContext = model.FilterModel.SearchContext;
            param.InShippingMehtodId = model.FilterModel.ShippingMehtodId;
            param.Status = (int)ReturnGood.ReturnStatusEnum.Audited;
            if (model.FilterModel.CreateBy != null)
            {
                param.CreateBy = model.FilterModel.CreateBy.Trim();
            }
            else
            {
                param.CreateBy = model.FilterModel.CreateBy;
            }
            model.ReturnWayBillList = _returnGoodsService.GetExportReturnWayBillList(param).ToModelAsCollection<ReturnWayBillModelExt, ReturnWayBillModel>();
            var customers = _customerService.GetCustomerList("", false);
            if (model.ReturnWayBillList.Count > 0)
            {
                model.ReturnWayBillList.ForEach(p =>
                    {
                        var customer = customers.Find(c => c.CustomerCode == p.CustomerName);
                        if (customer != null)
                        {
                            p.CustomerName = customer.Name;
                        }
                        p.TypeName = WayBill.GetReturnGoodTypeDescription(p.Type);
                        if (p.IsReturnShipping)
                        {
                            p.IsReturnShippingName = "是";
                        }
                        else
                        {
                            p.IsReturnShippingName = "否";
                        }
                    });
                var titleList = new List<string>
                    {
                        "CustomerName-客户名称",
                        "WayBillNumber-运单号",
                        "CustomerOrderNumber-订单号",
                        "InShippingMehtodName-运输方式",
                        "TrackingNumber-跟踪号",
                        "TotalWeight-重量",
                        "CountryCode-国家简码",
                        "ChineseName-国家中文名",
                        "ShippingFee-总运费",
                        "TypeName-退货类型",
                        "Reason-退货原因",
                        "ReasonRemark-退货备注",
                        "IsReturnShippingName-是否退运费",
                        "OutCreatedOn-发货时间",
                        "ReturnCreatedOn-退货时间",
                        "AuditorDate-审核时间",
                        "Auditor-审核人",
                        "CreatedBy-退货操作人"
                    };
                string fileName = "exportReturnGoodsInfos" + DateTime.Now.ToString("yyyy-dd-MM-hh-mm-ss") + "1";
                ExportExcelByWeb.ListExcel(fileName, model.ReturnWayBillList, titleList);
            }
            return View(BindReturnWayBill(model.FilterModel));
        }

        /// <summary>
        /// 退货记录查询列表绑定
        /// Add by zhengsong
        /// Time:2014-05-16
        /// </summary>
        /// <param name="filterModel"></param>
        /// <returns></returns> 
        public ReturnWayBillViewModel BindReturnWayBill(ReturnWayBillFilterModel filterModel)
        {
            ReturnWayBillViewModel model = new ReturnWayBillViewModel();
            model.FilterModel = filterModel;
            WayBill.GetSearchFilterList().ForEach(p =>
            {
                if (p.ValueField != "4")
                {
                    model.SearchWheres.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = model.FilterModel.SearchWhere.HasValue && p.ValueField == model.FilterModel.SearchWhere.Value.ToString() });
                }
            });
            _customerService.GetCustomerList("", false).ForEach(p => model.Customers.Add(new SelectListItem() { Text = p.Name, Value = p.CustomerCode }));
            ReturnWayBillParam param = new ReturnWayBillParam();
            param.CustomerCode = filterModel.CustomerCode;
            param.ReturnStartTime = filterModel.ReturnStartTime;
            param.ReturnEndTime = filterModel.ReturnEndTime;
            param.SearchWhere = filterModel.SearchWhere;
            param.SearchContext = filterModel.SearchContext;
            param.InShippingMehtodId = filterModel.ShippingMehtodId;
            param.Status = (int)ReturnGood.ReturnStatusEnum.Audited;
            if (filterModel.CreateBy != null)
            {
                param.CreateBy = filterModel.CreateBy.Trim();
            }
            else
            {
                param.CreateBy = filterModel.CreateBy;
            }
            param.Page = filterModel.Page;
            param.PageSize = filterModel.PageSize;
            model.PagedReturnWayBillList = _returnGoodsService.GetPagedList(param).ToModelAsPageCollection<ReturnWayBillModelExt, ReturnWayBillModel>();
            return model;
        }
        #endregion

        #region 退货审核功能

        public ActionResult ReturnAuditList(ReturnAuditFilterModel filter)
        {
            if (filter.DateWhere == 0)
            {
                if (filter.ReturnStartTime == null)
                {
                    filter.ReturnStartTime = DateTime.Parse(DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd") + " " + "00:00");
                }
                if (filter.ReturnEndTime == null)
                {
                    filter.ReturnEndTime = DateTime.Parse(DateTime.Now.AddDays(+1).ToString("yyyy-MM-dd") + " " + "00:00");
                }
                filter.DateWhere = 1;
                filter.PageSize = 300;
            }
            return View(BindReturnAuditList(filter));
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("ReturnAuditList")]
        [FormValueRequired("btnSelect")]
        public ActionResult SelecstReturnAuditList(ReturnAuditViewModel model)
        {
            return View(BindReturnAuditList(model.FilterModel));
        }


        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("ReturnAuditList")]
        [FormValueRequired("btnSave")]
        public ActionResult SaveReturnAudit(ReturnAuditViewModel model)
        {
            string WayBillNumbers = Request.Form["OrderNumber"];
            string[] wayBilllist = WayBillNumbers.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (wayBilllist.Length <= 0)
            {
                SetViewMessage(ShowMessageType.Error, "请选择要审核的运单", false);
            }
            else
            {
                var result = _returnGoodsService.ReturnAuditList(wayBilllist);
                if (result)
                {
                    SetViewMessage(ShowMessageType.Success, "审核成功", false);

                }
                else
                {
                    SetViewMessage(ShowMessageType.Error, "审核失败", false);
                }
            }
            return View(BindReturnAuditList(model.FilterModel));
        }

        #region 修改退货信息

        public ActionResult UpdateReturnAudit(string waybillList)
        {
            UpdateReturnAuditViewModel model = new UpdateReturnAuditViewModel();
            model.WayBillList = waybillList;
            return View(model);
        }

        public JsonResult JsonUpdateReturnAudit(string waybillList, string type, string returnReason, string isReturnShipping)
        {
            var result = new ResponseResult();
            try
            {
                string[] wayBilllists = waybillList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                int types = int.Parse(type);
                bool isReturnShippings = bool.Parse(isReturnShipping);
                var resul = _returnGoodsService.UpdateReturnAuditList(wayBilllists, types, returnReason, isReturnShippings);
                if (resul)
                {
                    result.Result = true;

                }
                else
                {
                    result.Result = true;
                    result.Message = "审核失败";
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
        #endregion

        public ReturnAuditViewModel BindReturnAuditList(ReturnAuditFilterModel filter)
        {
            ReturnAuditViewModel model = new ReturnAuditViewModel();
            model.FilterModel = filter;
            WayBill.GetSearchFilterList().ForEach(p =>
            {
                if (p.ValueField != "4")
                {
                    model.SearchWheres.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = model.FilterModel.SearchWhere.HasValue && p.ValueField == model.FilterModel.SearchWhere.Value.ToString() });
                }
            });
            _customerService.GetCustomerList("", false).ForEach(p => model.Customers.Add(new SelectListItem() { Text = p.Name, Value = p.CustomerCode }));



            ReturnWayBillParam param = new ReturnWayBillParam();
            param.CustomerCode = filter.CustomerCode;
            param.ReturnStartTime = filter.ReturnStartTime;
            param.ReturnEndTime = filter.ReturnEndTime;
            param.SearchWhere = filter.SearchWhere;
            param.SearchContext = filter.SearchContext;
            param.InShippingMehtodId = filter.ShippingMehtodId;
            if (!string.IsNullOrWhiteSpace(filter.IsReturnShipping) && filter.IsReturnShipping == "true")
            {
                param.IsReturnShipping = true;
            }
            else if (!string.IsNullOrWhiteSpace(filter.IsReturnShipping) && filter.IsReturnShipping == "false")
            {
                param.IsReturnShipping = false;
            }
            param.Status = 1;
            param.ReturnReason = filter.ReturnReason;
            if (filter.CreateBy != null)
            {
                param.CreateBy = filter.CreateBy.Trim();
            }
            else
            {
                param.CreateBy = filter.CreateBy;
            }

            param.Page = filter.Page;
            param.PageSize = filter.PageSize;
            model.PagedReturnAuditList = _returnGoodsService.GetPagedList(param).ToModelAsPageCollection<ReturnWayBillModelExt, ReturnWayBillModel>();
            return model;
        }

        #endregion


        #region 快递查看货物详细信息

        /// <summary>
        /// Add By zhengsong
        /// Time:2014-06-19
        /// </summary>
        /// <returns></returns>
        public ActionResult ExpressWayBillInfoList(ExpressWayBillFilterModel filterModel)
        {
            if (filterModel.DateWhere == 0)
            {
                if (filterModel.StartTime == null)
                {
                    filterModel.StartTime = DateTime.Parse(DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd") + " " + "00:00");
                }
                if (filterModel.EndTime == null)
                {
                    filterModel.EndTime = DateTime.Parse(DateTime.Now.AddDays(+1).ToString("yyyy-MM-dd") + " " + "00:00");
                }
                filterModel.DateWhere = 3;
            }
            return View(BindExpressWayBillList(filterModel));
        }

        [System.Web.Mvc.HttpPost]
        [FormValueRequired("btnSearch")]
        [System.Web.Mvc.ActionName("ExpressWayBillInfoList")]
        public ActionResult SelectWayBillInfoList(ExpressWayBillFilterModel filterModel)
        {
            return View(BindExpressWayBillList(filterModel));
        }

        [System.Web.Mvc.HttpPost]
        [FormValueRequired("btnExprot")]
        [System.Web.Mvc.ActionName("ExpressWayBillInfoList")]
        public ActionResult ExprotWayBillInfoList(ExpressWayBillFilterModel filterModel)
        {
            ExpressWayBillParam param = new ExpressWayBillParam();
            param.CustomerCode = filterModel.CustomerCode;
            param.ShippingMethodId = filterModel.ShippingMethodId;
            param.DateWhere = filterModel.DateWhere;
            param.StartTime = filterModel.StartTime;
            param.EndTime = filterModel.EndTime;
            param.SearchWhere = filterModel.SearchWhere;
            param.SearchContext = filterModel.SearchContext;
            param.Status = filterModel.Status;
            var customers = _customerService.GetCustomerList("", true);
            var countrys = _countryService.GetCountryList("");
            var models = _orderService.GetExprotWayBillList(param);
            models.ForEach(p =>
                {
                    var customer = customers.FirstOrDefault(c => c.CustomerCode == p.CustomerCode);
                    if (customer != null)
                    {
                        p.CustomerCode = customer.Name;
                    }
                    var country = countrys.FirstOrDefault(c => c.CountryCode == p.CountryCode);
                    if (country != null)
                    {
                        p.CountryCode = country.ChineseName;
                    }
                    p.DetailWeight = (p.DetailWeight + p.AddWeight);
                });
            var titleList = new List<string> { 
                "CustomerCode-客户名称",
                "WayBillNumber-运单号",
                "CustomerOrderNumber-订单号", 
                "TrackingNumber-跟踪号",
                "PackageDetailID-货物流水号",
                "InShippingMethodName-运输方式",
                "CountryCode-发货国家", 
                "Length-长(cm)",
                "Width-宽(cm)",
                "Height-高(cm)",
                "DetailWeight-物品重量(kg)",
                "DetailSettleWeight-结算重量(kg)"
            };
            string fileName = "货物详细信息表.xlsx";
            ExportExcelByWeb.WriteToDownLoad(fileName, "Sheet1", models, titleList, null);
            return View(BindExpressWayBillList(filterModel));
        }

        public ExpressWayBillViewModel BindExpressWayBillList(ExpressWayBillFilterModel filterModel)
        {
            ExpressWayBillFilterModel filter = new ExpressWayBillFilterModel();
            var have = WayBill.StatusEnum.Have.GetStatusValue();
            var waitOrder = WayBill.StatusEnum.WaitOrder.GetStatusValue();
            var send = WayBill.StatusEnum.Send.GetStatusValue();
            var delivered = WayBill.StatusEnum.Delivered.GetStatusValue();
            if (filterModel != null)
            {
                filter = filterModel;
            }
            ExpressWayBillViewModel model = new ExpressWayBillViewModel();
            model.FilterModel = filter;
            model.StatusList.Add(new SelectListItem() { Text = "请选择", Value = "", Selected = !filter.Status.HasValue });
            WayBill.GetStatusList().ForEach(p =>
            {
                if (p.ValueField == have.ToString() || p.ValueField == waitOrder.ToString() ||
                    p.ValueField == send.ToString() || p.ValueField == delivered.ToString())
                {
                    model.StatusList.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = filter.Status.HasValue && p.ValueField == filter.Status.Value.ToString() });
                }
            });
            if (filter.SearchWhere == null)
            {
                WayBill.GetSearchFilterList().ForEach(p =>
                {
                    if (p.ValueField != "4")
                    {
                        model.SearchWheres.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField });
                    }
                });
                model.SearchWheres.FirstOrDefault(m => m.Value == "3").Selected = true;
            }
            else
            {
                WayBill.GetSearchFilterList().ForEach(p =>
                {
                    if (p.ValueField != "4")
                    {
                        model.SearchWheres.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = filter.SearchWhere.HasValue && p.ValueField == filter.SearchWhere.Value.ToString() });
                    }
                });
            }
            WayBill.GetDateFilterList().ForEach(p =>
            {
                model.DateWheres.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = p.ValueField == filter.DateWhere.ToString() });
            });
            _customerService.GetCustomerList("", true).ForEach(p => model.Customers.Add(new SelectListItem() { Text = p.Name, Value = p.CustomerCode }));
            ExpressWayBillParam param = new ExpressWayBillParam();
            param.CustomerCode = filter.CustomerCode;
            param.ShippingMethodId = filter.ShippingMethodId;
            param.DateWhere = filter.DateWhere;
            param.StartTime = filter.StartTime;
            param.EndTime = filter.EndTime;
            param.SearchWhere = filter.SearchWhere;
            param.SearchContext = filter.SearchContext;
            param.Status = filter.Status;
            param.Page = filter.Page;
            param.PageSize = filter.PageSize;
            model.PagedList = _orderService.GetPagedExpressWayBillList(param).ToModelAsPageCollection<ExpressWayBillViewExt, ExpressWayBillInfoModel>();
            return model;
        }
        #endregion

        #region 入仓重量配置
        /// <summary>
        /// 入仓重量配置列表
        /// yungchu
        /// 2014-08-05
        /// </summary>
        /// <returns></returns>
        public ActionResult InStorageWeightCompare(InStorageWeightDeviationFilterModel filterModel)
        {
            return View(WeightDeviationDataBind(filterModel));
        }

        [System.Web.Mvc.HttpPost]
        [FormValueRequired("Search")]
        public ActionResult InStorageWeightCompare(InStorageWeightDeviationModel model)
        {
            return View(WeightDeviationDataBind(model.FilterModel));
        }


        public InStorageWeightDeviationModel WeightDeviationDataBind(InStorageWeightDeviationFilterModel filterModel)
        {
            var model = new InStorageWeightDeviationModel
            {
                FilterModel = filterModel,
                PagedList = _inStorageService.GetInStorageWeightDeviationPagedList(new WeightDeviationParam()
                {
                    CustomerCode = filterModel.CustomerCode,
                    ShippingMethodID = filterModel.ShippingMethodID,
                    Page = filterModel.Page,
                    PageSize = filterModel.PageSize,
                    Status = 1
                }).ToModelAsPageCollection<InStorageWeightDeviation, InStorageWeightDeviationInfoModel>()
            };

            return model;
        }
        //新增-1，编辑-2入仓重量配置
        public ActionResult AddOrEditWeightCompare(int type, int inStorageWeightDeviationID, string returnUrl)
        {
            InStorageWeightDeviationModel model = new InStorageWeightDeviationModel();
            model.Type = type;
            model.InStorageWeightDeviationID = inStorageWeightDeviationID;
            model.ReturnUrl = returnUrl;

            if (type == 2)
            {
                InStorageWeightDeviation getDeviation = _inStorageService.GetInStorageWeightDeviation(inStorageWeightDeviationID);
                model.CustomerCode = getDeviation.CustomerCode;
                model.CustomerName = getDeviation.CustomerName;
                model.ShippingMethodID = getDeviation.ShippingMethodID;
                model.ShippingMethodName = getDeviation.ShippingMethodName;
                model.DeviationValue = getDeviation.DeviationValue.HasValue ? getDeviation.DeviationValue.Value : 0;
            }

            return View(model);
        }




        //编辑，添加入仓重量配置
        public JsonResult AddOrEditWeightCompareAjax(InStorageWeightDeviationFilterModel filterModel)
        {
            var result = new ResponseResult();
            InStorageWeightDeviation model = new InStorageWeightDeviation();

            model.CustomerCode = filterModel.CustomerCode;
            model.CustomerName = filterModel.CustomerName;
            model.ShippingMethodID = filterModel.ShippingMethodID;
            model.ShippingMethodName = filterModel.ShippingMethodName;
            model.DeviationValue = Convert.ToDecimal(filterModel.DeviationValue.ToString("F3"));


            //新增
            if (filterModel.Type == 1)
            {
                try
                {
                    bool isSuccess = _inStorageService.AddInStorageWeightDeviations(model);
                    if (isSuccess)
                    {
                        result.Result = true;
                    }
                    else
                    {
                        result.Result = false;
                        result.Message = "保存失败！";
                    }
                }
                catch (Exception ex)
                {
                    result.Result = false;
                    result.Message = ex.Message;
                }

                return Json(result, JsonRequestBehavior.AllowGet);

            }//编辑
            else
            {
                try
                {
                    model.InStorageWeightDeviationID = filterModel.inStorageWeightDeviationId;
                    bool isSuccess = _inStorageService.EditInStorageWeightDeviations(model);
                    if (isSuccess)
                    {
                        result.Result = true;
                    }
                    else
                    {
                        result.Result = false;
                        result.Message = "保存失败！";
                    }
                }
                catch (Exception ex)
                {
                    result.Result = false;
                    result.Message = ex.Message;
                }

                return Json(result, JsonRequestBehavior.AllowGet);

            }

        }

        //删除入仓重量配置
        public ActionResult DeleteWeightCompare(int id, string returnUrl)
        {

            try
            {
                bool isSuccess = _inStorageService.DeleteInStorageWeightDeviations(id);
                if (isSuccess) SuccessNotification("删除成功");
                else ErrorNotification("删除失败");
            }
            catch (Exception e)
            {
                Log.Exception(e);
                ErrorNotification("删除失败，原因为：" + e.Message);
            }
            return Redirect(returnUrl);
        }

        #endregion


        #region 中美专线，欧洲专线模板导出

        public ActionResult WayBillExcelExportList(WayBillListFilterModel param)
        {
            if (param.DateWhere == 0)
            {
                if (param.StartTime == null)
                {
                    param.StartTime = DateTime.Parse(DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd") + " " + "00:00");
                }
                if (param.EndTime == null)
                {
                    param.EndTime = DateTime.Parse(DateTime.Now.AddDays(+1).ToString("yyyy-MM-dd") + " " + "00:00");
                }
                param.DateWhere = 1;
            }
            return View(WayBillExcelExportBindList(param));
        }

        /// <summary>
        /// 运单查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [FormValueRequired("btnSearch")]
        public ActionResult WayBillExcelExportList(WayBillListViewModel param)
        {
            param.FilterModel.Page = 1;
            return View("WayBillExcelExportList", WayBillExcelExportBindList(param.FilterModel));
        }

        /// <summary>
        /// 运单导出
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [System.Web.Mvc.ActionName("WayBillExcelExportList")]
        [FormValueRequired("btnExport")]
        public ActionResult WayBillExcelExport(WayBillListViewModel param)
        {
            List<ExportWayBillModel> models = new List<ExportWayBillModel>();
            List<string> wayBillNumbers = new List<string>();
            var countryList = _countryService.GetCountryList("");
            var customerList = _customerService.GetCustomerList("", false);
            ExportWayBillModel model = new ExportWayBillModel();
            //得到运单号
            int MaxSubColum = 0;
            ExportWayBillList(param.FilterModel).WayBillInfoModels.ForEach(WayBillInfoModel =>
            {
                model = new ExportWayBillModel();
                CustomerOrderInfoModel customerOrderInfos = _orderService.GetCustomerOrderInfoById(WayBillInfoModel.CustomerOrderID).ToModel<CustomerOrderInfoModel>();
                if (customerOrderInfos != null)
                {
                    model.CustomerOrderNumber = customerOrderInfos.CustomerOrderNumber;
                    model.CustomerCode = customerOrderInfos.CustomerCode;
                    if (customerList.FirstOrDefault(p => p.CustomerCode == model.CustomerCode) != null)
                    {
                        model.Name = customerList.First(p => p.CustomerCode == model.CustomerCode).Name;
                    }
                    else
                    {
                        model.Name = "";
                    }
                    model.InsureAmount = customerOrderInfos.InsureAmount;
                    model.PackageNumber = customerOrderInfos.PackageNumber;
                    model.AppLicationType = CustomerOrder.GetApplicationTypeDescription(customerOrderInfos.AppLicationType);
                }
                else
                {
                    model.CustomerOrderNumber = "";
                }
                model.WayBillNumber = WayBillInfoModel.WayBillNumber;
                model.InShippingMethodName = WayBillInfoModel.InShippingMethodName;
                model.TrackingNumber = WayBillInfoModel.TrackingNumber;
                model.Weight = WayBillInfoModel.Weight;
                model.SettleWeight = WayBillInfoModel.SettleWeight;
                model.Length = WayBillInfoModel.Length;
                model.Width = WayBillInfoModel.Width;
                model.Height = WayBillInfoModel.Height;
                model.WayCreatedOn = WayBillInfoModel.CreatedOn.ToString();
                model.ShiCreatedOn = WayBillInfoModel.InStorageTime.ToString();
                model.SenCreatedOn = WayBillInfoModel.OutStorageTime.ToString();
                model.Status = WayBill.GetStatusDescription(WayBillInfoModel.Status);
                //是否关税预付
                model.EnableTariffPrepay = WayBillInfoModel.EnableTariffPrepay;

                ShippingInfoModel shippingInfo = _orderService.GetshippingInfoById(WayBillInfoModel.ShippingInfoID).ToModel<ShippingInfoModel>();
                SenderInfoModel senderInfo = _orderService.GetSenderInfoById(WayBillInfoModel.SenderInfoID).ToModel<SenderInfoModel>();
                if (shippingInfo != null)
                {
                    model.CountryCode = WayBillInfoModel.CountryCode;
                    if (countryList.First(p => p.CountryCode == model.CountryCode) != null)
                    {
                        model.ChineseName = countryList.First(p => p.CountryCode == model.CountryCode).ChineseName;
                    }
                    else
                    {
                        model.ChineseName = "";
                    }
                    model.ShippingFirstName = shippingInfo.ShippingFirstName;
                    model.ShippingLastName = shippingInfo.ShippingLastName;
                    model.ShippingAddress = shippingInfo.ShippingAddress + " " + shippingInfo.ShippingAddress1 + " " + shippingInfo.ShippingAddress2;
                    model.ShippingCity = shippingInfo.ShippingCity;
                    model.ShippingState = shippingInfo.ShippingState;
                    model.ShippingZip = shippingInfo.ShippingZip;
                    model.ShippingPhone = shippingInfo.ShippingPhone;
                    model.ShippingCompany = shippingInfo.ShippingCompany;
                    model.ShippingTaxId = shippingInfo.ShippingTaxId;
                }
                else
                {
                    model.CountryCode = "";
                    model.ChineseName = "";
                    model.ShippingFirstName = "";
                    model.ShippingLastName = "";
                    model.ShippingAddress = "";
                    model.ShippingCity = "";
                    model.ShippingState = "";
                    model.ShippingZip = "";
                    model.ShippingPhone = "";
                    model.ShippingCompany = "";
                    model.ShippingTaxId = "";
                }
                if (senderInfo != null)
                {
                    model.SenderFirstName = senderInfo.SenderFirstName;
                    model.SenderLastName = senderInfo.SenderLastName;
                    model.SenderCompany = senderInfo.SenderCompany;
                    model.SenderAddress = senderInfo.SenderAddress;
                    model.SenderCity = senderInfo.SenderCity;
                    model.SenderState = senderInfo.SenderState;
                    model.SenderZip = senderInfo.SenderZip;
                    model.SenderPhone = senderInfo.SenderPhone;
                }
                else
                {
                    model.SenderFirstName = "";
                    model.SenderLastName = "";
                    model.SenderCompany = "";
                    model.SenderAddress = "";
                    model.SenderCity = "";
                    model.SenderState = "";
                    model.SenderZip = "";
                    model.SenderPhone = "";
                }
                model.IsReturn = WayBillInfoModel.IsReturn;
                InsuredCalculationModel insuredCalculation = _orderService.GetInsuredCalculationById(WayBillInfoModel.InsuredID).ToModel<InsuredCalculationModel>();
                if (insuredCalculation != null)
                {
                    model.InsuredName = insuredCalculation.InsuredName;
                }
                else
                {
                    model.InsuredName = "";
                }
                CustomerOrderInfoModel customerOrderInfo = _orderService.GetCustomerOrderInfoById(WayBillInfoModel.CustomerOrderID).ToModel<CustomerOrderInfoModel>();
                SensitiveTypeInfoModel sensitiveTypeInfo = _orderService.GetSensitiveTypeInfoById(customerOrderInfo.SensitiveTypeID).ToModel<SensitiveTypeInfoModel>();
                if (customerOrderInfos != null && sensitiveTypeInfo != null)
                {
                    model.SensitiveTypeName = sensitiveTypeInfo.SensitiveTypeName;
                }
                else
                {
                    model.SensitiveTypeName = "";
                }


                List<ApplicationInfoModel> applicationInfoModels =
                    _orderService.GetApplicationInfoByWayBillNumber(WayBillInfoModel.WayBillNumber)
                                 .ToModelAsCollection<ApplicationInfo, ApplicationInfoModel>();
                model.ApplicationInfoModels = applicationInfoModels;
                if (applicationInfoModels != null)
                {
                    if (applicationInfoModels.Count >= MaxSubColum)
                    {
                        MaxSubColum = applicationInfoModels.Count;
                    }
                }
                models.Add(model);
                wayBillNumbers.Add(WayBillInfoModel.WayBillNumber.ToString());
            });
            List<string> lstName = new List<string>
                {
                    "客户订单号",
                    "客户代码",
                    "客户名称",
                    "运单号",
                    "入仓运输方式",
                    "跟踪号",
                    "国家简码",
                    "国家中文名",
                    "收件人姓",
                    "收件人名字",
                    "收件人公司",
                    "收货地址",
                    "城市",
                    "省/州",
                    "邮编",
                    "电话",
                    "创建时间",
                    "收货时间",
                    "发货时间",
                    "状态",
                    "收件人税号",
                    "发件人姓",
                    "发件人名",
                    "发件人公司",
                    "发件人地址",
                    "城市",
                    "省/州",
                    "发件人邮编",
                    "发件人电话",
                    "是否退回",
                    "保险类型",
                    "保险价值RMB",
                    "敏感货物",
                    "申报类型",
                    "件数",
                    "长cm",
                    "宽cm",
                    "高cm",
                    "称重重量kg",
                    "结算重量kg",
					"是否关税预付"
                };
            for (int i = 1; i <= MaxSubColum; i++)
            {
                lstName.Add("申报名称" + i);
                lstName.Add("申报中文名称" + i);
                lstName.Add("海关编码" + i);
                lstName.Add("数量" + i);
                lstName.Add("单价" + i + "(usd)");
                lstName.Add("净重量" + i + "(kg)");
                lstName.Add("销售链接" + i);
                lstName.Add("备注" + i);
            }
            string fileName = sysConfig.ExcelTemplateWebPath + sysConfig.ExportWayBill;
            ExportExcelByWeb.ListWayBillExcel(fileName, models, lstName);
            return View(WayBillExcelExportBindList(param.FilterModel));
        }


        /// <summary>
        /// 中美专线模板导出
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [System.Web.Mvc.ActionName("WayBillExcelExportList")]
        [FormValueRequired("btnSinoUSExport")]
        public ActionResult SinoUSWayBillExport(WayBillListViewModel param)
        {
            List<SinoUSWayBill> models = new List<SinoUSWayBill>();
            //得到运单号
            var wayBillInfoModels = ExportWayBillList(param.FilterModel).WayBillInfoModels;

            wayBillInfoModels.ForEach(WayBillInfoModel =>
            {
                SinoUSWayBill model = new SinoUSWayBill();
                CustomerOrderInfoModel customerOrderInfos = _orderService.GetCustomerOrderInfoById(WayBillInfoModel.CustomerOrderID).ToModel<CustomerOrderInfoModel>();
                //if (customerOrderInfos != null)
                //{
                //    model.CustomerOrderNumber = customerOrderInfos.CustomerOrderNumber;
                //}
                //else
                //{
                //    model.CustomerOrderNumber = "";
                //}
                model.WayBillNumber = WayBillInfoModel.WayBillNumber;
                model.TrackingNumber = WayBillInfoModel.TrackingNumber;
                model.Length = WayBillInfoModel.Length;
                model.Width = WayBillInfoModel.Width;
                model.Height = WayBillInfoModel.Height;
                model.Weight = WayBillInfoModel.Weight;
                model.IsRed = "0";
                ShippingInfoModel shippingInfo = _orderService.GetshippingInfoById(WayBillInfoModel.ShippingInfoID).ToModel<ShippingInfoModel>();
                if (shippingInfo != null)
                {
                    model.CountryCode = shippingInfo.CountryCode;

                    model.ShippingName = shippingInfo.ShippingFirstName + " " + shippingInfo.ShippingLastName;
                    //地址

                    model.ShippingAddress1 = shippingInfo.ShippingAddress;
                    model.ShippingAddress2 = shippingInfo.ShippingAddress1;
                    model.ShippingCity = shippingInfo.ShippingCity;
                    model.ShippingState = shippingInfo.ShippingState;
                    model.ShippingZip = shippingInfo.ShippingZip;
                    model.ShippingPhone = shippingInfo.ShippingPhone;
                    model.ShippingCompany = shippingInfo.ShippingCompany;
                }
                else
                {
                    model.CountryCode = "";
                    model.ShippingName = "";
                    //地址
                    model.ShippingAddress1 = "";
                    model.ShippingAddress2 = "";
                    model.ShippingCity = "";
                    model.ShippingState = "";
                    model.ShippingZip = "";
                    model.ShippingPhone = "";
                    model.ShippingCompany = "";
                }

                model.DimUom = "CM";
                model.GrossWeightUom = "kg";
                model.PackageCurrencyCode = "USD";
                List<ApplicationInfoModel> applicationInfoModels =
                    _orderService.GetApplicationInfoByWayBillNumber(WayBillInfoModel.WayBillNumber)
                                 .ToModelAsCollection<ApplicationInfo, ApplicationInfoModel>();

                if (applicationInfoModels != null)
                {
                    applicationInfoModels.ForEach(p =>
                        {
                            model.ApplicationPrice += (p.Qty * p.UnitPrice) ?? 0;
                            model.ApplicationName += p.ApplicationName + "/";
                            model.Remark1 = p.Remark;
                        });
                }
                model.ApplicationName = model.ApplicationName.Substring(0, model.ApplicationName.Length - 1);
                models.Add(model);
            });
            var titleList = new List<string>
                {
                    "WayBillNumber-Waybillnumber",
                    "ShippingCompany-RecipientCompany",
                    "ShippingName-RecipientName",
                    "ShippingAddress1-RecipientAddress1",
                    "ShippingAddress2-RecipientAddress2",
                    "ShippingCity-RecipientCity",
                    "ShippingState-RecipientStateOrProvince",
                    "ShippingZip-RecipientPostalCode",
                    "CountryCode-RecipientAlpha2IsoCountryCode",
                    "ShippingEmail-RecipientEmail",
                    "ShippingPhone-RecipientPhoneNumber",
                    "Height-PackageHeight",
                    "Length-PackageLength",
                    "Width-PackageWidth",
                    "DimUom-DimUom",
                    "Weight-GrossWeightValue",
                    "GrossWeightUom-GrossWeightUom",
                    "ApplicationPrice-PackageDeclaredValue",
                    "PackageCurrencyCode-PackageCurrencyCode",
                    "ApplicationName-ContentDescription",
                    "PurchasingUrl-PurchasingUrl",
                    "Remark1-Reference1",
                    "Remark2-Reference2",
                    "TrackingNumber-TrackingNumber",
                    "HouseAirWaybillNumber-HouseAirWaybillNumber",
                    "SkuTouchFulfillmentNo-SkuTouchFulfillmentNo",
                    "CarrierServiceCode-CarrierServiceCode",
                    "Printed-Printed",
                    "ValidationMessage-ValidationMessage"
                };
            string excelName = "中美专线导出模版" + "-" + DateTime.Now.ToString("hh-mm-ss");
            ExportExcelByWeb.WriteToDownLoad(excelName, "Sheet1", models, titleList, null);
            return View(WayBillExcelExportBindList(param.FilterModel));
        }

        /// <summary>
        /// 欧洲专线模板导出
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [System.Web.Mvc.ActionName("WayBillExcelExportList")]
        [FormValueRequired("btnEuropeExport")]
        public ActionResult EuropeWayBillExport(WayBillListViewModel param)
        {

            List<EuropeWayBill> models = new List<EuropeWayBill>();
            var shippingMethodList = _freightService.GetShippingMethodList(true);
            //得到运单号
            ExportWayBillList(param.FilterModel).WayBillInfoModels.ForEach(WayBillInfoModel =>
            {
                EuropeWayBill model = new EuropeWayBill();
                //申报信息
                List<ApplicationInfoModel> applicationInfoModels =
                   _orderService.GetApplicationInfoByWayBillNumber(WayBillInfoModel.WayBillNumber)
                                .ToModelAsCollection<ApplicationInfo, ApplicationInfoModel>();
                //订单
                CustomerOrderInfoModel customerOrderInfos = _orderService.GetCustomerOrderInfoById(WayBillInfoModel.CustomerOrderID).ToModel<CustomerOrderInfoModel>();
                if (customerOrderInfos != null)
                {
                    model.CustomerOrderNumber = customerOrderInfos.CustomerOrderNumber;
                }
                else
                {
                    model.CustomerOrderNumber = "";
                }
                model.WayBillNumber = WayBillInfoModel.WayBillNumber;
                model.Currency = "EUR";
                if (WayBillInfoModel.InShippingMethodID != null)
                {
                    var shippingMethod = shippingMethodList.FirstOrDefault(p => p.ShippingMethodId == WayBillInfoModel.InShippingMethodID);
                    if (shippingMethod != null)
                    {
                        model.ShippingMethod = shippingMethod.Code;
                    }
                }
                ShippingInfoModel shippingInfo = _orderService.GetshippingInfoById(WayBillInfoModel.ShippingInfoID).ToModel<ShippingInfoModel>();
                if (shippingInfo != null)
                {
                    model.CountryCode = shippingInfo.CountryCode;
                    switch (model.CountryCode.ToUpperInvariant())
                    {
                        case "IT":
                            model.CountryCode = "ITA";
                            break;
                        case "DE":
                            model.CountryCode = "DEU";
                            break;
                        case "ES":
                            model.CountryCode = "ESP";
                            break;
                        case "FR":
                            model.CountryCode = "FRA";
                            break;
                        case "GB":
                            model.CountryCode = "GBR";
                            break;
                    }

                    model.ShippingName = shippingInfo.ShippingFirstName + " " + shippingInfo.ShippingLastName;
                    //地址

                    model.ShippingAddress = shippingInfo.ShippingAddress;
                    model.ShippingCity = shippingInfo.ShippingCity;
                    model.ShippingState = shippingInfo.ShippingState;
                    model.ShippingZip = shippingInfo.ShippingZip;
                    if (!string.IsNullOrWhiteSpace(shippingInfo.ShippingPhone))
                    {
                        model.ShippingPhone = Regex.Replace(shippingInfo.ShippingPhone, @"[^\d]*", "");
                    }
                    model.ShippingCompany = shippingInfo.ShippingCompany;
                }
                else
                {
                    model.CountryCode = "";
                    model.ShippingName = "";
                    //地址
                    model.ShippingAddress = "";
                    model.ShippingCity = "";
                    model.ShippingState = "";
                    model.ShippingZip = "";
                    model.ShippingPhone = "";
                    model.ShippingCompany = "";
                }
                if (applicationInfoModels != null)
                {
                    applicationInfoModels.ForEach(p =>
                        {

                            EuropeWayBill europeWayBill = new EuropeWayBill();
                            model.CopyTo(europeWayBill);
                            europeWayBill.HsCode = p.HSCode;
                            europeWayBill.ApplicationName = p.ApplicationName;
                            if (!string.IsNullOrWhiteSpace(p.Remark))
                            {
                                europeWayBill.ApplicationRemark = Regex.Replace(p.Remark, @"[^0-9a-zA-Z]*", "");
                            }
                            europeWayBill.Qty = p.Qty;
                            europeWayBill.UnitPrice = (p.UnitPrice * p.Qty) ?? 0;
                            europeWayBill.UnitWeight = (p.UnitWeight * p.Qty) ?? 0;
                            europeWayBill.ProductUrl = p.ProductUrl;
                            models.Add(europeWayBill);
                        });
                }
                else
                {
                    model.HsCode = "";
                    model.ApplicationName = "";
                    model.ApplicationRemark = "";
                    model.Qty = 0;
                    model.UnitPrice = 0;
                    model.UnitWeight = 0;
                    models.Add(model);
                }
            });

            var titleList = new List<string>
                {
                    "WayBillNumber-Order Number",
                    "CustomerOrderNumber-Package Barcode",
                    "PrealertReference-Prealert Reference",
                    "ShippingMethod-Shipping Method",
                    "UnitWeight-Weiht",
                    "ShippingCompany-CompanyName",
                    "ShippingName-ConsigneeName",
                    "ShippingPhone-PhoneNumber",
                    "ShippingEmail-EmailAddress",
                    "ShippingAddress-Street",
                    "HouseNumber-HouseNumber",
                    "HouseNumberExtension-HouseNumberExtension",
                    "AdditionalAddressInfo-AdditionalAddressInfo",
                    "ShippingCity-CityOrTown",
                    "ShippingState-StateOrProvince",
                    "ShippingZip-ZIPCode",
                    "CountryCode-CountryCode",
                    "ApplicationRemark-SKUCode1",
                    "ApplicationName-SKUDescription1",
                    "Qty-Quantity1",
                    "Currency-Currency1",
                    "UnitPrice-Price1",
                    "ShippingCosts-ShippingCosts",
                    "HsCode-Hs Code",
                    "ProductUrl-Website"
                };
            string excelName = "欧洲专线导出模版" + "-" + DateTime.Now.ToString("hh-mm-ss");
            ExportExcelByWeb.WriteToDownLoad(excelName, "Sheet1", models, titleList, null);
            return View(WayBillExcelExportBindList(param.FilterModel));
        }

        /// <summary>
        /// EUB模板导出
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [System.Web.Mvc.ActionName("WayBillExcelExportList")]
        [FormValueRequired("btnEUBExport")]
        public ActionResult EUBWayBillExport(WayBillListViewModel param)
        {

            List<EUBWayBill> models = new List<EUBWayBill>();
            var countryList = _countryService.GetCountryList("");
            models = _orderService.GetExportWayBillInfo(new OrderListParam()
            {
                CountryCode = param.FilterModel.CountryCode,
                CustomerCode = param.FilterModel.CustomerCode,
                DateWhere = param.FilterModel.DateWhere,
                EndTime = param.FilterModel.EndTime,
                SearchWhere = param.FilterModel.SearchWhere,
                SearchContext = param.FilterModel.SearchContext,
                ShippingMethodId = param.FilterModel.ShippingMethodId,
                IsHold = param.FilterModel.IsHold,
                Status = param.FilterModel.Status,
                StartTime = param.FilterModel.StartTime,
            }).ToModelAsCollection<WayBillInfo, EUBWayBill>();

            int number = 1;
            models.ForEach(model =>
            {
                model.Number = number;
                if (model.SensitiveTypeID != null && model.SensitiveTypeID < 3)
                {
                    model.IsCharged = "是";
                }
                else
                {
                    model.IsCharged = "否";
                }
                var fileContentResult = BarCode128(model.TrackingNumber) as FileContentResult;
                if (fileContentResult != null)
                    model.TrackingNumberFile = fileContentResult.FileContents;

                if (countryList.FirstOrDefault(p => p.CountryCode == model.CountryCode) != null)
                {

                    model.CountryCode = countryList.First(p => p.CountryCode == model.CountryCode).ChineseName;
                }
                else
                {
                    model.CountryCode = "";
                }
                number++;
            });
            var titleList = new List<string>
                {
                    "Number-序号",
                    "TrackingNumberCode-跟踪号条码",
                    "CreatedOn-创建日期",
                    "InStorageCreatedOn-收货日期",
                    "OutStorageCreatedOn-发货日期",
                    "TrackingNumber-跟踪号",
                    "CountryCode-寄达国家",
                    "SettleWeight-结算重量",
                    "IsCharged-是否带电",
                    "Online-线上/线下"
                    

                };
            string excelName = "EUB模板导出" + "-" + DateTime.Now.ToString("hh-mm-ss");
            ExportExcelByWeb.EUBListExcel(excelName, models, titleList, false);
            return View(WayBillExcelExportBindList(param.FilterModel));
        }

        public ActionResult BarCode128(string code, int dpix = 80, int dpiy = 75, bool showText = false, float angleF = 0)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return Content("无号码");
            }
            var builder = new BarCodeBuilder { SymbologyType = Symbology.Code128, CodeText = code };
            builder.Resolution.DpiX = dpix;
            builder.Resolution.DpiY = dpiy;

            builder.CodeTextFont = new Font("宋体", 12, FontStyle.Regular);

            if (!showText) builder.CodeLocation = CodeLocation.None;
            builder.RotationAngleF = angleF;
            Image image = builder.BarCodeImage;
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            image.Dispose();
            byte[] bytes = ms.ToArray();
            ms.Dispose();
            return File(bytes, "image/Bmp");
        }




        #endregion

        #region 绑定渠道发货配置 Add By zhengsong

        public ActionResult OutStorageShippinMethodConfigure()
        {
            OutStorageConfigureViewModel model = new OutStorageConfigureViewModel();
            return View(BindOutStorageConfigure(model));
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        [FormValueRequired("Save")]
        //[System.Web.Mvc.ActionName("SelectOutStorageShippinMethodConfigure")]
        public ActionResult OutStorageShippinMethodConfigure(OutStorageConfigureViewModel model)
        {
            DeliveryChannelConfiguration dmodel = new DeliveryChannelConfiguration();
            string id = model.OutStorageConfigureModel.InShippingMethodId.ToString();
            string name = GetShippingMethodSelectList().Find(p => p.Value == id).Text;
            dmodel.InShippingMethodId = model.OutStorageConfigureModel.InShippingMethodId;
            dmodel.InShippingMethodName = name;
            dmodel.VenderId = model.OutStorageConfigureModel.VenderId;
            dmodel.VenderName = model.OutStorageConfigureModel.VenderName;
            dmodel.OutShippingMethodId = model.OutStorageConfigureModel.OutShippingMethodId;
            dmodel.OutShippingMethodName = model.OutStorageConfigureModel.OutShippingMethodName;
            dmodel.CreatedOn = DateTime.Now;
            dmodel.CreatedBy = _workContext.User.UserUame;

            //#region 操作日志
            ////yungchu
            ////敏感字：发货服务商，发货渠道

            //StringBuilder sbBuilder = new StringBuilder();
            //sbBuilder.Append("");

            //string venderName = _outStorageService.GetDeliveryChannelConfigurations(model.OutStorageConfigureModel.InShippingMethodId)[0].VenderName;
            //string outShippingmethodName = _outStorageService.GetDeliveryChannelConfigurations(model.OutStorageConfigureModel.InShippingMethodId)[0].OutShippingMethodName;

            //if (venderName != model.OutStorageConfigureModel.VenderName)
            //{
            //	sbBuilder.AppendFormat("服务商从{0}更改为{1}", venderName, model.OutStorageConfigureModel.VenderName);
            //}
            //if (outShippingmethodName != model.OutStorageConfigureModel.OutShippingMethodName)
            //{
            //	sbBuilder.AppendFormat(" 渠道从{0}更改为{1}", outShippingmethodName, model.OutStorageConfigureModel.OutShippingMethodName);
            //}


            //BizLog bizlog = new BizLog()
            //{
            //	Summary = sbBuilder.ToString() != "" ? "[发货渠道绑定]" + sbBuilder : "发货渠道绑定",
            //	KeywordType = KeywordType.ShippingMethodId,
            //	Keyword = model.OutStorageConfigureModel.InShippingMethodId.ToString(),
            //	UserCode = _workContext.User.UserUame,
            //	UserRealName = _workContext.User.UserUame,
            //	UserType = UserType.LMS_User,
            //	SystemCode = SystemType.LMS,
            //	ModuleName = "发货渠道绑定"
            //};

            //_operateLogServices.WriteLog(bizlog, model);

            //#endregion

            var result = _outStorageService.AddDeliveryChannelConfiguration(dmodel);

            if (result)
            {
                SetViewMessage(ShowMessageType.Success, "添加成功", false);
            }
            else
            {
                SetViewMessage(ShowMessageType.Error, "添加失败", false);
            }
            return View(BindOutStorageConfigure(model));
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="inshippingMethodId"></param>
        /// <returns></returns>
        public ActionResult SelectOutStorageShippinMethodConfigure(string inshippingMethodId)
        {
            OutStorageConfigureViewModel model = new OutStorageConfigureViewModel();
            model.OutStorageConfigureModel.InShippingMethodId = int.Parse(inshippingMethodId);
            return View("OutStorageShippinMethodConfigure", BindOutStorageConfigure(model));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="inshippingMethodId"></param>
        /// <param name="deliveryChannelConfigurationId"></param>
        /// <returns></returns>
        public ActionResult DeleteOutStorageShippinMethodConfigure(string inshippingMethodId, int deliveryChannelConfigurationId)
        {
            OutStorageConfigureViewModel model = new OutStorageConfigureViewModel();
            model.OutStorageConfigureModel.InShippingMethodId = int.Parse(inshippingMethodId);
            var result = _outStorageService.DeleteDeliveryChannelConfiguration(deliveryChannelConfigurationId);
            if (result)
            {
                SetViewMessage(ShowMessageType.Success, "删除成功", false);
            }
            else
            {
                SetViewMessage(ShowMessageType.Error, "删除失败", false);
            }
            return View("OutStorageShippinMethodConfigure", BindOutStorageConfigure(model));
        }

        #endregion

        #region 已发货运单 Add By zhengosng /2014-09-12

        public ActionResult ShippingWayBillList(ShippingWayBillFilterModel filterModel)
        {
            if (filterModel.DateWhere == 0)
            {
                if (filterModel.StartTime == null)
                {
                    filterModel.StartTime = DateTime.Parse(DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd") + " " + "00:00");
                }
                if (filterModel.EndTime == null)
                {
                    filterModel.EndTime = DateTime.Parse(DateTime.Now.AddDays(+1).ToString("yyyy-MM-dd") + " " + "00:00");
                }
                filterModel.DateWhere = 3;
                filterModel.PageSize = 300;
            }
            return View(BindShippingWayBill(filterModel));
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("ShippingWayBillList")]
        [FormValueRequired("btnSearch")]
        public ActionResult SelectShippingWayBillList(ShippingWayBillListViewModel model)
        {
            model.FilterModel.Page = 1;
            return View(BindShippingWayBill(model.FilterModel));
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("ShippingWayBillList")]
        [FormValueRequired("btnExport")]
        public ActionResult ExportShippingWayBillList(ShippingWayBillListViewModel viewModel)
        {
            List<ExportWayBillModel> models = new List<ExportWayBillModel>();
            List<string> wayBillNumbers = new List<string>();
            var countryList = _countryService.GetCountryList("");
            var customerList = _customerService.GetCustomerList("", false);
            ExportWayBillModel model = new ExportWayBillModel();
            ShippingWayBillParam param = new ShippingWayBillParam();
            param.SearchWhere = viewModel.FilterModel.SearchWhere;
            param.SearchContext = viewModel.FilterModel.SearchContext;
            param.StartTime = viewModel.FilterModel.StartTime;
            param.EndTime = viewModel.FilterModel.EndTime;
            param.InShippingMehtodId = viewModel.FilterModel.InShippingMehtodId;
            param.OutShippingMehtodId = viewModel.FilterModel.OutShippingMehtodId;
            param.OutCreateBy = viewModel.FilterModel.OutCreateBy;
            param.VenderCode = viewModel.FilterModel.VenderCode;

            var ShippingWayBillList = _orderService.GetShippingWayBillList(param).ToModelAsCollection<ShippingWayBillExt, ShippingWayBillListModel>();

            //得到运单号
            int MaxSubColum = 0;
            ShippingWayBillList.ForEach(WayBillInfoModel =>
            {
                model = new ExportWayBillModel();
                CustomerOrderInfoModel customerOrderInfos = _orderService.GetCustomerOrderInfoById(WayBillInfoModel.CustomerOrderID).ToModel<CustomerOrderInfoModel>();
                if (customerOrderInfos != null)
                {
                    model.CustomerOrderNumber = customerOrderInfos.CustomerOrderNumber;
                    model.CustomerCode = customerOrderInfos.CustomerCode;
                    if (customerList.FirstOrDefault(p => p.CustomerCode == model.CustomerCode) != null)
                    {
                        model.Name = customerList.First(p => p.CustomerCode == model.CustomerCode).Name;
                    }
                    else
                    {
                        model.Name = "";
                    }
                    model.InsureAmount = customerOrderInfos.InsureAmount;
                    model.PackageNumber = customerOrderInfos.PackageNumber;
                    model.AppLicationType = CustomerOrder.GetApplicationTypeDescription(customerOrderInfos.AppLicationType);
                }
                else
                {
                    model.CustomerOrderNumber = "";
                }
                model.WayBillNumber = WayBillInfoModel.WayBillNumber;
                model.InShippingMethodName = WayBillInfoModel.InShippingMethodName;
                model.TrackingNumber = WayBillInfoModel.TrackingNumber;
                model.Weight = WayBillInfoModel.Weight;
                model.SettleWeight = WayBillInfoModel.SettleWeight;
                model.Length = WayBillInfoModel.Length;
                model.Width = WayBillInfoModel.Width;
                model.Height = WayBillInfoModel.Height;
                model.WayCreatedOn = WayBillInfoModel.CreatedOn.ToString();
                model.ShiCreatedOn = WayBillInfoModel.InStorageCreatedOn.ToString();
                model.SenCreatedOn = WayBillInfoModel.OutStorageCreatedOn.ToString();
                model.Status = WayBill.GetStatusDescription(WayBillInfoModel.Status);
                //是否关税预付
                model.EnableTariffPrepay = WayBillInfoModel.EnableTariffPrepay;

                ShippingInfoModel shippingInfo = _orderService.GetshippingInfoById(WayBillInfoModel.ShippingInfoID).ToModel<ShippingInfoModel>();
                SenderInfoModel senderInfo = _orderService.GetSenderInfoById(WayBillInfoModel.SenderInfoID).ToModel<SenderInfoModel>();
                if (shippingInfo != null)
                {
                    model.CountryCode = shippingInfo.CountryCode;
                    if (countryList.First(p => p.CountryCode == model.CountryCode) != null)
                    {
                        model.ChineseName = countryList.First(p => p.CountryCode == model.CountryCode).ChineseName;
                    }
                    else
                    {
                        model.ChineseName = "";
                    }
                    model.ShippingFirstName = shippingInfo.ShippingFirstName;
                    model.ShippingLastName = shippingInfo.ShippingLastName;
                    model.ShippingAddress = shippingInfo.ShippingAddress + " " + shippingInfo.ShippingAddress1 + " " + shippingInfo.ShippingAddress2;
                    model.ShippingCity = shippingInfo.ShippingCity;
                    model.ShippingState = shippingInfo.ShippingState;
                    model.ShippingZip = shippingInfo.ShippingZip;
                    model.ShippingPhone = shippingInfo.ShippingPhone;
                    model.ShippingCompany = shippingInfo.ShippingCompany;
                    model.ShippingTaxId = shippingInfo.ShippingTaxId;
                }
                else
                {
                    model.CountryCode = "";
                    model.ChineseName = "";
                    model.ShippingFirstName = "";
                    model.ShippingLastName = "";
                    model.ShippingAddress = "";
                    model.ShippingCity = "";
                    model.ShippingState = "";
                    model.ShippingZip = "";
                    model.ShippingPhone = "";
                    model.ShippingCompany = "";
                    model.ShippingTaxId = "";
                }
                if (senderInfo != null)
                {
                    model.SenderFirstName = senderInfo.SenderFirstName;
                    model.SenderLastName = senderInfo.SenderLastName;
                    model.SenderCompany = senderInfo.SenderCompany;
                    model.SenderAddress = senderInfo.SenderAddress;
                    model.SenderCity = senderInfo.SenderCity;
                    model.SenderState = senderInfo.SenderState;
                    model.SenderZip = senderInfo.SenderZip;
                    model.SenderPhone = senderInfo.SenderPhone;
                }
                else
                {
                    model.SenderFirstName = "";
                    model.SenderLastName = "";
                    model.SenderCompany = "";
                    model.SenderAddress = "";
                    model.SenderCity = "";
                    model.SenderState = "";
                    model.SenderZip = "";
                    model.SenderPhone = "";
                }
                model.IsReturn = WayBillInfoModel.IsReturn;
                InsuredCalculationModel insuredCalculation = _orderService.GetInsuredCalculationById(WayBillInfoModel.InsuredID).ToModel<InsuredCalculationModel>();
                if (insuredCalculation != null)
                {
                    model.InsuredName = insuredCalculation.InsuredName;
                }
                else
                {
                    model.InsuredName = "";
                }
                CustomerOrderInfoModel customerOrderInfo = _orderService.GetCustomerOrderInfoById(WayBillInfoModel.CustomerOrderID).ToModel<CustomerOrderInfoModel>();
                SensitiveTypeInfoModel sensitiveTypeInfo = _orderService.GetSensitiveTypeInfoById(customerOrderInfo.SensitiveTypeID).ToModel<SensitiveTypeInfoModel>();
                if (customerOrderInfos != null && sensitiveTypeInfo != null)
                {
                    model.SensitiveTypeName = sensitiveTypeInfo.SensitiveTypeName;
                }
                else
                {
                    model.SensitiveTypeName = "";
                }


                List<ApplicationInfoModel> applicationInfoModels =
                    _orderService.GetApplicationInfoByWayBillNumber(WayBillInfoModel.WayBillNumber)
                                 .ToModelAsCollection<ApplicationInfo, ApplicationInfoModel>();
                model.ApplicationInfoModels = applicationInfoModels;
                if (applicationInfoModels != null)
                {
                    if (applicationInfoModels.Count >= MaxSubColum)
                    {
                        MaxSubColum = applicationInfoModels.Count;
                    }
                }
                models.Add(model);
                wayBillNumbers.Add(WayBillInfoModel.WayBillNumber.ToString());
            });
            List<string> lstName = new List<string>
                {
                    "客户订单号",
                    "客户代码",
                    "客户名称",
                    "运单号",
                    "入仓运输方式",
                    "跟踪号",
                    "国家简码",
                    "国家中文名",
                    "收件人姓",
                    "收件人名字",
                    "收件人公司",
                    "收货地址",
                    "城市",
                    "省/州",
                    "邮编",
                    "电话",
                    "创建时间",
                    "收货时间",
                    "发货时间",
                    "状态",
                    "收件人税号",
                    "发件人姓",
                    "发件人名",
                    "发件人公司",
                    "发件人地址",
                    "城市",
                    "省/州",
                    "发件人邮编",
                    "发件人电话",
                    "是否退回",
                    "保险类型",
                    "保险价值RMB",
                    "敏感货物",
                    "申报类型",
                    "件数",
                    "长cm",
                    "宽cm",
                    "高cm",
                    "称重重量kg",
                    "结算重量kg",
					"是否关税预付"
                };
            for (int i = 1; i <= MaxSubColum; i++)
            {
                lstName.Add("申报名称" + i);
                lstName.Add("申报中文名称" + i);
                lstName.Add("海关编码" + i);
                lstName.Add("数量" + i);
                lstName.Add("单价" + i + "(usd)");
                lstName.Add("净重量" + i + "(kg)");
                lstName.Add("销售链接" + i);
                lstName.Add("备注" + i);
            }
            string fileName = sysConfig.ExcelTemplateWebPath + sysConfig.ExportWayBill;
            ExportExcelByWeb.ListWayBillExcel(fileName, models, lstName);

            return View(BindShippingWayBill(viewModel.FilterModel));
        }


        public ActionResult UpdateOutStorageInfo(string type)
        {
            UpdateOutStorageViewModel model = new UpdateOutStorageViewModel();
            model.Type = type;
            return View(model);
        }

        //修改出仓渠道
        //1-代表按照出仓单号整批修改
        //2-代表从已发货界面修改
        //3-代表从按查询条件修改
        public JsonResult JsonUpdateOutStorageInfo(string outStorageId, string outshippingMethodId, string outshippingMethodName, string outvenderCode, string remark, string type, string filterModel)
        {
            var result = new ResponseResult();
            try
            {

                ShippingWayBillFilterModel filter = new ShippingWayBillFilterModel();
                if (filterModel != null)
                {
                    filter = JsonHelper.JsonDeserialize<ShippingWayBillFilterModel>(filterModel);
                }
                else
                {
                    filter = null;
                }
                int shiId = int.Parse(outshippingMethodId);
                if (type == "1")
                {
                    var ret = _outStorageService.UpdateOutStorageInfo(outStorageId, shiId, outshippingMethodName, outvenderCode, remark);
                    if (ret.Result)
                    {
                        result.Result = true;
                    }
                    else
                    {
                        result.Result = false;
                        result.Message = "修改失败，请重试！";
                        if (!ret.Result && ret.Message != "")
                        {
                            result.Message = ret.Message;
                        }
                    }
                }
                else if (type == "2")
                {
                    List<string> outStorageIdList = new List<string>();
                    //修改运单，创建新出仓单
                    var ret = _outStorageService.UpdateOutStorageInfoAll(outStorageId, shiId, outshippingMethodName, outvenderCode, remark, out outStorageIdList);
                    //修改老出仓单信息
                    var retOutStorage = _outStorageService.UpdateOldOutStorageInfo(outStorageIdList);
                    if (ret.Result && retOutStorage)
                    {
                        result.Result = true;
                    }
                    else
                    {
                        result.Result = false;
                        result.Message = "修改失败，请重试!";
                        if (!ret.Result && ret.Message != "")
                        {
                            result.Message = ret.Message;
                        }
                    }
                }
                else if (type == "3" && filter != null)
                {
                    List<string> outStorageIdList = new List<string>();
                    ShippingWayBillParam param = new ShippingWayBillParam();
                    param.SearchWhere = filter.SearchWhere;
                    param.SearchContext = filter.SearchContext;
                    param.StartTime = filter.StartTime;
                    param.EndTime = filter.EndTime;
                    param.InShippingMehtodId = filter.InShippingMehtodId;
                    param.OutShippingMehtodId = filter.OutShippingMehtodId;
                    param.OutCreateBy = filter.OutCreateBy;
                    param.VenderCode = filter.VenderCode;
                    //修改运单，创建新出仓单
                    var wayBillList = _orderService.GetAllShippingWayBillList(param);

                    var ret = _outStorageService.UpdateOutStorageInfoAll(wayBillList, shiId, outshippingMethodName, outvenderCode, remark, out outStorageIdList);
                    //修改老出仓单信息
                    var retOutStorage = _outStorageService.UpdateOldOutStorageInfo(outStorageIdList);
                    if (ret.Result && retOutStorage)
                    {
                        result.Result = true;
                    }
                    else
                    {
                        result.Result = false;
                        result.Message = "修改失败，请重试!";
                        if (!ret.Result && ret.Message != "")
                        {
                            result.Message = ret.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private ShippingWayBillListViewModel BindShippingWayBill(ShippingWayBillFilterModel filter)
        {
            ShippingWayBillListViewModel model = new ShippingWayBillListViewModel();
            model.FilterModel = filter;
            model.FilterModel.Filter = "";
            if (filter.SearchWhere == null)
            {
                WayBill.GetSearchFilterList().ForEach(p =>
                {
                    if (p.ValueField != "4")
                    {
                        model.SearchWheres.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField });
                    }
                });
                model.SearchWheres.FirstOrDefault(m => m.Value == "5").Selected = true;
            }
            else
            {
                WayBill.GetSearchFilterList().ForEach(p =>
                {
                    if (p.ValueField != "4")
                    {
                        model.SearchWheres.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = filter.SearchWhere.HasValue && p.ValueField == filter.SearchWhere.Value.ToString() });
                    }
                });
            }

            ShippingWayBillParam param = new ShippingWayBillParam();
            param.SearchWhere = filter.SearchWhere;
            param.SearchContext = filter.SearchContext;
            param.StartTime = filter.StartTime;
            param.EndTime = filter.EndTime;
            param.InShippingMehtodId = filter.InShippingMehtodId;
            param.OutShippingMehtodId = filter.OutShippingMehtodId;
            param.OutCreateBy = filter.OutCreateBy;
            param.VenderCode = filter.VenderCode;
            param.Page = filter.Page;
            param.PageSize = filter.PageSize;
            model.FilterModel.Filter = JsonHelper.JsonSerializer(filter);
            model.PagedList = _orderService.GetShippingWayBillPagedList(param).ToModelAsPageCollection<ShippingWayBillExt, ShippingWayBillListModel>();
            List<string> wayBillList = new List<string>();
            model.PagedList.ToList().ForEach(p => wayBillList.Add(p.WayBillNumber));
            var AllWayBill = _orderService.GetIsUpdateShippingWayBillList(wayBillList);
            foreach (var row in model.PagedList)
            {
                if (AllWayBill != null && AllWayBill.Contains(row.WayBillNumber))
                {
                    row.IsUpdate = true;
                }
            }
            return model;
        }

        #endregion


        #region 运单修改 add by yungchu

        public ActionResult WaybillInfoUpdate(WaybillInfoUpdateFilterModel filterModel)
        {
            if (!filterModel.IsFirstIn)//第一次进入
            {
                WaybillInfoUpdateViewModel model = new WaybillInfoUpdateViewModel();
                model.FilterModel.IsFirstIn = true;
                model.FilterModel.StartTime = DateTime.Parse(DateTime.Now.AddDays(1).AddMonths(-1).ToString("yyyy-MM-dd"));
                model.FilterModel.EndTime = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
                BindComboList(model, filterModel);//下拉框绑定数据

                return View(model);

            }
            return View(WaybillInfoUpdateDataBind(filterModel));

        }


        [System.Web.Mvc.HttpPost]
        [FormValueRequired("btnSearch")]
        public ActionResult WaybillInfoUpdate(WaybillInfoUpdateViewModel model)
        {
            return View(WaybillInfoUpdateDataBind(model.FilterModel));
        }


        //返回数据
        public WaybillInfoUpdateViewModel WaybillInfoUpdateDataBind(WaybillInfoUpdateFilterModel filterModel)
        {

            var model = new WaybillInfoUpdateViewModel
            {
                FilterModel = filterModel,
                PagedList = _orderService.GetWaybillInfoUpdatePagedList(new WaybillInfoUpdateParam()
                {
                    Page = filterModel.Page,
                    PageSize = filterModel.PageSize,
                    CustomerCode = filterModel.CustomerCode,
                    ShippingMethodId = filterModel.ShippingMethodId,
                    SearchWhere = filterModel.SearchWhere,
                    SearchContext = filterModel.SearchContext,
                    StartTime = filterModel.StartTime,
                    EndTime = filterModel.EndTime,
                    Status = filterModel.Status
                }).ToModelAsPageCollection<WaybillInfoUpdateExt, WaybillInfoUpdateModel>()

            };

            BindComboList(model, filterModel);

            return model;
        }

        //绑定数据
        public WaybillInfoUpdateViewModel BindComboList(WaybillInfoUpdateViewModel model, WaybillInfoUpdateFilterModel filterModel)
        {
            string submittedStutas = ((int)WayBill.StatusEnum.Submitted).ToString();
            string haveStatus = ((int)WayBill.StatusEnum.Have).ToString();

            string customerOrderNumbeWhere = ((int)WayBill.SearchFilterEnum.CustomerOrderNumber).ToString();
            string wayBillNumberWhere = ((int)WayBill.SearchFilterEnum.WayBillNumber).ToString();
            string trackingNumbereWhere = ((int)WayBill.SearchFilterEnum.TrackingNumber).ToString();


            model.StatusList.Add(new SelectListItem() { Text = "请选择", Value = "", Selected = !filterModel.Status.HasValue });
            WayBill.GetStatusList().ForEach(p =>
            {
                if (p.ValueField == submittedStutas || p.ValueField == haveStatus)
                {
                    model.StatusList.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = filterModel.Status.HasValue && p.ValueField == filterModel.Status.Value.ToString() });
                }
            });


            WayBill.GetSearchFilterList().ForEach(p =>
            {
                if (p.ValueField == customerOrderNumbeWhere || p.ValueField == wayBillNumberWhere || p.ValueField == trackingNumbereWhere)
                {
                    model.SearchWheres.Add(new SelectListItem()
                    {
                        Text = p.TextField,
                        Value = p.ValueField,
                        Selected = filterModel.Status.HasValue && p.ValueField == filterModel.Status.Value.ToString()
                    });
                }
            });


            //运输方式
            model.ShippingMethodLists = GetShippingMethodSelectList();


            //操作权限
            model.DisplayBatchHold = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.BatchHoldOn);
            model.DisplayCancelHold = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.BatchCancelAbnormalWayBill);
            model.DisPlayModifyShippingMethod = _userService.Authorize(_workContext.User.UserUame,
                                                           PermissionRecords.BatchModifyShippingMethod);
            return model;
        }


        /// <summary>
        /// 批量修改运输方式(已提交，已收货进行不同逻辑)
        /// 2014/09/23 add by yungchu
        /// </summary>
        /// <param name="wayBillNumbers"></param>
        /// <param name="shippingMethodId"></param>
        /// <param name="shippingMethodName"></param>
        /// <returns></returns>
        [ButtonPermissionValidator(PermissionRecords.BatchModifyShippingMethod)]
        public JsonResult BatchModifyShippingMethodByStatus(string wayBillNumbers, int? shippingMethodId, string shippingMethodName)
        {

            var model = new ResponseResult();
            var submittedStutas = (int)WayBill.StatusEnum.Submitted;
            var haveStatus = (int)WayBill.StatusEnum.Have;
            List<WayBillInfo> listWayBillInfo = new List<WayBillInfo>();
            List<string> submitWaybillList = new List<string>();
            List<string> haveWaybillList = new List<string>();


            try
            {
                if (!string.IsNullOrWhiteSpace(wayBillNumbers))
                {
                    var arr = wayBillNumbers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    listWayBillInfo = _wayBillInfoRepository.GetList(a => arr.Contains(a.WayBillNumber) && (a.Status == submittedStutas || a.Status == haveStatus));

                    listWayBillInfo.ForEach(a =>
                    {
                        //已提交
                        if (a.Status == submittedStutas)
                        {
                            submitWaybillList.Add(a.WayBillNumber);
                        }//已收货
                        else
                        {
                            haveWaybillList.Add(a.WayBillNumber);
                        }
                    });


                    if (submitWaybillList.Count != 0)
                    {
                        _orderService.BatchUpdateWayBillInfo(submitWaybillList, shippingMethodId, shippingMethodName);
                    }
                    if (haveWaybillList.Count != 0)
                    {
                        _orderService.OperateWaybillByFee(haveWaybillList, shippingMethodId, shippingMethodName);
                    }

                    model.Result = true;
                }
                else
                {
                    model.Result = false;
                    model.Message = "请选择需要修改运输方式的运单";
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



        #endregion

        #region 运单修改 zxq

        [System.Web.Mvc.HttpGet]
        public ActionResult ModifWayBillDetail(string wayBillNumber)
        {
            var wayBillInfo = _wayBillInfoRepository.Get(wayBillNumber);

            var customer = _customerService.GetCustomer(wayBillInfo.CustomerCode);

            //add by yungchu
            var customerOrderInfo = new CustomerOrderInfo();
            if (wayBillInfo.CustomerOrderID.HasValue)
            {
                customerOrderInfo = _customerOrderService.Get(wayBillInfo.CustomerOrderID.Value);
            }

            WayBillBusinessDateInfo wayBillBusinessDateInfo = _inStorageService.GetWayBillBusinessDateInfo(wayBillNumber);
            DateTime businessDate = wayBillBusinessDateInfo != null? wayBillBusinessDateInfo.ReceivingDate : System.DateTime.Now;

            var model = new ModifWayBillDetailModel()
                {
                    WayBillNumber = wayBillInfo.WayBillNumber,
                    CustomerOrderNumber = wayBillInfo.CustomerOrderNumber,
                    CustomerCode = wayBillInfo.CustomerCode,
                    CustomerName = customer.Name,
                    CustomerId = customer.CustomerID.ToString(),
                    CustomerTypeId = customer.CustomerTypeID,
                    PackageNumber = customerOrderInfo.PackageNumber.HasValue ? customerOrderInfo.PackageNumber.Value : 0,//件数
                    IsSubmitStatus = wayBillInfo.Status == (int)WayBill.StatusEnum.Submitted,//是否是提交状态
                    ShippingMethodId = wayBillInfo.InShippingMethodID.Value,
                    ShippingName = wayBillInfo.InShippingMethodName,
                    CountryCode = wayBillInfo.CountryCode,
                    CountryChineseName = GetGetCountryListFromCache().First(p => p.CountryCode == wayBillInfo.CountryCode).ChineseName,
                    TrackingNumber = wayBillInfo.TrackingNumber,
                    ShippingInfo = wayBillInfo.ShippingInfo,
                    SenderInfo = wayBillInfo.SenderInfo,
                    AppLicationType = wayBillInfo.CustomerOrderInfo.AppLicationType,
                    ApplicationInfos = wayBillInfo.CustomerOrderInfo.ApplicationInfos.ToList(),
                    IsInsured = wayBillInfo.CustomerOrderInfo.IsInsured,
                    InsuredID = wayBillInfo.CustomerOrderInfo.InsuredID,
                    InsureAmount = wayBillInfo.CustomerOrderInfo.InsureAmount,
                    SensitiveTypeID = wayBillInfo.CustomerOrderInfo.SensitiveTypeID,
                    IsReturn = wayBillInfo.IsReturn,
                    EnableTariffPrepay = wayBillInfo.EnableTariffPrepay,
                    InsuredList = GetInsuredList(wayBillInfo.CustomerOrderInfo.InsuredID),
                    SensitiveTypeList = GetSensitiveTypeList(wayBillInfo.CustomerOrderInfo.SensitiveTypeID),
                    ApplicationTypeList = GetApplicationTypeList(wayBillInfo.CustomerOrderInfo.AppLicationType),
                    BusinessDate = businessDate  //add by yungchu
                };

            model.ReturnUrl = Request["retUrl"] != null ? Server.UrlDecode(Request["retUrl"]) : Url.Action("WaybillInfoUpdate");
            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult SaveWayBillDetail(ModifWayBillDetailModel model)
        {
            ResponseResult responseResult = new ResponseResult();

            var responseCheckShippingMethodCountry = (ResponseResult)((JsonResult)CheckShippingMethodCountry(model.ShippingMethodId, model.CountryCode)).Data;

            if (!responseCheckShippingMethodCountry.Result)
            {
                responseResult.Message += "校验运输方式支持的国家失败\n";
            }

            var responseCheckTrackingNumber = (ResponseResult)((JsonResult)CheckTrackingNumber(model.TrackingNumber, model.WayBillNumber)).Data;

            if (!responseCheckTrackingNumber.Result)
            {
                responseResult.Message += "校验跟踪号失败\n";
            }

            if (model.EnableTariffPrepay)
            {
                var responseCheckTariffPrepayFee = (ResponseResult)((JsonResult)CheckTariffPrepayFee(model.ShippingMethodId, model.CustomerCode)).Data;
                if (!responseCheckTariffPrepayFee.Result)
                {
                    responseResult.Message += "校验关税预付失败\n";
                }
            }

            var responseCheckShippingInfo = (ResponseResult)((JsonResult)CheckShippingInfo(model.ShippingMethodId, model.ShippingInfo)).Data;
            if (!responseCheckShippingInfo.Result)
            {
                responseResult.Message += Regex.Match(responseCheckShippingInfo.Message, "value:'(\\w*)'").Groups[1] + "\n";
            }


            foreach (var applicationInfo in model.ApplicationInfos)
            {
                var responseCheckApplicationInfo = (ResponseResult)((JsonResult)CheckApplicationInfo(model.ShippingMethodId, applicationInfo)).Data;
                if (!responseCheckApplicationInfo.Result)
                {
                    responseResult.Message += Regex.Match(responseCheckApplicationInfo.Message, "value:'(\\w*)'").Groups[1] + "\n";
                }
            }

            if (!string.IsNullOrWhiteSpace(responseResult.Message))
            {
                return Json(responseResult);
            }

            //判断保险值
            if (!model.InsuredID.HasValue)
            {
                model.InsureAmount = null;
            }

            var wayBillInfo = _wayBillInfoRepository.Get(model.WayBillNumber);

            if (!wayBillInfo.IsHold)
            {
                responseResult.Message = "修改运单前，请先Hold";
                return Json(responseResult);
            }

            if (wayBillInfo.Status != (int)WayBill.StatusEnum.Submitted && wayBillInfo.Status != (int)WayBill.StatusEnum.Have)
            {
                responseResult.Message = "运单的状态不为已提交或已收货";
                return Json(responseResult);
            }

            try
            {
           
                WayBillInfo newWayBillInfo = null;
                bool isNewWayBillInfo = false;

                if (wayBillInfo.Status == (int)WayBill.StatusEnum.Submitted)
                {
                    newWayBillInfo = wayBillInfo;
                }
                else if (wayBillInfo.InShippingMethodID == model.ShippingMethodId && wayBillInfo.CountryCode == model.CountryCode && wayBillInfo.EnableTariffPrepay == model.EnableTariffPrepay)
                {
                    newWayBillInfo = wayBillInfo;
                }
                else
                {
                    isNewWayBillInfo = true;
                    //newWayBillInfo = _orderService.OperateWaybillByFee(new List<string>() { model.WayBillNumber }, model.ShippingMethodId, model.ShippingName).FirstOrDefault();
                    newWayBillInfo =
                        _wayBillInfoRepository.Get(
                            _orderService.OperateWaybillByFee(new List<string>() { model.WayBillNumber },
                                                              model.ShippingMethodId, model.ShippingName, model.TrackingNumber)
                                         .FirstOrDefault()
                                         .WayBillNumber);
                }


                if (!isNewWayBillInfo)
                {
                    //如果有新填跟踪号，就取新填的
                    if (!string.IsNullOrWhiteSpace(model.TrackingNumber))
                    {
                        newWayBillInfo.TrackingNumber = model.TrackingNumber;
                        newWayBillInfo.CustomerOrderInfo.TrackingNumber = model.TrackingNumber;
                    }
                    else
                    {
                        //如果没有新填跟踪号，并且运输方式需要分配跟踪号。分配新跟踪号
                        var shippingMethodModel = _freightService.GetShippingMethod(model.ShippingMethodId);
                        if (shippingMethodModel.IsSysTrackNumber)
                        {
                            var trackNumbers = _trackingNumberService.TrackNumberAssignStandard(model.ShippingMethodId, 1, wayBillInfo.CountryCode);
                            if (trackNumbers != null && trackNumbers.Any())
                            {
                                newWayBillInfo.TrackingNumber = trackNumbers.FirstOrDefault();
                                newWayBillInfo.CustomerOrderInfo.TrackingNumber = trackNumbers.FirstOrDefault();
                            }
                            else
                            {
                                throw new ArgumentException("[{0}]运输方式无可分配的跟踪号！".FormatWith(model.ShippingName));
                            }
                    newWayBillInfo.CustomerOrderInfo.AppLicationType = model.AppLicationType;
                        }
                    }
                }

                using (var transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.MaxValue))
                {
					newWayBillInfo.ApplicationInfos.Clear();
	                newWayBillInfo.CustomerOrderInfo.ApplicationInfos.Clear();
					model.ApplicationInfos.ForEach(p =>
						{
							p.Total = p.Qty * p.UnitPrice;
							p.CustomerOrderID = newWayBillInfo.CustomerOrderID;
							p.CreatedBy = _workContext.User.UserUame;
							p.CreatedOn = DateTime.Now;
							p.LastUpdatedOn = DateTime.Now;
							p.LastUpdatedBy = _workContext.User.UserUame;
							newWayBillInfo.ApplicationInfos.Add(p);
						});
		
                    newWayBillInfo.CustomerOrderInfo.PackageNumber = model.PackageNumber;//件数
                    newWayBillInfo.CustomerOrderInfo.AppLicationType = model.AppLicationType;
                    newWayBillInfo.SenderInfo = model.SenderInfo;
					newWayBillInfo.SenderInfo.CountryCode = "CN";
                    newWayBillInfo.CustomerOrderInfo.SenderInfo = newWayBillInfo.SenderInfo;
                    model.ShippingInfo.CountryCode = model.CountryCode;
                    newWayBillInfo.ShippingInfo = model.ShippingInfo;
                    newWayBillInfo.CustomerOrderInfo.ShippingInfo = newWayBillInfo.ShippingInfo;
                    newWayBillInfo.InShippingMethodID = model.ShippingMethodId;
                    newWayBillInfo.CustomerOrderInfo.ShippingMethodId = model.ShippingMethodId;
                    newWayBillInfo.InShippingMethodName = model.ShippingName;
                    newWayBillInfo.CustomerOrderInfo.ShippingMethodName = model.ShippingName;
                    newWayBillInfo.CustomerOrderInfo.ShippingMethodName = model.ShippingName;
                    newWayBillInfo.CustomerOrderInfo.TrackingNumber = model.TrackingNumber;
                    newWayBillInfo.InsuredID = model.InsuredID;
                    newWayBillInfo.CustomerOrderInfo.InsuredID = model.InsuredID;
                    newWayBillInfo.CustomerOrderInfo.SensitiveTypeID = model.SensitiveTypeID;
                    newWayBillInfo.CustomerOrderInfo.IsBattery = model.SensitiveTypeID.HasValue;
                    newWayBillInfo.IsReturn = model.IsReturn;
                    newWayBillInfo.CustomerOrderInfo.IsReturn = model.IsReturn;
                    newWayBillInfo.EnableTariffPrepay = model.EnableTariffPrepay;
                    newWayBillInfo.CustomerOrderInfo.EnableTariffPrepay = model.EnableTariffPrepay;
                    newWayBillInfo.CountryCode = model.CountryCode;
                    newWayBillInfo.IsHold = false;
                    newWayBillInfo.CustomerOrderInfo.IsHold = false;//解除hold
                    if (!isNewWayBillInfo)
                    {
                        newWayBillInfo.AbnormalWayBillLog.AbnormalStatus = (int)WayBill.AbnormalStatusEnum.OK.GetAbnormalStatusValue();//异常状态已完成
                    }
                    newWayBillInfo.CountryCode = model.CountryCode;
                    //更新更改日志
                    _orderService.UpdateWayBillChangeLog(newWayBillInfo, wayBillInfo.WayBillNumber, model.ChangeType.Value, model.ChangeReason);

                    //修改业务日期 add by yungchu
                    _inStorageService.AddWayBillBusinessDateInfos(new WayBillBusinessDateInfo
                    {
                        WayBillNumber = newWayBillInfo.WayBillNumber,
                        ReceivingDate = model.BusinessDate
                    });

	                _wayBillInfoRepository.UnitOfWork.Commit();

                    transaction.Complete();
                }

                responseResult.Result = true;
                return Json(responseResult);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                responseResult.Message = ex.Message;

                return Json(responseResult);
            }
        }

        private List<SelectListItem> GetInsuredList(int? insuredId)
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Value = "", Text = "无", Selected = insuredId == null });
            _insuredCalculationService.GetList().ForEach(c => list.Add(new SelectListItem
            {
                Value = c.InsuredID.ToString(),
                Text = c.InsuredName,
                Selected = insuredId == c.InsuredID

            }));

            return list;
        }

        private List<SelectListItem> GetSensitiveTypeList(int? sensitiveTypeId)
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Value = "", Text = "无", Selected = sensitiveTypeId == null });
            _sensitiveTypeInfoService.GetList().ForEach(c => list.Add(new SelectListItem
            {
                Value = c.SensitiveTypeID.ToString(),
                Text = c.SensitiveTypeName
            }));

            return list;
        }

        private List<SelectListItem> GetGoodsTypeList(int? goodsTypeId)
        {
            var list = new List<SelectListItem>();
            _goodsTypeService.GetList().ForEach(c => list.Add(new SelectListItem
            {
                Value = c.GoodsTypeID.ToString(CultureInfo.InvariantCulture),
                Text = c.GoodsTypeName,
                Selected = goodsTypeId == c.GoodsTypeID,
            }));

            return list;
        }

        private List<SelectListItem> GetApplicationTypeList(int? applicationType)
        {
            var list = new List<SelectListItem>();

            CustomerOrder.GetApplicationTypeList().ForEach(c => list.Add(new SelectListItem
            {
                Value = c.ValueField,
                Text = c.TextField,
                Selected = applicationType == Convert.ToInt32(c.ValueField),
            }));

            return list;
        }

        public ActionResult CheckTrackingNumber(string trackingNumber, string wayBillNumber)
        {
            ResponseResult responseResult = new ResponseResult();

            if (string.IsNullOrWhiteSpace(trackingNumber))
            {
                responseResult.Result = true;
                return Json(responseResult);
            }

            var wayBillInfo = _wayBillInfoRepository.GetWayBillByTrackingNumber(trackingNumber);
            if (wayBillInfo == null || wayBillInfo.Status == (int)WayBill.StatusEnum.Return)
            {
                responseResult.Result = true;
            }
            else if (wayBillInfo.WayBillNumber == wayBillNumber)
            {
                responseResult.Result = true;
            }
            else
            {
                responseResult.Result = false;
            }
            return Json(responseResult);
        }

        public ActionResult CheckShippingMethodCountry(int shippingMethodId, string countryCode)
        {
            ResponseResult responseResult = new ResponseResult();
            responseResult.Result = GetShippingMethodCountriesFromCache(shippingMethodId).Exists(p => p.CountryCode == countryCode);
            return Json(responseResult);
        }

        public ActionResult CheckTariffPrepayFee(int shippingMethodId, string customerCode)
        {
            ResponseResult responseResult = new ResponseResult();
            var listTariffPrepayFee = _freightService.GetShippingMethodsTariffPrepay(customerCode);
            if (listTariffPrepayFee == null)
            {
                responseResult.Result = false;
            }
            else
            {
                responseResult.Result = listTariffPrepayFee.Exists(p => p.ShippingMethodId == shippingMethodId && p.EnableTariffPrepay);
            }
            return Json(responseResult);
        }

        public ActionResult CheckShippingInfo(int shippingMethodId, ShippingInfo shippingInfo)
        {
            ResponseResult responseResult = new ResponseResult();
            var shippingMethod = _freightService.GetShippingMethod(shippingMethodId);
            var errorDic = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(shippingInfo.ShippingFirstName) && string.IsNullOrWhiteSpace(shippingInfo.ShippingLastName))
            {
                errorDic.Add("ShippingInfo_ShippingFirstName", "收件人姓或名至少填一个");
                errorDic.Add("ShippingInfo_ShippingLastName", "收件人姓或名至少填一个");
            }

            if (string.IsNullOrWhiteSpace(shippingInfo.ShippingAddress))
            {
                errorDic.Add("ShippingInfo_ShippingAddress", "收货地址1不能为空");
            }

            if (string.IsNullOrWhiteSpace(shippingInfo.ShippingCity))
            {
                errorDic.Add("ShippingInfo_ShippingCity", "收件人城市不能为空");
            }

            //是否属中美专线
            if (sysConfig.SinoUSShippingMethodCode.Split(',').ToList().Contains(shippingMethod.Code))
            {
                if (!string.IsNullOrWhiteSpace(shippingInfo.ShippingFirstName) && shippingInfo.ShippingFirstName.Length > 35)
                {
                    errorDic.Add("ShippingInfo_ShippingFirstName", "收件人姓的长度不能超过35个字符");
                }

                if (!string.IsNullOrWhiteSpace(shippingInfo.ShippingLastName) && shippingInfo.ShippingLastName.Length > 35)
                {
                    errorDic.Add("ShippingInfo_ShippingLastName", "收件人名的长度不能超过35个字符");
                }

                if (!string.IsNullOrWhiteSpace(shippingInfo.ShippingCity) && shippingInfo.ShippingCity.Length > 35)
                {
                    errorDic.Add("ShippingInfo_ShippingCity", "收件人城市的长度不能超过35个字符");
                }

                if (string.IsNullOrWhiteSpace(shippingInfo.ShippingState))
                {
                    errorDic.Add("ShippingInfo_ShippingState", "收件人省/州不能为空");
                }

                if (!string.IsNullOrWhiteSpace(shippingInfo.ShippingState) && shippingInfo.ShippingState.Length != 2)
                {
                    errorDic.Add("ShippingInfo_ShippingState", "收件人省/州的长度必须为2个字符");
                }

                if (string.IsNullOrWhiteSpace(shippingInfo.ShippingZip))
                {
                    errorDic.Add("ShippingInfo_ShippingZip", "收件人邮编不能为空");
                }

                if (!string.IsNullOrWhiteSpace(shippingInfo.ShippingZip) && (shippingInfo.ShippingZip.Length < 5 || shippingInfo.ShippingZip.Length > 9))
                {
                    errorDic.Add("ShippingInfo_ShippingZip", "收件人邮编的长度为5-9个字符");
                }

                if (!string.IsNullOrWhiteSpace(shippingInfo.ShippingCompany) && shippingInfo.ShippingCompany.Length > 35)
                {
                    errorDic.Add("ShippingInfo_ShippingCompany", "收件人公司的长度不能超过35个字符");
                }

                if (shippingInfo.ShippingAddress.Length > 35)
                {
                    errorDic.Add("ShippingInfo_ShippingAddress", "收货地址1的长度不能超过35个字符");
                }

                if (!string.IsNullOrWhiteSpace(shippingInfo.ShippingAddress1) && shippingInfo.ShippingAddress1.Length > 35)
                {
                    errorDic.Add("ShippingInfo_ShippingAddress1", "收货地址2的长度不能超过35个字符");
                }

                if (!string.IsNullOrWhiteSpace(shippingInfo.ShippingAddress2) && shippingInfo.ShippingAddress2.Length > 35)
                {
                    errorDic.Add("ShippingInfo_ShippingAddress2", "收货地址3的长度不能超过35个字符");
                }
            }

            responseResult.Result = !errorDic.Any();
            responseResult.Message = "[" + string.Join(",", errorDic.Select(p => "{" + string.Format("key:'{0}',value:'{1}'", p.Key, p.Value) + "}")) + "]";
            return Json(responseResult);
        }

        public ActionResult CheckApplicationInfo(int shippingMethodId, ApplicationInfo applicationInfo)
        {
            ResponseResult responseResult = new ResponseResult();
            var shippingMethod = _freightService.GetShippingMethod(shippingMethodId);
            var errorDic = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(applicationInfo.ApplicationName))
            {
                errorDic.Add("ApplicationName", "申报名称必填");
            }

            if (!applicationInfo.Qty.HasValue || applicationInfo.Qty <= 0)
            {
                errorDic.Add("Qty", "申报数量不正确");
            }

            if (!applicationInfo.UnitPrice.HasValue || applicationInfo.UnitPrice <= 0)
            {
                errorDic.Add("UnitPrice", "申报单价不正确");
            }

            if (!applicationInfo.UnitWeight.HasValue || applicationInfo.UnitWeight <= 0)
            {
                errorDic.Add("UnitWeight", "申报净重量不正确");
            }

            //是否属欧洲专线
            if (sysConfig.DDPShippingMethodCode.Split(',').ToList().Contains(shippingMethod.Code) || sysConfig.DDPRegisterShippingMethodCode.Split(',').ToList().Contains(shippingMethod.Code))
            {
                if (string.IsNullOrWhiteSpace(applicationInfo.ProductUrl))
                {
                    errorDic.Add("ProductUrl", "销售链接不能为空");
                }

                if (string.IsNullOrWhiteSpace(applicationInfo.HSCode))
                {
                    errorDic.Add("HSCode", "海关编码不能为空");
                }

                if (string.IsNullOrWhiteSpace(applicationInfo.Remark))
                {
                    errorDic.Add("Remark", "备注不能为空");
                }
            }

            //EUB 验证
            if (shippingMethod.Code.Trim() == "EUB_CS" || shippingMethod.Code.Trim() == "EUB-SZ" ||
                shippingMethod.Code.Trim() == "EUB-FZ")
            {
                if (string.IsNullOrWhiteSpace(applicationInfo.ApplicationName) ||
                    applicationInfo.ApplicationName.Length > 128)
                {
                    errorDic.Add("ApplicationName", "行申报名称不能为空或超过128个字符");
                }

                if (string.IsNullOrWhiteSpace(applicationInfo.PickingName) || applicationInfo.PickingName.Length > 60)
                {
                    errorDic.Add("PickingName", "行申报中文名称不能为空或超过字符长度");
                }
                else
                {
                    if (!Regex.IsMatch(applicationInfo.PickingName, @"[\u4e00-\u9fa5]{1}[\u4e00-\u9fa5]+"))
                    {
                        errorDic.Add("PickingName", "列申报中文名称必须包含两个中文字符");
                    }
                }
            }

            //福州邮政申报信息判断
            if (shippingMethod.Code.Trim() == "CNPOST-FZ" || shippingMethod.Code.Trim() == "CNPOSTP_FZ" ||
                shippingMethod.Code.Trim() == "CNPOST-FYB")
            {
                if (string.IsNullOrWhiteSpace(applicationInfo.ApplicationName) ||
                    applicationInfo.ApplicationName.Length > 60)
                {
                    errorDic.Add("ApplicationName", "行申报名称不能为空或超过字符长度");
                }

                if (string.IsNullOrWhiteSpace(applicationInfo.PickingName) || applicationInfo.PickingName.Length > 60)
                {
                    errorDic.Add("PickingName", "行申报中文名称不能为空或超过字符长度");
                }
            }

            responseResult.Result = !errorDic.Any();
            responseResult.Message = "[" + string.Join(",", errorDic.Select(p => "{" + string.Format("key:'{0}',value:'{1}'", p.Key, p.Value) + "}")) + "]";
            return Json(responseResult);
        }

        #endregion

        #region 入仓重量异常单 yungchu

        public ActionResult InStorageWeightAbnormal(InStorageWeightAbnormalFilterModel filterModel)
        {
            if (!filterModel.IsFirstIn)
            {
                if (filterModel.StartTime == null)
                {
                    filterModel.StartTime = DateTime.Parse(DateTime.Now.AddDays(1).AddMonths(-1).ToString("yyyy-MM-dd"));
                }
                if (filterModel.EndTime == null)
                {
                    filterModel.EndTime = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
                }
                filterModel.IsFirstIn = true;
            }
   
            return View(InStorageWeightAbnormalDataBind(filterModel));
        }


        [System.Web.Mvc.HttpPost]
        [FormValueRequired("btnSearch")]
        public ActionResult InStorageWeightAbnormal(InStorageWeightAbnormalViewModel model)
        {
            return View(InStorageWeightAbnormalDataBind(model.FilterModel));
        }



        //导出
        [System.Web.Mvc.ActionName("InStorageWeightAbnormal")]
        [FormValueRequired("btnExport")]
        public ActionResult ExportInStorageWeightAbnormal(InStorageWeightAbnormalViewModel model)
        {
            string fileName = "重量异常运单表格";

            List<string> titleList = new List<string>
			{
				"CustomerName-客户名称",
				"CustomerOrderNumber-客户订单号",
				"WayBillNumber-运单号",
				"InShippingMethodName-入仓运输方式",
				"TrackingNumber-跟踪号",
				"CountryCode-国家简码",
				"ChineseName-国家中文名",
				"CreatedOn-创建时间",
				"PackageNumber-件数",
				"Length-长",
				"Width-宽",
				"Height-高",
				"ForecastWeight-预报重量",
				"Weight-称重重量",
				"Deviation-偏差值"

			};

            var getModel = _orderService.GetExportInStorageWeightAbnormalExt(new InStorageWeightAbnormalParam
            {
                CustomerCode = model.FilterModel.CustomerCode,
                ShippingMethodId = model.FilterModel.ShippingMethodId,
                SearchWhere = model.FilterModel.SearchWhere,
                SearchContext = model.FilterModel.SearchContext,
                StartTime = model.FilterModel.StartTime,
                EndTime = model.FilterModel.EndTime
            });

            ExportExcelByWeb.ListExcel(fileName, getModel, titleList);

            return View(InStorageWeightAbnormalDataBind(model.FilterModel));

        }


        //数据列表绑定
        public InStorageWeightAbnormalViewModel InStorageWeightAbnormalDataBind(InStorageWeightAbnormalFilterModel filterModel)
        {

            var model = new InStorageWeightAbnormalViewModel
            {
                FilterModel = filterModel,
                PagedList = _orderService.GetInStorageWeightAbnormalPagedList(new InStorageWeightAbnormalParam
                {
                    IsWeightGtWeight=filterModel.IsWeightGtWeight,//重量对比情况
                    CustomerCode = filterModel.CustomerCode,
                    ShippingMethodId = filterModel.ShippingMethodId,
                    SearchWhere = filterModel.SearchWhere,
                    SearchContext = filterModel.SearchContext,
                    StartTime = filterModel.StartTime,
                    EndTime = filterModel.EndTime,
                    Page = filterModel.Page,
                    PageSize = filterModel.PageSize
                }).ToModelAsPageCollection<InStorageWeightAbnormalExt, InStorageWeightAbnormal>()
            };
            //赋值
            ReturnModel(model, filterModel);

            return model;
        }


        //返回赋值数据
        public void ReturnModel(InStorageWeightAbnormalViewModel model, InStorageWeightAbnormalFilterModel filterModel)
        {
            string inStorageNumberWhere = ((int)WayBill.SearchFilterEnum.InStorageNumber).ToString();
            string outStorageNumberWhere = ((int)WayBill.SearchFilterEnum.OutStorageNumber).ToString();

            WayBill.GetSearchFilterList().ForEach(p =>
            {
                if (p.ValueField != inStorageNumberWhere && p.ValueField != outStorageNumberWhere)
                {
                    model.SearchWheres.Add(new SelectListItem()
                    {
                        Text = p.TextField,
                        Value = p.ValueField,
                        Selected = filterModel.SearchWhere.HasValue && p.ValueField == filterModel.SearchWhere.Value.ToString()
                    });
                }
            });



            var selectListItem = new List<SelectListItem>()
            {
                 new SelectListItem() {Value = "", Text = "全部"},
                 new SelectListItem() {Value = "true", Text = "比实际重量高"},
                 new SelectListItem() {Value = "false", Text = "比实际重量低"}
            };
            selectListItem.ForEach(
                a =>
                    model.WeightListItem.Add(new SelectListItem()
                    {
                        Value = a.Value,
                        Text = a.Text,
                        Selected = a.Value == filterModel.IsWeightGtWeight
                    }));
         



            model.PagedList.InnerList.ForEach(a =>
            {
                a.CustomerName = _customerService.GetCustomer(a.CustomerCode).Name;
            });
            model.PagedList.InnerList.ForEach(p =>
            {
                p.AbnormalTypeName = WayBill.GetAbnormalTypeDescription(p.OperateType);
            });

            //权限
            model.DisplayCancelHold = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.BatchCancelAbnormalWayBill);
            model.DisplayBatchDelete = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.BatchDelete);
            model.IsFastInStorageBut = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.FastInStorageCode);

        }


        //入仓扫描时--拦截重量异常单
        public void AddInStorageWeightAbnormal(InStorageWeightAbnormalParm param)
        {
            var result = new ResponseResult();
            WayBillInfoExtSilm getWayBillInfo = param.WayBillInfoExtSilm;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.MaxValue))
            {
                //增加运单异常日志
                string abnormalDescription = string.Format("称重重量与预报重量相差{0}g大于配置的{1}g不能入仓！", param.DeviationValue,
                                                           param.ConfigurationValue);


                _orderService.AddAbnormalWayBill(getWayBillInfo.WayBillNumber,
                                                 WayBill.AbnormalTypeEnum.InStorageWeightAbnormal,
                                                 abnormalDescription);

                //增加入仓对比重量异常单
                if (_orderService.IsExistInStorageWeightAbnormal(getWayBillInfo.WayBillNumber))
                {
                    _orderService.UpdateInStorageWeightAbnormal(getWayBillInfo.WayBillNumber, param.Weight);
                }
                else
                {
                    _orderService.AddInStorageWeightAbnormal(new WeightAbnormalLog
                        {
                            CustomerCode = getWayBillInfo.CustomerCode,
                            WayBillNumber = getWayBillInfo.WayBillNumber,
                            CustomerOrderID = getWayBillInfo.CustomerOrderID,
                            TrackingNumber = getWayBillInfo.TrackingNumber,
                            Length = param.Length ?? getWayBillInfo.Length,
                            Width = param.Width ?? getWayBillInfo.Width,
                            Height = param.Height ?? getWayBillInfo.Height,
                            Weight = param.Weight, //称重重量
                            CreatedOn = System.DateTime.Now,
                            CreatedBy = _workContext.User.UserUame
                        });

                }
                result.Result = true;
                transaction.Complete();
            }

        }

        //批量取消拦截，删除重量异常单
        [ButtonPermissionValidator(PermissionRecords.BatchCancelAbnormalWayBill)]
        public JsonResult BatchCancelHoldWayBill(string wayBillNumbers)
        {

            var model = new ResponseResult();
            try
            {
                if (!string.IsNullOrWhiteSpace(wayBillNumbers))
                {
                    List<string> listStr = new List<string>();
                    var arr = wayBillNumbers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var wayBillNumberList = arr.ToList();

                    wayBillNumberList.ForEach(a =>
                    {
                        if (!listStr.Contains(a))
                        {
                            listStr.Add(a);
                        }
                    });

                    //解除hold
                    _orderService.BatchCancelHoldWayBillInfo(listStr);

                    //删除重量异常单
                    _orderService.DeleteInStorageWeightAbnormal(listStr);

                    model.Result = true;
                }
                else
                {
                    model.Result = true;
                    model.Message = "请选择需要的取消异常运单";
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

        // 批量删除异常运单，删除重量异常单
        [ButtonPermissionValidator(PermissionRecords.BatchDeleteAbnormalWayBill)]
        public JsonResult BatchDeleteAbnormalWayBillInfo(string wayBillNumbers)
        {

            var model = new ResponseResult();
            try
            {
                List<string> numberList = wayBillNumbers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                BateDelete(wayBillNumbers);

                _orderService.DeleteInStorageWeightAbnormal(numberList);
                model.Result = true;

            }
            catch (Exception ex)
            {
                model.Result = false;
                Log.Error(ex.Message);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 无预报异常 zxq

        public ActionResult NoForecastAbnormalList(NoForecastAbnormalFilterModel filterModel)
        {
            if (filterModel.IsFirstIn)
            {
                filterModel.EndTime = DateTime.Now;
                filterModel.StartTime = DateTime.Now.AddDays(-7);
            }
            filterModel.IsFirstIn = false;
            return View(NoForecastAbnormalBindList(filterModel));
        }

        private NoForecastAbnormalViewModel NoForecastAbnormalBindList(NoForecastAbnormalFilterModel filterModel)
        {
            var model = new NoForecastAbnormalViewModel();

            if (!filterModel.Status.HasValue)
            {
                filterModel.Status = (int)WayBill.NoForecastAbnormalEnum.NoForecast;
            }

            WayBill.GetNoForecastAbnormalList().ForEach(p => model.StatusList.Add(new SelectListItem()
            {
                Text = p.TextField,
                Value = p.ValueField,
                Selected = p.ValueField == (filterModel.Status).ToString()
            }));

            model.StatusList.Add(new SelectListItem()
            {
                Text = "全部",
                Value = "0",
            });

            NoForecastAbnormalParam param = new NoForecastAbnormalParam();
            filterModel.CopyTo(param);

            model.PagedList = _orderService.GetNoForecastAbnormalExtPagedList(param);

            var shippingMethodList = _freightService.GetShippingMethodListByCustomerTypeId(null, true);

            model.PagedList.InnerList.ForEach(p =>
            {
                p.CustomerName = GetGetCustomerListFromCache().Find(c => c.CustomerCode == p.CustomerCode).Name;
                p.ShippingMethodName = shippingMethodList.Find(l => l.ShippingMethodId == p.ShippingMethodId).ShippingMethodName;
            });

            model.FilterModel = filterModel;
            //add by yungchu
            if (_workContext.User.IsSuperAdmin || _workContext.User.Permissions.Any(p => p.PermissionCode == PermissionRecords.NoForecastAbnormalDelete.ToString()))
            {
                model.DisplayDelete = true;
            }
            return model;
        }

        [ButtonPermissionValidator(PermissionRecords.NoForecastAbnormalDelete)]
        public ActionResult NoForecastAbnormalDelete(string noForecastAbnormalId)
        {
            try
            {
                int[] noForecastAbnormalIds = noForecastAbnormalId.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => Convert.ToInt32(p)).ToArray();
                _orderService.DeleteNoForecastAbnormal(noForecastAbnormalIds);

                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return Json(new { Result = false, Message = ex.Message });
            }
        }

        public ActionResult NoForecastAbnormalReturn(string noForecastAbnormalId)
        {
            try
            {
                int[] noForecastAbnormalIds = noForecastAbnormalId.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => Convert.ToInt32(p)).ToArray();
                _orderService.ReturnNoForecastAbnormal(noForecastAbnormalIds);

                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return Json(new { Result = false, Message = ex.Message });
            }
        }


        //导出
        [System.Web.Mvc.ActionName("NoForecastAbnormalList")]
        [FormValueRequired("btnExport")]
        public ActionResult NoForecastAbnormalExport(NoForecastAbnormalFilterModel filterModel)
        {
            List<string> titleList = new List<string>
			{
				"CustomerName-客户名称",
				"Number-单号",
				"ShippingMethodName-运输方式",
				"Weight-重量kg",
				"StatusStr-状态",
				"Description-异常说明",
				"CreatedBy-创建人",
                "CreatedOn-创建时间",
			};

            var newFilterModel = new NoForecastAbnormalFilterModel();
            filterModel.CopyTo(newFilterModel);
            newFilterModel.Page = 1;
            newFilterModel.PageSize = int.MaxValue;

            var model = NoForecastAbnormalBindList(newFilterModel);

            ExportExcelByWeb.WriteToDownLoad(NoForecastAbnormalBindList(filterModel).PagedList.InnerList, titleList, null);

            return View(model);

        }

        #endregion


        #region 运单汇总报表
        public ActionResult WayBillSummary(WaybillSummaryParam filter)
        {
            filter.StartTime = DateTime.Now.AddMonths(-3);
            filter.EndTime = DateTime.Now;
            return View(WayBillSummaryDataBind(filter, true));
        }
        [System.Web.Mvc.HttpPost]
        [FormValueRequired("btnSearch")]
        [System.Web.Mvc.ActionName("WayBillSummary")]
        public ActionResult SearchWayBillSummary(WayBillSummaryViewModel filter)
        {
            return View(WayBillSummaryDataBind(filter.FilterModel, false));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="isfirst">是否第一次加载</param>
        /// <returns></returns>
        private WayBillSummaryViewModel WayBillSummaryDataBind(WaybillSummaryParam filter, bool isfirst)
        {
            var model = new WayBillSummaryViewModel()
            {
                FilterModel = filter
            };
            model.SelectShippingMethods.Add(new SelectListItem() { Selected = model.FilterModel.SelectShippingMethod == 1, Text = "入仓渠道", Value = "1" });
            model.SelectShippingMethods.Add(new SelectListItem() { Selected = model.FilterModel.SelectShippingMethod == 2, Text = "出仓渠道", Value = "2" });
            WayBill.GetDateFilterList().ForEach(p =>
            {
                if (isfirst)
                {
                    model.SelectTimeNames.Add(new SelectListItem()
                    {
                        Text = p.TextField,
                        Value = p.ValueField
                    });
                }
                else
                {
                    model.SelectTimeNames.Add(new SelectListItem()
                    {
                        Selected = p.ValueField == model.FilterModel.SelectTimeName.ToString(),
                        Text = p.TextField,
                        Value = p.ValueField
                    });
                }
            });
            WayBill.GetStatusList().ForEach(p =>
                {
                    if (p.ValueField != WayBill.StatusToValue(WayBill.StatusEnum.Delete).ToString())
                    {
                        model.SelectStatus.Add(new SelectListItem()
                            {
                                Text = p.TextField,
                                Value = p.ValueField
                            });
                    }
                });
            if (!isfirst)
            {
                var list = _orderService.GetWaybillSummaryList(model.FilterModel);
                var venderdic = new Dictionary<string, string>();
                _freightService.GetVenderList(true).ForEach(p =>
                    {
                        if (!venderdic.ContainsKey(p.VenderCode))
                        {
                            venderdic.Add(p.VenderCode, p.VenderName);
                        }
                    });
                var shippingdic = new Dictionary<int, string>();
                _freightService.GetShippingMethods("", true).ForEach(p =>
                    {
                        if (!shippingdic.ContainsKey(p.ShippingMethodId))
                        {
                            shippingdic.Add(p.ShippingMethodId, p.FullName);
                        }
                    });
                var userdic = new Dictionary<string, string>();
                _customerService.GetCustomerList(model.FilterModel.CustomerCode, new int?()).ForEach(p =>
                    {
                        if (!userdic.ContainsKey(p.CustomerCode))
                        {
                            userdic.Add(p.CustomerCode, p.Name);
                        }
                    });
                foreach (var waybillSummary in list)
                {
                    if (!waybillSummary.CustomerCode.IsNullOrWhiteSpace() &&
                        userdic.ContainsKey(waybillSummary.CustomerCode))
                        waybillSummary.CustomerName = userdic[waybillSummary.CustomerCode];
                    if (!waybillSummary.VenderCode.IsNullOrWhiteSpace() &&
                        venderdic.ContainsKey(waybillSummary.VenderCode))
                        waybillSummary.VenderName = venderdic[waybillSummary.VenderCode];
                    if (waybillSummary.InShippingMethodID.HasValue &&
                        shippingdic.ContainsKey(waybillSummary.InShippingMethodID.Value))
                        waybillSummary.InShippingMethodName = shippingdic[waybillSummary.InShippingMethodID.Value];
                    if (waybillSummary.OutShippingMethodID.HasValue &&
                        shippingdic.ContainsKey(waybillSummary.OutShippingMethodID.Value))
                        waybillSummary.OutShippingMethodName = shippingdic[waybillSummary.OutShippingMethodID.Value];
                }
                model.List = list;
            }
            return model;
        }
        #endregion

        public ActionResult SelectCountry()
        {
            return View();
        }

        public JsonResult GetSelectCountry(string keyword, int? shippingMethodId)
        {
            var listShow = _countryService.GetCountryList(keyword).ToModelAsCollection<Country, CountryModel>();

            if (shippingMethodId.HasValue)
            {
                var listShippingMethodCountryModel = GetShippingMethodCountriesFromCache(shippingMethodId.Value);

                var list = listShow.Select(p => new CountryExt
                    {
                        CountryCode = p.CountryCode,
                        ChineseName = p.ChineseName,
                        Name = p.Name,
                        CanSelect = listShippingMethodCountryModel.Exists(sc => sc.CountryCode == p.CountryCode)
                    });

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var list = listShow.Select(p => new CountryExt
                {
                    CountryCode = p.CountryCode,
                    ChineseName = p.ChineseName,
                    Name = p.Name,
                    CanSelect = true,
                });

                return Json(list, JsonRequestBehavior.AllowGet);
            }

        }

        private InStorageListViewModel InStorageListDataBind(InStorageFilterModel filter)
        {
            var model = new InStorageListViewModel
                {
                    FilterModel = filter,
                    PagedList = _inStorageService.GetInStorageInfoExtPagedList(new InStorageListSearchParam()
                        {
                            CustomerCode = filter.CustomerCode,
                            InStartDate = filter.InStartDate,
                            InStorageID = filter.InStorageID,
                            InEndDate = filter.InEndDate,
                            ShippingMethodId = filter.ShippingMethodId,
                            Page = filter.Page,
                            PageSize = filter.PageSize
                        })
                };
            return model;
        }

        private OutStorageListViewModel OutStorageListDataBind(OutStorageFilterModel filter)
        {
            //var model = new OutStorageListViewModel
            //    {
            //        FilterModel = filter,
            //        PagedList = _outStorageService.GetOutStoragePagedList(new OutStorageListSearchParam()
            //            {
            //                VenderCode = filter.VenderCode,
            //                OutStorageID = filter.OutStorageID,
            //                OutStartDate = filter.OutStartDate,
            //                OutEndDate = filter.OutEndDate,
            //                Page = filter.Page,
            //                PageSize = filter.PageSize
            //            }).ToModelAsPageCollection<OutStorageInfo, OutStorageInfoModel>()
            //    };

            OutStorageListViewModel model = new OutStorageListViewModel();
            model.FilterModel = filter;
            var list = _outStorageService.GetOutStoragePagedList(new OutStorageListSearchParam()
                {
                    VenderCode = filter.VenderCode,
                    OutStorageID = filter.OutStorageID,
                    OutStartDate = filter.OutStartDate,
                    OutEndDate = filter.OutEndDate,
                    PostBagNumber=filter.PostBagNumber,
                    Page = filter.Page,
                    PageSize = filter.PageSize
                });
            model.PagedList = new PagedList<OutStorageInfoModel>();
            model.PagedList.TotalCount = list.TotalCount;
            model.PagedList.TotalPages = list.TotalPages;
            model.PagedList.PageIndex = list.PageIndex;
            model.PagedList.PageSize = list.PageSize;

            OutStorageInfoModel m;
            foreach (var p in list)
            {
                m = new OutStorageInfoModel();
                m.OutStorageID = p.OutStorageID;
                m.CreatedBy = p.CreatedBy;
                m.CreatedOn = p.CreatedOn;
                m.DeliveryStaff = p.DeliveryStaff;
                m.Freight = p.Freight;
                m.FuelCharge = p.FuelCharge;
                m.LastUpdatedBy = p.LastUpdatedBy;
                m.LastUpdatedOn = p.LastUpdatedOn;
                m.VenderCode = p.VenderCode;
                m.TotalWeight = p.TotalWeight;
                m.TotalFee = p.TotalFee;
                m.PostBagNumber = p.PostBagNumber;
                m.Remark = p.Remark;
                model.PagedList.InnerList.Add(m);
            }               
                
           //list.ToModelAsPageCollection<OutStorageInfo, OutStorageInfoModel>();


            if (model.PagedList != null)
            {
                foreach (var row in model.PagedList)
                {
                    if (_outStorageService.IsUpdateOutStorage(row.OutStorageID, row.TotalQty ?? 0))
                    {
                        row.isUpdate = true;
                    }
                    else
                    {
                        row.isUpdate = false;
                    }
                }
            }
            return model;
        }

        


        private WayBillListSilmViewModel ListDataBindSilm(WayBillListFilterModel filter)
        {


            var model = new WayBillListSilmViewModel
                {

                    FilterModel = filter,
                    PagedList = _orderService.GetWayBillInfoPagedListSilm(new OrderListParam()
                        {
                            CountryCode = filter.CountryCode,
                            CustomerCode = filter.CustomerCode,
                            DateWhere = filter.DateWhere,
                            EndTime = filter.EndTime,
                            SearchWhere = filter.SearchWhere,
                            SearchContext = filter.SearchContext,
                            ShippingMethodId = filter.ShippingMethodId,
                            //Status = filter.Status,
                            GetStatus = filter.GetStatus,
                            StartTime = filter.StartTime,
                            IsHold = filter.IsHold,
                            Page = filter.Page,
                            PageSize = filter.PageSize,
                            Operator = filter.Operator,
                            OperatorType = filter.OperatorType,
                            ShowTestWaybill = filter.ShowTestWaybill,
                        })
                };

            DecorateWayBillListViewModel(model, filter);

            return model;
        }

        private WayBillListViewModel ListDataBind(WayBillListFilterModel filter)
        {
            var model = new WayBillListViewModel
                {

                    FilterModel = filter,
                    PagedList = _orderService.GetWayBillInfoPagedList(new OrderListParam()
                        {
                            CountryCode = filter.CountryCode,
                            CustomerCode = filter.CustomerCode,
                            DateWhere = filter.DateWhere,
                            EndTime = filter.EndTime,
                            SearchWhere = filter.SearchWhere,
                            SearchContext = filter.SearchContext,
                            ShippingMethodId = filter.ShippingMethodId,
                            Status = filter.Status,
                            StartTime = filter.StartTime,
                            IsHold = filter.IsHold,
                            Page = filter.Page,
                            PageSize = filter.PageSize
                        }).ToModelAsPageCollection<WayBillInfo, WayBillInfoModel>()
                };

            DecorateWayBillListViewModel(model, filter);

            return model;

            //model.ShippingMethodLists = GetShippingMethodSelectList();
            //var customerList = _customerService.GetCustomerList("");
            //customerList.ForEach(p => model.CustomerList.Add(new SelectListItem
            //    {
            //        Text = p.Name,
            //        Value = p.CustomerCode
            //    }));
            //model.StatusList.Add(new SelectListItem() { Text = "请选择", Value = "", Selected = !filter.Status.HasValue });
            //WayBill.GetStatusList().ForEach(p =>
            //    {
            //        if (p.ValueField != "6")
            //        {
            //            model.StatusList.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = filter.Status.HasValue && p.ValueField == filter.Status.Value.ToString() });
            //        }
            //    });
            //WayBill.GetSearchFilterList().ForEach(p =>
            //    {
            //        model.SearchWheres.Add(new SelectListItem()
            //            {
            //                Text = p.TextField,
            //                Value = p.ValueField,
            //                Selected = filter.SearchWhere.HasValue && p.ValueField == filter.SearchWhere.Value.ToString()
            //            });

            //    });
            //WayBill.GetDateFilterList().ForEach(p =>
            //    {
            //        model.DateWheres.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = p.ValueField == filter.DateWhere.ToString() });
            //    });
            //model.IsFastOutStorageBut = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.FastOutStorageCode);
            //model.IsFastInStorageBut = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.FastInStorageCode);
            //model.DisPlayModifyShippingMethod = _userService.Authorize(_workContext.User.UserUame,
            //                                                           PermissionRecords.BatchModifyShippingMethod);
            //model.DisplayBatchDelete = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.BatchDelete);
            //model.DisplayBatchHold = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.BatchHoldOn);
            //return model;
        }

        private void DecorateWayBillListViewModel(WayBillListViewModelBase model, WayBillListFilterModel filter)
        {
            model.ShippingMethodLists = GetShippingMethodSelectList();
            var customerList = _customerService.GetCustomerList("");
            customerList.ForEach(p => model.CustomerList.Add(new SelectListItem
            {
                Text = p.Name,
                Value = p.CustomerCode
            }));


            //获取状态列表
            WayBill.GetStatusList().ForEach(p => model.GetStatusList.Add(new DropListStatus { Id = Convert.ToInt32(p.ValueField), Name = p.TextField }));

            //运单状态赋值/Add By zhengsong
            model.StatusList.Add(new SelectListItem() { Text = "请选择", Value = "", Selected = !filter.Status.HasValue });
            WayBill.GetStatusList().ForEach(p =>
                {
                    if (p.ValueField != "6")
                    {
                        model.StatusList.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = filter.Status.HasValue && p.ValueField == filter.Status.Value.ToString() });
                    }
                });


            WayBill.GetSearchFilterList().ForEach(p =>
            {
                model.SearchWheres.Add(new SelectListItem()
                {
                    Text = p.TextField,
                    Value = p.ValueField,
                    Selected = filter.SearchWhere.HasValue && p.ValueField == filter.SearchWhere.Value.ToString()
                });

            });
            WayBill.GetDateFilterList().ForEach(p =>
            {
                model.DateWheres.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = p.ValueField == filter.DateWhere.ToString() });
            });

            model.OperatorTypeList.Add(new SelectListItem() { Text = "入仓操作人", Value = "0", Selected = "0" == filter.OperatorType.ToString() });
            model.OperatorTypeList.Add(new SelectListItem() { Text = "出仓操作人", Value = "1", Selected = "1" == filter.OperatorType.ToString() });

            model.IsFastOutStorageBut = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.FastOutStorageCode);
            model.IsFastInStorageBut = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.FastInStorageCode);
            model.DisPlayModifyShippingMethod = _userService.Authorize(_workContext.User.UserUame,
                                                                       PermissionRecords.BatchModifyShippingMethod);
            model.DisplayBatchDelete = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.BatchDelete);
            model.DisplayBatchHold = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.BatchHoldOn);
            //return model;
        }

        private WayBillListViewModel WayBillExcelExportBindList(WayBillListFilterModel filter)
        {
            var model = new WayBillListViewModel
            {

                FilterModel = filter,
                ExportPagedList = _orderService.GetExportWayBillInfoPagedList(new OrderListParam()
                {
                    CountryCode = filter.CountryCode,
                    CustomerCode = filter.CustomerCode,
                    DateWhere = filter.DateWhere,
                    EndTime = filter.EndTime,
                    SearchWhere = filter.SearchWhere,
                    SearchContext = filter.SearchContext,
                    ShippingMethodId = filter.ShippingMethodId,
                    Status = filter.Status,
                    StartTime = filter.StartTime,
                    IsHold = filter.IsHold,
                    Page = filter.Page,
                    PageSize = filter.PageSize
                }).ToModelAsPageCollection<WayBillInfo, WayBillExcelExport>()
            };
            model.ShippingMethodLists = GetShippingMethodSelectList();
            var customerList = _customerService.GetCustomerList("");
            customerList.ForEach(p => model.CustomerList.Add(new SelectListItem
            {
                Text = p.Name,
                Value = p.CustomerCode
            }));
            model.StatusList.Add(new SelectListItem() { Text = "请选择", Value = "", Selected = !filter.Status.HasValue });
            WayBill.GetStatusList().ForEach(p =>
            {
                if (p.ValueField != "6")
                {
                    model.StatusList.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = filter.Status.HasValue && p.ValueField == filter.Status.Value.ToString() });
                }
            });
            WayBill.GetSearchFilterList().ForEach(p =>
            {
                model.SearchWheres.Add(new SelectListItem()
                {
                    Text = p.TextField,
                    Value = p.ValueField,
                    Selected = filter.SearchWhere.HasValue && p.ValueField == filter.SearchWhere.Value.ToString()
                });

            });
            WayBill.GetDateFilterList().ForEach(p =>
            {
                model.DateWheres.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = p.ValueField == filter.DateWhere.ToString() });
            });
            model.IsFastOutStorageBut = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.FastOutStorageCode);
            model.IsFastInStorageBut = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.FastInStorageCode);
            model.DisPlayModifyShippingMethod = _userService.Authorize(_workContext.User.UserUame,
                                                                       PermissionRecords.BatchModifyShippingMethod);
            model.DisplayBatchDelete = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.BatchDelete);
            model.DisplayBatchHold = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.BatchHoldOn);
            return model;
        }

        private WayBillListViewModel SelectWayBillList(WayBillListFilterModel filter)
        {
            var model = new WayBillListViewModel
            {

                FilterModel = filter,
                WayBillInfoModels = _orderService.GetWayBillInfo(new OrderListParam()
                {
                    CountryCode = filter.CountryCode,
                    CustomerCode = filter.CustomerCode,
                    DateWhere = filter.DateWhere,
                    EndTime = filter.EndTime,
                    SearchWhere = filter.SearchWhere,
                    SearchContext = filter.SearchContext,
                    ShippingMethodId = filter.ShippingMethodId,
                    IsHold = filter.IsHold,
                    Status = filter.Status,
                    GetStatus = filter.GetStatus,//下拉框多选
                    StartTime = filter.StartTime,
                }).ToModelAsCollection<WayBillInfo, WayBillInfoModel>()
            };
            model.StatusList.Add(new SelectListItem() { Text = "请选择", Value = "", Selected = !filter.Status.HasValue });
            WayBill.GetStatusList().ForEach(p =>
            {
                model.StatusList.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = filter.Status.HasValue && p.ValueField == filter.Status.Value.ToString() });
            });
            WayBill.GetSearchFilterList().ForEach(p =>
            {
                model.SearchWheres.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = filter.SearchWhere.HasValue && p.ValueField == filter.SearchWhere.Value.ToString() });
            });
            WayBill.GetDateFilterList().ForEach(p =>
            {
                model.DateWheres.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = p.ValueField == filter.DateWhere.ToString() });
            });
            model.IsFastOutStorageBut = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.FastOutStorageCode);
            model.DisPlayModifyShippingMethod = _userService.Authorize(_workContext.User.UserUame,
                                                                      PermissionRecords.BatchModifyShippingMethod);
            model.DisplayBatchDelete = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.BatchDelete);
            model.DisplayBatchHold = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.BatchHoldOn);
            return model;
        }

        private WayBillListViewModel ExportWayBillList(WayBillListFilterModel filter)
        {
            var model = new WayBillListViewModel
            {

                FilterModel = filter,
                WayBillInfoModels = _orderService.GetExportWayBillInfo(new OrderListParam()
                {
                    CountryCode = filter.CountryCode,
                    CustomerCode = filter.CustomerCode,
                    DateWhere = filter.DateWhere,
                    EndTime = filter.EndTime,
                    SearchWhere = filter.SearchWhere,
                    SearchContext = filter.SearchContext,
                    ShippingMethodId = filter.ShippingMethodId,
                    IsHold = filter.IsHold,
                    Status = filter.Status,
                    StartTime = filter.StartTime,
                }).ToModelAsCollection<WayBillInfo, WayBillInfoModel>()
            };
            model.StatusList.Add(new SelectListItem() { Text = "请选择", Value = "", Selected = !filter.Status.HasValue });
            WayBill.GetStatusList().ForEach(p =>
            {
                model.StatusList.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = filter.Status.HasValue && p.ValueField == filter.Status.Value.ToString() });
            });
            WayBill.GetSearchFilterList().ForEach(p =>
            {
                model.SearchWheres.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = filter.SearchWhere.HasValue && p.ValueField == filter.SearchWhere.Value.ToString() });
            });
            WayBill.GetDateFilterList().ForEach(p =>
            {
                model.DateWheres.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = p.ValueField == filter.DateWhere.ToString() });
            });
            model.IsFastOutStorageBut = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.FastOutStorageCode);
            model.DisPlayModifyShippingMethod = _userService.Authorize(_workContext.User.UserUame,
                                                                      PermissionRecords.BatchModifyShippingMethod);
            model.DisplayBatchDelete = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.BatchDelete);
            model.DisplayBatchHold = _userService.Authorize(_workContext.User.UserUame, PermissionRecords.BatchHoldOn);
            return model;
        }

        private AbnormalWayBillListViewModel AbnormalWayBillListDataBind(AbnormalWayBillListFilterModel filter)
        {
            var model = new AbnormalWayBillListViewModel
            {
                FilterModel = filter,
                PagedList = _orderService.GetAbnormalWayBillPagedList(new AbnormalWayBillParam()
                {
                    CountryCode = filter.CountryCode,
                    CustomerCode = filter.CustomerCode,
                    DateWhere = 1,
                    EndTime = filter.EndTime,
                    SearchWhere = filter.SearchWhere,
                    SearchContext = filter.SearchContext,
                    ShippingMethodId = filter.ShippingMethodId,
                    Status = filter.Status,
                    StartTime = filter.StartTime,
                    Page = filter.Page,
                    PageSize = filter.PageSize,
                    WaybillStatus = filter.WaybillStatus

                }),
            };
            model.PagedList.InnerList.ForEach(p =>
                {
                    p.AbnormalTypeName = WayBill.GetAbnormalTypeDescription(p.OperateType);
                });



            //运单状态
            model.WayBillStatusList.Add(new SelectListItem() { Text = "请选择", Value = "", Selected = !filter.WaybillStatus.HasValue });
            WayBill.GetStatusList().ForEach(p =>
            {
                model.WayBillStatusList.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = filter.WaybillStatus.HasValue && p.ValueField == filter.WaybillStatus.Value.ToString() });
            });

            model.StatusList.Add(new SelectListItem() { Text = "请选择", Value = "", Selected = !filter.Status.HasValue });
            WayBill.GetAbnormalStatusList().ForEach(p =>
            {
                model.StatusList.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = filter.Status.HasValue && p.ValueField == filter.Status.Value.ToString() });
            });
            WayBill.GetSearchFilterList().ForEach(p =>
            {
                model.SearchWheres.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = filter.SearchWhere.HasValue && p.ValueField == filter.SearchWhere.Value.ToString() });
            });
            //int createonwhere = WayBill.DateFilterToValue(WayBill.DateFilterEnum.CreatedOn);
            //model.StatusList.Add(new SelectListItem() { Text = WayBill.GetDateFilterDescription(createonwhere), Value = createonwhere.ToString() });
            //WayBill.GetDateFilterList().ForEach(p =>
            //{
            //    model.DateWheres.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = p.ValueField == filter.DateWhere.ToString() });
            //});
            model.DisplayBatchDelete = _userService.Authorize(_workContext.User.UserUame,
                                                              PermissionRecords.BatchDeleteAbnormalWayBill);
            model.DisplayCancelHold = _userService.Authorize(_workContext.User.UserUame,
                                                             PermissionRecords.BatchCancelAbnormalWayBill);
            return model;
        }

        private InFeeInfoListViewModel InFeeInfoListDataBind(InFeeListFilterModel filter)
        {
            var model = new InFeeInfoListViewModel { FilterModel = filter };
            decimal alltotalfee = 0;
            InFeeListParam inFeeListParam = null;
            if (!string.IsNullOrWhiteSpace(filter.SearchContext))
            {
                inFeeListParam = new InFeeListParam()
                    {
                        SearchContext = filter.SearchContext,
                        SearchWhere = filter.SearchWhere,
                        Page = filter.Page,
                        PageSize = filter.PageSize
                    };
            }
            else
            {
                inFeeListParam = new InFeeListParam()
                    {
                        CustomerCode = filter.CustomerCode,
                        CountryCode = filter.CountryCode,
                        EndTime = filter.EndTime,
                        ShippingMethodId = filter.ShippingMethodId,
                        StartTime = filter.StartTime,
                        Page = filter.Page,
                        PageSize = filter.PageSize
                    };
            }
            model.PagedList = _feeManageService.GetInFeeInfoPagedList(inFeeListParam, out alltotalfee).ToModelAsPageCollection<InFeeInfoExt, InFeeInfoModel>();
            model.AllTotalFee = alltotalfee;
            WayBill.GetSearchFilterList().ForEach(p =>
            {
                model.SearchWheres.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = filter.SearchWhere.HasValue && p.ValueField == filter.SearchWhere.Value.ToString() });
            });
            return model;
        }

        private InFeeInfoListViewModel InFeeInfoListExport(InFeeListFilterModel filter)
        {
            var model = new InFeeInfoListViewModel { FilterModel = filter };
            decimal alltotalfee = 0;
            model.List = _feeManageService.GetInFeeInfoList(new InFeeListParam()
            {
                CustomerCode = filter.CustomerCode,
                CountryCode = filter.CountryCode,
                EndTime = filter.EndTime,
                SearchContext = filter.SearchContext,
                ShippingMethodId = filter.ShippingMethodId,
                SearchWhere = filter.SearchWhere,
                StartTime = filter.StartTime,
            }, out alltotalfee).ToModelAsCollection<InFeeInfoExt, InFeeInfoModel>().ToList();
            model.AllTotalFee = alltotalfee;
            WayBill.GetSearchFilterList().ForEach(p =>
            {
                model.SearchWheres.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = filter.SearchWhere.HasValue && p.ValueField == filter.SearchWhere.Value.ToString() });
            });
            return model;
        }
        private OutFeeInfoListViewModel OutFeeInfoListDataBind(OutFeeListFilterModel filter)
        {
            var model = new OutFeeInfoListViewModel { FilterModel = filter };
            decimal alltotalfee = 0;
            OutFeeListParam outFeelistParam = null;
            if (!string.IsNullOrWhiteSpace(filter.SearchContext))
            {
                outFeelistParam = new OutFeeListParam()
                    {

                        SearchContext = filter.SearchContext,
                        SearchWhere = filter.SearchWhere,
                        Page = filter.Page,
                        PageSize = filter.PageSize
                    };
            }
            else
            {
                outFeelistParam = new OutFeeListParam()
                    {
                        VenderCode = filter.VenderCode,
                        CountryCode = filter.CountryCode,
                        EndTime = filter.EndTime,
                        ShippingMethodId = filter.ShippingMethodId,
                        StartTime = filter.StartTime,
                        Page = filter.Page,
                        PageSize = filter.PageSize
                    };
            }

            model.PagedList = _feeManageService.GetOutFeeInfoPagedList(outFeelistParam, out alltotalfee).ToModelAsPageCollection<OutFeeInfoExt, OutFeeInfoModel>();
            model.AllTotalFee = alltotalfee;
            WayBill.GetSearchFilterList().ForEach(p =>
            {
                model.SearchWheres.Add(new SelectListItem() { Text = p.TextField.Replace("入仓号", "出仓号"), Value = p.ValueField, Selected = filter.SearchWhere.HasValue && p.ValueField == filter.SearchWhere.Value.ToString() });
            });
            return model;
        }

        private OutFeeInfoListViewModel OutFeeInfoListExport(OutFeeListFilterModel filter)
        {
            var model = new OutFeeInfoListViewModel { FilterModel = filter };
            decimal alltotalfee = 0;
            model.List = _feeManageService.GetOutFeeInfoList(new OutFeeListParam()
            {
                VenderCode = filter.VenderCode,
                CountryCode = filter.CountryCode,
                EndTime = filter.EndTime,
                SearchContext = filter.SearchContext,
                ShippingMethodId = filter.ShippingMethodId,
                SearchWhere = filter.SearchWhere,
                StartTime = filter.StartTime
            }, out alltotalfee).ToModelAsCollection<OutFeeInfoExt, OutFeeInfoModel>().ToList();
            model.AllTotalFee = alltotalfee;
            WayBill.GetSearchFilterList().ForEach(p =>
            {
                model.SearchWheres.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = filter.SearchWhere.HasValue && p.ValueField == filter.SearchWhere.Value.ToString() });
            });
            return model;
        }

        private OutStorageConfigureViewModel BindOutStorageConfigure(OutStorageConfigureViewModel model)
        {
            model.InShippingMethods = GetShippingMethodSelectList();
            if (model.OutStorageConfigureModel.InShippingMethodId == 0)
            {
                model.OutStorageConfigureModel.InShippingMethodId = int.Parse(model.InShippingMethods[0].Value);
            }
            model.OutStorageConfigureModels = _outStorageService.GetDeliveryChannelConfigurations(model.OutStorageConfigureModel.InShippingMethodId).ToModelAsCollection<DeliveryChannelConfiguration, OutStorageConfigureModel>();
            return model;
        }

        private List<SelectListItem> GetTemplateTypeList()
        {
            List<SelectListItem> list = new List<SelectListItem> { new SelectListItem { Text = "全部", Value = "" } };
            WayBillTemplateInfo.GetTemplateTypeList().Each(p => list.Add(new SelectListItem()
            {
                Text = p.TextField,
                Value = p.ValueField
            }));
            return list;
        }
        private List<SelectListItem> GetStatusList()
        {
            List<SelectListItem> list = new List<SelectListItem> { new SelectListItem { Text = "全部", Value = "" } };
            WayBillTemplateInfo.GetStatusList().Each(p => list.Add(new SelectListItem()
                {
                    Text = p.TextField,
                    Value = p.ValueField
                }));
            return list;
        }
    }

    public class PrintController : Controller
    {
        private IInStorageService _inStorageService;
        private ICustomerService _customerService;
        private IOrderService _orderService;
        private ICountryService _countryService;
        private IExpressService _expressService;
        private ICustomerOrderService _customerOrderService;
        private IFreightService _freightService;
        private IWayBillTemplateService _wayBillTemplateService;
        private readonly string _tempPathForExcel;
        private IWorkContext _workContext;
        private IOperateLogServices _operateLogServices;

        public PrintController(IInStorageService inStorageService,
                                ICustomerService customerService,
                                IOrderService orderService,
                                ICountryService countryService,
                                IExpressService expressService,
            ICustomerOrderService customerOrderService,
            IFreightService freightService,
            IWayBillTemplateService wayBillTemplateService,
            IWorkContext workContext,
            IOperateLogServices operateLogServices)
        {
            _inStorageService = inStorageService;
            _customerService = customerService;
            _orderService = orderService;
            _countryService = countryService;
            _expressService = expressService;
            _customerOrderService = customerOrderService;
            _freightService = freightService;
            _wayBillTemplateService = wayBillTemplateService;
            _workContext = workContext;
            _operateLogServices = operateLogServices;
        }

        public ActionResult DownLoadPdf(string wayBillNumber)
        {
            return File(sysConfig.PdfTemplatePath + wayBillNumber + ".pdf", "application/pdf");
        }

        public ActionResult InvoicePrinter(string ids)
        {
            List<string> wayBillNUmber = new List<string>();
            string[] arr = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            arr.Each(v => wayBillNUmber.Add(v.Trim()));
            var list = _customerOrderService.GetCustomerOrderIdByWayBillNumber(wayBillNUmber);



            string customerOrderIds = "";
            list.ForEach(p =>
            {
                customerOrderIds += p + ",";
            });
            return View(BindInvoicePrinter(customerOrderIds));
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult PrintPreview(InvoivePrinterViewModel viewModel)
        {
            InvoivePrinterViewModel model;
            //var list = CacheHelper.Get("cache_countryList") as List<Country>;
            if (TempData["PrinterViewModel"] != null)
            {
                model = TempData["PrinterViewModel"] as InvoivePrinterViewModel;
            }
            else
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                model = BindInvoicePrinter(viewModel.Ids, viewModel.TemplateName);
                sw.Stop();
                string aa = sw.ElapsedMilliseconds.ToString();
            }
            return View(model);
        }

        public ActionResult Printer(string typeId, string ids)
        {
            var model = BindPrinterViewModel(typeId, ids);
            return View(model);
        }

        private PrinterViewModelCommon BindPrinterViewModel(string typeId, string ids, string templateName = "")
        {
            var viewModel = new PrinterViewModelCommon()
            {
                TypeId = typeId,
                Ids = ids
            };

            string[] arr = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length == 0)
            {
                return viewModel;
            }
            List<int> customerOrderIds = new List<int>();
            arr.Each(v => customerOrderIds.Add(v.ConvertTo<int>()));
            var selectList = new List<SelectListItem>();
            var shippingMethodIds = new List<int>();

            var list = GetCustomerOrderListModelCommon(customerOrderIds);
            if (list == null)
            {
                return viewModel;
            }
            //过滤相同的运输方式
            list.ForEach(p =>
            {
                if (shippingMethodIds.Contains(p.ShippingMethodId)) return;
                shippingMethodIds.Add(p.ShippingMethodId);
            });
            IEnumerable<WayBillTemplateExt> wayBillTemplateModelList = new List<WayBillTemplateExt>();
            IEnumerable<WayBillTemplateExt> wayBillTemplateModels = _wayBillTemplateService.GetWayBillTemplateList(shippingMethodIds, typeId);
            var billTemplateModelList = wayBillTemplateModels as WayBillTemplateExt[] ?? wayBillTemplateModels.ToArray();
            if (billTemplateModelList.Any())
            {
                wayBillTemplateModelList = billTemplateModelList;
                string filter = string.Empty;
                foreach (var wayBillTemplate in billTemplateModelList)
                {
                    string val = wayBillTemplate.TemplateName;
                    var listItem = new SelectListItem()
                    {
                        Value = wayBillTemplate.TemplateName,
                        Text = wayBillTemplate.TemplateName,
                        Selected = val.Equals(templateName)
                    };
                    if (filter.Contains(val)) continue;
                    filter += val + ",";
                    selectList.Add(listItem);
                }
            }

            if (selectList.Count > 0)
            {
                //没有选中模板就默认第一个被选中
                SelectListItem item = selectList.FirstOrDefault(p => p.Selected);
                if (item == null)
                {
                    item = selectList.First();
                }
                templateName = item.Value;
            }

            if (!string.IsNullOrWhiteSpace(templateName))
            {
                wayBillTemplateModelList = _wayBillTemplateService.GetGetWayBillTemplateExtByName(templateName);
            }
            viewModel.SelectList = selectList;
            viewModel.CustomerOrderInfoModels = list;
            viewModel.WayBillTemplates = wayBillTemplateModelList;
            return viewModel;
        }

        public ActionResult DHLPrintPreview(string wayBillNumber)
        {
            string[] wayBillNumbers = wayBillNumber.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            DHLPrintViewModel viewModel = new DHLPrintViewModel()
            {
                ExpressAccountInfos = _expressService.GetExpressAccountInfos().ToList(),
                WayBillInfos = _inStorageService.GetWayBillByWayBillNumbers(wayBillNumbers).ToList().FindAll(p => p.ExpressRespons != null)
            };

            var shippingMethods = new List<int>();
            foreach (var wayBillInfo in viewModel.WayBillInfos)
            {
                if (!shippingMethods.Contains(wayBillInfo.OutShippingMethodID.Value))
                {
                    shippingMethods.Add(wayBillInfo.OutShippingMethodID.Value);
                }
            }

            viewModel.ShippingMethodCodes = _freightService.GetShippingMethodsByIds(shippingMethods).ToDictionary(p => p.ShippingMethodId, p => p.Code);

            //打印运单记录到打印日志表
            var getWaybillInfo = _orderService.GetWayBillInfos(wayBillNumbers).ToList().FindAll(p => p.WayBillNumber != null);
            foreach (var wayBillInfo in getWaybillInfo)
            {

                if (!string.IsNullOrWhiteSpace(wayBillInfo.VenderCode) && wayBillInfo.OutShippingMethodID != null &&
                    !string.IsNullOrWhiteSpace(wayBillInfo.TrackingNumber))
                {
                    try
                    {
                        //增加打印日志
                        _inStorageService.AddWayBillPrintLog(
                            new WayBillPrintLog()
                            {
                                waybillnumber = wayBillInfo.WayBillNumber,
                                sendGoodsVender = wayBillInfo.VenderCode,
                                sendGoodsChannel = wayBillInfo.OutShippingMethodID.ToString(),
                                newTrackNumber = wayBillInfo.TrackingNumber,
                                printPerson = _customerService.GetCustomer(wayBillInfo.CustomerCode).Name,
                                printDate = DateTime.Now

                            });
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                    }

                }

            }

            return View(viewModel);
        }

        public ActionResult DHLPrintPreview_1(string wayBillNumber)
        {
            return View((DHLPrintPreview(wayBillNumber) as ViewResult).Model);
        }

        public ActionResult NetherlandsParcelPreview(string wayBillNumber)
        {
            string[] wayBillNumbers = wayBillNumber.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            NLPOSTViewModel model = new NLPOSTViewModel()
                {
                    WayBillInfos = _inStorageService.GetWayBillByWayBillNumbers(wayBillNumbers).ToList()
                };
            return View(model);
        }

        //立陶宛快速打印--不支持打多张
        //Add By zhengsong
        //Time:2015-01-14
        public ActionResult LithuaniaPrintView(string wayBillNumber)
        {
            string[] wayBillNumbers = wayBillNumber.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var lithuania= _inStorageService.GetLithuaniaInfoByWayBillNumber(wayBillNumber);
            LithuaniaViewModel model = new LithuaniaViewModel();
            model.WayBillInfos = _inStorageService.GetWayBillByWayBillNumbers(wayBillNumbers).ToList();
            if (lithuania != null)
            {
                model.MailNo = lithuania.MailNo;
                model.AgentMailNo = lithuania.AgentMailNo;
            }
            return View(model);
        }

        public InvoivePrinterViewModel BindInvoicePrinter(string ids, string templateName = "")
        {
            InvoivePrinterViewModel viewModel = new InvoivePrinterViewModel
            {
                Ids = ids
            };
            string[] arr = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length == 0)
            {
                return viewModel;
            }
            List<int> customerOrderIds = new List<int>();
            arr.Each(v => customerOrderIds.Add(v.ConvertTo<int>()));
            var selectList = new List<SelectListItem>();
            var shippingMethodIds = new List<int>();
            var list = GetPrinterList(customerOrderIds);
            if (list == null)
            {
                return viewModel;
            }
            //过滤相同的运输方式
            list.ForEach(p =>
            {
                if (shippingMethodIds.Contains(p.ShippingMethodId)) return;
                shippingMethodIds.Add(p.ShippingMethodId);
            });
            IEnumerable<WayBillTemplateExt> wayBillTemplateModelList = new List<WayBillTemplateExt>();
            IEnumerable<WayBillTemplateExt> wayBillTemplateModels = _wayBillTemplateService.GetWayBillTemplateList(shippingMethodIds, "DT1308100023");
            var billTemplateModelList = wayBillTemplateModels as WayBillTemplateExt[] ?? wayBillTemplateModels.ToArray();
            if (billTemplateModelList.Any())
            {
                wayBillTemplateModelList = billTemplateModelList;
                string filter = string.Empty;
                foreach (var wayBillTemplate in billTemplateModelList)
                {
                    string val = wayBillTemplate.TemplateName;
                    var listItem = new SelectListItem()
                    {
                        Value = wayBillTemplate.TemplateName,
                        Text = wayBillTemplate.TemplateName,
                        Selected = val.Equals(templateName)
                    };
                    if (filter.Contains(val)) continue;
                    filter += val + ",";
                    selectList.Add(listItem);
                }
            }

            if (selectList.Count > 0)
            {
                //没有选中模板就默认第一个被选中
                SelectListItem item = selectList.FirstOrDefault(p => p.Selected);
                if (item == null)
                {
                    item = selectList.First();
                }
                templateName = item.Value;
            }

            if (!string.IsNullOrWhiteSpace(templateName))
            {
                wayBillTemplateModelList = _wayBillTemplateService.GetGetWayBillTemplateExtByName(templateName);
            }
            viewModel.SelectList = selectList;
            viewModel.CustomerOrderInfoModels = list;
            viewModel.WayBillTemplates = wayBillTemplateModelList;
            return viewModel;
        }

        public ActionResult BarCodeFile(string code, int dpix = 80, int dpiy = 75, bool showText = false)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return Content("无号码");
            }

            var builder = new BarCodeBuilder { SymbologyType = Symbology.Code39Standard, CodeText = code, xDimension = 0.3f };
            //builder.Resolution.DpiX = dpix;
            //builder.Resolution.DpiY = dpiy;

            builder.CodeTextFont = new Font("宋体", 12, FontStyle.Regular);

            if (!showText) builder.CodeLocation = CodeLocation.None;
            Image image = builder.BarCodeImage;
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            image.Dispose();
            byte[] bytes = ms.ToArray();
            ms.Dispose();
            return File(bytes, "image/Bmp");
        }


        /// <summary>
        /// 获取要打印的数据
        /// Add by zhengsong
        /// </summary>
        /// <param name="customerOrderIds"></param>
        /// <returns></returns>
        private List<InvoivePrinterOrderInfoModel> GetPrinterList(IEnumerable<int> customerOrderIds)
        {
            //var cacheList = Cache.Get("cache_customerOrderIds") as int[];
            var orderIds = customerOrderIds as int[] ?? customerOrderIds.ToArray();
            //if (orderIds.Length <= 0) return null;
            //if (null == cacheList)
            //{
            //    Cache.Add("cache_customerOrderIds", orderIds);


            //    var cacheOrderList = GetCustomerOrderListModel(orderIds);

            //    if (cacheOrderList != null)
            //        Cache.Add("cache_customerOrders", cacheOrderList);
            //    return cacheOrderList;
            //}
            //else
            //{
            //    if (Tools.CompareArrContent(orderIds, cacheList))
            //    {
            //        var cacheOrderList = Cache.Get("cache_customerOrders") as List<InvoivePrinterOrderInfoModel>;
            //        if (cacheOrderList == null || cacheOrderList.Count==0)
            //        {
            //            cacheOrderList = GetCustomerOrderListModel(orderIds);
            //            if (cacheOrderList != null)
            //                Cache.Add("cache_customerOrders", cacheOrderList);
            //        }
            //        return cacheOrderList;
            //    }
            //    else
            //    {
            //        var cacheOrderList = GetCustomerOrderListModel(orderIds);
            //        Cache.Add("cache_customerOrderIds", orderIds);
            //        if (cacheOrderList != null)
            //            Cache.Add("cache_customerOrders", cacheOrderList);
            //        return cacheOrderList;
            //    }
            //}

            return GetCustomerOrderListModel(orderIds);
        }

        private List<InvoivePrinterOrderInfoModel> GetCustomerOrderListModel(int[] orderIds)
        {

            try
            {
                //System.Diagnostics.Stopwatch sw = new Stopwatch();
                //sw.Start();
                var list = _customerOrderService.PrintByCustomerOrderIds(orderIds);
                var listModel = new List<InvoivePrinterOrderInfoModel>();

                list.ForEach(p =>
                {
                    var wayBillInfo = p.WayBillInfos.FirstOrDefault();
                    var ApplicationInfos = new List<ApplicationInfoModel1>();
                    p.ApplicationInfos.ToList().ForEach(m => ApplicationInfos.Add(new ApplicationInfoModel1()
                    {
                        ApplicationID = m.ApplicationID,
                        ApplicationName = m.ApplicationName,
                        HSCode = m.HSCode,
                        PickingName = m.PickingName,
                        Qty = m.Qty ?? 0,
                        Remark = m.Remark,
                        Total = m.Total ?? 0,
                        UnitPrice = m.UnitPrice ?? 0,
                        UnitWeight = m.UnitWeight ?? 0,
                        WayBillNumber = m.WayBillNumber
                    }));
                    listModel.Add(new InvoivePrinterOrderInfoModel()
                    {
                        CustomerOrderID = p.CustomerOrderID,
                        CustomerOrderNumber = p.CustomerOrderNumber,
                        CustomerCode = p.CustomerCode,
                        TrackingNumber = p.TrackingNumber,
                        ShippingMethodId = wayBillInfo.OutShippingMethodID ?? 0,
                        ShippingMethodName = wayBillInfo.OutShippingMethodName,
                        GoodsTypeID = p.GoodsTypeID ?? 0,
                        InsuredID = p.InsuredID ?? 0,
                        IsReturn = p.IsReturn,
                        IsInsured = p.IsInsured,
                        IsBattery = p.IsBattery,
                        IsPrinted = p.IsPrinted,
                        IsHold = p.IsHold,
                        Status = p.Status,
                        CreatedOn = p.CreatedOn,
                        SensitiveTypeID = p.SensitiveTypeID ?? 0,
                        PackageNumber = p.PackageNumber,
                        AppLicationType = p.AppLicationType,
                        Weight = p.Weight,
                        Length = p.Length,
                        Width = p.Width,
                        Height = p.Height,
                        ApplicationInfoList = ApplicationInfos,
                        WayBillInfos = p.WayBillInfos.ToList(),
                        ShippingAddress = p.ShippingInfo.ShippingAddress,
                        ShippingCity = p.ShippingInfo.ShippingCity,
                        ShippingCompany = p.ShippingInfo.ShippingCompany,
                        ShippingFirstLastName = p.ShippingInfo.ShippingFirstName + " " + p.ShippingInfo.ShippingLastName,
                        ShippingFirstName = p.ShippingInfo.ShippingFirstName,
                        ShippingLastName = p.ShippingInfo.ShippingLastName,
                        ShippingPhone = p.ShippingInfo.ShippingPhone,
                        ShippingState = p.ShippingInfo.ShippingState,
                        ShippingZip = p.ShippingInfo.ShippingZip,
                        ShippingTaxId = p.ShippingInfo.ShippingTaxId,
                        CountryCode = p.ShippingInfo.CountryCode,

                        ShippingZone = GetShippingZone(p.ShippingMethodId ?? 0, p.ShippingInfo.ShippingZip, p.ShippingInfo.CountryCode)



                    });

                    //#region 操作日志
                    ////yungchu
                    ////敏感字-无
                    //BizLog bizlog = new BizLog()
                    //{
                    //	Summary = "打印发票",
                    //	KeywordType = KeywordType.CustomerOrderNumber,
                    //	Keyword = p.CustomerOrderNumber,
                    //	UserCode = _workContext.User.UserUame,
                    //	UserRealName = _workContext.User.UserUame,
                    //	UserType = UserType.LMS_User,
                    //	SystemCode = SystemType.LMS,
                    //	ModuleName = "打印发票"
                    //};

                    //_operateLogServices.WriteLog(bizlog, listModel);
                    //#endregion


                }
                    );
                listModel.ForEach(p =>
                {

                    if (string.IsNullOrWhiteSpace(p.TrackingNumber))
                    {
                        var firstOrDefault = p.WayBillInfos.FirstOrDefault();
                        if (firstOrDefault != null)
                            p.TrackingNumber = firstOrDefault.WayBillNumber;
                    }
                    p.BarCode = "<img id=\"img\" src=\"/barcode.ashx?m=0&h=35&vCode=" + p.TrackingNumber + "\" alt=\"" +
                                p.TrackingNumber + "\" style=\"width:200px;height:35px;\" />";
                    //var entity = list.Find(m => m.CustomerOrderID.Equals(p.CustomerOrderID));
                    //entity.ShippingInfo.ToModel(p);
                    var country = _countryService.GetCountryByCode(p.CountryCode);
                    p.CountryName = country.Name;
                    p.CountryChineseName = country.ChineseName;

                });
                //sw.Stop();
                //double time = sw.Elapsed.TotalMilliseconds;
                return listModel;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return null;
            }
        }

        private List<CustomerOrderInfoModelCommon> GetCustomerOrderListModelCommon(IEnumerable<int> orderIds)
        {

            try
            {
                //System.Diagnostics.Stopwatch sw = new Stopwatch();
                //sw.Start();
                var list = _customerOrderService.PrintByCustomerOrderIds(orderIds);

                var listModel = new List<CustomerOrderInfoModelCommon>();


                list.ForEach(p => listModel.Add(DecorateCustomerOrderInfo(p)));

                return listModel;

            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return null;
            }
        }

        /// <summary>
        /// 丰富信息
        /// </summary>
        /// <returns></returns>
        private CustomerOrderInfoModelCommon DecorateCustomerOrderInfo(CustomerOrderInfo model)
        {
            var ApplicationInfos = new List<ApplicationInfoModelCommon>();

            model.ApplicationInfos.ToList().ForEach(m => ApplicationInfos.Add(new ApplicationInfoModelCommon()
            {
                ApplicationID = m.ApplicationID,
                ApplicationName = m.ApplicationName,
                HSCode = m.HSCode,
                PickingName = m.PickingName,
                Qty = m.Qty ?? 0,
                Remark = m.Remark,
                Total = m.Total ?? 0,
                UnitPrice = m.UnitPrice ?? 0,
                UnitWeight = m.UnitWeight ?? 0,
                WayBillNumber = m.WayBillNumber
            }));

            var customer = _customerService.GetCustomer(model.CustomerCode);

            var customerOrderInfoModel = new CustomerOrderInfoModelCommon()
            {
                CustomerOrderID = model.CustomerOrderID,
                CustomerOrderNumber = model.CustomerOrderNumber,
                CustomerCode = model.CustomerCode,
                TrackingNumber = model.TrackingNumber,
                ShippingMethodId = model.ShippingMethodId ?? 0,
                ShippingMethodName = model.ShippingMethodName,
                GoodsTypeID = model.GoodsTypeID ?? 0,
                InsuredID = model.InsuredID ?? 0,
                IsReturn = model.IsReturn,
                IsInsured = model.IsInsured,
                IsBattery = model.IsBattery,
                IsPrinted = model.IsPrinted,
                IsHold = model.IsHold,
                Status = model.Status,
                CreatedOn = model.CreatedOn,
                SensitiveTypeID = model.SensitiveTypeID ?? 0,
                PackageNumber = model.PackageNumber,
                AppLicationType = model.AppLicationType,
                Weight = model.Weight,
                Length = model.Length,
                Width = model.Width,
                Height = model.Height,
                ApplicationInfoList = ApplicationInfos,
                WayBillInfos = model.WayBillInfos.ToList(),
                WayBillNumber = model.WayBillInfos.FirstOrDefault() == null ? "" : model.WayBillInfos.FirstOrDefault().WayBillNumber,
                ShippingAddress = (model.ShippingInfo.ShippingAddress + " " + model.ShippingInfo.ShippingAddress1 + " " + model.ShippingInfo.ShippingAddress2).Trim(),
                ShippingCity = model.ShippingInfo.ShippingCity,
                ShippingCompany = model.ShippingInfo.ShippingCompany,
                ShippingFirstLastName = model.ShippingInfo.ShippingFirstName + " " + model.ShippingInfo.ShippingLastName,
                ShippingFirstName = model.ShippingInfo.ShippingFirstName,
                ShippingLastName = model.ShippingInfo.ShippingLastName,
                ShippingPhone = model.ShippingInfo.ShippingPhone,
                ShippingState = model.ShippingInfo.ShippingState,
                ShippingZip = model.ShippingInfo.ShippingZip,
                ShippingTaxId = model.ShippingInfo.ShippingTaxId,
                CountryCode = model.ShippingInfo.CountryCode,
                CustomerName = customer.Name,
                ShippingZone = GetShippingZone(model.ShippingMethodId ?? 0, model.ShippingInfo.ShippingZip, model.ShippingInfo.CountryCode),

            };

            if (string.IsNullOrWhiteSpace(customerOrderInfoModel.TrackingNumber))
            {
                var firstOrDefault = customerOrderInfoModel.WayBillInfos.FirstOrDefault();
                if (firstOrDefault != null)
                    customerOrderInfoModel.TrackingNumber = firstOrDefault.WayBillNumber;
            }

            customerOrderInfoModel.BarCode = "<img id=\"img\" src=\"/barcode.ashx?m=0&h=35&vCode=" + customerOrderInfoModel.TrackingNumber + "\" alt=\"" +
                                             customerOrderInfoModel.TrackingNumber + "\" style=\"width:200px;height:35px;\" />";

            customerOrderInfoModel.BarCode128 = "<img id=\"img\" src=\"/print/barcode128h?Code=" + customerOrderInfoModel.TrackingNumber + "\" alt=\"" +
                                                customerOrderInfoModel.TrackingNumber + "\" style=\"\" />";

            customerOrderInfoModel.CustomerOrderNumberCode39 = "<img id=\"img\" src=\"/print/barcode39?Code=" + customerOrderInfoModel.CustomerOrderNumber + "\" alt=\"" +
                                                               customerOrderInfoModel.CustomerOrderNumber + "\" style=\"\" />";

            customerOrderInfoModel.CustomerOrderNumberCode128 = "<img id=\"img\" src=\"/print/barcode128?Code=" + customerOrderInfoModel.CustomerOrderNumber + "\" alt=\"" +
                                                                customerOrderInfoModel.CustomerOrderNumber + "\" style=\"\" />";

            customerOrderInfoModel.CustomerOrderNumberCode128L = "<img id=\"img\" src=\"/print/barcode128?Code=" + customerOrderInfoModel.CustomerOrderNumber + "&dpiy=40\" alt=\"" +
                                                                 customerOrderInfoModel.CustomerOrderNumber + "\" style=\"\" />";

            customerOrderInfoModel.TrackingNumberCode39 = "<img id=\"img\" src=\"/print/barcode39?Code=" + customerOrderInfoModel.TrackingNumber + "\" alt=\"" +
                                                          customerOrderInfoModel.TrackingNumber + "\" style=\"\" />";

            customerOrderInfoModel.TrackingNumberCode128 = "<img id=\"img\" src=\"/print/barcode128?Code=" + customerOrderInfoModel.TrackingNumber + "\" alt=\"" +
                                                           customerOrderInfoModel.TrackingNumber + "\" style=\"\" />";

            customerOrderInfoModel.WayBillNumberCode39 = "<img id=\"img\" src=\"/print/barcode39?Code=" + customerOrderInfoModel.WayBillNumber + "\" alt=\"" +
                                                         customerOrderInfoModel.WayBillNumber + "\" style=\"\" />";

            customerOrderInfoModel.WayBillNumberCode128 = "<img id=\"img\" src=\"/print/barcode128?Code=" + customerOrderInfoModel.WayBillNumber + "\" alt=\"" +
                                                          customerOrderInfoModel.WayBillNumber + "\" style=\"\" />";

            customerOrderInfoModel.CustomerOrderNumberCode128Lh = "<img id=\"img\" src=\"/print/barcode128?Code=" + customerOrderInfoModel.CustomerOrderNumber + "&dpiy=40&angleF=90&showText=true\" alt=\"" +
                                                                 customerOrderInfoModel.CustomerOrderNumber + "\" style=\"\" />";

            var country = GetCountryList().Single(c => c.CountryCode == customerOrderInfoModel.CountryCode);
            customerOrderInfoModel.CountryName = country.Name;
            customerOrderInfoModel.CountryChineseName = country.ChineseName;
            customerOrderInfoModel.MouthNumber = GetMouthCountryList().Find(c => c.CountryCode == customerOrderInfoModel.CountryCode).MouthNumber;
            return customerOrderInfoModel;
        }

        /// <summary>
        /// 获取运输方式
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetShippingMethodList()
        {

            List<SelectListItem> listItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "" } };
            var list = _freightService.GetShippingMethodByTypeId();
            foreach (var shippingMethodModel in list)
            {
                SelectListItem item = new SelectListItem();
                item.Text = shippingMethodModel.FullName;
                item.Value = shippingMethodModel.ShippingMethodId.ToString();
                listItems.Add(item);
            }
            return listItems;
        }

        private List<Country> GetCountryList()
        {
            var list = Cache.Get("cache_countryList") as List<Country>;
            if (list == null)
            {
                list = _countryService.GetCountryList("");
                Cache.Add("cache_countryList", list);
            }
            return list;
        }

        private List<MouthCountry> GetMouthCountryList()
        {
            var list = Cache.Get("cache_MouthCountryList") as List<MouthCountry>;
            if (list == null)
            {
                list = _countryService.GetMouthCountryList();
                Cache.Add("cache_MouthCountryList", list);
            }
            return list;
        }

        /// <summary>
        /// 获取收件人的所在区号
        /// </summary>
        /// <param name="shippingMethodId">运输方式ID</param>
        /// <param name="postCode">邮政编号</param>
        /// <param name="countryCode">国家代码</param>
        /// <returns></returns>
        private int GetShippingZone(int shippingMethodId, string postCode, string countryCode)
        {
            int zone = 0;

            //非俄速通小包专线挂号时就返回0
            if (shippingMethodId != sysConfig.SpecialShippingMethodId)
            {
                List<ShippingMethodCountryModel> shippingMethod = _freightService.GetCountryArea(shippingMethodId, countryCode);
                if (shippingMethod != null && shippingMethod.Count > 0)
                    zone = shippingMethod.First().AreaId;
                return zone;
            }

            if (string.IsNullOrWhiteSpace(postCode))
                return zone;

            var firstStr = postCode.Substring(0, 1);
            if (firstStr == "1" || firstStr == "2" || firstStr == "3" || firstStr == "4")
            {
                switch (firstStr)
                {
                    case "1":
                        zone = 1;
                        break;
                    case "2":
                        zone = 2;
                        break;
                    case "3":
                        zone = 3;
                        break;
                    case "4":
                        zone = 4;
                        break;
                    default:
                        zone = 0;
                        break;
                }
            }
            else
            {
                var twoStr = postCode.Substring(0, 2);
                if (twoStr == "60" || twoStr == "61" || twoStr == "62")
                {
                    switch (twoStr)
                    {
                        case "60":
                        case "61":
                        case "62":
                            zone = 4;
                            break;
                        default:
                            zone = 6;
                            break;
                    }
                }
                else
                {
                    var threeStr = postCode.Substring(0, 3);
                    if (threeStr == "640" || threeStr == "641")
                    {
                        switch (threeStr)
                        {
                            case "640":
                            case "641":
                                zone = 4;
                                break;
                            default:
                                zone = 6;
                                break;
                        }
                    }
                    else
                    {
                        zone = 6;
                    }
                }
            }

            return zone;
        }


    }

    public class ExportWayBillModel
    {
        public ExportWayBillModel()
        {
            ApplicationInfoModels = new List<ApplicationInfoModel>();
        }

        //运单信息表
        public string CustomerOrderNumber { get; set; }

        public string CustomerCode { get; set; }

        public string Name { get; set; }

        public string WayBillNumber { get; set; }

        public bool IsReturn { get; set; }

        public string TrackingNumber { get; set; }

        public string TrueTrackingNumber { get; set; }

        public string InShippingMethodName { get; set; }

        public string InsuredName { get; set; }

        public string WayCreatedOn { get; set; }

        public string Status { get; set; }
        //收件人信息
        public string CountryCode { get; set; }

        public string ChineseName { get; set; }

        public string ShippingFirstName { get; set; }

        public string ShippingLastName { get; set; }

        public string ShippingCompany { get; set; }

        public string ShippingAddress { get; set; }

        public string ShippingCity { get; set; }

        public string ShippingState { get; set; }

        public string ShippingZip { get; set; }

        public string ShippingPhone { get; set; }

        public string ShippingTaxId { get; set; }

        public string ShiCreatedOn { get; set; }

        //发件人信息
        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderCompany { get; set; }
        public string SenderAddress { get; set; }
        public string SenderCity { get; set; }
        public string SenderState { get; set; }
        public string SenderZip { get; set; }
        public string SenderPhone { get; set; }
        public string SenCreatedOn { get; set; }

        //敏感货物类型表
        public string SensitiveTypeName { get; set; }

        public Nullable<decimal> Weight { get; set; }
        public Nullable<decimal> SettleWeight { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Height { get; set; }

        public Nullable<decimal> InsureAmount { get; set; }
        public Nullable<int> PackageNumber { get; set; }
        public string AppLicationType { get; set; }
        public Nullable<decimal> UnitWeight { get; set; }
        public string PickingName { get; set; }
        public string Remark { get; set; }

        //是否关税预付
        public bool EnableTariffPrepay { get; set; }

        //申报信息表
        public List<ApplicationInfoModel> ApplicationInfoModels { get; set; }
    }


    public class EUBWayBill
    {
        public int Number { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? InStorageCreatedOn { get; set; }

        public DateTime? OutStorageCreatedOn { get; set; }

        public string TrackingNumber { get; set; }

        public string TrackingNumberCode { get; set; }

        public byte[] TrackingNumberFile { get; set; }

        public string CountryCode { get; set; }

        public decimal? SettleWeight { get; set; }

        public int? SensitiveTypeID { get; set; }

        public string IsCharged { get; set; }

        public string Online { get; set; }
    }

    public class SinoUSWayBill
    {
        public string WayBillNumber { get; set; }

        public string TrackingNumber { get; set; }

        //收件人信息
        public string CountryCode { get; set; }

        public string ShippingName { get; set; }

        public string ShippingCompany { get; set; }

        public string ShippingAddress1 { get; set; }

        public string ShippingAddress2 { get; set; }

        public string ShippingCity { get; set; }

        public string ShippingState { get; set; }

        public string ShippingZip { get; set; }

        public string ShippingPhone { get; set; }

        //邮箱
        public string ShippingEmail { get; set; }



        public Nullable<decimal> Weight { get; set; }

        //单位
        public string GrossWeightUom { get; set; }

        //申报价值（总）
        public decimal ApplicationPrice { get; set; }

        //申报名称
        public string ApplicationName { get; set; }

        //币种
        public string PackageCurrencyCode { get; set; }

        //计量单位
        public string DimUom { get; set; }

        public string PurchasingUrl { get; set; }

        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Height { get; set; }


        public string Remark1 { get; set; }
        public string Remark2 { get; set; }

        public string HouseAirWaybillNumber { get; set; }
        public string SkuTouchFulfillmentNo { get; set; }
        public string CarrierServiceCode { get; set; }
        public string Printed { get; set; }
        public string ValidationMessage { get; set; }

        //是否需要标红的数据
        //0 表示不需要
        //1 表示需要
        public string IsRed { get; set; }

    }

    public class EuropeWayBill
    {
        public string CustomerOrderNumber { get; set; }

        public string WayBillNumber { get; set; }

        public string PrealertReference { get; set; }

        public string ShippingMethod { get; set; }
        //收件人信息
        public string CountryCode { get; set; }

        public string ShippingName { get; set; }

        public string ShippingCompany { get; set; }

        public string ShippingAddress { get; set; }

        public string ShippingCity { get; set; }

        public string ShippingState { get; set; }

        public string ShippingZip { get; set; }

        public string ShippingPhone { get; set; }

        //邮箱
        public string ShippingEmail { get; set; }

        //币种
        public string Currency { get; set; }
        //申报名称
        public string ApplicationName { get; set; }
        //数量
        public Nullable<int> Qty { get; set; }
        //申报信息净重
        public Nullable<decimal> UnitWeight { get; set; }
        //申报名称单价
        public Nullable<decimal> UnitPrice { get; set; }

        public string HouseNumber { get; set; }
        public string HouseNumberExtension { get; set; }
        public string AdditionalAddressInfo { get; set; }

        //申报信息备注
        public string ApplicationRemark { get; set; }
        //海关编码
        public string HsCode { get; set; }

        public string ShippingCosts { get; set; }

        public string ProductUrl { get; set; }

    }

    public class CountryListModel
    {
        public CountryListModel()
        {
            FilterModel = new CountryFilterModel();
            CountryModels = new PagedList<CountryModel>();
            Status = new List<SelectListItem>();
        }

        public CountryFilterModel FilterModel { get; set; }
        public IPagedList<CountryModel> CountryModels { get; set; }
        public IList<SelectListItem> Status { get; set; }
    }

    public class DataResult
    {
        public int Type { get; set; }
        public string TemplateName { get; set; }
    }

    public class ShippingMethodJsonReuslt
    {
        public ShippingMethodJsonReuslt()
        {
            Items = new List<ShippingMethodItem>();
        }

        public List<ShippingMethodItem> Items;
    }
    public class ShippingMethodItem
    {
        public int ShippingMethodId { get; set; }
        public string ShippingMethodName { get; set; }
    }

    public class CountryFilterModel : SearchParam
    {
        public CountryFilterModel()
        {
            PageSize = 9999;
        }

        public string SelectedField { get; set; }
        public string SeekValue { get; set; }
        public string CountryCode { get; set; }
        public string Name { get; set; }
        public string ChineseName { get; set; }
        public string ChineseNameList { get; set; }
        public string TrackingNumberID { get; set; }
        public string Codes { get; set; }
        public string SiteId { get; set; }
        public string SelectValue { get; set; }
    }

    public class ExportList
    {
        public int Index { get; set; }
        public string CountryCode { get; set; }
        public int PackCount { get; set; }
        public decimal TotalWeight { get; set; }
        public string CountryName { get; set; }
        public int Area { get; set; }
    }

    public class CanQuickPrintResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string[] Urls{ get; set; }
    }

    public class UpdateOutStorageViewModel
    {
        public int OutShippingMethodID { get; set; }
        public string OutStorageID { get; set; }
        public string Url { get; set; }
        public string Remark { get; set; }
        public string Type { get; set; }
    }

    public class OutWayBillModel
    {
        public int Status { get; set; }
        public bool IsHold { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public string TrackingNumber { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string WayBillNumber { get; set; }
        public string CountryCode { get; set; }
        public string InShippingMethodName { get; set; }
        public int? InShippingMethodID { get; set; }
        public string CustomerCode { get; set; }
    }
}