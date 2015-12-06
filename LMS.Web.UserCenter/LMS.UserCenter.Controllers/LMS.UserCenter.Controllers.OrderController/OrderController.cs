using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Util;
using Aspose.BarCode;
using FluentValidation.Results;
using LMS.Services.SF.Model;
using LighTake.Infrastructure.Common.BizLogging.Enums;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LMS.Services.CommonServices;
using LMS.Services.CountryServices;
using LMS.Services.CustomerOrderServices;
using LMS.Services.CustomerServices;
using LMS.Services.DictionaryTypeServices;
using LMS.Services.EubWayBillServices;
using LMS.Services.FreightServices;
using LMS.Services.InStorageServices;
using LMS.Services.OrderServices;
using LMS.Services.WayBillTemplateServices;
using LMS.Services.TrackingNumberServices;
using LMS.UserCenter.Controllers.OrderController.Models;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Excel;
using LighTake.Infrastructure.Common.InversionOfControl;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Web;
using LighTake.Infrastructure.Web.Controllers;
using LighTake.Infrastructure.Web.Filters;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using RazorEngine;
using LMS.Services.OperateLogServices;
using LighTake.Infrastructure.Common.BizLogging;
using LighTake.Infrastructure.CommonQueue;

namespace LMS.UserCenter.Controllers.OrderController
{
    [MemberOnly]
    public class OrderController : BaseController
    {
        private readonly ICustomerOrderService _customerOrderService;
        private readonly IWorkContext _workContext;
        private readonly IFreightService _freightService;
        private readonly ICountryService _countryService;
        private readonly IInsuredCalculationService _insuredCalculationService;
        private readonly ISensitiveTypeInfoService _sensitiveTypeInfoService;
        private readonly IOrderService _orderService;
        private readonly IGoodsTypeService _goodsTypeService;
        private readonly IDictionaryTypeService _dictionaryTypeService;
        private readonly IWayBillTemplateService _wayBillTemplateService;
        private readonly ITrackingNumberService _trackingNumberService;
        private readonly IEubWayBillService _eubWayBillService;
        private readonly ICustomerService _customerService;
        private readonly IWayBillInfoRepository _wayBillInfoRepository;
        private readonly IOperateLogServices _operateLogServices;
	    private readonly ICustomerOrderInfoRepository _customerOrderInfoRepository;

        public OrderController(IWorkContext workContext,
            ICustomerOrderService customerOrderService,
            IFreightService freightService,
            ICountryService countryService,
            IInsuredCalculationService insuredCalculationService,
            ISensitiveTypeInfoService sensitiveTypeInfoService,
            IOrderService orderService,
            IGoodsTypeService goodsTypeService,
            IDictionaryTypeService dictionaryTypeService,
            IWayBillTemplateService wayBillTemplateService,
            ITrackingNumberService trackingNumberService,
            IEubWayBillService eubWayBillService,
            ICustomerService customerService,
            IWayBillInfoRepository wayBillInfoRepository,
			IOperateLogServices operateLogServices, ICustomerOrderInfoRepository customerOrderInfoRepository)
        {
            _workContext = workContext;
            _customerOrderService = customerOrderService;
            _freightService = freightService;
            _countryService = countryService;
            _insuredCalculationService = insuredCalculationService;
            _sensitiveTypeInfoService = sensitiveTypeInfoService;
            _orderService = orderService;
            _goodsTypeService = goodsTypeService;
            _dictionaryTypeService = dictionaryTypeService;
            _wayBillTemplateService = wayBillTemplateService;
            _trackingNumberService = trackingNumberService;
            _eubWayBillService = eubWayBillService;
            _customerService = customerService;
            _wayBillInfoRepository = wayBillInfoRepository;
            _operateLogServices = operateLogServices;
	        _customerOrderInfoRepository = customerOrderInfoRepository;
        }

        #region Utilities

        public List<SelectListItem> GetShipingMethods(bool isEnabled = false)
        {
            var list = new List<SelectListItem> { new SelectListItem { Value = "", Text = "" } };
            _freightService.GetShippingMethodListByCustomerCode(_workContext.User.UserUame, isEnabled).ForEach(s => list.Add(new SelectListItem
                {
                    Value = s.ShippingMethodId.ToString(CultureInfo.InvariantCulture),
                    Text = s.ShippingMethodName
                }));

            return list;
        }

        public List<SelectListItem> GetCountryList(string keyword)
        {
            var list = new List<SelectListItem> { new SelectListItem { Value = "", Text = "" } };
            //var list = new List<SelectListItem>();
            _countryService.GetCountryList(keyword).ForEach(c => list.Add(new SelectListItem
                {
                    Value = c.CountryCode,
                    Text = string.Format("{0}|{1}", c.CountryCode, c.ChineseName)
                }));

            return list;
        }

        public List<SelectListItem> GetInsuredList()
        {
            var list = new List<SelectListItem>();
            _insuredCalculationService.GetList().ForEach(c => list.Add(new SelectListItem
                {
                    Value = c.InsuredID.ToString(CultureInfo.InvariantCulture) + "_" + c.InsuredCalculation1,
                    Text = c.InsuredName
                }));

            return list;
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

        public List<SelectListItem> GetGoodsTypeList()
        {
            var list = new List<SelectListItem>();
            _goodsTypeService.GetList().ForEach(c => list.Add(new SelectListItem
            {
                Value = c.GoodsTypeID.ToString(CultureInfo.InvariantCulture),
                Text = c.GoodsTypeName
            }));

            return list;
        }

        public List<SelectListItem> GetWaybillStatusList()
        {
            var list = new List<SelectListItem> { new SelectListItem { Value = "", Text = "" } };
            var statusList = new List<int>
                {
                        CustomerOrder.StatusEnum.None.GetStatusValue(),
                        CustomerOrder.StatusEnum.OK.GetStatusValue(),
                        CustomerOrder.StatusEnum.Submitted.GetStatusValue(),
                    };
            CustomerOrder.GetStatusList().ForEach(c =>
            {
                if (!statusList.Contains(Convert.ToInt32(c.ValueField)))
                {
                    list.Add(new SelectListItem
                        {
                            Value = c.ValueField,
                            Text = c.TextField
                        });
                }
            })
            ;

            return list;
        }

        public List<SelectListItem> GetWaybillALLStatusList()
        {
            var list = new List<SelectListItem> { new SelectListItem { Value = "", Text = "" } };
            CustomerOrder.GetStatusList().ForEach(c => list.Add(new SelectListItem
                {
                    Value = c.ValueField,
                    Text = c.TextField
                }))
            ;

            return list;
        }

        public static string ToJson(object obj)
        {
            //JSON序列化
            var serializer = new DataContractJsonSerializer(obj.GetType());
            //定义一个stream用来存放序列化之后的内容
            Stream stream = new MemoryStream();
            serializer.WriteObject(stream, obj);
            //从头到尾将stream读取成一个字符串形式的数据，并且返回
            stream.Position = 0;
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public void InitAdd(bool isEnabled = false)
        {
            ViewBag.CountryList = GetCountryList("");
            ViewBag.ShippingMethods = GetShipingMethods(isEnabled);
            ViewBag.InsuredList = GetInsuredList();
            ViewBag.SensitiveType = GetSensitiveTypeList();
            ViewBag.WaybillStatusList = GetWaybillStatusList();
            ViewBag.GoodsTypeList = GetGoodsTypeList();
        }

        #endregion

        /*//
        // GET: /Order/
        public ActionResult Index()
        {
            return View();
        }*/

        #region Order

        public CustomerOrderViewModels List(CustomerOrderFilter filter)
        {
            if (IsPostRequest)
                filter.Page = 1;

            var viewModels = new CustomerOrderViewModels() { Filter = filter };
            WayBill.GetSearchFilterList().ForEach(p =>
            {
                if (p.ValueField != "4")
                {
                    viewModels.SearchWheres.Add(new SelectListItem() { Text = p.TextField, Value = p.ValueField, Selected = filter.SearchWhere.HasValue && p.ValueField == filter.SearchWhere.Value.ToString() });
                }
            });
            viewModels.PrintTemplate =
                _dictionaryTypeService.GetSelectList(DictionaryTypeInfo.WayBillTemplateType);
            TrackingNumberInfo.GetAddressLabelList().ForEach(p => viewModels.AddressLabel.Add(new SelectListItem { Value = p.ValueField, Text = p.TextField }));
            var param = new CustomerOrderParam
                {
                    CustomerOrderNumber = filter.CustomerOrderNumber,
                    SearchContext = filter.SearchContext,
                    SearchWhere = filter.SearchWhere,
                    CountryCode = filter.CountryCode,
                    CreatedOnFrom = filter.CreatedOnFrom,
                    CreatedOnTo = (filter.CreatedOnTo.HasValue ? filter.CreatedOnTo.Value.ToString("yyyy-MM-dd 23:59:59") : null).ConvertTo<DateTime?>(),
                    ShippingMethodId = filter.ShippingMethodId,
                    Status = filter.Status,
                    IsReceived = filter.IsReceived,
                    IsDeliver = filter.IsDeliver,
                    IsPrinted = filter.IsPrinted,
                    IsAll = filter.IsAll,
                    IsHold = filter.IsHold,
                    Page = filter.Page,
                    PageSize = filter.PageSize,
                    WayBillNumber = filter.WayBillNumber,
                    CustomerCode = _workContext.User.UserUame

                };
            var maxCustomerOrderId = 0;
            if (filter.Page > 1)
            {
                if (Session["MaxCustomerOrderId"] != null)
                {
                    maxCustomerOrderId = int.Parse(Session["MaxCustomerOrderId"].ToString());
                }
                else
                {
                    Session["MaxCustomerOrderId"] = _customerOrderService.GetMaxCustomerOrderID();
                }
            }
            else
            {
                Session["MaxCustomerOrderId"] = _customerOrderService.GetMaxCustomerOrderID();
            }
            if (!filter.IsHold)
            {
                var list = _customerOrderService.GetList(param, maxCustomerOrderId);
                viewModels.OrderList = list.ToModelAsPageCollection<CustomerOrderInfoExt, CustomerOrderInfoModel>();
            }
            else
            {
                var list = _customerOrderService.GetCustomerOrderByBlockedList(param, maxCustomerOrderId);
                viewModels.OrderList = list.ToModelAsPageCollection<CustomerOrderInfoExt, CustomerOrderInfoModel>();

            }

            InitAdd();
            if (filter.IsAll)
            {
                ViewBag.WaybillStatusList = GetWaybillALLStatusList();
            }

            return viewModels;
        }


        public ActionResult Add()
        {
            CustomerOrderInfoModel model = new CustomerOrderInfoModel();
            var applicationTypelist = new List<SelectListItem>();
            CustomerOrder.GetApplicationTypeList().ForEach(c =>
                    applicationTypelist.Add(new SelectListItem
                    {
                        Value = c.ValueField,
                        Text = c.TextField,
                    })
               );
            model.AppLicationTypes = applicationTypelist;
            InitAdd();
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(CustomerOrderInfoModel model)
        {
            var applicationTypelist = new List<SelectListItem>();
            var insureList = _insuredCalculationService.GetList();
            CustomerOrder.GetApplicationTypeList().ForEach(c =>
                    applicationTypelist.Add(new SelectListItem
                    {
                        Value = c.ValueField,
                        Text = c.TextField,
                    })
               );
            model.AppLicationTypes = applicationTypelist;

            if (!ModelState.IsValid)
            {
                InitAdd();
                return View(model);
            }
            var shippingMethod = _freightService.GetShippingMethod(model.ShippingMethodId);


            if (model.Width == null || model.Width <= 0)
            {
                model.Width = 1;
            }
            if (model.Height == null || model.Height <= 0)
            {
                model.Height = 1;
            }
            if (model.Length == null || model.Length <= 0)
            {
                model.Length = 1;
            }

            //中美专线
            if (shippingMethod != null &&
                sysConfig.SinoUSShippingMethodCode.Split(',').ToList().Contains(shippingMethod.Code))
            {
                if (model.ShippingAddress.Length > 35)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingAddress", "长度超过35个字符".FormatWith(model.ShippingAddress));
                    return View(model);
                }
                else if (!string.IsNullOrWhiteSpace(model.ShippingAddress1) && model.ShippingAddress1.Trim().Length > 35)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingAddress1", "长度超过35个字符".FormatWith(model.ShippingAddress1));
                    return View(model);
                }
                else if (!string.IsNullOrWhiteSpace(model.ShippingAddress2) &&
                         model.ShippingAddress2.Trim().Length > 35)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingAddress2", "长度超过35个字符".FormatWith(model.ShippingAddress2));
                    return View(model);
                }

                if (model.CustomerOrderNumber.Length > 35)
                {
                    InitAdd();
                    ModelState.AddModelError("CustomerOrderNumber", "长度超过35个字符".FormatWith(model.CustomerOrderNumber));
                    return View(model);
                }

                if (!string.IsNullOrWhiteSpace(model.ShippingCompany) && model.ShippingCompany.Trim().Length > 35)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingCompany", "长度超过35个字符".FormatWith(model.ShippingCompany));
                    return View(model);
                }

                if (!string.IsNullOrWhiteSpace(model.ShippingFirstName) &&
                    !string.IsNullOrWhiteSpace(model.ShippingLastName))
                {
                    if ((model.ShippingFirstName.Trim().Length + model.ShippingLastName.Trim().Length) > 35)
                    {
                        InitAdd();
                        ModelState.AddModelError("ShippingFirstName", "长度超过35个字符".FormatWith(model.ShippingFirstName));
                        return View(model);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(model.ShippingFirstName))
                {
                    if (model.ShippingFirstName.Trim().Length > 35)
                    {
                        InitAdd();
                        ModelState.AddModelError("ShippingFirstName", "长度超过35个字符".FormatWith(model.ShippingFirstName));
                        return View(model);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(model.ShippingLastName))
                {
                    if (model.ShippingLastName.Trim().Length > 35)
                    {
                        InitAdd();
                        ModelState.AddModelError("ShippingLastName", "长度超过35个字符".FormatWith(model.ShippingLastName));
                        return View(model);
                    }
                }

                if (!string.IsNullOrWhiteSpace(model.ShippingCity) && model.ShippingCity.Length > 35)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingCity", "城市超过35个字符".FormatWith(model.ShippingCity));
                    return View(model);
                }

                if (!string.IsNullOrWhiteSpace(model.ShippingState) && model.ShippingState.Length > 2)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingState", "省份超过2个字符".FormatWith(model.ShippingState));
                    return View(model);
                }

                if (!string.IsNullOrWhiteSpace(model.ShippingZip))
                {
                    if ((model.ShippingZip.Length > 9 || model.ShippingZip.Length < 5))
                    {
                        InitAdd();
                        ModelState.AddModelError("ShippingZip", "字符长度为5-9之间".FormatWith(model.ShippingZip));
                        return View(model);
                    }
                }

                if (model.CountryCode.Length > 2)
                {
                    InitAdd();
                    ModelState.AddModelError("CountryCode", "简码超过2个字符".FormatWith(model.CountryCode));
                    return View(model);
                }

            }
            else if(shippingMethod!=null&&sysConfig.NLPOSTMethodCode.Split(new string[]{","},StringSplitOptions.RemoveEmptyEntries).ToList().Contains(shippingMethod.Code))
            {
                //顺丰荷兰小包
                ValidationResult customerOrderResult = new CustomerOrderInfoModelNlPostValidator().Validate(model);
                if (!customerOrderResult.IsValid)
                {
                    var errsb = new StringBuilder();
                    foreach (var err in customerOrderResult.Errors)
                    {
                        errsb.AppendLine(err.ErrorMessage);
                    }
                    InitAdd();
                    ErrorNotification(errsb.ToString());
                    return View(model);
                }
            }
            else if (shippingMethod != null && sysConfig.LithuaniaMethodCode.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Contains(shippingMethod.Code))
            {
                // 俄罗斯挂号、平邮运输方式编码
                ValidationResult customerOrderResult = new OrderSfModelValidator().Validate(new OrderSfModel()
                    {
                        ShippingName = model.ShippingFirstName + model.ShippingLastName,
                        ShippingAddress = model.ShippingAddress + model.ShippingAddress1 + model.ShippingAddress2,
                        ShippingCity = model.ShippingCity,
                        ShippingCompany = model.ShippingCompany,
                        ShippingPhone = model.ShippingPhone,
                        ShippingState = model.ShippingState,
                        ShippingTel = model.ShippingPhone,
                        ShippingZip = model.ShippingZip,
                        CountryCode = model.CountryCode
                    });
                if (!customerOrderResult.IsValid)
                {
                    var errsb = new StringBuilder();
                    foreach (var err in customerOrderResult.Errors)
                    {
                        errsb.AppendLine(err.ErrorMessage);
                    }
                    InitAdd();
                    ErrorNotification(errsb.ToString());
                    return View(model);
                }
            }

            if (model.ShippingMethodId != 0)
            {
                //客户是否开启关税预付 yungchu
                List<TariffPrepayFeeShippingMethod> listTariffPrepayFee = _freightService.GetShippingMethodsTariffPrepay(_workContext.User.UserUame);

                if ((model.EnableTariffPrepay))
                {
                    if (listTariffPrepayFee == null || listTariffPrepayFee.Count == 0)
                    {
                        InitAdd();
                        ErrorNotification("您未开通关税预付权限，请联系业务");
                        return View(model);
                    }
                    else  //客户是否开启该运输方式关税预付
                    {
                        List<int> listStr = new List<int>();
                        listTariffPrepayFee.ForEach(a => listStr.Add(a.ShippingMethodId));
                        if (!listStr.Contains(model.ShippingMethodId))
                        {
                            InitAdd();
                            ErrorNotification("您未开通关税预付权限，请联系业务");
                            return View(model);

                        }
                    }
                }

            }

            if (_customerOrderService.IsExists(_workContext.User.UserUame, model.CustomerOrderNumber))
            {
                InitAdd();
                ModelState.AddModelError("CustomerOrderNumber", "[客户订单号-{0}]已经存在".FormatWith(model.CustomerOrderNumber));
                return View(model);
            }
            if (!string.IsNullOrWhiteSpace(model.TrackingNumber))
            {
                if (_orderService.IsExitTrackingNumber(model.TrackingNumber))
                {
                    InitAdd();
                    ModelState.AddModelError("TrackingNumber", "[跟踪号-{0}]已经存在".FormatWith(model.TrackingNumber));
                    return View(model);
                }
            }

            //收货国家俄罗斯，邮编不能为空
            if (shippingMethod != null && shippingMethod.Code.Trim() == "LTPOST")
            {
                if (model.CountryCode.ToUpperInvariant() == "RU")
                {
                    if (string.IsNullOrWhiteSpace(model.ShippingZip))
                    {
                        InitAdd();
                        ModelState.AddModelError("ShippingZip", "邮编不能为空".FormatWith(model.ShippingZip));
                        return View(model);
                    }
                }
            }

            #region 中邮挂号福州
            if (shippingMethod != null && (shippingMethod.Code.Trim().ToUpperInvariant() == "CNPOST-FZ" || shippingMethod.Code.Trim().ToUpperInvariant() == "CNPOST-FYB"))
            {
                if (model.CustomerOrderNumber != null && model.CustomerOrderNumber.Length > 30)
                {
                    InitAdd();
                    ModelState.AddModelError("CustomerOrderNumber", "订单号长度必须小于等于30".FormatWith(model.CustomerOrderNumber));
                    return View(model);
                }
                //国家两位
                if (model.CountryCode != null && model.CountryCode.Length != 2)
                {
                    InitAdd();
                    ModelState.AddModelError("CountryCode", "国家简码必须是两位".FormatWith(model.CountryCode));
                    return View(model);
                }
                //收件人州或省
                if (model.ShippingState != null && model.ShippingState.Length > 50)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingState", "收件人省或州长度不能超过50".FormatWith(model.ShippingState));
                    return View(model);
                }
                //收件人城市
                if (model.ShippingCity != null && model.ShippingCity.Length > 50)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingCity", "收件人城市长度不能超过50".FormatWith(model.ShippingCity));
                    return View(model);
                }
                //收件人地址
                string address = "";
                if (model.ShippingAddress != null)
                {
                    address += model.ShippingAddress;
                }
                if (model.ShippingAddress1 != null)
                {
                    address += model.ShippingAddress1;
                }
                if (model.ShippingAddress2 != null)
                {
                    address += model.ShippingAddress2;
                }
                if (address.Length > 120)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingAddress", "收件人地址长度不能超过120".FormatWith(model.ShippingAddress));
                    return View(model);
                }
                //收件人邮编
                if (model.ShippingZip != null && model.ShippingZip.Length > 12)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingZip", "收件人邮编长度不能超过12".FormatWith(model.ShippingZip));
                    return View(model);
                }
                //收件人名字
                string name = "";
                if (model.ShippingFirstName != null)
                {
                    name += model.ShippingFirstName;
                }
                if (model.ShippingLastName != null)
                {
                    name += model.ShippingLastName;
                }
                if (name.Length > 64)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingFirstName", "收件人名字长度不能超过64".FormatWith(model.ShippingFirstName));
                    return View(model);
                }
                //收件人电话
                if (model.ShippingPhone != null && model.ShippingPhone.Length > 20)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingPhone", "收件人电话长度不能超过20".FormatWith(model.ShippingPhone));
                    return View(model);
                }
                //发件人省份
                if (model.SenderState != null && model.SenderState.Length > 20)
                {
                    InitAdd();
                    ModelState.AddModelError("SenderState", "发件人州省长度不能超过20".FormatWith(model.SenderState));
                    return View(model);
                }
                //发件人城市
                if (model.SenderCity !=null && model.SenderCity.Length>64)
                {
                    InitAdd();
                    ModelState.AddModelError("SenderCity", "发件人城市长度不能超过64".FormatWith(model.SenderCity));
                    return View(model);
                }
                //发件人街道
                if (model.SenderAddress != null && model.SenderAddress.Length > 120)
                {
                    InitAdd();
                    ModelState.AddModelError("SenderAddress", "发件人地址长度不能超过120".FormatWith(model.SenderAddress));
                    return View(model);
                }
                //发件人邮编
                if (model.SenderZip != null && model.SenderZip.Length > 6)
                {
                    InitAdd();
                    ModelState.AddModelError("SenderZip", "发件人邮编长度不能超过6".FormatWith(model.SenderZip));
                    return View(model);
                }
                //发件人名字
                string senderName = "";
                if (model.SenderFirstName != null)
                {
                    senderName += model.SenderFirstName;
                }
                if (model.SenderLastName != null)
                {
                    senderName += model.SenderLastName;
                }
                if (senderName.Length > 20)
                {
                    InitAdd();
                    ModelState.AddModelError("SenderFirstName", "发件人名字长度不能超过20".FormatWith(model.SenderFirstName));
                    return View(model);
                }
                //发件人电话
                if (model.SenderPhone != null && model.SenderPhone.Length > 20)
                {
                    InitAdd();
                    ModelState.AddModelError("SenderPhone", "发件人电话长度不能超过20".FormatWith(model.SenderPhone));
                    return View(model);
                }
            }

            #endregion

            #region DHL 上传验证

            if (shippingMethod != null && (shippingMethod.Code.Trim().ToUpperInvariant() == "HKDHL" || shippingMethod.Code.Trim().ToUpperInvariant() == "DHLCN" || shippingMethod.Code.Trim().ToUpperInvariant() == "DHLSG"))
            {
                if (model.InsureAmount != null && model.InsureAmount.ToString().Length > 14)
                {
                    InitAdd();
                    ModelState.AddModelError("InsureAmount", "保险金额长度不能超过14个字符".FormatWith(model.InsureAmount));
                    return View(model);
                }
                else if (model.InsureAmount != null && !Regex.IsMatch(model.InsureAmount.ToString(), "^[0-9]+[.]{0,1}[0-9]{0,2}$"))
                {
                    InitAdd();
                    ModelState.AddModelError("InsureAmount", "保险金额有数字组成，最多保留两位小数".FormatWith(model.InsureAmount));
                    return View(model);
                }

                if (model.ShippingCompany !=null && model.ShippingCompany.Length > 35)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingCompany", "收件人公司长度为0-35个字符".FormatWith(model.ShippingCompany));
                    return View(model);
                }
                string address = "";
                if (model.ShippingAddress != null)
                {
                    address += model.ShippingAddress;
                }
                if (model.ShippingAddress1 != null)
                {
                    address += model.ShippingAddress1;
                }
                if (model.ShippingAddress2 != null)
                {
                    address += model.ShippingAddress2;
                }
                try
                {
                    if (string.IsNullOrWhiteSpace(address) || address.Length > 70 || address.StringSplitLengthWords(35).Count > 2)
                    {
                        InitAdd();
                        ModelState.AddModelError("ShippingAddress", "收件人地址不能为空或者超长".FormatWith(model.ShippingAddress));
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                    InitAdd();
                    ModelState.AddModelError("ShippingAddress", "收件人地址格式错误".FormatWith(model.ShippingAddress));
                    return View(model);
                }
                
                if (string.IsNullOrWhiteSpace(model.ShippingCity) || model.ShippingCity.Length > 35)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingCity", "收件人城市长度为1-35个字符".FormatWith(model.ShippingCity));
                    return View(model);
                }

                if (model.ShippingState != null && model.ShippingState.Length > 35)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingState", "收件人州/省长度为1-35个字符".FormatWith(model.ShippingState));
                    return View(model);
                }
                if (string.IsNullOrWhiteSpace(model.ShippingZip) || model.ShippingZip.Length > 12)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingZip", "收件人邮编长度为1-12个字符".FormatWith(model.ShippingZip));
                    return View(model);
                }

                if (!string.IsNullOrWhiteSpace(model.ShippingTaxId) &&
                    model.ShippingTaxId.Length > 20)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingTaxId", "收件人税号不能超过20字符".FormatWith(model.ShippingTaxId));
                    return View(model);
                }
                if ((model.ShippingFirstName + model.ShippingLastName).Length > 35)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingFirstName", "收件人姓名不能超过35个字符".FormatWith(model.ShippingFirstName));
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.ShippingPhone))
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingPhone", "收件人电话不能为空".FormatWith(model.ShippingPhone));
                    return View(model);
                }
                else if (model.ShippingPhone.Length > 25)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingPhone", "收件人电话不能超过25个字符".FormatWith(model.ShippingPhone));
                    return View(model);
                }
            }

            #endregion

            #region EUB 上传验证

            if (shippingMethod != null &&
                    (shippingMethod.Code.Trim().ToUpperInvariant() == "EUB_CS" || shippingMethod.Code.Trim().ToUpperInvariant() == "EUB-SZ" ||
                     shippingMethod.Code.Trim().ToUpperInvariant() == "EUB-FZ"))
            {
                if (model.CustomerOrderNumber != null && (model.CustomerOrderNumber.Length > 32 || model.CustomerOrderNumber.Length < 4))
                {
                    InitAdd();
                    ModelState.AddModelError("CustomerOrderNumber", "订单号长度必须为4-32个字符".FormatWith(model.CustomerOrderNumber));
                    return View(model);
                }
                if (
                    (model.ShippingFirstName + model.ShippingLastName)
                        .Length > 256)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingFirstName", "收件人姓名不能超过256个字符".FormatWith(model.ShippingFirstName));
                    return View(model);
                }
                if (model.ShippingCity != null &&
                    model.ShippingCity.Length > 128)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingCity", "收件人城市不能超过128个字符".FormatWith(model.ShippingCity));
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.ShippingState) ||
                    model.ShippingState.Length > 128)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingState", "收件人州/省长度为1-128个字符".FormatWith(model.ShippingState));
                    return View(model);
                }

                if (model.ShippingZip != null)
                {
                    if (model.ShippingZip.Length > 16)
                    {
                        InitAdd();
                        ModelState.AddModelError("ShippingZip", "收件人邮编不能超过16个字符".FormatWith(model.ShippingZip));
                        return View(model);
                    }
                    else
                    {
                        switch (model.CountryCode.ToUpperInvariant())
                        {
                            case "US":
                                if (!Regex.IsMatch(model.ShippingZip, "^(^[0-9]{5}-[0-9]{4}$)|(^[0-9]{5}-[0-9]{5}$)|(^[0-9]{5}$)$"))
                                {
                                    InitAdd();
                                    ModelState.AddModelError("ShippingZip", "邮编不合法".FormatWith(model.ShippingZip));
                                    return View(model);
                                }
                                break;
                            case "AU":
                                if (!Regex.IsMatch(model.ShippingZip, "^[0-9]{4}$"))
                                {
                                    InitAdd();
                                    ModelState.AddModelError("ShippingZip", "邮编不合法".FormatWith(model.ShippingZip));
                                    return View(model);
                                }
                                break;
                            case "CA":
                                if (!Regex.IsMatch(model.ShippingZip, "^(^[A-Za-z][0-9][A-Za-z][ ][0-9][A-Za-z][0-9]$)|(^[A-Za-z][0-9][A-Za-z][0-9][A-Za-z][0-9]$)$"))
                                {
                                    InitAdd();
                                    ModelState.AddModelError("ShippingZip", "邮编不合法".FormatWith(model.ShippingZip));
                                    return View(model);
                                }
                                break;
                            case "GB":
                                if (!Regex.IsMatch(model.ShippingZip, "^[A-Za-z0-9]{2,4} [A-Za-z0-9]{3}$"))
                                {
                                    InitAdd();
                                    ModelState.AddModelError("ShippingZip", "邮编不合法".FormatWith(model.ShippingZip));
                                    return View(model);
                                }
                                break;
                            case "FR":
                                if (!Regex.IsMatch(model.ShippingZip, "^[0-9]{5}$"))
                                {
                                    InitAdd();
                                    ModelState.AddModelError("ShippingZip", "邮编不合法".FormatWith(model.ShippingZip));
                                    return View(model);
                                }
                                break;
                            case "RU":
                                if (!Regex.IsMatch(model.ShippingZip, "^[0-9]{6}$"))
                                {
                                    InitAdd();
                                    ModelState.AddModelError("ShippingZip", "邮编不合法".FormatWith(model.ShippingZip));
                                    return View(model);
                                }
                                break;
                        }
                    }
                }
                else if (model.CountryCode != "HK")
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingZip", "邮编不能为空".FormatWith(model.ShippingZip));
                    return View(model);
                }

            }

            #endregion

            //Add By zhengsong
            //是否是需要计算偏远附加费 ，需要验证省/州，城市，邮编
            if (shippingMethod !=null && shippingMethod.FuelRelateRAF)
            {
                if (string.IsNullOrWhiteSpace(model.ShippingZip))
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingZip", "邮编不能为空".FormatWith(model.ShippingZip));
                    return View(model);
                }
                if (string.IsNullOrWhiteSpace(model.ShippingState))
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingState", "州/省不能为空".FormatWith(model.ShippingState));
                    return View(model);
                }
                if (string.IsNullOrWhiteSpace(model.ShippingCity))
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingCity", "城市不能为空".FormatWith(model.ShippingCity));
                    return View(model);
                }
            }

            if (model.ShippingMethodId == sysConfig.SpecialShippingMethodId)
            {
                if (!string.IsNullOrWhiteSpace(model.ShippingZip))
                {
                    if (!Tools.CheckPostCode(model.ShippingZip))
                    {
                        InitAdd();
                        ModelState.AddModelError("ShippingZip", "邮编为6位纯数字且只能以1 2 3 4 6开头".FormatWith(model.ShippingZip));
                        return View(model);
                    }
                }
                else
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingZip", "邮编不能为空".FormatWith(model.ShippingZip));
                    return View(model);
                }
                if (!string.IsNullOrWhiteSpace(model.ShippingPhone))
                {
                    if (!Tools.CheckShippingPhone(model.ShippingPhone))
                    {
                        InitAdd();
                        ModelState.AddModelError("ShippingPhone", "电话号码最长不能超过11位数字".FormatWith(model.ShippingPhone));
                        return View(model);
                    }
                }
                else
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingPhone", "联系电话不能为空".FormatWith(model.ShippingPhone));
                    return View(model);
                }
            }
            int packageNumber = 1;
            if (string.IsNullOrWhiteSpace(model.PackageNumberValue))
            {
                model.PackageNumber = 1;
            }
            if (int.TryParse(model.PackageNumberValue, out packageNumber))
            {
                if (packageNumber < 0)
                {
                    InitAdd();
                    ModelState.AddModelError("PackageNumberValue", "件数必须大于零".FormatWith(model.PackageNumberValue));
                    return View(model);
                }
                else
                {
                    if (packageNumber == 0)
                    {
                        packageNumber = 1;
                    }
                    model.PackageNumber = packageNumber;
                }
            }
            else
            {
                InitAdd();
                ModelState.AddModelError("PackageNumberValue", "件数必须为数字".FormatWith(model.PackageNumberValue));
                return View(model);
            }
            if (model.IsInsured)
            {
                string[] insuredValue = model.InsuredValue.Split('_');
                if (insuredValue.Length > 0)
                {
                    model.InsuredID = int.Parse(insuredValue[0]);
                }
                if (model.InsuredID == 2)
                {
                    decimal insureAmountvalue;
                    if (string.IsNullOrWhiteSpace(model.InsureAmountValue))
                    {
                        InitAdd();
                        ModelState.AddModelError("InsureAmountValue", "保险价值不能为空".FormatWith(model.InsureAmountValue));
                        return View(model);
                    }
                    if (decimal.TryParse(model.InsureAmountValue, out insureAmountvalue))
                    {
                        if (insureAmountvalue <= 0)
                        {
                            InitAdd();
                            ModelState.AddModelError("InsureAmountValue",
                                                     "保险价值必须大于零".FormatWith(model.InsureAmountValue));
                            return View(model);
                        }
                        else
                        {
                            model.InsureAmount = insureAmountvalue;
                        }
                    }
                    else
                    {
                        InitAdd();
                        ModelState.AddModelError("InsureAmountValue", "保险价值必须为数字".FormatWith(model.InsureAmountValue));
                        return View(model);
                    }
                }
                else
                {
                    model.InsureAmount = decimal.Parse(insureList.Find(p => p.InsuredID == model.InsuredID).InsuredCalculation1);
                }
            }

            if (null == model.ProductDetail)
            {
                InitAdd();
                ErrorNotification("添加失败，申报信息不能为空");
                return View(model);
            }

            model.AppLicationType = int.Parse(model.AppLicationTypeId);
            var info = new CustomerOrderInfo { ShippingInfo = new ShippingInfo(), SenderInfo = new SenderInfo() };
            model.ToEntity(info);
            model.ToEntity(info.ShippingInfo);
            model.ToEntity(info.SenderInfo);
            info.SenderInfo.CountryCode = "CN";
            var js = new JavaScriptSerializer();
            var receiver = js.Deserialize<List<ApplicationInfoModel>>(model.ProductDetail);
            //统一将单号转成大写插入数据库--Add by zhengsong
            info.CustomerOrderNumber = info.CustomerOrderNumber.Trim().ToUpperInvariant();
            if (info.TrackingNumber != null)
            {
                info.TrackingNumber = info.TrackingNumber.Trim().ToUpperInvariant();
            }
            info.CreatedOn = DateTime.Now;
            info.LastUpdatedOn = DateTime.Now;
            info.CustomerCode = _workContext.User.UserUame.ToUpperInvariant();
            info.CreatedBy = info.CustomerCode;
            info.LastUpdatedBy = info.CreatedBy;
            info.ShippingMethodName = GetShipingMethods().First(p => p.Value == info.ShippingMethodId.ToString()).Text;
            if (!info.IsBattery) info.SensitiveTypeID = null;
            if (!info.IsInsured) info.InsuredID = null;
            if (receiver != null)
            {
                if (shippingMethod != null && sysConfig.NLPOSTMethodCode.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Contains(shippingMethod.Code))
                {
                    //荷兰小包申报信息验证
                    var applicationInfoModelValidator = new ApplicationInfoModelValidator();
                    var strsb = new StringBuilder();
                    var n = 1;
                    foreach (var applicationInfoModel in receiver)
                    {
                        var appinfoValidatorResult = applicationInfoModelValidator.Validate(applicationInfoModel);
                        if (!appinfoValidatorResult.IsValid)
                        {
                            foreach (var err in appinfoValidatorResult.Errors)
                            {
                                strsb.AppendLine("第" + n + "行"+err.ErrorMessage);
                            }
                        }
                        n++;
                    }
                    if (strsb.Length > 0)
                    {
                        InitAdd();
                        ErrorNotification("添加失败，" + strsb.ToString());
                        return View(model);
                    }
                }else if (shippingMethod != null &&
                          sysConfig.LithuaniaMethodCode.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries)
                                   .ToList()
                                   .Contains(shippingMethod.Code))
                {
                    // 俄罗斯挂号、平邮申报信息验证
                    var applicationInfoModelValidator = new ApplicationSfModelValidator();
                    var strsb = new StringBuilder();
                    var n = 1;
                    foreach (var applicationInfoModel in receiver)
                    {
                        var appinfoValidatorResult = applicationInfoModelValidator.Validate(new ApplicationSfModel() { ApplicationName = applicationInfoModel.ApplicationName,Qty = applicationInfoModel.Qty,UnitPrice = applicationInfoModel.UnitPrice,UnitWeight = applicationInfoModel.UnitWeight});
                        if (!appinfoValidatorResult.IsValid)
                        {
                            foreach (var err in appinfoValidatorResult.Errors)
                            {
                                strsb.AppendLine("第" + n + "行" + err.ErrorMessage);
                            }
                        }
                        n++;
                    }
                    if (strsb.Length > 0)
                    {
                        InitAdd();
                        ErrorNotification("添加失败，" + strsb.ToString());
                        return View(model);
                    }
                }
                var list = receiver.ToEntityAsCollection<ApplicationInfoModel, ApplicationInfo>();
                list.ForEach(item =>
                    {
                        item.CreatedBy = info.CreatedBy.ToUpperInvariant();
                        item.CreatedOn = info.CreatedOn;
                        item.LastUpdatedBy = info.CreatedBy;
                        item.LastUpdatedOn = info.CreatedOn;
                        item.LastUpdatedOn = info.CreatedOn;
                        info.ApplicationInfos.Add(item);
                    });
            }

            //验证申报信息
            if (info.ApplicationInfos.Count > 0)
            {
                var n = 1;
                string error = "";
                foreach (var row in info.ApplicationInfos)
                {
                    if (row.UnitWeight == null || row.UnitWeight < 0)
                    {
                        error += "第" + n + "列申报重量不能为0";
                    }
                    n++;
                }
                if (!string.IsNullOrWhiteSpace(error))
                {
                    InitAdd();
                    ErrorNotification("添加失败，" + error);
                    return View(model);
                }
            }

            //EUB 验证
            if (shippingMethod != null &&
                (shippingMethod.Code.Trim().ToUpperInvariant() == "EUB_CS" ||
                 shippingMethod.Code.Trim().ToUpperInvariant() == "EUB-SZ" ||
                 shippingMethod.Code.Trim().ToUpperInvariant() == "EUB-FZ"))
            {
                if (info.ApplicationInfos.Count > 0)
                {
                    var n = 1;
                    string error = "";

                    foreach (var row in info.ApplicationInfos)
                    {
                        if (string.IsNullOrWhiteSpace(row.ApplicationName) || row.ApplicationName.Length > 128)
                        {
                            error += "第" + n + "行申报名称不能为空或超过128个字符";
                        }

                        if (string.IsNullOrWhiteSpace(row.PickingName) || row.PickingName.Length > 64)
                        {
                            error += "第" + n + "行申报中文名称不能为空或超过字符长度";
                        }
                        else
                        {
                            if (!Regex.IsMatch(row.PickingName, @"[\u4e00-\u9fa5]+[A-Za-z0-9]*[\s\S]*[\u4e00-\u9fa5]+"))
                            {
                                error += "第" + n + "列申报中文名称必须包含两个中文字符";
                            }
                        }
                        n++;
                    }
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        InitAdd();
                        ErrorNotification("添加失败，" + error);
                        return View(model);
                    }
                }
            }

            //DHL验证申报信息
            if (shippingMethod != null &&
                (shippingMethod.Code.Trim().ToUpperInvariant() == "HKDHL" ||
                 shippingMethod.Code.Trim().ToUpperInvariant() == "DHLCN" ||
                 shippingMethod.Code.Trim().ToUpperInvariant() == "DHLSG"))
            {
                if (info.ApplicationInfos.Count > 0)
                {
                    var n = 1;
                    string error = "";

                    foreach (var row in info.ApplicationInfos)
                    {
                        if (string.IsNullOrWhiteSpace(row.ApplicationName) || row.ApplicationName.Length > 60)
                        {
                            error += "第" + n + "行申报名称不能为空或超过字符长度";
                        }
                        else
                        {
                            if (Regex.IsMatch(row.ApplicationName,
                                               @"[\~]{1}|[\@]{1}|[\#]{1}|[\$]{1}|[\￥]{1}|[\%]{1}|[\^]{1}|[\&]{1}|[\*]{1}|[\(]{1}|[\)]{1}|[\u4e00-\u9fa5]+"))
                            {
                                error += "第" + n + "列申报英文名称不能包含特殊字符和汉字";
                            }
                        }
                        n++;
                    }
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        InitAdd();
                        ErrorNotification("添加失败，" + error);
                        return View(model);
                    }
                }
            }

            //福州邮政申报信息判断
            if (shippingMethod != null && (shippingMethod.Code.Trim().ToUpperInvariant() == "CNPOST-FZ" || shippingMethod.Code.Trim().ToUpperInvariant() == "CNPOSTP_FZ" || shippingMethod.Code.Trim().ToUpperInvariant() == "CNPOST-FYB"))
            {
                if (info.ApplicationInfos.Count > 0)
                {
                    var n = 1;
                    string error = "";

                    foreach (var row in info.ApplicationInfos)
                    {
                        if (string.IsNullOrWhiteSpace(row.ApplicationName) || row.ApplicationName.Length > 60)
                        {
                            error += "第" + n + "行申报名称不能为空或超过字符长度";
                        }

                        if (string.IsNullOrWhiteSpace(row.PickingName) || row.PickingName.Length>60)
                        {
                            error += "第" + n + "行申报中文名称不能为空或超过字符长度";
                        }
                        n++;
                    }
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        InitAdd();
                        ErrorNotification("添加失败，" + error);
                        return View(model);
                    }
                }
            }

            //@"^[\~]{1}|[\@]{1}|[\#]{1}|[\$]{1}|[\￥]{1}|[\%]{1}|[\^]{1}|[\&]{1}|[\*]{1}|[\(]{1}|[\)]{1}|[\u4e00-\u9fa5]+$"

            //欧洲专线 需要判断 申报信息 欧洲专线上传 限制 Add By zhengsong
            if (shippingMethod != null && (shippingMethod.Code == sysConfig.DDPRegisterShippingMethodCode || shippingMethod.Code == sysConfig.DDPShippingMethodCode || sysConfig.EuropeShippingMethodCode == shippingMethod.Code))
            {
                // OrderNumber 上传系统时限制只能是数字或字母，不能有其他符合  ，比如 - （ ）*，字符数量小于25
                    //Regex r = new Regex(@"^[A-Za-z0-9]{0,25}$");
                    //MatchCollection customerOrderNumber = r.Matches(model.CustomerOrderNumber);
                    //if (customerOrderNumber.Count < 1 || customerOrderNumber[0].Value != model.CustomerOrderNumber)
                    //{
                    //    InitAdd();
                    //    ModelState.AddModelError("CustomerOrderNumber", "格式不符合要求".FormatWith(model.CustomerOrderNumber));
                    //    return View(model);
                    //}
                if (model.CustomerOrderNumber.Length > 25)
                {
                    InitAdd();
                    ModelState.AddModelError("CustomerOrderNumber", "不能超过25个字符".FormatWith(model.CustomerOrderNumber));
                    return View(model);
                }
                   
                    ////PhoneNumber   只能是数字，不能出现其他字符，比如：&#43; &amp;
                    //Regex c = new Regex(@"^[0-9]{0,}$");
                    //MatchCollection shippingPhone = c.Matches(model.ShippingPhone);
                    //if (shippingPhone.Count < 1 || shippingPhone[0].Value != model.ShippingPhone)
                    //{
                    //    InitAdd();
                    //    ModelState.AddModelError("ShippingPhone", "格式不符合要求".FormatWith(model.ShippingPhone));
                    //    return View(model);
                    //}
                
                if (info.ApplicationInfos.Count > 0)
                {
                    var n = 1;
                    string error = "";
                    
                    foreach (var row in info.ApplicationInfos)
                    {
                        if (row.HSCode == null || string.IsNullOrWhiteSpace(row.HSCode))
                        {
                            error += "第" + n + "行申报信息海关编码不能为空";
                        }
                        if (row.ProductUrl == null || string.IsNullOrWhiteSpace(row.ProductUrl))
                        {
                            error += "第" + n + "行申报信息销售链接不能为空";
                        }
                        if (row.Remark == null || string.IsNullOrWhiteSpace(row.Remark))
                        {
                            error += "第" + n + "行申报信息备注不能为空";
                        }
                        //else
                        //{
                        //    //SKUCode1  上传系统时限制只能是数字或字母，不能有其他符合  ，比如 - （ ）*，字符数量小于30
                        //    Regex z = new Regex(@"^[A-Za-z0-9]{0,30}$");
                        //    MatchCollection remark = z.Matches(row.Remark);
                        //    if (remark.Count < 1 || remark[0].Value != row.Remark)
                        //    {
                        //        error += "第" + n + "行申报信息备注格式不符合要求";
                        //    }
                        //}
                        n++;
                    }
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        InitAdd();
                        ErrorNotification("添加失败，" + error);
                        return View(model);
                    }
                }
            }

            try
            {
                _customerOrderService.Add(info);
                SuccessNotification("添加成功");
            }
            catch (Exception e)
            {
                Log.Exception(e);
                ErrorNotification("添加失败，原因为：" + e.Message);
                InitAdd();
                return View(model);
            }

            return RedirectToAction("Add");
        }



        public ActionResult BatchAdd()
        {
            ViewBag.GoodsTypeList = GetGoodsTypeList();
            var viewModels = new BatchAddViewModels();
            viewModels.FilePathDate = new FileInfo(sysConfig.ExcelTemplatePath).LastWriteTime.ToShortDateString();
            return View(viewModels);
        }

        /// <summary>
        /// 小包页面，add by huhaiyou 2014-4-17
        /// </summary>
        /// <returns></returns>
        public ActionResult BatchAddPackage()
        {
            ViewBag.GoodsTypeList = GetGoodsTypeList();
            var viewModels = new BatchAddViewModels();
            viewModels.FilePathDate = new FileInfo(sysConfig.ExcelTemplatePath).LastWriteTime.ToShortDateString();
            return View(viewModels);
        }

        //[HttpPost]
        // public ActionResult BatchAdd(List<OrderModel> list)
        // {
        //     ViewBag.GoodsTypeList = GetGoodsTypeList();
        //     var viewModels = new BatchAddViewModels();
        //     viewModels.OrderList = list;
        //     return View(viewModels);
        // }


        [HttpPost]
        [FormValueRequired("Upload")]
        public ActionResult BatchAdd(HttpPostedFileBase file, int goodsTypeId, List<OrderModel> lists = null)
        {
            try
            {
                string strFilePath = SaveFile(file);

                List<OrderModel> listResult = new List<OrderModel>();
                if (lists == null)
                {
                    var list = GetOrderList(strFilePath, goodsTypeId);
                    listResult = ValidationOrderModel(list);
                }
                else
                {
                    listResult = ValidationOrderModel(lists);
                }
                var freightService = EngineContext.Current.Resolve<IFreightService>();

                var shippingMethodeList = freightService.GetShippingMethods(null, true);

                ViewBag.GoodsTypeList = GetGoodsTypeList();

                var viewModels = new BatchAddViewModels
                    {
                        GoodsTypeID = goodsTypeId,
                        OrderList = listResult.OrderByDescending(p => p.ErrorType).ThenByDescending(p => p.ErrorMessage.ToString()).ThenBy(p => p.ExeclRow).ToList(),
                        FilePath = strFilePath,
                        CountryModels = GetCountryList(""),
                        FilePathDate = new FileInfo(sysConfig.ExcelTemplatePath).LastWriteTime.ToShortDateString()
                    };
                shippingMethodeList.ForEach(p => viewModels.ShippingMethodModels.Add(new SelectListItem()
                    {
                        Text = p.FullName,
                        Value = p.Code
                    }));
                //var countryList = freightService.GetCountrys();
                //countryList.ForEach(p=> viewModels.CountryModels.Add(new SelectListItem()
                //    {
                //        Text = p.CountryCode,
                //        Value = p.CountryCode
                //    }));
                return View(viewModels);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                ErrorNotification("上传失败，原因为：" + e.Message);
            }


            return RedirectToAction("BatchAdd");

        }

        /// <summary>
        /// 上传包裹数据
        /// </summary>
        /// <param name="file"></param>
        /// <param name="goodsTypeId"></param>
        /// <param name="lists"></param>
        /// <returns></returns>
        [HttpPost]
        [FormValueRequired("UploadPackage")]
        public ActionResult BatchAddPackage(HttpPostedFileBase file, int goodsTypeId, List<OrderModel> lists = null)
        {
            try
            {
                string strFilePath = SaveFile(file);

                List<OrderModel> listResult = new List<OrderModel>();
                if (lists == null)
                {
                    var list = GetOrderPackageList(strFilePath, goodsTypeId);
                    listResult = ValidationOrderPackageModel(list);
                }
                else
                {
                    listResult = ValidationOrderPackageModel(lists);
                }
                var freightService = EngineContext.Current.Resolve<IFreightService>();

                var shippingMethodeList = freightService.GetShippingMethods(null, true);

                ViewBag.GoodsTypeList = GetGoodsTypeList();

                var viewModels = new BatchAddViewModels
                {
                    GoodsTypeID = goodsTypeId,
                    OrderList = listResult.OrderByDescending(p => p.ErrorType).ThenByDescending(p => p.ErrorMessage.ToString()).ThenBy(p => p.ExeclRow).ToList(),
                    FilePath = strFilePath,
                    CountryModels = GetCountryList(""),
                    FilePathDate = new FileInfo(sysConfig.ExcelTemplatePath).LastWriteTime.ToShortDateString()
                };
                shippingMethodeList.ForEach(p => viewModels.ShippingMethodModels.Add(new SelectListItem()
                {
                    Text = p.FullName,
                    Value = p.Code
                }));

                //var countryList = freightService.GetCountrys();
                //countryList.ForEach(p=> viewModels.CountryModels.Add(new SelectListItem()
                //    {
                //        Text = p.CountryCode,
                //        Value = p.CountryCode
                //    }));
                return View(viewModels);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                ErrorNotification("上传失败，原因为：" + e.Message);
            }


            return RedirectToAction("BatchAddPackage");

        }

        [HttpPost]
        [FormValueRequired("Save")]
        public ActionResult BatchAdd(string filePath, int goodsTypeId)
        {
            try
            {
                List<OrderModel> list = new List<OrderModel>();
                if (null != TempData["orderList"])
                {
                    list = TempData["orderList"] as List<OrderModel>;
                    TempData.Remove("orderList");
                }
                else
                {
                    list = GetOrderList(filePath, goodsTypeId);
                    list = ValidationOrderModel(list);
                }
                if (list.Any(o => !o.IsValid))
                {
                    ErrorNotification("保存失败，信息验证有误，请重新上传。");
                    return RedirectToAction("BatchAdd");
                }
                var listInfo = new List<CustomerOrderInfo>();

                string customerCode = _workContext.User.UserUame.ToUpperInvariant();

                var shippingMethods = _freightService.GetShippingMethods(null, true);
                foreach (OrderModel model in list)
                {
                    var entity = model.ToEntity<CustomerOrderInfo>();

                    entity.ShippingInfo = new ShippingInfo();
                    entity.SenderInfo = new SenderInfo();
                    model.ToEntity(entity.ShippingInfo);
                    model.ToEntity(entity.SenderInfo);
                    entity.Weight = model.Weight ?? 0;
                    entity.Length = model.Length ?? 1;
                    entity.Width = model.Width ?? 1;
                    entity.Height = model.Height ?? 1;

                    if (entity.PackageNumber == null || entity.PackageNumber <= 0)
                    {
                        entity.PackageNumber = 1;
                    }

                    entity.SenderInfo.CountryCode = "CN";
                    string code = model.ShippingMethodCode.ToUpperInvariant();
                    var shipingMethod = shippingMethods.Single(s => s.Code.ToUpperInvariant() == code);
                    entity.ShippingMethodId = shipingMethod.ShippingMethodId;
                    entity.ShippingMethodName = shipingMethod.FullName;
                    //在写入数据库时，将所有的单号都转换成大写——Add by zhengsong
                    entity.CustomerOrderNumber = entity.CustomerOrderNumber.ToUpperInvariant();
                    if (entity.TrackingNumber != null)
                    {
                        entity.TrackingNumber = entity.TrackingNumber.ToUpperInvariant();
                    }
                    entity.ShippingInfo.CountryCode = entity.ShippingInfo.CountryCode.ToUpperInvariant();
                    /*entity.InsuredID = entity.IsInsured ? entity.InsuredID : null;
                    entity.SensitiveTypeID = entity.IsBattery ? entity.SensitiveTypeID : null;*/
                    entity.CustomerCode = customerCode;
                    entity.EnableTariffPrepay = model.EnableTariffPrepayString == "Y";
                    listInfo.Add(entity);


                    //#region 操作日志
                    ////yungchu
                    ////敏感字-无
                    //var bizlog = new BizLog()
                    //{
                    //	Summary = "批量创建订单",
                    //	KeywordType = KeywordType.CustomerOrderNumber,
                    //	Keyword = model.CustomerOrderNumber,
                    //	UserCode = _workContext.User.UserUame,
                    //	UserRealName = _customerService.GetCustomer(_workContext.User.UserUame).Name ?? _workContext.User.UserUame,
                    //	UserType = UserType.LMS_User,
                    //	SystemCode = SystemType.LMS,
                    //	ModuleName = "批量创建订单"
                    //};


                    //CustomerOrderInfoExportExt customerOrderInfoExportExt = new CustomerOrderInfoExportExt();
                    //model.CopyTo(customerOrderInfoExportExt);

                    //_operateLogServices.WriteLog(bizlog, customerOrderInfoExportExt);
                    //#endregion

                }
                _customerOrderService.BatchAdd(listInfo);
                SuccessNotification("保存成功");
            }
            catch (Exception e)
            {
                Log.Exception(e);
                ErrorNotification("保存失败，原因为：" + e.Message);
            }

            return RedirectToAction("BatchAdd");
        }

        /// <summary>
        /// 保存小包上传资料
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="goodsTypeId"></param>
        /// <returns></returns>
        [HttpPost]
        [FormValueRequired("SavePackage")]
        public ActionResult BatchAddPackage(string filePath, int goodsTypeId)
        {
            try
            {
                var viewModel = new BatchAddViewModels();
                viewModel.FilePathDate = new FileInfo(sysConfig.ExcelTemplatePath).LastWriteTime.ToShortDateString();
                List<OrderModel> list = new List<OrderModel>();
                if (null != TempData["orderList"])
                {
                    list = TempData["orderList"] as List<OrderModel>;
                    TempData.Remove("orderList");
                }
                else
                {
                    list = GetOrderPackageList(filePath, goodsTypeId);
                    //list = ValidationOrderModel(list);
                }
                if (list.Any(o => !o.IsValid))
                {
                    ErrorNotification("保存失败，信息验证有误，请重新上传。");
                    return RedirectToAction("BatchAddPackage");
                }
                var listInfo = new List<CustomerOrderInfo>();

                string customerCode = _workContext.User.UserUame.ToUpperInvariant();

                var shippingMethods = _freightService.GetShippingMethods(null, true);
                foreach (OrderModel model in list)
                {
                    var entity = model.ToEntity<CustomerOrderInfo>();
                    entity.ShippingInfo = new ShippingInfo();
                    entity.SenderInfo = new SenderInfo();
                    model.ToEntity(entity.ShippingInfo);
                    model.ToEntity(entity.SenderInfo);
                    entity.SenderInfo.CountryCode = "CN";
                    string code = model.ShippingMethodCode.ToUpperInvariant();
                    var shipingMethod = shippingMethods.Single(s => s.Code.ToUpperInvariant() == code);
                    entity.ShippingMethodId = shipingMethod.ShippingMethodId;
                    entity.ShippingMethodName = shipingMethod.FullName;
                    entity.Weight = model.Weight ?? 0;
                    entity.Width = 1;
                    entity.Height = 1;
                    entity.Length = 1;

                    if (entity.PackageNumber == null || entity.PackageNumber <= 0)
                    {
                        entity.PackageNumber = 1;
                    }

                    //在写入数据库时，将所有的单号都转换成大写——Add by zhengsong
                    entity.CustomerOrderNumber = entity.CustomerOrderNumber.ToUpperInvariant();
                    if (entity.TrackingNumber != null)
                    {
                        entity.TrackingNumber = entity.TrackingNumber.ToUpperInvariant();
                    }
                    entity.ShippingInfo.CountryCode = entity.ShippingInfo.CountryCode.ToUpperInvariant();
                    /*entity.InsuredID = entity.IsInsured ? entity.InsuredID : null;
                    entity.SensitiveTypeID = entity.IsBattery ? entity.SensitiveTypeID : null;*/
                    entity.CustomerCode = customerCode;
                    listInfo.Add(entity);

                    //#region 操作日志
                    ////yungchu
                    ////敏感字-无
                    //var bizlog = new BizLog()
                    //{
                    //	Summary = "小包批量创建订单",
                    //	KeywordType = KeywordType.CustomerOrderNumber,
                    //	Keyword = model.CustomerOrderNumber,
                    //	UserCode = _workContext.User.UserUame,
                    //	UserRealName = _customerService.GetCustomer(_workContext.User.UserUame).Name ?? _workContext.User.UserUame,
                    //	UserType = UserType.LMS_User,
                    //	SystemCode = SystemType.LMS,
                    //	ModuleName = "小包批量创建订单"
                    //};
                    //CustomerOrderInfoExportExt customerOrderInfoExportExt = new CustomerOrderInfoExportExt();
                    //model.CopyTo(customerOrderInfoExportExt);
                    //_operateLogServices.WriteLog(bizlog, customerOrderInfoExportExt); 
                    //#endregion


                }
                _customerOrderService.BatchAdd(listInfo);
                SuccessNotification("保存成功");

            }
            catch (Exception e)
            {
                Log.Exception(e);
                ErrorNotification("保存失败!");
            }

            return RedirectToAction("BatchAddPackage");
        }

        #region 中美专线订单上传

        public ActionResult BatchAddSinoUS()
        {
            ViewBag.GoodsTypeList = GetGoodsTypeList();
            var viewModels = new BatchAddViewModels();
            viewModels.FilePathDate = new FileInfo(sysConfig.ExcelTemplatePath).LastWriteTime.ToShortDateString();
            return View(viewModels);
        }

        [HttpPost]
        [FormValueRequired("UploadSinoUS")]
        public ActionResult BatchAddSinoUS(HttpPostedFileBase file, int goodsTypeId, List<OrderModel> lists = null)
        {
            try
            {
                string strFilePath = SaveFile(file);

                List<OrderModel> listResult = new List<OrderModel>();
                if (lists == null)
                {
                    var list = GetOrderSinoUSList(strFilePath, goodsTypeId);
                    listResult = ValidationOrderSinoUSModel(list);
                }
                else
                {
                    listResult = ValidationOrderSinoUSModel(lists);
                }
                var freightService = EngineContext.Current.Resolve<IFreightService>();

                var shippingMethodeList = freightService.GetShippingMethods(null, true);

                ViewBag.GoodsTypeList = GetGoodsTypeList();

                var viewModels = new BatchAddViewModels
                {
                    GoodsTypeID = goodsTypeId,
                    OrderList = listResult.OrderByDescending(p => p.ErrorType).ThenByDescending(p => p.ErrorMessage.ToString()).ThenBy(p => p.ExeclRow).ToList(),
                    FilePath = strFilePath,
                    CountryModels = GetCountryList(""),
                    FilePathDate = new FileInfo(sysConfig.ExcelTemplatePath).LastWriteTime.ToShortDateString()
                };
                shippingMethodeList.ForEach(p => viewModels.ShippingMethodModels.Add(new SelectListItem()
                {
                    Text = p.FullName,
                    Value = p.Code
                }));
                return View(viewModels);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                ErrorNotification("上传失败，原因为：" + e.Message);
            }
            return RedirectToAction("BatchAddSinoUS");
        }

        [HttpPost]
        [FormValueRequired("SaveSinoUS")]
        public ActionResult BatchAddSinoUS(string filePath, int goodsTypeId)
        {
            try
            {
                List<OrderModel> list = new List<OrderModel>();
                if (null != TempData["orderList"])
                {
                    list = TempData["orderList"] as List<OrderModel>;
                    TempData.Remove("orderList");
                }
                else
                {
                    list = GetOrderSinoUSList(filePath, goodsTypeId);
                    //list = ValidationOrderModel(list);
                }
                if (list.Any(o => !o.IsValid))
                {
                    ErrorNotification("保存失败，信息验证有误，请重新上传。");
                    return RedirectToAction("BatchAdd");
                }
                var listInfo = new List<CustomerOrderInfo>();

                string customerCode = _workContext.User.UserUame.ToUpperInvariant();

                var shippingMethods = _freightService.GetShippingMethods(null, true);
                foreach (OrderModel model in list)
                {
                    var entity = model.ToEntity<CustomerOrderInfo>();

                    entity.ShippingInfo = new ShippingInfo();
                    entity.SenderInfo = new SenderInfo();
                    model.ToEntity(entity.ShippingInfo);
                    model.ToEntity(entity.SenderInfo);
                    entity.Weight = model.Weight ?? 0;
                    entity.Length = model.Length ?? 1;
                    entity.Width = model.Width ?? 1;
                    entity.Height = model.Height ?? 1;
                    entity.SenderInfo.CountryCode = "CN";
                    entity.PackageNumber = 1;
                    entity.EnableTariffPrepay = false;
                    entity.IsInsured = false;
                    entity.IsBattery = false;
                    entity.IsHold = false;
                    entity.IsPrinted = false;
                    string code = model.ShippingMethodCode.ToUpperInvariant();
                    var shipingMethod = shippingMethods.Single(s => s.Code.ToUpperInvariant() == code);
                    entity.ShippingMethodId = shipingMethod.ShippingMethodId;
                    entity.ShippingMethodName = shipingMethod.FullName;
                    //在写入数据库时，将所有的单号都转换成大写——Add by zhengsong
                    entity.CustomerOrderNumber = entity.CustomerOrderNumber.ToUpperInvariant();
                    if (entity.TrackingNumber != null)
                    {
                        entity.TrackingNumber = entity.TrackingNumber.ToUpperInvariant();
                    }
                    entity.ShippingInfo.CountryCode = entity.ShippingInfo.CountryCode.ToUpperInvariant();
                    /*entity.InsuredID = entity.IsInsured ? entity.InsuredID : null;
                    entity.SensitiveTypeID = entity.IsBattery ? entity.SensitiveTypeID : null;*/
                    entity.CustomerCode = customerCode;
                    entity.EnableTariffPrepay = model.EnableTariffPrepayString == "Y";
                    listInfo.Add(entity);


                    //#region 操作日志
                    ////yungchu
                    ////敏感字-无
                    //BizLog bizlog = new BizLog()
                    //{
                    //	Summary = "中美专线订单上传",
                    //	KeywordType = KeywordType.CustomerOrderNumber,
                    //	Keyword = model.CustomerOrderNumber,
                    //	UserCode = _workContext.User.UserUame,
                    //	UserRealName = _customerService.GetCustomer(_workContext.User.UserUame).Name ?? _workContext.User.UserUame,
                    //	UserType = UserType.LMS_User,
                    //	SystemCode = SystemType.LMS,
                    //	ModuleName = "中美专线订单上传"
                    //};

                    //CustomerOrderInfoExportExt customerOrderInfoExportExt = new CustomerOrderInfoExportExt();
                    //model.CopyTo(customerOrderInfoExportExt);
                    //_operateLogServices.WriteLog(bizlog, customerOrderInfoExportExt);
                    //#endregion

                }
                _customerOrderService.BatchAdd(listInfo);

                SuccessNotification("保存成功");
            }
            catch (Exception e)
            {
                Log.Exception(e);
                ErrorNotification("保存失败，原因为：" + e.Message);
            }

            return RedirectToAction("BatchAddSinoUS");
        }


        #endregion

        public List<OrderModel> GetOrderList(string strFilePath, int goodsType)
        {
            var list = new List<OrderModel>();
            string name = _workContext.User.UserUame;
            DateTime now = DateTime.Now;
            string fileExt = Path.GetExtension(strFilePath).ToLower();
            using (var stream = new FileStream(strFilePath, FileMode.Open, FileAccess.Read))
            {

                IWorkbook workbook;
                if (fileExt == ".xls" || fileExt == ".et")
                {
                    workbook = new HSSFWorkbook(stream);
                }
                else
                {
                    workbook = new XSSFWorkbook(stream);
                }
                var sheet = workbook.GetSheetAt(0);
                var headerRow = sheet.GetRow(0);
                int emptyRow = 0;
                for (int i = (sheet.FirstRowNum + 1); i < (sheet.LastRowNum + 1); i++)
                {
                    int execlRow = i + 1;
                    IRow row = sheet.GetRow(i);
                    if (row == null)
                    {
                        if (emptyRow >= 4)
                            break;
                        emptyRow++;
                        continue;
                    }
                    emptyRow = 0;
                    var orderModel = new OrderModel { GoodsTypeID = goodsType };
                    int num = row.FirstCellNum;
                    if (row.Cells.Any(p => !string.IsNullOrWhiteSpace(row.GetCell(num).ToString())))
                    {
                        orderModel.CustomerOrderNumber = GetValue(row, ref num).Trim();
                        orderModel.ShippingMethodCode = GetValue(row, ref num).Trim();
                        orderModel.TrackingNumber = GetValue(row, ref num).Trim();
                        orderModel.CountryCode = GetValue(row, ref num).Trim();
                        orderModel.ShippingFirstName = GetValue(row, ref num).Trim();
                        orderModel.ShippingLastName = GetValue(row, ref num).Trim();
                        orderModel.ShippingCompany = GetValue(row, ref num).Trim();
                        orderModel.ShippingAddress = GetValue(row, ref num).Trim();
                        orderModel.ShippingCity = GetValue(row, ref num).Trim();
                        orderModel.ShippingState = GetValue(row, ref num).Trim();
                        orderModel.ShippingZip = GetValue(row, ref num).Trim();
                        orderModel.ShippingPhone = GetValue(row, ref num).Trim();
                        orderModel.ShippingTaxId = GetValue(row, ref num).Trim();
                        orderModel.SenderFirstName = GetValue(row, ref num).Trim();
                        orderModel.SenderLastName = GetValue(row, ref num).Trim();
                        orderModel.SenderCompany = GetValue(row, ref num).Trim();
                        orderModel.SenderAddress = GetValue(row, ref num).Trim();
                        orderModel.SenderCity = GetValue(row, ref num).Trim();
                        orderModel.SenderState = GetValue(row, ref num).Trim();
                        orderModel.SenderZip = GetValue(row, ref num).Trim();
                        orderModel.SenderPhone = GetValue(row, ref num).Trim();

                        string returnString = GetValue(row, ref num).Trim().ToUpper();
                        switch (returnString)
                        {
                            case "Y":
                                orderModel.ReturnString = "Y";
                                break;
                            case "N":
                                orderModel.ReturnString = "N";
                                break;
                            default:
                                orderModel.ReturnString = "N";
                                break;
                        }
                        orderModel.InsuredValue = GetValue(row, ref num).Trim();
                        string insureAmountValue = GetValue(row, ref num).Trim();
                        orderModel.InsureAmountValue = !string.IsNullOrWhiteSpace(insureAmountValue) ? insureAmountValue : "0";
                        orderModel.SensitiveTypeID = GetValue(row, ref num).Trim();
                        string appLicationTypeIds = GetValue(row, ref num).Trim().ToUpper();
                        switch (appLicationTypeIds)
                        {
                            case "GIFT":
                                orderModel.AppLicationTypeId =
                                    CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Gift);
                                break;
                            case "DOCUMENTS":
                                orderModel.AppLicationTypeId =
                                    CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Documents);
                                break;
                            case "SAMEPLE":
                                orderModel.AppLicationTypeId =
                                    CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Sameple);
                                break;
                            case "OTHERS":
                                orderModel.AppLicationTypeId =
                                    CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Others);
                                break;
                            default:
                                orderModel.AppLicationTypeId =
                                    CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Others);
                                break;
                        }
                        string packageNumbers = GetValue(row, ref num).Trim();
                        if (!string.IsNullOrWhiteSpace(packageNumbers))
                        {
                            int packageNumber = 1;
                            if (int.TryParse(packageNumbers, out packageNumber))
                                orderModel.PackageNumber = packageNumber;
                        }
                        else
                        {
                            orderModel.PackageNumber = 1;
                        }
                        string lengths = GetValue(row, ref num).Trim();
                        if (!string.IsNullOrWhiteSpace(lengths))
                        {
                            decimal length = 1;
                            if (decimal.TryParse(lengths, out length))
                            {
                                orderModel.Length = length == 0 ? 1 : length;
                            }
                        }
                        else
                        {
                            orderModel.Length = 1;
                        }
                        string widths = GetValue(row, ref num).Trim();
                        if (!string.IsNullOrWhiteSpace(widths))
                        {
                            decimal width = 1;
                            if (decimal.TryParse(widths, out width))
                            {
                                orderModel.Width = width == 0 ? 1 : width;
                            }
                        }
                        else
                        {
                            orderModel.Width = 1;
                        }
                        string heights = GetValue(row, ref num).Trim();
                        if (!string.IsNullOrWhiteSpace(heights))
                        {
                            decimal height = 1;
                            if (decimal.TryParse(heights, out height))
                            {
                                orderModel.Height = height == 0 ? 1 : height;
                            }
                        }
                        else
                        {
                            orderModel.Height = 1;
                        }
                        string weights = GetValue(row, ref num).Trim();
                        if (!string.IsNullOrWhiteSpace(weights))
                        {
                            decimal weight = 0;
                            if (decimal.TryParse(weights, out weight))
                            {
                                orderModel.Weight = weight;
                            }
                        }
                        else
                        {
                            orderModel.Weight = 0;
                        }

                        string enableTariffPrepayString = GetValue(row, ref num).Trim().ToUpper();
                        switch (enableTariffPrepayString)
                        {
                            case "Y":
                                orderModel.EnableTariffPrepayString = "Y";
                                break;
                            case "N":
                                orderModel.EnableTariffPrepayString = "N";
                                break;
                            default:
                                orderModel.EnableTariffPrepayString = "N";
                                break;
                        }

                        orderModel.CreatedBy = name;
                        orderModel.CreatedOn = now;
                        orderModel.LastUpdatedOn = now;
                        orderModel.LastUpdatedBy = name;
                        orderModel.ExeclRow = execlRow;
                        //if (orderModel.CustomerOrderNumber != null &&
                        //    list.Any(o => o.CustomerOrderNumber == orderModel.CustomerOrderNumber))
                        //{
                        //    orderModel.IsValid = false;
                        //    orderModel.ErrorType = 4;
                        //    orderModel.ErrorMessage.AppendLine(string.Format("存在相同的订单号[{0}]",
                        //                                                     orderModel.CustomerOrderNumber));
                        //}
                        //else if (orderModel.CustomerOrderNumber != null)
                        //{
                        //    if (_orderService.IsExitOrderNUmber(orderModel.CustomerOrderNumber,
                        //                                        _workContext.User.UserUame))
                        //    {
                        //        orderModel.IsValid = false;
                        //        orderModel.ErrorType = 4;
                        //        orderModel.ErrorMessage.AppendLine(string.Format("订单号[{0}]已存在",
                        //                                                         orderModel.CustomerOrderNumber));
                        //    }
                        //}
                        //if (orderModel.TrackingNumber != null &&
                        //    list.Any(o => o.TrackingNumber == orderModel.TrackingNumber))
                        //{
                        //    orderModel.IsValid = false;
                        //    orderModel.ErrorType = 4;
                        //    orderModel.ErrorMessage.AppendLine(string.Format("存在相同的跟踪号[{0}]",
                        //                                                     orderModel.TrackingNumber));
                        //}
                        //else if (!string.IsNullOrWhiteSpace(orderModel.TrackingNumber) && _orderService.IsExitTrackingNumber(orderModel.TrackingNumber, _workContext.User.UserUame))
                        //{
                        //    orderModel.IsValid = false;
                        //    orderModel.ErrorType = 4;
                        //    orderModel.ErrorMessage.AppendLine(string.Format("跟踪号[{0}]已存在",
                        //                                                     orderModel.TrackingNumber));
                        //}
                        int x = (headerRow.LastCellNum + 1 - num) / 8;

                        //int y = (headerRow.LastCellNum - num)%7;

                        //if (y != 0)
                        //{
                        //    orderModel.IsValid = false;
                        //    orderModel.ErrorType = 4;
                        //    orderModel.ErrorMessage.AppendLine(string.Format("申报信息第{0}项信息不完整", x + 1));
                        //}

                        list.Add(orderModel);
                        if (x > 0)
                        {
                            for (int j = 0; j < x; j++)
                            {
                                var productModel = new ProductModel();

                                if (num >= row.LastCellNum)
                                {
                                    break;
                                }

                                productModel.ApplicationName = GetValue(row, ref num).Trim();
                                productModel.PickingName = GetValue(row, ref num).Trim();
                                productModel.HSCode = GetValue(row, ref num).Trim();
                                productModel.Qty = GetValue(row, ref num).Trim();
                                productModel.UnitPrice = GetValue(row, ref num).Trim();
                                productModel.UnitWeight = GetValue(row, ref num).Trim();
                                productModel.ProductUrl = GetValue(row, ref num).Trim();
                                productModel.Remark = GetValue(row, ref num).Trim();
                                productModel.CreatedBy = name;
                                productModel.CreatedOn = now;
                                productModel.LastUpdatedOn = now;
                                productModel.LastUpdatedBy = name;

                                if (productModel.ApplicationName.IsNullOrWhiteSpace()
                                    && productModel.HSCode.IsNullOrWhiteSpace()
                                    && productModel.Qty.IsNullOrWhiteSpace()
                                    && productModel.UnitPrice.IsNullOrWhiteSpace())
                                    continue;
                                orderModel.ApplicationInfos.Add(productModel);

                            }
                        }

                    }
                    else
                    {
                        break;
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 中美专线上传 数据读取
        /// Add By zhengsong
        /// Time:2014-08-08 
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <param name="goodsType"></param>
        /// <returns></returns>
        public List<OrderModel> GetOrderSinoUSList(string strFilePath, int goodsType)
        {
            var list = new List<OrderModel>();
            string name = _workContext.User.UserUame;
            DateTime now = DateTime.Now;
            string fileExt = Path.GetExtension(strFilePath).ToLower();
            using (var stream = new FileStream(strFilePath, FileMode.Open, FileAccess.Read))
            {

                IWorkbook workbook;
                if (fileExt == ".xls" || fileExt == ".et")
                {
                    workbook = new HSSFWorkbook(stream);
                }
                else
                {
                    workbook = new XSSFWorkbook(stream);
                }
                var sheet = workbook.GetSheetAt(0);
                var headerRow = sheet.GetRow(0);
                int emptyRow = 0;
                for (int i = (sheet.FirstRowNum + 1); i < (sheet.LastRowNum + 1); i++)
                {
                    int execlRow = i + 1;
                    IRow row = sheet.GetRow(i);
                    if (row == null)
                    {
                        if (emptyRow >= 4)
                            break;
                        emptyRow++;
                        continue;
                    }
                    emptyRow = 0;
                    var orderModel = new OrderModel { GoodsTypeID = goodsType };
                    int num = row.FirstCellNum;
                    if (row.Cells.Any(p => !string.IsNullOrWhiteSpace(row.GetCell(num).ToString())))
                    {
                        orderModel.CustomerOrderNumber = GetValue(row, ref num).Trim();
                        orderModel.ShippingMethodCode = GetValue(row, ref num).Trim();
                        orderModel.TrackingNumber = GetValue(row, ref num).Trim();
                        orderModel.ShippingCompany = GetValue(row, ref num).Trim();
                        orderModel.ShippingFirstName = GetValue(row, ref num).Trim();
                        orderModel.ShippingAddress = GetValue(row, ref num).Trim();
                        orderModel.ShippingAddress1 = GetValue(row, ref num).Trim();
                        orderModel.ShippingCity = GetValue(row, ref num).Trim();
                        orderModel.ShippingState = GetValue(row, ref num).Trim();
                        orderModel.ShippingZip = GetValue(row, ref num).Trim();
                        orderModel.CountryCode = GetValue(row, ref num).Trim();
                        orderModel.ShippingPhone = GetValue(row, ref num).Trim();

                        //orderModel.ShippingTaxId = GetValue(row, ref num).Trim();
                        //orderModel.SenderFirstName = GetValue(row, ref num).Trim();
                        //orderModel.SenderLastName = GetValue(row, ref num).Trim();
                        //orderModel.SenderCompany = GetValue(row, ref num).Trim();
                        //orderModel.SenderAddress = GetValue(row, ref num).Trim();
                        //orderModel.SenderCity = GetValue(row, ref num).Trim();
                        //orderModel.SenderState = GetValue(row, ref num).Trim();
                        //orderModel.SenderZip = GetValue(row, ref num).Trim();
                        //orderModel.SenderPhone = GetValue(row, ref num).Trim();

                        //string returnString = GetValue(row, ref num).Trim().ToUpper();

                        //switch (returnString)
                        //{
                        //    case "Y":
                        //        orderModel.ReturnString = "Y";
                        //        break;
                        //    case "N":
                        //        orderModel.ReturnString = "N";
                        //        break;
                        //    default:
                        //        orderModel.ReturnString = "N";
                        //        break;
                        //}
                        //orderModel.InsuredValue = GetValue(row, ref num).Trim();
                        //string insureAmountValue = GetValue(row, ref num).Trim();
                        //orderModel.InsureAmountValue = !string.IsNullOrWhiteSpace(insureAmountValue) ? insureAmountValue : "0";
                        //orderModel.SensitiveTypeID = GetValue(row, ref num).Trim();
                        //string appLicationTypeIds = GetValue(row, ref num).Trim().ToUpper();
                        //switch (appLicationTypeIds)
                        //{
                        //    case "GIFT":
                        //        orderModel.AppLicationTypeId =
                        //            CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Gift);
                        //        break;
                        //    case "DOCUMENTS":
                        //        orderModel.AppLicationTypeId =
                        //            CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Documents);
                        //        break;
                        //    case "SAMEPLE":
                        //        orderModel.AppLicationTypeId =
                        //            CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Sameple);
                        //        break;
                        //    case "OTHERS":
                        //        orderModel.AppLicationTypeId =
                        //            CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Others);
                        //        break;
                        //    default:
                        //        orderModel.AppLicationTypeId =
                        //            CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Others);
                        //        break;
                        //}
                        //string packageNumbers = GetValue(row, ref num).Trim();
                        //if (!string.IsNullOrWhiteSpace(packageNumbers))
                        //{
                        //    int packageNumber = 1;
                        //    if (int.TryParse(packageNumbers, out packageNumber))
                        //        orderModel.PackageNumber = packageNumber;
                        //}
                        //else
                        //{
                        //    orderModel.PackageNumber = 1;
                        //}
                        string lengths = GetValue(row, ref num).Trim();
                        if (!string.IsNullOrWhiteSpace(lengths))
                        {
                            decimal length = 1;
                            if (decimal.TryParse(lengths, out length))
                            {
                                orderModel.Length = length == 0 ? 1 : length;
                            }
                        }
                        else
                        {
                            orderModel.Length = 1;
                        }
                        string widths = GetValue(row, ref num).Trim();
                        if (!string.IsNullOrWhiteSpace(widths))
                        {
                            decimal width = 1;
                            if (decimal.TryParse(widths, out width))
                            {
                                orderModel.Width = width == 0 ? 1 : width;
                            }
                        }
                        else
                        {
                            orderModel.Width = 1;
                        }
                        string heights = GetValue(row, ref num).Trim();
                        if (!string.IsNullOrWhiteSpace(heights))
                        {
                            decimal height = 1;
                            if (decimal.TryParse(heights, out height))
                            {
                                orderModel.Height = height == 0 ? 1 : height;
                            }
                        }
                        else
                        {
                            orderModel.Height = 1;
                        }
                        string weights = GetValue(row, ref num).Trim();
                        if (!string.IsNullOrWhiteSpace(weights))
                        {
                            decimal weight = 0;
                            if (decimal.TryParse(weights, out weight))
                            {
                                orderModel.Weight = weight;
                            }
                        }
                        else
                        {
                            orderModel.Weight = 0;
                        }

                        //string enableTariffPrepayString = GetValue(row, ref num).Trim().ToUpper();
                        //switch (enableTariffPrepayString)
                        //{
                        //    case "Y":
                        //        orderModel.EnableTariffPrepayString = "Y";
                        //        break;
                        //    case "N":
                        //        orderModel.EnableTariffPrepayString = "N";
                        //        break;
                        //    default:
                        //        orderModel.EnableTariffPrepayString = "N";
                        //        break;
                        //}
                        orderModel.ReturnString = "Y";
                        orderModel.CreatedBy = name;
                        orderModel.CreatedOn = now;
                        orderModel.LastUpdatedOn = now;
                        orderModel.LastUpdatedBy = name;
                        orderModel.ExeclRow = execlRow;
                        //if (orderModel.CustomerOrderNumber != null &&
                        //    list.Any(o => o.CustomerOrderNumber == orderModel.CustomerOrderNumber))
                        //{
                        //    orderModel.IsValid = false;
                        //    orderModel.ErrorType = 4;
                        //    orderModel.ErrorMessage.AppendLine(string.Format("存在相同的订单号[{0}]",
                        //                                                     orderModel.CustomerOrderNumber));
                        //}
                        //else if (orderModel.CustomerOrderNumber != null)
                        //{
                        //    if (_orderService.IsExitOrderNUmber(orderModel.CustomerOrderNumber,
                        //                                        _workContext.User.UserUame))
                        //    {
                        //        orderModel.IsValid = false;
                        //        orderModel.ErrorType = 4;
                        //        orderModel.ErrorMessage.AppendLine(string.Format("订单号[{0}]已存在",
                        //                                                         orderModel.CustomerOrderNumber));
                        //    }
                        //}
                        //if (orderModel.TrackingNumber != null &&
                        //    list.Any(o => o.TrackingNumber == orderModel.TrackingNumber))
                        //{
                        //    orderModel.IsValid = false;
                        //    orderModel.ErrorType = 4;
                        //    orderModel.ErrorMessage.AppendLine(string.Format("存在相同的跟踪号[{0}]",
                        //                                                     orderModel.TrackingNumber));
                        //}
                        //else if (!string.IsNullOrWhiteSpace(orderModel.TrackingNumber) && _orderService.IsExitTrackingNumber(orderModel.TrackingNumber, _workContext.User.UserUame))
                        //{
                        //    orderModel.IsValid = false;
                        //    orderModel.ErrorType = 4;
                        //    orderModel.ErrorMessage.AppendLine(string.Format("跟踪号[{0}]已存在",
                        //                                                     orderModel.TrackingNumber));
                        //}

                        string appLicationTypeIds = GetValue(row, ref num).Trim().ToUpper();
                        switch (appLicationTypeIds)
                        {
                            case "GIFT":
                                orderModel.AppLicationTypeId =
                                    CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Gift);
                                break;
                            case "DOCUMENTS":
                                orderModel.AppLicationTypeId =
                                    CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Documents);
                                break;
                            case "SAMEPLE":
                                orderModel.AppLicationTypeId =
                                    CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Sameple);
                                break;
                            case "OTHERS":
                                orderModel.AppLicationTypeId =
                                    CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Others);
                                break;
                            default:
                                orderModel.AppLicationTypeId =
                                    CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Others);
                                break;
                        }

                        int x = (headerRow.LastCellNum - num) / 5;

                        //int y = (headerRow.LastCellNum - num)%7;

                        //if (y != 0)
                        //{
                        //    orderModel.IsValid = false;
                        //    orderModel.ErrorType = 4;
                        //    orderModel.ErrorMessage.AppendLine(string.Format("申报信息第{0}项信息不完整", x + 1));
                        //}

                        list.Add(orderModel);
                        if (x > 0)
                        {
                            for (int j = 0; j < x; j++)
                            {
                                var productModel = new ProductModel();

                                if (num >= row.LastCellNum)
                                {
                                    break;
                                }

                                productModel.ApplicationName = GetValue(row, ref num).Trim();
                                productModel.PickingName = GetValue(row, ref num).Trim();
                                //productModel.HSCode = GetValue(row, ref num).Trim();
                                productModel.UnitPrice = GetValue(row, ref num).Trim();
                                productModel.Qty = GetValue(row, ref num).Trim();
                                //productModel.UnitWeight = GetValue(row, ref num).Trim();
                                productModel.Remark = GetValue(row, ref num).Trim();
                                productModel.CreatedBy = name;
                                productModel.CreatedOn = now;
                                productModel.LastUpdatedOn = now;
                                productModel.LastUpdatedBy = name;

                                if (productModel.ApplicationName.IsNullOrWhiteSpace()
                                    && productModel.HSCode.IsNullOrWhiteSpace()
                                    && productModel.Qty.IsNullOrWhiteSpace()
                                    && productModel.UnitPrice.IsNullOrWhiteSpace())
                                    continue;
                                orderModel.ApplicationInfos.Add(productModel);

                            }
                        }

                    }
                    else
                    {
                        break;
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 读取Excle数据 add by huhaiyou 2014-4-18
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <param name="goodsType"></param>
        /// <returns></returns>
        public List<OrderModel> GetOrderPackageList(string strFilePath, int goodsType)
        {
            var list = new List<OrderModel>();
            string name = _workContext.User.UserUame;
            DateTime now = DateTime.Now;
            ExcelOperation op = new ExcelOperation();
            string fileExt = Path.GetExtension(strFilePath).ToLower();
            using (var stream = new FileStream(strFilePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook;
                if (fileExt == ".xls" || fileExt == ".et")
                {
                    workbook = new HSSFWorkbook(stream);
                }
                else
                {
                    workbook = new XSSFWorkbook(stream);
                }
                var sheet = workbook.GetSheetAt(0);
                var headerRow = sheet.GetRow(0);
                int emptyRow = 0;
                for (int i = (sheet.FirstRowNum + 1); i < (sheet.LastRowNum + 1); i++)
                {
                    int execlRow = i + 1;
                    IRow row = sheet.GetRow(i);
                    if (row == null)
                    {
                        if (emptyRow >= 4)
                            break;
                        emptyRow++;
                        continue;
                    }
                    emptyRow = 0;
                    var orderModel = new OrderModel { GoodsTypeID = goodsType };
                    int num = row.FirstCellNum;
                    if (row.Cells.Any(p => !string.IsNullOrWhiteSpace(row.GetCell(num).ToString())))
                    {
                        orderModel.CustomerOrderNumber = GetValue(row, ref num).Trim();
                        orderModel.ShippingMethodCode = GetValue(row, ref num).Trim();
                        orderModel.TrackingNumber = GetValue(row, ref num).Trim();
                        orderModel.CountryCode = GetValue(row, ref num).Trim();
                        orderModel.ShippingFirstName = GetValue(row, ref num).Trim();
                        orderModel.ShippingLastName = GetValue(row, ref num).Trim();
                        orderModel.ShippingCompany = GetValue(row, ref num).Trim();
                        orderModel.ShippingAddress = GetValue(row, ref num).Trim();
                        orderModel.ShippingCity = GetValue(row, ref num).Trim();
                        orderModel.ShippingState = GetValue(row, ref num).Trim();
                        orderModel.ShippingZip = GetValue(row, ref num).Trim();
                        orderModel.ShippingPhone = GetValue(row, ref num).Trim();

                        string returnString = GetValue(row, ref num).Trim().ToUpper();
                        switch (returnString)
                        {
                            case "Y":
                                orderModel.ReturnString = "Y";
                                break;
                            case "N":
                                orderModel.ReturnString = "N";
                                break;
                            default:
                                orderModel.ReturnString = "N";
                                break;
                        }
                        string weights = GetValue(row, ref num).Trim();
                        if (!string.IsNullOrWhiteSpace(weights))
                        {
                            decimal weight = 0;
                            if (decimal.TryParse(weights, out weight))
                            {
                                orderModel.Weight = weight;
                            }
                        }
                        else
                        {
                            orderModel.Weight = 0;
                        }
                        string appLicationTypeIds = GetValue(row, ref num).Trim().ToUpper();
                        switch (appLicationTypeIds)
                        {
                            case "GIFT":
                                orderModel.AppLicationTypeId =
                                    CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Gift);
                                break;
                            case "DOCUMENTS":
                                orderModel.AppLicationTypeId =
                                    CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Documents);
                                break;
                            case "SAMEPLE":
                                orderModel.AppLicationTypeId =
                                    CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Sameple);
                                break;
                            case "OTHERS":
                                orderModel.AppLicationTypeId =
                                    CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Others);
                                break;
                            default:
                                orderModel.AppLicationTypeId =
                                    CustomerOrder.ApplicationTypeToValue(CustomerOrder.ApplicationTypeEnum.Others);
                                break;
                        }
                        orderModel.CreatedBy = name;
                        orderModel.CreatedOn = now;
                        orderModel.LastUpdatedOn = now;
                        orderModel.LastUpdatedBy = name;
                        orderModel.ExeclRow = execlRow;

                        int x = (headerRow.LastCellNum - num) / 7;//获取Excle列数，判断报关信息
                        list.Add(orderModel);
                        if (x > 0)
                        {
                            for (int j = 0; j < x; j++)
                            {
                                var productModel = new ProductModel();

                                if (num >= row.LastCellNum)
                                {
                                    break;
                                }

                                productModel.ApplicationName = GetValue(row, ref num).Trim();
                                productModel.PickingName = GetValue(row, ref num).Trim();
                                productModel.HSCode = GetValue(row, ref num).Trim();
                                productModel.Qty = GetValue(row, ref num).Trim();
                                productModel.UnitPrice = GetValue(row, ref num).Trim();
                                productModel.UnitWeight = GetValue(row, ref num).Trim();
                                productModel.Remark = GetValue(row, ref num).Trim();
                                productModel.CreatedBy = name;
                                productModel.CreatedOn = now;
                                productModel.LastUpdatedOn = now;
                                productModel.LastUpdatedBy = name;

                                if (productModel.ApplicationName.IsNullOrWhiteSpace()
                                    && productModel.HSCode.IsNullOrWhiteSpace()
                                    && productModel.Qty.IsNullOrWhiteSpace()
                                    && productModel.UnitPrice.IsNullOrWhiteSpace())
                                    continue;
                                orderModel.ApplicationInfos.Add(productModel);

                            }
                        }

                    }
                    else
                    {
                        break;
                    }
                }
            }

            return list;
        }

        [HttpPost]
        public ActionResult SaveIsValid(string data, int execlRowId, string shippngMethodCode = null, string countryCode = null)
        {
            var list = JsonHelper.JsonDeserialize<List<OrderModel>>(data);
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    if (item.ExeclRow == execlRowId)
                    {
                        if (!string.IsNullOrWhiteSpace(shippngMethodCode))
                        { item.ShippingMethodCode = shippngMethodCode; }
                        if (!string.IsNullOrWhiteSpace(countryCode))
                        {
                            item.CountryCode = countryCode;
                        }
                    }
                    item.ErrorMessage = new StringBuilder("");
                    item.ErrorType = 0;
                    item.IsValid = true;
                }
            }
            list = ValidationOrderModel(list);
            TempData["orderList"] = list;
            var viewModels = new BatchAddViewModels();
            viewModels.OrderList = list;
            var freightService = EngineContext.Current.Resolve<IFreightService>();
            var shippingMethodeList = freightService.GetShippingMethods(null, true);
            shippingMethodeList.ForEach(p => viewModels.ShippingMethodModels.Add(new SelectListItem()
            {
                Text = p.FullName,
                Value = p.Code
            }));
            viewModels.CountryModels = GetCountryList("");
            return PartialView("_BatchAddList", viewModels);
        }

        public List<OrderModel> ValidationOrderModel(List<OrderModel> list)
        {
            var validator = new OrderModelValidator();
            var productValidator = new ProductModelValidator();
            var shippingMethods = _freightService.GetShippingMethods("", true);
            var insureList = _insuredCalculationService.GetList();
            List<string> customerNumber = new List<string>();
            List<string> trackingNumber = new List<string>();
            list.ForEach(p => customerNumber.Add(p.CustomerOrderNumber.ToUpperInvariant()));
            list.ForEach(p =>
            {
                if (!p.TrackingNumber.IsNullOrWhiteSpace())
                {
                    trackingNumber.Add(p.TrackingNumber.ToUpperInvariant());
                }
                p.ShippingMethodCode = p.ShippingMethodCode.ToUpperInvariant();

            });
            List<string> customerNumbers = new List<string>();
            List<string> trackingNumbers = new List<string>();
            trackingNumbers = _customerOrderService.GetCustomerOrderInfoByTrack(trackingNumber);
            customerNumbers = _customerOrderService.GetCustomerOrderInfos(customerNumber);
            //客户是否开启关税预付 yungchu
            List<TariffPrepayFeeShippingMethod> listTariffPrepayFee = _freightService.GetShippingMethodsTariffPrepay(_workContext.User.UserUame);
            foreach (var item in list)
            {
                //ValidationResult results = !sysConfig.NLPOSTMethodCode.Split(new string[]{","},StringSplitOptions.RemoveEmptyEntries).ToList().Contains(item.ShippingMethodCode.ToUpper()) ? validator.Validate(item) : new OrderNlPostModelValidator().Validate(item);
                ValidationResult results;
                if (sysConfig.NLPOSTMethodCode.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                              .ToList()
                              .Contains(item.ShippingMethodCode.ToUpper()))
                {
                    results = new OrderNlPostModelValidator().Validate(item);
                }
                else if (sysConfig.LithuaniaMethodCode.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                  .ToList()
                                  .Contains(item.ShippingMethodCode.ToUpper()))
                {
                    results = new OrderSfModelValidator().Validate(new OrderSfModel()
                    {
                        ShippingName = item.ShippingFirstName + item.ShippingLastName,
                        ShippingAddress = item.ShippingAddress + item.ShippingAddress1 + item.ShippingAddress2,
                        ShippingCity = item.ShippingCity,
                        ShippingCompany = item.ShippingCompany,
                        ShippingPhone = item.ShippingPhone,
                        ShippingState = item.ShippingState,
                        ShippingTel = item.ShippingPhone,
                        ShippingZip = item.ShippingZip,
                        CountryCode = item.CountryCode
                    });
                }
                else
                {
                    results = validator.Validate(item);
                }
                //中美专线特殊通道上传

                if (sysConfig.SinoUSShippingMethodCode.Split(',').ToList().Contains(item.ShippingMethodCode.ToUpper()))
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine("中美专线订单请从专属通道上传！");
                    continue;
                }

                if (item.Width == null || item.Width <= 0)
                {
                    item.Width = 1;
                }
                if (item.Height == null || item.Height <= 0)
                {
                    item.Height = 1;
                }
                if (item.Length == null || item.Length <= 0)
                {
                    item.Length = 1;
                }

                #region 中邮挂号福州
                if (item.ShippingMethodCode != null && (item.ShippingMethodCode.Trim().ToUpperInvariant().ToUpperInvariant() == "CNPOST-FZ" || item.ShippingMethodCode.Trim().ToUpperInvariant() == "CNPOST-FYB"))
                {
                    if (item.CustomerOrderNumber != null && item.CustomerOrderNumber.Length > 30)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("订单号长度必须小于等于30");
                    }
                    //国家两位
                    if (item.CountryCode != null && item.CountryCode.Length != 2)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("国家简码必须是两位");
                    }
                    //收件人州或省
                    if (item.ShippingState != null && item.ShippingState.Length > 50)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人省或州长度不能超过50");
                    }
                    //收件人城市
                    if (item.ShippingCity != null && item.ShippingCity.Length > 50)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人城市长度不能超过50");
                    }
                    //收件人地址
                    string address = "";
                    if (item.ShippingAddress != null)
                    {
                        address += item.ShippingAddress;
                    }
                    if (item.ShippingAddress1 != null)
                    {
                        address += item.ShippingAddress1;
                    }
                    if (item.ShippingAddress2 != null)
                    {
                        address += item.ShippingAddress2;
                    }
                    if (address.Length > 120)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人地址长度不能超过120");
                    }
                    //收件人邮编
                    if (item.ShippingZip != null && item.ShippingZip.Length > 12)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人邮编长度不能超过12");
                    }
                    //收件人名字
                    string name = "";
                    if (item.ShippingFirstName != null)
                    {
                        name += item.ShippingFirstName;
                    }
                    if (item.ShippingLastName != null)
                    {
                        name += item.ShippingLastName;
                    }
                    if (string.IsNullOrWhiteSpace(name) || name.Length > 64)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人名字长度为1-64字符");
                    }
                    //收件人电话
                    if (item.ShippingPhone != null && item.ShippingPhone.Length > 20)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人电话长度不能超过20");
                    }
                    //发件人省份
                    if (item.SenderState != null && item.SenderState.Length > 20)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("发件人州省长度不能超过20");
                    }
                    //发件人城市
                    if (item.SenderCity != null && item.SenderCity.Length > 64)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("发件人城市长度不能超过64");
                    }
                    //发件人街道
                    if (item.SenderAddress != null && item.SenderAddress.Length > 120)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("发件人地址长度不能超过120");
                    }
                    //发件人邮编
                    if (item.SenderZip != null && item.SenderZip.Length > 6)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("发件人邮编长度不能超过6");
                    }
                    //发件人名字
                    string senderName = "";
                    if (item.SenderFirstName != null)
                    {
                        senderName += item.SenderFirstName;
                    }
                    if (item.SenderLastName != null)
                    {
                        senderName += item.SenderLastName;
                    }
                    if (senderName.Length > 20)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("发件人名字长度不能超过20");
                    }
                    //发件人电话
                    if (item.SenderPhone != null && item.SenderPhone.Length > 20)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("发件人电话长度不能超过20");
                    }
                }
                #endregion

                #region DHL 上传验证

                if (item.ShippingMethodCode != null && (item.ShippingMethodCode.Trim().ToUpperInvariant() == "HKDHL" || item.ShippingMethodCode.Trim().ToUpperInvariant() == "DHLCN" || item.ShippingMethodCode.Trim().ToUpperInvariant() == "DHLSG"))
                {
                    if (item.InsureAmount != null && item.InsureAmount.ToString().Length > 14)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("保险金额长度不能超过14个字符");
                    }
                    else if (item.InsureAmount != null && !Regex.IsMatch(item.InsureAmount.ToString(), "^[0-9]+[.]{0,1}[0-9]{0,2}$"))
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("保险金额有数字组成，最多保留两位小数");
                    }

                    if (item.ShippingCompany != null && item.ShippingCompany.Length > 35)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人公司长度为0-35个字");
                    }
                    string address = "";
                    if (item.ShippingAddress != null)
                    {
                        address += item.ShippingAddress;
                    }
                    if (item.ShippingAddress1 != null)
                    {
                        address += item.ShippingAddress1;
                    }
                    if (item.ShippingAddress2 != null)
                    {
                        address += item.ShippingAddress2;
                    }
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(address) && address.StringSplitLengthWords(35).Count > 2)
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine("收件人地址超长");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.ToString());
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人地址格式错误");

                    }

                    if (item.ShippingCity != null && item.ShippingCity.Length > 35)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人城市不能超过35个字符");
                    }

                    if (string.IsNullOrWhiteSpace(item.ShippingState) || item.ShippingState.Length > 35)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人州/省长度为1-35个字符");
                    }
                    if (string.IsNullOrWhiteSpace(item.ShippingZip) || item.ShippingZip.Length > 12)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人邮编长度为1-12个字符");
                    }

                    if (!string.IsNullOrWhiteSpace(item.ShippingTaxId) &&
                        item.ShippingTaxId.Length > 20)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人税号不能超过20字符");
                    }
                    if ((item.ShippingFirstName + item.ShippingLastName).Length > 35)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人姓名不能超过35个字符");
                    }

                    if (string.IsNullOrWhiteSpace(item.ShippingPhone))
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人电话不能为空");
                    }
                    else if (item.ShippingPhone.Length > 25)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人电话不能超过25个字符");
                    }
                }

                #endregion

                #region EUB 上传验证
                if (item != null &&
                    (item.ShippingMethodCode.Trim().ToUpperInvariant() == "EUB_CS" || item.ShippingMethodCode.Trim().ToUpperInvariant() == "EUB-SZ" ||
                     item.ShippingMethodCode.Trim().ToUpperInvariant() == "EUB-FZ"))
                {
                    if (item.CustomerOrderNumber != null && (item.CustomerOrderNumber.Length > 32 || item.CustomerOrderNumber.Length < 4))
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("订单号长度必须为4-32个字符");
                    }
                    string name = "";
                    if (item.ShippingFirstName != null)
                    {
                        name += item.ShippingFirstName;
                    }
                    if (item.ShippingLastName != null)
                    {
                        name += item.ShippingLastName;
                    }
                    if (string.IsNullOrWhiteSpace(name) || name.Length > 256)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人姓名长度为1-256个字符");
                    }
                    if (string.IsNullOrWhiteSpace(item.ShippingCity) || item.ShippingCity.Length > 128)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人城市长度为1-128个字符");
                    }

                    if (string.IsNullOrWhiteSpace(item.ShippingState) || item.ShippingState.Length > 128)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人州长度为1-128个字符");
                    }

                    if (item.ShippingZip != null)
                    {
                        if (item.ShippingZip.Length > 16)
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine("收件人邮编不能超过16个字符");
                        }
                        else
                        {
                            switch (item.CountryCode.ToUpperInvariant())
                            {
                                case "US":
                                    if (!Regex.IsMatch(item.ShippingZip, "^(^[0-9]{5}-[0-9]{4}$)|(^[0-9]{5}-[0-9]{5}$)|(^[0-9]{5}$)$"))
                                    {
                                        item.IsValid = false;
                                        item.ErrorType = 4;
                                        item.ErrorMessage.AppendLine("邮编不合法");
                                    }
                                    break;
                                case "AU":
                                    if (!Regex.IsMatch(item.ShippingZip, "^[0-9]{4}$"))
                                    {
                                        item.IsValid = false;
                                        item.ErrorType = 4;
                                        item.ErrorMessage.AppendLine("邮编不合法");
                                    }
                                    break;
                                case "CA":
                                    if (!Regex.IsMatch(item.ShippingZip, "^(^[A-Za-z][0-9][A-Za-z][ ][0-9][A-Za-z][0-9]$)|(^[A-Za-z][0-9][A-Za-z][0-9][A-Za-z][0-9]$)$"))
                                    {
                                        item.IsValid = false;
                                        item.ErrorType = 4;
                                        item.ErrorMessage.AppendLine("邮编不合法");
                                    }
                                    break;
                                case "GB":
                                    if (!Regex.IsMatch(item.ShippingZip, "^[A-Za-z0-9]{2,4} [A-Za-z0-9]{3}$"))
                                    {
                                        item.IsValid = false;
                                        item.ErrorType = 4;
                                        item.ErrorMessage.AppendLine("邮编不合法");
                                    }
                                    break;
                                case "FR":
                                    if (!Regex.IsMatch(item.ShippingZip, "^[0-9]{5}$"))
                                    {
                                        item.IsValid = false;
                                        item.ErrorType = 4;
                                        item.ErrorMessage.AppendLine("邮编不合法");
                                    }
                                    break;
                                case "RU":
                                    if (!Regex.IsMatch(item.ShippingZip, "^[0-9]{6}$"))
                                    {
                                        item.IsValid = false;
                                        item.ErrorType = 4;
                                        item.ErrorMessage.AppendLine("邮编不合法");
                                    }
                                    break;
                            }
                        }
                    }
                    else if (item.CountryCode != "HK")
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("邮编不能为空");
                    }

                }
                #endregion

                //欧洲专线上传 限制
                //Add By zhengsong
                if (sysConfig.DDPShippingMethodCode == item.ShippingMethodCode.ToUpper() ||
                    sysConfig.DDPRegisterShippingMethodCode == item.ShippingMethodCode.ToUpper() ||
                    sysConfig.EuropeShippingMethodCode == item.ShippingMethodCode.ToUpper())
                {
                    // OrderNumber 上传系统时限制只能是数字或字母，不能有其他符合  ，比如 - （ ）*，字符数量小于25
                    //Regex r = new Regex(@"^[A-Za-z0-9]{0,25}$");
                    //MatchCollection customerOrderNumber = r.Matches(item.CustomerOrderNumber);
                    //if (customerOrderNumber.Count < 1 || customerOrderNumber[0].Value != item.CustomerOrderNumber)
                    //{
                    //    item.IsValid = false;
                    //    item.ErrorType = 4;
                    //    item.ErrorMessage.AppendLine(string.Format("订单号[{0}]格式不符合要求",
                    //                                                     item.CustomerOrderNumber));
                    //}

                    if (item.CustomerOrderNumber.Length > 25)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                       
                        item.ErrorMessage.AppendLine(string.Format("订单号[{0}]不能超过25个字符",
                                                                         item.CustomerOrderNumber));
                    }

                    ////SKUCode1  上传系统时限制只能是数字或字母，不能有其他符合  ，比如 - （ ）*，字符数量小于30
                    //Regex z = new Regex(@"^[A-Za-z0-9]{0,30}$");
                    //foreach (var row in item.ApplicationInfos)
                    //{
                    //    MatchCollection remark = z.Matches(row.Remark);
                    //    if (remark.Count < 1 || remark[0].Value != row.Remark)
                    //    {
                    //        item.IsValid = false;
                    //        item.ErrorType = 4;
                    //        item.ErrorMessage.AppendLine(string.Format("申报信息[{0}]格式不符合要求", row.Remark));
                    //        break;
                    //    }
                    //}

                    ////PhoneNumber   只能是数字，不能出现其他字符，比如：&#43; &amp;
                    //Regex c = new Regex(@"^[0-9]{0,}$");
                    //MatchCollection shippingPhone = c.Matches(item.ShippingPhone);

                    //if (shippingPhone.Count < 1 || shippingPhone[0].Value != item.ShippingPhone)
                    //{
                    //    item.IsValid = false;
                    //    item.ErrorType = 4;
                    //    item.ErrorMessage.AppendLine(string.Format("收件人电话[{0}]格式不符合要求",
                    //                                                     item.ShippingPhone));
                    //}
                }
                if (item.CustomerOrderNumber != null &&
                    list.FindAll(o => o.CustomerOrderNumber.ToUpper() == item.CustomerOrderNumber.ToUpper()).Count > 1)
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine(string.Format("存在相同的订单号[{0}]",
                                                                     item.CustomerOrderNumber));
                }
                else if (item.CustomerOrderNumber != null)
                {

                    if (customerNumbers.Any(p => p == item.CustomerOrderNumber.ToUpperInvariant()))
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine(string.Format("订单号[{0}]已存在",
                                                                         item.CustomerOrderNumber));
                    }
                }

                var shipping = shippingMethods.FirstOrDefault(p => p.ShippingMethodId == sysConfig.SpecialShippingMethodId);
                if (shipping != null && item.ShippingMethodCode.Trim().ToUpperInvariant() == shipping.Code)
                {
                    if (!string.IsNullOrWhiteSpace(item.ShippingZip))
                    {
                        if (!Tools.CheckPostCode(item.ShippingZip))
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine("收件人邮编为6位纯数字且只能以1 2 3 4 6开头");
                        }
                    }
                    else
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人邮编不能为空");
                    }
                    if (!string.IsNullOrWhiteSpace(item.ShippingPhone))
                    {
                        if (!Tools.CheckShippingPhone(item.ShippingPhone))
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine("收件人电话号码最长不能超过11位数字");
                        }
                    }
                    else
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人联系电话不能为空");
                    }
                }
                if (!string.IsNullOrWhiteSpace(item.TrackingNumber) &&
                    list.FindAll(o => o.TrackingNumber.ToUpper() == item.TrackingNumber.ToUpperInvariant()).Count > 1)
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine(string.Format("存在相同的跟踪号[{0}]",
                                                                     item.TrackingNumber));
                }
                else if (!string.IsNullOrWhiteSpace(item.TrackingNumber) && trackingNumbers.Any(p => p.ToUpper() == item.TrackingNumber.ToUpperInvariant()))
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine(string.Format("跟踪号[{0}]已存在",
                                                                     item.TrackingNumber));
                }
                if (item.Weight == null || item.Weight <= 0)
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine("包裹重量必须大于零");
                }

                //收货国家俄罗斯，邮编不能为空
                if (item.ShippingMethodCode.Trim().ToUpperInvariant() == "LTPOST")
                {
                    if (item.CountryCode.ToUpperInvariant() == "RU")
                    {
                        if (string.IsNullOrWhiteSpace(item.ShippingZip))
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine("邮编不能为空");
                        }
                    }
                }

                //Add By zhengsong
                //是否是需要计算偏远附加费 ，需要验证省/州，城市，邮编
                var shippingMethod = shippingMethods.FirstOrDefault(p => p.Code == item.ShippingMethodCode);
                if (shippingMethod != null && shippingMethod.FuelRelateRAF)
                {
                    if (string.IsNullOrWhiteSpace(item.ShippingZip))
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("邮编不能为空");
                    }
                    if (string.IsNullOrWhiteSpace(item.ShippingState))
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("州/省不能为空");
                    }
                    if (string.IsNullOrWhiteSpace(item.ShippingCity))
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("城市不能为空");
                    }
                }


                if (!string.IsNullOrEmpty(item.ShippingMethodCode))
                {
                    if ((item.EnableTariffPrepayString.ToUpperInvariant() == "Y" || item.EnableTariffPrepayString.Trim() == "是"))
                    {
                        if (listTariffPrepayFee == null || listTariffPrepayFee.Count == 0)
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine("您未开通关税预付权限，请联系业务");
                        }
                        else  //客户是否开启该运输方式关税预付
                        {
                            List<string> listStr = new List<string>();
                            listTariffPrepayFee.ForEach(a => listStr.Add(a.Code.ToUpperInvariant()));
                            if (!listStr.Contains(item.ShippingMethodCode.ToUpperInvariant()))
                            {
                                item.IsValid = false;
                                item.ErrorType = 4;
                                item.ErrorMessage.AppendLine("您未开通关税预付权限，请联系业务");
                            }
                        }
                    }

                }
                int x = 1;
                if (item.ApplicationInfos.Count < 1)
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine("没有订单明细");
                }
                else
                {
                    foreach (var row in item.ApplicationInfos)
                    {
                        if (string.IsNullOrWhiteSpace(row.ApplicationName))
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项申报名称不能为空", x));
                        }
                        //福州邮政申报信息判断
                        if (shippingMethod != null && (shippingMethod.Code == "CNPOST-FZ" || shippingMethod.Code == "CNPOSTP_FZ" || shippingMethod.Code == "CNPOST-FYB"))
                        {
                            if (string.IsNullOrWhiteSpace(row.PickingName)|| row.PickingName.Length > 60)
                            {
                                item.IsValid = false;
                                item.ErrorType = 4;
                                item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项申报中文名称不能为空或超长", x));
                            }

                            if (row.ApplicationName.Length > 60)
                            {
                                item.IsValid = false;
                                item.ErrorType = 4;
                                item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项申报名称超长", x));
                            }
                            //if (string.IsNullOrWhiteSpace(row.UnitWeight) || row.UnitWeight == "0")
                            //{
                            //    item.IsValid = false;
                            //    item.ErrorType = 4;
                            //    item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项申报重量为空", x));
                            //}
                        }


                        //DHL验证申报信息
                        if (shippingMethod != null &&
                            (shippingMethod.Code.Trim().ToUpperInvariant() == "HKDHL" ||
                             shippingMethod.Code.Trim().ToUpperInvariant() == "DHLCN" ||
                             shippingMethod.Code.Trim().ToUpperInvariant() == "DHLSG"))
                        {
                                    if (string.IsNullOrWhiteSpace(row.ApplicationName) || row.ApplicationName.Length > 60)
                                    {
                                        item.IsValid = false;
                                        item.ErrorType = 4;
                                        item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项申报英文名称不能为空或超过字符长度", x));
                                    }
                                    else
                                    {
                                        if (Regex.IsMatch(row.ApplicationName,
                                                           @"[\~]{1}|[\@]{1}|[\#]{1}|[\$]{1}|[\￥]{1}|[\%]{1}|[\^]{1}|[\&]{1}|[\*]{1}|[\(]{1}|[\)]{1}|[\u4e00-\u9fa5]+"))
                                        {
                                            item.IsValid = false;
                                            item.ErrorType = 4;
                                            item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项申报英文名称不能包含特殊字符和汉字", x));
                                            
                                        }
                                    }
                                    //if (row.UnitWeight == null || row.UnitWeight =="0")
                                    //{
                                    //    item.IsValid = false;
                                    //    item.ErrorType = 4;
                                    //    item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项列申报重量不能为0", x));
                                        
                                    //}
                        }


                        //EUB 申报信息验证
                        if (shippingMethod != null && (shippingMethod.Code.Trim() == "EUB_CS" || shippingMethod.Code.Trim() == "EUB-SZ" || shippingMethod.Code.Trim() == "EUB-FZ"))
                        {

                            if (string.IsNullOrWhiteSpace(row.PickingName) || row.PickingName.Length > 64)
                            {
                                item.IsValid = false;
                                item.ErrorType = 4;
                                item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项申报中文名称不能为空或超长", x));
                            }
                            else
                            {
                                if (!Regex.IsMatch(row.PickingName, @"[\u4e00-\u9fa5]+[A-Za-z0-9]*[\s\S]*[\u4e00-\u9fa5]+"))
                                {
                                    item.IsValid = false;
                                item.ErrorType = 4;
                                    item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项申报中文名称至少包含2个汉子", x));
                                }
                            }
                            if (string.IsNullOrWhiteSpace(row.ApplicationName) || row.ApplicationName.Length > 128)
                            {
                                item.IsValid = false;
                                item.ErrorType = 4;
                                item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项申报名称为空或超长", x));
                            }

                            //if (string.IsNullOrWhiteSpace(row.UnitWeight) || row.UnitWeight=="0")
                            //{
                            //    item.IsValid = false;
                            //    item.ErrorType = 4;
                            //    item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项申报重量为空", x));
                            //}
                        }

                        if (string.IsNullOrWhiteSpace(row.HSCode) &&
                                 (item.ShippingMethodCode.Trim().ToUpperInvariant() ==
                                  sysConfig.DDPRegisterShippingMethodCode ||
                                  item.ShippingMethodCode.Trim().ToUpperInvariant() == sysConfig.DDPShippingMethodCode))
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项海关编码不能为空", x));
                        }
                        if (string.IsNullOrWhiteSpace(row.ProductUrl) &&
                                 (item.ShippingMethodCode.Trim().ToUpperInvariant() ==
                                  sysConfig.DDPRegisterShippingMethodCode ||
                                  item.ShippingMethodCode.Trim().ToUpperInvariant() == sysConfig.DDPShippingMethodCode))
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项销售链接不能为空", x));
                        }
                        if (string.IsNullOrWhiteSpace(row.Remark) &&
                                 (item.ShippingMethodCode.Trim().ToUpperInvariant() ==
                                  sysConfig.DDPRegisterShippingMethodCode ||
                                  item.ShippingMethodCode.Trim().ToUpperInvariant() == sysConfig.DDPShippingMethodCode))
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项备注不能为空", x));
                        }
                        x++;
                    }
                }
                #region 判断保险类型
                if (string.IsNullOrWhiteSpace(item.InsuredValue))
                {
                    item.IsInsured = false;
                }
                else
                {
                    string[] id = item.InsuredValue.Split('_');
                    int idd;
                    if (int.TryParse(id[0], out idd))
                    {
                        if (insureList.Any(p => p.InsuredID == int.Parse(id[0])))
                        {
                            item.IsInsured = true;
                            item.InsuredID = id[0];
                            if (item.InsuredID == "2")
                            {
                                if (!string.IsNullOrWhiteSpace(item.InsureAmountValue))
                                {
                                    decimal amount;
                                    if (decimal.TryParse(item.InsureAmountValue, out amount))
                                    {
                                        if (amount < 1)
                                        {
                                            item.IsValid = false;
                                            item.ErrorType = 4;
                                            item.ErrorMessage.AppendLine("投保金额必须大于0");
                                        }
                                        else
                                        {
                                            item.InsureAmount = amount;
                                        }
                                    }
                                    else
                                    {
                                        item.IsValid = false;
                                        item.ErrorType = 4;
                                        item.ErrorMessage.AppendLine("投保金额必须位数字");
                                    }
                                }
                                else
                                {
                                    item.IsValid = false;
                                    item.ErrorType = 4;
                                    item.ErrorMessage.AppendLine("投保金额不能为空");
                                }
                            }
                            else
                            {
                                item.InsureAmount = decimal.Parse(insureList.Find(p => p.InsuredID == int.Parse(item.InsuredID))
                                               .InsuredCalculation1);
                            }
                        }
                        else
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine("保险类型不存在");
                        }
                    }
                    else
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("保险类型不存在");
                    }
                }
                #endregion
                bool validationSucceeded = results.IsValid;
                IList<ValidationFailure> failures = results.Errors;
                Console.WriteLine("******************{0}**********************".FormatWith(item.CustomerOrderNumber));
                if (!validationSucceeded)
                {
                    item.IsValid = false;
                    int ErrorType = 0;
                    foreach (var error in failures)
                    {

                        item.ErrorMessage.AppendLine(error.ErrorMessage);
                        if (error.ErrorMessage.Contains("运输方式代号"))
                        {
                            item.ErrorType = 1;
                            ErrorType++;
                        }
                        else if (error.ErrorMessage.Contains("国家代码"))
                        {
                            item.ErrorType = 2;
                            ErrorType++;
                        }
                    }
                    if (ErrorType > 1)
                    {
                        item.ErrorType = 3;
                    }
                }

                foreach (var product in item.ApplicationInfos)
                {
                    //ValidationResult productResults = !sysConfig.NLPOSTMethodCode.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Contains(item.ShippingMethodCode.ToUpper()) ? productValidator.Validate(product) : new ProductNlpostModelValidator().Validate(product);
                    ValidationResult productResults;
                    if (sysConfig.NLPOSTMethodCode.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                 .ToList()
                                 .Contains(item.ShippingMethodCode.ToUpper()))
                    {
                        productResults = new ProductNlpostModelValidator().Validate(product);
                    }
                    else if (sysConfig.LithuaniaMethodCode.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                .ToList()
                                .Contains(item.ShippingMethodCode.ToUpper()))
                    {
                        int qty;
                        decimal unitprice, unitweight;
                        if (!Int32.TryParse(product.Qty, out qty))
                        {
                            qty = 0;
                        }
                        if (!decimal.TryParse(product.UnitPrice, out unitprice))
                        {
                            unitprice = 0;
                        }
                        if (!decimal.TryParse(product.UnitWeight, out unitweight))
                        {
                            unitweight = 0;
                        }
                        productResults = new ApplicationSfModelValidator().Validate(new ApplicationSfModel()
                        {
                            ApplicationName = product.ApplicationName,
                            Qty = qty,
                            UnitPrice = unitprice,
                            UnitWeight = unitweight,
                        });
                    }
                    else
                    {
                        productResults = productValidator.Validate(product);
                    }
                    if (productResults.IsValid) continue;
                    item.IsValid = false;
                    if (item.ErrorType == 0)
                    {
                        item.ErrorType = 4;
                    }
                    IList<ValidationFailure> productFailures = productResults.Errors;
                    foreach (var error in productFailures)
                    {
                        item.ErrorMessage.AppendLine(error.ErrorMessage);
                    }
                }


            }

            return list;
        }

        /// <summary>
        /// 中美专线 上传验证 
        /// Add By zhengsong
        /// Time:2014-08-08
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<OrderModel> ValidationOrderSinoUSModel(List<OrderModel> list)
        {
            var validator = new OrderModelValidator();
            var productValidator = new ProductModelValidator();
            var shippingMethods = _freightService.GetShippingMethods("", true);
            //var insureList = _insuredCalculationService.GetList();
            List<string> customerNumber = new List<string>();
            //List<string> trackingNumber = new List<string>();
            list.ForEach(p => customerNumber.Add(p.CustomerOrderNumber.ToUpperInvariant()));
            //list.ForEach(p =>
            //{
            //    if (!p.TrackingNumber.IsNullOrWhiteSpace())
            //    {
            //        trackingNumber.Add(p.TrackingNumber.ToUpperInvariant());
            //    }

            //});
            List<string> customerNumbers = new List<string>();
            List<string> trackingNumbers = new List<string>();
            //trackingNumbers = _customerOrderService.GetCustomerOrderInfoByTrack(trackingNumber);
            customerNumbers = _customerOrderService.GetCustomerOrderInfos(customerNumber);
            foreach (var item in list)
            {
                ValidationResult results = validator.Validate(item);
                if (item.CustomerOrderNumber != null &&
                    list.FindAll(o => o.CustomerOrderNumber.ToUpper() == item.CustomerOrderNumber.ToUpper()).Count > 1)
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine(string.Format("存在相同的订单号[{0}]",
                                                                     item.CustomerOrderNumber));
                }
                else if (item.CustomerOrderNumber != null)
                {

                    if (customerNumbers.Any(p => p == item.CustomerOrderNumber.ToUpperInvariant()))
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine(string.Format("订单号[{0}]已存在",
                                                                         item.CustomerOrderNumber));
                    }
                }

                var shipping = shippingMethods.FirstOrDefault(p => p.ShippingMethodId == sysConfig.SpecialShippingMethodId);
                if (!sysConfig.SinoUSShippingMethodCode.Split(',').ToList().Contains(item.ShippingMethodCode.Trim().ToUpper()))
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine("改运输方式不是中美专线");
                }

                if (item.ShippingAddress.Trim().Length > 35)
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine("地址1长度超过35个字符");
                }
                else if (item.ShippingAddress1.Length > 35)
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine("地址2长度超过35个字符");
                }
                if (item.CustomerOrderNumber.Trim().Length > 35)
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine("订单号长度超过35个字符");
                }

                if (item.ShippingCompany.Trim().Length > 35)
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine("公司名称长度超过35个字符");
                }

                if (item.ShippingFirstName.Trim().Length > 35)
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine("收货人名称长度超过35个字符");
                }

                if (item.ShippingState.Trim().Length > 2)
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine("州/省长度超过2个字符");
                }

                if (item.ShippingCity.Trim().Length > 35)
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine("城市长度超过35个字符");
                }

                if (item.ShippingZip.Trim().Length > 9 || item.ShippingZip.Trim().Length < 5)
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine("邮编长度为5-9个字符");
                }

                if (item.CountryCode.Trim().Length > 2)
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine("国家简码超过2个字符");
                }

                if (item.Weight == null || item.Weight <= 0)
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine("重量必须大于零");
                }

                if (shipping != null && item.ShippingMethodCode.Trim() == shipping.Code)
                {
                    if (!string.IsNullOrWhiteSpace(item.ShippingZip))
                    {
                        if (!Tools.CheckPostCode(item.ShippingZip))
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine("收件人邮编为6位纯数字且只能以1 2 3 4 6开头");
                        }
                    }
                    else
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人邮编不能为空");
                    }
                    if (!string.IsNullOrWhiteSpace(item.ShippingPhone))
                    {
                        if (!Tools.CheckShippingPhone(item.ShippingPhone))
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine("收件人电话号码最长不能超过11位数字");
                        }
                    }
                    else
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人联系电话不能为空");
                    }
                }
                //if (!string.IsNullOrWhiteSpace(item.TrackingNumber) &&
                //    list.FindAll(o => o.TrackingNumber.ToUpper() == item.TrackingNumber.ToUpperInvariant()).Count > 1)
                //{
                //    item.IsValid = false;
                //    item.ErrorType = 4;
                //    item.ErrorMessage.AppendLine(string.Format("存在相同的跟踪号[{0}]",
                //                                                     item.TrackingNumber));
                //}
                //else if (!string.IsNullOrWhiteSpace(item.TrackingNumber) && trackingNumbers.Any(p => p.ToUpper() == item.TrackingNumber.ToUpperInvariant()))
                //{
                //    item.IsValid = false;
                //    item.ErrorType = 4;
                //    item.ErrorMessage.AppendLine(string.Format("跟踪号[{0}]已存在",
                //                                                     item.TrackingNumber));
                //}
                int x = 1;
                if (item.ApplicationInfos.Count < 1)
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine("没有订单明细");
                }
                else
                {
                    foreach (var row in item.ApplicationInfos)
                    {
                        if (string.IsNullOrWhiteSpace(row.ApplicationName))
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项信息不完整", x));
                        }
                        x++;
                    }
                }
                //#region 判断保险类型
                //if (string.IsNullOrWhiteSpace(item.InsuredValue))
                //{
                //    item.IsInsured = false;
                //}
                //else
                //{
                //    string[] id = item.InsuredValue.Split('_');
                //    int idd;
                //    if (int.TryParse(id[0], out idd))
                //    {
                //        if (insureList.Any(p => p.InsuredID == int.Parse(id[0])))
                //        {
                //            item.IsInsured = true;
                //            item.InsuredID = id[0];
                //            if (item.InsuredID == "2")
                //            {
                //                if (!string.IsNullOrWhiteSpace(item.InsureAmountValue))
                //                {
                //                    decimal amount;
                //                    if (decimal.TryParse(item.InsureAmountValue, out amount))
                //                    {
                //                        if (amount < 1)
                //                        {
                //                            item.IsValid = false;
                //                            item.ErrorType = 4;
                //                            item.ErrorMessage.AppendLine("投保金额必须大于0");
                //                        }
                //                        else
                //                        {
                //                            item.InsureAmount = amount;
                //                        }
                //                    }
                //                    else
                //                    {
                //                        item.IsValid = false;
                //                        item.ErrorType = 4;
                //                        item.ErrorMessage.AppendLine("投保金额必须位数字");
                //                    }
                //                }
                //                else
                //                {
                //                    item.IsValid = false;
                //                    item.ErrorType = 4;
                //                    item.ErrorMessage.AppendLine("投保金额不能为空");
                //                }
                //            }
                //            else
                //            {
                //                item.InsureAmount = decimal.Parse(insureList.Find(p => p.InsuredID == int.Parse(item.InsuredID))
                //                               .InsuredCalculation1);
                //            }
                //        }
                //        else
                //        {
                //            item.IsValid = false;
                //            item.ErrorType = 4;
                //            item.ErrorMessage.AppendLine("保险类型不存在");
                //        }
                //    }
                //    else
                //    {
                //        item.IsValid = false;
                //        item.ErrorType = 4;
                //        item.ErrorMessage.AppendLine("保险类型不存在");
                //    }
                //}
                //#endregion
                bool validationSucceeded = results.IsValid;
                IList<ValidationFailure> failures = results.Errors;
                Console.WriteLine("******************{0}**********************".FormatWith(item.CustomerOrderNumber));
                if (!validationSucceeded)
                {
                    item.IsValid = false;
                    int ErrorType = 0;
                    foreach (var error in failures)
                    {

                        item.ErrorMessage.AppendLine(error.ErrorMessage);
                        if (error.ErrorMessage.Contains("运输方式代号"))
                        {
                            item.ErrorType = 1;
                            ErrorType++;
                        }
                        else if (error.ErrorMessage.Contains("国家代码"))
                        {
                            item.ErrorType = 2;
                            ErrorType++;
                        }
                    }
                    if (ErrorType > 1)
                    {
                        item.ErrorType = 3;
                    }
                }

                foreach (var product in item.ApplicationInfos)
                {
                    ValidationResult productResults = productValidator.Validate(product);

                    if (productResults.IsValid) continue;
                    item.IsValid = false;
                    if (item.ErrorType == 0)
                    {
                        item.ErrorType = 4;
                    }
                    IList<ValidationFailure> productFailures = productResults.Errors;
                    foreach (var error in productFailures)
                    {
                        item.ErrorMessage.AppendLine(error.ErrorMessage);
                    }
                }


            }

            return list;
        }

        /// <summary>
        /// 验证Excle数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<OrderModel> ValidationOrderPackageModel(List<OrderModel> list)
        {
            var validator = new OrderModelValidator();
            var productValidator = new ProductModelValidator();
            var shippingMethods = _freightService.GetShippingMethods("", true);
            var insureList = _insuredCalculationService.GetList();
            List<string> customerNumber = new List<string>();
            List<string> trackingNumber = new List<string>();
            list.ForEach(p => customerNumber.Add(p.CustomerOrderNumber.ToUpperInvariant()));
            list.ForEach(p => trackingNumber.Add(p.TrackingNumber.ToUpperInvariant()));
            List<string> customerNumbers = new List<string>();
            List<string> trackingNumbers = new List<string>();
            trackingNumbers = _customerOrderService.GetCustomerOrderInfoByTrack(trackingNumber);
            customerNumbers = _customerOrderService.GetCustomerOrderInfos(customerNumber);
            foreach (var item in list)
            {
                //转换大小写
                item.ShippingMethodCode = item.ShippingMethodCode.ToUpperInvariant();
                ValidationResult results;
                if(sysConfig.NLPOSTMethodCode.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries)
                              .ToList()
                              .Contains(item.ShippingMethodCode.ToUpper()))
                {
                    results = new OrderNlPostModelValidator().Validate(item);
                }
                else if (sysConfig.LithuaniaMethodCode.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries)
                                  .ToList()
                                  .Contains(item.ShippingMethodCode.ToUpper()))
                {
                    results= new OrderSfModelValidator().Validate(new OrderSfModel()
                    {
                        ShippingName = item.ShippingFirstName + item.ShippingLastName,
                        ShippingAddress = item.ShippingAddress + item.ShippingAddress1 + item.ShippingAddress2,
                        ShippingCity = item.ShippingCity,
                        ShippingCompany = item.ShippingCompany,
                        ShippingPhone = item.ShippingPhone,
                        ShippingState = item.ShippingState,
                        ShippingTel = item.ShippingPhone,
                        ShippingZip = item.ShippingZip,
                        CountryCode = item.CountryCode
                    });
                }
                else
                {
                    results = validator.Validate(item);
                }
                if (sysConfig.SinoUSShippingMethodCode.Split(',').ToList().Contains(item.ShippingMethodCode.Trim().ToUpperInvariant()))
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine("中美专线订单请从专属通道上传！");
                    continue;
                }

                if (item.CustomerOrderNumber != null &&
                    list.FindAll(o => o.CustomerOrderNumber.ToUpperInvariant() == item.CustomerOrderNumber.ToUpperInvariant()).Count > 1)
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine(string.Format("存在相同的订单号[{0}]",
                                                                     item.CustomerOrderNumber));
                }
                else if (item.CustomerOrderNumber != null)
                {
                    if (customerNumbers.Any(p => p.ToUpperInvariant() == item.CustomerOrderNumber.ToUpperInvariant()))
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine(string.Format("订单号[{0}]已存在",
                                                                         item.CustomerOrderNumber));
                    }
                }

                if (item.Width == null || item.Width <= 0)
                {
                    item.Width = 1;
                }
                if (item.Height == null || item.Height <= 0)
                {
                    item.Height = 1;
                }
                if (item.Length == null || item.Length <= 0)
                {
                    item.Length = 1;
                }


                #region 中邮挂号福州
                if (item.ShippingMethodCode != null && (item.ShippingMethodCode.Trim().ToUpperInvariant() == "CNPOST-FZ" || item.ShippingMethodCode.Trim().ToUpperInvariant() == "CNPOST-FYB"))
                {
                    if (item.CustomerOrderNumber != null && item.CustomerOrderNumber.Length > 30)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("订单号长度必须小于等于30");
                    }
                    //国家两位
                    if (item.CountryCode != null && item.CountryCode.Length != 2)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("国家简码必须是两位");
                    }
                    //收件人州或省
                    if (item.ShippingState != null && item.ShippingState.Length > 50)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人省或州长度不能超过50");
                    }
                    //收件人城市
                    if (item.ShippingCity != null && item.ShippingCity.Length > 50)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人城市长度不能超过50");
                    }
                    //收件人地址
                    string address = "";
                    if (item.ShippingAddress != null)
                    {
                        address += item.ShippingAddress;
                    }
                    if (item.ShippingAddress1 != null)
                    {
                        address += item.ShippingAddress1;
                    }
                    if (item.ShippingAddress2 != null)
                    {
                        address += item.ShippingAddress2;
                    }
                    if (address.Length > 120)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人地址长度不能超过120");
                    }
                    //收件人邮编
                    if (item.ShippingZip != null && item.ShippingZip.Length > 12)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人邮编长度不能超过12");
                    }
                    //收件人名字
                    string name = "";
                    if (item.ShippingFirstName != null)
                    {
                        name += item.ShippingFirstName;
                    }
                    if (item.ShippingLastName != null)
                    {
                        name += item.ShippingLastName;
                    }
                    if (string.IsNullOrWhiteSpace(name) || name.Length > 64)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人名字长度为1-64字符");
                    }
                    //收件人电话
                    if (item.ShippingPhone != null && item.ShippingPhone.Length > 20)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人电话长度不能超过20");
                    }
                    //发件人省份
                    if (item.SenderState != null && item.SenderState.Length > 20)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("发件人州省长度不能超过20");
                    }
                    //发件人城市
                    if (item.SenderCity != null && item.SenderCity.Length > 64)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("发件人城市长度不能超过64");
                    }
                    //发件人街道
                    if (item.SenderAddress != null && item.SenderAddress.Length > 120)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("发件人地址长度不能超过120");
                    }
                    //发件人邮编
                    if (item.SenderZip != null && item.SenderZip.Length > 6)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("发件人邮编长度不能超过6");
                    }
                    //发件人名字
                    string senderName = "";
                    if (item.SenderFirstName != null)
                    {
                        senderName += item.SenderFirstName;
                    }
                    if (item.SenderLastName != null)
                    {
                        senderName += item.SenderLastName;
                    }
                    if (senderName.Length > 20)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("发件人名字长度不能超过20");
                    }
                    //发件人电话
                    if (item.SenderPhone != null && item.SenderPhone.Length > 20)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("发件人电话长度不能超过20");
                    }
                }
                #endregion

                #region DHL 上传验证

                if (item.ShippingMethodCode != null && (item.ShippingMethodCode.Trim().ToUpperInvariant() == "HKDHL" || item.ShippingMethodCode.Trim().ToUpperInvariant() == "DHLCN" || item.ShippingMethodCode.Trim().ToUpperInvariant() == "DHLSG"))
                {
                    if (item.InsureAmount != null && item.InsureAmount.ToString().Length > 14)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("保险金额长度不能超过14个字符");
                    }
                    else if (item.InsureAmount != null && !Regex.IsMatch(item.InsureAmount.ToString(), "^[0-9]+[.]{0,1}[0-9]{0,2}$"))
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("保险金额有数字组成，最多保留两位小数");
                    }

                    if (!string.IsNullOrWhiteSpace(item.ShippingCompany) &&  item.ShippingCompany.Length > 35)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人公司长度为0-35个字");
                    }
                    string address = "";
                    if (item.ShippingAddress != null)
                    {
                        address += item.ShippingAddress;
                    }
                    if (item.ShippingAddress1 != null)
                    {
                        address += item.ShippingAddress1;
                    }
                    if (item.ShippingAddress2 != null)
                    {
                        address += item.ShippingAddress2;
                    }
                    try
                    {

                        if (!string.IsNullOrWhiteSpace(address) && address.StringSplitLengthWords(35).Count > 2)
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine("收件人地址超长");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.ToString());
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人地址格式错误");
                    }

                    if (string.IsNullOrWhiteSpace(item.ShippingCity) || item.ShippingCity.Length > 35)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人城市不能为空或超过35个字符");
                    }

                    if (string.IsNullOrWhiteSpace(item.ShippingState) || item.ShippingState.Length > 35)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人州/省长度为1-35个字符");
                    }
                    if (string.IsNullOrWhiteSpace(item.ShippingZip) || item.ShippingZip.Length > 12)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人邮编长度为1-12个字符");
                    }

                    if (!string.IsNullOrWhiteSpace(item.ShippingTaxId) &&
                        item.ShippingTaxId.Length > 20)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人税号不能超过20字符");
                    }
                    string name = "";
                    if (item.ShippingFirstName != null)
                    {
                        name += item.ShippingFirstName;
                    }
                    if (item.ShippingLastName != null)
                    {
                        name += item.ShippingLastName;
                    }
                    if (string.IsNullOrWhiteSpace(name) || name.Length > 35)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人姓名不能为空或超过35个字符");
                    }

                    if (string.IsNullOrWhiteSpace(item.ShippingPhone))
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人电话不能为空");
                    }
                    else if (item.ShippingPhone.Length > 25)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人电话不能超过25个字符");
                    }
                }

                #endregion

                #region EUB 上传验证
                if (item != null &&
                    (item.ShippingMethodCode.Trim().ToUpperInvariant() == "EUB_CS" || item.ShippingMethodCode.Trim().ToUpperInvariant() == "EUB-SZ" ||
                     item.ShippingMethodCode.Trim().ToUpperInvariant() == "EUB-FZ"))
                {
                    if (item.CustomerOrderNumber != null && (item.CustomerOrderNumber.Length > 32 || item.CustomerOrderNumber.Length < 4))
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("订单号长度必须为4-32个字符");
                    }
                    string name = "";
                    if (item.ShippingFirstName != null)
                    {
                        name += item.ShippingFirstName;
                    }
                    if (item.ShippingLastName != null)
                    {
                        name += item.ShippingLastName;
                    }
                    if (string.IsNullOrWhiteSpace(name) || name.Length > 256)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人姓名长度为1-256个字符");
                    }
                    if (string.IsNullOrWhiteSpace(item.ShippingCity) ||
                        item.ShippingCity.Length > 128)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人城市不能为空或者超过128个字符");
                    }

                    if (string.IsNullOrWhiteSpace(item.ShippingState) || item.ShippingState.Length > 128)
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人州长度1-128个字符");
                    }

                    if (item.ShippingZip != null)
                    {
                        if (item.ShippingZip.Length > 16)
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine("收件人邮编不能超过16个字符");
                        }
                        else
                        {
                            switch (item.CountryCode.ToUpperInvariant())
                            {
                                case "US":
                                    if (!Regex.IsMatch(item.ShippingZip, "^(^[0-9]{5}-[0-9]{4}$)|(^[0-9]{5}-[0-9]{5}$)|(^[0-9]{5}$)$"))
                                    {
                                        item.IsValid = false;
                                        item.ErrorType = 4;
                                        item.ErrorMessage.AppendLine("邮编不合法");
                                    }
                                    break;
                                case "AU":
                                    if (!Regex.IsMatch(item.ShippingZip, "^[0-9]{4}$"))
                                    {
                                        item.IsValid = false;
                                        item.ErrorType = 4;
                                        item.ErrorMessage.AppendLine("邮编不合法");
                                    }
                                    break;
                                case "CA":
                                    if (!Regex.IsMatch(item.ShippingZip, "^(^[A-Za-z][0-9][A-Za-z][ ][0-9][A-Za-z][0-9]$)|(^[A-Za-z][0-9][A-Za-z][0-9][A-Za-z][0-9]$)$"))
                                    {
                                        item.IsValid = false;
                                        item.ErrorType = 4;
                                        item.ErrorMessage.AppendLine("邮编不合法");
                                    }
                                    break;
                                case "GB":
                                    if (!Regex.IsMatch(item.ShippingZip, "^[A-Za-z0-9]{2,4} [A-Za-z0-9]{3}$"))
                                    {
                                        item.IsValid = false;
                                        item.ErrorType = 4;
                                        item.ErrorMessage.AppendLine("邮编不合法");
                                    }
                                    break;
                                case "FR":
                                    if (!Regex.IsMatch(item.ShippingZip, "^[0-9]{5}$"))
                                    {
                                        item.IsValid = false;
                                        item.ErrorType = 4;
                                        item.ErrorMessage.AppendLine("邮编不合法");
                                    }
                                    break;
                                case "RU":
                                    if (!Regex.IsMatch(item.ShippingZip, "^[0-9]{6}$"))
                                    {
                                        item.IsValid = false;
                                        item.ErrorType = 4;
                                        item.ErrorMessage.AppendLine("邮编不合法");
                                    }
                                    break;
                            }
                        }
                    }
                    else if (item.CountryCode.ToUpperInvariant() != "HK")
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("邮编不能为空");
                    }

                }
                #endregion

                if (item.Weight == null || item.Weight <= 0)
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine("包裹重量必须大于零");
                }

                //收货国家俄罗斯，邮编不能为空
                if (item.ShippingMethodCode.Trim().ToUpperInvariant() == "LTPOST")
                {
                    if (item.CountryCode.ToUpperInvariant() == "RU")
                    {
                        if (string.IsNullOrWhiteSpace(item.ShippingZip))
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine("邮编不能为空");
                        }
                    }
                }

                var shipping = shippingMethods.FirstOrDefault(p => p.ShippingMethodId == sysConfig.SpecialShippingMethodId);
                if (shipping != null && item.ShippingMethodCode.Trim() == shipping.Code)
                {
                    if (!string.IsNullOrWhiteSpace(item.ShippingZip))
                    {
                        if (!Tools.CheckPostCode(item.ShippingZip))
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine("收件人邮编为6位纯数字且只能以1 2 3 4 6开头");
                        }
                    }
                    else
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("收件人邮编不能为空");
                    }
                    //if (!string.IsNullOrWhiteSpace(item.ShippingPhone))
                    //{
                    //    if (!Tools.CheckShippingPhone(item.ShippingPhone))
                    //    {
                    //        item.IsValid = false;
                    //        item.ErrorType = 4;
                    //        item.ErrorMessage.AppendLine("收件人电话号码最长不能超过11位数字");
                    //    }
                    //}
                    //else
                    //{
                    //    item.IsValid = false;
                    //    item.ErrorType = 4;
                    //    item.ErrorMessage.AppendLine("收件人联系电话不能为空");
                    //}
                }
                if (!string.IsNullOrWhiteSpace(item.TrackingNumber) &&
                    list.FindAll(o => o.TrackingNumber == item.TrackingNumber).Count > 1)
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine(string.Format("存在相同的跟踪号[{0}]",
                                                                     item.TrackingNumber));
                }
                else if (!string.IsNullOrWhiteSpace(item.TrackingNumber) && trackingNumbers.Any(p => p.ToUpper() == item.TrackingNumber.ToUpperInvariant()))
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine(string.Format("跟踪号[{0}]已存在",
                                                                     item.TrackingNumber));
                }
                int x = 1;
                if (item.ApplicationInfos.Count < 1)
                {
                    item.IsValid = false;
                    item.ErrorType = 4;
                    item.ErrorMessage.AppendLine("没有订单明细");
                }
                else
                {
                    foreach (var row in item.ApplicationInfos)
                    {
                        if (string.IsNullOrWhiteSpace(row.ApplicationName))
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项申报名称不能为空", x));
                        }
                        //福州邮政申报信息判断
                        if (item.ShippingMethodCode.Trim().ToUpperInvariant() == "CNPOST-FZ" || item.ShippingMethodCode.Trim().ToUpperInvariant() == "CNPOSTP_FZ" || item.ShippingMethodCode.Trim().ToUpperInvariant() == "CNPOST-FYB")
                        {
                            if (row.ApplicationName.Length > 60)
                            {
                                item.IsValid = false;
                                item.ErrorType = 4;
                                item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项申报名称超过字符长度", x));
                            }

                            if (string.IsNullOrWhiteSpace(row.PickingName) || row.PickingName.Length>60)
                            {
                                item.IsValid = false;
                                item.ErrorType = 4;
                                item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项申报中文名称不能为空或超过字符长度", x));
                            }
                            if (string.IsNullOrWhiteSpace(row.UnitWeight) || row.UnitWeight == "0")
                            {
                                item.IsValid = false;
                                item.ErrorType = 4;
                                item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项申报重量为空", x));
                            }
                        }

                        //DHL验证申报信息
                        if (item.ShippingMethodCode != null &&
                            (item.ShippingMethodCode.Trim().ToUpperInvariant() == "HKDHL" ||
                             item.ShippingMethodCode.Trim().ToUpperInvariant() == "DHLCN" ||
                             item.ShippingMethodCode.Trim().ToUpperInvariant() == "DHLSG"))
                        {
                            if (string.IsNullOrWhiteSpace(row.ApplicationName) || row.ApplicationName.Length > 60)
                            {
                                item.IsValid = false;
                                item.ErrorType = 4;
                                item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项申报英文名称不能为空或超过字符长度", x));
                            }
                            else
                            {
                                if (Regex.IsMatch(row.ApplicationName,
                                                   @"[\~]{1}|[\@]{1}|[\#]{1}|[\$]{1}|[\￥]{1}|[\%]{1}|[\^]{1}|[\&]{1}|[\*]{1}|[\(]{1}|[\)]{1}|[\u4e00-\u9fa5]+"))
                                {
                                    item.IsValid = false;
                                    item.ErrorType = 4;
                                    item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项申报英文名称不能包含特殊字符和汉字", x));

                                }
                            }
                            if (row.UnitWeight == null || row.UnitWeight == "0")
                            {
                                item.IsValid = false;
                                item.ErrorType = 4;
                                item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项列申报重量不能为0", x));

                            }
                        }

                        //EUB 申报信息验证
                        if (item != null && (item.ShippingMethodCode.Trim().ToUpperInvariant() == "EUB_CS" || item.ShippingMethodCode.Trim().ToUpperInvariant() == "EUB-SZ" || item.ShippingMethodCode.Trim().ToUpperInvariant() == "EUB-FZ"))
                        {

                            if (string.IsNullOrWhiteSpace(row.PickingName) || row.PickingName.Length > 64)
                            {
                                item.IsValid = false;
                                item.ErrorType = 4;
                                item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项申报中文名称不能为空或超长", x));
                            }
                            else
                            {
                                if (!Regex.IsMatch(row.PickingName, @"[\u4e00-\u9fa5]+[A-Za-z0-9]*[\s\S]*[\u4e00-\u9fa5]+"))
                                {
                                    item.IsValid = false;
                                    item.ErrorType = 4;
                                    item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项申报中文名称至少包含2个汉子", x));
                                }
                            }
                            if (string.IsNullOrWhiteSpace(row.ApplicationName) || row.ApplicationName.Length > 128)
                            {
                                item.IsValid = false;
                                item.ErrorType = 4;
                                item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项申报名称不能为空或超长", x));
                            }

                            if (string.IsNullOrWhiteSpace(row.UnitWeight) || row.UnitWeight=="0")
                            {
                                item.IsValid = false;
                                item.ErrorType = 4;
                                item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项申报重量不能为空", x));
                            }
                        }

                        if (string.IsNullOrWhiteSpace(row.HSCode) &&
                                 (item.ShippingMethodCode.Trim().ToUpperInvariant() ==
                                  sysConfig.DDPRegisterShippingMethodCode ||
                                  item.ShippingMethodCode.Trim().ToUpperInvariant() == sysConfig.DDPShippingMethodCode))
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项海关编码不能为空", x));
                        }
                        if (string.IsNullOrWhiteSpace(row.ProductUrl) &&
                                 (item.ShippingMethodCode.Trim().ToUpperInvariant() ==
                                  sysConfig.DDPRegisterShippingMethodCode ||
                                  item.ShippingMethodCode.Trim().ToUpperInvariant() == sysConfig.DDPShippingMethodCode))
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项销售链接不能为空", x));
                        }
                        if (string.IsNullOrWhiteSpace(row.Remark) &&
                                 (item.ShippingMethodCode.Trim().ToUpperInvariant() ==
                                  sysConfig.DDPRegisterShippingMethodCode ||
                                  item.ShippingMethodCode.Trim().ToUpperInvariant() == sysConfig.DDPShippingMethodCode))
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine(string.Format("申报信息第{0}项备注不能为空", x));
                        }
                        x++;
                    }
                }
                #region 判断保险类型
                if (string.IsNullOrWhiteSpace(item.InsuredValue))
                {
                    item.IsInsured = false;
                }
                else
                {
                    string[] id = item.InsuredValue.Split('_');
                    int idd;
                    if (int.TryParse(id[0], out idd))
                    {
                        if (insureList.Any(p => p.InsuredID == int.Parse(id[0])))
                        {
                            item.IsInsured = true;
                            item.InsuredID = id[0];
                            if (item.InsuredID == "2")
                            {
                                if (!string.IsNullOrWhiteSpace(item.InsureAmountValue))
                                {
                                    decimal amount;
                                    if (decimal.TryParse(item.InsureAmountValue, out amount))
                                    {
                                        if (amount < 1)
                                        {
                                            item.IsValid = false;
                                            item.ErrorType = 4;
                                            item.ErrorMessage.AppendLine("投保金额必须大于0");
                                        }
                                        else
                                        {
                                            item.InsureAmount = amount;
                                        }
                                    }
                                    else
                                    {
                                        item.IsValid = false;
                                        item.ErrorType = 4;
                                        item.ErrorMessage.AppendLine("投保金额必须位数字");
                                    }
                                }
                                else
                                {
                                    item.IsValid = false;
                                    item.ErrorType = 4;
                                    item.ErrorMessage.AppendLine("投保金额不能为空");
                                }
                            }
                            else
                            {
                                item.InsureAmount = decimal.Parse(insureList.Find(p => p.InsuredID == int.Parse(item.InsuredID))
                                               .InsuredCalculation1);
                            }
                        }
                        else
                        {
                            item.IsValid = false;
                            item.ErrorType = 4;
                            item.ErrorMessage.AppendLine("保险类型不存在");
                        }
                    }
                    else
                    {
                        item.IsValid = false;
                        item.ErrorType = 4;
                        item.ErrorMessage.AppendLine("保险类型不存在");
                    }
                }
                #endregion
                bool validationSucceeded = results.IsValid;
                IList<ValidationFailure> failures = results.Errors;
                Console.WriteLine("******************{0}**********************".FormatWith(item.CustomerOrderNumber));
                if (!validationSucceeded)
                {
                    item.IsValid = false;
                    int ErrorType = 0;
                    foreach (var error in failures)
                    {

                        item.ErrorMessage.AppendLine(error.ErrorMessage);
                        if (error.ErrorMessage.Contains("运输方式代号"))
                        {
                            item.ErrorType = 1;
                            ErrorType++;
                        }
                        else if (error.ErrorMessage.Contains("国家代码"))
                        {
                            item.ErrorType = 2;
                            ErrorType++;
                        }
                    }
                    if (ErrorType > 1)
                    {
                        item.ErrorType = 3;
                    }
                }

                foreach (var product in item.ApplicationInfos)
                {
                    ValidationResult productResults;
                    if (sysConfig.NLPOSTMethodCode.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries)
                                 .ToList()
                                 .Contains(item.ShippingMethodCode.ToUpper()))
                    {
                        productResults=new ProductNlpostModelValidator().Validate(product);
                    }
                    else if (sysConfig.LithuaniaMethodCode.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                .ToList()
                                .Contains(item.ShippingMethodCode.ToUpper()))
                    {
                        int qty;
                        decimal unitprice, unitweight;
                        if (!Int32.TryParse(product.Qty, out qty))
                        {
                            qty = 0;
                        }
                        if (!decimal.TryParse(product.UnitPrice, out unitprice))
                        {
                            unitprice = 0;
                        }
                        if (!decimal.TryParse(product.UnitWeight, out unitweight))
                        {
                            unitweight = 0;
                        }
                        productResults = new ApplicationSfModelValidator().Validate(new ApplicationSfModel()
                            {
                                ApplicationName = product.ApplicationName,
                                Qty = qty,
                                UnitPrice = unitprice,
                                UnitWeight = unitweight,
                            });
                    }
                    else
                    {
                        productResults = productValidator.Validate(product);
                    }
                        //!sysConfig.NLPOSTMethodCode.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries)
                        //          .ToList()
                        //          .Contains(item.ShippingMethodCode.ToUpper())
                        //    ? productValidator.Validate(product)
                        //    : new ProductNlpostModelValidator().Validate(product);

                    if (productResults.IsValid) continue;
                    item.IsValid = false;
                    if (item.ErrorType == 0)
                    {
                        item.ErrorType = 4;
                    }
                    IList<ValidationFailure> productFailures = productResults.Errors;
                    foreach (var error in productFailures)
                    {
                        item.ErrorMessage.AppendLine(error.ErrorMessage);
                    }
                }


            }

            return list;
        }

        public string GetValue(IRow row, ref int num)
        {
            string result = "";
            //var cell = row.GetCell(num);
            //if (cell != null)
            //{
            //    result = cell.ToString();
            //}

            var cell = row.GetCell(num);
            if (cell != null)
            {
                if (cell.CellType == CellType.Numeric)
                {
                    if (DateUtil.IsCellDateFormatted(row.GetCell(num)))//datetime
                    {
                        result = DateUtil.GetJavaDate(row.GetCell(num).NumericCellValue).ToString();
                    }
                    else
                    {
                        result = row.GetCell(num).NumericCellValue.ToString();
                    }
                }
                else if (cell.CellType == CellType.Formula)
                {
                    result = row.GetCell(num).NumericCellValue.ToString();
                }
                else
                {
                    result = row.GetCell(num).StringCellValue;
                }
            }

            num++;

            return result;
        }

        public string SaveFile(HttpPostedFileBase file)
        {
            if (file != null)
            {
                try
                {
                    // 文件上传后的保存路径
                    string filePath = sysConfig.TemporaryPath;

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

        public ActionResult Edit(int id)
        {
            InitAdd();
            var entity = _customerOrderService.Get(id);

            var model = entity.ToModel<CustomerOrderInfoModel>();
            var applicationTypelist = new List<SelectListItem>();
            CustomerOrder.GetApplicationTypeList().ForEach(c =>
                    applicationTypelist.Add(new SelectListItem
                    {
                        Value = c.ValueField,
                        Text = c.TextField,
                    })
               );
            entity.ShippingInfo.ToModel(model);
            entity.SenderInfo.ToModel(model);
            model.CountryCode = entity.ShippingInfo.CountryCode;
            model.ReturnUrl = Request.QueryString["returnurl"];
            model.AppLicationTypes = applicationTypelist;
            model.PackageNumberValue = model.PackageNumber.ToString();
            var productDetail = entity.ApplicationInfos.ToModelAsCollection<ApplicationInfo, ApplicationInfoModel>();

            if (productDetail != null && productDetail.Count > 0)
                model.ProductDetail = ToJson(productDetail);
            model.InsuredCalculationId = model.InsuredID.ToString();
            model.AppLicationTypeId = model.AppLicationType.ToString();
            model.InsureAmountValue = model.InsureAmount.ToString();
            if (model.IsInsured)
            {
                model.InsuredValue = model.InsuredID.ToString() + "_" +
                                 _insuredCalculationService.GetbyInsuredID(model.InsuredID).InsuredCalculation1;
                model.InsureAmountValue = model.InsureAmount.ToString();
            }

            return View(model);
        }

        /// <summary>
        /// 订单编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(CustomerOrderInfoModel model)
        {
            InitAdd();
            var applicationTypelist = new List<SelectListItem>();
            CustomerOrder.GetApplicationTypeList().ForEach(c =>
                    applicationTypelist.Add(new SelectListItem
                    {
                        Value = c.ValueField,
                        Text = c.TextField,
                    })
               );
            model.AppLicationTypes = applicationTypelist;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var info = _customerOrderService.Get(model.CustomerOrderID);

            //申报信息
            var js = new JavaScriptSerializer();
            var receiver = js.Deserialize<List<ApplicationInfoModel>>(model.ProductDetail);

			//已提交已拦截，修改件数(其余不可编辑) add by yungchu
	        bool isCanUpdatePackNumber=  (info.Status == CustomerOrder.StatusEnum.Submitted.GetStatusValue() && info.IsHold);	

            //提交失败订单编辑时可保存
            if (model.SubmitFailFlag == null)
            {
				
                if (!(info.Status == CustomerOrder.StatusEnum.None.GetStatusValue() ||
				 info.Status == CustomerOrder.StatusEnum.OK.GetStatusValue() || isCanUpdatePackNumber))
                {
                    return View(model);
                }
            }

            int appLicationType = 4;
            int.TryParse(model.AppLicationTypeId, out appLicationType);
            model.AppLicationType = appLicationType;
            var insureList = _insuredCalculationService.GetList();
            int packageNumber = 1;
            if (string.IsNullOrWhiteSpace(model.PackageNumberValue))
            {
                model.PackageNumber = 1;
            }

            if (model.Width == null || model.Width <= 0)
            {
                model.Width = 1;
            }
            if (model.Height == null || model.Height <= 0)
            {
                model.Height = 1;
            }
            if (model.Length == null || model.Length <= 0)
            {
                model.Length = 1;
            }

            var shippingMethod = _freightService.GetShippingMethod(model.ShippingMethodId);//中美专线
            if (shippingMethod != null && (sysConfig.SinoUSShippingMethodCode.Split(',').ToList().Contains(shippingMethod.Code)))
            {
                if (model.ShippingAddress.Length > 35)
                {
                    ModelState.AddModelError("ShippingAddress", "长度超过35个字符".FormatWith(model.ShippingAddress));
                    return View(model);
                }
                else if (!string.IsNullOrWhiteSpace(model.ShippingAddress1) && model.ShippingAddress1.Trim().Length > 35)
                {
                    ModelState.AddModelError("ShippingAddress1", "长度超过35个字符".FormatWith(model.ShippingAddress1));
                    return View(model);
                }
                else if (!string.IsNullOrWhiteSpace(model.ShippingAddress2) && model.ShippingAddress2.Trim().Length > 35)
                {
                    ModelState.AddModelError("ShippingAddress2", "长度超过35个字符".FormatWith(model.ShippingAddress2));
                    return View(model);
                }

                if (model.CustomerOrderNumber.Length > 35)
                {
                    ModelState.AddModelError("CustomerOrderNumber", "长度超过35个字符".FormatWith(model.CustomerOrderNumber));
                    return View(model);
                }

                if (!string.IsNullOrWhiteSpace(model.ShippingCompany) && model.ShippingCompany.Trim().Length > 35)
                {
                    ModelState.AddModelError("ShippingCompany", "长度超过35个字符".FormatWith(model.ShippingCompany));
                    return View(model);
                }

                if (!string.IsNullOrWhiteSpace(model.ShippingFirstName) && !string.IsNullOrWhiteSpace(model.ShippingLastName))
                {
                    if ((model.ShippingFirstName.Trim().Length + model.ShippingLastName.Trim().Length) > 35)
                    {
                        ModelState.AddModelError("ShippingFirstName", "长度超过35个字符".FormatWith(model.ShippingFirstName));
                        return View(model);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(model.ShippingFirstName))
                {
                    if (model.ShippingFirstName.Trim().Length > 35)
                    {
                        ModelState.AddModelError("ShippingFirstName", "长度超过35个字符".FormatWith(model.ShippingFirstName));
                        return View(model);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(model.ShippingLastName))
                {
                    if (model.ShippingLastName.Trim().Length > 35)
                    {
                        ModelState.AddModelError("ShippingLastName", "长度超过35个字符".FormatWith(model.ShippingLastName));
                        return View(model);
                    }
                }

                if (!string.IsNullOrWhiteSpace(model.TrackingNumber) && _orderService.IsExitTrackingNumber(model.TrackingNumber))
                {
                    ModelState.AddModelError("TrackingNumber", "跟踪号已存在".FormatWith(model.TrackingNumber));
                    return View(model);
                }

                if (!string.IsNullOrWhiteSpace(model.ShippingCity) && model.ShippingCity.Length > 35)
                {
                    ModelState.AddModelError("ShippingCity", "城市超过35个字符".FormatWith(model.ShippingCity));
                    return View(model);
                }

                if (!string.IsNullOrWhiteSpace(model.ShippingState) && model.ShippingState.Length > 2)
                {
                    ModelState.AddModelError("ShippingState", "省份超过2个字符".FormatWith(model.ShippingState));
                    return View(model);
                }
                if (!string.IsNullOrWhiteSpace(model.ShippingZip) && model.ShippingZip.Length > 9 || model.ShippingZip.Length < 5)
                {
                    ModelState.AddModelError("ShippingZip", "字符长度为5-9之间".FormatWith(model.ShippingZip));
                    return View(model);
                }

                if (model.CountryCode.Length > 2)
                {
                    ModelState.AddModelError("CountryCode", "简码超过2个字符".FormatWith(model.CountryCode));
                    return View(model);
                }

            }

            #region 中邮挂号福州
            if (shippingMethod != null && (shippingMethod.Code.Trim().ToUpperInvariant() == "CNPOST-FZ" || shippingMethod.Code.Trim().ToUpperInvariant() == "CNPOST-FYB"))
            {
                if (model.CustomerOrderNumber != null && model.CustomerOrderNumber.Length > 30)
                {
                    InitAdd();
                    ModelState.AddModelError("CustomerOrderNumber", "订单号长度必须小于等于30".FormatWith(model.CustomerOrderNumber));
                    return View(model);
                }
                //国家两位
                if (model.CountryCode != null && model.CountryCode.Length != 2)
                {
                    InitAdd();
                    ModelState.AddModelError("CountryCode", "国家简码必须是两位".FormatWith(model.CountryCode));
                    return View(model);
                }
                //收件人州或省
                if (model.ShippingState != null && model.ShippingState.Length > 50)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingState", "收件人省或州长度不能超过50".FormatWith(model.ShippingState));
                    return View(model);
                }
                //收件人城市
                if (model.ShippingCity != null && model.ShippingCity.Length > 50)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingCity", "收件人城市长度不能超过50".FormatWith(model.ShippingCity));
                    return View(model);
                }
                //收件人地址
                string address = "";
                if (model.ShippingAddress != null)
                {
                    address += model.ShippingAddress;
                }
                if (model.ShippingAddress1 != null)
                {
                    address += model.ShippingAddress1;
                }
                if (model.ShippingAddress2 != null)
                {
                    address += model.ShippingAddress2;
                }
                if (address.Length > 120)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingAddress", "收件人地址长度不能超过120".FormatWith(model.ShippingAddress));
                    return View(model);
                }
                //收件人邮编
                if (model.ShippingZip != null && model.ShippingZip.Length > 12)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingZip", "收件人邮编长度不能超过12".FormatWith(model.ShippingZip));
                    return View(model);
                }
                //收件人名字
                string name = "";
                if (model.ShippingFirstName != null)
                {
                    name += model.ShippingFirstName;
                }
                if (model.ShippingLastName != null)
                {
                    name += model.ShippingLastName;
                }
                if (name.Length > 64)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingFirstName", "收件人名字长度不能超过64".FormatWith(model.ShippingFirstName));
                    return View(model);
                }
                //收件人电话
                if (model.ShippingPhone != null && model.ShippingPhone.Length > 20)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingPhone", "收件人电话长度不能超过20".FormatWith(model.ShippingPhone));
                    return View(model);
                }
                //发件人省份
                if (model.SenderState != null && model.SenderState.Length > 20)
                {
                    InitAdd();
                    ModelState.AddModelError("SenderState", "发件人州省长度不能超过20".FormatWith(model.SenderState));
                    return View(model);
                }
                //发件人城市
                if (model.SenderCity != null && model.SenderCity.Length > 64)
                {
                    InitAdd();
                    ModelState.AddModelError("SenderCity", "发件人城市长度不能超过64".FormatWith(model.SenderCity));
                    return View(model);
                }
                //发件人街道
                if (model.SenderAddress != null && model.SenderAddress.Length > 120)
                {
                    InitAdd();
                    ModelState.AddModelError("SenderAddress", "发件人地址长度不能超过120".FormatWith(model.SenderAddress));
                    return View(model);
                }
                //发件人邮编
                if (model.SenderZip != null && model.SenderZip.Length > 6)
                {
                    InitAdd();
                    ModelState.AddModelError("SenderZip", "发件人邮编长度不能超过6".FormatWith(model.SenderZip));
                    return View(model);
                }
                //发件人名字
                string senderName = "";
                if (model.SenderFirstName != null)
                {
                    senderName += model.SenderFirstName;
                }
                if (model.SenderLastName != null)
                {
                    senderName += model.SenderLastName;
                }
                if (senderName.Length > 20)
                {
                    InitAdd();
                    ModelState.AddModelError("SenderFirstName", "发件人名字长度不能超过20".FormatWith(model.SenderFirstName));
                    return View(model);
                }
                //发件人电话
                if (model.SenderPhone != null && model.SenderPhone.Length > 20)
                {
                    InitAdd();
                    ModelState.AddModelError("SenderPhone", "发件人电话长度不能超过20".FormatWith(model.SenderPhone));
                    return View(model);
                }
            }

            #endregion

            #region DHL 上传验证

            if (shippingMethod != null && (shippingMethod.Code.Trim().ToUpperInvariant() == "HKDHL" || shippingMethod.Code.Trim().ToUpperInvariant() == "DHLCN" || shippingMethod.Code.Trim().ToUpperInvariant() == "DHLSG"))
            {
                if (model.InsureAmount != null && model.InsureAmount.ToString().Length > 14)
                {
                    InitAdd();
                    ModelState.AddModelError("InsureAmount", "保险金额长度不能超过14个字符".FormatWith(model.InsureAmount));
                    return View(model);
                }
                else if (model.InsureAmount != null && !Regex.IsMatch(model.InsureAmount.ToString(), "^[0-9]+[.]{0,1}[0-9]{0,2}$"))
                {
                    InitAdd();
                    ModelState.AddModelError("InsureAmount", "保险金额有数字组成，最多保留两位小数".FormatWith(model.InsureAmount));
                    return View(model);
                }

                if (model.ShippingCompany != null && model.ShippingCompany.Length > 35)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingCompany", "收件人公司长度为0-35个字符".FormatWith(model.ShippingCompany));
                    return View(model);
                }
                string address = "";
                if (model.ShippingAddress != null)
                {
                    address += model.ShippingAddress;
                }
                if (model.ShippingAddress1 != null)
                {
                    address += model.ShippingAddress1;
                }
                if (model.ShippingAddress2 != null)
                {
                    address += model.ShippingAddress2;
                }
                try
                {

                    if (address.Length > 70 || address.StringSplitLengthWords(35).Count > 2)
                    {
                        InitAdd();
                        ModelState.AddModelError("ShippingAddress", "收件人地址超长".FormatWith(model.ShippingAddress));
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                    InitAdd();
                    ModelState.AddModelError("ShippingAddress", "收件人地址格式错误".FormatWith(model.ShippingAddress));
                    return View(model);
                }

                if (model.ShippingCity != null && model.ShippingCity.Length > 35)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingCity", "收件人城市不能超过35个字符".FormatWith(model.ShippingCity));
                    return View(model);
                }

                if (model.ShippingState != null && model.ShippingState.Length > 35)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingState", "收件人州/省长度为1-35个字符".FormatWith(model.ShippingState));
                    return View(model);
                }
                if (string.IsNullOrWhiteSpace(model.ShippingZip) || model.ShippingZip.Length > 12)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingZip", "收件人邮编长度为1-12个字符".FormatWith(model.ShippingZip));
                    return View(model);
                }

                if (!string.IsNullOrWhiteSpace(model.ShippingTaxId) &&
                    model.ShippingTaxId.Length > 20)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingTaxId", "收件人税号不能超过20字符".FormatWith(model.ShippingTaxId));
                    return View(model);
                }
                if ((model.ShippingFirstName + model.ShippingLastName).Length > 35)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingFirstName", "收件人姓名不能超过35个字符".FormatWith(model.ShippingFirstName));
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.ShippingPhone))
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingPhone", "收件人电话不能为空".FormatWith(model.ShippingPhone));
                    return View(model);
                }
                else if (model.ShippingPhone.Length > 25)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingPhone", "收件人电话不能超过25个字符".FormatWith(model.ShippingPhone));
                    return View(model);
                }
            }
            #endregion

            #region EUB 上传验证
            if (shippingMethod != null &&
                    (shippingMethod.Code.Trim().ToUpperInvariant() == "EUB_CS" || shippingMethod.Code.Trim().ToUpperInvariant() == "EUB-SZ" ||
                     shippingMethod.Code.Trim().ToUpperInvariant() == "EUB-FZ"))
            {
                if (model.CustomerOrderNumber != null && (model.CustomerOrderNumber.Length > 32 || model.CustomerOrderNumber.Length < 4))
                {
                    InitAdd();
                    ModelState.AddModelError("CustomerOrderNumber", "订单号长度必须为4-32个字符".FormatWith(model.CustomerOrderNumber));
                    return View(model);
                }
                string name = "";
                if (model.ShippingFirstName != null)
                {
                    name += model.ShippingFirstName;
                }
                if (model.ShippingLastName != null)
                {
                    name += model.ShippingLastName;
                }
                if (string.IsNullOrWhiteSpace(name) || name.Length > 256)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingFirstName", "收件人姓名长度为1-256个字符".FormatWith(model.ShippingFirstName));
                    return View(model);
                }
                if (string.IsNullOrWhiteSpace(model.ShippingCity) || model.ShippingCity.Length > 128)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingCity", "收件人城市长度为1-128个字符".FormatWith(model.ShippingCity));
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.ShippingState) || model.ShippingState.Length > 128)
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingState", "收件人州/省长度为1-128个字符".FormatWith(model.ShippingState));
                    return View(model);
                }

                if (model.ShippingZip != null)
                {
                    if (model.ShippingZip.Length > 16)
                    {
                        InitAdd();
                        ModelState.AddModelError("ShippingZip", "收件人邮编不能超过16个字符".FormatWith(model.ShippingZip));
                        return View(model);
                    }
                    else
                    {
                        switch (model.CountryCode.ToUpperInvariant())
                        {
                            case "US":
                                if (!Regex.IsMatch(model.ShippingZip, "^(^[0-9]{5}-[0-9]{4}$)|(^[0-9]{5}-[0-9]{5}$)|(^[0-9]{5}$)$"))
                                {
                                    InitAdd();
                                    ModelState.AddModelError("ShippingZip", "邮编不合法".FormatWith(model.ShippingZip));
                                    return View(model);
                                }
                                break;
                            case "AU":
                                if (!Regex.IsMatch(model.ShippingZip, "^[0-9]{4}$"))
                                {
                                    InitAdd();
                                    ModelState.AddModelError("ShippingZip", "邮编不合法".FormatWith(model.ShippingZip));
                                    return View(model);
                                }
                                break;
                            case "CA":
                                if (!Regex.IsMatch(model.ShippingZip, "^(^[A-Za-z][0-9][A-Za-z][ ][0-9][A-Za-z][0-9]$)|(^[A-Za-z][0-9][A-Za-z][0-9][A-Za-z][0-9]$)$"))
                                {
                                    InitAdd();
                                    ModelState.AddModelError("ShippingZip", "邮编不合法".FormatWith(model.ShippingZip));
                                    return View(model);
                                }
                                break;
                            case "GB":
                                if (!Regex.IsMatch(model.ShippingZip, "^[A-Za-z0-9]{2,4} [A-Za-z0-9]{3}$"))
                                {
                                    InitAdd();
                                    ModelState.AddModelError("ShippingZip", "邮编不合法".FormatWith(model.ShippingZip));
                                    return View(model);
                                }
                                break;
                            case "FR":
                                if (!Regex.IsMatch(model.ShippingZip, "^[0-9]{5}$"))
                                {
                                    InitAdd();
                                    ModelState.AddModelError("ShippingZip", "邮编不合法".FormatWith(model.ShippingZip));
                                    return View(model);
                                }
                                break;
                            case "RU":
                                if (!Regex.IsMatch(model.ShippingZip, "^[0-9]{6}$"))
                                {
                                    InitAdd();
                                    ModelState.AddModelError("ShippingZip", "邮编不合法".FormatWith(model.ShippingZip));
                                    return View(model);
                                }
                                break;
                        }
                    }
                }
                else if (model.CountryCode != "HK")
                {
                    InitAdd();
                    ModelState.AddModelError("ShippingZip", "邮编不能为空".FormatWith(model.ShippingZip));
                    return View(model);
                }

            }
            #endregion
            if (int.TryParse(model.PackageNumberValue, out packageNumber))
            {
                if (packageNumber < 0)
                {
                    ModelState.AddModelError("PackageNumberValue", "件数必须大于零".FormatWith(model.PackageNumberValue));
                    return View(model);
                }
                else
                {
                    if (packageNumber == 0)
                    {
                        packageNumber = 1;
                    }
                    model.PackageNumber = packageNumber;
                }
            }
            else
            {
                ModelState.AddModelError("PackageNumberValue", "件数必须为数字".FormatWith(model.PackageNumberValue));
                return View(model);
            }
            if (model.ShippingMethodId == sysConfig.SpecialShippingMethodId)
            {
                if (!string.IsNullOrWhiteSpace(model.ShippingZip))
                {
                    if (!Tools.CheckPostCode(model.ShippingZip))
                    {
                        ModelState.AddModelError("ShippingZip", "邮编为6位纯数字且只能以1 2 3 4 6开头".FormatWith(model.ShippingZip));
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("ShippingZip", "邮编不能为空".FormatWith(model.ShippingZip));
                    return View(model);
                }
                if (!string.IsNullOrWhiteSpace(model.ShippingPhone))
                {
                    if (!Tools.CheckShippingPhone(model.ShippingPhone))
                    {
                        ModelState.AddModelError("ShippingPhone", "电话号码最长不能超过11位数字".FormatWith(model.ShippingPhone));
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("ShippingPhone", "联系电话不能为空".FormatWith(model.ShippingPhone));
                    return View(model);
                }
            }
            if (model.IsInsured)
            {
                string[] insuredValue = model.InsuredValue.Split('_');
                if (insuredValue.Length > 0)
                {
                    model.InsuredID = int.Parse(insuredValue[0]);
                }
                if (model.InsuredID == 2)
                {
                    decimal insureAmountvalue;
                    if (string.IsNullOrWhiteSpace(model.InsureAmountValue))
                    {
                        ModelState.AddModelError("InsureAmountValue", "保险价值不能为空".FormatWith(model.InsureAmountValue));
                        return View(model);
                    }
                    if (decimal.TryParse(model.InsureAmountValue, out insureAmountvalue))
                    {
                        if (insureAmountvalue <= 0)
                        {
                            ModelState.AddModelError("InsureAmountValue",
                                                     "保险价值必须大于零".FormatWith(model.InsureAmountValue));
                            return View(model);
                        }
                        else
                        {
                            model.InsureAmount = insureAmountvalue;
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("InsureAmountValue", "保险价值必须为数字".FormatWith(model.InsureAmountValue));
                        return View(model);
                    }
                }
                else
                {
                    model.InsureAmount = decimal.Parse(insureList.Find(p => p.InsuredID == model.InsuredID).InsuredCalculation1);
                }
            }


            //EUB 验证
            if (shippingMethod != null &&
                (shippingMethod.Code.Trim().ToUpperInvariant() == "EUB_CS" ||
                 shippingMethod.Code.Trim().ToUpperInvariant() == "EUB-SZ" ||
                 shippingMethod.Code.Trim().ToUpperInvariant() == "EUB-FZ"))
            {
                if (receiver.Count > 0)
                {
                    var n = 1;
                    string error = "";

                    foreach (var row in receiver)
                    {
                        if (string.IsNullOrWhiteSpace(row.ApplicationName) || row.ApplicationName.Length > 128)
                        {
                            error += "第" + n + "行申报名称不能为空或超过128个字符";
                        }

                        if (string.IsNullOrWhiteSpace(row.PickingName) || row.PickingName.Length > 64)
                        {
                            error += "第" + n + "行申报中文名称不能为空或超过字符长度";
                        }
                        else
                        {
                            if (!Regex.IsMatch(row.PickingName, @"[\u4e00-\u9fa5]+[A-Za-z0-9]*[\s\S]*[\u4e00-\u9fa5]+"))
                            {
                                error += "第" + n + "列申报中文名称必须包含两个中文字符";
                            }
                        }
                        if (row.UnitWeight <= 0)
                        {
                            error += "第" + n + "列申报重量必须大于0";
                        }
                        n++;
                    }
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        InitAdd();
                        ErrorNotification("添加失败，" + error);
                        return View(model);
                    }
                }
            }

            //验证申报信息
            if (receiver.Count > 0)
            {
                var n = 1;
                string error = "";
                foreach (var row in receiver)
                {

                    if (row.UnitWeight < 0)
                    {
                        error += "第" + n + "列申报重量不能为0";
                    }
                    n++;
                }
                if (!string.IsNullOrWhiteSpace(error))
                {
                    InitAdd();
                    ErrorNotification("添加失败，" + error);
                    return View(model);
                }
            }

            //福州邮政申报信息判断
            if (shippingMethod != null && (shippingMethod.Code.Trim().ToUpperInvariant() == "CNPOST-FZ" || shippingMethod.Code.Trim().ToUpperInvariant() == "CNPOSTP_FZ" || shippingMethod.Code.Trim().ToUpperInvariant() == "CNPOST-FYB"))
            {
                if (receiver.Count > 0)
                {
                    var n = 1;
                    string error = "";

                    foreach (var row in receiver)
                    {
                        if (string.IsNullOrWhiteSpace(row.ApplicationName) || row.ApplicationName.Length > 60)
                        {
                            error += "第" + n + "行申报名称不能为空或超过字符长度";
                        }

                        if (string.IsNullOrWhiteSpace(row.PickingName) || row.PickingName.Length > 60)
                        {
                            error += "第" + n + "行申报中文名称不能为空或超过字符长度";
                        }
                        n++;
                    }
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        InitAdd();
                        ErrorNotification("添加失败，" + error);
                        return View(model);
                    }
                }
            }

            //DHL验证申报信息
            if (shippingMethod != null &&
                (shippingMethod.Code.Trim().ToUpperInvariant() == "HKDHL" ||
                 shippingMethod.Code.Trim().ToUpperInvariant() == "DHLCN" ||
                 shippingMethod.Code.Trim().ToUpperInvariant() == "DHLSG"))
            {
                if (receiver.Count > 0)
                {
                    var n = 1;
                    string error = "";

                    foreach (var row in receiver)
                    {
                        if (string.IsNullOrWhiteSpace(row.ApplicationName) || row.ApplicationName.Length > 60)
                        {
                            error += "第" + n + "行申报名称不能为空或超过字符长度";
                        }
                        else
                        {
                            if (Regex.IsMatch(row.ApplicationName,
                                               @"[\~]{1}|[\@]{1}|[\#]{1}|[\$]{1}|[\￥]{1}|[\%]{1}|[\^]{1}|[\&]{1}|[\*]{1}|[\(]{1}|[\)]{1}|[\u4e00-\u9fa5]+"))
                            {
                                error += "第" + n + "列申报英文名称不能包含特殊字符和汉字";
                            }
                        }
                        n++;
                    }
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        InitAdd();
                        ErrorNotification("添加失败，" + error);
                        return View(model);
                    }
                }
            }

            if (shippingMethod != null && (shippingMethod.Code == sysConfig.DDPRegisterShippingMethodCode || shippingMethod.Code == sysConfig.DDPShippingMethodCode))
            {
                if (receiver.Count>0)
                {
                    var n = 1;
                    foreach (var row in receiver)
                    {
                        if (row.HSCode == null || string.IsNullOrWhiteSpace(row.HSCode))
                        {
                            InitAdd();
                            ErrorNotification("添加失败，第" + n + "行申报信息海关编码不能为空");
                            return View(model);
                        }
                        else if (row.ProductUrl == null || string.IsNullOrWhiteSpace(row.ProductUrl))
                        {
                            InitAdd();
                            ErrorNotification("添加失败，第" + n + "行申报信息销售链接不能为空");
                            return View(model);
                        }
                        else if (row.Remark == null || string.IsNullOrWhiteSpace(row.Remark))
                        {
                            InitAdd();
                            ErrorNotification("添加失败，第" + n + "行申报信息备注不能为空");
                            return View(model);
                        }
                        n++;
                    }
                }
            }



            //#region 操作日志记录
            ////yungchu 
            ////敏感字段--运输方式，国家  代码写在这里避免取到改变的缓存值
            //var sbBuilder = new StringBuilder();
            //sbBuilder.Append("");
            //if (info.ShippingMethodId != model.ShippingMethodId)
            //{
            //	List<int> li = new List<int> {model.ShippingMethodId};
            //	string shippingMethodName =_freightService.GetShippingMethodsByIds(li)[0].FullName;
            //	sbBuilder.AppendFormat(" 运输方式从{0}更改为{1} ", info.ShippingMethodName, shippingMethodName);
            //}
            //if (info.ShippingInfo.CountryCode != model.CountryCode)
            //{
            //	sbBuilder.AppendFormat(" 国家从{0}更改为{1}", info.ShippingInfo.CountryCode, model.CountryCode);
            //}

            //BizLog bizlog = new BizLog()
            //{
            //	Summary = sbBuilder.ToString() != "" ? "[订单编辑]" + sbBuilder : "订单编辑",
            //	KeywordType = KeywordType.CustomerOrderNumber,
            //	Keyword = model.CustomerOrderNumber,
            //	UserCode = _workContext.User.UserUame,
            //	UserRealName = _customerService.GetCustomer(_workContext.User.UserUame).Name ?? _workContext.User.UserUame,
            //	UserType = UserType.LMS_User,
            //	SystemCode = SystemType.LMS,
            //	ModuleName = "订单编辑"
            //};

            //_operateLogServices.WriteLog(bizlog, model);
            //#endregion


            model.ToEntity(info);
            model.ToEntity(info.ShippingInfo);
            model.ToEntity(info.SenderInfo);
            info.CustomerOrderNumber = info.CustomerOrderNumber.Trim();
            if (info.TrackingNumber != null)
            {
                info.TrackingNumber = info.TrackingNumber.Trim().ToUpperInvariant();
            }
            info.LastUpdatedOn = DateTime.Now;
            info.LastUpdatedBy = _workContext.User.UserUame;
            info.ShippingMethodName = GetShipingMethods().First(p => p.Value == info.ShippingMethodId.ToString()).Text;
             
            if (!info.IsBattery) info.SensitiveTypeID = null;
            if (!info.IsInsured) info.InsuredID = null;


            if (receiver != null)
            {
                //var list = receiver.ToEntityAsCollection<ApplicationInfoModel, ApplicationInfo>();
                var list = info.ApplicationInfos.ToList();
                //如果页面的HSCode已经存在，就修改，不存在就添加
                receiver.ForEach(r =>
                    {
                        //if (list.Exists(a => a.ApplicationID == r.ApplicationID))
                        //{
                        //    var entity = info.ApplicationInfos.FirstOrDefault(a => a.HSCode == r.HSCode);

                        //    entity.LastUpdatedBy = info.LastUpdatedBy;
                        //    entity.LastUpdatedOn = info.LastUpdatedOn;

                        //}
                        //else
                        //{
                        //    var item = r.ToEntity<ApplicationInfo>();
                        //    item.CreatedBy = info.CreatedBy;
                        //    item.CreatedOn = info.CreatedOn;
                        //    item.LastUpdatedBy = info.CreatedBy;
                        //    item.LastUpdatedOn = info.CreatedOn;
                        //    info.ApplicationInfos.Add(item);
                        //}
                        if (r.ApplicationID == 0)
                        {
                            var item = r.ToEntity<ApplicationInfo>();
                            item.CreatedBy = info.CreatedBy;
                            item.CreatedOn = info.CreatedOn;
                            item.LastUpdatedBy = info.CreatedBy;
                            item.LastUpdatedOn = info.CreatedOn;
                            info.ApplicationInfos.Add(item);
                        }

                    });
                //数据库存在的不存在页面，就删除
                foreach (var item in list.Where(item => !receiver.Exists(a => a.HSCode == item.HSCode)))
                {
                    info.ApplicationInfos.Remove(item);
                }
            }

            try
            {
                _customerOrderService.Moditfy(info);

                SuccessNotification("编辑成功");

            }
            catch (Exception e)
            {
                Log.Exception(e);
                ErrorNotification("编辑失败，原因为：" + e.Message);

                return View(model);
            }

            return View(model);
            //return RedirectToAction("Edit");
        }



	    //修改件数 add by yungchu
		public JsonResult EditPackageNumber(string customerOrderNumber, string packageNumber)
		{
			var result = new ResponseResult();
			try
			{
				var customerOrderInfo = _customerOrderInfoRepository.First(p => p.CustomerOrderNumber.Contains(customerOrderNumber));
	
				if (!string.IsNullOrEmpty(packageNumber))
				{
					customerOrderInfo.PackageNumber = Convert.ToInt32(packageNumber);
				}
				customerOrderInfo.IsHold = false;
				_customerOrderService.Moditfy(customerOrderInfo);
			

				//解除运单hold,异常运单已完成
				var listWaybillinfo = _wayBillInfoRepository.GetList(p => p.CustomerOrderNumber.Contains(customerOrderInfo.CustomerOrderNumber));
				listWaybillinfo.Each(p => p.IsHold = false);
				listWaybillinfo.Each(p => p.AbnormalWayBillLog.AbnormalStatus =(int)WayBill.AbnormalStatusEnum.OK.GetAbnormalStatusValue());

				foreach (var wayBillInfo in listWaybillinfo)
				{
					_wayBillInfoRepository.Modify(wayBillInfo);
					_wayBillInfoRepository.UnitOfWork.Commit();
				}

				result.Result = true;
				result.Message = packageNumber;
			}
			catch (Exception ex)
			{

				result.Result = false;
				result.Message = ex.Message;
			}
			return Json(result,JsonRequestBehavior.AllowGet);
		}




	    public ActionResult UnConfirmed(CustomerOrderFilter filter)
        {
            filter.Status = CustomerOrder.StatusEnum.None.GetStatusValue();

            return View(List(filter));
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var isSuccess = _customerOrderService.Delete(id);
                if (isSuccess) SuccessNotification("删除成功");
                else ErrorNotification("删除失败");

            }
            catch (Exception e)
            {
                Log.Exception(e);
                ErrorNotification("删除失败，原因为：" + e.Message);
            }

            return RedirectToAction("UnConfirmed");
        }

        /// <summary>
        /// 取消客户订单
        /// </summary>
        /// <param name="id">客户订单Id</param>
        /// <returns></returns>
        public ActionResult Cancel(int id)
        {

            try
            {
                var isSuccess = _customerOrderService.Cancel(id);
                if (isSuccess) SuccessNotification("取消成功");
                else ErrorNotification("取消失败");

            }
            catch (Exception e)
            {
                Log.Exception(e);
                ErrorNotification("取消失败，原因为：" + e.Message);
            }

            return RedirectToAction("Confirmed");
        }

        //[HttpPost]
        //[FormValueRequired("Confirmed")]
        //public ActionResult UnConfirmed(CustomerOrderFilter filter, List<int> selected)
        //{
        //    if (selected != null && selected.Count > 0)
        //    {
        //        var isSucess = true;
        //        foreach (var id in selected)
        //        {
        //            try
        //            {
        //                _customerOrderService.CustomerOrderConfirm(id);
        //            }
        //            catch (Exception e)
        //            {
        //                isSucess = false;
        //                Console.WriteLine(e);
        //            }
        //        }

        //        if (!isSucess) ErrorNotification("部分订单确认失败，请重新提交");
        //        else SuccessNotification("订单确认成功");
        //    }
        //    else
        //    {
        //        ErrorNotification("请选择需要确认的订单");
        //    }

        //    return RedirectToAction("UnConfirmed", filter);
        //}

        [HttpPost]
        [FormValueRequired("BatchConfirmed")]
        public ActionResult UnConfirmed(CustomerOrderFilter filter, List<int> selected)
        {
            if (selected != null && selected.Count > 0)
            {
                var isSucess = _customerOrderService.CustomerOrderConfirmBatch(selected);
                if (!isSucess) ErrorNotification("部分订单确认失败，请重新提交");
                else SuccessNotification("订单确认成功");
            }
            else
            {
                ErrorNotification("请选择需要确认的订单");
            }

            return RedirectToAction("UnConfirmed", filter);
        }


        public ActionResult Confirmed(CustomerOrderFilter filter)
        {
            filter.Status = CustomerOrder.StatusEnum.OK.GetStatusValue();

            return View(List(filter));

        }

        //[HttpPost]
        //[FormValueRequired("Submitted")]
        //public ActionResult Confirmed(CustomerOrderFilter filter, List<int> selected)
        //{
        //    if (selected != null && selected.Count > 0)
        //    {

        //        bool isSucess = true;
        //        foreach (var id in selected)
        //        {
        //            try
        //            {

        //                _customerOrderService.CustomerOrderSubmit(id);

        //            }
        //            catch (Exception e)
        //            {
        //                isSucess = false;
        //                Console.WriteLine(e);
        //            }
        //        }

        //        if (!isSucess) ErrorNotification("部分订单提交失败，请重新提交");
        //        else SuccessNotification("订单提交成功");
        //    }
        //    else
        //    {
        //        ErrorNotification("请选择需要提交的订单");
        //    }
        //    return RedirectToAction("Confirmed", filter);
        //}

        [HttpPost]
        [FormValueRequired("BatchSubmitted")]
        public ActionResult Confirmed(CustomerOrderFilter filter, List<int> selected)
        {
            SubmitCustomerOrder(selected);

            return RedirectToAction("Confirmed", filter);
        }

        [HttpPost]
        public ActionResult CheckRemoteArea(List<int> selected)
        {
            var list = _customerOrderService.CheckRemoteArea(selected);
            return Json(list);
        }

        private void SubmitCustomerOrder(List<int> listCustomerOrder)
        {
            if (listCustomerOrder != null && listCustomerOrder.Count > 0)
            {
                try
                {
                    int queueLength = QueueHelper.Count("SubmitOrder");

                    _customerOrderService.CustomerOrderSubmitQuick(listCustomerOrder);

                    queueLength += listCustomerOrder.Count;

                    double elapsedTime = queueLength * 0.1;//秒

                    string elapsedTimeStr = "";

                    if (elapsedTime < 1)
                    {
                        elapsedTimeStr = "小于1秒钟";
                    }
                    else if (elapsedTime / 60 < 1)
                    {
                        elapsedTimeStr = "小于1分钟";
                    }
                    else
                    {
                        elapsedTimeStr = (int)(elapsedTime / 60) + "分钟";
                    }

                    string notificationMessage = string.Format("订单已提交并正在处理，请稍后在已提交的订单中查看，预计处理完成时间：{0}后", elapsedTimeStr);

                    SuccessNotification(notificationMessage);
                }
                catch (Exception ex)
                {
                    ErrorNotification(ex.Message);
                }
            }
            else
            {
                ErrorNotification("请选择需要提交的订单");
            }
        }

        public ActionResult SubmitFail(CustomerOrderFilter filter)
        {
            filter.Status = CustomerOrder.StatusEnum.SubmitFail.GetStatusValue();

            return View(List(filter));
        }

        [HttpPost]
        [FormValueRequired("BatchSubmitted")]
        public ActionResult SubmitFail(CustomerOrderFilter filter, List<int> selected)
        {
            SubmitCustomerOrder(selected);
            return RedirectToAction("SubmitFail", filter);
        }
        /// <summary>
        /// 批量删除订单
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("unconfirmed")]
        [FormValueRequired("BatchDeletess")]
        public ActionResult BatchDeletess(CustomerOrderFilter filter, List<int> selected)
        {
            HttpContext.Server.ScriptTimeout = 100 * 60;
            if (selected != null && selected.Count > 0)
            {
                try
                {
                    bool isSucess = _customerOrderService.DeleteCustomerOrderInfoList(selected);
                    if (!isSucess) ErrorNotification("订单删除失败，请重新删除");
                    else SuccessNotification("订单删除成功");
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    ErrorNotification(ex.Message);
                }
            }
            else
            {
                ErrorNotification("请选择需要删除的订单");
            }
            return RedirectToAction("unconfirmed", filter);
        }





        /// 批量删除确定订单
        /// yungchu
        /// <param name="filter"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Confirmed")]
        [FormValueRequired("BatchDeleteOrder")]
        public ActionResult BatchDeleteOrder(CustomerOrderFilter filter, List<int> selected)
        {
            HttpContext.Server.ScriptTimeout = 100 * 60;
            if (selected != null && selected.Count > 0)
            {
                try
                {
                    bool isSucess = _customerOrderService.DeleteCustomerOrderInfoList(selected);
                    if (!isSucess) ErrorNotification("订单删除失败，请重新删除");
                    else SuccessNotification("订单删除成功");
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    ErrorNotification(ex.Message);
                }
            }
            else
            {
                ErrorNotification("请选择需要删除的订单");
            }
            return RedirectToAction("Confirmed", filter);
        }


        /// <summary>
        /// 已提交订单--批量删除（先批量拦截再批量删除）
        /// yungchu
        /// 2014-07-21
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Submitted")]
        [FormValueRequired("BatchDelete")]
        public ActionResult BatchDelete(CustomerOrderFilter filter, List<int> selected)
        {
            if (selected != null && selected.Count > 0)
            {
                try
                {
                    bool isSucessHold = false;
                    bool isSucessDelete = false;

                    isSucessHold = _customerOrderService.BatchHold(selected, "客户删除");
                    isSucessDelete = _customerOrderService.UpdateWaybillStatus(selected);


                    if (isSucessHold && isSucessDelete)
                    {
                        SuccessNotification("订单删除成功");
                    }
                    else ErrorNotification("订单删除失败，请重新删除");
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    ErrorNotification(ex.Message);
                }
            }
            else
            {
                ErrorNotification("请选择需要删除的订单");
            }


            return RedirectToAction("Submitted", filter);
        }

        /// <summary>
        /// 批量取消订单
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Confirmed")]
        [FormValueRequired("BatchCancelOrder")]
        public ActionResult BatchCancelOrder(CustomerOrderFilter filter, List<int> selected)
        {
            if (selected != null && selected.Count > 0)
            {
                bool isSucess = _customerOrderService.BatchCancel(selected);

                if (!isSucess) ErrorNotification("部分订单取消失败，请重新提交");
                else SuccessNotification("订单取消成功");
            }
            else
            {
                ErrorNotification("请选择需要取消的订单");
            }
            return RedirectToAction("Confirmed", filter);
        }

        /// <summary>
        /// 批量取消订单
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("submitfail")]
        [FormValueRequired("BatchCancelOrder")]
        public ActionResult BatchCancelOrderBySubmitfail(CustomerOrderFilter filter, List<int> selected)
        {
            if (selected != null && selected.Count > 0)
            {
                bool isSucess = _customerOrderService.BatchCancel(selected);

                if (!isSucess) ErrorNotification("部分订单取消失败，请重新提交");
                else SuccessNotification("订单取消成功");
            }
            else
            {
                ErrorNotification("请选择需要取消的订单");
            }
            return RedirectToAction("submitfail", filter);
        }

        /// <summary>
        /// 批量删除提交失败的订单
        /// yungchu
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("submitfail")]
        [FormValueRequired("BatchDeleteOrder")]
        public ActionResult BatchDeleteOrderBySubmitfail(CustomerOrderFilter filter, List<int> selected)
        {
            if (selected != null && selected.Count > 0)
            {
                bool isSucess = _customerOrderService.BatchDelete(selected);

                if (!isSucess) ErrorNotification("部分订单删除失败，请重新提交");
                else SuccessNotification("订单删除成功");
            }
            else
            {
                ErrorNotification("请选择需要删除的订单");
            }
            return RedirectToAction("submitfail", filter);
        }



        //public ActionResult Printer(int id)
        //{


        //    var model = GetPrinterInfo(id);

        //    if (model == null)
        //    {
        //        ErrorNotification("打印失败");
        //        return RedirectToAction("submitted");
        //    }
        //    return View();
        //    //return View(new List<CustomerOrderInfoModel> { model });
        //}





        [HttpPost]
        [ActionName("All")]
        [FormValueRequired("BatchExport")]
        public ActionResult BatchExportOrder(CustomerOrderFilter filter)
        {
            filter.IsAll = true;

            ExportExcel(filter);
            return RedirectToAction("All", filter);
        }

        [HttpPost]
        [ActionName("send")]
        [FormValueRequired("BatchExport")]
        public ActionResult BatchExportOrderBySend(CustomerOrderFilter filter)
        {
            filter.Status = CustomerOrder.StatusEnum.Send.GetStatusValue();
            ExportExcel(filter);
            return RedirectToAction("send", filter);
        }

        [HttpPost]
        [ActionName("received")]
        [FormValueRequired("BatchExport")]
        public ActionResult BatchExportOrderByReceived(CustomerOrderFilter filter)
        {
            filter.Status = CustomerOrder.StatusEnum.Have.GetStatusValue();
            filter.IsReceived = true;
            ExportExcel(filter);
            return RedirectToAction("received", filter);
        }

        [HttpPost]
        [ActionName("submitted")]
        [FormValueRequired("BatchExport")]
        public ActionResult BatchExportOrderBySubmitted(CustomerOrderFilter filter)
        {
            filter.Status = CustomerOrder.StatusEnum.Submitted.GetStatusValue();
            filter.IsSubmitted = true;
            ExportExcel(filter);
            return RedirectToAction("submitted", filter);
        }


        private void ExportExcel(CustomerOrderFilter filter)
        {
            List<CustomerOrderInfoModel> customerOrderlist = new List<CustomerOrderInfoModel>();
            CustomerOrderViewModels fieldItems = new CustomerOrderViewModels();
            var param = new CustomerOrderParam
            {
                CustomerOrderNumber = filter.CustomerOrderNumber,
                SearchWhere = filter.SearchWhere,
                SearchContext = filter.SearchContext,
                CountryCode = filter.CountryCode,
                CreatedOnFrom = filter.CreatedOnFrom,
                CreatedOnTo = (filter.CreatedOnTo.HasValue ? filter.CreatedOnTo.Value.ToString("yyyy-MM-dd 23:59:59") : null).ConvertTo<DateTime?>(),
                ShippingMethodId = filter.ShippingMethodId,
                Status = filter.Status,
                IsReceived = filter.IsReceived,
                IsPrinted = filter.IsPrinted,
                IsAll = filter.IsAll,
                IsHold = filter.IsHold,
                Page = filter.Page,
                PageSize = filter.PageSize,
                WayBillNumber = filter.WayBillNumber,
                CustomerCode = _workContext.User.UserUame

            };
            customerOrderlist = _customerOrderService.GetCustomerOrderInfoExport(param).ToModelAsCollection<CustomerOrderInfoExportExt, CustomerOrderInfoModel>();
            List<ApplicationInfoModel> applist = new List<ApplicationInfoModel>();
            List<int> ids = new List<int>();
            customerOrderlist.ForEach(p => ids.Add(p.CustomerOrderID));
            applist =
                _customerOrderService.GetApplicationInfoList(ids).ToModelAsCollection<ApplicationInfo, ApplicationInfoModel>();
            customerOrderlist.ForEach(p =>
                {
                    p.SenderFirstLastName = p.SenderFirstName + " " + p.SenderLastName;
                    p.ShippingFirstLastName = p.ShippingFirstName + " " + p.ShippingLastName;
                    p.AppLicationTypeId = CustomerOrder.GetApplicationTypeDescription(p.AppLicationType);
                    p.InsuredName = CustomerOrder.GetInsuredCalculationsTypeDescription(p.InsuredID);
                    p.SensitiveTypeName = CustomerOrder.GetSensitiveTypeInfosTypeDescription(p.SensitiveTypeID);
                    p.ApplicationInfoList = applist.FindAll(a => a.CustomerOrderID == p.CustomerOrderID);
                });
            int count = 0, countlist = 0;
            customerOrderlist.ForEach(c =>
            {
                count = c.ApplicationInfoList.Count;
                if (count > countlist)
                {
                    countlist = count;
                }
            });
            var strlist = filter.FieldIds.Split(',');


            List<string> lstTitles = new List<string>();
            string lstTitle;
            var applicationlist = fieldItems.FieldItems.FindAll(p => p.GroupName == "申报信息").FindAll(p => strlist.Contains(p.Id.ToString()));
            List<string> excepts = new List<string>();
            fieldItems.FieldItems.FindAll(p => p.GroupName == "申报信息").ForEach(p => excepts.Add(p.Id.ToString()));


            List<int> list = new List<int>();
            strlist.Except(excepts.ToArray()).Each(p => list.Add(Convert.ToInt32(p)));
            list.Sort();
            foreach (var id in list)
            {

                var field = fieldItems.FieldItems.Find(p => p.Id == id);
                if (id == 31)
                {
                    var lwg = field.Value.Split('&');
                    foreach (var l in lwg)
                    {
                        switch (l)
                        {
                            case "Length":
                                lstTitle = "Length-长cm";
                                lstTitles.Add(lstTitle);
                                break;
                            case "Width":
                                lstTitle = "Width-宽cm";
                                lstTitles.Add(lstTitle);
                                break;
                            case "Height":
                                lstTitle = "Height-高cm";
                                lstTitles.Add(lstTitle);
                                break;
                        }
                    }
                }
                else
                {
                    lstTitle = field.Value + "-" + field.Text;
                    lstTitles.Add(lstTitle);
                }

            }
            if (countlist > 0 && applicationlist.Count > 0)
            {
                for (int i = 1; i <= countlist; i++)
                {
                    foreach (var app in applicationlist)
                    {
                        var field = fieldItems.FieldItems.Find(p => p.Id == app.Id);
                        lstTitle = field.Value + i + "-" + field.Text + i;
                        lstTitles.Add(lstTitle);
                    }
                }
            }
            string fileName = "所有订单" + DateTime.Now.ToString("yyyy-MM-dd");
            ExportExcelByWeb.ListExcel(fileName, customerOrderlist, lstTitles);
        }

        #region 模板打印
        public ActionResult Printer(string typeId, int type, string ids)
        {
            var viewModel = BindPrinterViewModel(typeId, type, ids);
            return View("Printer", viewModel);
        }

        /// <summary>
        /// 判断广州小包发货地址是否足够
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public JsonResult GZPrinter(string typeId, string ids)
        {
            var model = new ResponseResult();
            model.Result = true;
            try
            {
                string[] arr = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length == 0)
                {
                }
                List<int> customerOrderIds = new List<int>();
                arr.Each(v => customerOrderIds.Add(v.ConvertTo<int>()));

                var list = GetPrinterList(customerOrderIds);
                var shippingMethodList = _freightService.GetShippingMethods(null, false);

                int n = 0;
                int number = 0;
                list.ForEach(p =>
                {
                    //Update:zhengsong
                    //如果是广州小包，并且是地址标签的 系统分配 发件人信息
                    var shipp = shippingMethodList.FirstOrDefault(z => z.ShippingMethodId == p.ShippingMethodId);
                    if (shipp != null && (shipp.Code == "CNPOST-GZ" || shipp.Code == "CNPOSTP-GZ"))
                    {
                        n++;
                    }
                });
                var gzPacketAddressList = _wayBillTemplateService.GetGZPacketAddressInfo();
                gzPacketAddressList.ForEach(p =>
                {
                    number += (50 - p.Number);
                });
                if (number < n)
                {
                    model.Result = false;
                    model.Message = "广州小包发货地址不足";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                model.Result = false;
                model.Message = "出现异常";
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 打印EUB运单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public ActionResult PrintEubOrder(int id, string url)
        {
            _customerOrderService.UpdateEubWayBillStatus(id);
            return Redirect(url);
        }

        /// <summary>
        /// 自动扫描打印标签
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public JsonResult AutoScanPrintLabel(ScanPrintLabelFilter filter)
        {
            var model = new JsonModelResult { IsSuccess = false, Message = string.Empty, HtmlString = string.Empty };
            if (string.IsNullOrWhiteSpace(filter.OrderNumber))
            {
                model.Message = "订单号不能为空！";
                return Json(model, JsonRequestBehavior.AllowGet);
            }

            var customerOrderModel = GetPrinterByOrderNumber(filter.OrderNumber.Trim());
            if (null == customerOrderModel)
            {
                model.Message = "无此订单，或订单未提交！";
                return Json(model, JsonRequestBehavior.AllowGet);
            }

            var wayBillTemplateModel =
                _wayBillTemplateService.GetWayBillTemplateByNameAndShippingMethod(filter.TemplateName,
                                                                                  customerOrderModel.ShippingMethodId)
                                       .FirstOrDefault();
            if (wayBillTemplateModel != null)
            {
                model.IsSuccess = true;
                model.HtmlString =
                    Razor.Parse(HttpUtility.HtmlDecode(wayBillTemplateModel.TemplateContent).Replace("orderModel", "Model"),
                                customerOrderModel);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            model.Message = "打印地址标签失败，未找到对应模版！";
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ScanPrintLabel(ScanPrintLabelFilter filter)
        {
            ScanPrintLabelViewModel viewModel = new ScanPrintLabelViewModel();
            var list = _wayBillTemplateService.GetList();

            var listDis = new List<WayBillTemplate>();

            //模板名称去重复
            list.ForEach(p =>
            {
                if (listDis.FindIndex(w => w.TemplateName == p.TemplateName) == -1)
                {
                    listDis.Add(p);
                }
            });

            listDis.ForEach(p => viewModel.PrintTemplate.Add(new SelectListItem() { Text = p.TemplateName, Value = p.TemplateName }));

            viewModel.Filter = filter;
            return View(viewModel);
        }
        [HttpPost]
        [ActionName("SinglePrint")]
        [FormValueRequired("SurePrint")]
        public ActionResult PrintLabel(ScanPrintLabelFilter filter)
        {
            ScanPrintLabelViewModel viewModel = new ScanPrintLabelViewModel();
            viewModel.Filter = filter;

            var customerOrderModel = GetPrinterByOrderNumber(filter.OrderNumber.Trim());
            if (null == customerOrderModel)
            {
                return View(viewModel);
            }

            var wayBillTemplateModel =
                _wayBillTemplateService.GetWayBillTemplateByNameAndShippingMethod(filter.TemplateName,
                                                                                  customerOrderModel.ShippingMethodId)
                                       .FirstOrDefault();
            if (wayBillTemplateModel != null)
            {
                viewModel.Filter.TemplateContent =
                    Razor.Parse(HttpUtility.HtmlDecode(wayBillTemplateModel.TemplateContent),
                                customerOrderModel);
                return View(viewModel);
            }
            return View(viewModel);
        }


        private CustomerOrderInfoModel GetPrinterByOrderNumber(string customerOrderNumber)
        {
            try
            {
                var entity = _customerOrderService.PrintByCustomerOrderNumber(customerOrderNumber);
                if (entity == null) return null;
                //var model = entity.ToModel<CustomerOrderInfoModel>();
                //if (string.IsNullOrWhiteSpace(model.TrackingNumber))
                //{
                //    var firstOrDefault = entity.WayBillInfos.FirstOrDefault();
                //    if (firstOrDefault != null)
                //        model.TrackingNumber = firstOrDefault.WayBillNumber;
                //}
                //model.BarCode = "<img id=\"img\" src=\"/barcode.ashx?m=0&h=35&vCode=" + model.TrackingNumber + "\" alt=\"" + model.TrackingNumber + "\" style=\"width:200px;height:35px;\" />";

                //entity.ShippingInfo.ToModel(model);
                //var country = GetCountryList().Single(c => c.CountryCode == entity.ShippingInfo.CountryCode);
                //model.CountryName = country.Name;
                //model.CountryChineseName = country.ChineseName;

                var model = DecorateCustomerOrderInfo(entity);
                return model;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return null;
            }
        }

        /// <summary>
        /// 获取要打印的数据
        /// </summary>
        /// <param name="customerOrderIds"></param>
        /// <returns></returns>
        private List<CustomerOrderInfoModel> GetPrinterList(IEnumerable<int> customerOrderIds)
        {

            return GetCustomerOrderListModel(customerOrderIds.ToArray());

            //var orderIds = customerOrderIds as int[] ?? customerOrderIds.ToArray();
            //var key = "cache_customerOrderIds-"+string.Join(",", orderIds);
            //if (orderIds.Length <= 0) return null;

            //var cacheList = CacheHelper.Get(key) as List<CustomerOrderInfoModel>;

            //if (null == cacheList)
            //{
            //    var cacheOrderList = GetCustomerOrderListModel(orderIds);

            //    if (cacheOrderList != null)
            //        CacheHelper.Insert(key, cacheOrderList);
            //    return cacheOrderList;
            //}
            //else
            //{

            //    return cacheList;
            //}
        }

        private List<CustomerOrderInfoModel> GetCustomerOrderListModel(int[] orderIds)
        {

            try
            {
                //System.Diagnostics.Stopwatch sw = new Stopwatch();
                //sw.Start();
                var list = _customerOrderService.PrintByCustomerOrderIds(orderIds);

                var listModel = new List<CustomerOrderInfoModel>();


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
        private CustomerOrderInfoModel DecorateCustomerOrderInfo(CustomerOrderInfo model)
        {
            var ApplicationInfos = new List<ApplicationInfoModel>();

            model.ApplicationInfos.ToList().ForEach(m => ApplicationInfos.Add(new ApplicationInfoModel()
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

            var customerOrderInfoModel = new CustomerOrderInfoModel()
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
            var mouthCountry = GetMouthCountryList().Find(c => c.CountryCode == customerOrderInfoModel.CountryCode);
            customerOrderInfoModel.MouthNumber = mouthCountry == null ? 0 : mouthCountry.MouthNumber;
            customerOrderInfoModel.SortingIdentity = GetSortingIdentityHtml(model.ShippingInfo.ShippingZip, model.ShippingInfo.CountryCode);

            customerOrderInfoModel.BatteryIdentity = (model.IsBattery &&(model.SensitiveTypeID == 1 || model.SensitiveTypeID == 2))
                                                         ? "D"
                                                         : "";
            return customerOrderInfoModel;
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

        /// <summary>
        /// 获取俄罗斯分拣代码
        /// </summary>
        /// <param name="postCode"></param>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        private string GetSortingIdentity(string postCode, string countryCode)
        {
            if (string.IsNullOrWhiteSpace(postCode)) return "";
            if (countryCode.ToUpper() != "RU") return "";

            string[] stpPostCodes = "16,17,18,19".Split(',');
            string[] sibPostCodes = "3,4, 60,61,62,63,64,65,66,67,68,69".Split(',');

            if (stpPostCodes.ToList().Exists(postCode.StartsWith)) return "STP";
            if (sibPostCodes.ToList().Exists(postCode.StartsWith)) return "SIB";

            return "MSC";
        }

        private string GetSortingIdentityHtml(string postCode, string countryCode)
        {
            string sortingIdentity = GetSortingIdentity(postCode, countryCode);

            if (string.IsNullOrWhiteSpace(sortingIdentity)) return sortingIdentity;

            string html = string.Format("<span style=\"display:inline-block;border:1px solid #000;padding:0 10px;line-height:20px;\">{0}</span>", sortingIdentity);

            return html;
        }

        private PrinterViewModel BindPrinterViewModel(string typeId, int type, string ids, string templateName = "")
        {
            PrinterViewModel viewModel = new PrinterViewModel()
            {
                Type = type,
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

            var list = GetPrinterList(customerOrderIds);
            
            if (list == null)
            {
                return viewModel;
            }
            //过滤相同的运输方式
            var shippingMethodList= _freightService.GetShippingMethods(null, false);
            list.ForEach(p =>
                {
                    if (!shippingMethodIds.Contains(p.ShippingMethodId))
                    {
                        shippingMethodIds.Add(p.ShippingMethodId);
                    }
                    //Update:zhengsong
                    //如果是广州小包，并且是地址标签的 系统分配 发件人信息
                    var shipp = shippingMethodList.FirstOrDefault(z => z.ShippingMethodId == p.ShippingMethodId);
                    if (shipp != null && typeId == "DT1308100021" && (shipp.Code == "CNPOST-GZ" || shipp.Code == "CNPOSTP-GZ"))
                    {
                        var gzPacketAddressInfo = _wayBillTemplateService.GetGZPacketAddressInfo().FirstOrDefault();
                        if (gzPacketAddressInfo != null)
                        {
                            p.Address = gzPacketAddressInfo.Address;
                            p.Name = gzPacketAddressInfo.Name;
                            gzPacketAddressInfo.Number = (gzPacketAddressInfo.Number + 1);
                            _wayBillTemplateService.UpdateGZPacketAddressInfo(gzPacketAddressInfo);
                        }

                    }
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

        [HttpPost]
        public ActionResult LoadPrintData(PrinterViewModel viewModel)
        {
            PrinterViewModel model;
            model = BindPrinterViewModel(viewModel.TypeId, viewModel.Type, viewModel.Ids, viewModel.TemplateName);
            TempData["PrinterViewModel"] = model;
            return PartialView("_PrintList", model);
        }

        private PrinterViewModel GetQuickPrintModel(string templateName, string number)
        {
            PrinterViewModel model;

            var wayBillInfo = _wayBillInfoRepository.GetWayBill(number);

            string typeId = _wayBillTemplateService.GetWayBillTemplateByNameAndShippingMethod(templateName, wayBillInfo.InShippingMethodID.Value).FirstOrDefault().TemplateTypeId;

            string customerOrderID = wayBillInfo.CustomerOrderID.Value.ToString();

            model = BindPrinterViewModel(typeId, 2, customerOrderID, templateName);

            return model;
        }

        public ActionResult QuickPrint(string templateName, string number)
        {
            var model = GetQuickPrintModel(templateName, number);
            return PartialView("_PrintList", model);
        }

        [HttpPost]
        public ActionResult PrintPreview(PrinterViewModel viewModel)
        {
            PrinterViewModel model;
            //var list = CacheHelper.Get("cache_countryList") as List<Country>;
            if (TempData["PrinterViewModel"] != null)
            {
                model = TempData["PrinterViewModel"] as PrinterViewModel;
            }
            else
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                model = BindPrinterViewModel(viewModel.TypeId, viewModel.Type, viewModel.Ids, viewModel.TemplateName);
                sw.Stop();
                string aa = sw.ElapsedMilliseconds.ToString();
            }
            return View(model);
        }

        #endregion

        //public ActionResult Print(string ids)
        //{
        //    string[] arr = ids.Split(',');
        //    var list = new List<CustomerOrderInfoModel>();
        //    foreach (string item in arr)
        //    {
        //        int id;
        //        if (!string.IsNullOrWhiteSpace(item) && int.TryParse(item, out id))
        //        {
        //            var model = GetPrinterInfo(id);
        //            if (model != null)
        //            {
        //                list.Add(model);
        //            }
        //        }
        //    }
        //    return View("Printer", list);
        //}

        #endregion

        #region Waybill

        public ActionResult WaybillList(WayBillListFilterModel filter)
        {
            if (IsPostRequest)
                filter.Page = 1;

            var viewModel = new WayBillListViewModel { Filter = filter };
            var list = _orderService.GetWayBillInfoPagedList(new OrderListParam
                {

                    CountryCode = filter.CountryCode,
                    StartTime = filter.StartTime,
                    IsHold = false,
                    EndTime = filter.EndTime,
                    ShippingMethodId = filter.ShippingMethodId,
                    Status = filter.Status,
                    CustomerCode = _workContext.User.UserUame,
                    SearchContext = filter.SearchContext,
                    SearchWhere = filter.SearchWhere,
                    Page = filter.Page,
                    PageSize = filter.PageSize
                });
            viewModel.PagedList = list.ToModelAsPageCollection<WayBillInfo, WayBillInfoModel>();

            InitAdd();

            return PartialView(viewModel);
        }

        public ActionResult Submitted(CustomerOrderFilter filter)
        {
            filter.Status = CustomerOrder.StatusEnum.Submitted.GetStatusValue();
            filter.IsSubmitted = true;

            return View(List(filter));
        }

        public ActionResult Received(CustomerOrderFilter filter)
        {
            filter.Status = CustomerOrder.StatusEnum.Have.GetStatusValue();
            filter.IsReceived = true;
            return View(List(filter));
        }

        public ActionResult Send(CustomerOrderFilter filter)
        {
            filter.Status = CustomerOrder.StatusEnum.Send.GetStatusValue();
            filter.IsDeliver = true;
            return View(List(filter));
        }

        public ActionResult All(CustomerOrderFilter filter)
        {
            filter.IsAll = true;
            return View(List(filter));
        }
        public ActionResult Blocked(CustomerOrderFilter filter)
        {
            filter.IsHold = true;

            return View(List(filter));
        }

        [HttpPost]
        public ActionResult HoldOn(int id, string msg)
        {
            return Content(_customerOrderService.IsHold(id, msg) ? "true" : "false");
        }

        [HttpPost]
        public ActionResult BatchHoldOn(string ids, string msg)
        {
            if (!string.IsNullOrWhiteSpace(ids))
            {
                var arr = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<int> list = new List<int>();
                foreach (var val in arr)
                {
                    list.Add(int.Parse(val));
                }
                bool isSucess = _customerOrderService.BatchHold(list, msg);
                return Content(isSucess ? "true" : "false");

            }
            return Content("false");
        }

        #endregion
        #region EubWayBillList

        public ActionResult EubWayBillList(EubWayBillFilter filter)
        {
            return View(BindEubWayBillList(filter));
        }
        [HttpPost]
        [ActionName("EubWayBillList")]
        [FormValueRequired("ApplyWayBill")]
        public ActionResult BatchApplyWayBill(EubWayBillFilter filter)
        {
            string printFormatValue;
            switch (filter.PrintFormat)
            {
                case 1:
                    printFormatValue = "00";
                    break;
                case 2:
                    printFormatValue = "01";
                    break;
                case 3:
                    printFormatValue = "03";
                    break;
                default:
                    printFormatValue = "00";
                    break;

            }
            if (!string.IsNullOrWhiteSpace(filter.WayBillNumbers))
            {
                var arr = filter.WayBillNumbers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var eubWayBillList = _customerOrderService.GetEUBWayBillList(arr, _workContext.User.UserUame).ToList();
                EubWayBillParam param = new EubWayBillParam()
                {
                    WayBillInfos = eubWayBillList,
                    PrintFormat = filter.PrintFormat,
                    PrintFormatValue = printFormatValue

                };
                var weyBilllist = _customerOrderService.ApplyEubWayBillInfo(param);
                if (weyBilllist.Count > 0)
                {
                    ErrorNotification(string.Join(",", weyBilllist.ToArray()) + "运单申请失败");

                }
                else
                {
                    SuccessNotification("申请成功");
                }

                //_eubWayBillService.StaticLabelDowLoad();//下载标签

            }
            filter.WayBillNumbers = "";
            return View(BindEubWayBillList(filter));
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
            var list = CacheHelper.Get("cache_countryList") as List<Country>;
            if (list == null)
            {
                list = _countryService.GetCountryList("");
                CacheHelper.Insert("cache_countryList", list);
            }
            return list;
        }

        private List<MouthCountry> GetMouthCountryList()
        {
            var list = CacheHelper.Get("cache_MouthCountryList") as List<MouthCountry>;
            if (list == null)
            {
                list = _countryService.GetMouthCountryList();
                CacheHelper.Insert("cache_MouthCountryList", list);
            }
            return list;
        }

        [HttpPost]
        public JsonResult BatchPrintOrder(string wayBillNumbers)
        {
            _eubWayBillService.StaticLabelDowLoad(); //下载标签
            var model = new ResponseResult() { Result = false, Message = "运单号不能为空" };
            var errorlist = new List<string>();
            if (!string.IsNullOrWhiteSpace(wayBillNumbers))
            {
                var arr = wayBillNumbers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> fileName = new List<string>();
                List<string> successWayBillNumbers = new List<string>();
                //var eubWayBillList = _customerOrderService.GetWayBillList(arr, _workContext.User.UserUame).ToList();

                //foreach (var item in eubWayBillList)
                //{
                //    fileName.Add(sysConfig.PdfTemplatePath + item.TrackingNumber + ".pdf");
                //}
                // update huzhiping 2014-07-05

                foreach (var wayBillNumber in arr)
                {

                    if (System.IO.File.Exists(sysConfig.PdfTemplatePath + wayBillNumber + ".pdf"))
                    {
                        if (IsFileInUse(sysConfig.PdfTemplatePath + wayBillNumber + ".pdf"))
                        {
                            errorlist.Add(wayBillNumber);
                            Log.Error("{0}文件已被占用!".FormatWith(sysConfig.PdfTemplatePath + wayBillNumber + ".pdf"));
                        }
                        else
                        {
                            fileName.Add(sysConfig.PdfTemplatePath + wayBillNumber + ".pdf");
                            successWayBillNumbers.Add(wayBillNumber);
                        }
                    }
                    else
                    {
                        errorlist.Add(wayBillNumber);
                        Log.Error("{0}文件不存在！".FormatWith(sysConfig.PdfTemplatePath + wayBillNumber + ".pdf"));
                    }
                }
                string zipFileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".zip";
                try
                {
                    Zip.ZipMultiFile(fileName.ToArray(), sysConfig.PdfTemplatePath + zipFileName, 5);

                    //更改状态,改成已经打印
                    _customerOrderService.UpdateEubWayBillInfoStatus(successWayBillNumbers.ToList(), (int)EubWayBillApplicationInfo.StatusEnum.Printer);

                    model.Result = true;
                    model.Message = "运单批量下载成功";
                    model.Url = sysConfig.PdfTemplateWebPath + zipFileName;
                }

                catch (Exception e)
                {
                    Log.Exception(e);
                    model.Result = false;
                    model.Message = "运单批量下载失败";
                }
            }
            if (errorlist != null && errorlist.Count > 0)
            {
                model.Message = model.Message + " 没有生成运单PDF的运单号：" + string.Join(",", errorlist);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BatchApplyOrder(string wayBillNumbers, int printFormat)
        {
            var model = new ResponseResult() { Result = false, Message = "运单号不能为空" };
            string printFormatValue;
            switch (printFormat)
            {
                case 1:
                    printFormatValue = "00";
                    break;
                case 2:
                    printFormatValue = "01";
                    break;
                case 3:
                    printFormatValue = "03";
                    break;
                default:
                    printFormatValue = "00";
                    break;

            }
            if (!string.IsNullOrWhiteSpace(wayBillNumbers))
            {
                var arr = wayBillNumbers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var eubWayBillList = _customerOrderService.GetWayBillList(arr, _workContext.User.UserUame).ToList();
                EubWayBillParam param = new EubWayBillParam()
                    {
                        WayBillInfos = eubWayBillList,
                        PrintFormat = printFormat,
                        PrintFormatValue = printFormatValue

                    };
                var weyBilllist = _customerOrderService.ApplyEubWayBillInfo(param);
                if (weyBilllist.Count > 0)
                {
                    model.Result = false;
                    model.Message = string.Join(",", weyBilllist.ToArray()) + "运单申请失败";

                }
                else
                {
                    model.Result = true;
                    model.Message = "运单申请成功";
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public EubWayBillViewModel BindEubWayBillList(EubWayBillFilter filter)
        {

            if (IsPostRequest)
                filter.Page = 1;

            var viewModels = new EubWayBillViewModel() { Filter = filter, ShippingMethods = GetShippingMethodList(), CountryList = GetCountryList("") };
            EubWayBillApplicationInfo.GetPrintFormatList().ForEach(p => viewModels.PrintFormatList.Add(new SelectListItem { Value = p.ValueField, Text = p.TextField }));
            EubWayBillApplicationInfo.GetStatusList().ForEach(p => viewModels.StatusList.Add(new SelectListItem { Value = p.ValueField, Text = p.TextField }));
            EubWayBillApplicationInfo.GetQueryNumberList().ForEach(p => viewModels.QueryNumberList.Add(new SelectListItem { Value = p.ValueField, Text = p.TextField }));
            EubWayBillApplicationInfo.GetTimeQueryList().ForEach(p => viewModels.TimeTypeList.Add(new SelectListItem { Value = p.ValueField, Text = p.TextField }));
            viewModels.StatusList.Insert(0, new SelectListItem { Value = "", Text = "" });
            var param = new EubWayBillApplicationInfoParam
            {
                CountryCode = filter.CountryCode,
                CreatedOnFrom = filter.CreatedOnFrom,
                CreatedOnTo = (filter.CreatedOnTo.HasValue ? filter.CreatedOnTo.Value.ToString("yyyy-MM-dd 23:59:59") : null).ConvertTo<DateTime?>(),
                ShippingMethodId = filter.ShippingMethodId,
                PrintFormat = filter.PrintFormat,
                TimeType = filter.TimeType,
                Status = filter.Status,
                Page = filter.Page,
                PageSize = filter.PageSize,
                CustomerCode = _workContext.User.UserUame
            };

            foreach (var item in viewModels.ShippingMethods.Where(item => !string.IsNullOrWhiteSpace(item.Value)))
            {
                int id;
                int.TryParse(item.Value, out id);
                param.ShippingMethods.Add(id);
            }

            switch (filter.QueryNumber)
            {
                case (int)EubWayBillApplicationInfo.QueryNumberEnum.WayBillNumber:
                    param.WayBillNumber = filter.Numbers;
                    break;
                case (int)EubWayBillApplicationInfo.QueryNumberEnum.TrackNumber:
                    param.TrackNumber = filter.Numbers;
                    break;
                case (int)EubWayBillApplicationInfo.QueryNumberEnum.OrderNumber:
                    param.CustomerOrderNumber = filter.Numbers;
                    break;
                case (int)EubWayBillApplicationInfo.QueryNumberEnum.BatchNumber:
                    param.BatchNumber = filter.Numbers;
                    break;

            }
            var maxCustomerOrderId = 0;
            if (filter.Page > 1)
            {
                if (Session["MaxCustomerOrderId"] != null)
                {
                    maxCustomerOrderId = int.Parse(Session["MaxCustomerOrderId"].ToString());
                }
                else
                {
                    Session["MaxCustomerOrderId"] = _customerOrderService.GetMaxCustomerOrderID();
                }
            }
            else
            {
                Session["MaxCustomerOrderId"] = _customerOrderService.GetMaxCustomerOrderID();
            }
            var list = _customerOrderService.GetEubWayBillList(param,maxCustomerOrderId);

            viewModels.PagedList = list.ToModelAsPageCollection<EubWayBillApplicationInfoExt, EubWayBillApplicationInfoModel>();

            return viewModels;
        }


        #endregion

        #region

        public ActionResult DownloadCountries()
        {
            var list = _countryService.GetCountryList("");

            ExcelHelper.WriteToDownLoad(string.Format("Countries_{0}.xls", DateTime.Now.ToString("yyMMdd")), "Sheet1", list, new List<string> { "CountryCode", "Name", "ChineseName" });

            return Content("");
        }

        public ActionResult DownloadShippingMethod()
        {
            var shippingMethods = _freightService.GetShippingMethods(null, true);
            ExcelHelper.WriteToDownLoad(string.Format("ShippingMethod_{0}.xls", DateTime.Now.ToString("yyMMdd")), "Sheet1", shippingMethods, new List<string> { "FullName", "EnglishName", "Code" });

            return Content("");
        }

        public ActionResult DownloadExcelTemplate()
        {
            string filePath = sysConfig.ExcelTemplatePath + sysConfig.LMSCustomerOrderUploadTemplate;
            var shippingMethods = _freightService.GetShippingMethods(null, true);
            var target = shippingMethods.Where(t => t.Enabled == true).Distinct().ToList();
            ExportExcelByWeb.WriteExcelTemplateData<ShippingMethodModel>(filePath, target, new List<string> { "FullName", "EnglishName", "Code" }, null, 1, false, "运输方式清单");
            //ExcelHelper.WriteToDownLoad(string.Format("ShippingMethod_{0}.xls", DateTime.Now.ToString("yyMMdd")), "Sheet1", shippingMethods, new List<string> { "FullName", "EnglishName", "Code" });

            return Content("");
        }

        public ActionResult DownloadExcelTemplatePackage()
        {
            string filePath = sysConfig.ExcelTemplatePath + sysConfig.LMSCustomerPackageUploadTemplate;
            var shippingMethods = _freightService.GetShippingMethods(null, true);
            var target = shippingMethods.Where(t => t.Enabled == true).Distinct().ToList();
            ExportExcelByWeb.WriteExcelTemplateData<ShippingMethodModel>(filePath, target, new List<string> { "FullName", "EnglishName", "Code" }, null, 1, false, "运输方式清单");
            //ExcelHelper.WriteToDownLoad(string.Format("ShippingMethod_{0}.xls", DateTime.Now.ToString("yyMMdd")), "Sheet1", shippingMethods, new List<string> { "FullName", "EnglishName", "Code" });

            return Content("");
        }

        public ActionResult DownloadExcelTemplateSinoUS()
        {
            string filePath = sysConfig.ExcelTemplatePath + sysConfig.LMSCustomerSinoUSUploadTemplate;
            var shippingMethods = _freightService.GetShippingMethods(null, true);
            var target = shippingMethods.Where(t => t.Enabled == true).Distinct().ToList();
            ExportExcelByWeb.WriteExcelTemplateData<ShippingMethodModel>(filePath, target, new List<string> { "FullName", "EnglishName", "Code" }, null, 1, false, "运输方式清单");
            //ExcelHelper.WriteToDownLoad(string.Format("ShippingMethod_{0}.xls", DateTime.Now.ToString("yyMMdd")), "Sheet1", shippingMethods, new List<string> { "FullName", "EnglishName", "Code" });

            return Content("");
        }

        #endregion

        #region 生成128条形码

        public ActionResult BarCode128H(string code)
        {
            var builder = new BarCodeBuilder { SymbologyType = Symbology.Code128, CodeText = code, xDimension = 0.3f };
            builder.CodeTextFont = new Font("宋体", 12, FontStyle.Bold);
            Image image = builder.BarCodeImage;
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            image.Dispose();
            byte[] bytes = ms.ToArray();
            ms.Dispose();
            return File(bytes, "image/Gif");
        }

        public ActionResult BarCode128(string code, int dpix = 80, int dpiy = 75, bool showText = false)
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
            Image image = builder.BarCodeImage;

            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            image.Dispose();
            byte[] bytes = ms.ToArray();
            ms.Dispose();
            return File(bytes, "image/Bmp");
        }
        #endregion

        #region 生成39条形码
        public ActionResult BarCode39(string code, int dpix = 80, int dpiy = 80, bool showText = false)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return Content("无号码");
            }

            var builder = new BarCodeBuilder { SymbologyType = Symbology.Code39Standard, CodeText = code };
            builder.Resolution.DpiX = dpix;
            builder.Resolution.DpiY = dpiy;

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
        #endregion

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
    }

    public class ResponseResult
    {
        public bool Result { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public string TrackNumber { get; set; }
    }

    public class JsonModelResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string HtmlString { get; set; }
    }

}
