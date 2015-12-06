using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Integration.WebApi;
using LMS.Core;
using LMS.Services.CustomerServices;
using LMS.Services.FreightServices;
using LMS.Services.LabelPrintWebAPIServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.InversionOfControl.Auotfac;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Seedwork.EF;

namespace LMS.GetLabelPrint.WebAPI.IOC
{
    public class LMSLabelPrintDependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterApiControllers(typeFinder.GetAssemblies().ToArray());
            builder.RegisterType<Data.Context.LMS_DbContext>().InstancePerApiRequest();

            builder.RegisterAssemblyTypes(typeFinder.GetAssemblies().ToArray())
                    .Where(t => t.Name.EndsWith("Repository"))
                    .AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerApiRequest();
            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerApiRequest();
            builder.RegisterType<FreightService>().As<IFreightService>().InstancePerApiRequest();
            builder.RegisterType<WorkContext>().As<IWorkContext>().InstancePerApiRequest();
            builder.RegisterType<LabelPrintWebApiService>().As<ILabelPrintWebApiService>().InstancePerApiRequest();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}