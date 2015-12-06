using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;

namespace LMS.Data.Repository.Extensions
{
    /// <summary>
    /// 批量数据处理扩展
    /// by daniel 2014-11-5
    /// </summary>
    public class Bulk
    {
        /// <summary>
        /// 大量数据插入并且返回自增长ID
        /// by daniel 2014-11-5
        /// </summary>
        /// <typeparam name="T">表实体</typeparam>
        /// <param name="ctx">DbContext</param>
        /// <param name="tableName">表名</param>
        /// <param name="pk">主键(自增长ID)名称</param>
        /// <param name="list">数据</param>
        /// <returns></returns>
        public static void BulkInsert<T>(DbContext ctx, string tableName, IList<T> list)
        {
            var table = new DataTable();
            var props = TypeDescriptor.GetProperties(typeof(T))
                                       .Cast<PropertyDescriptor>()
                                       .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                       .ToArray();

            //Dirty hack to make sure we only have system data types 
            //i.e. filter out the relationships/collections


            using (var tran = new TransactionScope())
            {
                using (var conn = new SqlConnection(ctx.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    using (var bulkCopy = new SqlBulkCopy(conn))
                    {
                        bulkCopy.BatchSize = list.Count;
                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.BulkCopyTimeout = 10000;
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

                //commit tran
                tran.Complete();
            }


        }
    }
}
