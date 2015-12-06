using System.Configuration;

namespace LighTake.Infrastructure.Common.Caching.Memcached
{
    /// <summary>
    /// 功能描述 : Memcached配置节点
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛
    /// 完成时间 : 2009年10月23日
    /// 修改历史 : 无
    /// </remarks>
    public class MemcachedConfigSection : ConfigurationSection
    {
        /// <summary>
        /// 默认的配置结点名称
        /// </summary>
        public const string SectionName = "memcached";

        /// <summary>
        /// 是否启用Memcached
        /// </summary>
        [ConfigurationProperty("enabled", IsRequired = true)]
        public bool Enabled
        {
            get
            {
                return (bool)base["enabled"];
            }
            set
            {
                base["enabled"] = value;
            }
        }

        /// <summary>
        /// 服务器列表
        /// </summary>
        [ConfigurationProperty("servers", IsDefaultCollection = true, IsRequired = true)]
        public MemcachedServerCollection Servers
        {
            get
            {
                return (MemcachedServerCollection)base["servers"];
            }
        }

        /// <summary>
        /// Socket连接池
        /// </summary>
        [ConfigurationProperty("socketPool", IsRequired = true)]
        public MemcachedSocketPoolElement SocketPool
        {
            get
            {
                return (MemcachedSocketPoolElement)base["socketPool"];
            }
        }
    }
}
