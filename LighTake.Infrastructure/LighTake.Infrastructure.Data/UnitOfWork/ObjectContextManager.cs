using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Text;
using System.Threading;

using System.Diagnostics;
using System.Web;
using System.Data.EntityClient;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Web.Utities;


namespace LighTake.Infrastructure.Data
{
    /// <summary>
    /// 实体数据上下文管理抽象基类
    /// </summary>
    /// <remarks>
    /// 完成时间 : 2010年11月19日
    /// 修改历史 : 无
    /// </remarks>
    public class ObjectContextManager<T> where T : ObjectContext, new()
    {
        private object _lockObject;

        /// <summary>
        /// 返回一个ObjectContext实例
        /// </summary>
        public virtual T ObjectContext
        {
            get
            {

                string contextKey;

                if (HttpContext.Current != null)
                {
                    contextKey = string.Format(Constants.OBJECT_CONTEXT_KEY, HttpContext.Current.GetHashCode().ToString("x"));
                }
                else
                {
                    Thread thread = Thread.CurrentThread;
                    if (string.IsNullOrWhiteSpace(thread.Name))
                    {
                        thread.Name = Guid.NewGuid().ToString();
                    }

                    contextKey = string.Format(Constants.OBJECT_CONTEXT_KEY, thread.Name.GetHashCode().ToString("x"));

                }


                T context = Local.Data[contextKey] as T;

                if (context == null)
                {
                    context = Activator.CreateInstance(typeof(T)) as T;
                    Debug.WriteLine("ObjectContextManager: Created new ObjectContext");
                    Local.Data[contextKey] = context;
                }

                return context;
            }
        }
    }
}
