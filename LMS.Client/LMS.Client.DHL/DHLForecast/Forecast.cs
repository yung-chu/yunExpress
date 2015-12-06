using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LMS.Services.CountryServices;
using LMS.Services.CustomerOrderServices;
using LMS.Services.ExpressServices;
using LMS.Services.FreightServices;
using LMS.Services.OrderServices;
using LMS.WinForm.InversionOfControl;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.DHL.DHLForecast
{
    public class Forecast
    {
        private readonly IFreightService _freightService = EngineContext.Current.Resolve<IFreightService>();
        private readonly IOrderService _orderService = EngineContext.Current.Resolve<IOrderService>();
        private readonly ICustomerOrderService _customerOrderService = EngineContext.Current.Resolve<ICustomerOrderService>();
        private readonly IExpressService _expressService = EngineContext.Current.Resolve<IExpressService>();
        private readonly ICountryService _countryService = EngineContext.Current.Resolve<ICountryService>();

        public static string dhlshippingMethodCode = System.Configuration.ConfigurationManager.AppSettings["DHL_ShippingMethodCode"].ToString();
        public static string eubshippingMethodCode = System.Configuration.ConfigurationManager.AppSettings["EUB_ShippingMethodCode"].ToString();
        public static string venderCode = System.Configuration.ConfigurationManager.AppSettings["DHL_VenderCode"].ToString();
        public static string dhlBarCodePath = System.Configuration.ConfigurationManager.AppSettings["DHLBarCodePath"].ToString();
        
        private void ForecastDHLandEUB()
        {
            string errorWayBills ="";
            List<int> inShippingMethodIds = new List<int>();
            List<int> dhlShippingMethodIds = new List<int>();
            List<int> eubShippingMethodIds = new List<int>();
            var dhlcodes = dhlshippingMethodCode.Split(',').ToList();
            var eubcodes = eubshippingMethodCode.Split(',').ToList();
            var shippingMethod = _freightService.GetShippingMethods("", true);
            shippingMethod.ForEach(p =>
            {
                if (dhlcodes.Contains(p.Code))
                {
                    inShippingMethodIds.Add(p.ShippingMethodId);
                    dhlShippingMethodIds.Add(p.ShippingMethodId);
                }
                else if (eubcodes.Contains(p.Code))
                {
                    inShippingMethodIds.Add(p.ShippingMethodId);
                    eubShippingMethodIds.Add(p.ShippingMethodId);
                }
            });
            int number = 0;
            while (true)
            {
                var wayBillList = _orderService.GetDHLandEUBWayBillInfos(inShippingMethodIds, number);
                if (wayBillList.Count < 1)
                {
                    break;
                }
                List<WayBillInfo> dhlwayBills = new List<WayBillInfo>();
                List<WayBillInfo> eubWayBills = new List<WayBillInfo>();
                wayBillList.ForEach(p =>
                    {
                        if (dhlShippingMethodIds.Contains(p.InShippingMethodID.Value))
                        {
                            dhlwayBills.Add(p);
                        }
                        else if (eubShippingMethodIds.Contains(p.InShippingMethodID.Value))
                        {
                            eubWayBills.Add(p);
                        }
                    });
                //DHL预报
                if (dhlwayBills.Count > 0)
                {
                    var errors = PostDHLShipment(dhlwayBills);
                    errorWayBills = errors.Aggregate(errorWayBills, (current, error) => current + ("[" + error.Key + "]"));
                    UpdateWayBillInfo(errors);
                    
                }
                //EUB预报
                if (eubWayBills.Count > 0)
                {
                    EubWayBillParam param = new EubWayBillParam()
                        {
                            WayBillInfos = eubWayBills,
                            PrintFormat = 2,
                            PrintFormatValue = "01"

                        };
                    var errors = _customerOrderService.ForecastEubWayBillInfo(param, true);
                    if (errors.Count > 0)
                    {
                        errorWayBills = errors.Aggregate(errorWayBills, (current, error) => current + ("[" + error.Key + "]"));

                        UpdateWayBillInfo(errors);
                        //errors.ForEach(p=> errorWayBills+="["+p+"]");
                    }
                }
                number += wayBillList.Count;
            }
            Log.Info("预报失败运单:" + errorWayBills);
        }

        public Dictionary<string,string> PostDHLShipment(List<WayBillInfo> dhLWayBill)
        {
                Dictionary<string,string> errorwayBills=new Dictionary<string, string>();
               
                var applicationName = string.Empty;
                foreach (var wayBillInfo in dhLWayBill)
                {
                    var expressAccount = _expressService.GetExpressAccountInfo(venderCode, wayBillInfo.InShippingMethodID.Value);
                    if (expressAccount == null)
                    {
                        //"DHL帐号不存在，请求DHL接口失败"
                        errorwayBills.Add(wayBillInfo.WayBillNumber, "DHL帐号不存在，请求DHL接口失败");
                        continue;
                    }
                    applicationName = wayBillInfo.ApplicationInfos.First().ApplicationName.Cutstring(70);
                    string[] shippingAddress = wayBillInfo.ShippingInfo.ShippingAddress.StringSplitLengthWords(35).ToArray();
                    wayBillInfo.VenderCode = venderCode;
                    wayBillInfo.OutShippingMethodID = wayBillInfo.InShippingMethodID;
                    if (null != wayBillInfo.ExpressRespons)
                    {
                        //"运单号为{0},已经调用DHL接口,无需重复请求"
                        errorwayBills.Add(wayBillInfo.WayBillNumber, "已经调用DHL接口,无需重复请求");
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
                                    Tools.Base64StringToImage(dhlBarCodePath,
                                                              "DHLRouting" + Guid.NewGuid().ToString(""),
                                                              responseResult.Barcodes.DHLRoutingBarCode,
                                                              fileExtension),
                                DHLRoutingDataId = responseResult.DHLRoutingDataId,
                                LicensePlate = responseResult.Pieces[0].LicensePlate,
                                LicensePlateBarCodeImg =
                                    Tools.Base64StringToImage(dhlBarCodePath,
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
                                    Tools.Base64StringToImage(dhlBarCodePath,
                                                              "WayBillNumber" +
                                                              responseResult.AirwayBillNumber,
                                                              responseResult.Barcodes.AWBBarCode, fileExtension),
                            };

                            wayBillInfo.TrackingNumber = responseResult.AirwayBillNumber;
                            wayBillInfo.CustomerOrderInfo.TrackingNumber = responseResult.AirwayBillNumber;
                            try
                            {
                                _expressService.AddExpressResponse(response, wayBillInfo,true);
                            }
                            catch (Exception ex)
                            {
                                Log.Exception(ex);
                                errorwayBills.Add(wayBillInfo.WayBillNumber, ex.Message);
                                //model.Message += "运单号为{0}：错误信息：{1}".FormatWith(wayBillInfo.WayBillNumber, ex.Message);
                            }
                        }
                        else
                        {
                            errorwayBills.Add(wayBillInfo.WayBillNumber, "请求DHL接口失败");
                            //errorwayBills += "[" + wayBillInfo.WayBillNumber + "]";
                            //model.Message += "运单号为{0}：请求DHL接口失败".FormatWith(wayBillInfo.WayBillNumber);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                        errorwayBills.Add(wayBillInfo.WayBillNumber, ex.Message);
                        //model.Message += "运单号为{0}：错误信息：{1}".FormatWith(wayBillInfo.WayBillNumber, ex.Message);
                    }
                }
            return errorwayBills;
        }

        //将申请失败的运单拦截
        public void UpdateWayBillInfo(Dictionary<string, string> errorWayBills)
        {
            try
            {
                _orderService.UpdateWayBillInfo(errorWayBills);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        public static void Start()
        {
            try
            {
                var forecast = new Forecast();
                forecast.ForecastDHLandEUB();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
    }
}
