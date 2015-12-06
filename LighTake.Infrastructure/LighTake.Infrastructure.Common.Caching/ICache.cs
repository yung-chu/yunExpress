using System;

namespace LighTake.Infrastructure.Common.Caching
{
    /// <summary>
    /// 公共缓存操作
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛
    /// 完成时间 : 2010年8月9日
    /// 修改历史 : 无
    /// </remarks>
    public interface ICache
    {
        /// <summary>
        /// 添加缓存项,有效时间: 永不过期
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="value">缓存对象</param>
        void Add(string key, object value);

        /// <summary>
        /// 添加缓存项,有效时间: 从添加起持续N分钟后缓存失效
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="value">缓存对象</param>
        /// <param name="duration">缓存有效时间(单位:分钟)</param>
        void Add(string key, object value, int duration);

        /// <summary>
        /// 添加缓存项,有效时间: 
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="value">缓存对象</param>
        /// <param name="absoluteTime">缓存有效时间</param>
        void Add(string key, object value, DateTime absoluteTime);

        /// <summary>
        /// 添加缓存项,有效时间: 缓存依赖文件,文件发生变化时缓存失效
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="value">缓存对象</param>
        /// <param name="filePath">缓存依赖文件</param>
        void Add(string key, object value, string filePath);

        /// <summary>
        /// 当前缓存容器中是否存在指定的缓存项
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <returns>存在则返回True 否则 False</returns>
        bool Exists(string key);

        /// <summary>
        /// 根据KEY获取对应的缓存对象
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <returns>缓存对象</returns>
        object Get(string key);

        /// <summary>
        /// 根据KEY获取对应的缓存对象
        /// </summary>
        /// <typeparam name="T">类型T</typeparam>
        /// <param name="key">缓存KEY</param>
        /// <returns>类型为T的缓存对象</returns>
        T Get<T>(string key);

        /// <summary>
        /// 根据KEY移除对应的缓存项
        /// </summary>
        /// <param name="key">缓存KEY</param>
        void Remove(string key);

        /// <summary>
        /// 刷新缓存,移除所有的缓存项
        /// </summary>
        void Flush();
    }
}
