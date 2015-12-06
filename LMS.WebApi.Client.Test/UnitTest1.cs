using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using LMS.Data.Entity;
using LMS.Services.CustomerOrderServices;
using LMS.Services.OrderServices;
using LMS.Services.TrackingNumberServices;
using LMS.WebAPI.Client;
using LMS.WebAPI.Client.Controllers;
using LMS.WebAPI.Client.Models;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Http;
using LighTake.Infrastructure.Http.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;
using ShippingInfoModel = LMS.WebAPI.Client.Models.ShippingInfoModel;
using ShippingMethodModel = LMS.WebAPI.Client.Models.ShippingMethodModel;

namespace LMS.WebApi.Client.Test
{
    [TestClass]
    public class UnitTest1
    {
        private HttpClient client;
       private string _baseAddress;

        WayBillController wayBillController;
        private readonly ICustomerOrderService _customerOrderService;
        private readonly ITrackingNumberService _trackingNumberService;
        private readonly IOrderService _orderService;

        [TestMethod]
        public void TestMethod1()
        {

        }
        [TestInitialize]
        public void Setup()
        {
            _baseAddress = ConfigurationManager.AppSettings["serviceBaseUri"];
            client = new HttpClient();
        }

        [TestMethod]
        public void BatchAddWayBill()
        {
            var appList = new List<ApplicationInfoModel>();
            appList.Add(new ApplicationInfoModel
            {
                ApplicationName = "摩托车护目镜",
                Qty = 1,
                HSCode = "12585",
                UnitPrice = 10,
                UnitWeight = 1,
                PickingName = "test",
                Remark = "test",
            });
            appList.Add(new ApplicationInfoModel
            {
                ApplicationName = "摩托车护目镜2",
                Qty = 1,
                HSCode = "12585",
                UnitPrice = 10,
                UnitWeight = 1,
                PickingName = "test",
                Remark = "test",
            });
            WayBillModel model = new WayBillModel
            {
                OrderNumber = "15075190526955",
                TrackingNumber = "EE982737115CN",
                IsReturn = true,
                InShippingMethodId = 15,
                InShippingMethodName = "UPS快递",
                ApplicationType = 2,
                PackageNumber = 1,
                ShippingMethodCode = "UPS",
                Weight = 10,
                Length = 1,
                Width = 1,
                Height = 1,
                InsuranceType = 2,
                SensitiveTypeID = 0,
                InsureAmount = 1000,
                ShippingInfo = new LMS.WebAPI.Client.Models.ShippingInfoModel
                {
                    ShippingTaxId = "cn",
                    CountryCode = "AD",
                    ShippingCity = "city",
                    ShippingCompany = "company",
                    ShippingAddress = "adress",
                    ShippingFirstName = "xing",
                    ShippingLastName = "ming",
                    ShippingPhone = "12345678456",
                    ShippingState = "guangdong",
                    ShippingZip = "123456"
                },
                SenderInfo = new SenderInfoModel
                {
                    SenderCity = "city",
                    SenderCompany = "company",
                    SenderAddress = "adress",
                    SenderFirstName = "xing",
                    SenderLastName = "ming",
                    SenderPhone = "12345678456",
                    SenderState = "guangdong",
                    SenderZip = "123456"
                },
                ApplicationInfos = appList



            };
            List<WayBillModel> wayBillModels = new List<WayBillModel>();
            wayBillModels.Add(model);

            var list = HttpHelper.DoRequest<List<WayBillModel>>(_baseAddress + "WayBill/BatchAdd", EnumHttpMethod.POST, EnumContentType.Json, wayBillModels);

            Assert.IsTrue(list.Value.Count > 0);
        }

        [TestMethod]
        public void GetCountryTest()
        {
            
            //获取Json流结果
            var countrylist = client.GetAsync(_baseAddress + "lms/GetCountry").Result.Content.ReadAsStringAsync().Result;
            //将Json流序列化成集合
            var countryModel = JsonHelper.JsonDeserialize<Response<List<CountryModel>>>(countrylist);
            Assert.IsTrue(countryModel.Item.Count == 250);
        }


        [TestMethod]
        public void GetShippingMethodTest()
        {
            //获取Json流结果
            var shippingMethodlist = client.GetAsync(_baseAddress + "lms/Get?countryCode=AD").Result.Content.ReadAsStringAsync().Result;
            var shippingMethodModel = JsonHelper.JsonDeserialize<Response<List<ShippingMethodModel>>>(shippingMethodlist);
            //7是数据库里的数据
            Assert.IsTrue(shippingMethodModel.Item.Count>2);
        }

        [TestMethod]
        public void GetGoodstypeTest()
        {
            //获取Json流结果
            var goodsTypeList = client.GetAsync(_baseAddress + "lms/GetGoodstype").Result.Content.ReadAsStringAsync().Result;
            var goodsTypeModel = JsonHelper.JsonDeserialize<Response<List<GoodsTypeModel>>>(goodsTypeList);
            Assert.IsTrue(goodsTypeModel.Item.Count == 2);
        }

        [TestMethod]
        public void GetPriceTest()
        {
            var priceList =
                client.GetAsync(_baseAddress +
                                "lms/GetPrice?countryCode=AE&weight=2&length=1&width=1&height=1&shippingTypeId=1")
                      .Result.Content.ReadAsStringAsync()
                      .Result;
            var goodsTypeModel = JsonHelper.JsonDeserialize<Response<List<QuotationModel>>>(priceList);
            Assert.IsTrue(goodsTypeModel.Item.Count >0);
        }

    }
}
