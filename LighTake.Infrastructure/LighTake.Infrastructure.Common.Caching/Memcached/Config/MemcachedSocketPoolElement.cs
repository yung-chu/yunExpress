using System.Configuration;

namespace LighTake.Infrastructure.Common.Caching.Memcached
{
    /// <summary>
    /// 功能描述 : Memcached配置节点-Socket连接池配置
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛
    /// 完成时间 : 2009年10月23日
    /// 修改历史 : 无
    /// </remarks>
    public class MemcachedSocketPoolElement : ConfigurationElement
    {
        /// <summary>
        /// Socket连接池名称
        /// </summary>
        [ConfigurationProperty("poolName", IsKey = true, DefaultValue = "default", IsRequired = true)]
        public string PoolName
        {
            get
            {
                return (string)base["poolName"];
            }
            set
            {
                base["poolName"] = value;
            }
        }

        /// <summary>
        /// 最小连接数量
        /// </summary>
        [ConfigurationProperty("minConnections", DefaultValue = 3, IsRequired = true)]
        public int MinConnections
        {
            get
            {
                return (int)base["minConnections"];
            }
            set
            {
                base["minConnections"] = value;
            }
        }

        /// <summary>
        /// 最大连接数量
        /// </summary>
        [ConfigurationProperty("maxConnections", DefaultValue = 5, IsRequired = true)]
        public int MaxConnections
        {
            get
            {
                return (int)base["maxConnections"];
            }
            set
            {
                base["maxConnections"] = value;
            }
        }

        /// <summary>
        /// Socket连接超时时间
        /// </summary>
        [ConfigurationProperty("socketConnectTimeout", DefaultValue = 1000, IsRequired = true)]
        public int SocketConnectTimeout
        {
            get
            {
                return (int)base["socketConnectTimeout"];
            }
            set
            {
                base["socketConnectTimeout"] = value;
            }
        }

        /// <summary>
        /// Socket处理超时时间
        /// </summary>
        [ConfigurationProperty("socketTimeout", DefaultValue = 3000, IsRequired = true)]
        public int SocketTimeout
        {
            get
            {
                return (int)base["socketTimeout"];
            }
            set
            {
                base["socketTimeout"] = value;
            }
        }

        /// <summary>
        /// 维护线程休眠时间
        /// </summary>
        [ConfigurationProperty("maintenanceSleep", DefaultValue = 30, IsRequired = true)]
        public int MaintenanceSleep
        {
            get
            {
                return (int)base["maintenanceSleep"];
            }
            set
            {
                base["maintenanceSleep"] = value;
            }
        }

        /// <summary>
        /// 是否启用故障转移
        /// </summary>
        [ConfigurationProperty("failover", DefaultValue = true, IsRequired = true)]
        public bool Failover
        {
            get
            {
                return (bool)base["failover"];
            }
            set
            {
                base["failover"] = value;
            }
        }

        /// <summary>
        /// 是否为Socket连接池使用Nagle算法
        /// </summary>
        [ConfigurationProperty("nagle", DefaultValue= true, IsRequired = true)]
        public bool Nagle
        {
            get
            {
                return (bool)base["nagle"];
            }
            set
            {
                base["nagle"] = value;
            }
        }
    }
}
