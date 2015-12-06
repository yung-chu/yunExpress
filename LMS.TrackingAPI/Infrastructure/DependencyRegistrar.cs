using Autofac;
using Autofac.Integration.WebApi;
using LMS.Core;
using LMS.Data.Context;
using LMS.Services.FinancialServices;
using LMS.Services.FreightServices;
using LMS.Services.OperateLogServices;
using LMS.Services.OrderServices;
using LMS.Services.ReturnGoodsServices;
using LMS.Services.TrackingNumberServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Configuration;
using LighTake.Infrastructure.Common.InversionOfControl.Auotfac;
using LighTake.Infrastructure.Common.TypeAdapter;
using LighTake.Infrastructure.Common.TypeAdapter.AutoMapper;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Seedwork.EF;
using LighTake.Infrastructure.Web;
using System.Linq;

namespace LMS.TrackingAPI.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            if (RunModel.GetInstance().Model.Value == RunModelEnum.Release)
            {
                builder.RegisterType<FormsAuthenticationService>().As<IAuthenticationService>().InstancePerApiRequest();
            }
            else
            {
                builder.RegisterType<FakeAuthenticationService>().As<IAuthenticationService>().InstancePerApiRequest();

            }


            builder.RegisterApiControllers(typeFinder.GetAssemblies().ToArray());




            builder.RegisterType<LMS_DbContext>().InstancePerApiRequest();

            builder.RegisterAssemblyTypes(typeFinder.GetAssemblies().ToArray())
                   .Where(t => t.Name.EndsWith("Repository"))
                   .AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<AutomapperTypeAdapterFactory>().As<ITypeAdapterFactory>().SingleInstance();

            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerApiRequest();
            
            builder.RegisterType<OrderService>().As<IOrderService>().InstancePerApiRequest();

            builder.RegisterType<FreightService>().As<IFreightService>().InstancePerApiRequest();
            builder.RegisterType<ReturnGoodsService>().As<IReturnGoodsService>().InstancePerApiRequest();
            builder.RegisterType<FinancialService>().As<IFinancialService>().InstancePerApiRequest();
            builder.RegisterType<TrackingNumberService>().As<ITrackingNumberService>().InstancePerApiRequest();
            
            builder.RegisterType<WorkContext>().As<IWorkContext>().InstancePerApiRequest();

            builder.RegisterType<OperateLogServices>().As<IOperateLogServices>().InstancePerApiRequest();

            

        }


        public int Order
        {
            get { return 1; }
        }
    }
}