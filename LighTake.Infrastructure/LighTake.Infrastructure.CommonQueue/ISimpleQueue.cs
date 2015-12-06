namespace LighTake.Infrastructure.CommonQueue
{
    using System;
    public interface ISimpleQueue
    {
        /// <summary>
        /// 出队列
        /// </summary>
        /// <param name="queueName">队列名称</param>
        /// <returns></returns>
        string Dequeue(string queueName, string rabbitsConfigKey);
        /// <summary>
        /// 入队列
        /// </summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="value"></param>
        bool Enqueue(string queueName, string[] value, string rabbitsConfigKey);
        /// <summary>
        /// 消息总数
        /// </summary>
        /// <param name="queueName">队列名称</param>
        /// <returns></returns>
        int Count(string queueName, string rabbitsConfigKey);
    }
}
