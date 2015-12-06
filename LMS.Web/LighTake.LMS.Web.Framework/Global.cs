using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using LMS.Services.UserServices;
using LighTake.Infrastructure.Common.Caching;
using LighTake.Infrastructure.Common.Configuration;
using LighTake.Infrastructure.Common.InversionOfControl;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Common.TypeAdapter;
using LighTake.Infrastructure.Web.Filters;
using LighTake.LMS.Web.Framework.Engine;

namespace LighTake.LMS.Web.Framework
{
    public class Global : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            if (RunModel.GetInstance().Model.Value == RunModelEnum.Release)
            {
                filters.Add(new SsoMemberOnlyAttribute(GetIgnoredActionMethod()));
            }
        }

        //配置不需要权限认证的路径
        private static IEnumerable<IgnoredActionMethod> GetIgnoredActionMethod()
        {
            List<IgnoredActionMethod> listIgnoredActionMethod = new List<IgnoredActionMethod>();
            listIgnoredActionMethod.Add(new IgnoredActionMethod()
                {
                    Controller = "Print",
                    Action = "*",

                });

            return listIgnoredActionMethod;
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{*BarCodeHandler}", new { BarCodeHandler = @"(.*/)?barcode.ashx(/.*)?" });
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
              "Login", // 路由名称
              "Login", // 带有参数的 URL
              new { controller = "User", action = "Login" }
            );

            routes.MapRoute(
                "HomePage", // 路由名称
                "HomePage", // 带有参数的 URL
                new { controller = "Home", action = "Index" }
            );

            routes.MapRoute(
               "AccessDenied",
               "security/accessDenied",
               new { controller = "Security", action = "AccessDenied" }
            );

            routes.MapRoute(
               "Default", // 路由名称
               "{controller}/{action}", // 带有参数的 URL
               new { controller = "User", action = "Login" } // 参数默认值
           );

        }

        protected void Initialize()
        {
            EngineContext.Initialize(false);

            //set dependency resolver
            var dependencyResolver = new GrouponDependencyResolver();
            DependencyResolver.SetResolver(dependencyResolver);

            // initialize cache
            //普通缓存(可以是EntLib、Memcached Or Redis)
            Cache.InitializeWith(new CacheProviderFactory(ConfigurationManager.AppSettings["CacheProvider"]));
            //分布式缓存(Memcached Or Redis)
            DistributedCache.InitializeWith(new CacheProviderFactory(ConfigurationManager.AppSettings["DistributedCacheProvider"]));
            Config.SetSystemCode("S012");

            var typeAdapterFactory = EngineContext.Current.Resolve<ITypeAdapterFactory>();
            TypeAdapterFactory.SetCurrent(typeAdapterFactory);
        }

        protected void Application_Start()
        {

            Initialize();

            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            MvcHandler.DisableMvcResponseHeader = true;//去掉无用的http header
        }

        protected void Application_Error(object sender, EventArgs e)
        {

            Exception ex = Server.GetLastError();

            //if (ex is ThreadAbortException)
            //    return;

            Log.Exception(ex);

            //Response.Redirect("unexpectederror.htm");
        }
    }
}
