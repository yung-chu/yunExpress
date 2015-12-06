using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MQS_Data.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace MQS_Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string queueName = "SubmitOrder";

            LtQueueService.LtQueueClient client = new LtQueueService.LtQueueClient();

            TaskQueue taskQueue = GetLastTaskQueue();

            LtQueueService.QueueModel ret = client.Enqueue(queueName, JsonConvert.SerializeObject(taskQueue));
            bool success = (bool)ret.Value;

            if (success)
            {
                Console.WriteLine("入队成功");
            }
            else
            {
                Console.WriteLine(ret.ErrorMessage);
            }

            ret = client.Dequeue(queueName);

            if (ret.Status == 1)
            {
                Console.WriteLine(ret.Value.ToString());

                Console.WriteLine("出队成功");
            }
            else
            {
                Console.WriteLine(ret.ErrorMessage);
            }

            

            Console.ReadLine();
        }

        [TestMethod]
        public void GetLength()
        {
            string queueName = "SubmitOrder";

            LtQueueService.LtQueueClient client = new LtQueueService.LtQueueClient();

            LtQueueService.QueueModel ret = client.QueueLength(queueName);

            if (ret.Value!=null)
            {
                Debug.WriteLine(ret.Value);
            }
            else
            {
                Debug.WriteLine(ret.ErrorMessage);
            }
        }

        [TestMethod]
        public void Queue()
        {
            string queueName = "SubmitOrder";

            LtQueueService.LtQueueClient client = new LtQueueService.LtQueueClient();

            LtQueueService.QueueModel ret = client.Dequeue(queueName);

            if (ret.Status == 1)
            {
                TaskQueue taskQueue = JsonConvert.DeserializeObject<TaskQueue>(ret.Value.ToString());

                Debug.WriteLine(taskQueue.Value);

                Debug.WriteLine("出队成功");
            }
            else
            {
                Debug.WriteLine(ret.ErrorMessage);
            }

        }

        private TaskQueue GetLastTaskQueue()
        {
            using (var db = new MQS_DBEntities())
            {
                var taskQueue = (from t in db.TaskQueues orderby t.CreatedOn descending select t).FirstOrDefault();

                return taskQueue;
            }
        }
    }
}
