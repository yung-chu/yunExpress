using System;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;

namespace LighTake.Infrastructure.Common.Caching
{
    /// <summary>
    /// 提供基于企业库缓存应用块的缓存操作
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛
    /// 完成时间 : 2010年8月9日
    /// 修改历史 : 无
    /// </remarks>
    public class EntLibCacheProvider : ICache
    {
        #region Fields & Properties

        private readonly ICacheManager cacheManager;

        #endregion

        #region .ctors

        /// <summary>
        /// 创建默认的缓存管理对象
        /// </summary>
        public EntLibCacheProvider()
        {
            cacheManager = CacheFactory.GetCacheManager();
        }

        /// <summary>
        /// 根据名称创建缓存提供者对象
        /// </summary>
        /// <param name="cacheManagerName"></param>
        public EntLibCacheProvider(string cacheManagerName)
        {
            cacheManager = CacheFactory.GetCacheManager(cacheManagerName);
        }

        #endregion

        /// <summary>
        /// 添加缓存项,有效时间: 永不过期
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="value">缓存对象</param>
        public void Add(string key, object value)
        {
            if (Exists(key))
            {
                Remove(key);
            }

            cacheManager.Add(key, value);
        }

        /// <summary>
        /// 添加缓存项,有效时间: 从添加起持续N分钟后缓存失效
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="value">缓存对象</param>
        /// <param name="duration">缓存有效时间(单位:分钟)</param>
        public void Add(string key, object value, int duration)
        {
            Add(key, value, CacheItemPriority.Normal, null, duration);
        }

        /// <summary>
        /// 添加缓存项,有效时间: 有效持续到指定时间
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="value">缓存对象</param>
        /// <param name="absoluteTime">缓存有效时间</param>
        public void Add(string key, object value, DateTime absoluteTime)
        {
            Add(key, value, CacheItemPriority.Normal, null, absoluteTime);
        }

        /// <summary>
        /// 添加缓存项,有效时间: 缓存依赖文件,文件发生变化时缓存失效
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="value">缓存对象</param>
        /// <param name="filePath">缓存依赖文件</param>
        public void Add(string key, object value, string filePath)
        {
            Add(key, value, CacheItemPriority.Normal, null, filePath);
        }

        /// <summary>
        /// 添加缓存项,有效时间: 从添加起持续N分钟后缓存失效
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="value">缓存对象</param>
        /// <param name="priority">缓存优先级</param>
        /// <param name="refreshAction">缓存项更新回调</param>
        /// <param name="duration">缓存有效时间(单位:分钟)</param>
        public void Add(string key, object value, CacheItemPriority priority, ICacheItemRefreshAction refreshAction, int duration)
        {
            if (Exists(key))
            {
                Remove(key);
            }

            cacheManager.Add(key, value, priority, refreshAction, new SlidingTime(TimeSpan.FromMinutes(duration)));
        }

        /// <summary>
        /// 添加缓存项,有效时间: 有效持续到指定时间
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="value">缓存对象</param>
        /// <param name="priority">缓存优先级</param>
        /// <param name="refreshAction">缓存项更新回调</param>
        /// <param name="absoluteTime">缓存有效时间</param>
        public void Add(string key, object value, CacheItemPriority priority, ICacheItemRefreshAction refreshAction, DateTime absoluteTime)
        {
            if (Exists(key))
            {
                Remove(key);
            }

            cacheManager.Add(key, value, priority, refreshAction, new AbsoluteTime(absoluteTime));
        }

        /// <summary>
        /// 添加缓存项,有效时间: 缓存依赖文件,文件发生变化时缓存失效
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="value">缓存对象</param>
        /// <param name="priority">缓存优先级</param>
        /// <param name="refreshAction">缓存项更新回调</param>
        /// <param name="filePath">缓存依赖文件</param>
        public void Add(string key, object value, CacheItemPriority priority, ICacheItemRefreshAction refreshAction, string filePath)
        {
            if (Exists(key))
            {
                Remove(key);
            }

            cacheManager.Add(key, value, priority, refreshAction, new FileDependency(filePath));
        }

        /// <summary>
        /// 当前缓存容器中是否存在指定的缓存项
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <returns>存在则返回True 否则 False</returns>
        public bool Exists(string key)
        {
            return cacheManager.Contains(key);
        }

        /// <summary>
        /// 根据KEY获取对应的缓存对象
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <returns>缓存对象</returns>
        public object Get(string key)
        {
            return cacheManager.GetData(key);
        }

        /// <summary>
        /// 根据KEY获取对应的缓存对象
        /// </summary>
        /// <typeparam name="T">类型T</typeparam>
        /// <param name="key">缓存KEY</param>
        /// <returns>类型为T的缓存对象</returns>
        public T Get<T>(string key)
        {
            T obj = (T)cacheManager.GetData(key);

            return obj;
        }

        /// <summary>
        /// 根据KEY移除对应的缓存项
        /// </summary>
        /// <param name="key">缓存KEY</param>
        public void Remove(string key)
        {
            cacheManager.Remove(key);
        }

        /// <summary>
        /// 刷新缓存,移除所有的缓存项
        /// </summary>
        public void Flush()
        {
            cacheManager.Flush();
        }
    }
}
