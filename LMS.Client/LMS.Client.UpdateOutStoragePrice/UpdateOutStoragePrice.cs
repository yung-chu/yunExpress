using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amib.Threading;
using LMS.Client.UpdateOutStoragePrice.Controller;
using LMS.Client.UpdateOutStoragePrice.Model;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.UpdateOutStoragePrice
{
    public class UpdateOutStoragePrice
    {
        private static string path = AppDomain.CurrentDomain.BaseDirectory + System.Configuration.ConfigurationManager.AppSettings["LogLastTimePath"].ToString();
        private static string ismultithreading = System.Configuration.ConfigurationManager.AppSettings["IsMultiThreading"].ToString();
        private static int maxThreads = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["MaxThreads"].ToString());
        private static int minThreads = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["MinThreads"].ToString());

        public static void UpdatePrice()
        {
            var datenow = DateTime.Now;
            var lastupdated = WayBillController.GetLastUpdateTime(path);
            var shippinglist = FreightController.GetShippingMethodList(DateTime.Parse(lastupdated));
            if (shippinglist != null && shippinglist.List != null && shippinglist.List.Count > 0)
            {
                Log.Info("开始同步运输方式!");
                shippinglist.List.ForEach(p => WayBillController.SynchronousShippingMethod(p.ShippingMethodId,p.ShippingMethodTypeId,p.Enabled));
                Log.Info("完成同步运输方式!");
            }
            var list = WayBillController.GetUpdatePriceWayBillList(lastupdated);
            //更新最够更新时间
            LogTime.WriteFile(path, datenow.ToString("yyyy-MM-dd HH:mm:ss"));
            Log.Info(datenow.ToString("yyyy-MM-dd HH:mm:ss"));

            if (list == null || list.Count <= 0) return;
            if (ismultithreading.ToLowerInvariant() == "no")
            {
                //单线程
                foreach (var venderPackageModel in list)
                {
                    if (!WayBillController.UpdatePriceWayBill(FreightController.PostVenderPrice(venderPackageModel),
                                                         venderPackageModel.WayBillNumber))
                    {
                        Log.Error(string.Format("运单号：{0}更新错误！", venderPackageModel.WayBillNumber));
                    }
                }
            }
            else if(ismultithreading.ToLowerInvariant()=="yes")
            {
                //多线程
                if (minThreads > maxThreads)
                {
                    maxThreads = minThreads;
                }
                var threadPool = new SmartThreadPool { MaxThreads = maxThreads<1?1:maxThreads, MinThreads = minThreads<1?1:minThreads };
                var pendingWorkItems = new IWorkItemResult[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    pendingWorkItems[i] = threadPool.QueueWorkItem(new WorkItemCallback(MUpdatePrice), list[i]);
                }

                if (SmartThreadPool.WaitAll(pendingWorkItems))
                {
                    threadPool.Shutdown();
                }
            }
        }
        public static object MUpdatePrice(object value)
        {
            var model = value as VenderInfoPackageRequest;
            if (!WayBillController.UpdatePriceWayBill(FreightController.PostVenderPrice(model),
                                                         model.WayBillNumber))
            {
                Log.Error(string.Format("运单号：{0}更新错误！", model.WayBillNumber));
            }
            return true;
        }
    }
}
