using LighTake.Infrastructure.Common.Caching;
using LighTake.LMS.Web.Framework.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Common.Configuration;
//using LighTake.Infrastructure.Common.TypeAdapter;
using LMS.Services.UserServices;
using System.Configuration;
using System.Threading;
using LighTake.Infrastructure.CommonQueue;
using LighTake.Infrastructure.Common;
using LMS.Data.Entity;
using LMS.Services.InStorageServices;
using LighTake.Infrastructure.Common.Logging;
using LMS.WinForm.InversionOfControl;

namespace LMS.Client.InStorage
{
    class Program
    {
        const string WAYBILL_INSTORAGE_QUEUENAME = "WayBillInStorageQueue";
        const string RABBITMQ_CONFIGKEY = "lms";

        static void Main(string[] args)
        {
            Initialize();

            while (true)
            {
                InStorageSyncHandler();

                Thread.Sleep(3 * 1000);
            }
        }

        static void InStorageSyncHandler()
        {
            while (true)
            {
                string taskJson = string.Empty;
                try
                {
                    Log.Info("获取任务中..");
                    taskJson = QueueHelper.Dequeue(WAYBILL_INSTORAGE_QUEUENAME, RABBITMQ_CONFIGKEY);
                    if (string.IsNullOrWhiteSpace(taskJson))
                    {
                        Log.Info("没有任务.");
                        return;
                    }
                    var task = SerializeUtil.FromJson<Task>(taskJson);
                    var service = EngineContext.Current.Resolve<IInStorageService>();
                    service.CreateInStorageAsync(task);
                    Log.Info(string.Format("异步处理完成,运单号:{0}", task.TaskKey));
                }
                catch (Exception ex)
                {
                    var x = ex.InnerException == null ? ex : ex.InnerException;
                    Log.Error(x.ToString() + "<br/>" + taskJson);
                }
            }
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
                Cache.InitializeWith(new CacheProviderFactory(ConfigurationManager.AppSettings["CacheProvider"]));
                //分布式缓存(Memcached Or Redis)
                DistributedCache.InitializeWith(new CacheProviderFactory(ConfigurationManager.AppSettings["DistributedCacheProvider"]));
                Config.SetSystemCode("S012");

                //var typeAdapterFactory = EngineContext.Current.Resolve<ITypeAdapterFactory>();
                //TypeAdapterFactory.SetCurrent(typeAdapterFactory);
            }
            catch (Exception ex)
            {
                Log.Exception(ex.InnerException == null ? ex : ex.InnerException);
            }
        }
    }
}
