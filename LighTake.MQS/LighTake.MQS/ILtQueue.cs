using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using LighTake.MQS.Dto;

namespace LighTake.MQS
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“ILtQueue”。
    [ServiceContract]
    public interface ILtQueue
    {
        //[OperationContract]
        //QueueModel Enqueue(string queueName, string value);

        [OperationContract]
        QueueModel Enqueue(string queueName, string[] values);

        [OperationContract]
        QueueModel Dequeue(string queueName);

        [OperationContract]
        QueueModel QueueLength(string queueName);

        [OperationContract]
        bool WaitEnqueue(string queueName, int timeOut);
    }
}
