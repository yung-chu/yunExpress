using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web.Http;
using System.Xml;
using FluentValidation;
using FluentValidation.Results;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Data.Express.DHL.Response;
using LMS.Data.Repository;
using LMS.Services.CountryServices;
using LMS.Services.CustomerOrderServices;
using LMS.Services.CustomerServices;
using LMS.Services.ExpressServices;
using LMS.Services.FreightServices;
using LMS.Services.OrderServices;
using LMS.Services.SF;
using LMS.Services.SF.Model;
using LMS.Services.SequenceNumber;
using LMS.Services.TrackingNumberServices;
using LMS.Services.TrackServices;
using LMS.WebAPI.Client.Helper;
using LMS.WebAPI.Client.Models;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Http;
using LMS.WebAPI.Client.Properties;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Web.Utities;
using Customer = LMS.Data.Entity.Customer;
using ShippingInfoModel = LMS.WebAPI.Client.Models.ShippingInfoModel;
using Status = LMS.WebAPI.Client.Helper.Status;

namespace LMS.WebAPI.Client.Controllers
{
    public class WayBillController : ApiControllerBase
    {
        List<OrderResponseResult> orderResponseResults = new List<OrderResponseResult>();

        List<TrackingNumberDetailInfo> trackingNumberDetailInfos = new List<TrackingNumberDetailInfo>();
        //List<int> detailIds = new List<int>();

        private readonly ICustomerOrderService _customerOrderService;
        private readonly ITrackingNumberService _trackingNumberService;
        private readonly IFreightService _freightService;
        private readonly IOrderService _orderService;
        private readonly ITrackingService _trackingService;
        private readonly IExpressService _expressService;
        private readonly ICountryService _countryService;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerBalanceRepository _customerBalanceRepository;
        private readonly ICustomerSourceInfoRepository _customerSourceInfoRepository;

        public WayBillController(ICustomerOrderService customerOrderService, ITrackingNumberService trackingNumberService, IFreightService freightService, IOrderService orderService, ITrackingService trackingService, IExpressService expressService, ICountryService countryService, ICustomerRepository customerRepository, ICustomerBalanceRepository customerBalanceRepository, ICustomerSourceInfoRepository customerSourceInfoRepository)
        {
            _customerOrderService = customerOrderService;
            _trackingNumberService = trackingNumberService;
            _freightService = freightService;
            _orderService = orderService;
            _trackingService = trackingService;
            _expressService = expressService;
            _countryService = countryService;
            _customerRepository = customerRepository;
            _customerBalanceRepository = customerBalanceRepository;
            _customerSourceInfoRepository = customerSourceInfoRepository;
        }

        /// <summary>
        /// 获取单个运单
        /// </summary>
        /// <param name="wayBillNumber">物流系统运单号，客户订单或跟踪号</param>
        /// <returns></returns>
        //api/WayBill/GetWayBill
        public Response<WayBillInfoModel> GetWayBill(string wayBillNumber)
        {
            var responseResult = new Response<WayBillInfoModel>()
            {
                ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error9999),
                ResultDesc = Resource.Error9999
            };
            try
            {
                var way = _customerOrderService.GetWayBill(wayBillNumber, CustomerCode);
                //WayBillModel model = way.ToModel<WayBillModel>();
                if (way != null)
                {
                    var model=new WayBillInfoModel()
                        {
                            WayBillNumber = way.WayBillNumber,
                            OrderNumber = way.CustomerOrderNumber,
                            TrackingNumber = way.TrackingNumber,
                            ApplicationType = way.CustomerOrderInfo.AppLicationType,
                            PackageNumber = way.CustomerOrderInfo.PackageNumber??1,
                            EnableTariffPrepay = way.EnableTariffPrepay,
                            Height = way.Height,
                            InsureAmount = way.CustomerOrderInfo.InsureAmount,
                            InsuranceType = way.CustomerOrderInfo.InsuredID??0,
                            IsReturn = way.IsReturn,
                            Length = way.Length,
                            SensitiveTypeID = way.CustomerOrderInfo.SensitiveTypeID,
                            SettleWeight = way.SettleWeight,
                            Status = way.CustomerOrderInfo.Status,
                            SenderInfo = way.SenderInfo.ToModel<SenderInfoModel>(),
                            ShippingInfo = way.ShippingInfo.ToModel<ShippingInfoModel>(),
                            Weight = way.Weight,
                            Width = way.Width
                        };
                    if (way.CustomerOrderInfo.ShippingMethodId.HasValue)
                    {
                        var shipping = _freightService.GetShippingMethod(way.CustomerOrderInfo.ShippingMethodId.Value);
                        model.ShippingMethodCode = shipping.Code;
                    }
                    foreach (var app in way.ApplicationInfos)
                    {
                        model.ApplicationInfos.Add(new ApplicationInfoModel()
                            {
                                ApplicationName = app.ApplicationName,
                                HSCode = app.HSCode,
                                PickingName = app.PickingName,
                                ProductUrl = app.ProductUrl,
                                Qty = app.Qty,
                                Remark = app.Remark,
                                UnitPrice = app.UnitPrice,
                                UnitWeight = app.UnitWeight
                            });
                    }
                    responseResult.Item = model;
                    responseResult.ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error0000);
                    responseResult.ResultDesc = Resource.Error0000;
                }
                else
                {
                    responseResult.ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error1006);
                    responseResult.ResultDesc = Resource.Error1006;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return responseResult;
        }

        /// <summary>
        /// 获取tis_db 订单跟踪信息
        /// yungchu 
        /// 2014-07-12
        /// </summary>
        /// <param name="number">物流系统运单号</param>
        /// <returns></returns>
        //api/WayBill/GetOrder
        public Response<OrderModel> GetOrder(string number)
        {
            return ReturnResponseResult(number, 1);
        }

        /// <summary>
        /// 获取tis_db 订单跟踪信息
        /// yungchu 
        /// 2014-10-20
        /// </summary>
        /// <param name="number">物流系统运单号,跟踪号，客户订单号</param>
        /// <returns></returns>
        //api/WayBill/GetTrackingNumber
        public Response<OrderModel> GetTrackingNumber(string trackingNumber)
        {
            return ReturnResponseResult(trackingNumber, 2);
        }

        public Response<OrderModel> ReturnResponseResult(string number, int flag)
        {
            var responseResult = new Response<OrderModel>()
            {
                ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error9999),
                ResultDesc = Resource.Error9999
            };

            try
            {
                OrderModel getOrderModel = new OrderModel();
                OrderModel orderTrackingDetails = new OrderModel();
                List<InTrackingLogInfo> getInTackingLogInfo = new List<InTrackingLogInfo>();
                OrderTrackingModel orderTrackingModel = new OrderTrackingModel();
                WayBillInfo wayBillInfo = new WayBillInfo();

                //获取运单信息
                if (flag == 1)//运单号查询
                {
                    wayBillInfo = _trackingService.GetWayBillInfo(number);
                }
                else//运单号，客户订单号，跟踪号查询
                {
                    List<string> wayBillnumbers = number.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                    wayBillInfo = _trackingService.GetWayBillInfoList(wayBillnumbers) != null ? _trackingService.GetWayBillInfoList(wayBillnumbers)[0] : null;
                }

                if (wayBillInfo == null)
                {
                    responseResult.ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error1006);
                    responseResult.ResultDesc = Resource.Error1006;
                    return responseResult;
                }


                //获取内部数据列表
                getInTackingLogInfo = _trackingService.GetInTrackingLogInfos(wayBillInfo.WayBillNumber).OrderByDescending(a => a.ProcessDate).ToList();
                List<InTrackingLogInfo> getInTackingLogInfoList = _trackingService.GetInTrackingLogInfos(wayBillInfo.WayBillNumber).ToList();

                //调用api 获取ups,dhl抓取信息
                orderTrackingModel = _freightService.GetOutTrackingInfo(wayBillInfo.TrueTrackingNumber) ?? _freightService.GetOutTrackingInfo(wayBillInfo.TrackingNumber);


                //去除外部明细信息最后一条
                if (orderTrackingModel != null && orderTrackingModel.OrderTrackingDetails.Any())
                {
                    orderTrackingModel.OrderTrackingDetails.RemoveAt(orderTrackingModel.OrderTrackingDetails.Count - 1);
                }


                //都有数据
                if (orderTrackingModel != null && getInTackingLogInfo.Count != 0)
                {

                    //返回跟踪明细
                    if (orderTrackingModel.OrderTrackingDetails != null && orderTrackingModel.OrderTrackingDetails.Count != 0)
                    {
                        orderTrackingModel.OrderTrackingDetails.ForEach(
                            p => orderTrackingDetails.OrderTrackingDetails.Add(
                                new OrderTrackingDetailModels
                                {
                                    ProcessDate = p.ProcessDate,
                                    ProcessContent = p.ProcessContent,
                                    ProcessLocation = p.ProcessLocation
                                })
                            );
                    }

                    //返回内部信息
                    getInTackingLogInfo.ForEach(
                                p => orderTrackingDetails.OrderTrackingDetails.Add(
                                    new OrderTrackingDetailModels
                                    {
                                        ProcessDate = p.ProcessDate,
                                        ProcessContent = p.ProcessContent,
                                        ProcessLocation = p.ProcessLocation
                                    })
                      );


                    //外部信息包裹状态
                    int? getPackageState = orderTrackingModel.PackageState.HasValue ? orderTrackingModel.PackageState : 0;
                    //签收天数
                    int? getSpanDay = 0;
                    if (orderTrackingModel.OrderTrackingDetails.Any())
                    {

                        //获取内部数据收货时间(第二条)
                        DateTime getSecondInnerDate = getInTackingLogInfoList.Count >= 2 ? getInTackingLogInfoList.Skip(1).Take(1).ToList()[0].ProcessDate.Value : getInTackingLogInfoList[0].ProcessDate.Value;

                        TimeSpan ts = orderTrackingModel.OrderTrackingDetails.First().ProcessDate.Value - getSecondInnerDate;
                        getSpanDay = Convert.ToInt32(Math.Ceiling(Math.Abs(ts.TotalDays)));
                    }



                    //返回数据  3--已签收  0--未知
                    getOrderModel = new OrderModel()
                    {

                        WayBillNumber = wayBillInfo.WayBillNumber,
                        TrackingNumber = wayBillInfo.TrueTrackingNumber,
                        CountryCode = wayBillInfo.CountryCode,
                        CreatedBy = wayBillInfo.CreatedBy,
                        PackageState = getPackageState,
                        //签收天数(加上内部天数)
                        IntervalDays = getPackageState == 3 && getSpanDay != 0 ? getSpanDay : null,
                        //跟踪明细
                        OrderTrackingDetails = orderTrackingDetails.OrderTrackingDetails
                    };
                    responseResult.Item = getOrderModel;
                    responseResult.ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error0000);
                    responseResult.ResultDesc = Resource.Error0000;

                }//只取内部信息
                else if (orderTrackingModel == null && getInTackingLogInfo.Count != 0)
                {

                    getInTackingLogInfo.ForEach(
                    p => orderTrackingDetails.OrderTrackingDetails.Add(
                        new OrderTrackingDetailModels
                        {
                            ProcessDate = p.ProcessDate,
                            ProcessContent = p.ProcessContent,
                            ProcessLocation = p.ProcessLocation
                        })
                    );


                    //已提交
                    if (wayBillInfo.Status == 3)
                    {
                        wayBillInfo.Status = 11;
                    }
                    //已签收
                    if (wayBillInfo.Status == 10)
                    {
                        wayBillInfo.Status = 3;
                    }

                    //返回数据
                    getOrderModel = new OrderModel()
                    {

                        WayBillNumber = wayBillInfo.WayBillNumber,
                        TrackingNumber = wayBillInfo.TrueTrackingNumber,
                        CountryCode = wayBillInfo.CountryCode,
                        CreatedBy = wayBillInfo.CreatedBy,
                        PackageState = wayBillInfo.Status,
                        //签收天数 没有外部信息查不到签收天数
                        IntervalDays = null,
                        //跟踪明细
                        OrderTrackingDetails = orderTrackingDetails.OrderTrackingDetails
                    };
                    responseResult.Item = getOrderModel;
                    responseResult.ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error0000);
                    responseResult.ResultDesc = Resource.Error0000;

                }//都没有数据
                else if (orderTrackingModel == null && getInTackingLogInfo.Count == 0)
                {
                    responseResult.ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error1006);
                    responseResult.ResultDesc = Resource.Error1006;
                }

            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            return responseResult;
        }





        /// <summary>
        /// 批量获取运单
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        //api/WayBill/WayBillList
        //[HttpPost]
        //public Response<List<WayBillModel>> WayBillList(string[] wayBillNumber)
        //{

        //    var responseResult = new Response<List<WayBillModel>>()
        //    {
        //        ResultCode = GetErrorCode(ErrorCode.Error9999),
        //        ResultDesc = Resource.Error9999
        //    };
        //    if (null == wayBillNumber)
        //    {
        //        responseResult.ResultCode = GetErrorCode(ErrorCode.Error1004);
        //        responseResult.ResultCode = Resource.Error1004;
        //    }
        //    else
        //    {
        //        if (!wayBillNumber.Any())
        //        {
        //            responseResult.ResultCode = GetErrorCode(ErrorCode.Error1004);
        //            responseResult.ResultCode = Resource.Error1004;
        //        }
        //    }
        //    try
        //    {
        //        var wayList = _customerOrderService.GetWayBillList(wayBillNumber, CustomerCode).ToList();
        //        var list = wayList.ToModelAsCollection<WayBillInfo, WayBillModel>();
        //        if (list != null && list.Count > 0)
        //        {
        //            responseResult.Item = list;
        //            responseResult.ResultCode = GetErrorCode(ErrorCode.Error0000);
        //            responseResult.ResultDesc = Resource.Error0000;
        //        }
        //        else
        //        {
        //            responseResult.ResultCode = GetErrorCode(ErrorCode.Error1006);
        //            responseResult.ResultDesc = Resource.Error1006;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Exception(ex);
        //    }
        //    return responseResult;
        //}

        /// <summary>
        /// 批量申请运单锁 zxq
        /// </summary>
        private static readonly object LockBatchAdd = new object();

        /// <summary>
        /// 批量申请运单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="wayBillModels"></param>
        /// <returns></returns>
        //api/WayBill/BatchAdd
        [HttpPost]
        public Response<List<OrderResponseResult>> BatchAdd(List<WayBillModel> wayBillModels)
        {
            lock (LockBatchAdd)
            {
                var responseResult = new Response<List<OrderResponseResult>>()
                    {
                        ResultCode = GetErrorCode(ErrorCode.Error9999),
                        ResultDesc = Resource.Error9999
                    };
                List<WayBillModel> list = new List<WayBillModel>();
                if (wayBillModels == null)
                {
                    responseResult.ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error1004);
                    responseResult.ResultDesc = Resource.Error1004;
                    return responseResult;
                }
                if (!wayBillModels.Any())
                {
                    responseResult.ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error1004);
                    responseResult.ResultDesc = Resource.Error1004;
                    return responseResult;
                }
                list = ValidationOrderModel(wayBillModels);
                var wayBillList = new List<WayBillInfo>();
                var nlList = new Dictionary<string, WayBillInfo>();
                var nlparcellist = new List<NetherlandsParcelRespons>();
                var rulist = new Dictionary<string, WayBillInfo>();
                var ruparcellist = new List<SfOrderResponse>();
                var shippingMethods = _freightService.GetShippingMethods("", true);
                list.Where(p => p.IsValid).Each(p =>
                    {
                        WayBillInfo wayBill = new WayBillInfo();
                        wayBill.WayBillNumber = p.WayBillNumber;
                        wayBill.CustomerOrderNumber = p.OrderNumber.GetSafeHtml();
                        wayBill.CustomerCode = CustomerCode;
                        wayBill.IsReturn = p.IsReturn;
                        wayBill.CountryCode = p.ShippingInfo.CountryCode.GetSafeHtml();
                        wayBill.InShippingMethodID = p.InShippingMethodId;
                        wayBill.InShippingMethodName = p.InShippingMethodName;
                        wayBill.Height = p.Height;
                        wayBill.Length = p.Length;
                        wayBill.Width = p.Width;
                        wayBill.Weight = p.Weight;
                        wayBill.CreatedBy = CustomerCode;
                        wayBill.CreatedOn = DateTime.Now;
                        wayBill.LastUpdatedBy = CustomerCode;
                        wayBill.LastUpdatedOn = DateTime.Now;
                        wayBill.InsuredID = p.InsuranceType == 0 ? (int?)null : p.InsuranceType;
                        wayBill.TrackingNumber = p.TrackingNumber.GetSafeHtml();
                        wayBill.GoodsTypeID = 1; //默认是包裹
                        wayBill.Status = WayBill.StatusToValue(WayBill.StatusEnum.Submitted);

                        var shippingInfo = new ShippingInfo
                            {
                                ShippingTaxId = p.ShippingInfo.ShippingTaxId.GetSafeHtml(),
                                CountryCode = p.ShippingInfo.CountryCode.GetSafeHtml(),
                                ShippingCity = p.ShippingInfo.ShippingCity.GetSafeHtml(),
                                ShippingCompany = p.ShippingInfo.ShippingCompany.GetSafeHtml(),
                                ShippingAddress = p.ShippingInfo.ShippingAddress.GetSafeHtml(),
                                ShippingAddress1 = p.ShippingInfo.ShippingAddress1.GetSafeHtml(),
                                ShippingAddress2 = p.ShippingInfo.ShippingAddress2.GetSafeHtml(),
                                ShippingFirstName = p.ShippingInfo.ShippingFirstName.GetSafeHtml(),
                                ShippingLastName = p.ShippingInfo.ShippingLastName.GetSafeHtml(),
                                ShippingPhone = p.ShippingInfo.ShippingPhone.GetSafeHtml(),
                                ShippingState = p.ShippingInfo.ShippingState.GetSafeHtml(),
                                ShippingZip = p.ShippingInfo.ShippingZip.GetSafeHtml()
                            };
                        wayBill.ShippingInfo = shippingInfo;
                        var senderInfo = new SenderInfo()
                            {
                                CountryCode = "CN"
                            };
                        if (p.SenderInfo != null)
                        {
                            senderInfo=new SenderInfo()
                                {
                                    CountryCode =
                                        string.IsNullOrWhiteSpace(p.SenderInfo.CountryCode)
                                            ? "CN"
                                            : p.SenderInfo.CountryCode.GetSafeHtml(),
                                    SenderCity = p.SenderInfo.SenderCity.GetSafeHtml(),
                                    SenderAddress = p.SenderInfo.SenderAddress.GetSafeHtml(),
                                    SenderFirstName = p.SenderInfo.SenderFirstName.GetSafeHtml(),
                                    SenderLastName = p.SenderInfo.SenderLastName.GetSafeHtml(),
                                    SenderPhone = p.SenderInfo.SenderPhone.GetSafeHtml(),
                                    SenderState = p.SenderInfo.SenderState.GetSafeHtml(),
                                    SenderZip = p.SenderInfo.SenderZip.GetSafeHtml(),
                                    SenderCompany = p.SenderInfo.SenderCompany.GetSafeHtml()
                                };
                        }
                        wayBill.SenderInfo = senderInfo;
                        var customerOrderInfo = new CustomerOrderInfo();
                        customerOrderInfo.LastUpdatedBy = customerOrderInfo.CreatedBy = CustomerCode;
                        customerOrderInfo.CustomerCode = CustomerCode;
                        customerOrderInfo.LastUpdatedOn = customerOrderInfo.CreatedOn = DateTime.Now;
                        customerOrderInfo.CustomerOrderNumber = p.OrderNumber.GetSafeHtml();
                        customerOrderInfo.Weight = p.Weight ?? 0;
                        customerOrderInfo.Length = p.Length ?? 1;
                        customerOrderInfo.Height = p.Height ?? 1;
                        customerOrderInfo.Width = p.Width ?? 1;
                        customerOrderInfo.ShippingMethodId = p.InShippingMethodId;
                        customerOrderInfo.ShippingMethodName = p.InShippingMethodName;
                        customerOrderInfo.SensitiveTypeID = p.SensitiveTypeID.HasValue && p.SensitiveTypeID.Value == 0
                                                                ? null
                                                                : p.SensitiveTypeID;

                        customerOrderInfo.ShippingInfo = shippingInfo;
                        customerOrderInfo.SenderInfo = senderInfo;
                        customerOrderInfo.InsuredID = p.InsuranceType == 0 ? (int?)null : p.InsuranceType;
                        customerOrderInfo.IsInsured = p.InsuranceType != 0;
                        customerOrderInfo.TrackingNumber = p.TrackingNumber.GetSafeHtml();
                        customerOrderInfo.AppLicationType = p.ApplicationType;
                        customerOrderInfo.PackageNumber = p.PackageNumber;
                        customerOrderInfo.InsureAmount = p.InsureAmount;
                        customerOrderInfo.IsReturn = p.IsReturn;
                        customerOrderInfo.GoodsTypeID = 1; //默认是包裹
                        customerOrderInfo.Status = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Submitted);
                        customerOrderInfo.CustomerOrderStatuses.Add(new CustomerOrderStatus()
                            {
                                CreatedOn = DateTime.Now,
                                Remark = "物流API批量导入",
                                Status = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Submitted)
                            });
                        wayBill.CustomerOrderInfo = customerOrderInfo;


                        p.ApplicationInfos.Each(d =>
                            {
                                var applicationInfo = new ApplicationInfo
                                    {
                                        CustomerOrderInfo = customerOrderInfo,
                                        ApplicationName = d.ApplicationName.GetSafeHtml(),
                                        Qty = d.Qty,
                                        HSCode = d.HSCode.GetSafeHtml(),
                                        UnitPrice = d.UnitPrice,
                                        UnitWeight = d.UnitWeight,
                                        Total = d.Qty * d.UnitPrice,
                                        PickingName = d.PickingName.GetSafeHtml(),
                                        Remark = d.Remark.GetSafeHtml(),
                                        CreatedBy = CustomerCode,
                                        CreatedOn = DateTime.Now,
                                        LastUpdatedBy = CustomerCode,
                                        LastUpdatedOn = DateTime.Now,
                                        ProductUrl = d.ProductUrl,
                                    };
                                wayBill.ApplicationInfos.Add(applicationInfo);
                            });
                        //顺丰荷兰小包
                        if (sysConfig.NLPOSTShippingMethodID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Contains(wayBill.InShippingMethodID.ToString()))
                        {
                            nlList.Add(wayBill.WayBillNumber, wayBill);
                        }
                        else if (
                            sysConfig.LithuaniaShippingMethodID.Split(new string[] {","},
                                                                   StringSplitOptions.RemoveEmptyEntries)
                                     .Contains(wayBill.InShippingMethodID.ToString()))
                        {
                            //俄罗斯挂号、平邮
                            rulist.Add(wayBill.WayBillNumber, wayBill);
                        }
                        wayBillList.Add(wayBill);
                    });
                if (wayBillList.Count <= 0)
                {
                    responseResult.ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error1001);
                    responseResult.ResultDesc = Resource.Error1001;
                    responseResult.Item = orderResponseResults;
                    return responseResult;
                }
                bool result = false;
                var message = string.Empty;
                try
                {
                    if (nlList.Any())
                    {
                        foreach (var nldiction in nlList)
                        {
                            var nlerrmsg = string.Empty;
                            var nlparcel = NLPOST(nldiction.Value,ref nlerrmsg);
                            var w = wayBillList.SingleOrDefault(p => p.WayBillNumber == nldiction.Key);
                            wayBillList.Remove(p => p.WayBillNumber == nldiction.Key);
                            var l = list.SingleOrDefault(p => p.WayBillNumber == nldiction.Key);
                            list.Remove(p => p.WayBillNumber == nldiction.Key);
                            if (nlparcel != null)
                            {
                                nlparcellist.Add(nlparcel);
                                w.OutShippingMethodID = Int32.Parse(sysConfig.NLPOSTShippingMethodID.Split(new string[]{","},StringSplitOptions.RemoveEmptyEntries)[0]);
                                w.VenderCode = sysConfig.NLPOSTVenderCode;
                                w.TrackingNumber = nlparcel.AgentMailNo;
                                w.CustomerOrderInfo.TrackingNumber = nlparcel.AgentMailNo;
                                wayBillList.Add(w);
                                l.TrackingNumber = nlparcel.AgentMailNo;
                                l.ErrorMessage.Clear();
                                l.ErrorMessage.Append(nlparcel.MailNo);
                            }
                            else
                            {
                                l.IsValid = false;
                                l.ErrorMessage.AppendLine(nlerrmsg);
                                orderResponseResults.Add(new OrderResponseResult()
                                {
                                    CustomerOrderId = l.OrderNumber,
                                    Status = (int)Status.Fail, //失败
                                    Feedback = nlerrmsg
                                });
                            }
                            list.Add(l);
                        }
                    }
                    if (rulist.Any())
                    {
                        //9-挂号，10-平邮
                        foreach (var rudiction in rulist)
                        {
                            var ruerrmsg = string.Empty;
                            var rumodel = new OrderSfModel()
                                {
                                    OrderId = rudiction.Value.WayBillNumber,
                                    ExpressType =
                                        sysConfig.LithuaniaShippingMethodID.Split(new string[] {","},
                                                                                  StringSplitOptions.RemoveEmptyEntries)
                                            [0] == rudiction.Value.InShippingMethodID.ToString()
                                            ? 10
                                            : 9,
                                    ShippingName=rudiction.Value.ShippingInfo.ShippingFirstName+" "+rudiction.Value.ShippingInfo.ShippingLastName,
                                    ShippingCompany=rudiction.Value.ShippingInfo.ShippingCompany,
                                    ShippingTel=rudiction.Value.ShippingInfo.ShippingPhone,
                                    ShippingPhone = rudiction.Value.ShippingInfo.ShippingPhone,
                                    ShippingAddress = string.Format("{0} {1} {2}", (rudiction.Value.ShippingInfo.ShippingAddress ?? "").Trim(), (rudiction.Value.ShippingInfo.ShippingAddress1 ?? "").Trim(), (rudiction.Value.ShippingInfo.ShippingAddress2 ?? "").Trim()),
                                    ParcelQuantity=1,
                                    ShippingState=rudiction.Value.ShippingInfo.ShippingState,
                                    ShippingCity=rudiction.Value.ShippingInfo.ShippingCity,
                                    CountryCode=rudiction.Value.ShippingInfo.CountryCode,
                                    ShippingZip=rudiction.Value.ShippingInfo.ShippingZip,
                                    ApplicationTotalPrice=rudiction.Value.ApplicationInfos.Sum(p=>(p.Qty??1)*(p.UnitPrice??0)),
                                    ApplicationTotalWeight = rudiction.Value.ApplicationInfos.Sum(p => (p.Qty ?? 1) * (p.UnitWeight ?? 0))
                                };
                            rudiction.Value.ApplicationInfos.Each(p => rumodel.Applications.Add(new ApplicationSfModel()
                                {
                                    ApplicationName = p.ApplicationName,
                                    UnitPrice = p.UnitPrice??0,
                                    UnitWeight = p.UnitWeight??0,
                                    Qty = p.Qty??1
                                }));
                            var ruresponse = LMSSFCommon.SubmitSf(rumodel, sysConfig.LithuaniaAuthorization);
                            var w = wayBillList.SingleOrDefault(p => p.WayBillNumber == rudiction.Key);
                            wayBillList.Remove(p => p.WayBillNumber == rudiction.Key);
                            var l = list.SingleOrDefault(p => p.WayBillNumber == rudiction.Key);
                            list.Remove(p => p.WayBillNumber == rudiction.Key);
                            if (ruresponse.ErrorMsg.IsNullOrWhiteSpace())
                            {
                                ruparcellist.Add(ruresponse.Model);
                                w.OutShippingMethodID = w.InShippingMethodID;
                                w.VenderCode = sysConfig.LithuaniaVenderCode;
                                w.TrackingNumber = ruresponse.Model.MailNo;
                                w.CustomerOrderInfo.TrackingNumber =ruresponse.Model.MailNo;
                                wayBillList.Add(w);
                                l.TrackingNumber = ruresponse.Model.MailNo;
                            }
                            else
                            {
                                l.IsValid = false;
                                l.ErrorMessage.AppendLine(ruresponse.ErrorMsg);
                                orderResponseResults.Add(new OrderResponseResult()
                                {
                                    CustomerOrderId = l.OrderNumber,
                                    Status = (int)Status.Fail, //失败
                                    Feedback = ruresponse.ErrorMsg
                                });
                            }
                            list.Add(l);
                        }
                    }
                    if (wayBillList.Any())
                    {
                        try
                        {
                            //提交 DHL新加坡 EUB预报 
                            List<int> DHLshippingMethodIds = new List<int>();
                            List<int> EUBshippingMethodIds = new List<int>();
                            List<WayBillInfo> DHLwayBillNumbers = new List<WayBillInfo>();
                            List<WayBillInfo> EUBwayBillNumbers = new List<WayBillInfo>();
                            var dhlshippingMethodCod = sysConfig.DHLSingaporeCode.Split(',').ToList();
                            var eubshippingMethodCod = sysConfig.EUBSingaporeCode.Split(',').ToList();
                            shippingMethods.ForEach(p =>
                                {
                                    if (dhlshippingMethodCod.Contains(p.Code))
                                    {
                                        DHLshippingMethodIds.Add(p.ShippingMethodId);
                                    }
                                    if (eubshippingMethodCod.Contains(p.Code))
                                    {
                                        EUBshippingMethodIds.Add(p.ShippingMethodId);
                                    }
                                });
                            wayBillList.ForEach(p =>
                                {

                                    p.OutShippingMethodID = p.InShippingMethodID;
                                    p.OutShippingMethodName = p.InShippingMethodName;
                                    if (DHLshippingMethodIds.Contains(p.InShippingMethodID.Value))
                                    {
                                        DHLwayBillNumbers.Add(p);
                                    }
                                    if ( EUBshippingMethodIds.Contains(p.InShippingMethodID.Value) && string.IsNullOrWhiteSpace(p.TrackingNumber))
                                    {
                                        EUBwayBillNumbers.Add(p);
                                    }
                                });
                            if (DHLwayBillNumbers.Count > 0 || EUBwayBillNumbers.Count> 0)
                            {
                                //事物控制一起执行
                                using (var transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.MaxValue))
                                {
                                    ResultDHLShipment postDHLShipmenResult = new ResultDHLShipment();
                                    if (DHLwayBillNumbers.Count > 0)
                                    {
                                        //DHL 预报 申请跟踪号
                                        postDHLShipmenResult = PostDHLShipment(DHLwayBillNumbers);
                                        //移除找不到预报的运单
                                        postDHLShipmenResult.ResultDhls.ForEach(p =>
                                            {
                                                var waybill = wayBillList.Find(z => z.WayBillNumber == p.WayBillNumber);
                                                var lis = list.Find(z => z.WayBillNumber == p.WayBillNumber);
                                                if (waybill != null)
                                                {
                                                    if (!p.IsSuccess)
                                                    {
                                                        lis.IsValid = false;
                                                        orderResponseResults.Add(new OrderResponseResult()
                                                            {
                                                                CustomerOrderId =
                                                                    waybill.CustomerOrderInfo.CustomerOrderNumber,
                                                                Status = (int) Status.Fail, //失败
                                                                Feedback = p.AirwayBillNumber
                                                            });
                                                        //排除掉不能正常获取DHL预报的订单
                                                        wayBillList.RemoveAll(z => z.WayBillNumber == p.WayBillNumber);
                                                        //排除DHL运单中申请预报失败的DHL运单
                                                        DHLwayBillNumbers.RemoveAll(z => z.WayBillNumber == p.WayBillNumber);
                                                    }
                                                    else
                                                    {
                                                        waybill.TrackingNumber = p.AirwayBillNumber;
                                                        waybill.VenderCode = sysConfig.DHLVenderCode.Trim();
                                                        waybill.CustomerOrderInfo.TrackingNumber = p.AirwayBillNumber;
                                                        lis.TrackingNumber = p.AirwayBillNumber;
                                                        //给预报成功的DHL运单加上服务商
                                                        DHLwayBillNumbers.Find(z => z.WayBillNumber == p.WayBillNumber).VenderCode = sysConfig.DHLVenderCode.Trim();
                                                        
                                                    }
                                                }
                                            });
                                    }

                                    //EUB预报
                                    if (EUBwayBillNumbers.Count > 0)
                                    {
                                        EubWayBillParam param = new EubWayBillParam()
                                            {
                                                WayBillInfos = EUBwayBillNumbers,
                                                PrintFormat = 2,
                                                PrintFormatValue = "01"

                                            };
                                        var resultEUBs = _customerOrderService.ApplyApiEubWayBillInfo(param);
                                        //排除请求预报失败的运单
                                        resultEUBs.ForEach(p =>
                                            {
                                                var waybill = wayBillList.Find(z => z.WayBillNumber == p.WayBillNumber);
                                                var lis = list.Find(z => z.WayBillNumber == p.WayBillNumber);
                                                if (waybill != null)
                                                {
                                                    if (!p.IsSuccess)
                                                    {
                                                        lis.IsValid = false;
                                                        orderResponseResults.Add(new OrderResponseResult()
                                                        {
                                                            CustomerOrderId =
                                                                waybill.CustomerOrderInfo.CustomerOrderNumber,
                                                            Status = (int)Status.Fail, //失败
                                                            Feedback = p.AirwayBillNumber
                                                        });
                                                        //排除掉不能正常获取EUB预报的订单
                                                        wayBillList.RemoveAll(z => z.WayBillNumber == p.WayBillNumber);
                                                        //排除EUB运单中申请预报失败的EUB运单
                                                        EUBwayBillNumbers.RemoveAll(z => z.WayBillNumber == p.WayBillNumber);
                                                    }
                                                    else
                                                    {
                                                        waybill.TrackingNumber = p.AirwayBillNumber;
                                                        waybill.CustomerOrderInfo.TrackingNumber = p.AirwayBillNumber;
                                                        lis.TrackingNumber = p.AirwayBillNumber;
                                                    }
                                                }
                                            });

                                    }
                                    if (wayBillList.Count > 0)
                                    {

                                        result = _orderService.BatchCreateWayBillInfo(wayBillList);
                                    }
                                    else
                                    {
                                        result = false;
                                    }
                                    if (result)
                                    {
                                        //DHL添加预报日志信息
                                        if (postDHLShipmenResult.ExpressResponses !=null && postDHLShipmenResult.ExpressResponses.Count > 0)
                                        {
                                            _expressService.AddExpressResponseToDHL(postDHLShipmenResult.ExpressResponses);
                                            _expressService.AddApiDHLDeliveryChannelChangeLog(DHLwayBillNumbers);
                                        }
                                        //EUB添加预报日志信息
                                        if (EUBwayBillNumbers.Count>0)
                                        {
                                            _customerOrderService.AddEubWayBillApplicationInfo(EUBwayBillNumbers);
                                        }
                                    }
                                    transaction.Complete();
                                }
                            }
                            else
                            {
                                result = _orderService.BatchCreateWayBillInfo(wayBillList);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Exception(ex);
                            //如果事物里提交失败。所有运单都标示不成功
                            result = false;
                        }
                    }
                    
                    if (nlparcellist.Any())
                    {
                        var nlparceladdlist = new List<NetherlandsParcelRespons>();
                        if (result)
                        {
                            foreach (var netherlandsParcelResponse in nlparcellist)
                            {
                                NLPOSTConfirm(netherlandsParcelResponse.WayBillNumber, netherlandsParcelResponse.MailNo);
                                netherlandsParcelResponse.Status = 2;
                                netherlandsParcelResponse.CreatedOn =
                                    netherlandsParcelResponse.LastUpdatedOn = DateTime.Now;
                                netherlandsParcelResponse.LastUpdatedBy =
                                    netherlandsParcelResponse.CreatedBy = CustomerCode;
                                nlparceladdlist.Add(netherlandsParcelResponse);
                            }
                        }
                        else
                        {
                            foreach (var netherlandsParcelResponse in nlparcellist)
                            {
                                NLPOSTConfirm(netherlandsParcelResponse.WayBillNumber, netherlandsParcelResponse.MailNo, 2);
                            }
                        }
                    if (nlparceladdlist.Any())
                    {
                        _orderService.BulkInsert("NetherlandsParcelResponses", nlparceladdlist);
                    }
                    }
                    if (ruparcellist.Any())
                    {
                        var ruparceladdlist = new List<LithuaniaInfo>();
                        if (result)
                        {
                            foreach (var rursp in ruparcellist)
                            {
                                //LMSSFCommon.SfConfirm(rursp.OrderId, rursp.MailNo, sysConfig.LithuaniaAuthorization);
                                ruparceladdlist.Add(new LithuaniaInfo()
                                    {
                                        Status = 2,
                                        AgentMailNo = rursp.AgentMailNo,
                                        CreatedBy = CustomerCode,
                                        CreatedOn = DateTime.Now,
                                        DestCode = rursp.DestCode,
                                        FilterResult = rursp.FilterResult,
                                        LastUpdatedBy = CustomerCode,
                                        LastUpdatedOn = DateTime.Now,
                                        MailNo = rursp.MailNo,
                                        OriginCode = rursp.OriginCode,
                                        WayBillNumber = rursp.OrderId,
                                        Remark = rursp.Remark,
                                        TrackNumber = rursp.TrackNumber
                                    });
                            }
                        }
                        else
                        {
                            //foreach (var rursp in ruparcellist)
                            //{
                            //    LMSSFCommon.SfConfirm(rursp.OrderId, rursp.MailNo, sysConfig.LithuaniaAuthorization,2);
                            //}
                        }
                        if (ruparceladdlist.Any())
                        {
                            _orderService.BulkInsert("LithuaniaInfos", ruparceladdlist);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    message = ex.Message;
                }

                //批量提交失败 
                if (!result)
                {
                    list.Where(p => p.IsValid).Each(p =>
                        {
                            p.IsValid = false;
                            p.ErrorMessage.Append(Resource.Error1001);
                            //添加到响应信息
                            orderResponseResults.Add(new OrderResponseResult()
                                {
                                    CustomerOrderId = p.OrderNumber,
                                    Status = (int)Status.Fail, //失败
                                    Feedback = Resource.Error1001
                                });
                        });
                    responseResult.ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error1001);
                    responseResult.ResultDesc = string.IsNullOrWhiteSpace(message) ? Resource.Error1001 : message;

                }
                else
                {
                    string str = null;
                    //提交成功
                    list.Where(p => p.IsValid).Each(p =>
                        {
                            var shippingMethod = shippingMethods.FirstOrDefault(z => z.Code == p.ShippingMethodCode);
                            if (shippingMethod != null && shippingMethod.FuelRelateRAF)
                            {
                                bool res = _freightService.IsRemoteArea(p.InShippingMethodId,
                                                                        p.ShippingInfo.CountryCode.GetSafeHtml(),
                                                                        p.ShippingInfo.ShippingCity,
                                                                        p.ShippingInfo.ShippingZip);
                                if (res)
                                {
                                    str += "该单地址为偏远地址,会产生偏远附加费";
                                }
                            }
                            //运输方式带跟踪号
                            if (CommonMethodHelper.ShippingMethodHaveTrackingNum(p.ShippingMethodCode))
                            {
                                if (string.IsNullOrWhiteSpace(p.TrackingNumber))
                                {
                                    //添加到响应信息
                                    orderResponseResults.Add(new OrderResponseResult()
                                        {
                                            CustomerOrderId = p.OrderNumber,
                                            OrderId = p.WayBillNumber,
                                            Status = (int)Status.Success, //成功
                                            TrackStatus = ((int)TrackStatus.WaitOrder).ToString(),
                                            Feedback = Resource.Error0000 + str
                                        });
                                }
                                else
                                {
                                    //添加到响应信息
                                    var s = new OrderResponseResult();
                                        s.CustomerOrderId = p.OrderNumber;
                                        s.OrderId = p.TrackingNumber;
                                        s.Status = (int)Status.Success; //成功
                                        s.TrackStatus = ((int)TrackStatus.Send).ToString();
                                        s.Feedback = Resource.Error0000 + str;
                                    var customerCodes = sysConfig.IsTurnSFNumber.Split(',').ToList();
                                    if (customerCodes.Contains(CustomerCode))
                                    {
                                        s.AgentNumber = p.ErrorMessage.ToString();
                                        p.ErrorMessage.Clear();
                                    }
                                    orderResponseResults.Add(s);
                                }

                            }
                            else //不带跟踪号
                            {
                                //添加到响应信息
                                    orderResponseResults.Add(new OrderResponseResult()
                                    {
                                        CustomerOrderId = p.OrderNumber,
                                        OrderId = p.WayBillNumber,
                                        Status = (int)Status.Success, //成功
                                        TrackStatus = ((int)TrackStatus.None).ToString(),
                                        Feedback = Resource.Error0000 + str
                                    });
                            }
                            p.ErrorMessage.Append(Resource.Error0000);
                        });
                    if (list != null && list.Exists(p => p.IsValid == false))
                    {
                        responseResult.ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error1011);
                        responseResult.ResultDesc = Resource.Error1011;

                    }
                    else
                    {
                        responseResult.ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error0000);
                        responseResult.ResultDesc = Resource.Error0000;
                    }
                }
                responseResult.Item = orderResponseResults;
                return responseResult;
            }
        }

        //DHL 新加坡预报
        //Add By zhengsong
        //Time:2014-12-25
        public ResultDHLShipment PostDHLShipment(List<WayBillInfo> wayBillList)
        {
            ResultDHLShipment Result=new ResultDHLShipment();
            //Dictionary<string, string> Result = new Dictionary<string, string>();
            var applicationName = string.Empty;
            var list = wayBillList;
            foreach (var wayBillInfo in list)
            {
                ResultDHL rsl = new ResultDHL();
                rsl.IsSuccess = false;
                var expressAccount = _expressService.GetExpressAccountInfo(sysConfig.DHLVenderCode.Trim(), wayBillInfo.InShippingMethodID.Value);
                if (expressAccount == null)
                {
                    rsl.IsSuccess=false;
                    rsl.WayBillNumber=wayBillInfo.WayBillNumber;
                    rsl.AirwayBillNumber="DHL帐号不存在，请求DHL接口失败";
                    Result.ResultDhls.Add(rsl);
                    continue;
                }
                applicationName = wayBillInfo.ApplicationInfos.First().ApplicationName.Cutstring(70);
                string[] shippingAddress = wayBillInfo.ShippingInfo.ShippingAddress.StringSplitLengthWords(35).ToArray();
                wayBillInfo.VenderCode = sysConfig.DHLVenderCode.Trim();
                //wayBillInfo.OutShippingMethodID = 2;
                if (null != wayBillInfo.ExpressRespons)
                {
                    rsl.IsSuccess = false;
                    rsl.WayBillNumber = wayBillInfo.WayBillNumber;
                    rsl.AirwayBillNumber = "已经调用DHL接口,无需重复请求";
                    Result.ResultDhls.Add(rsl);
                    continue;
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

                        //wayBillInfo.TrackingNumber = responseResult.AirwayBillNumber;
                        //wayBillInfo.CustomerOrderInfo.TrackingNumber = responseResult.AirwayBillNumber;
                        //返回DHL跟踪号给客户
                        try
                        {
                            //获取到要跟新的类容
                            Result.ExpressResponses.Add(_expressService.AddExpressResponseToAPI(response, wayBillInfo));
                            rsl.IsSuccess = true;
                            rsl.WayBillNumber = wayBillInfo.WayBillNumber;
                            rsl.AirwayBillNumber = responseResult.AirwayBillNumber;
                            Result.ResultDhls.Add(rsl);
                        }
                        catch (Exception ex)
                        {
                            Log.Exception(ex);
                            rsl.IsSuccess = false;
                            rsl.WayBillNumber = wayBillInfo.WayBillNumber;
                            rsl.AirwayBillNumber = ex.Message;
                            Result.ResultDhls.Add(rsl);
                            //Result.Add(wayBillInfo.WayBillNumber, ex.Message);
                            //model.Message += "运单号为{0}：错误信息：{1}".FormatWith(wayBillInfo.WayBillNumber, ex.Message);
                        }
                    }
                    else
                    {
                        rsl.IsSuccess = false;
                        rsl.WayBillNumber = wayBillInfo.WayBillNumber;
                        rsl.AirwayBillNumber = "请求DHL接口失败";
                        Result.ResultDhls.Add(rsl);
                        //Result.Add(wayBillInfo.WayBillNumber, "请求DHL接口失败");
                        //model.Message += "运单号为{0}：请求DHL接口失败".FormatWith(wayBillInfo.WayBillNumber);
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    rsl.IsSuccess = false;
                    rsl.WayBillNumber = wayBillInfo.WayBillNumber;
                    rsl.AirwayBillNumber = ex.Message;
                    Result.ResultDhls.Add(rsl);
                    //Result.Add(wayBillInfo.WayBillNumber, ex.Message);
                    //model.Message += "运单号为{0}：错误信息：{1}".FormatWith(wayBillInfo.WayBillNumber, ex.Message);
                }
            }
            return Result;
        }

        public class ResultDHLShipment
        {
            public ResultDHLShipment()
            {
                ExpressResponses=new List<ExpressRespons>();
                ResultDhls=new List<ResultDHL>();
            }
            public List<ResultDHL> ResultDhls { get; set; }
            public List<ExpressRespons> ExpressResponses { get; set; }
        }

        public class ResultDHL
        {
            public string WayBillNumber { get; set; }
            public string AirwayBillNumber { get; set; }
            public bool IsSuccess { get; set; }
        }



        /// <summary>
        /// 验证参数是否有效 (for BatchAdd)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [NonAction]
        private List<WayBillModel> ValidationOrderModel(List<WayBillModel> list)
        {
            //过滤重复的订单号
            var distinctList = list.Distinct(new WayBillModelComparer()).ToList();
            //获取荷兰小包所到国家
            var targetCountryCodeList = _freightService.GetCountryArea(Int32.Parse(sysConfig.NLPOSTShippingMethodID.Split(new string[]{","},StringSplitOptions.RemoveEmptyEntries)[0]));
            //俄罗斯挂号、平邮所到国家
            var ruCountryCodeList = _freightService.GetCountryArea(Int32.Parse(sysConfig.LithuaniaShippingMethodID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0]));
            foreach (var item in list)
            {
                //订单号存在重复
                if (!distinctList.Contains(item))
                {
                    item.IsValid = false;
                    item.ErrorMessage.AppendLine(Resource.Error2004);
                    orderResponseResults.Add(new OrderResponseResult()
                    {
                        CustomerOrderId = item.OrderNumber,
                        Status = (int)Status.Fail,
                        Feedback = Resource.Error2004
                    });
                    continue;
                }
                item.ShippingMethodCode = item.ShippingMethodCode.ToUpperInvariant();
                if (item.Width == null || item.Width<=0)
                {
                    item.Width = 1;
                }
                if (item.Height == null || item.Height<=0)
                {
                    item.Height = 1;
                }
                if (item.Length == null || item.Length<=0)
                {
                    item.Length = 1;
                }
                Data.Entity.ShippingMethodModel shippingMethod = CommonMethodHelper.GetShippingMethodInfo(item.ShippingMethodCode);

                if (shippingMethod == null)
                {
                    item.IsValid = false;
                    item.ErrorMessage.AppendLine(Resource.Error2018);
                    orderResponseResults.Add(new OrderResponseResult()
                    {
                        CustomerOrderId = item.OrderNumber,
                        Status = (int)Status.Fail,
                        Feedback = Resource.Error2018
                    });
                    continue;
                }

                if (item.EnableTariffPrepay == null)//没有赋值 
                {
                    item.EnableTariffPrepay = false;//默认没有开启
                }

                if (item.Weight == null || item.Weight <= 0)//判断重量大于零
                {
                    item.IsValid = false;
                    item.ErrorMessage.AppendLine(Resource.Error2009);
                    orderResponseResults.Add(new OrderResponseResult()
                    {
                        CustomerOrderId = item.OrderNumber,
                        Status = (int)Status.Fail,
                        Feedback = Resource.Error2009
                    });
                    continue;
                }

                if (item.Weight == null || item.Weight <= 0)//判断重量大于零
                {
                    item.IsValid = false;
                    item.ErrorMessage.AppendLine(Resource.Error2009);
                    orderResponseResults.Add(new OrderResponseResult()
                    {
                        CustomerOrderId = item.OrderNumber,
                        Status = (int)Status.Fail,
                        Feedback = Resource.Error2009
                    });
                    continue;
                }

                if (item.PackageNumber == 0)
                {
                    item.IsValid = false;
                    item.ErrorMessage.AppendLine(Resource.Error2017);
                    orderResponseResults.Add(new OrderResponseResult()
                    {
                        CustomerOrderId = item.OrderNumber,
                        Status = (int)Status.Fail,
                        Feedback = Resource.Error2017
                    });
                    continue;
                }

                //Add By zhengsong
                //是否是需要计算偏远附加费 ，需要验证省/州，城市，邮编
                var shipping = CommonMethodHelper.GetShippingMethodInfo(item.ShippingMethodCode.ToUpper());
                if (shipping != null && shipping.FuelRelateRAF)
                {
                    if (item.ShippingInfo == null || string.IsNullOrWhiteSpace(item.ShippingInfo.ShippingZip))
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine(Resource.Error2014);
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = Resource.Error2014
                        });
                        continue;
                    }
                    if (item.ShippingInfo == null || string.IsNullOrWhiteSpace(item.ShippingInfo.ShippingState))
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine(Resource.Error2015);
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = Resource.Error2015
                        });
                        continue;
                    }
                    if (item.ShippingInfo == null || string.IsNullOrWhiteSpace(item.ShippingInfo.ShippingCity))
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine(Resource.Error2016);
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = Resource.Error2016
                        });
                        continue;
                    }
                }
                #region 中邮挂号福州
                //中邮挂号福州
                if (item.ShippingMethodCode != null && (item.ShippingMethodCode.Trim().ToUpperInvariant() == "CNPOST-FZ" || item.ShippingMethodCode.Trim().ToUpperInvariant() == "CNPOST-FYB"))
                {
                    if (item.OrderNumber != null && item.OrderNumber.Length > 30)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("订单号长度必须小于等于30");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "订单号长度必须小于等于30"
                        });
                        continue;
                    }
                    //国家两位
                    if (item.ShippingInfo.CountryCode != null && item.ShippingInfo.CountryCode.Length != 2)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("国家简码必须是两位");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "国家简码必须是两位"
                        });
                        continue;
                    }
                    //收件人州或省
                    if (item.ShippingInfo.ShippingState != null && item.ShippingInfo.ShippingState.Length > 50)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("收件人省或州长度不能超过50");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "收件人省或州长度不能超过50"
                        });
                        continue;
                    }
                    //收件人城市
                    if (item.ShippingInfo.ShippingCity != null && item.ShippingInfo.ShippingCity.Length > 50)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("收件人城市长度不能超过50");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "收件人城市长度不能超过50"
                        });
                        continue;
                    }
                    //收件人地址
                    string address = "";
                    if (item.ShippingInfo.ShippingAddress != null)
                    {
                        address += item.ShippingInfo.ShippingAddress;
                    }
                    if (item.ShippingInfo.ShippingAddress1 != null)
                    {
                        address += item.ShippingInfo.ShippingAddress1;
                    }
                    if (item.ShippingInfo.ShippingAddress2 != null)
                    {
                        address += item.ShippingInfo.ShippingAddress2;
                    }
                    if (address.Length > 120)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("收件人地址长度不能超过120");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "收件人地址长度不能超过120"
                        });
                        continue;
                    }
                    //收件人邮编
                    if (item.ShippingInfo.ShippingZip != null && item.ShippingInfo.ShippingZip.Length > 12)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("收件人邮编长度不能超过12");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "收件人邮编长度不能超过12"
                        });
                        continue;
                    }
                    //收件人名字
                    string name = "";
                    if (item.ShippingInfo.ShippingFirstName != null)
                    {
                        name += item.ShippingInfo.ShippingFirstName;
                    }
                    if (item.ShippingInfo.ShippingLastName != null)
                    {
                        name += item.ShippingInfo.ShippingLastName;
                    }
                    if (name.Length > 64)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("收件人名字长度不能超过64");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "收件人名字长度不能超过64"
                        });
                        continue;
                    }
                    //收件人电话
                    if (item.ShippingInfo.ShippingPhone != null && item.ShippingInfo.ShippingPhone.Length > 20)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("收件人电话长度不能超过20");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "收件人电话长度不能超过20"
                        });
                        continue;
                    }
                    //发件人省份
                    if (item.SenderInfo.SenderState != null && item.SenderInfo.SenderState.Length > 20)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("发件人州省长度不能超过20");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "发件人州省长度不能超过20"
                        });
                        continue;
                    }
                    //发件人城市
                    if (item.SenderInfo.SenderCity != null && item.SenderInfo.SenderCity.Length > 64)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("发件人城市长度不能超过64");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "发件人城市长度不能超过64"
                        });
                        continue;
                    }
                    //发件人街道
                    if (item.SenderInfo.SenderAddress != null && item.SenderInfo.SenderAddress.Length > 120)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("发件人地址长度不能超过120");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "发件人地址长度不能超过120"
                        });
                        continue;
                    }
                    //发件人邮编
                    if (item.SenderInfo.SenderZip != null && item.SenderInfo.SenderZip.Length > 6)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("发件人邮编长度不能超过6");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "发件人邮编长度不能超过6"
                        });
                        continue;
                    }
                    //发件人名字
                    string senderName = "";
                    if (item.SenderInfo.SenderFirstName != null)
                    {
                        senderName += item.SenderInfo.SenderFirstName;
                    }
                    if (item.SenderInfo.SenderLastName != null)
                    {
                        senderName += item.SenderInfo.SenderLastName;
                    }
                    if (senderName.Length > 20)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("发件人名字长度不能超过20");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "发件人名字长度不能超过20"
                        });
                        continue;
                    }
                    //发件人电话
                    if (item.SenderInfo.SenderPhone != null && item.SenderInfo.SenderPhone.Length > 20)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("发件人电话长度不能超过20");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "发件人电话长度不能超过20"
                        });
                        continue;
                    }
                }
                #endregion

                #region DHL 验证

                if (item.ShippingMethodCode != null &&
                    (item.ShippingMethodCode.Trim().ToUpperInvariant() == "HKDHL" || item.ShippingMethodCode.Trim().ToUpperInvariant() == "DHLCN" ||
                     item.ShippingMethodCode.Trim().ToUpperInvariant() == "DHLSG"))
                {
                    if (item.InsureAmount != null && item.InsureAmount.ToString().Length > 14)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("保险金额长度不能超过14个字符");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "保险金额长度不能超过14个字符"
                        });
                        continue;
                    }
                    else if (item.InsureAmount != null && !Regex.IsMatch(item.InsureAmount.ToString(), "^[0-9]+[.]{0,1}[0-9]{0,2}$"))
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("保险金额有数字组成，最多保留两位小数");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "保险金额有数字组成，最多保留两位小数"
                        });
                        continue;
                    }
                    if (item.ShippingInfo.ShippingCompany !=null && item.ShippingInfo.ShippingCompany.Length > 35)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("收件人公司长度为0-35个字符");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "收件人公司长度为0-35个字符"
                        });
                        continue;
                    }

                    try
                    {
                        string address = "";
                        if (!string.IsNullOrWhiteSpace(item.ShippingInfo.ShippingAddress))
                        {
                            address += item.ShippingInfo.ShippingAddress;
                        }
                        if (!string.IsNullOrWhiteSpace(item.ShippingInfo.ShippingAddress1))
                        {
                            address += item.ShippingInfo.ShippingAddress1;
                        }
                        if (!string.IsNullOrWhiteSpace(item.ShippingInfo.ShippingAddress2))
                        {
                            address += item.ShippingInfo.ShippingAddress2;
                        }
                        if (string.IsNullOrWhiteSpace(address) || address.StringSplitLengthWords(35).Count > 2)
                        {
                            item.IsValid = false;
                            item.ErrorMessage.AppendLine("收件人地址为空或者超长");
                            orderResponseResults.Add(new OrderResponseResult()
                            {
                                CustomerOrderId = item.OrderNumber,
                                Status = (int)Status.Fail,
                                Feedback = "收件人地址为空或者超长"
                            });
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.ToString()); 
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("收件人地址格式错误");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "收件人地址格式错误"
                        });
                        continue;
                    }

                    if (item.ShippingInfo.ShippingCity != null && item.ShippingInfo.ShippingCity.Length > 35)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("收件人城市不能超过35个字符");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "收件人城市不能超过35个字符"
                        });
                        continue;
                    }

                    if (item.ShippingInfo.ShippingState != null && item.ShippingInfo.ShippingState.Length > 35)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("收件人州/省长度为1-35个字符");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "收件人州/省长度为1-35个字符"
                        });
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(item.ShippingInfo.ShippingZip) ||
                        item.ShippingInfo.ShippingZip.Length > 12)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("收件人邮编长度为1-12个字符");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "收件人邮编长度为1-12个字符"
                        });
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(item.ShippingInfo.ShippingTaxId) &&
                        item.ShippingInfo.ShippingTaxId.Length > 20)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("收件人税号不能超过20字符");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "收件人税号不能超过20字符"
                        });
                        continue;
                    }
                    if (
                        (item.ShippingInfo.ShippingFirstName + item.ShippingInfo.ShippingLastName)
                            .Length > 35)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("收件人姓名不能超过35个字符");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "收件人姓名不能超过35个字符"
                        });
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(item.ShippingInfo.ShippingPhone))
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("收件人电话不能为空");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "收件人电话不能为空"
                        });
                        continue;
                    }
                    else if (item.ShippingInfo.ShippingPhone != null && item.ShippingInfo.ShippingPhone.Length > 25)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("收件人电话不能超过25个字符");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "收件人电话不能超过25个字符"
                        });
                        continue;
                    }
                }

                #endregion

                #region EUB 订单上传验证

                if (item.ShippingMethodCode != null &&
                    (item.ShippingMethodCode.Trim().ToUpperInvariant() == "EUB_CS" || item.ShippingMethodCode.Trim().ToUpperInvariant() == "EUB-SZ" ||
                     item.ShippingMethodCode.Trim().ToUpperInvariant() == "EUB-FZ"))
                {
                    if (item.OrderNumber != null && (item.OrderNumber.Length > 32 || item.OrderNumber.Length < 4))
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("订单号长度必须为4-32个字符");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "订单号长度必须为4-32个字符"
                        });
                        continue;
                    }
                    if (
                        (item.ShippingInfo.ShippingFirstName + item.ShippingInfo.ShippingLastName)
                            .Length > 256)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("收件人姓名不能超过256个字符");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "收件人姓名不能超过256个字符"
                        });
                        continue;
                    }
                    if (item.ShippingInfo.ShippingCity != null &&
                        item.ShippingInfo.ShippingCity.Length > 128)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("收件人城市不能超过128个字符");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "收件人城市不能超过128个字符"
                        });
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(item.ShippingInfo.ShippingState) || item.ShippingInfo.ShippingState.Length > 128)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("收件人州长度为1-128个字符");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "收件人州长度为1-128个字符"
                        });
                        continue;
                    }

                    if (item.ShippingInfo.ShippingZip != null)
                    {
                        if (item.ShippingInfo.ShippingZip.Length > 16)
                        {
                            item.IsValid = false;
                            item.ErrorMessage.AppendLine("收件人邮编不能超过16个字符");
                            orderResponseResults.Add(new OrderResponseResult()
                            {
                                CustomerOrderId = item.OrderNumber,
                                Status = (int)Status.Fail,
                                Feedback = "收件人邮编不能超过16个字符"
                            });
                            continue;
                        }
                        else
                        {
                            switch (item.ShippingInfo.CountryCode.ToUpperInvariant())
                            {
                                case "US":
                                    if (!Regex.IsMatch(item.ShippingInfo.ShippingZip, "^(^[0-9]{5}-[0-9]{4}$)|(^[0-9]{5}-[0-9]{5}$)|(^[0-9]{5})$"))
                                    {
                                        item.IsValid = false;
                                        item.ErrorMessage.AppendLine("邮编不合法");
                                        orderResponseResults.Add(new OrderResponseResult()
                                        {
                                            CustomerOrderId = item.OrderNumber,
                                            Status = (int)Status.Fail,
                                            Feedback = "邮编不合法"
                                        });
                                        continue;
                                    }
                                    break;
                                case "AU":
                                    if (!Regex.IsMatch(item.ShippingInfo.ShippingZip, "^[0-9]{4}$"))
                                    {
                                        item.IsValid = false;
                                        item.ErrorMessage.AppendLine("邮编不合法");
                                        orderResponseResults.Add(new OrderResponseResult()
                                        {
                                            CustomerOrderId = item.OrderNumber,
                                            Status = (int)Status.Fail,
                                            Feedback = "邮编不合法"
                                        });
                                        continue;
                                    }
                                    break;
                                case "CA":
                                    if (!Regex.IsMatch(item.ShippingInfo.ShippingZip, "^(^[A-Za-z][0-9][A-Za-z][ ][0-9][A-Za-z][0-9]$)|(^[A-Za-z][0-9][A-Za-z][0-9][A-Za-z][0-9]$)$"))
                                    {
                                        item.IsValid = false;
                                        item.ErrorMessage.AppendLine("邮编不合法");
                                        orderResponseResults.Add(new OrderResponseResult()
                                        {
                                            CustomerOrderId = item.OrderNumber,
                                            Status = (int)Status.Fail,
                                            Feedback = "邮编不合法"
                                        });
                                        continue;
                                    }
                                    break;
                                case "GB":
                                    if (!Regex.IsMatch(item.ShippingInfo.ShippingZip, "^[A-Za-z0-9]{2,4} [A-Za-z0-9]{3}$"))
                                    {
                                        item.IsValid = false;
                                        item.ErrorMessage.AppendLine("邮编不合法");
                                        orderResponseResults.Add(new OrderResponseResult()
                                        {
                                            CustomerOrderId = item.OrderNumber,
                                            Status = (int)Status.Fail,
                                            Feedback = "邮编不合法"
                                        });
                                        continue;
                                    }
                                    break;
                                case "FR":
                                    if (!Regex.IsMatch(item.ShippingInfo.ShippingZip, "^[0-9]{5}$"))
                                    {
                                        item.IsValid = false;
                                        item.ErrorMessage.AppendLine("邮编不合法");
                                        orderResponseResults.Add(new OrderResponseResult()
                                        {
                                            CustomerOrderId = item.OrderNumber,
                                            Status = (int)Status.Fail,
                                            Feedback = "邮编不合法"
                                        });
                                        continue;
                                    }
                                    break;
                                case "RU":
                                    if (!Regex.IsMatch(item.ShippingInfo.ShippingZip, "^[0-9]{6}$"))
                                    {
                                        item.IsValid = false;
                                        item.ErrorMessage.AppendLine("邮编不合法");
                                        orderResponseResults.Add(new OrderResponseResult()
                                        {
                                            CustomerOrderId = item.OrderNumber,
                                            Status = (int)Status.Fail,
                                            Feedback = "邮编不合法"
                                        });
                                        continue;
                                    }
                                    break;
                            }
                        }
                    }
                    else if (item.ShippingInfo.CountryCode.ToUpperInvariant() != "HK")
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine("邮编不能为空");
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = "邮编不能为空"
                        });
                        continue;
                    }

                }
                #endregion

                //判断目的地国家是否是俄罗斯
                if (item.ShippingMethodCode.Trim().ToUpperInvariant() == "LTPOST")
                {
                    if (item.ShippingInfo == null || string.IsNullOrWhiteSpace(item.ShippingInfo.CountryCode))
                    {
                        if (item.ShippingInfo.CountryCode.ToUpperInvariant() == "RU" &&
                            string.IsNullOrWhiteSpace(item.ShippingInfo.ShippingZip))
                        {
                            item.IsValid = false;
                            item.ErrorMessage.AppendLine(Resource.Error2014);
                            orderResponseResults.Add(new OrderResponseResult()
                                {
                                    CustomerOrderId = item.OrderNumber,
                                    Status = (int) Status.Fail,
                                    Feedback = Resource.Error2014
                                });
                            continue;
                        }
                    }
                }


                //欧洲专线上传 限制
                //Add By zhengsong
                if (sysConfig.DDPShippingMethodCode == item.ShippingMethodCode.ToUpper() ||
                    sysConfig.DDPRegisterShippingMethodCode == item.ShippingMethodCode.ToUpper() ||
                    sysConfig.EuropeShippingMethodCode == item.ShippingMethodCode.ToUpper())
                {
                    // OrderNumber 上传系统时限制只能是数字或字母，不能有其他符合  ，比如 - （ ）*，字符数量小于25
                    //Regex r = new Regex(@"^[A-Za-z0-9]{0,25}$");
                    //MatchCollection customerOrderNumber = r.Matches(item.OrderNumber);
                    //if (customerOrderNumber.Count < 1 || customerOrderNumber[0].Value != item.OrderNumber)
                    //{
                    //    item.IsValid = false;
                    //    item.ErrorMessage.AppendLine(Resource.Error2013);
                    //    orderResponseResults.Add(new OrderResponseResult()
                    //    {
                    //        CustomerOrderId = item.OrderNumber,
                    //        Status = (int)Status.Fail,
                    //        Feedback = Resource.Error2013
                    //    });
                    //    continue;
                    //}

                    if (item.OrderNumber.Length > 25)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine(Resource.Error2013);
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = Resource.Error2013
                        });
                        continue;
                    }


                    ////PhoneNumber   只能是数字，不能出现其他字符，比如：&#43; &amp;
                    //Regex c = new Regex(@"^[0-9]{0,}$");
                    //MatchCollection shippingPhone = c.Matches(item.ShippingInfo.ShippingPhone);
                    //if (shippingPhone.Count < 1 || shippingPhone[0].Value != item.ShippingInfo.ShippingPhone)
                    //{
                    //    item.IsValid = false;
                    //    item.ErrorMessage.AppendLine(Resource.Error2012);
                    //    orderResponseResults.Add(new OrderResponseResult()
                    //    {
                    //        CustomerOrderId = item.OrderNumber,
                    //        Status = (int)Status.Fail,
                    //        Feedback = Resource.Error2012
                    //    });
                    //    continue;
                    //}
                }

                #region 关税预付
                if (!string.IsNullOrEmpty(item.ShippingMethodCode))
                {
                    //客户对应的运输方式是否关税预付 yungchu
                    List<TariffPrepayFeeShippingMethod> listTariffPrepayFee = _freightService.GetShippingMethodsTariffPrepay(CustomerCode);

                    if (item.EnableTariffPrepay.Value)
                    {
                        if (listTariffPrepayFee == null || listTariffPrepayFee.Count == 0)
                        {
                            item.IsValid = false;
                            item.ErrorMessage.AppendLine(Resource.Error2008);
                            orderResponseResults.Add(new OrderResponseResult()
                            {
                                CustomerOrderId = item.OrderNumber,
                                Status = (int)Status.Fail,
                                Feedback = Resource.Error2008
                            });
                            continue;
                        }
                        else  //客户是否开启该运输方式关税预付
                        {
                            List<string> listStr = new List<string>();
                            listTariffPrepayFee.ForEach(a => listStr.Add(a.Code.ToUpperInvariant()));
                            if (!listStr.Contains(item.ShippingMethodCode.ToUpperInvariant()))
                            {
                                item.IsValid = false;
                                item.ErrorMessage.AppendLine(Resource.Error2008);
                                orderResponseResults.Add(new OrderResponseResult()
                                {
                                    CustomerOrderId = item.OrderNumber,
                                    Status = (int)Status.Fail,
                                    Feedback = Resource.Error2008
                                });
                                continue;
                            }

                        }
                    }
                }
                #endregion

                #region 订单号在数据库中是否存在
                var customerOrder = _customerOrderService.GetCustomerOrderInfo(item.OrderNumber);//从数据库获取运单信息，并判断运单的跟踪号是否为空
                if (null != customerOrder)
                {
                    item.IsValid = false;
                    item.ErrorMessage.AppendLine(Resource.Error2005);
                    orderResponseResults.Add(new OrderResponseResult()
                    {
                        CustomerOrderId = item.OrderNumber,
                        Status = (int)Status.Fail,
                        Feedback = Resource.Error2005
                    });
                    continue;
                }
                #endregion

                #region 跟踪号在数据库中已经存在
                if (!string.IsNullOrWhiteSpace(item.TrackingNumber))
                {
                    var trackNunber = _orderService.IsExitTrackingNumber(item.TrackingNumber);//从数据库获取运单信息，并判断运单的跟踪号是否为空
                    if (trackNunber)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine(Resource.Error2007);
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.TrackingNumber,
                            Status = (int)Status.Fail,
                            Feedback = Resource.Error2007
                        });
                        continue;
                    }
                }
                #endregion

                #region 验证参数

                
                item.InShippingMethodId = shippingMethod.ShippingMethodId;
                item.InShippingMethodName = shippingMethod.FullName;

                
                
                var shippingInfoSinoUSValidator = new ShippingInfoSinoUSModelValidator();
                var senderInfoValidator = new SenderInfoModelValidator();

                AbstractValidator<ApplicationInfoModel> applicationInfoValidator;

                if (item.ShippingMethodCode.ToUpperInvariant() == "EUDDP" || item.ShippingMethodCode.ToUpperInvariant() == "EUDDPG" || sysConfig.EuropeShippingMethodCode == item.ShippingMethodCode.ToUpperInvariant())
                {
                    applicationInfoValidator = new ApplicationInfoModelEudValidator();
                }
                else if (sysConfig.NLPOSTShippingMethodID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Contains(item.InShippingMethodId.ToString()))
                {
                    //顺丰荷兰小包
                    applicationInfoValidator=new ApplicationInfoModelNlpostValidator();
                }                
                else if (sysConfig.LithuaniaShippingMethodID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Contains(item.InShippingMethodId.ToString()))
                {
                    //俄罗斯挂号、平邮
                    applicationInfoValidator=new ApplicationInfoModelSfModelValidator();
                }                
                else if (item.ShippingMethodCode.ToUpperInvariant() == "CNPOST-FZ" || item.ShippingMethodCode.ToUpperInvariant() == "CNPOSTP_FZ" || item.ShippingMethodCode.Trim().ToUpperInvariant() == "CNPOST-FYB")
                {
                    //福州邮政申报信息判断
                    applicationInfoValidator = new ApplicationInfoFuZhouModelValidator();
                }
                else if (item.ShippingMethodCode != null &&
                   (item.ShippingMethodCode.Trim().ToUpperInvariant() == "EUB_CS" || item.ShippingMethodCode.Trim().ToUpperInvariant() == "EUB-SZ" ||
                    item.ShippingMethodCode.Trim().ToUpperInvariant() == "EUB-FZ"))
                {
                    //EUB申报信息判断
                    applicationInfoValidator = new ApplicationInfoEUBModelValidator();
                }
                else if (item.ShippingMethodCode != null &&
                         (item.ShippingMethodCode.Trim().ToUpperInvariant() == "HKDHL" ||
                          item.ShippingMethodCode.Trim().ToUpperInvariant() == "DHLCN" ||
                          item.ShippingMethodCode.Trim().ToUpperInvariant() == "DHLSG"))
                {
                    //DHL申报信息判断
                    applicationInfoValidator = new ApplicationInfoDHLModelValidator();
                }else
                {
                    applicationInfoValidator = new ApplicationInfoModelValidator();
                }
                var wayBillModelValidator = new WayBillModelValidator();
                ValidationResult results = wayBillModelValidator.Validate(item);

                bool validationSucceeded = results.IsValid;
                IList<ValidationFailure> failures = results.Errors;
                if (!validationSucceeded)
                {
                    item.IsValid = false;
                    foreach (var error in failures)
                    {
                        item.ErrorMessage.AppendLine(error.ErrorMessage);
                    }
                }

                //判断重量
                if (item.Weight == null || item.Weight <= 0)
                {
                    item.IsValid = false;
                    item.ErrorMessage.AppendLine("实际重量不能小于等于0");
                    continue;
                }

                if (item.ShippingMethodCode == "SPLUSZ")
                {
                    ValidationResult shippingInfoResults = shippingInfoSinoUSValidator.Validate(item.ShippingInfo);

                    if (!shippingInfoResults.IsValid)
                    {
                        item.IsValid = false;
                        IList<ValidationFailure> shippingFailures = shippingInfoResults.Errors;
                        foreach (var error in shippingFailures)
                        {
                            item.ErrorMessage.AppendLine(error.ErrorMessage);
                        }
                    }

                    #region 订单号是否超长

                    if (item.OrderNumber.Trim().Length > 35)
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine(Resource.Error2002);
                        orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = Resource.Error2002
                        });
                        continue;
                    }
                    #endregion
                }
                else
                {
                    AbstractValidator<ShippingInfoModel> shippingInfoValidator;
                    if (sysConfig.NLPOSTShippingMethodID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Contains(item.InShippingMethodId.ToString()))
                    {
                        shippingInfoValidator=new ShippingInfoNlpostModelValidator();
                    }
                    else if (sysConfig.LithuaniaShippingMethodID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Contains(item.InShippingMethodId.ToString()))
                    {
                        shippingInfoValidator=new ShippingInfoSfModelValidator();
                    }
                    else
                    {
                        shippingInfoValidator = new ShippingInfoModelValidator();
                    }
                    //验证收件人
                    ValidationResult shippingInfoResults = shippingInfoValidator.Validate(item.ShippingInfo);

                    if (!shippingInfoResults.IsValid)
                    {
                        item.IsValid = false;
                        IList<ValidationFailure> shippingFailures = shippingInfoResults.Errors;
                        foreach (var error in shippingFailures)
                        {
                            item.ErrorMessage.AppendLine(error.ErrorMessage);
                        }
                    }
                }
                //验证发件人
                if (item.SenderInfo != null)
                {
                    ValidationResult senderInfoResults = senderInfoValidator.Validate(item.SenderInfo);

                    if (!senderInfoResults.IsValid)
                    {
                        item.IsValid = false;
                        IList<ValidationFailure> senderFailures = senderInfoResults.Errors;
                        foreach (var error in senderFailures)
                        {
                            item.ErrorMessage.AppendLine(error.ErrorMessage);
                        }
                    }
                }
                //验证申报信息
                if (item.ApplicationInfos.Count <= 0)
                {
                    item.IsValid = false;
                    item.ErrorMessage.AppendLine("最少需要要有一条申报信息");
                    orderResponseResults.Add(new OrderResponseResult()
                    {
                        CustomerOrderId = item.OrderNumber,
                        Status = (int)Status.Fail,
                        Feedback = "最少需要要有一条申报信息"
                    });
                    continue;
                }
                else
                {
                    foreach (var appInfo in item.ApplicationInfos)
                    {
                        ValidationResult appInfoResults = applicationInfoValidator.Validate(appInfo);

                        if (appInfoResults.IsValid) continue;
                        item.IsValid = false;
                        IList<ValidationFailure> appInfoFailures = appInfoResults.Errors;
                        foreach (var error in appInfoFailures)
                        {
                            item.ErrorMessage.AppendLine(error.ErrorMessage);
                        }
                    }
                }
                #endregion

                if (!item.IsValid)
                {
                    orderResponseResults.Add(new OrderResponseResult()
                    {
                        CustomerOrderId = item.OrderNumber,
                        Status = (int)Status.Fail,
                        Feedback = item.ErrorMessage.ToString()
                    });
                    continue;
                }
                //判断跟踪号是否为空,如果为空则判断需要系统分配跟踪号

                

                #region 验证荷兰小包是否发送到此国家
                if (sysConfig.NLPOSTShippingMethodID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Contains(item.InShippingMethodId.ToString()))
                {
                    if (
                        !targetCountryCodeList.Exists(
                            p =>
                            p.CountryCode.ToUpperInvariant() ==
                            item.ShippingInfo.CountryCode.ToUpperInvariant()))
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine(Resource.Error2010);
                        orderResponseResults.Add(new OrderResponseResult()
                            {
                                CustomerOrderId = item.OrderNumber,
                                Status = (int)Status.Fail,
                                Feedback = Resource.Error2010
                            });
                        continue;
                    }
                }
                #endregion
                
                #region 验证俄罗斯挂号、平邮发送
                if (
                            sysConfig.LithuaniaShippingMethodID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                     .Contains(item.InShippingMethodId.ToString()))
                {
                    if (
                        !ruCountryCodeList.Exists(
                            p =>
                            p.CountryCode.ToUpperInvariant() ==
                            item.ShippingInfo.CountryCode.ToUpperInvariant()))
                    {
                        item.IsValid = false;
                        item.ErrorMessage.AppendLine(Resource.Error2010);
                        orderResponseResults.Add(new OrderResponseResult()
                            {
                                CustomerOrderId = item.OrderNumber,
                                Status = (int)Status.Fail,
                                Feedback = Resource.Error2010
                            });
                        continue;
                    }
                } 
                #endregion
                //跟踪号为空
                if (string.IsNullOrWhiteSpace(item.TrackingNumber) && !sysConfig.NLPOSTShippingMethodID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Contains(item.InShippingMethodId.ToString()))
                {
                    // update huhaiyou 2014-06-30
                    //Data.Entity.ShippingMethodModel shippingMethod =
                    //    CommonMethodHelper.GetShippingMethodInfo(item.ShippingMethodCode);
                    //item.InShippingMethodId = shippingMethod.ShippingMethodId;
                    //item.InShippingMethodName = shippingMethod.FullName;

                    #region 运输方式需要系统分配跟踪号,则根据订单运输方式及国家获取 否则直接获取传过来的跟踪号

                    if (shippingMethod.IsSysTrackNumber)
                    {
                        var trackingNumbers = GetTrackNumber(shippingMethod.ShippingMethodId, item.ShippingInfo.CountryCode, list);

                        if (trackingNumbers != null)
                        {
                            // trackingNumberDetailInfos.Add(trackNumberDetail);
                            //detailIds.Add(trackNumberDetail.TrackingNumberDetailID);
                            item.TrackingNumber = trackingNumbers;
                        }
                        else
                        {
                            item.IsValid = false;
                            orderResponseResults.Add(new OrderResponseResult()
                                {
                                    CustomerOrderId = item.OrderNumber,
                                    Status = (int)Status.Fail,
                                    Feedback = Resource.Error2006
                                });
                            continue;
                        }
                    }
                    #endregion
                }


                //产生运单号
                //item.WayBillNumber = SequenceNumberService.GetSequenceNumber(PrefixCode.OrderID);
                item.WayBillNumber = SequenceNumberService.GetWayBillNumber(CustomerCode);
                if (string.IsNullOrWhiteSpace(item.WayBillNumber))
                {
                    orderResponseResults.Add(new OrderResponseResult()
                        {
                            CustomerOrderId = item.OrderNumber,
                            Status = (int)Status.Fail,
                            Feedback = Resource.Error2003
                        });
                }
            }

            return list;
        }

        private string GetTrackNumber(int shippingMethodId, string countryCode, List<WayBillModel> list)
        {
            try
            {
                while (true)
                {
                    var trackingNumbers = _trackingNumberService.TrackNumberAssignStandard(shippingMethodId, 1, countryCode);
                    if (trackingNumbers != null && trackingNumbers.Any())
                    {
                        if (list.Any(t => t.TrackingNumber == trackingNumbers[0]))
                        {
                            //重复
                        }
                        else
                        {
                            return trackingNumbers[0];//不重复
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return null;
            }
        }


        /// <summary>
        /// 请求查询跟踪号接口
        /// </summary>
        /// <param name="orderId">客户订单号,多个以逗号分开</param>
        /// <returns></returns>
        /// api/WayBill/GetTrackNumber
        public Response<List<OrderInfo>> GetTrackNumber(string orderId)
        {
            if (!string.IsNullOrWhiteSpace(orderId))
            {
                if (orderId.Length > 2000)
                {
                    return new Response<List<OrderInfo>>()
                    {
                        Item = null,
                        ResultCode = GetErrorCode(ErrorCode.Error2002),
                        ResultDesc = Resource.Error2002
                    };
                }

                string[] orderIds = orderId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (orderIds.Length > 0)
                {
                    try
                    {
                        var order = _customerOrderService.GetCustomerOrderList(orderIds, CustomerCode);
                        if (order.Count > 0)
                        {
                            var orderList = new List<OrderInfo>();

                            order.ForEach(p => orderList.Add(new OrderInfo()
                                {
                                    OrderNumber = p.CustomerOrderNumber,
                                    TrackingNumber = string.IsNullOrWhiteSpace(p.TrackingNumber) ? "" : p.TrackingNumber,
                                    WayBillNumber = p.WayBillNumber
                                }));
                            var res = new Response<List<OrderInfo>>()
                            {
                                Item = orderList,
                                ResultCode = GetErrorCode(ErrorCode.Error0000),
                                ResultDesc = Resource.Error0000
                            };
                            return res;
                        }
                    }
                    catch (Exception)
                    {
                        ;
                    }

                }
            }
            return new Response<List<OrderInfo>>()
            {
                Item = null,
                ResultCode = GetErrorCode(ErrorCode.Error1006),
                ResultDesc = Resource.Error1006
            };
        }
        /// <summary>
        /// 根据订单号和运单号查询代理单号
        /// </summary>
        /// <param name="orderIds">客户订单号,多个以逗号分开</param>
        /// <returns></returns>
        public Response<List<AgentNumberInfo>> GetAgentNumbers(string orderIds)
        {
            if (!CustomerCode.IsNullOrWhiteSpace() && !orderIds.IsNullOrWhiteSpace())
            {
                var customerCodes = sysConfig.IsTurnSFNumber.Split(',').ToList();
                if (customerCodes.Contains(CustomerCode))
                {
                    try
                    {
                        return new Response<List<AgentNumberInfo>>()
                            {
                                Item = _expressService.GetAgentNumbers(orderIds.Split(',').ToList(), CustomerCode),
                                ResultCode = GetErrorCode(ErrorCode.Error0000),
                                ResultDesc = Resource.Error0000
                            };
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                    }
                }
            }
            return new Response<List<AgentNumberInfo>>()
                    {
                        Item = null,
                        ResultCode = GetErrorCode(ErrorCode.Error1006),
                        ResultDesc = Resource.Error1006
                    };
        }



        /// <summary>
        /// 预申请运输方式的跟踪号
        /// </summary>
        /// <param name="code">为运输方式code</param>
        /// <returns></returns>
        /// api/WayBill/GetTrackNumbers    code为运输方式code
        public Response<List<TrackNumberInfo>> GetTrackNumbers(string code)
        {

            if (string.IsNullOrWhiteSpace(code))
            {
                return new Response<List<TrackNumberInfo>>()
                    {
                        Item = null,
                        ResultCode = GetErrorCode(ErrorCode.Error1004),
                        ResultDesc = Resource.Error1004
                    };
            }
            try
            {
                var shippingModel = _freightService.GetShippingMethodByCode(code);
                if (shippingModel == null)
                {
                    return new Response<List<TrackNumberInfo>>()
                        {
                            Item = null,
                            ResultCode = GetErrorCode(ErrorCode.Error1006),
                            ResultDesc = Resource.Error1006
                        };
                }
                var trackingNumbers = _trackingNumberService.TrackNumberAssignStandard(shippingModel.ShippingMethodId, 100, string.Empty).ToList();
                if (trackingNumbers.Count == 0)
                {
                    return new Response<List<TrackNumberInfo>>()
                        {
                            Item = null,
                            ResultCode = GetErrorCode(ErrorCode.Error1006),
                            ResultDesc = Resource.Error1006
                        };
                }
                var list = new List<TrackNumberInfo>();

                trackingNumbers.ForEach(p => list.Add(new TrackNumberInfo()
                    {
                        TrackingNumber = p
                    }));
                return new Response<List<TrackNumberInfo>>()
                    {
                        Item = list,
                        ResultCode = GetErrorCode(ErrorCode.Error0000),
                        ResultDesc = Resource.Error0000 + ",获取跟踪号数量:" + trackingNumbers.Count
                    };

            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return new Response<List<TrackNumberInfo>>()
                {
                    Item = null,
                    ResultCode = GetErrorCode(ErrorCode.Error9999),
                    ResultDesc = Resource.Error9999
                };
        }
        /// <summary>
        /// 向顺丰申请订单发货确定
        /// </summary>
        /// <param name="wayBillNumber">运单号</param>
        /// <param name="mailno">顺丰单号</param>
        /// <param name="dealType">订单操作标识 :1 -订单确认 2-消单</param>
        /// <returns></returns>
        private bool NLPOSTConfirm(string wayBillNumber, string mailno, int dealType = 1)
        {
            string confirmXML = @"<Request service='OrderConfirmService' lang='en'>
                                                                     <Head>" + sysConfig.ClientCode + @"</Head>
                                                                     <Body>
                                                                        <OrderConfirm orderid ='" + wayBillNumber.WayBillNumberReplace() + @"' 
                                                                                      mailno='" + mailno + @"'
                                                                                      dealtype='" + dealType.ToString() + @"'>
                                                                        </OrderConfirm>
                                                                    </Body>
                                                                </Request>";
            string responseResult = LMSSFService.SfExpressService(confirmXML, (confirmXML + sysConfig.SFCheckWord).ToMD5());
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(responseResult);
            XmlNode root = xdoc.SelectSingleNode("/Response/Head");
            if (root != null && root.InnerText == "OK")
            {
                //var parcel = new NetherlandsParcelRespons();
                //XmlNode o = xdoc.SelectSingleNode("/Response/Body/OrderConfirmResponse");
                //if (o != null && o.Attributes != null)
                //{
                //    if (o.Attributes["mailno"] != null)
                //    {
                //        parcel.MailNo = o.Attributes["mailno"].Value;
                //    }
                //    if (o.Attributes["orderid"] != null)
                //    {
                //        parcel.WayBillNumber = o.Attributes["orderid"].Value;
                //    }
                //    parcel.Status = 2;
                return true;
                //}
            }
            else if (root != null && root.InnerText == "ERR")
            {
                XmlNode err = xdoc.SelectSingleNode("/Response/ERROR");
                if (err != null && err.Attributes != null && err.Attributes["code"] != null)
                {
                    Log.Error("运单号为：{2}订单发货确定顺丰API错误代码为:{0},错误信息：{1}".FormatWith(err.Attributes["code"], err.InnerText,
                                                                             wayBillNumber));

                }
            }
            return false;
        }
        /// <summary>
        /// 向顺丰下订单
        /// </summary>
        /// <param name="wayBillInfo"></param>
        /// <returns></returns>
        private NetherlandsParcelRespons NLPOST(WayBillInfo wayBillInfo,ref string nlerrmsg)
        {
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
                                                             d_address='" + string.Format("{0} {1} {2}", (wayBillInfo.ShippingInfo.ShippingAddress ?? "").Trim(), (wayBillInfo.ShippingInfo.ShippingAddress1 ?? "").Trim(),(wayBillInfo.ShippingInfo.ShippingAddress2 ?? "").Trim()).ToDBC().StripXML() + @"' 
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
                    return parcel;
                }
            }
            else if (root != null && root.InnerText == "ERR")
            {
                XmlNode err = xdoc.SelectSingleNode("/Response/ERROR");
                if (err != null && err.Attributes != null && err.Attributes["code"] != null&&!err.Attributes["code"].Value.IsNullOrWhiteSpace())
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
                                                         .GetString(p).IsNullOrWhiteSpace() ? "[{0}]数据格式验证失败".FormatWith(p) : "[{0}]{1}".FormatWith(p, LMS.Data.Express.NLPOST.ErrorCode.ResourceManager
                                                         .GetString(p))));
                        Log.Error("运单号为：{2}提交顺丰API错误代码为:{0},错误信息：{1}".FormatWith(err.Attributes["code"].Value,
                                                                                 string.Join(",",errmsg),
                                                                                 wayBillInfo.WayBillNumber));
                        nlerrmsg = string.Join(",", errmsg);
                    }
                    else
                    {
                        var parcel = SearchNLPOST(wayBillInfo.WayBillNumber);
                        if (!parcel.AgentMailNo.IsNullOrWhiteSpace() && !parcel.Remark.IsNullOrWhiteSpace() &&
                            parcel.Remark == "A")
                        {
                            return parcel;
                        }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 查询顺丰订单
        /// </summary>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        private NetherlandsParcelRespons SearchNLPOST(string wayBillNumber)
        {
            var model = new NetherlandsParcelRespons() { WayBillNumber = wayBillNumber };
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





        /// <summary>
        /// 用户注册
        /// add by yungchu
        /// </summary>
        /// <returns></returns>
        public Response<CustomerModel> RegisterUser(RegisterCustomerModel registerModel)
        {

            Customer customer = new Customer();

            #region 数据验证

            if (string.IsNullOrWhiteSpace(registerModel.AccountId))
            {
                return new Response<CustomerModel>()
                {
                    Item = null,
                    ResultCode = GetErrorCode(ErrorCode.Error2020),
                    ResultDesc = Resource.Error2020
                };
            }
            else //格式错误
            {

                registerModel.AccountId = registerModel.AccountId.Trim();
                if (registerModel.AccountId.Length > 100)
                {
                    return new Response<CustomerModel>()
                    {
                        Item = null,
                        ResultCode = GetErrorCode(ErrorCode.Error2002),
                        ResultDesc = Resource.Error2002
                    };
                }

                //用户注册只能输入 数字、字母、点号、@ 、下划线（英文）、中划线（英文）
                if (!Regex.IsMatch(registerModel.AccountId, @"^[A-Za-z0-9.@_-]*$"))
                 {
                     return new Response<CustomerModel>()
                    {
                        Item = null,
                        ResultCode = GetErrorCode(ErrorCode.Error2020),
                        ResultDesc = Resource.Error2020
                    };
                 }
            }


            if (string.IsNullOrWhiteSpace(registerModel.AccountPassWord))
            {
                return new Response<CustomerModel>()
                {
                    Item = null,
                    ResultCode = GetErrorCode(ErrorCode.Error2021),
                    ResultDesc = Resource.Error2021
                };
            }
            else
            {
                registerModel.AccountPassWord = registerModel.AccountPassWord.Trim();
                if (registerModel.AccountPassWord.Length > 128)
                {
                    return new Response<CustomerModel>()
                    {
                        Item = null,
                        ResultCode = GetErrorCode(ErrorCode.Error2002),
                        ResultDesc = Resource.Error2002
                    };
                }
            }

            if (string.IsNullOrWhiteSpace(registerModel.AccountConfirmPassWord) || registerModel.AccountConfirmPassWord != registerModel.AccountPassWord)
            {
                 return new Response<CustomerModel>()
                {
                    Item = null,
                    ResultCode = GetErrorCode(ErrorCode.Error2022),
                    ResultDesc = Resource.Error2022
                };
            }

            if (string.IsNullOrWhiteSpace(registerModel.LinkMan))
            {
                return new Response<CustomerModel>()
                {
                    Item = null,
                    ResultCode = GetErrorCode(ErrorCode.Error2023),
                    ResultDesc = Resource.Error2023
                };
            }
            else
            {
                registerModel.LinkMan = registerModel.LinkMan.Trim();
                if (registerModel.LinkMan.Length>50)
                {
                    return new Response<CustomerModel>()
                    {
                        Item = null,
                        ResultCode = GetErrorCode(ErrorCode.Error2002),
                        ResultDesc = Resource.Error2002
                    };
                }
            }



            if (string.IsNullOrWhiteSpace(registerModel.Tele))
            {
                return new Response<CustomerModel>()
                {
                    Item = null,
                    ResultCode = GetErrorCode(ErrorCode.Error2024),
                    ResultDesc = Resource.Error2024
                };
            }
            else
            {
                registerModel.Tele = registerModel.Tele.Trim();
                if (registerModel.Tele.Length>64)
                {
                    return new Response<CustomerModel>()
                    {
                        Item = null,
                        ResultCode = GetErrorCode(ErrorCode.Error2002),
                        ResultDesc = Resource.Error2002
                    };
                }
            }

            if (string.IsNullOrWhiteSpace(registerModel.Name))
            {
                return new Response<CustomerModel>()
                {
                    Item = null,
                    ResultCode = GetErrorCode(ErrorCode.Error2025),
                    ResultDesc = Resource.Error2025
                };
            }
            else
            {
                registerModel.Name = registerModel.Name.Trim();
                if (registerModel.Name.Length>128)
                {
                    return new Response<CustomerModel>()
                    {
                        Item = null,
                        ResultCode = GetErrorCode(ErrorCode.Error2002),
                        ResultDesc = Resource.Error2002
                    };
                }
            }




            if (string.IsNullOrWhiteSpace(registerModel.Email))
            {
                return new Response<CustomerModel>()
                {
                    Item = null,
                    ResultCode = GetErrorCode(ErrorCode.Error2025),
                    ResultDesc = Resource.Error2026
                };
            }
            else
            {
                registerModel.Email = registerModel.Email.Trim();
                if (registerModel.Email.Length>64)
                {
                    return new Response<CustomerModel>()
                    {
                        Item = null,
                        ResultCode = GetErrorCode(ErrorCode.Error2002),
                        ResultDesc = Resource.Error2002
                    };
                }
            }

            if (string.IsNullOrWhiteSpace(registerModel.Address))
            {
                return new Response<CustomerModel>()
                {
                    Item = null,
                    ResultCode = GetErrorCode(ErrorCode.Error2027),
                    ResultDesc = Resource.Error2027
                };
            }
            else
            {
                registerModel.Address = registerModel.Address.Trim();
                if (registerModel.Address.Length > 128)
                {
                    return new Response<CustomerModel>()
                    {




                        Item = null,
                        ResultCode = GetErrorCode(ErrorCode.Error2002),
                        ResultDesc = Resource.Error2002
                    };
                }
            }

            if (registerModel.Platform==null)
            {
                return new Response<CustomerModel>()
                {
                    Item = null,
                    ResultCode = GetErrorCode(ErrorCode.Error2028),
                    ResultDesc = Resource.Error2028
                };
            }


            //联系人电话 可以为空
            if (!string.IsNullOrWhiteSpace(registerModel.Mobile))
            {
                registerModel.Mobile = registerModel.Mobile.Trim();
                if (registerModel.Mobile.Length > 64)
                {
                    return new Response<CustomerModel>()
                    {
                        Item = null,
                        ResultCode = GetErrorCode(ErrorCode.Error2002),
                        ResultDesc = Resource.Error2002
                    };
                }

                customer.Mobile = registerModel.Mobile;
            }

            #endregion 



          
            CustomerBalance customerBalance = new CustomerBalance();
            if (_customerRepository.Count(p => p.AccountID == registerModel.AccountId) > 0)
            {
                return new Response<CustomerModel>()
                {
                    Item = null,
                    ResultCode = GetErrorCode(ErrorCode.Error2029),
                    ResultDesc = Resource.Error2029
                };
            }

            try
            {
                customer.CustomerCode = GenerateCustomerCode();//生成客户编码

                //根据客户编码生成ApiSecret
                ApiResult apiResult = GetSecret(customer.CustomerCode);
                string getApiKey = apiResult != null ? apiResult.ApiKey : "";
                string getApiSecret = apiResult != null ? apiResult.ApiSecret : "";
                customer.AccountID = registerModel.AccountId;
                customer.CustomerTypeID = GetCustomerTypeId("网站公开价");//网站公开价
                customer.PaymentTypeID = 4;//结算类型-现结
                customer.AccountPassWord = registerModel.AccountPassWord.ToMD5();
                customer.Status = 1;//默认未审核
                customer.Name = registerModel.Name;
                customer.LinkMan = registerModel.LinkMan;//联系人
                customer.Address = registerModel.Address;//地址
                customer.Email = registerModel.Email;
                customer.Tele = registerModel.Tele;
 
                customer.CustomerID = Guid.NewGuid();
                customer.ApiKey = getApiKey;
                customer.ApiSecret = getApiSecret;//获取密钥
                customer.CreatedOn = customer.LastUpdatedOn = DateTime.Now;
                customer.CreatedBy = customer.LastUpdatedBy = customer.AccountID;
                customer.EnableCredit = true;
                customer.MaxDelinquentAmounts = 2000;
                _customerRepository.Add(customer);
                  

                //用户来源平台
                _customerSourceInfoRepository.Add(new CustomerSourceInfo
                {
                    CustomerCode= customer.CustomerCode,
                    SourceType =registerModel.Platform.Value,
                    CreatedOn =System.DateTime.Now,
                    CreatedBy =CustomerCode,
                    LastUpdatedOn = System.DateTime.Now,
                    LastUpdatedBy = CustomerCode
                });


                //用户余额
                customerBalance.CustomerID = customer.CustomerID;
                customerBalance.CustomerCode = customer.CustomerCode.ToUpperInvariant().Trim();
                customerBalance.Balance = 0;
                customerBalance.CreatedOn = customer.CreatedOn;
                customerBalance.LastUpdatedOn = customer.CreatedOn;
                _customerBalanceRepository.Add(customerBalance);
                  

                using (var transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.MaxValue))
                {
                    _customerRepository.UnitOfWork.Commit();
                    _customerSourceInfoRepository.UnitOfWork.Commit();
                    _customerBalanceRepository.UnitOfWork.Commit();
                    transaction.Complete();
                }

               //成功
                return  new Response<CustomerModel>()
                {
                    Item =new CustomerModel
                    {
                        CustomerCode = customer.CustomerCode,
                        AccountId = customer.AccountID,
                        ApiSecret = getApiSecret
                    },
                    ResultCode = GetErrorCode(ErrorCode.Error0000),
                    ResultDesc = Resource.Error0000
                };

            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return new Response<CustomerModel>()
                {
                    Item = null,
                    ResultCode = GetErrorCode(ErrorCode.Error2019),
                    ResultDesc = Resource.Error2019
                };
            }

        }

        #region 注册用户需要的资料

        //生成该客户的ApiSecret
        private ApiResult GetSecret(string code)
        {
            string key = GetRandom(24);
            return new ApiResult
            {
               ApiKey =key,
               ApiSecret = SecurityUtil.EncryptDES(code, key)
            };
        }

        //生成字母和数字随机数
        private string GetRandom(int length)
        {
            char[] Pattern = new char[]
                {
                    '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K'
                    , 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
                };
            string result = "";
            int n = Pattern.Length;
            Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < length; i++)
            {
                int rnd = random.Next(0, n);
                result += Pattern[rnd];
            }
            return result;
        }

        //生成客户编码 
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

            //字母加上5位数字
            customerCode = "C" + customerCode;
            return _customerRepository.Exists(l => l.CustomerCode == customerCode) ? GenerateCustomerCode() : customerCode;
        }

        //获取客户类别id
        private int GetCustomerTypeId(string name)
        {
            CustomerType customerType=  _freightService.GetCustomerTypeList().First(a => a.CustomerTypeName == name);
            return customerType != null ? customerType.CustomerTypeId : 12;//12 是线上的网站公开价
        }


        #endregion

    }
}
