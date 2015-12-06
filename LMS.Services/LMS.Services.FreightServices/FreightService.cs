using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LMS.Data.Repository;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Http;
using LighTake.Infrastructure.Http.Infrastructure;

namespace LMS.Services.FreightServices
{
    public class FreightService : IFreightService
    {
     //   private ICustomerRepository _customerRepository;
        private ICountryRepository _countryRepository;

        public FreightService(ICustomerRepository _customerRepository,ICountryRepository countryRepository)
        {
           // _customerRepository = customerRepository;
            _countryRepository = countryRepository;
        }

        public List<CustomerType> GetCustomerTypeList()
        {
            var list = new List<CustomerType>();
            GetCustomerTypes().ForEach(p => list.Add(new CustomerType()
                {
                    CustomerTypeId = p.CustomerTypeId,
                    CustomerTypeName = p.Name
                }));

            return list;
        }

        public List<Vender> GetVenderList(bool IsAll)
        {
            var list = new List<Vender>();
            GetVenders(IsAll).ForEach(p => list.Add(new Vender()
                {
                    VenderCode = p.Code,
                    VenderName = p.Name,
                    VenderId = p.VenderId
                }));
            return list;
        }

        public VenderModel GetVender(string venderCode)
        {
            return GetVenders(false).FirstOrDefault(p => p.Code == venderCode);
        }

        public List<ShippingMethod> GetShippingMethodListByCustomerCode(string customerCode, bool IsAll)
        {
            var list = new List<ShippingMethod>();
            GetShippingMethods("",IsAll).ForEach(p => list.Add(new ShippingMethod()
                {
                    ShippingMethodId = p.ShippingMethodId,
                    ShippingMethodName = p.FullName,
                    ShippingMethodEName = p.EnglishName,
                    WeightOrVolume = p.CalcVolumeweight,
                    ShippingMethodCode = p.Code,
                    HaveTrackingNum = p.HaveTrackingNum,
                    ShippingMethodTypeId=p.ShippingMethodTypeId,
                    FuelRelateRAF = p.FuelRelateRAF
                }));
            return list;
        }

        public List<ShippingMethod> GetShippingMethodListByCustomerTypeId(int? customerTypeId, bool IsAll)
        {
            var list = new List<ShippingMethod>();
            if (customerTypeId == null)
            {
                GetShippingMethods("",IsAll).ForEach(p => list.Add(new ShippingMethod()
                    {
                        ShippingMethodId = p.ShippingMethodId,
                        ShippingMethodName = p.FullName,
                        WeightOrVolume = p.CalcVolumeweight,
                        ShippingMethodCode = p.Code,
                        HaveTrackingNum = p.HaveTrackingNum,
                        ShippingMethodTypeId = p.ShippingMethodTypeId,
                        FuelRelateRAF = p.FuelRelateRAF
                    }));
            }
            else
            {
                GetCustomerShippingMethods(customerTypeId.ToString(), IsAll).ForEach(p => list.Add(new ShippingMethod()
                    {
                        ShippingMethodId = p.ShippingMethodId,
                        ShippingMethodName = p.FullName,
                        WeightOrVolume = p.CalcVolumeweight,
                        ShippingMethodCode = p.Code,
                        HaveTrackingNum = p.HaveTrackingNum,
                        ShippingMethodTypeId = p.ShippingMethodTypeId,
                        FuelRelateRAF = p.FuelRelateRAF
                    }));
            }
            return list;
        }

        public List<ShippingMethod> GetShippingMethodListByCustomerTypeId(string customerId, int? customerTypeId,
                                                                          bool IsAll)
        {
            var list = new List<ShippingMethod>();
            if (string.IsNullOrWhiteSpace(customerId) && (customerTypeId == 0 || customerTypeId == null))
            {
                GetShippingMethods("", IsAll).ForEach(p => list.Add(new ShippingMethod()
                    {
                        ShippingMethodId = p.ShippingMethodId,
                        ShippingMethodName = p.FullName,
                        WeightOrVolume = p.CalcVolumeweight,
                        ShippingMethodCode = p.Code,
                        HaveTrackingNum = p.HaveTrackingNum,
                        IsHideTrackingNumber = p.IsHideTrackingNumber,
                        ShippingMethodTypeId = p.ShippingMethodTypeId,
                        FuelRelateRAF = p.FuelRelateRAF
                    }));
            }
            else
            {
                GetCustomerShippingMethodsInstoerage(customerId, customerTypeId.ToString(), IsAll)
                    .ForEach(p => list.Add(new ShippingMethod()
                        {
                            ShippingMethodId = p.ShippingMethodId,
                            ShippingMethodName = p.FullName,
                            WeightOrVolume = p.CalcVolumeweight,
                            ShippingMethodCode = p.Code,
                            HaveTrackingNum = p.HaveTrackingNum,
                            IsHideTrackingNumber = p.IsHideTrackingNumber,
                            ShippingMethodTypeId = p.ShippingMethodTypeId,
                            FuelRelateRAF = p.FuelRelateRAF
                        }));
            }
            return list;
        }

        /// <summary>
        ///  获取商业快递类型运输方式
        /// add by zhengsong
        /// </summary>
        /// <param name="IsAll"></param>
        /// <returns></returns>
        public List<ShippingMethodModel> GetShippingMethodList(bool IsAll)
        {
            List<ShippingMethodModel> list = new List<ShippingMethodModel>();
            //2,代表商业快递--运输方式类型.FindAll(p => p.ShippingMethodTypeId == 2)
            list = GetShippingMethods("", true);
            return list;
        }

        public List<ShippingMethod> GetShippingMethodListByVenderCode(string venderCode, bool IsAll)
        {
            return GetShippingMethodListByVenderCode(venderCode, 0, IsAll);
        }

        public List<ShippingMethod> GetShippingMethodListByVenderCode(string venderCode,int shippingMethodType, bool IsAll)
        {
            var list = new List<ShippingMethod>();
            GetShippingMethods(venderCode, IsAll, shippingMethodType).ForEach(p => list.Add(new ShippingMethod()
                {
                    ShippingMethodId = p.ShippingMethodId,
                    ShippingMethodName = p.FullName,
                    WeightOrVolume = p.CalcVolumeweight,
                    ShippingMethodCode = p.Code,
                    HaveTrackingNum = p.HaveTrackingNum,
                    IsHideTrackingNumber = p.IsHideTrackingNumber,
                    ShippingMethodTypeId = p.ShippingMethodTypeId,
                    FuelRelateRAF = p.FuelRelateRAF
                }));
            return list;
        }

        public PriceProviderResult GetCustomerShippingPrice(CustomerPackageModel customerPackageModel)
        {
            return PostCustomerTypePrice(customerPackageModel);
        }

        public PriceProviderResult GetCustomerShippingPrice(CustomerInfoPackageModel customerPackageModel)
        {
            return PostCustomerPrice(customerPackageModel);
        }

        public List<PriceProviderResult> GetCustomerShippingPrices(FreightPackageModel customerPackageModel)
        {
            //return PostCustomerTypePrices(customerPackageModel);
            return PostCustomerPrices(customerPackageModel);
        }


        public PriceProviderResult GetVenderShippingPrice(VenderPackageModel venderPackageModel)
        {
            return PostVenderPrice(venderPackageModel);
        }

        public List<ShippingMethodCountryModel> GetCountryArea(int shippingMethodId, string countryCode)
        {
            return GetShippingMethodCountries(shippingMethodId, countryCode);
        }


        public List<ShippingMethodCountryModel> GetCountryArea(int shippingMethodId)
        {
            return GetShippingMethodCountries(shippingMethodId);
        }

        public string GetChineseName(string countryCode)
        {
            var model = _countryRepository.First(p => p.CountryCode == countryCode);
            return model != null ? model.ChineseName : "";
        }

        public List<Country> GetCountryList()
        {
            return _countryRepository.GetAll().ToList();
        }

        private List<CustomerTypeModel> GetCustomerTypes()
        {
            try
            {
                var list =
                    HttpHelper.DoRequest<List<CustomerTypeModel>>(
                        sysConfig.LISAPIPath + "API/LIS/GetCustomerTypes?enabled=true", EnumHttpMethod.GET);
                Log.Info(list.RawValue);
                return list.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LISAPIPath + "API/LIS/GetCustomerTypes?enabled=true");
                Log.Exception(ex);
            }
            return new List<CustomerTypeModel>();
        }

        private List<VenderModel> GetVenders(bool IsAll)
        {
            var url = sysConfig.LISAPIPath + "API/LIS/GetVenders";
            if (!IsAll) url = url + "?enabled=true";
            try
            {
                var list = HttpHelper.DoRequest<List<VenderModel>>(url, EnumHttpMethod.GET);
                Log.Info(list.RawValue);
                return list.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + url);
                Log.Exception(ex);
            }
            return new List<VenderModel>();
        }

        private List<ShippingMethodCountryModel> GetShippingMethodCountries(int shippingMethodId, string countryCode)
        {
            try
            {
                var list =
                    HttpHelper.DoRequest<List<ShippingMethodCountryModel>>(
                        sysConfig.LISAPIPath +
                        string.Format("API/LIS/GetShippingMethodCountries?shippingMethodId={0}&countryCode={1}",
                                      shippingMethodId, countryCode), EnumHttpMethod.GET);
                Log.Info(list.RawValue);
                return list.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LISAPIPath +
                          string.Format("API/LIS/GetShippingMethodCountries?shippingMethodId={0}&countryCode={1}",
                                        shippingMethodId, countryCode));
                Log.Exception(ex);
                return null;
            }
            //return new List<ShippingMethodCountryModel>();
        }

        private List<ShippingMethodCountryModel> GetShippingMethodCountries(int shippingMethodId)
        {
            string urlRequest = sysConfig.LISAPIPath +
                                string.Format("API/LIS/GetShippingMethodCountries?shippingMethodId={0}",shippingMethodId);
            try
            {
                var list =
                    HttpHelper.DoRequest<List<ShippingMethodCountryModel>>(
                       urlRequest, EnumHttpMethod.GET);
                Log.Info(list.RawValue);
                return list.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + urlRequest);
                Log.Exception(ex);
                return null;
            }
            //return new List<ShippingMethodCountryModel>();
        }

        public List<ShippingMethodModel> GetShippingMethods(string venderCode, bool IsAll)
        {
            return GetShippingMethods(venderCode, IsAll, 0);
        }
        /// <param name="shippingMethodType">运输方式类型 0--全部，1--小包，2--商业快递</param>

        public List<ShippingMethodModel> GetShippingMethods(string venderCode, bool IsAll,int shippingMethodType=0)
        {
            var url = sysConfig.LISAPIPath + "API/LIS/GetShippingMethods";

            NameValueCollection nameValueCollection=new NameValueCollection();

            if (!IsAll)
            {
                nameValueCollection.Add("enabled","true");
            }

            nameValueCollection.Add("venderCode", venderCode);
            nameValueCollection.Add("shippingMethodType", shippingMethodType.ToString());

            url = url.AppendUrlParameters(nameValueCollection);

            try
            {
                var list = HttpHelper.DoRequest<List<ShippingMethodModel>>(url, EnumHttpMethod.GET);
                Log.Info(list.RawValue);
                return list.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + url);
                Log.Exception(ex);
            }
            return new List<ShippingMethodModel>();
        }

        /// <summary>
        /// 获取需要隐藏跟踪号的运输方式
        /// Add By zhengsong
        /// Time:2014-06-11
        /// </summary>
        /// <returns></returns>
        public List<ShippingMethodModel> GetShippingMethodsByHide()
        {

            var url = sysConfig.LISAPIPath + "API/LIS/GetShippingMethodsByHide";
            try
            {
                var list = HttpHelper.DoRequest<List<ShippingMethodModel>>(url, EnumHttpMethod.GET);
                Log.Info(list.RawValue);
                return list.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + url);
                Log.Exception(ex);
            }
            return new List<ShippingMethodModel>();
        }

        /// <summary>
        /// Add by zhengsong
        /// </summary>
        /// <param name="customerTypeId">获取客户运输方式</param>
        /// <param name="IsAll"></param>
        /// <returns></returns>
        public List<ShippingMethodModel> GetCustomerShippingMethods(string customerTypeId, bool IsAll)
        {
            var url = sysConfig.LISAPIPath + "API/LIS/GetCustomerShippingMethods";
            if (IsAll) url = url + "?enabled=true";
            if (!string.IsNullOrWhiteSpace(customerTypeId))
            {
                if (url.IndexOf('?') > 0)
                {
                    url = url + "&customerTypeId=" + customerTypeId;
                }
                else
                {
                    url = url + "?customerTypeId=" + customerTypeId;
                }
            }
            try
            {
                var list = HttpHelper.DoRequest<List<ShippingMethodModel>>(url, EnumHttpMethod.GET);
                Log.Info(list.RawValue);
                return list.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + url);
                Log.Exception(ex);
            }
            return new List<ShippingMethodModel>();
        }


        /// <summary>
        /// Add by zhengsong
        /// </summary>
        /// <param name="customerTypeId">获取客户运输方式</param>
        /// <param name="IsAll"></param>
        /// <returns></returns>
        public List<ShippingMethodModel> GetCustomerShippingMethods(string customerId, string customerTypeId, bool IsAll)
        {
            var url = sysConfig.LISAPIPath + "API/LIS/GetCustomerShippingMethodsByCustomerId";
            if (IsAll) url = url + "?enabled=true";
            if (!string.IsNullOrWhiteSpace(customerTypeId))
            {
                if (url.IndexOf('?') > 0)
                {
                    url = url + "&customerId=" + customerId + "&customerTypeId=" + customerTypeId;
                }
                else
                {
                    url = url + "?customerId=" + customerId + "&customerTypeId=" + customerTypeId;
                }
            }
            try
            {
                var list = HttpHelper.DoRequest<List<ShippingMethodModel>>(url, EnumHttpMethod.GET);
                Log.Info(list.RawValue);
                return list.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + url);
                Log.Exception(ex);
            }
            return new List<ShippingMethodModel>();
        }

        /// <summary>
        ///   add huhaiyou 2014-5-17
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="customerTypeId"></param>
        /// <param name="IsAll"></param>
        /// <returns></returns>
        public List<ShippingMethodModel> GetCustomerShippingMethodsInstoerage(string customerId, string customerTypeId,
            bool IsAll)
        {
            var url = sysConfig.LISAPIPath + "API/LIS/GetShippingMethodsByCustomerId";
            //if (IsAll) url = url + "?enabled=true";
            url = url
                .AppendUrlParameters("customerId", customerId)
				.AppendUrlParameters("customerTypeId", customerTypeId)
				.AppendUrlParametersIf("enabled", "true", IsAll);
            try
            {
                var list = HttpHelper.DoRequest<List<ShippingMethodModel>>(url, EnumHttpMethod.GET);
                Log.Info(list.RawValue);
                return list.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + url);
                Log.Exception(ex);
            }
            return new List<ShippingMethodModel>();
        }

        /// <summary>
        /// 根据运输方式类型来获取运输方式
        /// </summary>
        /// <param name="shippingMethodTypeId"></param>
        /// <returns></returns>
        public List<ShippingMethodModel> GetShippingMethodByTypeId()
        {
            return GetShippingMethodByTypeId(5); //获取类型为中邮EUB的运输方式
        }

        /// <summary>
        /// 根据运输方式类型来获取运输方式
        /// </summary>
        /// <param name="shippingMethodTypeId"></param>
        /// <returns></returns>
        public List<ShippingMethodModel> GetShippingMethodByTypeId(int shippingMethodTypeId)
        {
            var url = sysConfig.LISAPIPath + "API/LIS/GetShippingMethodsByTypeId";
            url = url + "?shippingMethodTypeId=" + shippingMethodTypeId;
            try
            {
                var model = HttpHelper.DoRequest<List<ShippingMethodModel>>(url, EnumHttpMethod.GET);
                Log.Info(model.RawValue);
                return model.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + url);
                Log.Exception(ex);
            }
            return null;
        }

        /// <summary>
        /// 获取指定的运输方式信息
        /// Add by zhengsong
        /// </summary>
        /// <param name="shippingMethodId"></param>
        /// <returns></returns>
        public ShippingMethodModel GetShippingMethod(int shippingMethodId)
        {
            var url = sysConfig.LISAPIPath + "API/LIS/GetShippingMethod";
            if (shippingMethodId != 0)
            {
                if (url.IndexOf('?') > 0)
                {
                    url = url + "&shippingMethodId=" + shippingMethodId;
                }
                else
                {
                    url = url + "?shippingMethodId=" + shippingMethodId;
                }
            }
            try
            {
                var model = HttpHelper.DoRequest<ShippingMethodModel>(url, EnumHttpMethod.GET);
                Log.Info(model.RawValue);
                return model.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + url);
                Log.Exception(ex);
            }
            return new ShippingMethodModel();
        }

        public ShippingMethodModel GetShippingMethodByCode(string code)
        {
            var url = sysConfig.LISAPIPath + "API/LIS/GetShippingMethodByCode";

            if (!string.IsNullOrWhiteSpace(code))
            {
                url = url + "?code=" + code;
            }
            try
            {
                var model = HttpHelper.DoRequest<ShippingMethodModel>(url, EnumHttpMethod.GET);
                Log.Info(model.RawValue);
                return model.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + url);
                Log.Exception(ex);
            }
            return new ShippingMethodModel();
        }

        /// <summary>
        /// 获取指定的运输方式信息
        /// </summary>
        /// <param name="shippingMethodIds">运输方式IDs</param>
        /// <returns></returns>
        public List<ShippingMethodModel> GetShippingMethodsByIds(List<int> shippingMethodIds)
        {
            var url = sysConfig.LISAPIPath + "API/LIS/PostShippingMethodsByIds";
            try
            {
                var list = HttpHelper.DoRequest<List<ShippingMethodModel>>(url, EnumHttpMethod.POST,
                                                                           EnumContentType.Json, shippingMethodIds);
                Log.Info(list.RawValue);
                return list.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + url);
                Log.Exception(ex);
            }
            return null;
        }


        /// <summary>
        /// 获取所有国家
        /// Add by zhengsong
        /// </summary>
        /// <returns></returns>
        public List<ShippingMethodCountryModel> GetCountrys()
        {
            var url = sysConfig.LISAPIPath + "API/LIS/GetCountrys";
            try
            {
                var list = HttpHelper.DoRequest<List<ShippingMethodCountryModel>>(url, EnumHttpMethod.GET);
                Log.Info(list.RawValue);
                return list.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + url);
                Log.Exception(ex);
            }
            return new List<ShippingMethodCountryModel>();
        }

        private PriceProviderResult PostVenderPrice(VenderPackageModel package)
        {
            var result = new PriceProviderResult();
            try
            {
                var list = HttpHelper.DoRequest<PriceProviderResult>(sysConfig.LISAPIPath + "API/LIS/PostVenderPrice",
                                                                     EnumHttpMethod.POST, EnumContentType.Json, package);
                result = list.Value;
                Log.Info(list.RawValue);
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LISAPIPath + "API/LIS/PostVenderPrice");
                Log.Exception(ex);
                result.CanShipping = false;
                result.Message = ex.Message;
            }
            return result;
        }

        private PriceProviderResult PostCustomerTypePrice(CustomerPackageModel package)
        {
            var result = new PriceProviderResult();
            try
            {
                var list =
                    HttpHelper.DoRequest<PriceProviderResult>(
                        sysConfig.LISAPIPath + "API/LIS/PostCustomerTypeSpecialPrice", EnumHttpMethod.POST,
                        EnumContentType.Json, package);
                result = list.Value;
                Log.Info(list.RawValue);
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LISAPIPath + "API/LIS/PostCustomerTypePrice");
                Log.Exception(ex);
                result.CanShipping = false;
                result.Message = ex.Message;
            }
            return result;
        }

        private PriceProviderResult PostCustomerPrice(CustomerInfoPackageModel package)
        {
            var result = new PriceProviderResult();
            try
            {
                var list =
                    HttpHelper.DoRequest<PriceProviderResult>(
                        sysConfig.LISAPIPath + "API/LIS/PostCustomerSpecialPrice", EnumHttpMethod.POST,
                        EnumContentType.Json, package);
                result = list.Value;
                Log.Info(list.RawValue);
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LISAPIPath + "API/LIS/PostCustomerSpecialPrice");
                Log.Exception(ex);
                result.CanShipping = false;
                result.Message = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 是否超周长
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public ResponseResult IsOverMaxGirthSingle(CustomerInfoPackageModel package)
        {
            var result = new ResponseResult();
            try
            {
                var list =
                    HttpHelper.DoRequest<ResponseResult>(
                        sysConfig.LISAPIPath + "API/LIS/IsOverMaxGirthSingle", EnumHttpMethod.POST,
                        EnumContentType.Json, package);
                result = list.Value;
                Log.Info(list.RawValue);
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LISAPIPath + "API/LIS/IsOverMaxGirthSingle");
                Log.Exception(ex);
                result.Message = ex.Message;
            }
            return result;
        }

        private List<PriceProviderResult> PostCustomerPrices(FreightPackageModel package)
        {
            var result = new List<PriceProviderResult>();
            try
            {
                var list =
                    HttpHelper.DoRequest<List<PriceProviderResult>>(
                        sysConfig.LISAPIPath + "API/LIS/PostCustomerSpecialPricesAuto", EnumHttpMethod.POST,
                        EnumContentType.Json, package);
                result = list.Value;
                Log.Info(list.RawValue);
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LISAPIPath + "API/LIS/PostCustomerSpecialPricesAuto");
                Log.Exception(ex);
                /*result.CanShipping = false;
                result.Message = ex.Message;*/
            }
            return result;
        }

        private List<PriceProviderResult> PostCustomerTypePrices(FreightPackageModel package)
        {
            var result = new List<PriceProviderResult>();
            try
            {
                var list =
                    HttpHelper.DoRequest<List<PriceProviderResult>>(
                        sysConfig.LISAPIPath + "API/LIS/PostCustomerTypeSpecialPrices", EnumHttpMethod.POST,
                        EnumContentType.Json, package);
                result = list.Value;
                Log.Info(list.RawValue);
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + sysConfig.LISAPIPath + "API/LIS/PostCustomerTypeSpecialPrices");
                Log.Exception(ex);
                /*result.CanShipping = false;
                result.Message = ex.Message;*/
            }
            return result;
        }

        /// <summary> 提交跟踪号请求服务抓取跟踪信息
        /// Add By zhengsong
        /// Time:2014-06-09
        /// </summary>
        /// <param name="orderTrackingRequests"></param>
        /// <returns></returns>
        public int AddOutTrackingInfo(List<OrderTrackingRequestModel> orderTrackingRequests)
        {
            var url = sysConfig.TISAPIPath + "API/OrderTracking/Add";
            try
            {
                var model = HttpHelper.DoRequest<int>(url, EnumHttpMethod.POST, EnumContentType.Json,
                                                      orderTrackingRequests);
                Log.Info(model.RawValue);
                return model.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + url);
                Log.Exception(ex);
                throw;
            }
        }

        /// <summary> 根据跟踪号获取跟踪信息
        /// Add By zhengsong
        /// Time:2014-06-10
        /// </summary>
        /// <param name="trackingNumberList"></param>
        /// <returns></returns>
        public List<OrderTrackingModel> GetOutTrackingInfoList(string trackingNumberList)
        {
            string[] trackingNumbers = trackingNumberList.Split(',');
            var url = sysConfig.TISAPIPath + "API/OrderTracking/GetList";
            try
            {
                var model = HttpHelper.DoRequest<List<OrderTrackingModel>>(url, EnumHttpMethod.POST, EnumContentType.Json, trackingNumbers);
                Log.Info(model.RawValue);
                return model.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + url);
                Log.Exception(ex);
                throw;
            }
        }
        public OrderTrackingModel GetOutTrackingInfo(string trackingNumber)
        {
            var url = sysConfig.TISAPIPath + "API/OrderTracking/Get?trackingNumber=";
            try
            {
                var model = HttpHelper.DoRequest<OrderTrackingModel>(url + trackingNumber, EnumHttpMethod.POST, EnumContentType.Json);
                Log.Info(model.RawValue);
                return model.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + url);
                Log.Exception(ex);
                throw;
            }
        }

		/// <summary>
		/// 获取客户可用的关税预付运输方式信息
		/// yungchu
		/// </summary>
		/// <param name="customerCode"></param>
		/// <returns></returns>
	    public List<TariffPrepayFeeShippingMethod> GetShippingMethodsTariffPrepay(string customerCode)
		{
			var url = sysConfig.LISAPIPath + "API/LIS/GetShippingMethodsTariffPrepayByCustomerCode";
			url = url + "?customerCode=" + customerCode;
			try
			{
				var model = HttpHelper.DoRequest<List<TariffPrepayFeeShippingMethod>>(url, EnumHttpMethod.GET);
				Log.Info(model.RawValue);
				return model.Value;
			}
			catch (Exception ex)
			{
				Log.Error("错误地址：" + url);
				Log.Exception(ex);
			    throw;
			}
		}
		/// <summary>
		/// 根据customerCode获取lis客户信息
		/// yungchu
		/// </summary>
		/// <param name="customerCode"></param>
		/// <returns></returns>
		public List<Customer> GetListCustomers(string customerCode)
		{
			var url = sysConfig.LISAPIPath + "API/LIS/GetCustomerList";

			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection.Add("keyWord", customerCode);
			url = url.AppendUrlParameters(nameValueCollection);

			//url = url + "?keyWord=" + customerCode;
			try
			{
				var model = HttpHelper.DoRequest<List<Customer>>(url, EnumHttpMethod.GET);
				Log.Info(model.RawValue);
				return model.Value;
			}
			catch (Exception ex)
			{
				Log.Error("错误地址：" + url);
				Log.Exception(ex);
			}
			return null;
		}
		/// <summary>
		/// 更新的用户同步到lis客户表
		/// </summary>
		/// <returns></returns>
		public bool UpdateCustomerInfoToLis(CustomerInfoParam customerInfoParam)
	    {
			var url = sysConfig.LISAPIPath + "API/LIS/PostUpdateCustomerInfoFromLms";

			try
			{
				var model = HttpHelper.DoRequest<bool>(url, EnumHttpMethod.POST, EnumContentType.Json, customerInfoParam);
				Log.Info(model.RawValue);
				return model.Value;
			}
			catch (Exception ex)
			{
				Log.Error("错误地址：" + url);
				Log.Exception(ex);
			}
			return false;
	    }

        //福州邮政发送API
        //Add By zhengsong
        //Time:2014-09-23
        public ResponseResult AddWayBillZONGTENG(string wayBill)
        {
            var Result = new ResponseResult();
            var url = sysConfig.ZONGTENGPath;
            try
            {
                var model = HttpHelper.DoRequest<XmlHelper>(url, EnumHttpMethod.POST, EnumContentType.String, wayBill);

                if (model != null && model.RawValue != null)
                {
                    var rawvalue = new XmlDocument();

                    rawvalue.LoadXml(model.RawValue);

                    if (rawvalue.GetElementsByTagName("success")[0].InnerText.Trim() == "true")
                    {
                        Result.Result = true;
                    }
                    else if (rawvalue.GetElementsByTagName("success")[0].InnerText.Trim() == "false")
                    {
                        Result.Result = false;
                        Result.Message = rawvalue.GetElementsByTagName("reason")[0].InnerText.Trim();
                    }
                    else
                    {
                        Result.Result = false;
                        Result.Message = "没有返回是否成功结果";
                    }
                }
                else
                {
                    Result.Result = false;
                    Result.Message = "未找到返回结果";
                }

                //Log.Info(model.RawValue);
                return Result;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + url);
                Log.Exception(ex);
            }
            return Result;
        }

        /// <summary>
        /// Add By zhengsong
        /// 判读是否是偏远地址
        /// Time:2014-11-27
        /// </summary>
        /// <param name="shippingMethodId"></param>
        /// <param name="countryCode"></param>
        /// <param name="shippingCity"></param>
        /// <param name="shippingZip"></param>
        /// <returns></returns>
        public bool IsRemoteArea(int? shippingMethodId, string countryCode, string shippingCity,
                                           string shippingZip)
        {
            try
            {
                var url = sysConfig.LISAPIPath + "API/LIS/IsRemoteArea?shippingMethodId=" + shippingMethodId + "&countryCode=" + countryCode + "&shippingCity=" + shippingCity
                    + "&shippingZip=" + shippingZip;

                var model = HttpHelper.DoRequest<bool>(url, EnumHttpMethod.GET);
                Log.Info(model.RawValue);
                return model.Value;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }


		//获取偏远地址 add by yungchu
		public List<RemoteAreaAddressExt> GetPagedListRemoteAreaAddress(RemoteAreaAddressParam param)
		{
			var url = sysConfig.LISAPIPath + "API/LIS/PostPagedListRemoteAreaAddress";

			var pagedListRemoteAreaAddressExt = new List<RemoteAreaAddressExt>();


			try
			{
				var model = HttpHelper.DoRequest<List<RemoteAreaAddressExt>>(url, EnumHttpMethod.POST, EnumContentType.Json, param);
				Log.Info(model.RawValue);
				pagedListRemoteAreaAddressExt = model.Value;
				return pagedListRemoteAreaAddressExt;
			}
			catch (Exception ex)
			{
				Log.Error("错误地址：" + url);
				Log.Exception(ex);
			}

			return pagedListRemoteAreaAddressExt;
		}

    }






    ///// <summary>
            ///// Add  by zhengsong
            ///// </summary>
            ///// <param name="customerModel"></param>
            ///// <returns></returns>
            //public ResponseResult PostAddCustomer(CustomerModel customerModel)
            //{
            //    ResponseResult model = new ResponseResult();
            //    try
            //    {
            //        var list = HttpHelper.DoRequest<ResponseResult>(sysConfig.LISWebApi + "API/LIS/PostAddCustomer", EnumHttpMethod.POST, EnumContentType.Json, customerModel);
            //        Log.Info(list.RawValue);
            //        model.Result = true;
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.Error("错误地址：" + sysConfig.LISWebApi + "API/LIS/PostAddCustomer");
            //        Log.Exception(ex);
            //        model.Result = false;
            //    }
            //    return model;
            //}
}
