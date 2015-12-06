namespace LighTake.Infrastructure.CommonQueue
{
    using System;

    /// <summary>
    /// Wcf Queue 获取的Message实体
    /// </summary>
    public class TaskQueue
    {
        public long TaskQueueId { get; set; }
        public string QueueName { get; set; }
        public string Value { get; set; }
        public int Status { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
    }
}
