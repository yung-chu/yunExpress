using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Data
{
    /// <summary>
    /// 数据访问框架约定的保留字段
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年09月3日
    /// 修改历史 : 无
    /// </remarks>
    public class ReservedColumns
    {
        /// <summary>
        /// 创建人
        /// </summary>
        public const string CREATED_BY = "CreatedBy";
        /// <summary>
        /// 创建时间
        /// </summary>
        public const string CREATED_ON = "CreatedOn";
        /// <summary>
        /// 最后更新人
        /// </summary>
        public const string LAST_UPDATED_BY = "LastUpdatedBy";
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public const string LAST_UPDATED_ON = "LastUpdatedOn";
        /// <summary>
        /// 逻辑删除标识
        /// </summary>
        public const string IS_DELETED = "IsDeleted";
    }
}
