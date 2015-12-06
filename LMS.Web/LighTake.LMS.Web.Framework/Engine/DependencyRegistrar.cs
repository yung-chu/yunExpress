using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Integration.Mvc;
using LMS.Core;
using LMS.Data.Context;
using LMS.Services.BillingServices;
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
using LMS.Services.FubServices;
using LMS.Services.HomeServices;
using LMS.Services.InStorageServices;
using LMS.Services.NewServices;
using LMS.Services.OperateLogServices;
using LMS.Services.OrderServices;
using LMS.Services.OutStorageServices;
using LMS.Services.ReturnGoodsServices;
using LMS.Services.SettlementServices;
using LMS.Services.TrackingNumberServices;
using LMS.Services.UserServices;
using LMS.Services.WayBillTemplateServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Configuration;
using LighTake.Infrastructure.Common.InversionOfControl.Auotfac;
using LighTake.Infrastructure.Common.TypeAdapter;
using LighTake.Infrastructure.Common.TypeAdapter.AutoMapper;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Seedwork.EF;
using LighTake.Infrastructure.Web;
using LighTake.Infrastructure.Web.UI;
using LighTake.Infrastructure.Common.Logging;

namespace LighTake.LMS.Web.Framework.Engine
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            if (RunModel.GetInstance().Model.Value == RunModelEnum.Release)
            {
                builder.RegisterType<SsoAuthenticationService>().As<IAuthenticationService>().InstancePerHttpRequest();
            }
            else
            {
                builder.RegisterType<FakeAuthenticationService>().As<IAuthenticationService>().InstancePerHttpRequest();
            }

            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerHttpRequest();
            builder.RegisterType<PageTitleBuilder>().As<IPageTitleBuilder>().InstancePerHttpRequest();

            var assemblies = typeFinder.GetAssemblies().ToArray();

            //Log.Info("assemblies.Count : " + assemblies.Count().ToString());
            //StringBuilder sb = new StringBuilder();
            //foreach (var s in assemblies)
            //{
            //    sb.AppendFormat("{0}{1}", s.FullName, Environment.NewLine);
            //}
            //Log.Info(sb.ToString());

            builder.RegisterControllers(assemblies);
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();


            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerHttpRequest();


            builder.RegisterType<AutomapperTypeAdapterFactory>().As<ITypeAdapterFactory>().SingleInstance();
            builder.RegisterType<LMS_DbContext>().InstancePerHttpRequest();
            builder.RegisterType<WorkContext>().As<IWorkContext>().InstancePerHttpRequest();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerHttpRequest();
            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerHttpRequest();
            builder.RegisterType<CustomerOrderService>().As<ICustomerOrderService>().InstancePerHttpRequest();
            builder.RegisterType<PermissionService>().As<IPermissionService>().InstancePerHttpRequest();
            builder.RegisterType<FeeManageService>().As<IFeeManageService>().InstancePerHttpRequest();
            builder.RegisterType<OrderService>().As<IOrderService>().InstancePerHttpRequest();
            builder.RegisterType<InStorageService>().As<IInStorageService>().InstancePerHttpRequest();
            builder.RegisterType<OutStorageService>().As<IOutStorageService>().InstancePerHttpRequest();
            builder.RegisterType<CountryService>().As<ICountryService>().InstancePerHttpRequest();
            builder.RegisterType<FreightService>().As<IFreightService>().InstancePerHttpRequest();
            builder.RegisterType<HomeService>().As<IHomeService>().InstancePerHttpRequest();
            builder.RegisterType<NewService>().As<INewService>().InstancePerHttpRequest();
            builder.RegisterType<TrackingNumberService>().As<ITrackingNumberService>().InstancePerHttpRequest();
            builder.RegisterType<WayBillTemplateService>().As<IWayBillTemplateService>().InstancePerHttpRequest();
            builder.RegisterType<DictionaryTypeService>().As<IDictionaryTypeService>().InstancePerHttpRequest();
            builder.RegisterType<ExpressService>().As<IExpressService>().InstancePerHttpRequest();
            builder.RegisterType<InsuredCalculationService>().As<IInsuredCalculationService>().InstancePerHttpRequest();
            builder.RegisterType<ReturnGoodsService>().As<IReturnGoodsService>().InstancePerHttpRequest();

            builder.RegisterType<FinancialService>().As<IFinancialService>().InstancePerHttpRequest();

            builder.RegisterType<EubWayBillService>().As<IEubWayBillService>().InstancePerHttpRequest();

			builder.RegisterType<OperateLogServices>().As<IOperateLogServices>().InstancePerHttpRequest();
	
            builder.RegisterType<SensitiveTypeInfoService>().As<ISensitiveTypeInfoService>().InstancePerHttpRequest();
            builder.RegisterType<GoodsTypeService>().As<IGoodsTypeService>().InstancePerHttpRequest();
            builder.RegisterType<SettlementService>().As<ISettlementService>().InstancePerHttpRequest();
            builder.RegisterType<BillingService>().As<IBillingService>().InstancePerHttpRequest();
            builder.RegisterType<FubService>().As<IFubService>().InstancePerHttpRequest();
        }


        public int Order
        {
            get { return 0; }
        }
    }
}
