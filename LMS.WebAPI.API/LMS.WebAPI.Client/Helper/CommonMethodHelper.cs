using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using Autofac;
using Autofac.Integration.WebApi;
using LMS.Data.Entity;
using LMS.Services.CustomerServices;
using LMS.Services.FreightServices;
using LighTake.Infrastructure.Common.Caching;
using LighTake.Infrastructure.Common.InversionOfControl;
using System.Web.Http;

namespace LMS.WebAPI.Client.Helper
{
    public class CommonMethodHelper
    {
        
        /// <summary>
        ///  运输方式是否启用
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ShippingMethodIsEnable(string value)
        {

            var list = GetShippingMethodList();

            if (list == null) return false;

            if (list.Count == 0) return false;

            return list.Any(m => m.Code == value.Trim());
        }
        /// <summary>
        /// 运输方式是否带跟踪号
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ShippingMethodHaveTrackingNum(string value)
        {

            var list = GetShippingMethodList();

            if (list == null) return false;

            if (list.Count == 0) return false;
            var item = list.FirstOrDefault(p => p.Code == value);
            if (null != item) return item.HaveTrackingNum;
            return false;
        }

        /// <summary>
        /// 运输方式是否系统分配跟踪号
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ShippingMethodIsSysTrackNumber(string value)
        {

            var list = GetShippingMethodList();

            if (list == null) return false;

            if (list.Count == 0) return false;
            var item = list.FirstOrDefault(p => p.Code == value);
            if (null != item) return item.IsSysTrackNumber;// 在LIS数据库在运输方式表添加一个IsSysTrackNumber （系统分配跟踪号）字段
            return false;
        }

        /// <summary>
        /// 获取运输方式ID
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static int GetShippingMethodId(string code)
        {
            var list = GetShippingMethodList();
            if (list == null) return 0;
            if (list.Count == 0) return 0;
            var item = list.FirstOrDefault(p => p.Code == code);
            if (null != item) return item.ShippingMethodId;// 
            return 0;
        }

        /// <summary>
        /// 获取运输方式名称
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetShippingMethodName(string code)
        {
            var list = GetShippingMethodList();
            if (list == null) return "";
            if (list.Count == 0) return "";
            var item = list.FirstOrDefault(p => p.Code == code);
            if (null != item) return item.FullName;// 
            return "";
        }

        public static Data.Entity.ShippingMethodModel GetShippingMethodInfo(string code)
        {
            var list = GetShippingMethodList();
            if (list == null) return null;
            if (list.Count == 0) return null;
            var item = list.FirstOrDefault(p => p.Code == code);
            return item;
        }

        private static List<Data.Entity.ShippingMethodModel> GetShippingMethodList()
        {
            //var list = Cache.Get("cache_ShippingMethods");
            //if (list == null)
            //{
            var freightService = GlobalConfiguration.Configuration.DependencyResolver.BeginScope().GetService(typeof(IFreightService)) as FreightService;
            if (freightService == null)
            {
                throw new NullReferenceException("GetShippingMethodList");
            }
                var methods = freightService.GetShippingMethods(null, false);
                if (methods != null && methods.Count > 0)
                {
                   // Cache.Add("cache_ShippingMethods", methods);
                    return methods;
                }
                return null;
            //}

            //return list as List<Data.Entity.ShippingMethodModel>;
        }

        public static bool CountryCodeIsEnable(string value)
        {
            var list = GetCountryList();
            if (list == null) return false;
            if (list.Count == 0) return false;
            return list.Any(m => m.CountryCode == value.Trim());
        }

        private static List<ShippingMethodCountryModel> GetCountryList()
        {
            //var list = Cache.Get("cache_CountryMethods");
            //if (list == null)
            //{
                var service = GlobalConfiguration.Configuration.DependencyResolver.BeginScope().GetService(typeof(IFreightService)) as FreightService;
                if (service == null)
                {
                    throw new NullReferenceException("GetCountryList");
                }
                //var service = EngineContext.Current.Resolve<IFreightService>();
                var countrys = service.GetCountrys();
                if (countrys != null && countrys.Count > 0)
                {
                    //Cache.Add("cache_CountryMethods", countrys);
                    return countrys;
                }
                return null;
            //}
            //return list as List<ShippingMethodCountryModel>;
        }

    }
}