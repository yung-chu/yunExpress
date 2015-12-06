using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace LighTake.Infrastructure.Common
{
    /// <summary>
    /// 字符串操作常用扩展
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年04月21日
    /// 修改历史 : 无
    /// </remarks>
    public static class Validation
    {
        #region Validation
        /// <summary>
        /// 验证指定的字符串中是否仅包含字母
        /// </summary>
        /// <param name="evalString">待验证的字符串.</param>
        /// <returns>
        /// 	<c>true</c> 如果指定的字符串中是否仅包含字母; 否则, <c>false</c>.
        /// </returns>
        public static bool IsAlpha(this string evalString)
        {
            return !Regex.IsMatch(evalString, RegexPattern.ALPHA);
        }

        /// <summary>
        /// 验证指定的字符串中是否仅包含字母和数字
        /// </summary>
        /// <param name="evalString">待验证的字符串.</param>
        /// <returns>
        /// 	<c>true</c> 如果指定的字符串中是否仅包含字母和数字; 否则, <c>false</c>.
        /// </returns>
        public static bool IsAlphaNumeric(this string evalString)
        {
            return !Regex.IsMatch(evalString, RegexPattern.ALPHA_NUMERIC);
        }

        /// <summary>
        /// 验证指定的字符串中是否仅包含字母和数字
        /// </summary>
        /// <param name="evalString">待验证的字符串.</param>
        /// <param name="allowSpaces">是否允许空格</param>
        /// <returns>
        /// 	<c>true</c> 如果指定的字符串中是否仅包含字母和数字; 否则, <c>false</c>.
        /// </returns>
        public static bool IsAlphaNumeric(this string evalString, bool allowSpaces)
        {
            if (allowSpaces)
                return !Regex.IsMatch(evalString, RegexPattern.ALPHA_NUMERIC_SPACE);
            return IsAlphaNumeric(evalString);
        }

        /// <summary>
        /// 验证指定的字符串中是否仅包含数字
        /// </summary>
        /// <param name="evalString">待验证的字符串.</param>
        /// <returns>
        /// 	<c>true</c> 如果指定的字符串中是否仅包含数字; 否则, <c>false</c>.
        /// </returns>
        public static bool IsNumeric(this string evalString)
        {
            return !Regex.IsMatch(evalString, RegexPattern.NUMERIC);
        }

        /// <summary>
        /// 验证指定的字符串中是否为有效的Email地址
        /// </summary>
        /// <param name="evalString">待验证的字符串.</param>
        /// <returns>
        /// 	<c>true</c> 如果指定的字符串中是否为有效的Email地址; 否则, <c>false</c>.
        /// </returns>
        public static bool IsEmail(this string emailAddressString)
        {
            return Regex.IsMatch(emailAddressString, RegexPattern.EMAIL);
        }

        /// <summary>
        /// 验证指定的字符串中是否仅包含小写字符
        /// </summary>
        /// <param name="evalString">待验证的字符串.</param>
        /// <returns>
        /// 	<c>true</c> 如果指定的字符串中是否仅包含小写字符; 否则, <c>false</c>.
        /// </returns>
        public static bool IsLowerCase(this string inputString)
        {
            return Regex.IsMatch(inputString, RegexPattern.LOWER_CASE);
        }

        /// <summary>
        /// 验证指定的字符串中是否仅包含大写字符
        /// </summary>
        /// <param name="evalString">待验证的字符串.</param>
        /// <returns>
        /// 	<c>true</c> 如果指定的字符串中是否仅包含大写字符; 否则, <c>false</c>.
        /// </returns>
        public static bool IsUpperCase(this string inputString)
        {
            return Regex.IsMatch(inputString, RegexPattern.UPPER_CASE);
        }

        /// <summary>
        /// 验证指定的字符串中是否为有效的Guid
        /// </summary>
        /// <param name="evalString">待验证的字符串.</param>
        /// <returns>
        /// 	<c>true</c> 如果指定的字符串中是否为有效的Guid; 否则, <c>false</c>.
        /// </returns>
        public static bool IsGuid(this string guid)
        {
            return Regex.IsMatch(guid, RegexPattern.GUID);
        }

        /// <summary>
        /// 验证指定的字符串中是否为有效的IP地址
        /// </summary>
        /// <param name="evalString">待验证的字符串.</param>
        /// <returns>
        /// 	<c>true</c> 如果指定的字符串中是否为有效的IP地址; 否则, <c>false</c>.
        /// </returns>
        public static bool IsIPAddress(this string ipAddress)
        {
            return Regex.IsMatch(ipAddress, RegexPattern.IP_ADDRESS);
        }

        /// <summary>
        /// 验证指定的字符串中是否为有效的URL
        /// </summary>
        /// <param name="evalString">待验证的字符串.</param>
        /// <returns>
        /// 	<c>true</c> 如果指定的字符串中是否为有效的URL; 否则, <c>false</c>.
        /// </returns>
        public static bool IsURL(this string url)
        {
            return Regex.IsMatch(url, RegexPattern.URL);
        }

        /// <summary>
        /// 验证指定的字符串中是否为有效的强密码串
        /// </summary>
        /// <param name="evalString">待验证的字符串.</param>
        /// <returns>
        /// 	<c>true</c> 如果指定的字符串中是否为有效的强密码串; 否则, <c>false</c>.
        /// </returns>
        public static bool IsStrongPassword(this string password)
        {
            return Regex.IsMatch(password, RegexPattern.STRONG_PASSWORD);
        }

        #endregion
    }
}
