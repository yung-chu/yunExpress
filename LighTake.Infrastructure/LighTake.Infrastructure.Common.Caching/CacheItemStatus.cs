using System;

namespace LighTake.Infrastructure.Common.Caching
{
    /// <summary>
    /// 缓存项的状态信息
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年7月20日
    /// 修改历史 : 无
    /// </remarks>
    [Serializable]
    public class CacheItemStatus
    {
        /// <summary>
        /// 缓存项类型
        /// </summary>
        public Type ItemType
        {
            get;
            set;
        }

        /// <summary>
        /// 缓存项添加时间
        /// </summary>
        public DateTime AddTime
        {
            get;
            set;
        }

        public DateTime ExpireTime
        {
            get;
            set;
        }
    }
}
