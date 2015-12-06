using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace LighTake.Infrastructure.Common
{
    /// <summary>
    /// 常用约束检查
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2009年08月19日
    /// 修改历史 : 无
    /// </remarks>
    public class Check
    {
        internal Check()
        {
        }

        /// <summary>
        /// 参数有效性约束检查
        /// </summary>
        public class Argument
        {
            internal Argument()
            {
            }

            [DebuggerStepThrough]
            public static void IsNotEmpty(Guid argument, string argumentName)
            {
                if (argument == Guid.Empty)
                {
                    throw new ArgumentException("参数: \"{0}\" 不能为空GUID.".FormatWith(argumentName), argumentName);
                }
            }

            /// <summary>
            /// 不能为空
            /// </summary>
            /// <param name="argument"></param>
            /// <param name="argumentName"></param>
            [DebuggerStepThrough]
            public static void IsNotEmpty(string argument, string argumentName)
            {
                if (string.IsNullOrEmpty((argument ?? string.Empty).Trim()))
                {
                    throw new ArgumentException("参数: \"{0}\" 不能为空.".FormatWith(argumentName), argumentName);
                }
            }

            /// <summary>
            /// 不能为Null、空或空白
            /// </summary>
            /// <param name="argument"></param>
            /// <param name="argumentName"></param>
            [DebuggerStepThrough]
            public static void IsNullOrWhiteSpace(string argument, string argumentName)
            {
                if (string.IsNullOrWhiteSpace((argument ?? string.Empty).Trim()))
                {
                    throw new ArgumentException("参数: \"{0}\" 不能为空或空白.".FormatWith(argumentName), argumentName);
                }
            }

            /// <summary>
            /// 不能超过{1}个字符
            /// </summary>
            /// <param name="argument"></param>
            /// <param name="length"></param>
            /// <param name="argumentName"></param>
            [DebuggerStepThrough]
            public static void IsNotOutOfLength(string argument, int length, string argumentName)
            {
                if (argument.Trim().Length > length)
                {
                    throw new ArgumentException("参数:　\"{0}\" 不能超过{1}个字符.".FormatWith(argumentName, length), argumentName);
                }
            }

            /// <summary>
            /// 不能为空
            /// </summary>
            /// <param name="argument"></param>
            /// <param name="argumentName"></param>
            [DebuggerStepThrough]
            public static void IsNotNull(object argument, string argumentName)
            {
                if (argument == null)
                {
                    throw new ArgumentNullException(argumentName, "参数:　\"{0}\" 不能为空.".FormatWith(argumentName));
                }
            }

            /// <summary>
            /// 不能小于零
            /// </summary>
            /// <param name="argument"></param>
            /// <param name="argumentName"></param>
            [DebuggerStepThrough]
            public static void IsNotNegative(int argument, string argumentName)
            {
                if (argument < 0)
                {
                    throw new ArgumentOutOfRangeException(argumentName, "参数:　\"{0}\" 不能小于零.".FormatWith(argumentName));
                }
            }

            /// <summary>
            /// 必须等于零
            /// </summary>
            /// <param name="argument"></param>
            /// <param name="argumentName"></param>
            [DebuggerStepThrough]
            public static void IsNotZero(int argument, string argumentName)
            {
                if (argument == 0)
                {
                    throw new ArgumentOutOfRangeException(argumentName, "参数:　\"{0}\" 必须等于零.".FormatWith(argumentName));
                }
            }

            /// <summary>
            /// 必须等于零
            /// </summary>
            /// <param name="argument"></param>
            /// <param name="argumentName"></param>
            [DebuggerStepThrough]
            public static void IsNotZero(decimal argument, string argumentName)
            {
                if (argument == 0)
                {
                    throw new ArgumentOutOfRangeException(argumentName, "参数:　\"{0}\" 必须等于零.".FormatWith(argumentName));
                }
            }

            /// <summary>
            /// 必须大于零
            /// </summary>
            /// <param name="argument"></param>
            /// <param name="argumentName"></param>
            [DebuggerStepThrough]
            public static void IsNotNegativeOrZero(int argument, string argumentName)
            {
                if (argument <= 0)
                {
                    throw new ArgumentOutOfRangeException(argumentName, "参数:　\"{0}\" 必须大于零.".FormatWith(argumentName));
                }
            }

            /// <summary>
            /// 必须大于零
            /// </summary>
            /// <param name="argument"></param>
            /// <param name="argumentName"></param>
            [DebuggerStepThrough]
            public static void IsNotNegativeOrZero(decimal argument, string argumentName)
            {
                if (argument <= 0)
                {
                    throw new ArgumentOutOfRangeException(argumentName, "参数:　\"{0}\" 必须大于零.".FormatWith(argumentName));
                }
            }

            [DebuggerStepThrough]
            public static void IsNotInPast(DateTime argument, string argumentName)
            {
                if (argument < DateTime.Now)
                {
                    throw new ArgumentOutOfRangeException(argumentName);
                }
            }

            [DebuggerStepThrough]
            public static void IsNotInFuture(DateTime argument, string argumentName)
            {
                if (argument > DateTime.Now)
                {
                    throw new ArgumentOutOfRangeException(argumentName);
                }
            }

            [DebuggerStepThrough]
            public static void IsNotNegative(TimeSpan argument, string argumentName)
            {
                if (argument < TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(argumentName);
                }
            }

            [DebuggerStepThrough]
            public static void IsNotNegativeOrZero(TimeSpan argument, string argumentName)
            {
                if (argument <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(argumentName);
                }
            }

            [DebuggerStepThrough]
            public static void IsNotEmpty<T>(ICollection<T> argument, string argumentName)
            {
                IsNotNull(argument, argumentName);

                if (argument.Count == 0)
                {
                    throw new ArgumentException("集合不能为空.", argumentName);
                }
            }

            [DebuggerStepThrough]
            public static void IsNotOutOfRange(int argument, int min, int max, string argumentName)
            {
                if ((argument < min) || (argument > max))
                {
                    throw new ArgumentOutOfRangeException(argumentName, "参数: {0} 必须在\"{1}\"-\"{2}\"之间.".FormatWith(argumentName, min, max));
                }
            }

            [DebuggerStepThrough]
            public static void IsNotInvalidEmail(string argument, string argumentName)
            {
                IsNotEmpty(argument, argumentName);

                if (!argument.IsEmail())
                {
                    throw new ArgumentException("参数: \"{0}\" 不是一个有效的电子邮件地址.".FormatWith(argumentName), argumentName);
                }
            }
        }
    }
}
