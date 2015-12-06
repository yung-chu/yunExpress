using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Diagnostics;
using LighTake.Infrastructure.Common;


namespace LighTake.Infrastructure.Data
{
    /// <summary>
    /// 实体框架相关扩展
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年11月20日
    /// 修改历史 : 无
    /// </remarks>
    public static class ObjectContextExtensions
    {
        /// <summary>
        /// 判断指定实体对象是否关联到当前上下文中
        /// </summary>
        /// <param name="context">实体对象上下文</param>
        /// <param name="entity">数据实体</param>
        /// <returns>TRUE:存在,否则FALSE</returns>
        public static bool IsAttached(this ObjectContext context, object entity)
        {
            Check.Argument.IsNotNull(entity, "entity");

            ObjectStateEntry entry;

            try
            {
                entry = context.ObjectStateManager.GetObjectStateEntry(entity);

                return entry.State != EntityState.Detached;
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.ToString());
            }

            return false;
        }
    }
}
