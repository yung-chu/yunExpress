using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using LMS.Client.FuZhouPostal.Controller;
using LMS.WinForm.InversionOfControl;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Common.TypeAdapter;

namespace LMS.Client.FuZhouPostal
{
    class Program
    {
        static void Main(string[] args)
        {
            Initialize();
           
            PostalController.Start();
        }
        static void Initialize()
        {
            try
            {
                EngineContext.Initialize(false);

                Log.Info("初始化成功!");
                //set dependency resolver
                //var dependencyResolver = new GrouponDependencyResolver();
                //DependencyResolver.SetResolver(dependencyResolver);

                // initialize cache
                //普通缓存(可以是EntLib、Memcached Or Redis)
                //Cache.InitializeWith(new CacheProviderFactory(ConfigurationManager.AppSettings["CacheProvider"]));
                //分布式缓存(Memcached Or Redis)
                //DistributedCache.InitializeWith(new CacheProviderFactory(ConfigurationManager.AppSettings["DistributedCacheProvider"]));
                //Config.SetSystemCode("S012");

                var typeAdapterFactory = EngineContext.Current.Resolve<ITypeAdapterFactory>();
                TypeAdapterFactory.SetCurrent(typeAdapterFactory);
            }
            catch (Exception ex)
            {
                Log.Exception(ex.InnerException == null ? ex : ex.InnerException);
            }
        }
    }
}
