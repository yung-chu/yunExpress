using System;
using System.Diagnostics;

namespace LighTake.Infrastructure.Common
{
    /// <summary>
    /// 表示一个可回收的资源
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年8月3日
    /// 修改历史 : 无
    /// </remarks>
    public abstract class DisposableResource : IDisposable
    {
        ~DisposableResource()
        {
            Dispose(false);
        }

        [DebuggerStepThrough]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
