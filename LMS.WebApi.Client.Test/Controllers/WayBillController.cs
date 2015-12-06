using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using LMS.Data.Entity;
using LMS.WebAPI.Client;
using LMS.WebAPI.Client.Models;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Http;
using LighTake.Infrastructure.Http.Infrastructure;
using NUnit.Framework;
using ShippingInfoModel = LMS.WebAPI.Client.Models.ShippingInfoModel;

namespace LMS.WebApi.Client.Test.Controllers
{
    [TestFixture]
    public class WayBillController
    {
        HttpClient  client;
        private string _baseAddress = string.Empty;
        [SetUp]
        public void Setup()
        {
            _baseAddress = ConfigurationManager.AppSettings["serviceBaseUri"];
            client=new HttpClient();
        }
        [Test]
        public void Add()
        {
            var appList = new List<ApplicationInfoModel>();
            appList.Add(new ApplicationInfoModel
                {
                     ApplicationName ="摩托车护目镜",
                                        Qty = 1,
                                        HSCode = "12585",
                                        UnitPrice = 10,
                                        UnitWeight = 1,
                                        PickingName ="test",
                                        Remark = "test",
                });
               appList.Add(new ApplicationInfoModel
                {
                     ApplicationName ="摩托车护目镜2",
                                        Qty = 1,
                                        HSCode = "12585",
                                        UnitPrice = 10,
                                        UnitWeight = 1,
                                        PickingName ="test",
                                        Remark = "test",
                });
            WayBillModel model = new WayBillModel
                {
                    OrderNumber = "15075190526955",
                    TrackingNumber = "EE982737115CN",
                    IsReturn = true,
                    InShippingMethodName = "UPS快递",
                    ApplicationType = 1,
                    PackageNumber = 1,
                    ShippingMethodCode="UPS",
                    Weight=10,
                    Length=1,
                    Width = 1,
                    Height = 1,
                    InsuranceType = 1,
                    SensitiveTypeID=1,
                    InsureAmount = 1000,
                    ShippingInfo = new LMS.WebAPI.Client.Models.ShippingInfoModel
                        {
                             ShippingTaxId = "cn",
                                CountryCode = "AD",
                                ShippingCity = "city",
                                ShippingCompany ="company",
                                ShippingAddress ="adress",
                                ShippingFirstName = "xing",
                                ShippingLastName ="ming",
                                ShippingPhone = "12345678456",
                                ShippingState = "guangdong",
                                ShippingZip = "123456"
                        },
                        SenderInfo = new SenderInfoModel
                        {
                                SenderCity = "city",
                                SenderCompany ="company",
                                SenderAddress ="adress",
                                SenderFirstName = "xing",
                                SenderLastName ="ming",
                                SenderPhone = "12345678456",
                                SenderState = "guangdong",
                                SenderZip = "123456"
                        },
                        ApplicationInfos=appList
                };
            List<WayBillModel> wayBillModels=new List<WayBillModel>();
            wayBillModels.Add(model);

            string s = JsonHelper.JsonSerializer(wayBillModels);
           // client.PostAsync(_baseAddress + "WayBill/BatchAdd")
            var list = HttpHelper.DoRequest<List<WayBillModel>>(_baseAddress + "WayBill/BatchAdd", EnumHttpMethod.POST, EnumContentType.Json, wayBillModels);
            //var list=HttpHelper.DoRequestBasic<List<WayBillModel>>(_baseAddress + "WayBill/BatchAdd", EnumHttpMethod.POST,"C48233","mwV1weJ1QVY=", EnumContentType.Json, wayBillModels)
            Assert.IsTrue(list.Value.Count > 0);
        }
    }
}
