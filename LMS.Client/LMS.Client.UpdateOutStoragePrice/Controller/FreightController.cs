using System;
using LMS.Client.UpdateOutStoragePrice.Model;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Http;
using LighTake.Infrastructure.Http.Infrastructure;

namespace LMS.Client.UpdateOutStoragePrice.Controller
{
    public class FreightController
    {
        private static readonly string _baseLISUrl = System.Configuration.ConfigurationManager.AppSettings["LISUrl"].ToString();
        /// <summary>
        /// 获取成本价
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public static PriceProviderResult PostVenderPrice(VenderInfoPackageRequest package)
        {
            var result = new PriceProviderResult();
            try
            {
                var list = HttpHelper.DoRequest<PriceProviderResult>(_baseLISUrl + "API/LIS/PostVenderPriceAuto",
                                                                     EnumHttpMethod.POST, EnumContentType.Json, package);
                result = list.Value;
                Log.Info(list.RawValue);
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + _baseLISUrl + "API/LIS/PostVenderPriceAuto");
                Log.Exception(ex);
                result.CanShipping = false;
                result.Message = "";
            }
            return result;
        }
        /// <summary>
        /// 获取运输方式
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static ShippingMethodList GetShippingMethodList(DateTime dt)
        {
            string timestamp = dt.ConvertToUTCString();
            var result = new ShippingMethodList();
            try
            {
                var list = HttpHelper.DoRequest<ShippingMethodList>(_baseLISUrl + "API/LIS/GetShippingMethodList",
                                                                     EnumHttpMethod.GET, EnumContentType.Json, timestamp);
                result = list.Value;
                Log.Info(list.RawValue);
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + _baseLISUrl + "API/LIS/GetShippingMethodList");
                Log.Exception(ex);
            }
            return result;
        }
    }
}
