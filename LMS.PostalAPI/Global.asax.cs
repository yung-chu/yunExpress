using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.SessionState;
using LighTake.Infrastructure.Common.InversionOfControl;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Common.TypeAdapter;
using LighTake.Infrastructure.Web;

namespace LMS.PostalAPI
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            Initialize();
            //GlobalConfiguration.Configuration.Services.Add(typeof(System.Web.Http.Validation.ModelValidatorProvider), new WebApiFluentValidationModelValidatorProvider());
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.UseXmlSerializer = true;
            WebApiConfig.Register(GlobalConfiguration.Configuration);
        }

        private void Initialize()
        {
            EngineContext.Initialize(false);
            //set dependency resolver
            var dependencyResolver = new MVCDependencyResolver();
            DependencyResolver.SetResolver(dependencyResolver);

            // initialize cache
            //Cache.InitializeWith(new CacheProviderFactory(ConfigurationManager.AppSettings["CacheProvider"]));


            ////initialize AutoMapper
            //Mapper.Initialize(x => x.AddProfile<AutoMapperProfile>());

            var typeAdapterFactory = EngineContext.Current.Resolve<ITypeAdapterFactory>();
            TypeAdapterFactory.SetCurrent(typeAdapterFactory);

        }


        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();

            //if (ex is ThreadAbortException)
            //    return;

            Log.Exception(ex);
        }

    }
}