namespace LighTake.Infrastructure.CommonQueue
{
    using System;
    using LighTake.Infrastructure.Common;
    using LighTake.Infrastructure.Common.Logging;
    using LighTake.Infrastructure.CommonQueue.API.Queue;

    /// <summary>
    /// Wcf Queue 实现的 Queue
    /// </summary>
    public class WcfQueue : ISimpleQueue
    {
        public string Dequeue(string queueName,string rabbitsConfigKey)
        {
            QueueModel result = new QueueModel();
            using (API.Queue.LtQueueClient client = new API.Queue.LtQueueClient())
            {
                result = client.Dequeue(queueName);
            }
            if (string.IsNullOrWhiteSpace(result.ErrorMessage))
            {
                var task = SerializeUtil.FromJson<TaskQueue>(result.Value as string);
                return task.Value;
            }
            else
            {
                if (result.ErrorMessage != "队列为空或不存在")
                {
                    Log.Error(result.ErrorMessage);
                    throw new BusinessLogicException(result.ErrorMessage);
                }
                return string.Empty;
            }
        }


        public int Count(string queueName, string rabbitsConfigKey)
        {
            QueueModel result = new QueueModel();
            using (API.Queue.LtQueueClient client = new API.Queue.LtQueueClient())
            {
                result = client.QueueLength(queueName);
            }
            if (string.IsNullOrWhiteSpace(result.ErrorMessage))
            {
                return (int)result.Value;
            }
            else
            {
                Log.Error(result.ErrorMessage);
                //throw new BusinessLogicException(result.ErrorMessage);
                return 0;
            }
        }


        public bool Enqueue(string queueName, string[] value, string rabbitsConfigKey)
        {
            using (API.Task.LtTaskQueueClient client = new API.Task.LtTaskQueueClient())
            {
                Log.Info(queueName);
                try
                {
                    return client.AddTaskQueue(queueName, value);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.InnerException == null ? ex.ToString() : ex.InnerException.ToString());
                    throw new BusinessLogicException("入WCF队列失败.", ex.InnerException ?? ex);
                    //return false;
                }
            }
        }
    }
}
