using System;
using System.Web;

namespace LighTake.Infrastructure.Web
{
    /// <summary>
    /// 用于处理PING请求
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年07月28日
    /// 修改历史 : 无
    /// </remarks>
    public class Ping : IHttpHandler
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Ping");
        }

        #endregion
    }
}
