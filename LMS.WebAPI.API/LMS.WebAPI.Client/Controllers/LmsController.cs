using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Services.FreightServices;
using LMS.Services.OrderServices;
using LMS.Services.CustomerServices;
using LMS.WebAPI.Client.Helper;
using LMS.WebAPI.Client.Models;
using LMS.WebAPI.Client.Properties;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Common.Caching;
using LighTake.Infrastructure.Http;
using LighTake.Infrastructure.Http.Infrastructure;
using ShippingMethodModel = LMS.WebAPI.Client.Models.ShippingMethodModel;

namespace LMS.WebAPI.Client.Controllers
{
    public class LmsController : ApiControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IFreightService _freightService;
        private readonly IWorkContext _workContext;
        public LmsController(IOrderService orderService,
                             ICustomerService customerService,
                             IFreightService freightService,
                             IWorkContext workContext)
        {
            _orderService = orderService;
            _customerService = customerService;
            _freightService = freightService;
            _workContext = workContext;
        }

        #region 获取国家列表
        //api/lms/GetCountry
        //[HttpGet]
        public Response<List<CountryModel>> GetCountry()
        {
            
            //if (Cache.Get("GetCountryList") != null)
            //{
            //    var list = Cache.Get("GetCountryList") as Response<List<CountryModel>>;
            //    return list;
            //}
            //else
            //{
                var model = new Response<List<CountryModel>>
                {
                    Item = null,
                    ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error0000),
                    ResultDesc = Resource.Error0000
                };

                model.Item = new List<CountryModel>();
                GetCountrys().ForEach(p => model.Item.Add(new CountryModel()
                {
                    CountryCode = p.CountryCode,
                    EName = p.EName,
                    CName = p.CName
                }));
                if (model.Item.Count < 1)
                {
                    model.ResultDesc = ErrorCodeHelper.GetErrorCode(ErrorCode.Error1006);
                    model.ResultDesc = Resource.Error1006;
                }
                //else
                //{
                //    Cache.Add("GetCountryList", model, 120);
                //}
                return model;
            //}
        }

        [NonAction]
        public List<CountryModel> GetCountrys()
        {
            try
            {
                var list = HttpHelper.DoRequest<List<CountryModel>>(sysConfig.LISWebApi + "API/LIS/GetCountrysList?enabled=true", EnumHttpMethod.GET);
                Log.Info(list.RawValue);
                return list.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LISWebApi + "API/LIS/GetCountrysList?enabled=true");
                Log.Exception(ex);
            }
            return new List<CountryModel>();
        }
        #endregion

        #region 获取运输方式列表

        public Response<List<ShippingMethodModel>> Get()
        {
            //if (Cache.Get("GetAllShippingMethod") != null && Cache.Get("CustomerCode") == CustomerCode)
            //{
            //    return Cache.Get("GetAllShippingMethod") as Response<List<ShippingMethodModel>>;
            //}
            //else
            //{

                var model = new Response<List<ShippingMethodModel>>
                {
                    Item = null,
                    ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error0000),
                    ResultDesc = Resource.Error0000
                };

                model.Item = new List<ShippingMethodModel>();
                int customerTypeId;
                Guid customerId = new Guid();
                var customer = _customerService.GetCustomer(CustomerCode);
                if (customer != null)
                {
                    customerTypeId = customer.CustomerTypeID ?? 0;
                    customerId = customer.CustomerID;
                }
                else
                {
                    customerTypeId = 0;
                }
                GetShippingMethodes(customerId, customerTypeId).ForEach(p => model.Item.Add(new ShippingMethodModel
                {
                    Code = p.Code,
                    EnglishName = p.EnglishName,
                    FullName = p.FullName,
                    DisplayName = p.DisplayName,
                    HaveTrackingNum = p.HaveTrackingNum
                }));
                if (model.Item.Count < 1)
                {
                    model.ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error1006);
                    model.ResultDesc = Resource.Error1006;
                }
                //else
                //{
                //    Cache.Add("CustomerCode", CustomerCode,60);
                //    Cache.Add("GetAllShippingMethod", model, 60);
                //}
                return model;
            //}
        }

        //api/lms/Get?countryCode=
        public Response<List<ShippingMethodModel>> Get(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
            {
                return Get();
            }
            //if (Cache.Get("GetShippingMethodList") != null && Cache.Get("countryCode") == countryCode && Cache.Get("CustomerCode") == CustomerCode)
            //{
            //    return Cache.Get("GetShippingMethodList") as Response<List<ShippingMethodModel>>;
            //}
            //else
            //{

                var model = new Response<List<ShippingMethodModel>>
                {
                    Item = null,
                    ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error0000),
                    ResultDesc = Resource.Error0000
                };

                model.Item = new List<ShippingMethodModel>();
                int customerTypeId;
                Guid customerId=new Guid();
                var customer = _customerService.GetCustomer(CustomerCode);
                if (customer != null)
                {
                    customerTypeId = customer.CustomerTypeID ?? 0;
                    customerId = customer.CustomerID;
                }
                else
                {
                    customerTypeId = 0;
                }
                GetShippingMethodes(customerId, customerTypeId, countryCode).ForEach(p => model.Item.Add(new ShippingMethodModel
                {
                    Code = p.Code,
                    EnglishName = p.EnglishName,
                    FullName = p.FullName,
                    DisplayName = p.DisplayName,
                    HaveTrackingNum = p.HaveTrackingNum
                }));
                if (model.Item.Count < 1)
                {
                    model.ResultDesc = ErrorCodeHelper.GetErrorCode(ErrorCode.Error1006);
                    model.ResultDesc = Resource.Error1006;
                }
                //else
                //{
                //    Cache.Add("CustomerCode",CustomerCode,60);
                //    Cache.Add("countryCode", countryCode, 60);
                //    Cache.Add("GetShippingMethodList", model, 60);
                //}
                return model;
            //}
        }

        private List<ShippingMethodModel> GetShippingMethodes(System.Guid customerId, int customerTypeId, string countryCode = null)
        {
            try
            {
                if (string.IsNullOrEmpty(countryCode))
                {
                    var list = HttpHelper.DoRequest<List<ShippingMethodModel>>(sysConfig.LISWebApi + string.Format("API/LIS/GetShippingMethodsByTemplateCustomerTypeId?customerTypeId={0}&customerId={1}&enabled=True", customerTypeId,customerId), EnumHttpMethod.GET);
                    Log.Info(list.RawValue);
                    return list.Value;
                }
                else
                {
                    var list = HttpHelper.DoRequest<List<ShippingMethodModel>>(sysConfig.LISWebApi + string.Format("API/LIS/GetShippingMethodsByTemplateCustomerTypeId?customerTypeId={0}&customerId={1}&countryCode={2}&enabled=True", customerTypeId, customerId, countryCode), EnumHttpMethod.GET);
                    //var liss=HttpHelper.DoRequest<List<ShippingMethodModel>>("url",EnumHttpMethod.POST,EnumContentType.Json,)
                    Log.Info(list.RawValue);
                    return list.Value;
                }
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LISWebApi + string.Format("API/LIS/GetShippingMethodsByCustomerTypeId?customerTypeId={0}&enabled=True", customerTypeId));
                Log.Exception(ex);
            }
            return new List<ShippingMethodModel>();
        }
        #endregion

        #region 获取商品类型
        //api/lms/GetGoodstype
        public Response<List<GoodsTypeModel>> GetGoodstype()
        {
            var model = new Response<List<GoodsTypeModel>>
            {
                Item = null,
                ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error0000),
                ResultDesc = Resource.Error0000
            };
            model.Item = new List<GoodsTypeModel>();
            if (_orderService.GetGoodsTypes().Count > 1)
            {
                _orderService.GetGoodsTypes().ForEach(p => model.Item.Add(new GoodsTypeModel()
                {
                    GoodsTypeID = p.GoodsTypeID,
                    GoodsTypeName = p.GoodsTypeName
                }));
            }
            if (model.Item.Count < 1)
            {
                model.ResultDesc = ErrorCodeHelper.GetErrorCode(ErrorCode.Error1006);
                model.ResultDesc = Resource.Error1006;
            }
            return model;
        }
        #endregion

        #region 价格查询
        //api/lms/GetPrice?countryCode=AE&weight=2&length=1&width=1&height=1& packageType=15
        //string countryCode, string weight, string length, string width, string height, string packageType
        public Response<List<QuotationModel>> GetPrice(string countryCode, string weight, string length, string width, string height, string shippingTypeId, bool enableTariffPrepay=false)
        {
            var model = new Response<List<QuotationModel>>
            {
                Item = null,
                ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error0000),
                ResultDesc = Resource.Error0000
            };
            FreightTrialFilterModel filter = new FreightTrialFilterModel();
            #region 参数及结果验证
            int lengths=1;
            int widths=1;
            int heights=1;
            int shippingTypeIds;
            decimal weights;
            if (string.IsNullOrWhiteSpace(weight) || string.IsNullOrWhiteSpace(shippingTypeId) || string.IsNullOrWhiteSpace(countryCode))
            {
                model.ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error1004);
                model.ResultDesc = Resource.Error1004;
                return model;
            }
            filter.CountryCode = countryCode;
            if (Int32.TryParse(length, out lengths))
            {
                filter.Length = lengths;
            }else
            {
                filter.Length = lengths;
            }
            if (Int32.TryParse(width, out widths))
            {
                filter.Width = widths;
            }else
            {
                filter.Width = widths;
            }
            if (Int32.TryParse(height, out heights))
            {
                filter.Height = heights;
            }else
            {
                filter.Height = heights;
            }
            if (Int32.TryParse(shippingTypeId, out shippingTypeIds))
            {
                filter.PackageType = shippingTypeIds;
            }else
            {
                model.ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error1004);
                model.ResultDesc = Resource.Error1004;
                return model;
            }
            if (decimal.TryParse(weight, out weights))
            {
                filter.Weight = weights;
            }else
            {
                model.ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error1004);
                model.ResultDesc = Resource.Error1004;
                return model;
            }

            filter.EnableTariffPrepay = enableTariffPrepay;//是否启用关税预付服务

            List<FreightModel> freightList = GetFreightList(filter).FreightList;

            if (freightList.Count < 1)
            {
                model.ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error1006);
                model.ResultDesc = Resource.Error1006;
            }
            else
            {
                model.Item = new List<QuotationModel>();
                freightList.ForEach(p => model.Item.Add(new QuotationModel()
                {
                    Code = p.Code,
                    ShippingMethodName = p.ShippingMethodName,
                    ShippingMethodEName = p.ShippingMethodEName,
                    ShippingFee = p.ShippingFee,
                    RegistrationFee = p.RegistrationFee,
                    FuelFee = p.FuelFee,
                    SundryFee = p.SundryFee,
                    TariffPrepayFee = p.TariffPrepayFee,
                    TotalFee = p.TotalFee,
                    DeliveryTime = p.DeliveryTime,
                    Remarks = p.Remarks
                }));
            }
            #endregion
           
            return model;

        }

        private PriceModel GetFreightList (FreightTrialFilterModel filter)
        {
            var returnModels = new PriceModel { Filter = filter };

            if (filter.Weight==0 )
            {
                throw new Exception("重量必填");
            }

            if (string.IsNullOrWhiteSpace(filter.CountryCode))
            {
                throw new Exception("请选择发货国家");
            }
           
            var customer = _customerService.GetCustomer(CustomerCode);
            if (customer == null || !customer.CustomerTypeID.HasValue)
            {
                throw new Exception("客户类型不存在");
            }
            
            var list = _freightService.GetCustomerShippingPrices(new FreightPackageModel()
            {
                Weight = filter.Weight,
                Length = filter.Length ?? 0,
                Width = filter.Width ?? 0,
                Height = filter.Height ?? 0,
                CountryCode = filter.CountryCode,
                ShippingTypeId = filter.PackageType,
                CustomerTypeId = customer.CustomerTypeID.Value,
                CustomerId = customer.CustomerID,
                EnableTariffPrepay = filter.EnableTariffPrepay,
            });

            var shippingList = _freightService.GetShippingMethodListByCustomerCode(CustomerCode, true);
            foreach (var item in list)
            {
                if (!item.CanShipping) continue;
                if (item.ShippingMethodId == null) throw new Exception(string.Format("没有运输方式"));
                var shippingMethod =
                    shippingList.First(
                        s => s.ShippingMethodId == item.ShippingMethodId.Value);
                if (shippingMethod == null) throw new Exception(string.Format("运输方式【{0}】不存在", item.ShippingMethodId.Value));

                returnModels.FreightList.Add(new FreightModel
                {
                    ShippingMethodName = shippingList.First(s => item.ShippingMethodId != null && s.ShippingMethodId == item.ShippingMethodId.Value).ShippingMethodName,
                    ShippingMethodEName = shippingList.First(s => item.ShippingMethodId != null && s.ShippingMethodId == item.ShippingMethodId.Value).ShippingMethodEName,
                    Code = shippingList.First(s => item.ShippingMethodId != null && s.ShippingMethodId == item.ShippingMethodId.Value).ShippingMethodCode,
                    Weight = item.Weight,
                    ShippingFee = item.ShippingFee,
                    RegistrationFee = item.RegistrationFee,
                    RemoteAreaFee = item.RemoteAreaFee,
                    FuelFee = item.FuelFee,
                    OtherFee = item.OtherFee,
                    DeliveryTime = item.DeliveryTime,
                    Remarks = item.Remark,
                    TariffPrepayFee = item.TariffPrepayFee
                });
            }
            returnModels.FreightList = returnModels.FreightList.OrderBy(o => o.TotalFee).ToList();
            return returnModels;
        }
        #endregion
    }
}