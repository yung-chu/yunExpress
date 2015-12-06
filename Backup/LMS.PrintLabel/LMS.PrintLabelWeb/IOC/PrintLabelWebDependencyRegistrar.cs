using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Integration.Mvc;
using LMS.Core;
using LMS.Services.CountryServices;
using LMS.Services.CustomerServices;
using LMS.Services.FreightServices;
using LMS.Services.WayBillTemplateServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.InversionOfControl.Auotfac;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Seedwork.EF;

namespace LMS.PrintLabelWeb.IOC
{
    public class PrintLabelWebDependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            var dataAccess = typeFinder.GetAssemblies().ToArray();
            builder.RegisterAssemblyTypes(dataAccess)
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<Data.Context.LMS_DbContext>().InstancePerHttpRequest();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerHttpRequest();
            builder.RegisterType<WorkContext>().As<IWorkContext>().InstancePerHttpRequest();
            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerHttpRequest();
            builder.RegisterType<FreightService>().As<IFreightService>().InstancePerHttpRequest();
            builder.RegisterType<WayBillTemplateService>().As<IWayBillTemplateService>().InstancePerHttpRequest();
            builder.RegisterType<CountryService>().As<ICountryService>().InstancePerHttpRequest();
        }

        public int Order { get; private set; }
    }
}