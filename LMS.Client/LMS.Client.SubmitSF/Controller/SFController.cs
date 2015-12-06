using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using LMS.Client.SubmitSF.Model;
using LMS.Services.SF;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.SubmitSF.Controller
{
    public class SFController
    {
        private static readonly string SFCheckWord = System.Configuration.ConfigurationManager.AppSettings["SFCheckWord"];
        private static readonly string ClientCode = System.Configuration.ConfigurationManager.AppSettings["ClientCode"];
        /// <summary>
        /// 向顺丰申请订单发货确定
        /// </summary>
        /// <param name="wayBillNumber">运单号</param>
        /// <param name="mailno">顺丰单号</param>
        /// <param name="dealType">订单操作标识 :1 -订单确认 2-消单</param>
        /// <returns></returns>
        public static bool NlPostConfirm(string wayBillNumber, string mailno, int dealType = 1)
        {
            string confirmXML = @"<Request service='OrderConfirmService' lang='en'>
                                                                     <Head>" + ClientCode + @"</Head>
                                                                     <Body>
                                                                        <OrderConfirm orderid ='" + wayBillNumber.WayBillNumberReplace() + @"' 
                                                                                      mailno='" + mailno + @"'
                                                                                      dealtype='" + dealType.ToString() + @"'>
                                                                        </OrderConfirm>
                                                                    </Body>
                                                                </Request>";
            string responseResult = LMSSFService.SfExpressService(confirmXML, (confirmXML + SFCheckWord).MD5Encrypt());
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
                    Log.Error("运单号为：{2}订单发货确定顺丰API错误代码为:{0},错误信息：{1}".FormatWith(err.Attributes["code"], err.InnerText,
                                                                             wayBillNumber));

                }
            }
            return false;
        }

        /// <summary>
        /// 查询顺丰订单
        /// </summary>
        /// <param name="wayBillNumber">运单号</param>
        /// <returns></returns>
        public static NetherlandsParcelModel SearchNlPost(string wayBillNumber)
        {
            var model = new NetherlandsParcelModel() { WayBillNumber = wayBillNumber };
            string searchXML = @"<Request service='OrderSearchService' lang='en'>
                                <Head>" + ClientCode + @"</Head>
                                <Body>
                                <OrderSearch orderid ='" + wayBillNumber.WayBillNumberReplace() + @"' />
                                </Body>
                                </Request>
                                ";
            string responseResult = LMSSFService.SfExpressService(searchXML, (searchXML + SFCheckWord).MD5Encrypt());
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
        /// 向顺丰下订单
        /// </summary>
        /// <param name="wayBillInfo">订单信息</param>
        /// <returns></returns>
        public static NetherlandsParcelModel NlPost(WayBillSfModel wayBillInfo)
        {
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
                                                             d_company='" + (wayBillInfo.ShippingCompany ?? "-").Trim().ToDBC().StripXML() + @"'  
                                                             d_contact='" + (wayBillInfo.ShippingName ?? "").Trim().StripXML() + @"'
                                                             d_tel='" + (wayBillInfo.ShippingPhone.GetNumber() == "" ? "-" : wayBillInfo.ShippingPhone.GetNumber()) + @"'
                                                             d_mobile='" + (wayBillInfo.ShippingPhone.GetNumber() == "" ? "-" : wayBillInfo.ShippingPhone.GetNumber()) + @"'
                                                             d_address='" + wayBillInfo.ShippingAddress.Trim().ToDBC().StripXML() + @"' 
                                                             parcel_quantity='" + wayBillInfo.PackageNumber + @"' 
                                                             j_province='Guangdong province' 
                                                             j_city='Shenzhen' 
                                                             j_post_code='518055' 
                                                             j_country='CN'
                                                             d_country='" + wayBillInfo.CountryCode.Trim() + @"'      
                                                             d_post_code='" + wayBillInfo.ShippingZip.GetNumber() + @"'    
                                                             d_province='" + (wayBillInfo.ShippingState ?? "").Trim().ToDBC().StripXML() + @"'
                                                             d_city='" + (wayBillInfo.ShippingCity ?? "-").Trim().ToDBC().StripXML() + @"'
                                                             returnsign='" + (wayBillInfo.IsReturn ? "Y" : "N") + @"'       
                                                             cargo_total_weight='" + wayBillInfo.ApplicationInfo.Sum(p => p.Qty * p.UnitWeight).ToString("F3") + @"'    >                                     
                                           {1}
                                        </Order>
                                        </Body>
                                        </Request>";
            string application = string.Empty;
            foreach (var app in wayBillInfo.ApplicationInfo)
            {
                application += "<Cargo ename='" + app.ApplicationName.Trim().ToDBC().StripXML();
                if (app.HsCode.GetNumber() != "")
                {
                    application += "' hscode='" + app.HsCode.GetNumber();
                }
                application += "' count='" + app.Qty + "' unit='PCE' weight='" + app.UnitWeight + "'  amount='" +
                 app.UnitPrice.ToString("F2") + @"'> </Cargo> ";
            }
            string responseResult = LMSSFService.SfExpressService(strXML.FormatWith(ClientCode, application),
                                          (strXML.FormatWith(ClientCode, application) +
                                           SFCheckWord).MD5Encrypt());
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(responseResult);
            XmlNode root = xdoc.SelectSingleNode("/Response/Head");
            if (root != null && root.InnerText == "OK")
            {
                var parcel = new NetherlandsParcelModel();
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
                if (err != null && err.Attributes != null && err.Attributes["code"] != null && !err.Attributes["code"].Value.IsNullOrWhiteSpace())
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
                        if (errmsg.Count == 1&&err.Attributes["code"].Value.Trim()=="9002")
                        {
                            //客户订单号存在重复
                            var parcel = SearchNlPost(wayBillInfo.WayBillNumber);
                            if (!parcel.AgentMailNo.IsNullOrWhiteSpace() && !parcel.Remark.IsNullOrWhiteSpace() &&
                                parcel.Remark == "A")
                            {
                                return parcel;
                            }
                        }
                        else
                        {
                            Log.Error("运单号为：{2}提交顺丰API错误代码为:{0},错误信息：{1}".FormatWith(err.Attributes["code"].Value,
                                                                                     string.Join(",", errmsg),
                                                                                     wayBillInfo.WayBillNumber));

                            //拆入错误记录
                            WayBillController.SubmitFailure(wayBillInfo.WayBillNumber,
                                                            "错误代码为:{0},错误信息：{1}".FormatWith(
                                                                err.Attributes["code"].Value,
                                                                string.Join(",", errmsg)));
                        }
                    }
                    else
                    {
                        var parcel = SearchNlPost(wayBillInfo.WayBillNumber);
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
    }
}
