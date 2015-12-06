using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amib.Threading;
using LMS.Services.SequenceNumber;
using LighTake.Infrastructure.Common.Logging;

namespace TestCreateWayBillNumber
{
    class Program
    {
        private static Hashtable dictionary = Hashtable.Synchronized(new Hashtable());
        static void Main(string[] args)
        {
            try
            {
                Parallel.For(0, 10, i =>
                    {
                        MUpdatePrice(i);
                    });
                //var threadPool = new SmartThreadPool { MaxThreads = 10, MinThreads = 1 };
                //var pendingWorkItems = new IWorkItemResult[10];
                //for (int i = 0; i < 10; i++)
                //{
                //    pendingWorkItems[i] = threadPool.QueueWorkItem(new WorkItemCallback(MUpdatePrice), i);
                //}

                //if (SmartThreadPool.WaitAll(pendingWorkItems))
                //{
                //    threadPool.Shutdown();
                //}
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            Console.ReadLine();
        }
        public static object MUpdatePrice(object value)
        {
            var j = Int32.Parse(value.ToString());
            string customerCode = ConfigurationManager.AppSettings["CustomerCode"];
            Log.Info("开始生成客户编码：" + customerCode + "，第" + j + "批");
            for (int i = 0; i < 1000; i++)
            {
                string waybillnumber = SequenceNumberService.GetWayBillNumber(customerCode);
                Log.Info(waybillnumber);
                if (dictionary.ContainsKey(waybillnumber))
                {
                    Log.Error(waybillnumber);
                }
                else
                {
                    dictionary.Add(waybillnumber,waybillnumber);
                }
                Log.Info(dictionary.Count.ToString());
            }
            Log.Info("完成生成客户编码：" + customerCode + "，第" + j + "批");
            return true;
        }
    }
}
