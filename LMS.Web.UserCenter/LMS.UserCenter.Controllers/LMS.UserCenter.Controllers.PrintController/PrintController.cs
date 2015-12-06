using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Aspose.BarCode;
using LighTake.Infrastructure.Common.BizLogging;
using LighTake.Infrastructure.Common.BizLogging.Enums;
using LMS.Core;
using LMS.Data.Context;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LMS.Services.CountryServices;
using LMS.Services.CustomerOrderServices;
using LMS.Services.CustomerServices;
using LMS.Services.FreightServices;
using LMS.Services.OperateLogServices;
using LMS.Services.WayBillTemplateServices;
using LMS.UserCenter.Controllers.OrderController.Models;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Web.Filters;
using LighTake.Infrastructure.Common.Logging;
using RazorEngine;
using CustomerOrderInfoModel = LMS.UserCenter.Controllers.OrderController.Models.CustomerOrderInfoModel;

namespace LMS.UserCenter.Controllers.PrintController
{
    public class PrintController : Controller
    {
        private readonly ICustomerOrderService _customerOrderService;
        private readonly IFreightService _freightService;
        private readonly ICountryService _countryService;
        private readonly IWayBillTemplateService _wayBillTemplateService;
        private readonly ICustomerService _customerService;
        private readonly IWayBillInfoRepository _wayBillInfoRepository;
		private readonly IOperateLogServices _operateLogServices;
		private readonly IWorkContext _workContext;

        public PrintController(
            ICustomerOrderService customerOrderService,
            IFreightService freightService,
            ICountryService countryService,
            IWayBillTemplateService wayBillTemplateService,
            ICustomerService customerService,
            IWayBillInfoRepository wayBillInfoRepository,
			IOperateLogServices operateLogServices,
			IWorkContext workContext)
        {
            _customerOrderService = customerOrderService;
            _freightService = freightService;
            _countryService = countryService;
            _wayBillTemplateService = wayBillTemplateService;
            _customerService = customerService;
            _wayBillInfoRepository = wayBillInfoRepository;
	        _operateLogServices = operateLogServices;
	        _workContext = workContext;
        }


        #region 模板打印

        public ActionResult Printer(string typeId, int type, string ids)
        {
            var viewModel = BindPrinterViewModel(typeId, type, ids);
            return View("Printer", viewModel);
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
            var model = new JsonModelResult {IsSuccess = false, Message = string.Empty, HtmlString = string.Empty};
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
                    Razor.Parse(HttpUtility.HtmlDecode(wayBillTemplateModel.TemplateContent),
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
            list.ForEach(
                p => viewModel.PrintTemplate.Add(new SelectListItem() {Text = p.TemplateName, Value = p.TemplateName}));
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

        private bool GetCanPrint(string templateName, string number)
        {
            return _wayBillTemplateService.GetCanPrint(templateName, number);
        }

        private PrinterViewModel GetQuickPrintModel(string templateName, string number)
        {
            if (string.IsNullOrWhiteSpace(templateName) || string.IsNullOrWhiteSpace(number))
            {
                throw new Exception("模板名称或单号为空");
            }

            var wayBillInfo = _wayBillInfoRepository.GetWayBill(number);

            if (wayBillInfo == null || wayBillInfo.CustomerOrderID == null)
            {
                throw new Exception("没有找到该运单号或订单未提交");
            }

            if (wayBillInfo.CustomerOrderInfo.Status == (int) CustomerOrder.StatusEnum.Delete)
            {
                throw new Exception("订单已删除");
            }

            //转为充血模型
            var customerOrderInfoModel = DecorateCustomerOrderInfo(wayBillInfo.CustomerOrderInfo);



            PrinterViewModel model = new PrinterViewModel()
            {
                CustomerOrderInfoModels = new List<CustomerOrderInfoModel>() {customerOrderInfoModel},
                Type = 2,
            };

            var wayBillTemplate = _wayBillTemplateService.GetWayBillTemplate(wayBillInfo.InShippingMethodID.Value,
                templateName);

            if (wayBillTemplate == null)
            {
                throw new Exception("没有找到相应模板");
            }

            if (!wayBillTemplate.Countries.Contains(customerOrderInfoModel.CountryCode))
            {
                throw new Exception("模板不支持该国家");
            }

            model.WayBillTemplates = new List<WayBillTemplateExt> {wayBillTemplate};

            //Update:zhengsong
            //如果是广州小包，并且是地址标签的 系统分配 发件人信息
            var shippingMethodList = _freightService.GetShippingMethods(null, false);
            model.CustomerOrderInfoModels.ForEach(p =>
                {
                    var shipp = shippingMethodList.FirstOrDefault(z => z.ShippingMethodId == customerOrderInfoModel.ShippingMethodId);

                    if (shipp != null && wayBillTemplate.TemplateTypeId == "DT1308100021" && (shipp.Code == "CNPOST-GZ" || shipp.Code == "CNPOSTP-GZ"))
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

            return model;
        }

        private PrinterViewModel GetQuickPrintModel(string templateName, string number, string cacheKey)
        {
            if (string.IsNullOrWhiteSpace(cacheKey)) return GetQuickPrintModel(templateName, number);

            var cacheModel = CacheHelper.Get(cacheKey) as PrinterViewModel;

            if (cacheModel != null)
            {
                return cacheModel;
            }

            return GetQuickPrintModel(templateName, number);
        }

        public ActionResult QuickPrint(string templateName, string number, string cacheKey = "")
        {
            PrinterViewModel model = new PrinterViewModel();

            try
            {
                model = GetQuickPrintModel(templateName, number, cacheKey);

                var customerOrderInfo = model.CustomerOrderInfoModels[0];

                if (!customerOrderInfo.IsPrinted)
                {
                    //异步修改订单信息
                    ThreadPool.QueueUserWorkItem(ModifyCustomerOrderToPrinted, customerOrderInfo.CustomerOrderID);
                }

				//#region 操作日志
				////yungchu
				////敏感字-无
				//BizLog bizlog = new BizLog()
				//{
				//	Summary = "扫描打印",
				//	KeywordType = KeywordType.CustomerOrderNumber,
				//	Keyword = number,
				//	UserCode = _workContext.User.UserUame,
				//	UserRealName = _workContext.User.UserUame,
				//	UserType = UserType.LMS_User,
				//	SystemCode = SystemType.LMS,
				//	ModuleName = "扫描打印"
				//};

				//_operateLogServices.WriteLog(bizlog, model);
				//#endregion

            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

            return PartialView("Printer", model);
        }

        public ActionResult CanPrint(string templateName, string number)
        {
            ResponseResult result = new ResponseResult();

            try
            {
                //model = GetQuickPrintModel(templateName, number);

                //string cacheId = SaveObjectToCache(model);
                
                if (GetCanPrint(templateName, number))
                {
                    result.Result = true;
                    result.Message = ""; //cacheId;
                }
                else //如果不能打印，返回详情
                {
                    GetQuickPrintModel(templateName, number);
                }
                var wayBillInfo = _wayBillInfoRepository.GetWayBill(number);
                var shippingMethodList = _freightService.GetShippingMethods(null, false);
                WayBillTemplateExt wayBillTemplate=new WayBillTemplateExt();
                ShippingMethodModel shipp=new ShippingMethodModel();
                if (wayBillInfo != null)
                {
                    wayBillTemplate = _wayBillTemplateService.GetWayBillTemplate(wayBillInfo.InShippingMethodID ?? 0, templateName);
                    shipp = shippingMethodList.FirstOrDefault(z => z.ShippingMethodId == wayBillInfo.InShippingMethodID);
                }
                if (shipp != null && wayBillTemplate != null && wayBillTemplate.TemplateTypeId == "DT1308100021" && (shipp.Code == "CNPOST-GZ" || shipp.Code == "CNPOSTP-GZ"))
                {
                    
                    var gzPacketAddress = _wayBillTemplateService.GetGZPacketAddressInfo().FirstOrDefault();
                    if (gzPacketAddress == null)
                    {
                        result.Result = false;
                        result.Message = "广州小包发货地址不足";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;

                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改订单状态为已打印
        /// </summary>
        /// <param name="customerOrderID"></param>
        private void ModifyCustomerOrderToPrinted(object customerOrderID)
        {
            try
            {
                int orderID = Convert.ToInt32(customerOrderID);

                var customerOrderInfoRepository = new CustomerOrderInfoRepository(new LMS_DbContext());//TODO：不能直接使用数据仓库，请修改
                var orderInfo = customerOrderInfoRepository.GetList(p => p.CustomerOrderID == orderID).Single();

                if (!orderInfo.IsPrinted)
                {
                    orderInfo.IsPrinted = true;
                    orderInfo.LastUpdatedBy = orderInfo.CustomerCode;
                    orderInfo.LastUpdatedOn = DateTime.Now;
                    customerOrderInfoRepository.Modify(orderInfo);
                    customerOrderInfoRepository.UnitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// 保证对象到缓存
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>缓存key</returns>
        private string SaveObjectToCache(object obj)
        {
            var guid = Guid.NewGuid().ToString();
            CacheHelper.Insert(guid, obj);
            return guid;
        }

        private CustomerOrderInfoModel GetPrinterByOrderNumber(string customerOrderNumber)
        {
            try
            {
                var entity = _customerOrderService.PrintByCustomerOrderNumber(customerOrderNumber);
                if (entity == null) return null;
                var model = entity.ToModel<CustomerOrderInfoModel>();
                if (string.IsNullOrWhiteSpace(model.TrackingNumber))
                {
                    var firstOrDefault = entity.WayBillInfos.FirstOrDefault();
                    if (firstOrDefault != null)
                        model.TrackingNumber = firstOrDefault.WayBillNumber;
                }
                model.BarCode = "<img id=\"img\" src=\"/barcode.ashx?m=0&h=35&vCode=" + model.TrackingNumber + "\" alt=\"" + model.TrackingNumber + "\" style=\"width:200px;height:35px;\" />";
                entity.ShippingInfo.ToModel(model);
                var country = GetCountryList().Single(c => c.CountryCode == entity.ShippingInfo.CountryCode);
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
        /// 获取要打印的数据
        /// </summary>
        /// <param name="customerOrderIds"></param>
        /// <returns></returns>
        private List<CustomerOrderInfoModel> GetPrinterList(IEnumerable<int> customerOrderIds)
        {
            return GetCustomerOrderListModel(customerOrderIds.ToArray());

            //var cacheList = CacheHelper.Get("cache_customerOrderIds") as int[];
            //var orderIds = customerOrderIds as int[] ?? customerOrderIds.ToArray();
            //if (orderIds.Length <= 0) return null;
            //if (null == cacheList)
            //{
            //    CacheHelper.Insert("cache_customerOrderIds", orderIds);


            //    var cacheOrderList = GetCustomerOrderListModel(orderIds);

            //    if (cacheOrderList != null)
            //        CacheHelper.Insert("cache_customerOrders", cacheOrderList);
            //    return cacheOrderList;
            //}
            //else
            //{
            //    if (Tools.CompareArrContent(orderIds, cacheList))
            //    {
            //        var cacheOrderList = CacheHelper.Get("cache_customerOrders") as List<CustomerOrderInfoModel>;
            //        if (null == cacheOrderList)
            //        {
            //            cacheOrderList = GetCustomerOrderListModel(orderIds);
            //            if (cacheOrderList != null)
            //                CacheHelper.Insert("cache_customerOrders", cacheOrderList);
            //        }
            //        return cacheOrderList;
            //    }
            //    else
            //    {
            //        var cacheOrderList = GetCustomerOrderListModel(orderIds);
            //        CacheHelper.Insert("cache_customerOrderIds", orderIds);
            //        if (cacheOrderList != null)
            //            CacheHelper.Insert("cache_customerOrders", cacheOrderList);
            //        return cacheOrderList;
            //    }
            //}
        }

        private List<CustomerOrderInfoModel> GetCustomerOrderListModel(int[] orderIds)
        {

            try
            {
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
            if (string.IsNullOrWhiteSpace(postCode))return "";
            if (countryCode.ToUpper() != "RU") return "";

            string[] stpPostCodes="16,17,18,19".Split(',');
            string[] sibPostCodes="3,4, 60,61,62,63,64,65,66,67,68,69".Split(',');

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

        [HttpPost]
        public ActionResult LoadPrintData(PrinterViewModel viewModel)
        {
            PrinterViewModel model;
            model = BindPrinterViewModel(viewModel.TypeId, viewModel.Type, viewModel.Ids, viewModel.TemplateName);
            TempData["PrinterViewModel"] = model;
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
