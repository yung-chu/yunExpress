using System.Configuration;

namespace LighTake.Infrastructure.Common.Caching.Memcached
{
    /// <summary>
    /// 功能描述 : Memcached配置节点-服务器配置集合
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛
    /// 完成时间 : 2009年10月23日
    /// 修改历史 : 无
    /// </remarks>
    public class MemcachedServerCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MemcachedServerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MemcachedServerElement)element).ServerIP;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        protected override string ElementName
        {
            get
            {
                return "server";
            }
        }

        public MemcachedServerElement this[int index]
        {
            get
            {
                return (MemcachedServerElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

    }
}
