using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using FluentValidation.Results;
using LMS.Data.Entity;

using LMS.Data.Express.DHL.Request;
using LMS.Services.SequenceNumber;
using LMS.UserCenter.Controllers.OrderController.Models;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Http;
using LighTake.Infrastructure.Http.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Wuyi.Common;

namespace LMS.Tests
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestMethod1()
        {
            /*  var list = new List<OrderModel>()
                  {
                      new OrderModel
                          {
                              CustomerOrderNumber = "O1"
                          },
                      new OrderModel
                          {
                              CustomerOrderNumber = "O2"
                          }
                  };

              var validator = new OrderModelValidator();
              foreach (var item in list)
              {
                  ValidationResult results = validator.Validate(item);

                  bool validationSucceeded = results.IsValid;
                  IList<ValidationFailure> failures = results.Errors;
                  Console.WriteLine("******************{0}**********************".FormatWith(item.CustomerOrderNumber));
                  foreach (var error in failures)
                  {
                      if (!validationSucceeded)
                          Console.WriteLine(error.ErrorMessage);
                  }

              }
              */

        }

        [TestMethod]
        public void DHLShipmentRequest()
        {


            ShipmentValidateRequestAP ap = new ShipmentValidateRequestAP()
                {
                    Billing = new Billing()
                        {
                            DutyPaymentType = DutyTaxPaymentType.R,
                            ShipperAccountNumber = "550000055",
                            ShippingPaymentType = ShipmentPaymentType.S
                        },
                    Commodity = new[]
                        {
                            new Commodity()
                                {
                                    CommodityCode = "111111",
                                    CommodityName = "xiaoming"
                                }
                        },
                    Consignee = new Consignee() //收货人
                        {
                            AddressLine = new[]
                                {
                                    "2711 St", "Appt 211"
                                },
                            City = "RedWood City",
                            CompanyName = "Prasanta INC",
                            Contact = new Contact()
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
                                    PersonName = "Prasanta Sinha",
                                    FaxNumber = "11234325423",
                                    PhoneExtension = 45232,
                                    PhoneNumber = "11234325423",
                                    Telex = "454586"
                                },
                            CountryCode = "US",
                            CountryName = "United States of America",
                            DivisionCode = "CA",
                            PostalCode = "94065"
                        },
                    Dutiable = new Dutiable()
                        {
                            DeclaredCurrency = "USD",
                            DeclaredValue = "50.11",
                            ShipperEIN = "Text"
                        },
                    LanguageCode = "en",
                    PiecesEnabled = PiecesEnabled.Y,
                    Reference = new[]
                        {
                            new Reference()
                                {
                                    ReferenceID = "String",
                                    ReferenceType = "St"
                                }
                        },
                    Request = new Request()
                        {
                            ServiceHeader = new ServiceHeader()
                                {
                                    MessageReference = "1234567890123456789012345678901",
                                    //MessageTime = DateTime.Parse("2011-10-27T15:28:56.000-08:00"),
                                    Password = "testServVal",
                                    SiteID = "DServiceVal"
                                }
                        },
                    ShipmentDetails = new ShipmentDetails() //出货详情
                        {
                            Contents = "For testing purpose only. Please do not ship",
                            CurrencyCode = "USD",
                            Date = DateTime.Parse("2014-04-06"),
                            DimensionUnit = DimensionUnit.C,
                            DoorTo = DoorTo.DD,
                            GlobalProductCode = "P",
                            LocalProductCode = "P",
                            NumberOfPieces = 1,
                            PackageType = PackageType.EE,
                            Weight = (decimal) 0.5,
                            WeightUnit = WeightUnit.K,
                            //InsuredAmount = "10000",
                            Pieces = new[]
                                {
                                    new Piece
                                        {
                                            // PieceID = "String",
                                            Depth = 2,
                                            // DimWeight = (decimal)2.111,
                                            Height = 2,
                                            //PackageType = PackageType.EE,
                                            Weight = (decimal) 0.5,
                                            Width = 2
                                        }
                                }
                        },
                    Shipper = new Shipper() //发货人
                        {
                            ShipperID = "550000055",
                            AddressLine = new[] {"333 Twin"},
                            City = "Kuala Lumpur",
                            CompanyName = "Santa inc",
                            Contact = new Contact()
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
                                    PersonName = "santa santa",
                                    FaxNumber = "17456356365",
                                    PhoneExtension = 6536,
                                    PhoneNumber = "15356456364",
                                    Telex = "74558"
                                },
                            CountryCode = "MY",
                            CountryName = "Malaysia",
                            DivisionCode = "KL",
                            PostalCode = "94065"
                        }
                };

            var ns = new XmlSerializerNamespaces();
            ns.Add("req", "http://www.dhl.com");
            string url = "https://xmlpitest-ea.dhl.com/XMLShippingServlet";
            var responseResult = HttpHelper.PostSendRequest(ap, url, ns);
            try
            {
                var response =
                    SerializeUtil.DeserializeFromXml<LMS.Data.Express.DHL.Response.ShipmentValidateResponse>(
                        responseResult);

                Assert.IsNotNull(response);
            }
            catch (Exception)
            {
                var response =
                    SerializeUtil.DeserializeFromXml<LMS.Data.Express.DHL.Response.Error.ShipmentValidateErrorResponse>(
                        responseResult);

                Assert.Fail();
            }

        }

        [TestMethod]
        public void TestTrackingInfo()
        {
            OrderTrackingRequestModel orderTrackingRequest = new OrderTrackingRequestModel()
            {
                ShipmentID = 1,
                CustomerCode = "admin",
                TrackingNumber = "4329091572"
            };
            var success = HttpHelper.DoRequest<int>("http://localhost:18736/api/Tracking/AddOutTrackingInfo", EnumHttpMethod.POST, EnumContentType.Json, orderTrackingRequest);

            Assert.IsTrue(success.Value != 0);
        }

        List<string> listSequenceNumber = new List<string>();

        //[TestMethod]
        //public void TestSequenceNumber()
        //{
        //    int maxCount = 5000;

        //    List<object> list = new List<object>();

        //    for (int i = 0; i < maxCount; i++)
        //    {
        //        list.Add(i);
        //    }

            //WThreadPoll threadPoll = new WThreadPoll(50, GetSequenceNumber, list);

            //threadPoll.StartPoll();

            //threadPoll.WaitAllWorkItemComplete();

            //bool success = listSequenceNumber.Distinct().Count() == listSequenceNumber.Count;

            //Assert.IsTrue(success);

        private void GetSequenceNumber(object obj)
        {
            int per = 2;

            var firstSequenceNumber = SequenceNumberService.GetSequenceNumber(PrefixCode.ReturnGoodsID, per);

            var first =
                firstSequenceNumber.Substring(PrefixCode.ReturnGoodsID.Length,
                    firstSequenceNumber.Length - PrefixCode.ReturnGoodsID.Length).ConvertTo<long>();

            for (int j = 0; j < per; j++)
            {
                listSequenceNumber.Add(PrefixCode.ReturnGoodsID + (first + j).ToString());
            }
        }

        [TestMethod]
        public void TestStringSplitLengthWords()
        {
            //List<string> strs = "xxxxxx xxxxxx xxxxxx xxxxxx xxxxxx xxxxxx xxxxxx".StringSplitLengthWords(35);
            List<string> strs = @"Francisco rueda,6Pe,arroya-Pueblonuevo,Cordoba".StringSplitLengthWords(35);
            strs.ForEach(p => Debug.WriteLine(p));
        }

        [TestMethod]
        public void IsRemoteArea()
        {
            string url = "http://t.tinydx.com:901/LIS.API.V3/api/lis/IsRemoteArea";
            url=url.AppendUrlParameters("shippingMethodId", "1090")
               .AppendUrlParameters("countryCode", "AQ")
               .AppendUrlParameters("shippingZip", "E8G1R1");
            var success = HttpHelper.DoRequest<bool>(url, EnumHttpMethod.GET);

            Assert.IsTrue(success.Value);
        }
    }
}
