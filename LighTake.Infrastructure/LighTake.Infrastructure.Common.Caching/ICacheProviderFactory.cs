namespace LighTake.Infrastructure.Common.Caching
{
    /// <summary>
    /// 缓存提供程序构建工厂
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年7月20日
    /// 修改历史 : 无
    /// </remarks>
    public interface ICacheProviderFactory
    {
        /// <summary>
        /// 创建缓存提供程序实例
        /// </summary>
        /// <returns>缓存提供程序实例</returns>
        ICache CreateInstance();
    }
}
