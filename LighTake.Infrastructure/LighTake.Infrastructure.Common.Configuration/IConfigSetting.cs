using System;
using System.Collections.Generic;
using System.Text;

namespace LighTake.Infrastructure.Common.Configuration
{
    /// <summary>
    /// 表示一个配置节点
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年04月21日
    /// 修改历史 : 无
    /// </remarks>
    public interface IConfigSetting
    {
        /// <summary>
        /// 配置节点名称
        /// </summary>
        string Name
        {
            get;
        }
    }
}
