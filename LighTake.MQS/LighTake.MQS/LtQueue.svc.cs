using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using LighTake.Infrastructure.Common.Logging;
using LighTake.MQS.Dto;
using MQS_Data.Context;
using Newtonsoft.Json;

namespace LighTake.MQS
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“LtQueue”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 LtQueue.svc 或 LtQueue.svc.cs，然后开始调试。
    public class LtQueue : ILtQueue
    {
        private static List<IQueueMini> QueueMinis { set; get; }
        private static bool Initialized { set; get; }

        static LtQueue()
        {
            QueueMinis = new List<IQueueMini>();
            Initialize();
        }

        private static QueueModel EnqueueStatic(string queueName, string value)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException("value");
                }

                TaskQueue taskQueue = JsonConvert.DeserializeObject<TaskQueue>(value);

                if (taskQueue == null)
                {
                    throw new Exception("反序列化为TaskQueue失败");
                }

                IQueueMini queueMini = GetQueueMiniByName(queueName);

                if (queueMini == null)
                {
                    queueMini = CreatQueueMini(queueName);

                    QueueMinis.Add(queueMini);
                }

                queueMini.Enqueue(taskQueue);

                return new QueueModel()
                {
                    Value = true,
                };
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return new QueueModel()
                {
                    Value = false,
                    ErrorMessage = ex.Message,
                };
            }

        }

        public QueueModel Enqueue(string queueName, string value)
        {
            return EnqueueStatic(queueName, value);
        }

        public QueueModel Enqueue(string queueName, string[] values)
        {
            int successCount = 0;

            foreach (string value in values)
            {
                QueueModel queueModel = Enqueue(queueName, value);

                if (!Convert.ToBoolean(queueModel.Value))
                {
                    return new QueueModel()
                        {
                            Value = successCount,
                            ErrorMessage = queueModel.ErrorMessage,
                        };
                }
                else
                {
                    successCount++;
                }
            }

            return new QueueModel()
                {
                    Value = successCount,
                };
        }

        public QueueModel Dequeue(string queueName)
        {
            try
            {
                IQueueMini queueMini = GetQueueMiniByName(queueName);

                if (queueMini == null)
                {
                    throw new InvalidOperationException("队列名不存在");
                }

                var value = queueMini.Dequeue();

                return new QueueModel()
                {
                    Value = JsonConvert.SerializeObject(value),
                    Status = 1,
                };
            }
            catch(InvalidOperationException)
            {
                return new QueueModel()
                {
                    Value = null,
                    ErrorMessage = "队列为空或不存在",
                    Status = 10,
                };
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return new QueueModel()
                {
                    Value = null,
                    ErrorMessage = ex.Message,
                };
            }
        }

        public QueueModel QueueLength(string queueName)
        {
            try
            {
                IQueueMini queueMini = GetQueueMiniByName(queueName);

                if (queueMini == null)
                {
                    throw new InvalidOperationException("队列不存在");
                }

                return new QueueModel()
                {
                    Value = queueMini.Length,
                };
            }
            catch (InvalidOperationException ex)
            {
                return new QueueModel()
                {
                    Value = null,
                    ErrorMessage = ex.ToString(),
                };
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return new QueueModel()
                {
                    Value = null,
                    ErrorMessage = ex.ToString(),
                };
            }
        }

        public bool WaitEnqueue(string queueName,int timeOut)
        {
            const int maxTimeOut = 60 * 60 * 1000;

            if (timeOut > maxTimeOut) timeOut = maxTimeOut;

            try
            {
                IQueueMini queueMini = GetQueueMiniByName(queueName);

                if (queueMini == null)
                {
                    queueMini = CreatQueueMini(queueName);
                    QueueMinis.Add(queueMini);
                }

                var value = queueMini.WaitEnqueue(timeOut);

                return value;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return false;
            }
        }

        private static IQueueMini GetQueueMiniByName(string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new ArgumentNullException("queueName");
            }

            return QueueMinis.Find(q => q.Name == queueName);
        }

        private static IQueueMini CreatQueueMini(string queueName)
        {

            if (QueueMinis.Find(q => q.Name == queueName) != null)
            {
                throw new Exception("队列名称已存在");
            }

            IQueueMini queueMini = new QueueMini(queueName);

            queueMini.BeforEnqueue += BeforEnqueue;

            queueMini.BeforDequeue += BeforDequeue;

            queueMini.AfterDequeue += AfterDequeue;

            return queueMini;
        }

        private static bool BeforEnqueue(IQueueMini queueMini, object value)
        {

            TaskQueue taskQueue = value as TaskQueue;

            if (taskQueue == null)
            {
                throw new Exception("转化为TaskQueue实体失败");
            }

            using (var db = new MQS_DBEntities())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.ValidateOnSaveEnabled = false;

                string sql;

                if (Initialized)
                {
                    sql = string.Format("update dbo.TaskQueues set status=1 where status=0 and taskQueueId={0}", taskQueue.TaskQueueId);
                }
                else
                {
                    sql = string.Format("update dbo.TaskQueues set status=1 where taskQueueId={0}", taskQueue.TaskQueueId);
                }

                int successCount = db.Database.ExecuteSqlCommand(sql);

                if (successCount == 0)
                {
                    throw new Exception("入队失败，数据库找不到该id：" + taskQueue.TaskQueueId);
                }
            }
            return true;
        }

        private static bool BeforDequeue(IQueueMini queueMini)
        {
            if (queueMini.Length == 0)
            {
                throw new InvalidOperationException("队列长度为0");
            }

            //TaskQueue taskQueue = queueMini.Peek() as TaskQueue;

            //if (taskQueue == null)
            //{
            //    throw new Exception("转化为TaskQueue实体失败");
            //}

            //using (var db = new MQS_DBEntities())
            //{
            //    db.Configuration.AutoDetectChangesEnabled = false;
            //    db.Configuration.ValidateOnSaveEnabled = false;

            //    int successCount = db.Database.ExecuteSqlCommand(string.Format("update dbo.TaskQueues  set status=2  where taskQueueId={0}", taskQueue.TaskQueueId));

            //    if (successCount == 0)
            //    {
            //        throw new Exception("出队失败，数据库找不到该id：" + taskQueue.TaskQueueId);
            //    }
            //}

            return true;
        }

        private static void AfterDequeue(IQueueMini queueMini, object value)
        {
            try
            {
                if (value == null)
                {
                    throw new Exception("出队内容为null");
                }

                TaskQueue taskQueue = value as TaskQueue;

                if (taskQueue == null)
                {
                    throw new Exception("出队时转化为TaskQueue实体失败");
                }

                using (var db = new MQS_DBEntities())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    db.Configuration.ValidateOnSaveEnabled = false;

                    int successCount = db.Database.ExecuteSqlCommand(string.Format("update dbo.TaskQueues  set status=2  where taskQueueId={0}", taskQueue.TaskQueueId));

                    if (successCount == 0)
                    {
                        throw new Exception("出队失败，数据库找不到该id：" + taskQueue.TaskQueueId);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);

                if (value != null)
                {
                    bool reTrySuccess = Convert.ToBoolean(EnqueueStatic(queueMini.Name, value.ToString()).Value);

                    if (!reTrySuccess)
                        Log.Error("尝试重复入队失败");
                }
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private static void Initialize()
        {
            using (var db = new MQS_DBEntities())
            {
                var list = (from t in db.TaskQueues where t.Status == 1 orderby t.CreatedOn select t).ToList();

                list.ForEach(t => EnqueueStatic(t.QueueName, JsonConvert.SerializeObject(t)));
            }

            Initialized = true;

        }
    }
}
