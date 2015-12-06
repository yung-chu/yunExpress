using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using LighTake.Infrastructure.Common.InversionOfControl;
using LighTake.Infrastructure.Web;

namespace LMS.GetLabelPrint.WebAPI
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            EngineContext.Initialize(false);
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            //var dependencyResolver = new MVCDependencyResolver();
            //DependencyResolver.SetResolver(dependencyResolver);

        }
    }
}