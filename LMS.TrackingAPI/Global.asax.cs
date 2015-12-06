using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using LighTake.Infrastructure.Common.InversionOfControl;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Common.TypeAdapter;
using LighTake.Infrastructure.Web;

namespace LMS.TrackingAPI
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Initialize();
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void Initialize()
        {
            EngineContext.Initialize(false);
            //set dependency resolver
            var dependencyResolver = new MVCDependencyResolver();
            DependencyResolver.SetResolver(dependencyResolver);

            // initialize cache
            /*Cache.InitializeWith(new CacheProviderFactory(ConfigurationManager.AppSettings["CacheProvider"]));
            Config.SetSystemCode("S012");*/

            ////initialize AutoMapper
            //Mapper.Initialize(x => x.AddProfile<AutoMapperProfile>());

            var typeAdapterFactory = EngineContext.Current.Resolve<ITypeAdapterFactory>();
            TypeAdapterFactory.SetCurrent(typeAdapterFactory);

            //ConfigureFluentValidation();
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