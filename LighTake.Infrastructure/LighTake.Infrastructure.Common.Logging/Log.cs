using System;

namespace LighTake.Infrastructure.Common.Logging
{
    /// <summary>
    /// 通用的日志处理操作
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年8月3日
    /// 修改历史 : 无
    /// </remarks>
    public static class Log
    {
        /// <summary>
        /// 记录类型为[Info]的日志信息
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            //Check.Argument.IsNotEmpty(message, "message");

            GetLogger().Info(message);
        }

        /// <summary>
        /// 记录类型为[Debug]的日志信息
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Debug(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            //Check.Argument.IsNotEmpty(message, "message");

            GetLogger().Debug(message);
        }

        /// <summary>
        /// 记录类型为[Warning]的日志信息
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Warning(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            //Check.Argument.IsNotEmpty(message, "message");

            GetLogger().Warning(message);
        }

        /// <summary>
        /// 记录类型为[Error]的日志信息
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Error(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            //Check.Argument.IsNotEmpty(message, "message");

            GetLogger().Error(message);
        }

        /// <summary>
        /// 记录异常日志信息
        /// </summary>
        /// <param name="exception">异常信息</param>
        public static void Exception(Exception exception)
        {           
            Check.Argument.IsNotNull(exception, "message");

            GetLogger().Exception(exception);

            //如有内部异常，也记录
            if (exception.InnerException != null)
            {
                GetLogger().Exception(exception.InnerException);
            }
        }

        private static ILog GetLogger()
        {
            return new Log4netLogger();
        }
    }
}
