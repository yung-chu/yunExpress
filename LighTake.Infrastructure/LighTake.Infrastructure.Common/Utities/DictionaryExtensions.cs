using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Common
{
    /// <summary>
    /// 字典操作常用工具类
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年04月21日
    /// 修改历史 : 无
    /// </remarks>
    public static class DictionaryExtensions
    {
        public static TDictionary CopyFrom<TDictionary, TKey, TValue>(
            this TDictionary source,
            IDictionary<TKey, TValue> copy)
            where TDictionary : IDictionary<TKey, TValue>
        {
            foreach (var pair in copy)
            {
                source.Add(pair.Key, pair.Value);
            }

            return source;
        }

        public static TDictionary CopyFrom<TDictionary, TKey, TValue>(
            this TDictionary source,
            IDictionary<TKey, TValue> copy,
            IEnumerable<TKey> keys)
            where TDictionary : IDictionary<TKey, TValue>
        {
            foreach (var key in keys)
            {
                source.Add(key, copy[key]);
            }

            return source;
        }

        public static TDictionary RemoveKeys<TDictionary, TKey, TValue>(
            this TDictionary source,
            IEnumerable<TKey> keys)
            where TDictionary : IDictionary<TKey, TValue>
        {
            foreach (var key in keys)
            {
                source.Remove(key);
            }

            return source;
        }

        public static IDictionary<TKey, TValue> RemoveKeys<TKey, TValue>(
            this IDictionary<TKey, TValue> source,
            IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
            {
                source.Remove(key);
            }

            return source;
        }
    }


}
