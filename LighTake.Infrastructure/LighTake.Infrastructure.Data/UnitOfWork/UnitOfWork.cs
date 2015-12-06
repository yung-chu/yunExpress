using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;
using System.Data.Objects;

namespace LighTake.Infrastructure.Data
{
    /// <summary>
    /// 实现工作单元模式 用于管理实体对象上下文
    /// </summary>
    /// <remarks>
    /// 完成时间 : 2010年07月20日
    /// 修改历史 : 无
    /// </remarks>
    public class UnitOfWork<T> where T : ObjectContext, new()
    {
        static ObjectContextManager<T> _objectContextManager;

        static ObjectContextManager<T> ObjectContextManager
        {
            get 
            {
                if (_objectContextManager == null)
                {
                    _objectContextManager = new ObjectContextManager<T>();
                }

                return _objectContextManager;
            }
        }

        public static T CurrentContext
        {
            get
            {
                return ObjectContextManager.ObjectContext;
            }
        }
    }
}
