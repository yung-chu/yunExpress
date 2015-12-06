using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using FluentValidation;
using FluentValidation.Results;
using LMS.Services.SF.Model;
using LMS.Services.SF.References.SfCommon;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Services.SF
{
    public class LMSSFCommon
    {
        private static readonly ServiceClient SfServiceClient=new ServiceClient();
        public static string SfCommonService(string xml)
        {
            Log.Info(xml);
            return SfServiceClient.sfexpressService(xml);
        }
        public static SfResponse SubmitSf(OrderSfModel model, string authorization)
        {
            var response = new SfResponse();
            if (model == null)
            {
                response.ErrorMsg="对象为空";
                return response;
            }
            if (string.IsNullOrWhiteSpace(authorization))
            {
                response.ErrorMsg = "账号信息错误";
                return response;
            }
            if (!model.Applications.Any())
            {
                response.ErrorMsg = "申报信息不能为空";
                return response;
            }
            
            foreach (var app in model.Applications)
            {
                AbstractValidator<ApplicationSfModel> appValidator = new ApplicationSfModelValidator();
                ValidationResult results=appValidator.Validate(app);
                if (!results.IsValid)
                {
                    response.ErrorMsg+= string.Join(Environment.NewLine, results.Errors.Select(p => p.ErrorMessage));
                }
            }
            if (!response.ErrorMsg.IsNullOrWhiteSpace())
            {
                return response;
            }
            AbstractValidator<OrderSfModel> oValidator=new OrderSfModelValidator();
            ValidationResult result = oValidator.Validate(model);
            if (!result.IsValid)
            {
                response.ErrorMsg += string.Join(Environment.NewLine, result.Errors.Select(p => p.ErrorMessage));
                return response;
            }
            return PlaceTheOrder(model, authorization);
        }
        /// <summary>
        /// 提交到顺丰
        /// </summary>
        /// <param name="model"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
        public static SfResponse PlaceTheOrder(OrderSfModel model, string authorization)
        {
            var response = new SfResponse();
            string strXML = @"<Request service='OrderService' lang='en'>
                                         <Head>"+authorization+@"</Head>
                                         <Body>
                                             <Order orderid='" + model.OrderId.Trim() + @"' 
                                                             express_type='" + model.ExpressType + @"' 
                                                             j_province='Guangdong province' 
                                                             j_city='Shenzhen' 
                                                             j_county='Longgang District' 
                                                             j_company='Lightake' 
                                                             j_contact='cadi'
                                                             j_address='Lightake 5 B102 Jinguanghua Logistics Park 49 Wuhe road south Wuhe community Bantian street,Longgang District,Shenzhen,China'
                                                             j_post_code='518100' 
                                                             j_country='CN'
                                                             j_tel='13923866547'  
                                                             d_contact='" + (model.ShippingName).Trim().ToDBC().StripXML() + @"'
                                                             d_company='" + (model.ShippingCompany??model.ShippingName).Trim().ToDBC().StripXML() + @"'  
                                                             d_tel='" + (model.ShippingTel.GetNumber() == "" ? "-" : model.ShippingTel.GetNumber()) + @"'
                                                             d_mobile='" + (model.ShippingPhone.GetNumber() == "" ? "-" : model.ShippingPhone.GetNumber()) + @"'
                                                             d_address='" +  model.ShippingAddress.ToDBC().StripXML() + @"' 
                                                             parcel_quantity='"+model.ParcelQuantity+@"' 
                                                             pay_method='1'
                                                             d_province='" + (model.ShippingState ?? "").Trim().ToDBC().StripXML() + @"'
                                                             d_city='" + (model.ShippingCity ?? "-").Trim().ToDBC().StripXML() + @"'  
                                                             declared_value='" + model.ApplicationTotalPrice.ToString("F3") + @"' 
                                                             declared_value_currency ='USD' 
                                                             custid ='7555291632'
                                                             d_country='" + model.CountryCode.Trim() + @"'
                                                             d_deliverycode='MOW' 
                                                             d_post_code='" + model.ShippingZip + @"'
                                                             cargo_total_weight='" + model.ApplicationTotalWeight.ToString("F3") + @"'
                                                             remark='" + model.Remark + @"'
                                                            >
                                             {0}
                                         </Body>
                                    </Request>";
            string application = string.Empty;
            foreach (var app in model.Applications)
            {
                application += "<Cargo name='" + app.ApplicationName.Trim().ToDBC().StripXML() + "' count='" + app.Qty +
                               "'  unit='U' weight='" + app.UnitWeight.ToString("F3") + "'  amount='" +
                               app.UnitPrice.ToString("F2") + @"'  currency='USD'  source_area='China'/>  </Order>";
            }
            string responseResult = SfCommonService(strXML.FormatWith(application));
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(responseResult);
            XmlNode root = xdoc.SelectSingleNode("/Response/Head");
            if (root != null && root.InnerText == "OK")
            {
                XmlNode o = xdoc.SelectSingleNode("/Response/Body/OrderResponse");
                if (o != null && o.Attributes != null)
                {
                    if (o.Attributes["mailno"] != null)
                    {
                        response.Model.MailNo = o.Attributes["mailno"].Value.Trim();
                    }
                    if (o.Attributes["remark"] != null)
                    {
                        response.Model.Remark = o.Attributes["remark"].Value.Trim();
                    }
                    if (o.Attributes["agent_mailno"] != null)
                    {
                        response.Model.AgentMailNo = o.Attributes["agent_mailno"].Value.Trim();
                    }
                    if (o.Attributes["origincode"] != null)
                    {
                        response.Model.OriginCode = o.Attributes["origincode"].Value;
                    }
                    if (o.Attributes["destcode"] != null)
                    {
                        response.Model.DestCode = o.Attributes["destcode"].Value;
                    }
                    if (o.Attributes["orderid"] != null)
                    {
                        response.Model.OrderId = o.Attributes["orderid"].Value.Trim();
                    }
                    if (o.Attributes["filter_result"] != null)
                    {
                        response.Model.FilterResult = o.Attributes["filter_result"].Value.Trim();
                    }
                    if (o.Attributes["return_tracking_no"] != null)
                    {
                        response.Model.TrackNumber = o.Attributes["return_tracking_no"].Value.Trim();
                    }
                    return response;
                }
            }else if (root != null && root.InnerText == "ERR")
            {
                 XmlNode err = xdoc.SelectSingleNode("/Response/ERROR");
                if (err != null && err.Attributes != null && err.Attributes["code"] != null &&
                    !err.Attributes["code"].Value.IsNullOrWhiteSpace())
                {
                    var regex = new Regex(@"^[0-9,]*$");
                    if (regex.IsMatch(err.Attributes["code"].Value) && err.Attributes["code"].Value != "8016")
                    {
                        var errmsg = new List<string>();
                        err.Attributes["code"].Value.Split(',')
                                              .ToList()
                                              .ForEach(
                                                  p =>
                                                  errmsg.Add(
                                                      ErrorCode.ResourceManager
                                                         .GetString(p).IsNullOrWhiteSpace() ? "[{0}]数据格式验证失败".FormatWith(p) : "[{0}]{1}".FormatWith(p, ErrorCode.ResourceManager
                                                         .GetString(p))));
                        Log.Error("运单号为：{2}提交俄罗斯顺丰API错误代码为:{0},错误信息：{1}".FormatWith(err.Attributes["code"].Value,
                                                                                 string.Join(",",errmsg),
                                                                                 model.OrderId));
                        response.ErrorMsg = string.Join(",", errmsg);
                        return response;
                    }
                    else if (err.Attributes["code"].Value == "8016")
                    {
                        response.Model = SearchOrderInfo(model.OrderId,authorization);
                        if (!response.Model.MailNo.IsNullOrWhiteSpace())
                        {
                            return response;
                        }
                        else
                        {
                            response.ErrorMsg = ErrorCode.ResourceManager
                                                         .GetString(err.Attributes["code"].Value).IsNullOrWhiteSpace()
                                                    ? "[{0}]数据格式验证失败".FormatWith(err.Attributes["code"].Value)
                                                    : "[{0}]{1}".FormatWith(err.Attributes["code"].Value,
                                                                            ErrorCode.ResourceManager
                                                                                     .GetString(
                                                                                         err.Attributes["code"].Value));
                        }
                    }
                }
            }
            return response;
        }
        /// <summary>
        /// 查询顺丰
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
        public static SfOrderResponse SearchOrderInfo(string orderId, string authorization)
        {
            var model = new SfOrderResponse();
            string strXml = @"<Request service='OrderSearchService' lang='en'>
                                <Head>" + authorization + @"</Head>
                                <Body>
                                <OrderSearch orderid='"+orderId+@"' />
                                </Body>
                                </Request>
                                ";
            string responseResult = SfCommonService(strXml);
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
                        model.MailNo = o.Attributes["mailno"].Value.Trim();
                    }
                    if (o.Attributes["remark"] != null)
                    {
                        model.Remark = o.Attributes["remark"].Value.Trim();
                    }
                    if (o.Attributes["origincode"] != null)
                    {
                        model.OriginCode = o.Attributes["origincode"].Value;
                    }
                    if (o.Attributes["destcode"] != null)
                    {
                        model.DestCode = o.Attributes["destcode"].Value;
                    }
                    if (o.Attributes["agent_mailno"] != null)
                    {
                        model.AgentMailNo = o.Attributes["agent_mailno"].Value.Trim();
                    }
                    if (o.Attributes["orderid"] != null)
                    {
                        model.OrderId = o.Attributes["orderid"].Value.Trim();
                    }
                    if (o.Attributes["filter_result"] != null)
                    {
                        model.FilterResult = o.Attributes["filter_result"].Value.Trim();
                    }
                    if (o.Attributes["return_tracking_no"] != null)
                    {
                        model.TrackNumber = o.Attributes["return_tracking_no"].Value.Trim();
                    }
                }
            }
            else if (root != null && root.InnerText == "ERR")
            {
                XmlNode err = xdoc.SelectSingleNode("/Response/ERROR");
                if (err != null && err.Attributes != null && err.Attributes["code"] != null)
                {
                    Log.Error("运单号为：{2}订单查询俄罗斯顺丰API错误代码为:{0},错误信息：{1}".FormatWith(err.Attributes["code"].Value, ErrorCode.ResourceManager
                                                         .GetString(err.Attributes["code"].Value).IsNullOrWhiteSpace() ? "[{0}]数据格式验证失败".FormatWith(err.Attributes["code"].Value) : "[{0}]{1}".FormatWith(err.Attributes["code"].Value, ErrorCode.ResourceManager
                                                         .GetString(err.Attributes["code"].Value)),
                                                                             orderId));
                }
            }
            return model;
        }
        /// <summary>
        /// 向顺丰申请订单发货确定
        /// </summary>
        /// <param name="wayBillNumber">运单号</param>
        /// <param name="mailno">顺丰单号</param>
        /// <param name="dealType">订单操作标识 :1 -订单确认 2-消单</param>
        /// <returns></returns>
        public static bool SfConfirm(string wayBillNumber, string mailno, string authorization, int dealType = 1)
        {
            string confirmXML = @"<Request service='OrderConfirmService' lang='en'>
                                                                     <Head>" + authorization + @"</Head>
                                                                     <Body>
                                                                        <OrderConfirm orderid ='" + wayBillNumber + @"' 
                                                                                      mailno='" + mailno + @"'
                                                                                      dealtype='" + dealType.ToString() + @"'>
                                                                        </OrderConfirm>
                                                                    </Body>
                                                                </Request>";
            string responseResult = SfCommonService(confirmXML);
            Log.Info(responseResult);
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(responseResult);
            XmlNode root = xdoc.SelectSingleNode("/Response/Head");
            if (root != null && root.InnerText == "OK")
            {
                return true;
            }
            else if (root != null && root.InnerText == "ERR")
            {
                XmlNode err = xdoc.SelectSingleNode("/Response/ERROR");
                if (err != null && err.Attributes != null && err.Attributes["code"] != null)
                {
                    Log.Error("运单号为：{2}订单发货确定俄罗斯顺丰API错误代码为:{0},错误信息：{1}".FormatWith(err.Attributes["code"], err.InnerText,
                                                                             wayBillNumber));

                }
            }
            return false;
        }
    }
}
