using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LighTake.Infrastructure.Common
{
    public class ConvertListToDataTable
    {
        /// 将集合类转换成DataTable
        /// </summary>
        /// <param name="list">集合</param>
        /// <returns></returns>
        public static DataTable ToDataTable(IList list, List<string> columnNames)
        {
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertyList = list[0].GetType().GetProperties();
                foreach (var columnName in columnNames)
                {
                    if (propertyList.Select(p => p.Name).Contains(columnName))
                    {
                        Type type = propertyList.Single(p => p.Name == columnName).PropertyType;
                        if (type == typeof(Nullable<System.Int32>))
                            result.Columns.Add(columnName, typeof(System.Int32));
                        else if (type == typeof(Nullable<DateTime>))
                            result.Columns.Add(columnName, typeof(System.DateTime));
                        else if (type == typeof(Nullable<decimal>))
                            result.Columns.Add(columnName, typeof(System.Int32));
                        else
                            result.Columns.Add(columnName, type);
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();

                    foreach (var columnItem in columnNames)
                    {
                        var obj = propertyList.Single(p => p.Name == columnItem).GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        ///// <summary>
        ///// 将泛型集合类转换成DataTable
        ///// </summary>
        ///// <typeparam name="T">集合项类型</typeparam>
        ///// <param name="list">集合</param>
        ///// <returns>数据集(表)</returns>
        //public static DataTable ToDataTable<T>(IList<T> list)
        //{
        //    return ConvertX.ToDataTable<T>(list, null);
        //}

        /// <summary>
        /// 将泛型集合类转换成DataTable
        /// </summary>
        /// <typeparam name="T">集合项类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="propertyName">需要返回的列的列名</param>
        /// <returns>数据集(表)</returns>
        public static DataTable ToDataTable<T>(IList<T> list, params string[] propertyName)
        {
            List<string> propertyNameList = new List<string>();
            if (propertyName != null)
                propertyNameList.AddRange(propertyName);

            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name))
                            result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        if (propertyNameList.Count == 0)
                        {
                            object obj = pi.GetValue(list[i], null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (propertyNameList.Contains(pi.Name))
                            {
                                object obj = pi.GetValue(list[i], null);
                                tempList.Add(obj);
                            }
                        }
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }
    }
}
