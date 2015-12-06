using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Serialization;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LMS.Services.FreightServices;
using LMS.Services.OrderServices;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Seedwork;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LMS.PostalAPI.Controllers
{
    public class PostalController : ApiController
    {
        private readonly IFreightService _freightService;
        private readonly IOrderService _orderService;
        

        public PostalController(IOrderService orderService, IFreightService freightService)
        {
            _freightService = freightService;
            _orderService = orderService;
        }

        #region 福州邮政API对接/Add By zhengsong/Time:2014-09-25

        //福州邮政API对接
        /// <summary>
        /// Add By zhengsong
        /// Time:2014-11-04
        /// 该方法已禁用
        /// </summary>
        [System.Web.Http.HttpGet]
        //public void AddPostalWayBill()
        //{
        //    //发货运输方式Code
        //    string[] shippingMethodCode = sysConfig.PostalDockingCode.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //    var shippingMethod = _freightService.GetShippingMethods("",true);
        //    List<int> shippingMethodIds=new List<int>();
        //    shippingMethod.ForEach(p =>
        //        {
        //            if (shippingMethodCode.Contains(p.Code))
        //            {
        //                shippingMethodIds.Add(p.ShippingMethodId);
        //            }
        //        });
        //    //获取要对接的运单
        //    List<ErrorWayBillExt> errorWayBills = new List<ErrorWayBillExt>();
        //    var wayBillNumbers = _orderService.GetFZWayBillNumbers(shippingMethodIds).ToList();
        //    int x = 0;
        //    string successWayBills = "申请成功的福州邮政运单:";
        //    List<string> wayBillNumberList = new List<string>();
        //    wayBillNumbers.ForEach(z =>
        //        {
        //            //每500个提交一次
        //            if (x != 0 && x % 500 == 0)
        //            {
        //                List<WayBillInfoModel> wayBillInfoModels = new List<WayBillInfoModel>();
        //                //wayBillInfoModels = _orderService.GetPostalWayBillInfo(shippingMethodIds,wayBillNumberList).ToModelAsCollection<FZWayBillInfoExt, WayBillInfoModel>();

        //                var wayBillInfoList = ValidateWayBillInfoModel(wayBillInfoModels);
                        
        //                //申请的订单数量
        //                string successWayBill = "";
        //                foreach (var wayBillInfo in wayBillInfoList)
        //                {
        //                    #region
        //                    var customerOrder = _orderService.GetCustomerOrderInfoById(wayBillInfo.CustomerOrderID);

        //                    //申报信息
        //                    List<ApplicationInfoModel> applicationInfoModels =
        //                        _orderService.GetApplicationInfoByWayBillNumber(wayBillInfo.WayBillNumber)
        //                                     .ToModelAsCollection<ApplicationInfo, ApplicationInfoModel>();
        //                    applicationInfoModels = applicationInfoModels.OrderBy(p => p.ApplicationID).ToList();
        //                    int n = 1;
        //                    decimal itotleweight = 0;
        //                    decimal itotlevalue = 0;
        //                    string ShippingName = wayBillInfo.ShippingInfo.ShippingFirstName +
        //                                  wayBillInfo.ShippingInfo.ShippingLastName;
        //                    string SenderName = wayBillInfo.SenderInfo.SenderFirstName +
        //                                wayBillInfo.SenderInfo.SenderLastName;
        //                    if (string.IsNullOrWhiteSpace(ShippingName))
        //                    {
        //                        ShippingName = "N/A";
        //                    }
        //                    if (string.IsNullOrWhiteSpace(SenderName))
        //                    {
        //                        SenderName = "N/A";
        //                    }

        //                    #endregion

        //                    #region 参数
        //                    string file =
        //                        "<logisticsEventsRequest><logisticsEvent><eventHeader><eventType>LOGISTICS_BATCH_SEND</eventType><eventTime>" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
        //                        "</eventTime><eventSource>ZONGTENG</eventSource><eventTarget>NPP</eventTarget></eventHeader><eventBody><orderInfos>";
        //                    //申报信息
        //                    //验证申报信息是否正确
        //                    var resultApplication = ValidateApplication(applicationInfoModels, wayBillInfo.WayBillNumber, wayBillInfo.OutShippingMethodID);
        //                    if (!resultApplication)
        //                    {
        //                        //跳出本次循环
        //                        continue;
        //                    }
        //                    applicationInfoModels.ForEach(p =>
        //                    {
        //                        file += "<product><productNameCN><![CDATA[" + p.PickingName
        //                                + "]]></productNameCN><productNameEN><![CDATA[" + p.ApplicationName
        //                                + "]]></productNameEN><productQantity>" + p.Qty
        //                                + "</productQantity><productCateCN><![CDATA[" + p.PickingName
        //                                + "]]></productCateCN><productCateEN><![CDATA[" + p.ApplicationName
        //                                + "]]></productCateEN><productId>" + n
        //                                + "</productId><producingArea>N/A</producingArea><productWeight>" +
        //                                ((long)(p.Qty * p.UnitWeight) * 1000)
        //                                + "</productWeight><productPrice>" + ((long)(p.UnitPrice * 100))
        //                                + "</productPrice></product>";
        //                        itotleweight += (p.Qty ?? 0) * (p.UnitWeight ?? 0) * 1000;
        //                        itotlevalue += (p.Qty ?? 0) * (p.UnitPrice ?? 0) * 100;
        //                        n++;
        //                    });
        //                    file += "</orderInfos><ecCompanyId>35010102179000</ecCompanyId><whCode>ZT-01</whCode><logisticsOrderId>" + wayBillInfo.CustomerOrderNumber
        //                            + "</logisticsOrderId><mailNo>" + wayBillInfo.TrackingNumber
        //                            + "</mailNo><Rcountry>" + wayBillInfo.CountryCode
        //                            + "</Rcountry><Rprovince><![CDATA[" + wayBillInfo.ShippingInfo.ShippingState
        //                            + "]]></Rprovince><Rcity><![CDATA[" + wayBillInfo.ShippingInfo.ShippingCity
        //                            + "]]></Rcity><Raddress><![CDATA[" + wayBillInfo.ShippingInfo.ShippingAddress +
        //                            wayBillInfo.ShippingInfo.ShippingAddress1 + wayBillInfo.ShippingInfo.ShippingAddress2
        //                            + "]]></Raddress><Rname><![CDATA[" + ShippingName
        //                            + "]]></Rname><Rphone>" + wayBillInfo.ShippingInfo.ShippingPhone
        //                            + "</Rphone><Sname><![CDATA[" + SenderName
        //                            + "]]></Sname><Sprovince><![CDATA[" + wayBillInfo.SenderInfo.SenderState
        //                            + "]]></Sprovince><Scity><![CDATA[" + wayBillInfo.SenderInfo.SenderCity
        //                            + "]]></Scity><Saddress><![CDATA[" + wayBillInfo.SenderInfo.SenderAddress
        //                            + "]]></Saddress><Sphone>" + wayBillInfo.SenderInfo.SenderPhone
        //                            + "</Sphone><Spostcode>" + wayBillInfo.SenderInfo.SenderZip;
        //                    switch (wayBillInfo.InsuredID)
        //                    {
        //                        case 1:
        //                            file += "</Spostcode><insuranceValue>" + 600;
        //                            break;
        //                        case 2:
        //                            if (customerOrder != null)
        //                            {
        //                                file += "</Spostcode><insuranceValue>" + (long)((customerOrder.InsureAmount ?? 0) * 100);
        //                            }
        //                            else
        //                            {
        //                                file += "</Spostcode><insuranceValue>0";
        //                            }
        //                            break;
        //                        case 3:
        //                            file += "</Spostcode><insuranceValue>" + 800;
        //                            break;
        //                        default:
        //                            file += "</Spostcode><insuranceValue>0";
        //                            break;
        //                    }

        //                    bool hasBattery = false;
        //                    if (customerOrder.IsBattery)
        //                    {
        //                        if (customerOrder.SensitiveTypeID == (int)CustomerOrder.SensitiveTypeInfosTypeEnum.纯电池 ||
        //                            customerOrder.SensitiveTypeID == (int)CustomerOrder.SensitiveTypeInfosTypeEnum.钮扣电池)
        //                        {
        //                            hasBattery = true;
        //                        }
        //                    }

        //                    file += "</insuranceValue><Itotleweight>" + (long)itotleweight
        //                            + "</Itotleweight><Itotlevalue>" + (long)itotlevalue
        //                            + "</Itotlevalue><totleweight>" + (long)(wayBillInfo.SettleWeight ?? 0) * 1000
        //                            + "</totleweight><hasBattery>" + hasBattery
        //                            + "</hasBattery><country>CN</country><mailKind>" + customerOrder.AppLicationType.ToString()
        //                            + "</mailKind><mailClass>L</mailClass><batchNo>" + wayBillInfo.OutStorageID
        //                            + "</batchNo><mailType>ZONGTENG</mailType><faceType>1</faceType>";
        //                    if (customerOrder != null && customerOrder.IsReturn)
        //                    {
        //                        file += "<undeliveryOption>2</undeliveryOption>";
        //                    }
        //                    else
        //                    {
        //                        file += "<undeliveryOption>1</undeliveryOption>";
        //                    }
        //                    file += "</eventBody></logisticsEvent></logisticsEventsRequest>";

        //                    #endregion
        //                    //请求参数
        //                    string request = GetPostalWayBill(file);
        //                    //请求福州邮政对接接口
        //                    var result = _freightService.AddWayBillZONGTENG(request);
        //                    #region 提交
        //                    if (result.Result)
        //                    {
        //                        //记录成功信息
        //                        ErrorWayBillExt errorWayBill = new ErrorWayBillExt();
        //                        errorWayBill.WayBillNumber = wayBillInfo.WayBillNumber;
        //                        errorWayBill.ErrorMassge = "";
        //                        errorWayBill.result = result.Result;
        //                        errorWayBill.outShippingMethodID = wayBillInfo.OutShippingMethodID;
        //                        errorWayBills.Add(errorWayBill);
        //                        successWayBill += "[" + wayBillInfo.WayBillNumber + "]";
        //                    }
        //                    else
        //                    {
        //                        string errorMessage = "";
        //                        switch (result.Message)
        //                        {
        //                            case "S01":
        //                                errorMessage = "非法的XML/JSON";
        //                                break;
        //                            case "S02":
        //                                errorMessage = "非法的数字签名";
        //                                break;
        //                            case "S03":
        //                                errorMessage = "非法的物流公司/仓储公司";
        //                                break;
        //                            case "S04":
        //                                errorMessage = "非法的通知类型";
        //                                break;
        //                            case "S05":
        //                                errorMessage = "非法的通知内空";
        //                                break;
        //                            case "S06":
        //                                errorMessage = "网络超时，请重试";
        //                                break;
        //                            case "S07":
        //                                errorMessage = "系统异常，请重试";
        //                                break;
        //                            case "S08":
        //                                errorMessage = "HTTP状态异常（非200）";
        //                                break;
        //                            case "S09":
        //                                errorMessage = "返回报文为空";
        //                                break;
        //                            case "S10":
        //                                errorMessage = "找不到对应的网关信息";
        //                                break;
        //                            case "S11":
        //                                errorMessage = "非法的网关信息";
        //                                break;
        //                            case "S12":
        //                                errorMessage = "非法的请求参数";
        //                                break;
        //                            case "S13":
        //                                errorMessage = "业务服务异常";
        //                                break;
        //                            case "B00":
        //                                errorMessage = "未知业务错误";
        //                                break;
        //                            case "B01":
        //                                errorMessage = "关键字段缺失";
        //                                break;
        //                            case "B02":
        //                                errorMessage = "关键数据格式不正确";
        //                                break;
        //                            case "B03":
        //                                errorMessage = "没有找到请求的数据";
        //                                break;
        //                            case "B04":
        //                                errorMessage = "当前数据状态不能进行该项操作";
        //                                break;
        //                            case "B98":
        //                                errorMessage = "数据保存失败";
        //                                break;
        //                            default:
        //                                errorMessage = result.Message;
        //                                break;
        //                        }
        //                        //记录失败运单信息
        //                        ErrorWayBillExt errorWayBill = new ErrorWayBillExt();
        //                        errorWayBill.WayBillNumber = wayBillInfo.WayBillNumber;
        //                        errorWayBill.ErrorMassge = errorMessage;
        //                        errorWayBill.result = result.Result;
        //                        errorWayBill.outShippingMethodID = wayBillInfo.OutShippingMethodID;
        //                        errorWayBills.Add(errorWayBill);
        //                    }
        //                    #endregion
        //                }
        //                successWayBills += successWayBill;
        //                wayBillNumberList.Clear();
        //            }
        //            else
        //            {
        //                wayBillNumberList.Add(z.WayBillNumber); 
        //            }
        //            x++;
        //        });
        //    #region 循环后剩下的运单
        //    if (wayBillNumberList.Count > 0)
        //    {
        //        List<WayBillInfoModel> wayBillInfoModels = new List<WayBillInfoModel>();
        //        wayBillInfoModels = _orderService.GetPostalWayBillInfo(shippingMethodIds, wayBillNumberList).ToModelAsCollection<FZWayBillInfoExt, WayBillInfoModel>();

        //        var wayBillInfoList = ValidateWayBillInfoModel(wayBillInfoModels);

        //        //申请的订单数量
        //        string successWayBill = "";
        //        foreach (var wayBillInfo in wayBillInfoList)
        //        {
        //            #region
        //            var customerOrder = _orderService.GetCustomerOrderInfoById(wayBillInfo.CustomerOrderID);

        //            //申报信息
        //            List<ApplicationInfoModel> applicationInfoModels =
        //                _orderService.GetApplicationInfoByWayBillNumber(wayBillInfo.WayBillNumber)
        //                             .ToModelAsCollection<ApplicationInfo, ApplicationInfoModel>();
        //            applicationInfoModels = applicationInfoModels.OrderBy(p => p.ApplicationID).ToList();
        //            int n = 1;
        //            decimal itotleweight = 0;
        //            decimal itotlevalue = 0;

        //            string ShippingName = wayBillInfo.ShippingInfo.ShippingFirstName +
        //                                  wayBillInfo.ShippingInfo.ShippingLastName;
        //            string SenderName = wayBillInfo.SenderInfo.SenderFirstName +
        //                        wayBillInfo.SenderInfo.SenderLastName;
        //            if (string.IsNullOrWhiteSpace(ShippingName))
        //            {
        //                ShippingName = "N/A";
        //            }
        //            if (string.IsNullOrWhiteSpace(SenderName))
        //            {
        //                SenderName = "N/A";
        //            }

        //            #endregion

        //            #region 参数
        //            string file =
        //                "<logisticsEventsRequest><logisticsEvent><eventHeader><eventType>LOGISTICS_BATCH_SEND</eventType><eventTime>" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
        //                "</eventTime><eventSource>ZONGTENG</eventSource><eventTarget>NPP</eventTarget></eventHeader><eventBody><orderInfos>";
        //            //申报信息
        //            //验证申报信息是否正确
        //            var resultApplication = ValidateApplication(applicationInfoModels, wayBillInfo.WayBillNumber, wayBillInfo.OutShippingMethodID);
        //            if (!resultApplication)
        //            {
        //                //跳出本次循环
        //                continue;
        //            }
        //            applicationInfoModels.ForEach(p =>
        //            {
        //                file += "<product><productNameCN><![CDATA[" + p.PickingName
        //                        + "]]></productNameCN><productNameEN><![CDATA[" + p.ApplicationName
        //                        + "]]></productNameEN><productQantity>" + p.Qty
        //                        + "</productQantity><productCateCN><![CDATA[" + p.PickingName
        //                        + "]]></productCateCN><productCateEN><![CDATA[" + p.ApplicationName
        //                        + "]]></productCateEN><productId>" + n
        //                        + "</productId><producingArea>N/A</producingArea><productWeight>" +
        //                        ((long)(p.Qty * p.UnitWeight) * 1000)
        //                        + "</productWeight><productPrice>" + ((long)(p.UnitPrice * 100))
        //                        + "</productPrice></product>";
        //                itotleweight += (p.Qty ?? 0) * (p.UnitWeight ?? 0) * 1000;
        //                itotlevalue += (p.Qty ?? 0) * (p.UnitPrice ?? 0) * 100;
        //                n++;
        //            });
        //            file += "</orderInfos><ecCompanyId>35010102179000</ecCompanyId><whCode>ZT-01</whCode><logisticsOrderId>" + wayBillInfo.CustomerOrderNumber
        //                    + "</logisticsOrderId><mailNo>" + wayBillInfo.TrackingNumber
        //                    + "</mailNo><Rcountry>" + wayBillInfo.CountryCode
        //                    + "</Rcountry><Rprovince><![CDATA[" + wayBillInfo.ShippingInfo.ShippingState
        //                    + "]]></Rprovince><Rcity><![CDATA[" + wayBillInfo.ShippingInfo.ShippingCity
        //                    + "]]></Rcity><Raddress><![CDATA[" + wayBillInfo.ShippingInfo.ShippingAddress +
        //                    wayBillInfo.ShippingInfo.ShippingAddress1 + wayBillInfo.ShippingInfo.ShippingAddress2
        //                    + "]]></Raddress><Rname><![CDATA[" + ShippingName
        //                    + "]]></Rname><Rphone>" + wayBillInfo.ShippingInfo.ShippingPhone
        //                    + "</Rphone><Sname><![CDATA[" + SenderName
        //                    + "]]></Sname><Sprovince><![CDATA[" + wayBillInfo.SenderInfo.SenderState
        //                    + "]]></Sprovince><Scity><![CDATA[" + wayBillInfo.SenderInfo.SenderCity
        //                    + "]]></Scity><Saddress><![CDATA[" + wayBillInfo.SenderInfo.SenderAddress
        //                    + "]]></Saddress><Sphone>" + wayBillInfo.SenderInfo.SenderPhone
        //                    + "</Sphone><Spostcode>" + wayBillInfo.SenderInfo.SenderZip;
        //            switch (wayBillInfo.InsuredID)
        //            {
        //                case 1:
        //                    file += "</Spostcode><insuranceValue>" + 600;
        //                    break;
        //                case 2:
        //                    if (customerOrder != null)
        //                    {
        //                        file += "</Spostcode><insuranceValue>" + (long)((customerOrder.InsureAmount ?? 0) * 100);
        //                    }
        //                    else
        //                    {
        //                        file += "</Spostcode><insuranceValue>0";
        //                    }
        //                    break;
        //                case 3:
        //                    file += "</Spostcode><insuranceValue>" + 800;
        //                    break;
        //                default:
        //                    file += "</Spostcode><insuranceValue>0";
        //                    break;
        //            }

        //            bool hasBattery = false;
        //            if (customerOrder.IsBattery)
        //            {
        //                if (customerOrder.SensitiveTypeID == (int)CustomerOrder.SensitiveTypeInfosTypeEnum.纯电池 ||
        //                    customerOrder.SensitiveTypeID == (int)CustomerOrder.SensitiveTypeInfosTypeEnum.钮扣电池)
        //                {
        //                    hasBattery = true;
        //                }
        //            }

        //            file += "</insuranceValue><Itotleweight>" + (long)itotleweight
        //                    + "</Itotleweight><Itotlevalue>" + (long)itotlevalue
        //                    + "</Itotlevalue><totleweight>" + (long)(wayBillInfo.SettleWeight ?? 0) * 1000
        //                    + "</totleweight><hasBattery>" + hasBattery
        //                    + "</hasBattery><country>CN</country><mailKind>" + customerOrder.AppLicationType.ToString()
        //                    + "</mailKind><mailClass>L</mailClass><batchNo>" + wayBillInfo.OutStorageID
        //                    + "</batchNo><mailType>ZONGTENG</mailType><faceType>1</faceType>";
        //            if (customerOrder != null && customerOrder.IsReturn)
        //            {
        //                file += "<undeliveryOption>2</undeliveryOption>";
        //            }
        //            else
        //            {
        //                file += "<undeliveryOption>1</undeliveryOption>";
        //            }
        //            file += "</eventBody></logisticsEvent></logisticsEventsRequest>";

        //            #endregion
        //            //请求参数
        //            string request = GetPostalWayBill(file);
        //            //请求福州邮政对接接口
        //            var result = _freightService.AddWayBillZONGTENG(request);

        //            #region 提交

        //            if (result.Result)
        //            {
        //                //记录成功信息
        //                //记录失败运单信息
        //                ErrorWayBillExt errorWayBill = new ErrorWayBillExt();
        //                errorWayBill.WayBillNumber = wayBillInfo.WayBillNumber;
        //                errorWayBill.ErrorMassge = "";
        //                errorWayBill.result = result.Result;
        //                errorWayBill.outShippingMethodID = wayBillInfo.OutShippingMethodID;
        //                errorWayBills.Add(errorWayBill);
        //                successWayBill += "[" + wayBillInfo.WayBillNumber + "]";
        //            }
        //            else
        //            {
        //                string errorMessage = "";
        //                switch (result.Message)
        //                {
        //                    case "S01":
        //                        errorMessage = "非法的XML/JSON";
        //                        break;
        //                    case "S02":
        //                        errorMessage = "非法的数字签名";
        //                        break;
        //                    case "S03":
        //                        errorMessage = "非法的物流公司/仓储公司";
        //                        break;
        //                    case "S04":
        //                        errorMessage = "非法的通知类型";
        //                        break;
        //                    case "S05":
        //                        errorMessage = "非法的通知内空";
        //                        break;
        //                    case "S06":
        //                        errorMessage = "网络超时，请重试";
        //                        break;
        //                    case "S07":
        //                        errorMessage = "系统异常，请重试";
        //                        break;
        //                    case "S08":
        //                        errorMessage = "HTTP状态异常（非200）";
        //                        break;
        //                    case "S09":
        //                        errorMessage = "返回报文为空";
        //                        break;
        //                    case "S10":
        //                        errorMessage = "找不到对应的网关信息";
        //                        break;
        //                    case "S11":
        //                        errorMessage = "非法的网关信息";
        //                        break;
        //                    case "S12":
        //                        errorMessage = "非法的请求参数";
        //                        break;
        //                    case "S13":
        //                        errorMessage = "业务服务异常";
        //                        break;
        //                    case "B00":
        //                        errorMessage = "未知业务错误";
        //                        break;
        //                    case "B01":
        //                        errorMessage = "关键字段缺失";
        //                        break;
        //                    case "B02":
        //                        errorMessage = "关键数据格式不正确";
        //                        break;
        //                    case "B03":
        //                        errorMessage = "没有找到请求的数据";
        //                        break;
        //                    case "B04":
        //                        errorMessage = "当前数据状态不能进行该项操作";
        //                        break;
        //                    case "B98":
        //                        errorMessage = "数据保存失败";
        //                        break;
        //                    default:
        //                        errorMessage = result.Message;
        //                        break;
        //                }
        //                //记录失败运单信息
        //                ErrorWayBillExt errorWayBill = new ErrorWayBillExt();
        //                errorWayBill.WayBillNumber = wayBillInfo.WayBillNumber;
        //                errorWayBill.ErrorMassge = errorMessage;
        //                errorWayBill.result = result.Result;
        //                errorWayBill.outShippingMethodID = wayBillInfo.OutShippingMethodID;
        //                errorWayBills.Add(errorWayBill);
        //            }
        //            #endregion
        //        }
        //        successWayBills += successWayBill;
        //        _orderService.AddorUpdateFuzhouPostLog(errorWayBills);
        //    }
        //    #endregion
        //    Log.Info("申请的数量" + wayBillNumbers.Count + "|成功的运单:" + successWayBills);
        //}

        /// <summary>
        /// 验证数据的正确性
        /// Add By zhengsong
        /// Time:2014-11-04
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public List<WayBillInfoModel> ValidateWayBillInfoModel(List<WayBillInfoModel> models)
        {
            List<WayBillInfoModel> wayBillInfoModels=new List<WayBillInfoModel>();
            List<ErrorWayBillExt> errorWayBills = new List<ErrorWayBillExt>();
            string allWayBill = "";
            models.ForEach(p =>
                {
                    allWayBill += "["+p.WayBillNumber+"]";
                    string ErrorInfo = "";
                    var customerOrder = _orderService.GetCustomerOrderInfoById(p.CustomerOrderID);

                    if (string.IsNullOrWhiteSpace(p.SenderInfo.SenderState) || p.SenderInfo.SenderState.Length>64)
                    {
                        p.SenderInfo.SenderState = "福州";
                    }
                    if (string.IsNullOrWhiteSpace(p.SenderInfo.SenderAddress) || p.SenderInfo.SenderAddress.Length>512)
                    {
                        p.SenderInfo.SenderAddress = "N/A";
                    }
                    if (string.IsNullOrWhiteSpace(p.SenderInfo.SenderPhone) || p.SenderInfo.SenderPhone.Length>64)
                    {
                        p.SenderInfo.SenderPhone = "N/A";
                    }

                    if (string.IsNullOrWhiteSpace(p.ShippingInfo.ShippingState) || p.ShippingInfo.ShippingState.Length>64)
                    {
                        p.ShippingInfo.ShippingState = "N/A";
                    }

                    if (string.IsNullOrWhiteSpace(p.ShippingInfo.ShippingZip) || p.ShippingInfo.ShippingZip.Length>32)
                    {
                        p.ShippingInfo.ShippingZip = "N/A";
                    }

                    if (string.IsNullOrWhiteSpace(p.ShippingInfo.ShippingPhone) || p.ShippingInfo.ShippingPhone.Length>64)
                    {
                        p.ShippingInfo.ShippingPhone = "N/A";
                    }

                    if (string.IsNullOrWhiteSpace(customerOrder.CustomerCode) || customerOrder.CustomerCode.Length > 14)
                    {
                        ErrorInfo += "客户代码为空或超长,";
                    }
                    if (string.IsNullOrWhiteSpace(p.CustomerOrderNumber) || p.CustomerOrderNumber.Length > 30)
                    {
                        ErrorInfo += "订单号为空或超长,";
                    }
                    if (string.IsNullOrWhiteSpace(p.TrackingNumber) || p.TrackingNumber.Length > 22)
                    {
                        ErrorInfo += "跟踪号为空或超长,";
                    }
                    if (string.IsNullOrWhiteSpace(p.CountryCode) || p.CountryCode.Length > 32)
                    {
                        ErrorInfo += "收件人国家为空或超长,";
                    }
                    if (string.IsNullOrWhiteSpace(p.ShippingInfo.ShippingCity) || p.ShippingInfo.ShippingCity.Length > 64)
                    {
                        ErrorInfo += "收件人城市为空或超长,";
                    }
                    if (string.IsNullOrWhiteSpace(p.SenderInfo.SenderZip) || p.SenderInfo.SenderZip.Length > 32)
                    {
                        p.SenderInfo.SenderZip = "350000";
                    }
                    if (string.IsNullOrWhiteSpace(p.SenderInfo.SenderCity) || p.SenderInfo.SenderCity.Length > 64)
                    {
                        ErrorInfo += "发件人城市不存或超长";
                    }
                    if (p.SettleWeight == null)
                    {
                        ErrorInfo += "重量为空,";
                    }
                    if (customerOrder.AppLicationType != 1 && customerOrder.AppLicationType != 2 &&
                        customerOrder.AppLicationType != 3 && customerOrder.AppLicationType != 4)
                    {
                        ErrorInfo += "申报类型错误,";
                    }
                    if (p.OutStorageID == null)
                    {
                        ErrorInfo += "出库批次号为空";
                    }
                    if (string.IsNullOrWhiteSpace(ErrorInfo))
                    {
                        wayBillInfoModels.Add(p);
                    }
                    else
                    {
                        //记录失败运单信息
                        ErrorWayBillExt errorWayBill = new ErrorWayBillExt();
                        errorWayBill.WayBillNumber = p.WayBillNumber;
                        errorWayBill.ErrorMassge = ErrorInfo;
                        errorWayBill.result = false;
                        errorWayBill.outShippingMethodID = p.OutShippingMethodID;
                        errorWayBills.Add(errorWayBill);
                    }
                });

            _orderService.AddorUpdateFuzhouPostLog(errorWayBills);
            Log.Info("当前批次提取的福州邮政订单："+allWayBill);
            return wayBillInfoModels;
        }

        //验证申报信息
        public bool ValidateApplication(List<ApplicationInfoModel> applicationInfoModels, string WayBillNumber, int? OutShippingMethodID)
        {
            bool result = true;
            string ErrorMassge = "";
            int n = 1;
            List<ErrorWayBillExt> errorWayBills=new List<ErrorWayBillExt>();
            applicationInfoModels.ForEach(p =>
                {
                    if (string.IsNullOrWhiteSpace(p.PickingName) || p.PickingName.Length > 60)
                    {
                        ErrorMassge += "申报信息" + n + "中文名为空或者长度超过60个字符,";
                        result = false;
                    }
                    if (string.IsNullOrWhiteSpace(p.ApplicationName) || p.ApplicationName.Length > 60)
                    {
                        ErrorMassge += "申报信息" + n + "英文文名为空或者长度超过60个字符,";
                        result = false;
                    }
                    if (p.Qty == null)
                    {
                        ErrorMassge += "申报" + n + "件数为空";
                        result = false;
                    }
                    n++;
                });
            //增加或修改福州邮政申请信息
            if (!result)
            {
                //记录失败运单信息
                ErrorWayBillExt errorWayBill = new ErrorWayBillExt();
                errorWayBill.WayBillNumber = WayBillNumber;
                errorWayBill.ErrorMassge = ErrorMassge;
                errorWayBill.result = false;
                errorWayBill.outShippingMethodID = OutShippingMethodID;
                errorWayBills.Add(errorWayBill);
            }
            _orderService.AddorUpdateFuzhouPostLog(errorWayBills);
            return result;
        }

        private string Encode(string input,bool isCdata=false)
        {
            if (isCdata)
            {
                return string.Format("<![CDATA[{0}]]>", HttpUtility.UrlEncode(input));
            }
            else
            {
                return HttpUtility.UrlEncode(input);
            }
        }

        //参数序列化
        public string GetPostalWayBill(string files)
        {            
            //encode
            var xmlEnCode = Encode(files);

            //加上密钥
            var str = files + sysConfig.Postalkey;
            byte[] result = Encoding.UTF8.GetBytes(str);

            //MD5加密
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
           
            //string postalWayBillMD5 = GetMD5(str);
            //System.Text.UTF8Encoding utf8=new UTF8Encoding();
            //var bytes = utf8.GetBytes(postalWayBillMD5);

            //Base64加密
            string postalWayBillBase64 = GEtBase64(output);

            var aa = Encode(postalWayBillBase64);

            string file = "logistics_interface=" + xmlEnCode + "&data_digest=" + aa + "&msg_type=LOGISTICS_BATCH_SEND";
           
            return file;
        }

        //private string GetMD5(string str)
        //{
        //    var md5 = FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
        //    return md5;
        //}
        public string GEtBase64(byte[] aa)
        {
            string base64 = Convert.ToBase64String(aa);
            return base64;
        }

        //运单信息
        #endregion
    }

  
    //[XmlRoot(ElementName = "logisticsEventsRequest")]
    //public class LogisticsEventsRequest
    //{
    //    public LogisticsEventsRequest()
    //    {
    //        LogisticsEvent=new LogisticsEvent();
    //    }

    //    [XmlElement(ElementName = "logisticsEvent")]
    //    public LogisticsEvent LogisticsEvent { get; set; }


    //}

    //public class LogisticsEvent
    //{
    //    public LogisticsEvent()
    //    {
    //        EventHeader=new EventHeader();
    //        PostalWayBillModel=new PostalWayBillModel();
    //    }

    //    [XmlElement(ElementName = "eventHeader")]
    //    public EventHeader EventHeader { get; set; }
    //    [XmlElement(ElementName = "eventBody")]
    //    public PostalWayBillModel PostalWayBillModel { get; set; }

    //}
    ////头部
    //public class EventHeader
    //{
    //    [XmlElement(ElementName = "eventType")]
    //    public string eventType { get; set; }

    //    [XmlElement(ElementName = "eventTime")]
    //    public string eventTime { get; set; }

    //    [XmlElement(ElementName = "eventSource")]
    //    public string eventSource { get; set; }

    //    [XmlElement(ElementName = "eventTarget")]
    //    public string eventTarget { get; set; }
    //}


    ////运单信息
    //public class PostalWayBillModel
    //{
    //    public PostalWayBillModel()
    //    {
    //        OrderInfos = new OrderInfos();
    //    }

    //    [XmlElement(ElementName = "OrderInfos")]
    //    public OrderInfos OrderInfos { get; set; }

    //    [XmlElement(ElementName = "ecCompanyId")]
    //    public string CustomerCode { get; set; }

    //    [XmlElement(ElementName = "whCode")]
    //    public string whCode { get; set; }

    //    [XmlElement(ElementName = "logisticsOrderId")]
    //    public string CustomerOrderNumber { get; set; }

    //    [XmlElement(ElementName = "mailNo")]
    //    public string TrackingNumber { get; set; }

    //    [XmlElement(ElementName = "Rcountry")]
    //    public string CountryCode { get; set; }

    //    [XmlElement(ElementName = "Rprovince")]
    //    public string ShippingState { get; set; }

    //    [XmlElement(ElementName = "Rcity")]
    //    public string ShippingCity { get; set; }

    //    [XmlElement(ElementName = "Raddress")]
    //    public string ShippingAddress { get; set; }

    //    [XmlElement(ElementName = "Rpostcode")]
    //    public string ShippingZip { get; set; }

    //    [XmlElement(ElementName = "Rname")]
    //    public string ShippingName { get; set; }

    //    [XmlElement(ElementName = "Rphone")]
    //    public string ShippingPhone { get; set; }

    //    [XmlElement(ElementName = "Sname")]
    //    public string SenderName { get; set; }

    //    [XmlElement(ElementName = "Sprovince")]
    //    public string SenderState { get; set; }

    //    [XmlElement(ElementName = "Scity")]
    //    public string SenderCity { get; set; }

    //    [XmlElement(ElementName = "Saddress")]
    //    public string SenderAddress { get; set; }

    //    [XmlElement(ElementName = "Sphone")]
    //    public string SenderPhone { get; set; }

    //    [XmlElement(ElementName = "Spostcode")]
    //    public string SenderZip { get; set; }

    //    [XmlElement(ElementName = "insuranceValue")]
    //    public long InsuredMoney { get; set; }
        
    //    [XmlElement(ElementName = "Itotleweight")]
    //    public long Itotleweight { get; set; }

    //    [XmlElement(ElementName = "Itotlevalue")]
    //    public long Itotlevalue { get; set; }

    //    [XmlElement(ElementName = "totleweight")]
    //    public long SettleWeight { get; set; }

    //    [XmlElement(ElementName = "hasBattery")]
    //    public bool hasBattery { get; set; }

    //    [XmlElement(ElementName = "country")]
    //    public string Country { get; set; }

    //    [XmlElement(ElementName = "mailKind")]
    //    public string AppLicationType { get; set; }

    //    [XmlElement(ElementName = "mailClass")]
    //    public string MailClass { get; set; }

    //    [XmlElement(ElementName = "batchNo")]
    //    public string OutStorageID { get; set; }

    //    [XmlElement(ElementName = "mailType")]
    //    public string MailType { get; set; }

    //    [XmlElement(ElementName = "faceType")]
    //    public int FaceType { get; set; }

    //    [XmlElement(ElementName = "undeliveryOption")]
    //    public int undeliveryOption { get; set; }

    //}


    ////申报信息
    //public class OrderInfos
    //{
    //    public OrderInfos()
    //    {
    //        ApplicationModelList=new List<ApplicationModel>();
    //    }

    //    [XmlElement(ElementName = "product")]
    //    public List<ApplicationModel> ApplicationModelList { get; set; }

    //}

    ////申报信息
    //public class ApplicationModel
    //{
    //    [XmlElement(ElementName = "productNameCN")]
    //    public string PickingName { get; set; }

    //    [XmlElement(ElementName = "productNameEN")]
    //    public string ApplicationName { get; set; }

    //    [XmlElement(ElementName = "productQantity")]
    //    public int Qty { get; set; }

    //    [XmlElement(ElementName = "productCateCN")]
    //    public string NextPickingName { get; set; }

    //    [XmlElement(ElementName = "productCateEN")]
    //    public string NextApplicationName { get; set; }

    //    [XmlElement(ElementName = "productId")]
    //    public string Number { get; set; }

    //    [XmlElement(ElementName = "producingArea")]
    //    public string ProducingArea { get; set; }

    //    [XmlElement(ElementName = "productWeight")]
    //    public long TotalWeight { get; set; }

    //    [XmlElement(ElementName = "productPrice")]
    //    public long UnitPrice { get; set; }

    //}

    public class WayBillInfoModel
    {
        public WayBillInfoModel()
        {
            ShippingInfo = new ShippingInfoModel();
            SenderInfo = new SenderInfoModel();
        }
        public string WayBillNumber { get; set; }
        public string CustomerOrderNumber { get; set; }
        public int? CustomerOrderID { get; set; }
        public string TrackingNumber { get; set; }

        public decimal? Weight { get; set; }
        public decimal? SettleWeight { get; set; }
        public bool IsReturn { get; set; }
        public bool IsBattery { get; set; }
        public int Status { get; set; }
        public string OutStorageID { get; set; }
        public DateTime? OutStorageCreatedOn { get; set; }
        public string CountryCode { get; set; }
        public int? InsuredID { get; set; }
        public int? OutShippingMethodID { get; set; }
        public ShippingInfoModel ShippingInfo { get; set; }
        public SenderInfoModel SenderInfo { get; set; }
       
    }

    public class ShippingInfoModel
    {
        public string CountryCode { get; set; }
        public string ShippingFirstName { get; set; }
        public string ShippingLastName { get; set; }
        public string ShippingCompany { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingAddress1 { get; set; }
        public string ShippingAddress2 { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZip { get; set; }
        public string ShippingPhone { get; set; }
        public string ShippingTaxId { get; set; }
    }

    public class SenderInfoModel
    {
        public string CountryCode { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderCompany { get; set; }
        public string SenderAddress { get; set; }
        public string SenderCity { get; set; }
        public string SenderState { get; set; }
        public string SenderZip { get; set; }
        public string SenderPhone { get; set; }
    }

    public class ApplicationInfoModel
    {
        public int ApplicationID { get; set; }
        public string ApplicationName { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<decimal> UnitWeight { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public string PickingName { get; set; }
        public string Remark { get; set; }
    }
}