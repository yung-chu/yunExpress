using System;
using System.Configuration;

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
    public class CacheProviderFactory : ICacheProviderFactory
    {
        private readonly Type _cacheProviderType;

        public CacheProviderFactory(string cacheProviderType)
        {
            Check.Argument.IsNotEmpty(cacheProviderType, "cacheProviderType");

            _cacheProviderType = Type.GetType(cacheProviderType, true, true);
        }

        public CacheProviderFactory()
            : this(ConfigurationManager.AppSettings["CacheProvider"])
        {
        }

        public ICache CreateInstance()
        {
            return Activator.CreateInstance(_cacheProviderType) as ICache;
        }
    }
}
