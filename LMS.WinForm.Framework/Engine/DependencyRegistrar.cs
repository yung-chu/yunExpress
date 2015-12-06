using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Integration.Mvc;
using LMS.Core;
using LMS.Data.Context;
using LMS.Services.CommonServices;
using LMS.Services.CountryServices;
using LMS.Services.CustomerOrderServices;
using LMS.Services.CustomerServices;
using LMS.Services.DictionaryTypeServices;
using LMS.Services.EubWayBillServices;
using LMS.Services.ExpressServices;
using LMS.Services.FeeManageServices;
using LMS.Services.FinancialServices;
using LMS.Services.FreightServices;
using LMS.Services.HomeServices;
using LMS.Services.InStorageServices;
using LMS.Services.NewServices;
using LMS.Services.OrderServices;
using LMS.Services.OutStorageServices;
using LMS.Services.ReturnGoodsServices;
using LMS.Services.TrackingNumberServices;
using LMS.Services.UserServices;
using LMS.Services.WayBillTemplateServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Configuration;
//using LighTake.Infrastructure.Common.InversionOfControl.Auotfac;
using LighTake.Infrastructure.Common.TypeAdapter;
using LighTake.Infrastructure.Common.TypeAdapter.AutoMapper;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Seedwork.EF;
using LighTake.Infrastructure.Web;
using LighTake.Infrastructure.Web.UI;
using LighTake.Infrastructure.Common.Logging;
using LMS.WinForm.InversionOfControl.Autofac;
using LMS.Services.OperateLogServices;

namespace LighTake.LMS.Web.Framework.Engine
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            if (RunModel.GetInstance().Model.Value == RunModelEnum.Release)
            {
                builder.RegisterType<SsoAuthenticationService>().As<IAuthenticationService>().InstancePerLifetimeScope();
            }
            else
            {
                builder.RegisterType<FakeAuthenticationService>().As<IAuthenticationService>().InstancePerLifetimeScope();
            }

            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerLifetimeScope();
            builder.RegisterType<PageTitleBuilder>().As<IPageTitleBuilder>().InstancePerLifetimeScope();

            var assemblies = typeFinder.GetAssemblies().ToArray();

//#if DEBUG
//            Log.Info("assemblies.Count : " + assemblies.Count().ToString());
//            StringBuilder sb = new StringBuilder();
//            foreach (var s in assemblies)
//            {
//                sb.AppendFormat("{0}{1}", s.FullName.Split(',')[0], "<br/>");
//            }
//            Log.Info(sb.ToString()); 
//#endif

            builder.RegisterControllers(assemblies);
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();


            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();


            builder.RegisterType<AutomapperTypeAdapterFactory>().As<ITypeAdapterFactory>().SingleInstance();
            builder.RegisterType<LMS_DbContext>().InstancePerLifetimeScope();
            builder.RegisterType<WorkContext>().As<IWorkContext>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerOrderService>().As<ICustomerOrderService>().InstancePerLifetimeScope();
            builder.RegisterType<PermissionService>().As<IPermissionService>().InstancePerLifetimeScope();
            builder.RegisterType<FeeManageService>().As<IFeeManageService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderService>().As<IOrderService>().InstancePerLifetimeScope();
            builder.RegisterType<InStorageService>().As<IInStorageService>().InstancePerLifetimeScope();
            builder.RegisterType<OutStorageService>().As<IOutStorageService>().InstancePerLifetimeScope();
            builder.RegisterType<CountryService>().As<ICountryService>().InstancePerLifetimeScope();
            builder.RegisterType<FreightService>().As<IFreightService>().InstancePerLifetimeScope();
            builder.RegisterType<HomeService>().As<IHomeService>().InstancePerLifetimeScope();
            builder.RegisterType<NewService>().As<INewService>().InstancePerLifetimeScope();
            builder.RegisterType<TrackingNumberService>().As<ITrackingNumberService>().InstancePerLifetimeScope();
            builder.RegisterType<WayBillTemplateService>().As<IWayBillTemplateService>().InstancePerLifetimeScope();
            builder.RegisterType<DictionaryTypeService>().As<IDictionaryTypeService>().InstancePerLifetimeScope();
            builder.RegisterType<ExpressService>().As<IExpressService>().InstancePerLifetimeScope();
            builder.RegisterType<InsuredCalculationService>().As<IInsuredCalculationService>().InstancePerLifetimeScope();
            builder.RegisterType<ReturnGoodsService>().As<IReturnGoodsService>().InstancePerLifetimeScope();

            builder.RegisterType<FinancialService>().As<IFinancialService>().InstancePerLifetimeScope();

            builder.RegisterType<EubWayBillService>().As<IEubWayBillService>().InstancePerLifetimeScope();

            builder.RegisterType<OperateLogServices>().As<IOperateLogServices>().InstancePerLifetimeScope();
        }


        public int Order
        {
            get { return 0; }
        }
    }
}
