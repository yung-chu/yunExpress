using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aspose.BarCode;
using LMS.Core;
using LMS.Data.Entity;
using LMS.PrintLabelWeb.Models;
using LMS.PrintLabelWeb.UserCenter;
using LMS.Services.CountryServices;
using LMS.Services.CustomerServices;
using LMS.Services.FreightServices;
using LMS.Services.WayBillTemplateServices;
using LighTake.Infrastructure.Common.Caching;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.PrintLabelWeb.Controllers
{
    [LMSAuth]
    public class PrintController : Controller
    {
        private readonly IWayBillTemplateService _wayBillTemplateService;
        private readonly ICustomerService _customerService;
        private readonly IFreightService _freightService;
        private readonly ICountryService _countryService;
        public PrintController(IWayBillTemplateService wayBillTemplateService, ICustomerService customerService,
            IFreightService freightService, ICountryService countryService)
        {
            _wayBillTemplateService = wayBillTemplateService;
            _customerService = customerService;
            _freightService = freightService;
            _countryService = countryService;
        }
        //
        // GET: /Print/

        public ActionResult AddressLabelPrint(string orderNumbers)
        {
            var typeId = ConfigurationManager.AppSettings["AddressLabelTemplateTypeId"];
            var viewModel = BindPrinterViewModel(typeId, 0, orderNumbers);
            return View("AddressLabelPrint", viewModel);
        }

        [HttpPost]
        public ActionResult LoadPrintData(PrinterViewModel viewModel)
        {
            PrinterViewModel model;
            model = BindPrinterViewModel(viewModel.TypeId, viewModel.Type, viewModel.Ids, viewModel.TemplateName);
            TempData["PrinterViewModel"] = model;
            return PartialView("_PrintList", model);
        }

        //[HttpPost]
        //public ActionResult PrintPreview(PrinterViewModel viewModel)
        //{
        //    PrinterViewModel model;
        //    //var list = CacheHelper.Get("cache_countryList") as List<Country>;
        //    if (TempData["PrinterViewModel"] != null)
        //    {
        //        model = TempData["PrinterViewModel"] as PrinterViewModel;
        //    }
        //    else
        //    {
        //        model = BindPrinterViewModel(viewModel.TypeId, viewModel.Type, viewModel.Ids, viewModel.TemplateName);
        //    }
        //    return View(model);
        //}

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

        public ActionResult BarCode128(string code, int dpix = 80, int dpiy = 75, bool showText = false, float angleF=0)
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


        private PrinterViewModel BindPrinterViewModel(string typeId, int type, string ids, string templateName = "")
        {
            PrinterViewModel viewModel = new PrinterViewModel()
            {
                Type = type,
                TypeId = typeId,
                Ids = ids
            };

            var orderNumbers = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (!orderNumbers.Any())
            {
                return viewModel;
            }

            var selectList = new List<SelectListItem>();
            var shippingMethodIds = new List<int>();
            var list = GetCustomerOrderListModel(orderNumbers);
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
                SelectListItem item = selectList.FirstOrDefault(p => p.Selected) ?? selectList.First();
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

        private List<CustomerOrderInfoModel> GetCustomerOrderListModel(List<string> orderNumbers)
        {

            try
            {
                var list = _wayBillTemplateService.PrintByCustomerOrderNumbers(orderNumbers, HttpContext.User.Identity.Name);

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
            customerOrderInfoModel.MouthNumber = GetMouthCountryList().Find(c => c.CountryCode == customerOrderInfoModel.CountryCode).MouthNumber;
            customerOrderInfoModel.SortingIdentity = GetSortingIdentityHtml(model.ShippingInfo.ShippingZip, model.ShippingInfo.CountryCode);
            customerOrderInfoModel.BatteryIdentity = (model.IsBattery && (model.SensitiveTypeID == 1 || model.SensitiveTypeID == 2))
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
    }
}
