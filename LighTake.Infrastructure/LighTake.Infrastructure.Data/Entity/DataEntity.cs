using System;
using LighTake.Infrastructure.Common;

namespace LighTake.Infrastructure.Data.Entity
{
    /// <summary>
    /// 表示一个可以持久化到数据库业务实体 
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年07月20日
    /// 修改历史 : 无
    /// </remarks>
    public sealed class DataEntity : IEntity
    {
        public DataEntity()
        {
            CreateOn = DateTime.Now;
            CreateBy = string.Empty;
            LastUpdatedOn = DateTime.Now;
            LastUpdatedBy = string.Empty;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateOn 
        { 
            get; 
            protected set; 
        }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy
        {
            get;
            protected set;
        }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdatedOn
        {
            get;
            protected set;
        }

        /// <summary>
        /// 最后更新人
        /// </summary>
        public string LastUpdatedBy
        {
            get;
            protected set;
        } 
    }
}
