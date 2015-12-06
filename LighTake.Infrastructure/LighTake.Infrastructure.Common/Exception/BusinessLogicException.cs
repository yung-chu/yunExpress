using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Common
{
    /// <summary>
    /// 业务逻辑执行过程中发生的异常
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2009年11月28日
    /// 修改历史 : 无
    /// </remarks>
    [Serializable]
    public class BusinessLogicException : Exception
    {
        /// <summary>
        /// 创建业务逻辑异常实例
        /// </summary>
        public BusinessLogicException()
            : base("业务逻辑执行异常!")
        {

        }

        /// <summary>
        /// 创建业务逻辑异常实例
        /// </summary>
        /// <param name="message">异常信息</param>
        public BusinessLogicException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// 创建业务逻辑异常实例
        /// </summary>
        /// <param name="message">异常信息</param>
        /// <param name="innerException">内部异常</param>
        public BusinessLogicException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
