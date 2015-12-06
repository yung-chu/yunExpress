using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Text;
using System.Threading;
using System.Web;
using System.Diagnostics;
using LighTake.Infrastructure.Common;

namespace LighTake.Infrastructure.Web.Modules
{
    /// <summary>
    /// 用于释放每个HTTP请求中共享的ObjectContext实例
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年07月28日
    /// 修改历史 : 无
    /// </remarks>
    public class ObjectContextDisposeModule : IHttpModule
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context">应用程序上下文</param>
        public void Init(HttpApplication context)
        {
            context.EndRequest += new EventHandler(Context_EndRequest);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            
        }

        /// <summary> 
        /// HTTP请求结束时调用
        /// </summary>
        private void Context_EndRequest(object sender, EventArgs e)
        {
            DisposeObjectContext();
        }

        /// <summary>
        /// 释放当前请求中共享的ObjectContext实例
        /// </summary>
        public static void DisposeObjectContext()
        {
            if (HttpContext.Current == null)
                return;

            string contextKey = string.Format(Constants.OBJECT_CONTEXT_KEY, HttpContext.Current.GetHashCode().ToString("x"));

            if (HttpContext.Current.Items.Contains(contextKey))
            {
                ObjectContext objectContext = HttpContext.Current.Items[contextKey] as ObjectContext;

                if (objectContext != null)
                    objectContext.Dispose();

                HttpContext.Current.Items.Remove(contextKey);

                Debug.WriteLine("ObjectContextManager: Disposed ObjectContext");
            }
        }
    }
}
