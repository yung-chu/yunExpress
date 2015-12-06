namespace LighTake.Infrastructure.CommonQueue
{
    using System;
    using LighTake.Infrastructure.Common;
    using System.Configuration;
    using System.Collections.Generic;
    using LighTake.Infrastructure.Common.Logging;

    /// <summary>
    /// 队列使用帮助类 , 如果没有QueueType配置项，默认走RabbitMQ
    /// </summary>
    public class QueueHelper
    {
        static ISimpleQueue _queue;
        static int _queueType = 1;

        static QueueHelper()
        {
            _queueType = GetQueueType();
            switch (_queueType)
            {
                case 1:
                    _queue = new RabbitQueue();
                    break;
                case 2:
                    _queue = new WcfQueue();
                    break;
                default:
                    throw new BusinessLogicException("队列类型配置错误");
            }
        }

        static int GetQueueType()
        {
            var setting = ConfigurationManager.AppSettings["QueueType"];
            if (setting != null)
            {
                int result;
                if (int.TryParse(setting, out result))
                {
                    return result;
                }
            }

            return 1; //默认1,rabbitmq
        }

        /// <summary>
        /// 出队列
        /// </summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="rabbitsConfigKey">RabbitMQ配置Key(为空默认第一个配置Key)</param>
        /// <returns></returns>
        public static string Dequeue(string queueName, string rabbitsConfigKey = "")
        {
            Check.Argument.IsNotEmpty(queueName, "队列名称");
            return _queue.Dequeue(queueName, rabbitsConfigKey);
        }

        /// <summary>
        /// 入队列
        /// </summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="rabbitsConfigKey">RabbitMQ配置Key(为空默认第一个配置Key)</param>
        /// <param name="value"></param>
        public static bool Enqueue(string queueName, string[] values, string rabbitsConfigKey = "")
        {
            Check.Argument.IsNotEmpty(queueName, "队列名称");
            List<string> clears = new List<string>();
            values.Each(t =>
            {
                if (!string.IsNullOrWhiteSpace(t))
                { 
                    clears.Add(t);
                }               
            });
            if (clears.Count > 0)
            {
                Log.Info(string.Format("Enqueue:{0}", clears.Count));
                return _queue.Enqueue(queueName, clears.ToArray(), rabbitsConfigKey);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 消息总数
        /// </summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="rabbitsConfigKey">RabbitMQ配置Key(为空默认第一个配置Key)</param>
        /// <returns></returns>
        public static int Count(string queueName, string rabbitsConfigKey = "")
        {
            Check.Argument.IsNotEmpty(queueName, "队列名称");
            return _queue.Count(queueName, rabbitsConfigKey);
        }
    }
}
