namespace LighTake.Infrastructure.Common.InversionOfControl.Castle
{
    /// <summary>
    /// 依赖解析器构建工厂
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年7月20日
    /// 修改历史 : 无
    /// </remarks>
    public interface IDependencyResolverFactory
    {
        /// <summary>
        /// 创建依赖注入解析器实例
        /// </summary>
        /// <returns>依赖注入解析器实例</returns>
        IDependencyResolver CreateInstance();
    }
}
