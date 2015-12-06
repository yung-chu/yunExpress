using System.Linq;
using Autofac;
using Autofac.Integration.Mvc;
using LMS.Core;
using LMS.Data.Context;
using LMS.Services.CommonServices;
using LMS.Services.CountryServices;
using LMS.Services.CustomerServices;
using LMS.Services.FreightServices;
using LMS.Services.HomeServices;
using LMS.Services.NewServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.InversionOfControl.Auotfac;
using LighTake.Infrastructure.Common.TypeAdapter;
using LighTake.Infrastructure.Common.TypeAdapter.AutoMapper;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Seedwork.EF;
using LighTake.Infrastructure.Web;
using LighTake.Infrastructure.Web.UI;
using LMS.Services.TrackingNumberServices;
using LMS.Services.TrackServices;

namespace LMS.FrontDesk.Framework
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
           

            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerHttpRequest();
            builder.RegisterType<PageTitleBuilder>().As<IPageTitleBuilder>().InstancePerHttpRequest();

            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            var dataAccess = typeFinder.GetAssemblies().ToArray();
            builder.RegisterAssemblyTypes(dataAccess)
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces().InstancePerLifetimeScope();

            

            builder.RegisterType<FormsAuthenticationService>().As<IAuthenticationService>().InstancePerHttpRequest();

            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerHttpRequest();

            builder.RegisterType<AutomapperTypeAdapterFactory>().As<ITypeAdapterFactory>().SingleInstance();
            builder.RegisterType<LMS_DbContext>().InstancePerHttpRequest();
            builder.RegisterType<WorkContext>().As<IWorkContext>().InstancePerHttpRequest();
            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerHttpRequest();
            builder.RegisterType<NewService>().As<INewService>().InstancePerHttpRequest();
            builder.RegisterType<HomeService>().As<IHomeService>().InstancePerHttpRequest();
            builder.RegisterType<GoodsTypeService>().As<IGoodsTypeService>().InstancePerHttpRequest();
            builder.RegisterType<CountryService>().As<ICountryService>().InstancePerHttpRequest();
            builder.RegisterType<FreightService>().As<IFreightService>().InstancePerHttpRequest();
			builder.RegisterType<TrackingService>().As<ITrackingService>().InstancePerHttpRequest();
			builder.RegisterType<TrackingNumberService>().As<ITrackingNumberService>().InstancePerHttpRequest();
			
        }


        public int Order
        {
            get { return 0; }
        }
    }
}
