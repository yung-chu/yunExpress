using System;

namespace LighTake.MQS
{
    public interface IQueueMini
    {
        /// <summary>
        /// 队列名字
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 队列长度
        /// </summary>
        int Length { get; }

        /// <summary>
        /// 入队列前
        /// </summary>
        Func<IQueueMini, object, bool> BeforEnqueue{ get; set; }

        /// <summary>
        /// 入队列时
        /// </summary>
        Action<IQueueMini, object> AfterEnqueue { get; set; }

        /// <summary>
        /// 入队操作
        /// </summary>
        void Enqueue(object value);

        /// <summary>
        /// 出队列前
        /// </summary>
        Func<IQueueMini, bool> BeforDequeue{ get; set; }

        /// <summary>
        /// 队列后
        /// </summary>
        Action<IQueueMini, object> AfterDequeue { get; set; }

        /// <summary>
        /// 出队操作
        /// </summary>
        object Dequeue();

        /// <summary>
        /// 获取下一个出队
        /// </summary>
        object Peek();

        /// <summary>
        /// 挂起直到有入队
        /// </summary>
        /// <returns></returns>
        bool WaitEnqueue(int timeOut);
    }
}