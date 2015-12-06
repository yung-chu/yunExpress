namespace LighTake.Infrastructure.Web.Utities
{
    /// <summary>
    /// 定义统一接口用于在当前上下文中存储/读取数据
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年07月20日
    /// 修改历史 : 无
    /// </remarks>
    public interface ILocalData
    {
        /// <summary>
        /// 根据指定的键值获取对应的数据
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>指定的键值对应的数据</returns>
        object this[object key] { get; set; }

        /// <summary>
        /// 该实例中存储的数据数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 清除所有数据
        /// </summary>
        void Clear();
    }
}
