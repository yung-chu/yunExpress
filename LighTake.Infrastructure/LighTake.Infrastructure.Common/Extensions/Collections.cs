using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace LighTake.Infrastructure.Common
{
    /// <summary>
    /// 列表操作常用扩展
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年04月21日
    /// 修改历史 : 无
    /// </remarks>
    public static class Collections
    {
        /// <summary>
        /// 遍历列表并对列表中的每一项执行指定的动作
        /// </summary>
        /// <typeparam name="T">范型类型</typeparam>
        /// <param name="items">列表</param>
        /// <param name="action">动作代理</param>
        public static void Each<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var t in items)
            {
                action(t);
            }
        }

        /// <summary>
        /// 判断列表对象是否为空
        /// </summary>
        /// <typeparam name="T">范型类型</typeparam>
        /// <param name="collection">列表对象</param>
        /// <returns>是否为NULL或集合中的元素COUNT为0</returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return (collection == null) || (collection.Count == 0);
        }

        public static List<T> Copy<T>(this IEnumerable source)
            where T : class,new()
        {
            List<T> lstT = new List<T>();
            IEnumerator enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                lstT.Add(enumerator.Current.Copy<T>());
            }
            return lstT;
        }

        public static IList<R> Yield<T, R>(this IEnumerable<T> source, Func<T, R> func)
        {
            if (source == null)
            { return new List<R>(); }

            IList<R> lstR = new List<R>();
            foreach (var item in source)
            {
                lstR.Add(func(item));
            }
            return lstR;
        }

        public static IList<R> Yield<T, R>(this IEnumerable<T> source, Func<T, R> func, Func<T, bool> condition)
        {
            if (source == null)
            { return new List<R>(); }

            IList<R> lstR = new List<R>();
            foreach (var item in source)
            {
                if (condition(item))
                {
                    lstR.Add(func(item));
                }
            }
            return lstR;
        }

        public static IList<T> Insert<T>(this IList<T> source, T item, int index)
        {
            if (source == null)
                return null;

            source.Insert(index, item);

            return source;
        }

        public static IList<R> Yield<Key, Value, R>(this IDictionary<Key, Value> source, Func<KeyValuePair<Key, Value>, R> func)
        {
            if (source == null)
            { return new List<R>(); }

            IList<R> lstR = new List<R>();
            foreach (var item in source)
            {
                lstR.Add(func(item));
            }
            return lstR;
        }

        public static void Remove<T>(this IList<T> source, Func<T, bool> condition)
        {
            if (source == null)
                return;

            IList<T> lst = source.Yield(p => p, condition);
            foreach (T tmpT in lst)
            {
                source.Remove(tmpT);
            }
        }

        public static string YieldField<T>(this IEnumerable<T> source, Func<T, string> func, char separator = ',')
        {
            if (source == null)
            { return string.Empty; }

            string strResult = string.Empty;
            foreach (var item in source)
            {
                strResult += func(item) + separator;
            }
            return strResult.TrimEnd(separator);
        }
    }
}
