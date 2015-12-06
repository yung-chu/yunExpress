using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using LMS.Services.FreightServices;
using LMS.Services.OrderServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LighTake.Infrastructure.Common.InversionOfControl;
using LighTake.Infrastructure.Common.Caching;
using System.Configuration;
using LighTake.Infrastructure.Common.TypeAdapter;
using LMS.Services.UserServices;


namespace LMS.PostalAPI.Controllers
{
    [TestClass]
    public class TestController
    {
        private readonly IFreightService _freightService;
        private readonly IOrderService _orderService;

        public TestController()
        {
            EngineContext.Initialize(false);

            ////set dependency resolver
            //var dependencyResolver = new GrouponDependencyResolver();
            //System.Web.Mvc.DependencyResolver.SetResolver(dependencyResolver);

           _freightService= EngineContext.Current.Resolve<IFreightService>();
           _orderService = EngineContext.Current.Resolve<IOrderService>();
            //// initialize cache
           //Cache.InitializeWith(new CacheProviderFactory(ConfigurationManager.AppSettings["CacheProvider"]));
           Config.SetSystemCode("S012");

           var typeAdapterFactory = EngineContext.Current.Resolve<ITypeAdapterFactory>();
           TypeAdapterFactory.SetCurrent(typeAdapterFactory);
        

            //_freightService = freightService;
            //_orderService = orderService;
        }

        [TestMethod]
        public void Add()
        {
            PostalController aa = new PostalController(_orderService, _freightService);

            //aa.AddPostalWayBill();
        }
    }
}