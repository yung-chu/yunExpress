using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Common
{
    /// <summary>
    /// 数据访问过程中发生的异常
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2009年11月28日
    /// 修改历史 : 无
    /// </remarks>
    [Serializable]
    public class DataAccessException : InfrastructureException
    {
        /// <summary>
        /// 创建数据访问异常实例
        /// </summary>
        public DataAccessException()
            : base("数据访问异常!")
        {

        }

        /// <summary>
        /// 创建数据访问异常实例
        /// </summary>
        /// <param name="message">异常信息</param>
        public DataAccessException(string message)
            : base("数据访问异常 : {0}".FormatWith(message))
        {

        }

        /// <summary>
        /// 创建数据访问异常实例
        /// </summary>
        /// <param name="message">异常信息</param>
        /// <param name="innerException">内部异常</param>
        public DataAccessException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
