using System;
using System.Collections.Generic;

namespace LighTake.Infrastructure.Common.Caching
{
    /// <summary>
    /// 通用的缓存操作
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛
    /// 完成时间 : 2010年8月9日
    /// 修改历史 : 无
    /// </remarks>
    public static class Cache
    {
        private static ICache cacheProvier;

        /// <summary>
        /// 缓存项状态信息
        /// </summary>
        public static Dictionary<string, CacheItemStatus> CacheItems
        {
            get;
            private set;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="factory">依赖解析器创建工厂</param>
        public static void InitializeWith(ICacheProviderFactory factory)
        {
            Check.Argument.IsNotNull(factory, "factory");

            cacheProvier = factory.CreateInstance();

            CacheItems = new Dictionary<string, CacheItemStatus>();
        }

        /// <summary>
        /// 添加缓存项,有效时间: 永不过期
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="value">缓存对象</param>
        public static void Add(string key, object value)
        {
            cacheProvier.Add(key, value, 60);

            //CacheItems.Add(key, new CacheItemStatus
            //{
            //    AddTime = DateTime.Now,
            //    ExpireTime = DateTime.MaxValue,
            //    ItemType = value.GetType()
            //});
        }

        /// <summary>
        /// 添加缓存项,有效时间: 从添加起持续N分钟后缓存失效
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="value">缓存对象</param>
        /// <param name="duration">缓存有效时间(单位:分钟)</param>
        public static void Add(string key, object value, int duration)
        {
            cacheProvier.Add(key, value, duration);

            //CacheItems.Add(key, new CacheItemStatus
            //{
            //    AddTime = DateTime.Now,
            //    ExpireTime = DateTime.Now.AddMinutes(duration),
            //    ItemType = value.GetType()
            //});
        }

        /// <summary>
        /// 添加缓存项,有效时间: 
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="value">缓存对象</param>
        /// <param name="absoluteTime">缓存有效时间</param>
        public static void Add(string key, object value, DateTime absoluteTime)
        {
            cacheProvier.Add(key, value, absoluteTime);
        }

        /// <summary>
        /// 添加缓存项,有效时间: 缓存依赖文件,文件发生变化时缓存失效
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="value">缓存对象</param>
        /// <param name="filePath">缓存依赖文件</param>
        public static void Add(string key, object value, string filePath)
        {
            cacheProvier.Add(key, value, filePath);
        }

        /// <summary>
        /// 当前缓存容器中是否存在指定的缓存项
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <returns>存在则返回True 否则 False</returns>
        public static bool Exists(string key)
        {
            return cacheProvier.Exists(key);
        }

        /// <summary>
        /// 根据KEY获取对应的缓存对象
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <returns>缓存对象</returns>
        public static object Get(string key)
        {
            return cacheProvier.Get(key);
        }

        /// <summary>
        /// 根据KEY获取对应的缓存对象
        /// </summary>
        /// <typeparam name="T">类型T</typeparam>
        /// <param name="key">缓存KEY</param>
        /// <returns>类型为T的缓存对象</returns>
        public static T Get<T>(string key)
        {
            return cacheProvier.Get<T>(key);
        }

        public static T Get<T>(string key, Func<T> acquire)
        {
            return Get(key, 60, acquire);
        }

        public static T Get<T>(string key, int cacheMinute, Func<T> acquire)
        {
            T t = default(T);
            if (cacheProvier.Exists(key))
            {
                return cacheProvier.Get<T>(key);
            }
            var result = acquire();
            cacheProvier.Add(key, result, cacheMinute);
            return result;
        }

        /// <summary>
        /// 根据KEY移除对应的缓存项
        /// </summary>
        /// <param name="key">缓存KEY</param>
        public static void Remove(string key)
        {
            cacheProvier.Remove(key);
        }

        /// <summary>
        /// 移除所有的缓存项
        /// </summary>
        public static void Flush()
        {
            cacheProvier.Flush();
        }
    }
}
