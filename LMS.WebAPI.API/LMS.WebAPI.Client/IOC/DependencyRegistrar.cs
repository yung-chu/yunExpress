using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Integration.WebApi;
using LMS.Core;
using LMS.Data.Context;
using LMS.Services.CountryServices;
using LMS.Services.CustomerOrderServices;
using LMS.Services.CustomerServices;
using LMS.Services.ExpressServices;
using LMS.Services.FinancialServices;
using LMS.Services.FreightServices;
using LMS.Services.OperateLogServices;
using LMS.Services.ReturnGoodsServices;
using LMS.Services.TrackingNumberServices;
using LMS.Services.OrderServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.InversionOfControl.Auotfac;
using LighTake.Infrastructure.Common.TypeAdapter;
using LighTake.Infrastructure.Common.TypeAdapter.AutoMapper;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Seedwork.EF;
using LMS.Services.TrackServices;


namespace LMS.WebAPI.Client.IOC
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterApiControllers(typeFinder.GetAssemblies().ToArray());

            builder.RegisterType<LMS_DbContext>().InstancePerApiRequest();

            builder.RegisterAssemblyTypes(typeFinder.GetAssemblies().ToArray())
                   .Where(t => t.Name.EndsWith("Repository"))
                   .AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<AutomapperTypeAdapterFactory>().As<ITypeAdapterFactory>().SingleInstance();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerApiRequest();
           
			

            builder.RegisterType<OrderService>().As<IOrderService>().InstancePerApiRequest();
			builder.RegisterType<OperateLogServices>().As<IOperateLogServices>().InstancePerApiRequest();


            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerApiRequest();
            builder.RegisterType<CustomerOrderService>().As<ICustomerOrderService>().InstancePerApiRequest();
            builder.RegisterType<FreightService>().As<IFreightService>().InstancePerApiRequest();
            builder.RegisterType<TrackingNumberService>().As<ITrackingNumberService>().InstancePerApiRequest();
            builder.RegisterType<WorkContext>().As<IWorkContext>().InstancePerApiRequest();

			builder.RegisterType<FinancialService>().As<IFinancialService>().InstancePerApiRequest();
			builder.RegisterType<ReturnGoodsService>().As<IReturnGoodsService>().InstancePerApiRequest();
			builder.RegisterType<TrackingService>().As<ITrackingService>().InstancePerApiRequest();
            builder.RegisterType<CountryService>().As<ICountryService>().InstancePerApiRequest();
            builder.RegisterType<ExpressService>().As<IExpressService>().InstancePerApiRequest();
        }

        public int Order
        {
            get { return 999; }
        }
    }
}