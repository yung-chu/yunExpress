using System;
using LighTake.Infrastructure.Common.Caching.Memcached;
using LighTake.Infrastructure.Common.Logging;

namespace LighTake.Infrastructure.Common.Caching
{
    /// <summary>
    /// 提供基于Memcached的缓存操作
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛
    /// 完成时间 : 2010年8月9日
    /// 修改历史 : 无
    /// </remarks>
    public class MemcachedCacheProvider : ICache
    {
        /// <summary>
        /// 添加缓存项,有效时间: 永不过期
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="value">缓存对象</param>
        public void Add(string key, object value)
        {
            bool succeed = false;

            if (Exists(key))
            {
                succeed = MemcachedTools.CacheManager.Set(key, value);
            }
            else
            {
                succeed = MemcachedTools.CacheManager.Add(key, value);
            }

            if (!succeed)
            {
                Log.Debug("缓存 KEY:{0} 没有保存成功.".FormatWith(key));
            }
        }

        /// <summary>
        /// 添加缓存项,有效时间: 从添加起持续N分钟后缓存失效
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="value">缓存对象</param>
        /// <param name="duration">缓存有效时间(单位:分钟)</param>
        public void Add(string key, object value, int duration)
        {
            bool succeed = false;

            if (MemcachedTools.CacheManager.KeyExists(key))
            {
                succeed = MemcachedTools.CacheManager.Set(key, value, DateTime.Now.AddMinutes(duration));
            }
            else
            {
                succeed = MemcachedTools.CacheManager.Add(key, value, DateTime.Now.AddMinutes(duration));
            }

            if (!succeed)
            {
                Log.Debug("缓存 KEY:{0} 没有保存成功.".FormatWith(key));
            }
        }

        /// <summary>
        /// 添加缓存项,有效时间: 有效持续到指定时间
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="value">缓存对象</param>
        /// <param name="absoluteTime">缓存有效时间</param>
        public void Add(string key, object value, DateTime absoluteTime)
        {
            bool succeed = false;
            if (MemcachedTools.CacheManager.KeyExists(key))
            {
                succeed = MemcachedTools.CacheManager.Set(key, value, absoluteTime);
            }
            else
            {
                succeed = MemcachedTools.CacheManager.Add(key, value, absoluteTime);
            }

            if (!succeed)
            {
                Log.Debug("缓存 KEY:{0} 没有保存成功.".FormatWith(key));
            }
        }

        /// <summary>
        /// 添加缓存项,有效时间: 缓存依赖文件,文件发生变化时缓存失效
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="value">缓存对象</param>
        /// <param name="filePath">缓存依赖文件</param>
        public void Add(string key, object value, string filePath)
        {
            // 暂时不支持
            throw new NotSupportedException("缓存依赖文件在Memcached策略中暂不支持.");
        }

        /// <summary>
        /// 当前缓存容器中是否存在指定的缓存项
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <returns>存在则返回True 否则 False</returns>
        public bool Exists(string key)
        {
            return MemcachedTools.CacheManager.KeyExists(key);
        }

        /// <summary>
        /// 根据KEY获取对应的缓存对象
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <returns>缓存对象</returns>
        public object Get(string key)
        {
            return MemcachedTools.CacheManager.Get(key);
        }

        /// <summary>
        /// 根据KEY获取对应的缓存对象
        /// </summary>
        /// <typeparam name="T">类型T</typeparam>
        /// <param name="key">缓存KEY</param>
        /// <returns>类型为T的缓存对象</returns>
        public T Get<T>(string key)
        {
            T obj = (T)Get(key);

            return obj;
        }

        /// <summary>
        /// 根据KEY移除对应的缓存项
        /// </summary>
        /// <param name="key">缓存KEY</param>
        public void Remove(string key)
        {
            if (Exists(key))
                MemcachedTools.CacheManager.Delete(key);
        }

        /// <summary>
        /// 刷新缓存,移除所有的缓存项
        /// </summary>
        public void Flush()
        {
            MemcachedTools.CacheManager.FlushAll();
        }
    }
}
