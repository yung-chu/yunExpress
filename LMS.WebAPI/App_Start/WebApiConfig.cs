using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Autofac.Integration.WebApi;
using LighTake.Infrastructure.Common.InversionOfControl;
using LighTake.Infrastructure.Http.Filters;

namespace LMS.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Filters.Add(new LogInfoFilter());
            config.Filters.Add(new LogExceptionFilter());

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            config.DependencyResolver = new AutofacWebApiDependencyResolver(
                EngineContext.Current.ContainerManager.Container
                );

            config.Routes.MapHttpRoute(
               name: "DefaultApi",
               routeTemplate: "api/{controller}/{id}",
               constraints:new {controller="orders"},
               defaults: new { id = RouteParameter.Optional }
               );
            config.Routes.MapHttpRoute(
                name: "",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new {id = RouteParameter.Optional}
                );
           
          
        }
    }
}