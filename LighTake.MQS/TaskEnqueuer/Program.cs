using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using MQS_Data.Context;
using Newtonsoft.Json;

namespace MQS_TaskEnqueuer
{
    class Program
    {
        private static int _sleepTime = 2 * 1000;

        static void Main(string[] args)
        {
            GetSleepTime();
            DoWork();
        }

        static void GetSleepTime()
        {
            var sleepTime = ConfigurationManager.AppSettings["SleepTime"];
            if (sleepTime != null)
            {
                _sleepTime = Convert.ToInt32(sleepTime);
            }
        }

        private static void DoWork()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("正在获取需要入队数...");

                    List<TaskQueue> listTaskQueue = GetNeedEnqueue();

                    if (listTaskQueue.Count == 0)
                    {
                        WaitAddTaskQueue(5*60*1000);
                    }

                    listTaskQueue = GetNeedEnqueue();

                    Console.WriteLine("需要入队数:" + listTaskQueue.Count);

                    int successCount = 0;
                    int failCount = 0;

                    successCount = Enqueue(listTaskQueue);

                    failCount = listTaskQueue.Count - successCount;


                    Console.WriteLine(string.Format("成功数:{0},失败数:{1}", successCount, failCount));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Console.WriteLine("---------------------------------------");
                    Thread.Sleep(_sleepTime);
                }
            }
        }

        private static List<TaskQueue> GetNeedEnqueue()
        {
            using (var db = new MQS_DBEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.ValidateOnSaveEnabled = false;

                var list = (from t in db.TaskQueues where t.Status == 0 orderby t.CreatedOn select t).Take(200).ToList();

                return list;
            }
        }

        private static int Enqueue(IEnumerable<TaskQueue> taskQueues)
        {
            int successCount = 0;

            LtQueueService.LtQueueClient client = new LtQueueService.LtQueueClient();

            var group = taskQueues.GroupBy(p => p.QueueName).ToList();

            foreach (IGrouping<string, TaskQueue> queues in group)
            {
                string[] values = queues.Select(JsonConvert.SerializeObject).ToArray();
                LtQueueService.QueueModel ret = client.Enqueue(queues.Key, values);

                successCount += Convert.ToInt32(ret.Value);
            }

            client.Close();
            return successCount;

        }

        private static bool WaitAddTaskQueue(int timeOut)
        {
            LtTaskQueueService.LtTaskQueueClient client = new LtTaskQueueService.LtTaskQueueClient();
            bool ret = client.WaitAddTaskQueue(timeOut);
            client.Close();
            return ret;
        }
    }
}
