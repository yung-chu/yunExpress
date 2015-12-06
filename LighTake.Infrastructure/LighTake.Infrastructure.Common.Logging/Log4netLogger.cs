using System;
using System.IO;
using log4net;

namespace LighTake.Infrastructure.Common.Logging
{
    /// <summary>
    /// 封装基于Log4net的日志处理
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年8月3日
    /// 修改历史 : 无
    /// </remarks>
    public class Log4netLogger : ILog
    {
        private readonly log4net.ILog _logger;

        public Log4netLogger()
            : this("DefaultLogger")
        {

        }

        public Log4netLogger(string loggerName)
            : this(loggerName, string.Empty)
        {
        }

        public Log4netLogger(string loggerName, string configPath)
        {
            if (configPath.IsNullOrEmpty())
            {
                log4net.Config.XmlConfigurator.Configure();
            }
            else
            {
                log4net.Config.XmlConfigurator.Configure(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configPath)));
            }

            _logger = LogManager.GetLogger(loggerName);
        }

        #region ILog 成员

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Warning(string message)
        {
            _logger.Warn(message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Exception(Exception exception)
        {
            _logger.Error("出现异常:", exception);
        }

        #endregion
    }
}
