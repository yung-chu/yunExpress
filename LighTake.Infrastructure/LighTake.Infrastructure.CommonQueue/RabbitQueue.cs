namespace LighTake.Infrastructure.CommonQueue
{
    using System;
    using LighTake.Infrastructure.RabbitManager;
    using RabbitMQ.Client.Exceptions;
    using LighTake.Infrastructure.Common;
    using LighTake.Infrastructure.Common.Logging;

    /// <summary>
    /// RabbitMQ 实现的 Queue
    /// </summary>
    public class RabbitQueue : ISimpleQueue
    {
        public string Dequeue(string queueName, string rabbitsConfigKey)
        {
            try
            {
                using (var channel = ConnectionHelper.GetChannel(rabbitsConfigKey))
                {
                    var res = channel.BasicGet(queueName, true/*noAck*/);
                    if (res != null)
                    {
                        return System.Text.UTF8Encoding.UTF8.GetString(res.Body);
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            catch (OperationInterruptedException ex)
            {
                if (ex.Message.IndexOf("NOT_FOUND - no queue") > -1) //队列不存在
                {
                    Log.Error((ex.InnerException == null ? ex : ex.InnerException).ToString());
                    return string.Empty;
                }
                else
                {
                    throw ex;
                }
            }
        }

        public int Count(string queueName, string rabbitsConfigKey)
        {
            using (var channel = ConnectionHelper.GetChannel(rabbitsConfigKey))
            {
                var res = channel.BasicGet(queueName, false/*noAck*/);
                return (int)res.MessageCount;
            }
        }

        public bool Enqueue(string queueName, string[] value, string rabbitsConfigKey)
        {
            using (var channel = ConnectionHelper.GetChannel(rabbitsConfigKey))
            {
                var queueDecalre = channel.QueueDeclare(queueName, true/*durable*/, false, false, null);

                var property = channel.CreateBasicProperties();
                property.SetPersistent(true);

                foreach (var s in value)
                {
                    byte[] byteData = System.Text.Encoding.UTF8.GetBytes(s);
                    channel.BasicPublish("", queueDecalre.QueueName, property, byteData);
                }

                return true;
            }
        }
    }
}
