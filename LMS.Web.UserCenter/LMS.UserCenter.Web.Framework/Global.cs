using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using FluentValidation;
using FluentValidation.Mvc;
using LighTake.Infrastructure.Common.Caching;
using LighTake.Infrastructure.Common.InversionOfControl;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Common.TypeAdapter;
using LighTake.Infrastructure.Web;
using Cache = LighTake.Infrastructure.Common.Caching.Cache;

namespace LMS.UserCenter.Web.Framework
{
    public class Global : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());

        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.IgnoreRoute("{resource}.ashx/{*pathInfo}");
            routes.IgnoreRoute("{*BarCodeHandler}", new { BarCodeHandler = @"(.*/)?barcode.ashx(/.*)?" });
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //主页路由
            routes.MapLowerCaseUrlRoute(
               "HomePage", // 路由名称
               "account/index",
               new { controller = "Account", action = "Index" } // 参数默认值
           );

            //登陆路由
            routes.MapLowerCaseUrlRoute(
               "Login", // 路由名称
               "account/login",
               new { controller = "Account", action = "Login" } // 参数默认值
           );

            routes.MapLowerCaseUrlRoute(
              "Default", // 路由名称
              "{controller}/{action}/{id}", // 带有参数的 URL
              new { controller = "Account", action = "Index", id = UrlParameter.Optional } // 参数默认值
          );

        }

        protected void Initialize()
        {
            //initialize engine context
            EngineContext.Initialize(false);

            //set dependency resolver
            var dependencyResolver = new MVCDependencyResolver();
            DependencyResolver.SetResolver(dependencyResolver);

            // initialize cache
            Cache.InitializeWith(new CacheProviderFactory(ConfigurationManager.AppSettings["CacheProvider"]));


            ////initialize AutoMapper
            //Mapper.Initialize(x => x.AddProfile<AutoMapperProfile>());

            var typeAdapterFactory = EngineContext.Current.Resolve<ITypeAdapterFactory>();
            TypeAdapterFactory.SetCurrent(typeAdapterFactory);

            //Mapper.AssertConfigurationIsValid();

            ConfigureFluentValidation();
        }

        protected void ConfigureFluentValidation()
        {
            //            // 设置 FluentValidation 默认的资源文件提供程序 - 中文资源
            //            ValidatorOptions.ResourceProviderType = typeof(FluentValidationResource);

            /* 比如验证用户名 not null、not empty、length(2,int.MaxValue) 时，链式验证时，如果第一个验证失败，则停止验证 */
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure; // ValidatorOptions.CascadeMode 默认值为：CascadeMode.Continue

            // 配置 FluentValidation 模型验证为默认的 ASP.NET MVC 模型验证
            FluentValidationModelValidatorProvider.Configure();
        }


        protected void Application_BeginRequest()
        {

        }

        protected void Application_Start()
        {

            Initialize();

            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
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