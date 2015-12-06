using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Common.Logging
{
    public interface ILoggerService<T>
    {
        bool IsEnabled(LogLevel level);

        void DeleteLog(T log);

        void ClearLog();

        IPagedList<T> GetPagedLogs<P>(P param);

        T GetLogById(int logId);

        IList<T> GetLogsByIds(int[] logIds);

        T AddLog(LogLevel level, string shortMessage, string fullMessage = null, string loginIdentity = null);
    }
}
