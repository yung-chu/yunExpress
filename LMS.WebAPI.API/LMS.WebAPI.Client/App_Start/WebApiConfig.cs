using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Autofac.Integration.WebApi;
using LMS.Core;
using LMS.Services.CustomerServices;
using LMS.WebAPI.Client.Handler;
using LighTake.Infrastructure.Common.InversionOfControl;
using LighTake.Infrastructure.Http.Filters;

namespace LMS.WebAPI.Client
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
             
            
            config.MessageHandlers.Add(new HttpAuthenticationHandler());


            config.Filters.Add(new LogInfoFilter());
            config.Filters.Add(new LogExceptionFilter());
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            config.DependencyResolver = new AutofacWebApiDependencyResolver(
                                                EngineContext.Current.ContainerManager.Container
                                            );
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            config.Routes.MapHttpRoute(
              name: "ControllerAction",
              routeTemplate: "api/{controller}/{action}/{id}",
              defaults: new { id = RouteParameter.Optional },
              constraints: null
          );

            

            
        }
    }
}
