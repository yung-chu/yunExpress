using LMS.Services.WayBillTemplateServices;
using LMS.Data.Repository;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Caching;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Http;
using LighTake.Infrastructure.Http.Infrastructure;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LMS.Services.CustomerServices;
using LMS.Services.FreightServices;
using LMS.Services.InStorageServices;
using LMS.Services.OrderServices;
using LMS.Services.SequenceNumber;
using LMS.WebAPI.Client.Models;
using LMS.WebAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using LighTake.Infrastructure.Seedwork;

namespace LMS.WebAPI.Controllers
{
    public class WayBillController : BaseApiController
    {
        private readonly IInStorageService _inStorageService;
        private readonly IOrderService _orderService;
        private readonly IFreightService _freightService;
        private readonly ICustomerService _customerService;
private readonly IWayBillInfoRepository _wayBillInfoRepository;
        private readonly IWayBillTemplateService _wayBillTemplateService;
        public WayBillController(IInStorageService inStorageService, IOrderService orderService, IFreightService freightService, ICustomerService customerService,IWayBillTemplateService wayBillTemplateService,IWayBillInfoRepository wayBillInfoRepository)
        {
            _inStorageService = inStorageService;
            _orderService = orderService;
            _freightService = freightService;
            _customerService = customerService;
            _wayBillTemplateService = wayBillTemplateService;
            _wayBillInfoRepository = wayBillInfoRepository;
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


        /// <summary>
        /// CS 检查运单是否正确  add by huhaiyou 2014-4-24
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        public CheckOnWayBillResponseResult CheckOnWayBillCS(InStorageFormModel filter)
        {
            var model = new CheckOnWayBillResponseResult();
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
                    var waybillinfo = _wayBillInfoRepository.GetWayBillInfoExtSilm(NumberStr);

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

                            var chineseName = GetGetCountryListFromCache().Find(p => p.CountryCode == waybillinfo.CountryCode).ChineseName;


                            //var areaIdList =
                            //    _freightService.GetCountryArea(filter.ShippingMethodId, waybillinfo.CountryCode)
                            //                   .FirstOrDefault();


                            var listShippingMethodCountryModel = GetShippingMethodCountriesFromCache(filter.ShippingMethodId);
                            var areaIdList = listShippingMethodCountryModel.Find(p => p.CountryCode == waybillinfo.CountryCode);

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
                        
                        model.ShippingInfo = new ShippingInfoModel();

                        _wayBillInfoRepository.Get(waybillinfo.WayBillNumber).ShippingInfo.CopyTo(model.ShippingInfo);
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
                            CreatedBy = filter.OperatorUserName,
                            LastUpdatedOn = DateTime.Now,
                            LastUpdatedBy = filter.OperatorUserName,
                        };
                        _orderService.UpdateNoForecastAbnormal(noForecastAbnormal);

                        model.Result = false;
                        model.Message = string.Format("单号：{0}无预报，不能扫描入仓！", NumberStr);
                    }
                }
            }
            return model;
        }

        [HttpPost]
        /// <summary>
        /// CS版 检查入库 add by huhaiyou 2014-4-23
        /// </summary>
        /// <returns></returns>
        public InStorageWayBillModel CheckOnInStorageCS(InStorageFormModel filter)
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
            var waybillinfo = _inStorageService.GetWayBillInfo(NumberStr, filter.CustomerCode); //.ToModel<WayBillInfoModel>();
            if (!string.IsNullOrWhiteSpace(NumberStr) && waybillinfo != null)
            {
                //if (!string.IsNullOrWhiteSpace(filter.TrackingNumber) && waybillinfo.TrackingNumber != filter.TrackingNumber)
                //{
                //    error.AppendLine("跟踪号与输入的跟踪号不一样！<br/>");
                //}_orderService.AddAbnormalWayBill
                if (error.Length == 0 && WayBill.StatusToValue(WayBill.StatusEnum.Submitted) != waybillinfo.Status)
                {
                    error.AppendLine(string.Format("运单:{0}状态不是已提交！<br/>", NumberStr));
                }
                if (error.Length == 0 && waybillinfo.GoodsTypeID != filter.GoodsTypeID)
                {
                    error.AppendLine(string.Format("运单：{0}货物类型不是一致", NumberStr));
                }
                if (error.Length == 0 && waybillinfo.IsHold)
                {
                    error.AppendLine(string.Format("运单:{0}已经Hold！<br/>", NumberStr));
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
                    Customer customer = _customerService.GetCustomer(waybillinfo.CustomerCode);
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
                        CustomerId = customerId

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
                        model.Surcharge = result.RemoteAreaFee + result.OtherFee; //附加费
                        model.SettleWeight = result.Weight; //结算重量
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

                        //if (filter.ChkPrint)
                        //{
                        //    var customerOrder = GetPrinterInfo(waybillinfo.CustomerOrderID);
                        //    if (customerOrder != null)
                        //    {
                        //        var wayBillTemplateModel = _wayBillTemplateService.GetWayBillTemplateByNameAndShippingMethod(filter.PrintTemplateName, filter.ShippingMethodId).FirstOrDefault();
                        //        if (wayBillTemplateModel != null)
                        //        {
                        //            //model.HtmlString = this.RenderPartialViewToString("_PrintTNTOrder", customerOrderInfosModels);
                        //            if (!string.IsNullOrWhiteSpace(customerOrder.TrackingNumber))
                        //            {
                        //                model.HtmlString =
                        //                    System.Web.WebPages.Razor.Parse(HttpUtility.HtmlDecode(wayBillTemplateModel.TemplateContent),
                        //                                customerOrder);
                        //            }
                        //        }
                        //    }
                        //}

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
            return getWeightDeviation != null ? getWeightDeviation.DeviationValue.Value : 0;
        }

        /// <summary>
        /// CS版 单个包裹检测 add by zxq 2014-6-14
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public InStorageWayBillModel CheckOnInStorageSingele(InStorageFormModel filter)
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
                if (error.Length == 0 && waybillinfo.IsHold)
                {
                    error.AppendLine(string.Format("运单:{0}已经Hold！<br/>", NumberStr));
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

                //小包检查重量偏差
                if (error.Length == 0 && !filter.IsBusinessExpress)
                {
                    //该客户该运输方式设置的重量偏差值
                    var weightDeviations = GetWeightDeviations(filter.CustomerCode, filter.ShippingMethodId);

                    //预报重量与称重的偏差
                    var diff =Math.Abs((filter.Weight - waybillinfo.Weight ?? 0)*1000);

                    if (weightDeviations != 0 && diff > weightDeviations)
                    {
                        string errMessage = string.Format("称重重量与预报重量相差" + diff + "g大于配置的 " + weightDeviations + "g不能入仓！");
                        error.AppendLine(errMessage);

                        //增加运单异常日志
                        _orderService.AddAbnormalWayBill(waybillinfo.WayBillNumber, WayBill.AbnormalTypeEnum.InStorageWeightAbnormal, errMessage, filter.OperatorUserName);

                        if (_orderService.IsExistInStorageWeightAbnormal(waybillinfo.WayBillNumber))
                        {
                            _orderService.UpdateInStorageWeightAbnormal(waybillinfo.WayBillNumber, filter.Weight);
                        }
                        else
                        {
                            //增加入仓对比重量异常单
                            _orderService.AddInStorageWeightAbnormal(new WeightAbnormalLog
                            {
                                CustomerCode = waybillinfo.CustomerCode,
                                WayBillNumber = waybillinfo.WayBillNumber,
                                CustomerOrderID = waybillinfo.CustomerOrderID,
                                TrackingNumber = waybillinfo.TrackingNumber,
                                Length = filter.Length ?? waybillinfo.Length,
                                Width = filter.Width ?? waybillinfo.Width,
                                Height = filter.Height ?? waybillinfo.Height,
                                Weight = filter.Weight,//称重重量
                                CreatedOn = DateTime.Now,
                                CreatedBy = filter.OperatorUserName
                            });

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

                    //判断是否超周长
                    var result = _freightService.IsOverMaxGirthSingle(new CustomerInfoPackageModel()
                    {
                        CountryCode = waybillinfo.CountryCode,
                        CustomerTypeId = filter.CustomerType,
                        Height = filter.Height ?? 0,
                        Length = filter.Length ?? 0,
                        ShippingMethodId = filter.ShippingMethodId,
                        Weight = filter.Weight,
                        Width = filter.Width ?? 0,
                        ShippingTypeId = filter.GoodsTypeID,
                        CustomerId = customerId

                    });

                    //如果没有超周长
                    if (!result.Result)
                    {
                        model.IsSuccess = true;
                        model.Message = "";
                        model.WayBillNumber = waybillinfo.WayBillNumber;
                        model.CountryCode = waybillinfo.CountryCode;
                        model.CountryName = _freightService.GetChineseName(waybillinfo.CountryCode);
                        model.CustomerOrderNumber = waybillinfo.CustomerOrderNumber;
                        model.SettleWeight = Convert.ToDecimal(result.Message); //结算重量
                        model.EnableTariffPrepay = waybillinfo.EnableTariffPrepay;//是否启用关税预付服务
                        model.TrackingNumber = !string.IsNullOrWhiteSpace(waybillinfo.TrackingNumber) ? waybillinfo.TrackingNumber : "";

                    }
                    else
                    {
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

        [HttpPost]
        /// <summary>
        /// CS版，快递入仓 add by huhaiyou 2014-4-25
        /// </summary>
        /// <param name="model"></param>
        /// <param name="wayBilllist"></param>
        /// <returns></returns>
        public ResponseResult CreateInStorageCS(InStorageSaveModel model)
        {
            var responseResult = new ResponseResult();
            if (model != null && model.WayBillInfoSaveList != null && model.WayBillInfoSaveList.Count > 0)
            {
                var inStorage = new CreateInStorageExtCS
                {
                    InStorage =
                    {
                        CustomerCode = model.CustomerCode,
                        InStorageID = SequenceNumberService.GetSequenceNumber(PrefixCode.InStorageID),
                        TotalQty = 0,
                        TotalWeight = 0,
                        CreatedBy = model.OperatorUserName,
                        ReceivingDate = model.ReceivingDate,
                    }
                };

                model.WayBillInfoSaveList.ForEach(w =>
                {
                    var shippingMethod = _freightService.GetShippingMethod(w.ShippingMethodId);

                    if (!string.IsNullOrWhiteSpace(w.WayBillNumber))
                    {
                        //根据转换重量转换成文件类型 Add by zhengsong
                        #region
                        if (shippingMethod != null)
                        {
                            if (shippingMethod.Enabled && shippingMethod.ShippingMethodTypeId == 4)//4-代表EMS
                            {
                                if (w.Weight <= shippingMethod.PackageTransformFileWeight)
                                {
                                    w.GoodsTypeID = 2;//2-代表文件类型
                                }
                            }
                        }
                        #endregion
                        var extmodel = new LMS.Data.Entity.ExtModel.WayBillInfoExt();
                        extmodel.CustomerCode = model.CustomerCode.Trim();
                        extmodel.CustomerType = model.CustomerType;
                        extmodel.GoodsTypeID = w.GoodsTypeID;
                        extmodel.Length = w.Length;
                        extmodel.Height = w.Height;
                        extmodel.Width = w.Width;
                        extmodel.Weight = w.Weight;
                        extmodel.ShippingMethodId = w.ShippingMethodId;
                        extmodel.TrackingNumber = w.TrackingNumber;
                        extmodel.SettleWeight = w.SettleWeight;
                        extmodel.WayBillNumber = w.WayBillNumber.Trim();
                        extmodel.PriceResult = w.PriceResult;
                        extmodel.IsBusinessExpress = w.IsBusinessExpress;
                        extmodel.IsBattery = w.IsBattery;
                        extmodel.SensitiveType = w.SensitiveType;

                        foreach (var package in w.waybillPackageDetailList)
                        {
                            WaybillPackageDetailExt packageModel = new WaybillPackageDetailExt();
                            packageModel.WayBillNumber = w.WayBillNumber;
                            packageModel.Weight = package.Weight.Value;
                            packageModel.AddWeight = package.AddWeight.Value;
                            packageModel.SettleWeight = package.SettleWeight.Value;
                            packageModel.Length = package.Length.Value;
                            packageModel.Width = package.Width.Value;
                            packageModel.Height = package.Height.Value;
                            packageModel.LengthFee = package.LengthFee.Value;
                            packageModel.WeightFee = package.WeightFee.Value;
                            extmodel.WaybillPackageDetailList.Add(packageModel);
                        }

                        inStorage.WayBillInfos.Add(extmodel);
                        inStorage.InStorage.TotalWeight += w.SettleWeight;
                        inStorage.InStorage.TotalQty ++;
                    }

                });
                try
                {
                    _inStorageService.CreateInStorageCS(inStorage);
                    responseResult.Result = true;
                    responseResult.Message = inStorage.InStorage.InStorageID;

                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    responseResult.Result = false;
                    responseResult.Message = ex.Message;
                }
            }
            return responseResult;
        }

        /// <summary>
        /// 打印入库交接单 add by huhaiyou  2014-5-7
        /// </summary>
        /// <param name="InStorageId"></param>
        /// <returns></returns>
        public List<InStorageInfoModel> GetPrintInStorageInvoice(string InStorageID)
        {
            try
            {
                List<InStorageInfoModel> modelList = new List<InStorageInfoModel>();

                PrintInStorageInvoiceParam outStorageListParam = new PrintInStorageInvoiceParam
                {
                    InStorageID = InStorageID
                };
                modelList = _inStorageService.GetPrintInStorageInvoice(outStorageListParam).ToModelAsCollection<PrintInStorageInvoiceExt, InStorageInfoModel>();
                return modelList;

            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                throw;
            }

        }

        [HttpPost]
        /// <summary>
        /// CS 版本打印入仓发票
        /// </summary>
        /// <param name="InStorageId"></param>
        /// <returns></returns>
        public InStorageInfoModelDetailViewModel InStorageDetailCS(string InStorageId)
        {
            var model = new InStorageInfoModelDetailViewModel();
            if (!string.IsNullOrWhiteSpace(InStorageId))
            {
                model.InStorageInfoModel = _inStorageService.GetInStorageInfo(InStorageId).ToModel<InStorageModel>();
                if (model.InStorageInfoModel != null &&
                    !string.IsNullOrWhiteSpace(model.InStorageInfoModel.CustomerCode))
                {
                    var customer = _customerService.GetCustomer(model.InStorageInfoModel.CustomerCode);
                    if (customer != null)
                    {
                        model.Customer.CustomerCode = customer.CustomerCode;
                        model.Customer.Balance = customer.CustomerBalance.Balance ?? 0;
                        model.Customer.Name = customer.Name;
                        model.Customer.PaymentTypeName = customer.PaymentType.PaymentName;
                    }
                    if (model.InStorageInfoModel.WayBillInfos.Count > 0)
                    {
                        model.InStorageInfoModel.ShippingMethodName = model.InStorageInfoModel.WayBillInfos.First().InShippingName;
                    }
                }
            }
            return model;
        }

        
        /// <summary>
        /// 获取客户列表
        /// </summary>
        /// <param name="keyWord">关键词</param>
        /// <returns></returns>
        [HttpGet]
        public List<CustomerModel> GetCustomerListCS(string keyWord="")
        {
            try
            {
                return _customerService.GetCustomerList(keyWord, true).ToModelAsCollection<Customer, CustomerModel>();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                throw ex;
            }

        }

        /// <summary>
        /// 清空广州小包地址信息使用次数
        /// </summary>
        [HttpGet]
        public void UpdateGZPacketAddressNumber()
        {
            _wayBillTemplateService.UpdateGZPacketAddressNumber();
        }
    }


}