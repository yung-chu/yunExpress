using System;
using System.Configuration;

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
    public class DependencyResolverFactory : IDependencyResolverFactory
    {
        private readonly Type _resolverType;
        private readonly string _configPath;

        public DependencyResolverFactory(string resolverTypeName, string configPath)
        {
            Check.Argument.IsNotEmpty(resolverTypeName, "resolverTypeName");

            _resolverType = Type.GetType(resolverTypeName, true, true);

            _configPath = configPath;
        }

        public DependencyResolverFactory()
            : this(ConfigurationManager.AppSettings["DependencyResolverContainer"], "Windsor.config")
        {
        }

        public IDependencyResolver CreateInstance()
        {
            return Activator.CreateInstance(_resolverType, _configPath) as IDependencyResolver;
        }
    }
}
