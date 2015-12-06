using System;

namespace LighTake.Infrastructure.Common.Logging
{
    /// <summary>
    /// 通用的日志处理接口
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年8月3日
    /// 修改历史 : 无
    /// </remarks>
    public interface ILog
    {
        /// <summary>
        /// 记录类型为[Info]的日志信息
        /// </summary>
        /// <param name="message">日志内容</param>
        void Info(string message);

        /// <summary>
        /// 记录类型为[Debug]的日志信息
        /// </summary>
        /// <param name="message">日志内容</param>
        void Debug(string message);

        /// <summary>
        /// 记录类型为[Warning]的日志信息
        /// </summary>
        /// <param name="message">日志内容</param>
        void Warning(string message);

        /// <summary>
        /// 记录类型为[Error]的日志信息
        /// </summary>
        /// <param name="message">日志内容</param>
        void Error(string message);

        /// <summary>
        /// 记录类型为[Exception]的日志信息
        /// </summary>
        /// <param name="exception">异常信息</param>
        void Exception(Exception exception);
    }
}
