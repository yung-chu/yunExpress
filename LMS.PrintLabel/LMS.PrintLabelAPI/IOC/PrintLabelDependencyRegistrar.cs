using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Integration.WebApi;
using LMS.Core;
using LMS.Services.CustomerServices;
using LMS.Services.FreightServices;
using LMS.Services.PrintServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.InversionOfControl.Auotfac;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Seedwork.EF;

namespace LMS.PrintLabelAPI.IOC
{
    public class PrintLabelDependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterApiControllers(typeFinder.GetAssemblies().ToArray());
            builder.RegisterType<Data.Context.LMS_DbContext>().InstancePerApiRequest();

            builder.RegisterAssemblyTypes(typeFinder.GetAssemblies().ToArray())
                    .Where(t => t.Name.EndsWith("Repository"))
                    .AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerApiRequest();
            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerApiRequest();
            builder.RegisterType<PrintService>().As<IPrintService>().InstancePerApiRequest();
            builder.RegisterType<WorkContext>().As<IWorkContext>().InstancePerApiRequest();
            builder.RegisterType<FreightService>().As<IFreightService>().InstancePerApiRequest();
        }

        public int Order { get; private set; }
    }
}