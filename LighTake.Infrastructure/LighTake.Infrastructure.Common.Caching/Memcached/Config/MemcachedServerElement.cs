using System.Configuration;

namespace LighTake.Infrastructure.Common.Caching.Memcached
{
    /// <summary>
    /// 功能描述 : Memcached配置节点-服务器配置
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛
    /// 完成时间 : 2009年10月23日
    /// 修改历史 : 无
    /// </remarks>
    public class MemcachedServerElement : ConfigurationElement
    {
        /// <summary>
        /// 服务器IP地址
        /// </summary>
        [ConfigurationProperty("serverIP", IsKey = true, IsRequired = true)]
        public string ServerIP
        {
            get
            {
                return (string)base["serverIP"];
            }
            set
            {
                base["serverIP"] = value;
            }
        }

        /// <summary>
        /// 监听端口
        /// </summary>
        [ConfigurationProperty("port", IsRequired = true)]
        public string Port
        {
            get
            {
                return (string)base["port"];
            }
            set
            {
                base["port"] = value;
            }
        }
    }
}
