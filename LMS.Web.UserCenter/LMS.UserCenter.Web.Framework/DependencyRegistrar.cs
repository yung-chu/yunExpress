using System.Linq;
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
using LMS.Services.FeeManageServices;
using LMS.Services.FinancialServices;
using LMS.Services.FreightServices;
using LMS.Services.InStorageServices;
using LMS.Services.OperateLogServices;
using LMS.Services.OrderServices;
using LMS.Services.ReturnGoodsServices;
using LMS.Services.TrackingNumberServices;
using LMS.Services.WayBillTemplateServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.InversionOfControl.Auotfac;
using LighTake.Infrastructure.Common.TypeAdapter;
using LighTake.Infrastructure.Common.TypeAdapter.AutoMapper;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Seedwork.EF;
using LighTake.Infrastructure.Web;
using LighTake.Infrastructure.Web.UI;
using LMS.Services.ReturnGoodsServices;

namespace LMS.UserCenter.Web.Framework
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

            builder.RegisterType<CustomerOrderService>().As<ICustomerOrderService>().InstancePerHttpRequest();

            builder.RegisterType<OrderService>().As<IOrderService>().InstancePerHttpRequest();
            builder.RegisterType<ReturnGoodsService>().As<IReturnGoodsService>().InstancePerHttpRequest();

            builder.RegisterType<CountryService>().As<ICountryService>().InstancePerHttpRequest();
            builder.RegisterType<FreightService>().As<IFreightService>().InstancePerHttpRequest();
            
            builder.RegisterType<InsuredCalculationService>().As<IInsuredCalculationService>().InstancePerHttpRequest();
            builder.RegisterType<SensitiveTypeInfoService>().As<ISensitiveTypeInfoService>().InstancePerHttpRequest();
            builder.RegisterType<GoodsTypeService>().As<IGoodsTypeService>().InstancePerHttpRequest();
            
            builder.RegisterType<BillingService>().As<IBillingService>().InstancePerHttpRequest();
            builder.RegisterType<FeeManageService>().As<IFeeManageService>().InstancePerHttpRequest();
            builder.RegisterType<TrackingNumberService>().As<ITrackingNumberService>().InstancePerHttpRequest();
            builder.RegisterType<WayBillTemplateService>().As<IWayBillTemplateService>().InstancePerHttpRequest();
            builder.RegisterType<DictionaryTypeService>().As<IDictionaryTypeService>().InstancePerHttpRequest();
            builder.RegisterType<EubWayBillService>().As<IEubWayBillService>().InstancePerHttpRequest();

            builder.RegisterType<InStorageService>().As<IInStorageService>().InstancePerHttpRequest();

            builder.RegisterType<FinancialService>().As<IFinancialService>().InstancePerHttpRequest();

            builder.RegisterType<ReturnGoodsService>().As<IReturnGoodsService>().InstancePerHttpRequest();
			builder.RegisterType<OperateLogServices>().As<IOperateLogServices>().InstancePerHttpRequest(); 
            
        }


        public int Order
        {
            get { return 0; }
        }
    }
}
