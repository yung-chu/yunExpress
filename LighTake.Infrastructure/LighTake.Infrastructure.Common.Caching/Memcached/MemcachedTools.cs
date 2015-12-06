using System.Configuration;
using System.Collections;

namespace LighTake.Infrastructure.Common.Caching.Memcached
{
    /// <summary>
    /// Memcached操作工具封装
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛
    /// 完成时间 : 2009年10月26日
    /// 修改历史 : 无
    /// </remarks>
    public sealed class MemcachedTools
    {
        static MemcachedClient client = null;

        static SockIOPool pool = null;

        /// <summary>
        /// 是否启用Memcached
        /// </summary>
        public static bool IsMemcachedEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// 连接池名称
        /// </summary>
        public static string SocketPoolName
        {
            get;
            set;
        }

        /// <summary>
        /// 服务器列表
        /// </summary>
        public static string[] ServerList
        {
            get;
            set;
        }

        /// <summary>
        /// Memcached客户端管理对象
        /// </summary>
        public static MemcachedClient CacheManager
        {
            get
            {
                if (client == null)
                {
                    Initialization();
                }

                return client;
            }
        }

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static MemcachedTools()
        {
            Initialization();
        }

        /// <summary>
        /// 初始化相关配置项
        /// </summary>
        static void Initialization()
        {
            #region 加载配置文件
            
            MemcachedConfigSection memcachedConfig = (MemcachedConfigSection)ConfigurationManager.GetSection("memcached");

            if (memcachedConfig == null)
            {
                throw new ConfigurationErrorsException("未找到memcached对应的配置节点,请检查配置文件!");
            }

            IsMemcachedEnabled = memcachedConfig.Enabled;

            if (memcachedConfig.Servers == null || memcachedConfig.Servers.Count < 1)
            {
                throw new ConfigurationErrorsException("未找到任何服务器配置信息,请检查配置文件!");
            }

            ServerList = new string[memcachedConfig.Servers.Count];

            for (int i = 0; i < memcachedConfig.Servers.Count; i++)
            {
                ServerList[i] = string.Concat(memcachedConfig.Servers[i].ServerIP, ":", memcachedConfig.Servers[i].Port);
            }

            if (memcachedConfig.SocketPool == null)
            {
                throw new ConfigurationErrorsException("未找到Socket连接串配置信息,请检查配置文件!");
            }

            SocketPoolName = memcachedConfig.SocketPool.PoolName;

            #endregion

            #region 初始化Socket连接串

            pool = SockIOPool.GetInstance(SocketPoolName);

            pool.SetServers(ServerList);

            // 初始化链接数
            pool.InitConnections = memcachedConfig.SocketPool.MinConnections;

            // 最少链接数
            pool.MinConnections = memcachedConfig.SocketPool.MinConnections;

            // 最大连接数
            pool.MaxConnections = memcachedConfig.SocketPool.MaxConnections;

            // Socket链接超时时间
            pool.SocketConnectTimeout = memcachedConfig.SocketPool.SocketConnectTimeout;

            // Socket超时时间
            pool.SocketTimeout = memcachedConfig.SocketPool.SocketTimeout;

            // 维护线程休眠时间
            pool.MaintenanceSleep = memcachedConfig.SocketPool.MaintenanceSleep;

            // 失效转移(一种备份操作模式)
            pool.Failover = memcachedConfig.SocketPool.Failover;

            // 是否用nagle算法启动socket
            pool.Nagle = memcachedConfig.SocketPool.Nagle;

            // 散列算法
            pool.HashingAlgorithm = HashingAlgorithm.NewCompatibleHash;

            pool.Initialize();

            #endregion

            #region 初始化Memcached客户端

            client = new MemcachedClient();

            client.PoolName = SocketPoolName;

            // 是否启用压缩
            client.EnableCompression = false;

            #endregion
        }

        /// <summary>
        /// 关闭所有的Memcached Socket连接并停止维护线程
        /// </summary>
        public static void Shutdown()
        {
            if (IsMemcachedEnabled && pool != null)
            {
                pool.Shutdown();
            }
        }

        /// <summary>
        /// 根据缓存Key获取其所存储的服务器信息
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>Key所存储的服务器信息</returns>
        public static string GetHostInfoByCacheKey(string key)
        {
            string hostInfo = "";

            SockIO sock = null;

            try
            {
                sock = SockIOPool.GetInstance(SocketPoolName).GetSock(key);

                if (sock != null)
                {
                    hostInfo = sock.Host;
                }
            }
            finally
            {
                if (sock != null)
                    sock.Close();
            }

            return hostInfo;
        }

        /// <summary>
        /// 获取服务器运行状态(服务器信息, 统计数据等)
        /// </summary>
        /// <param name="serverList">服务器列表</param>
        /// <param name="command">Stats使令</param>
        /// <param name="param">参数</param>
        /// <returns>服务器运行状态(服务器信息, 统计数据等)</returns>
        public static ArrayList GetServerRunningState(ArrayList serverList, Stats command, string param)
        {
            ArrayList statsArray = new ArrayList();

            if (!string.IsNullOrEmpty(param))
            {
                param = param.ToLower();
            }

            string commandstr = "stats";

            //转换stats命令参数
            switch (command)
            {
                case Stats.Reset: { commandstr = "stats reset"; break; }
                case Stats.Malloc: { commandstr = "stats malloc"; break; }
                case Stats.Maps: { commandstr = "stats maps"; break; }
                case Stats.Sizes: { commandstr = "stats sizes"; break; }
                case Stats.Slabs: { commandstr = "stats slabs"; break; }
                case Stats.Items: { commandstr = "stats"; break; }
                case Stats.CachedDump:
                    {
                        string[] statsparams = param.Split(' ');
                        if (statsparams.Length == 2)
                            commandstr = "stats cachedump  " + param;

                        break;
                    }
                case Stats.Detail:
                    {
                        if (string.Equals(param, "on") || string.Equals(param, "off") || string.Equals(param, "dump"))
                            commandstr = "stats detail " + param.Trim();

                        break;
                    }
                default: { commandstr = "stats"; break; }
            }

            Hashtable stats = CacheManager.Stats(serverList, commandstr);

            foreach (string key in stats.Keys)
            {
                statsArray.Add(key);

                Hashtable values = (Hashtable)stats[key];

                foreach (string key2 in values.Keys)
                {
                    statsArray.Add(key2 + ":" + values[key2]);
                }
            }

            return statsArray;
        }
    }

    /// <summary>
    /// Stats命令行参数
    /// </summary>
    public enum Stats
    {
        /// <summary>
        /// stats : 显示服务器信息, 统计数据等
        /// </summary>
        Default = 0,
        /// <summary>
        /// stats reset : 清空统计数据
        /// </summary>
        Reset = 1,
        /// <summary>
        /// stats malloc : 显示内存分配数据
        /// </summary>
        Malloc = 2,
        /// <summary>
        /// stats maps : 显示"/proc/self/maps"数据
        /// </summary>
        Maps = 3,
        /// <summary>
        /// stats sizes
        /// </summary>
        Sizes = 4,
        /// <summary>
        /// stats slabs : 显示各个slab的信息,包括chunk的大小,数目,使用情况等
        /// </summary>
        Slabs = 5,
        /// <summary>
        /// stats items : 显示各个slab中item的数目和最老item的年龄(最后一次访问距离现在的秒数)
        /// </summary>
        Items = 6,
        /// <summary>
        /// stats cachedump slab_id limit_num : 显示某个slab中的前 limit_num 个 key 列表
        /// </summary>
        CachedDump = 7,
        /// <summary>
        /// stats detail [on|off|dump] : 设置或者显示详细操作记录   on:打开详细操作记录  off:关闭详细操作记录 dump: 显示详细操作记录(每一个键值get,set,hit,del的次数)
        /// </summary>
        Detail = 8
    }
}
