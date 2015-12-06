using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using LighTake.Infrastructure.Common.Logging;
using MQS_Data.Context;
using System.Diagnostics;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace LighTake.MQS
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“LtTaskQueue”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 LtTaskQueue.svc 或 LtTaskQueue.svc.cs，然后开始调试。
    public class LtTaskQueue : ILtTaskQueue
    {
        private static AutoResetEvent WaitHandle { set; get; }

        static LtTaskQueue()
        {
            WaitHandle = new AutoResetEvent(false);
        }

        public bool AddTaskQueue(string queueName, string[] values)
        {
            try
            {
                Log.Info(string.Format("LtTaskQueue.AddTaskQueue,count:{0}", values.Count()));
                ConcurrentBag<TaskQueue> list = new ConcurrentBag<TaskQueue>();
                Parallel.ForEach(values, value =>
                //foreach ( string value in values)
                {
                    TaskQueue t = new TaskQueue()
                        {
                            QueueName = queueName,
                            Value = value,
                            CreatedOn = DateTime.Now,
                            LastUpdatedOn = DateTime.Now,
                            Status = 0,
                        };
                    list.Add(t);
                });
                Log.Info(string.Format("LtTaskQueue.AddTaskQueue.list ConcurrentBag<TaskQueue>,count:{0}", list.Count()));
                using (var db = new MQS_DBEntities())
                {
                    BulkInsert(db.Database.Connection.ConnectionString, "TaskQueues", list.ToList());
                }

                WaitHandle.Set();

                return true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return false;
            }
        }

        public bool WaitAddTaskQueue(int timeOut)
        {
            const int maxTimeOut = 60 * 60 * 1000;

            if (timeOut > maxTimeOut) timeOut = maxTimeOut;

            return WaitHandle.WaitOne(timeOut);
        }

        private void BulkInsert<T>(string connection, string tableName, IList<T> list)
        {
            var table = new DataTable();
            var props = TypeDescriptor.GetProperties(typeof(T))
                //Dirty hack to make sure we only have system data types 
                //i.e. filter out the relationships/collections
                                       .Cast<PropertyDescriptor>()
                                       .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                       .ToArray();

            using (var bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.BatchSize = list.Count; 
                bulkCopy.DestinationTableName = tableName;

                foreach (var propertyInfo in props)
                {
                    bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                    table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                }

                var values = new object[props.Length];
                foreach (var item in list)
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item);
                    }

                    table.Rows.Add(values);
                }

                bulkCopy.WriteToServer(table);
            }
        }
    }
}
