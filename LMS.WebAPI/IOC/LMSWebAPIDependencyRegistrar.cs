using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Integration.WebApi;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Services.FeeManageServices;
using LMS.Services.FinancialServices;
using LMS.Services.OperateLogServices;
using LMS.Services.ReturnGoodsServices;
using LMS.Services.TrackingNumberServices;
using LMS.Services.UserServices;
using LMS.Services.WayBillTemplateServices;
using LMS.WebAPI;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Configuration;
using LighTake.Infrastructure.Common.InversionOfControl.Auotfac;
using LighTake.Infrastructure.Common.TypeAdapter;
using LighTake.Infrastructure.Common.TypeAdapter.AutoMapper;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Seedwork.EF;
using LMS.Services.OrderServices;
using LMS.Services.CustomerServices;
using LighTake.Infrastructure.Web;
using LMS.Services.InStorageServices;
using LMS.Services.FreightServices;

namespace LMS.WebAPI
{
    public class LMSWebAPIDependencyRegistrar:IDependencyRegistrar
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

            builder.RegisterType<Data.Context.LMS_DbContext>().InstancePerApiRequest();

            builder.RegisterAssemblyTypes(typeFinder.GetAssemblies().ToArray())
                   .Where(t => t.Name.EndsWith("Repository"))
                   .AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<AutomapperTypeAdapterFactory>().As<ITypeAdapterFactory>().SingleInstance();
            builder.RegisterGeneric(typeof (Repository<>)).As(typeof (IRepository<>)).InstancePerApiRequest();

            builder.RegisterType<OrderService>().As<IOrderService>().InstancePerApiRequest();
			builder.RegisterType<OperateLogServices>().As<IOperateLogServices>().InstancePerApiRequest();

            builder.RegisterType<UserService>().As<IUserService>().InstancePerApiRequest(); 

            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerApiRequest(); 

            builder.RegisterType<PermissionService>().As<IPermissionService>().InstancePerApiRequest();

            builder.RegisterType<FeeManageService>().As<IFeeManageService>().InstancePerApiRequest();

            builder.RegisterType<InStorageService>().As<IInStorageService>().InstancePerApiRequest();

            builder.RegisterType<FreightService>().As<IFreightService>().InstancePerApiRequest(); 

            builder.RegisterType<ReturnGoodsService>().As<IReturnGoodsService>().InstancePerApiRequest();

            builder.RegisterType<TrackingNumberService>().As<ITrackingNumberService>().InstancePerApiRequest(); 

            builder.RegisterType<WorkContext>().As<IWorkContext>().InstancePerApiRequest();

            builder.RegisterType<FinancialService>().As<IFinancialService>().InstancePerApiRequest();

            builder.RegisterType<WayBillTemplateService>().As<IWayBillTemplateService>().InstancePerApiRequest(); 

        }

        public int Order { get; private set; }
    }
}