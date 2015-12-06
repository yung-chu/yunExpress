using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace LighTake.Infrastructure.Common.Configuration
{
    /// 配置节点集合
    /// </summary>
    /// <typeparam name="T">配置节点类型</typeparam>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年04月21日
    /// 修改历史 : 无
    /// </remarks>
    [Serializable]
    public class ConfigSettingCollection<T> : CollectionBase where T : IConfigSetting, new()
    {
        /// <summary>
        /// 添加一个配置节点到配置节点集合中
        /// </summary>
        /// <param name="item">待添加的配置节点</param>
        /// <returns>新节点中集合中的索引位置</returns>
        public int Add(T item)
        {
            return this.List.Add(item);
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="i">索引</param>
        /// <returns>指定索引位置的配置节点</returns>
        public T this[int i]
        {
            get
            {
                return (T)this.List[i];
            }
        }
    }
}
