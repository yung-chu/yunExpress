using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amib.Threading;
using LMS.Client.UpdateReceivingPrice.Controller;
using LMS.Client.UpdateReceivingPrice.Model;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.UpdateReceivingPrice
{
    public class UpdateReceivingPrice
    {
        private static string ismultithreading = System.Configuration.ConfigurationManager.AppSettings["IsMultiThreading"].ToString();
        private static int maxThreads = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["MaxThreads"].ToString());
        private static int minThreads = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["MinThreads"].ToString());
        public static void UpdatePrice()
        {
            var list= WayBillController.GetUpdatePriceWayBillList();
            if (list == null || list.Count <= 0) return;
            if (ismultithreading.ToLowerInvariant() == "no")
            {
                //单线程
                foreach (var model in list)
                {
                    if (!WayBillController.UpdatePriceWayBill(FreightController.GetFreightPrice(model),
                                                         model.WayBillNumber,model.ReceivingExpenseID))
                    {
                        Log.Error(string.Format("运单号：{0}更新错误！", model.WayBillNumber));
                    }
                }
            }
            else if (ismultithreading.ToLowerInvariant() == "yes")
            {
                //多线程
                if (minThreads > maxThreads)
                {
                    maxThreads = minThreads;
                }
                var threadPool = new SmartThreadPool { MaxThreads = maxThreads < 1 ? 1 : maxThreads, MinThreads = minThreads < 1 ? 1 : minThreads };
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
            var model = value as WayBillPriceModel;

            if (!WayBillController.UpdatePriceWayBill(FreightController.GetFreightPrice(model),
                                                         model.WayBillNumber,model.ReceivingExpenseID))
            {
                Log.Error(string.Format("运单号：{0}更新错误！", model.WayBillNumber));
            }
            return true;
        }
    }
}
