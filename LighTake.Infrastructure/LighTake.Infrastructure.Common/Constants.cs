using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Common
{
    /// <summary>
    /// 通用常量
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// 数据库分片配置文件
        /// </summary>
        public const string SHARDING_CONFIG_FILE = "ShardingSchema.Config.xml";

        /// <summary>
        /// 逻辑方法映射配置文件
        /// </summary>
        public const string LOGICMAPPING_CONFIG_FILE = "LogicMapping.Config.xml";

        /// <summary>
        /// 系统出错消息
        /// </summary>
        public const string SYSTEM_EXCEPTION_MESSAGE = "服务器端出现异常!";

        /// <summary>
        /// 实体数据上下文
        /// </summary>
        public const string OBJECT_CONTEXT_KEY = "OBJECT_CONTEXT_{0}";

        /// <summary>
        /// 加密密钥
        /// </summary>
        public const string ENCRYPT_KEY = "LIGHTAKEWEBKEYS";


        /// <summary>
        /// 程序集所在路径
        /// </summary>
        public static string AssemblyPath
        {
            get
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + @"Bin\";  // Web Site

                if (Environment.CurrentDirectory == AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\')) // Others
                {
                    path = AppDomain.CurrentDomain.BaseDirectory;
                }

                if (Environment.CurrentDirectory.Equals(@"C:\WINDOWS\system32")) // Service
                {
                    path = AppDomain.CurrentDomain.BaseDirectory;
                }

                return path;
            }
        }
    }
}
