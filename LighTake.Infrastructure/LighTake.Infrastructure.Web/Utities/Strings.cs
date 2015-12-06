using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using LighTake.Infrastructure.Common;

namespace LighTake.Infrastructure.Web.Utities
{
    public static class Strings
    {
        #region 字符串的编码

        /// <summary>
        /// 返回 URL 字符串的编码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string GetUrlEncode(this string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        /// <summary>
        /// 返回 URL 字符串的解码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string GetUrlDecode(this string str)
        {
            return HttpUtility.UrlDecode(str);
        }

        #endregion

        #region 表单及网络安全相关
        /// <summary>
        /// 获得用户IP
        /// </summary>
        public static string GetUserIp(this string source)
        {
            string ip;
            string[] temp;
            bool isErr = false;
            if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_ForWARDED_For"] == null)
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            else
                ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_ForWARDED_For"].ToString();
            if (ip.Length > 15)
                isErr = true;
            else
            {
                temp = ip.Split('.');
                if (temp.Length == 4)
                {
                    for (int i = 0; i < temp.Length; i++)
                    {
                        if (temp[i].Length > 3) isErr = true;
                    }
                }
                else
                    isErr = true;
            }

            if (isErr)
                return "1.1.1.1";
            else
                return ip;
        }

        /// <summary>
        /// 获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">参数</param>
        /// <returns>Url或表单参数的值</returns>
        public static string GetString(this string strName)
        {
            return strName.GetString(false);
        }

        /// <summary>
        /// 获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>Url或表单参数的值</returns>
        public static string GetString(this string strName, bool sqlSafeCheck)
        {
            if ("".Equals(GetQueryString(strName)))
            {
                return GetFormString(strName, sqlSafeCheck);
            }
            else
            {
                return GetQueryString(strName, sqlSafeCheck);
            }
        }

        /// <summary>
        /// 获得指定Url参数的值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <returns>Url参数的值</returns>
        public static string GetQueryString(this string strName)
        {
            return strName.GetQueryString(false);
        }

        /// <summary>
        /// 获得指定Url参数的值
        /// </summary> 
        /// <param name="strName">Url参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>Url参数的值</returns>
        public static string GetQueryString(this string strName, bool sqlSafeCheck)
        {
            if (HttpContext.Current.Request.QueryString[strName] == null)
            {
                return "";
            }

            if (sqlSafeCheck && !IsSafeSqlString(HttpContext.Current.Request.QueryString[strName]))
            {
                return "unsafe string";
            }

            return HttpContext.Current.Request.QueryString[strName];
        }

        /// <summary>
        /// 获得指定表单参数的值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <returns>表单参数的值</returns>
        public static string GetFormString(this string strName)
        {
            return strName.GetQueryString(false);
        }

        /// <summary>
        /// 获得指定表单参数的值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>表单参数的值</returns>
        public static string GetFormString(this string strName, bool sqlSafeCheck)
        {
            if (HttpContext.Current.Request.Form[strName] == null)
            {
                return "";
            }

            if (sqlSafeCheck && !IsSafeSqlString(HttpContext.Current.Request.Form[strName]))
            {
                return "unsafe string";
            }

            return HttpContext.Current.Request.Form[strName];
        }

        /// <summary>
        /// 检测是否有Sql危险字符
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSqlString(string str)
        {

            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

        /// <summary>
        /// 检测是否有危险的可能用于链接的字符串
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeUserInfoString(string str)
        {
            return !Regex.IsMatch(str, @"^\s*$|^c:\\con\\con$|[%,\*" + "\"" + @"\s\t\<\>\&]|^Guest");
        }

        /// <summary>
        /// 对指定字符串进行MD5加密
        /// </summary>
        /// <returns>进行MD5加密后的字符串</returns>
        public static string ToMD5(this string source)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(source, "MD5");
        }

        /// <summary>
        /// DES加密指定字符串
        /// </summary>
        /// <param name="source">待加密字符串</param>
        /// <returns>进行DES加密后的字符串</returns>
        public static string EncryptDES(this string source)
        {
            if (source.IsNullOrEmpty())
                return source;

            return SecurityUtil.EncryptDES(source, Constants.ENCRYPT_KEY);
        }

        /// <summary>
        /// DES加密指定字符串
        /// </summary>
        /// <param name="source">待加密字符串</param>
        /// <returns>进行DES加密后的字符串</returns>
        public static string DecryptDES(this string source)
        {
            if (source.IsNullOrEmpty())
                return source;

            return SecurityUtil.DecryptDES(source, Constants.ENCRYPT_KEY);
        }

        /// <summary>
        /// 字节数组转换为字符串
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns>字符串</returns>
        public static string ToBinaryString(this byte[] bytes)
        {
            return System.Text.Encoding.ASCII.GetString(bytes, 0, bytes.Length);
        }


        #endregion
    }
}
