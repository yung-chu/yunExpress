using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;


namespace LighTake.Infrastructure.Web.Utities
{
    public static class WebTools
    {
        #region 读写Cookie
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName] ?? new HttpCookie(strName);
            cookie.Value = HttpUtility.UrlEncode(strValue);
            HttpContext.Current.Response.AppendCookie(cookie);

        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="key"> </param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string key, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName] ?? new HttpCookie(strName);
            cookie[key] = HttpUtility.UrlEncode(strValue);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="key">键</param>
        /// <param name="strValue">值</param>
        /// <param name="expires">过期时间(单位:分钟)</param>
        public static void WriteCookie(string strName, string key, string strValue, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName] ?? new HttpCookie(strName);
            cookie[key] = HttpUtility.UrlEncode(strValue);
            cookie.Expires = DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="key"> </param>
        /// <param name="strValue">值</param>
        /// <param name="doMain"> </param>
        public static void WriteCookie(string strName, string key, string strValue, string doMain)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName] ??
                                new HttpCookie(strName) { Domain = doMain };
            cookie[key] = HttpUtility.UrlEncode(strValue);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="key"></param>
        /// <param name="strValue">值</param>
        /// <param name="doMain">域</param>
        /// <param name="expires">过期时间(单位:分钟)</param>
        public static void WriteCookie(string strName, string key, string strValue, string doMain, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName] ??
                                new HttpCookie(strName) { Domain = doMain };
            cookie[key] = HttpUtility.UrlEncode(strValue);
            cookie.Expires = DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }


        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="expires">过期时间(分钟)</param>
        public static void WriteCookie(string strName, string strValue, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName] ?? new HttpCookie(strName);
            cookie.Value = HttpUtility.UrlEncode(strValue);
            cookie.Expires = DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);

        }

        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName)
        {
            if (HttpContext.Current.Request.Cookies[strName] != null)
            {
                var httpCookie = HttpContext.Current.Request.Cookies[strName];
                if (httpCookie != null)
                    return HttpUtility.UrlDecode(httpCookie.Value);
            }

            return "";
        }

        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="key"> </param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName, string key)
        {
            var cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie != null && (cookie[key] != null))
            {
                var httpCookie = HttpContext.Current.Request.Cookies[strName];
                if (httpCookie != null)
                    return HttpUtility.UrlDecode(httpCookie[key]);
            }

            return "";
        }

        /// <summary>
        /// 删除cookie
        /// </summary>
        /// <param name="strName"></param>
        public static void DelCookie(string strName)
        {

            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie != null)
            {
                cookie.Values.Clear();
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// 删除cookie
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="key"></param>
        public static void DelCookie(string strName, string key)
        {
            HttpCookie cookies = HttpContext.Current.Request.Cookies[strName];
            if (cookies != null)
            {
                cookies.Values.Remove(key);
                HttpContext.Current.Request.Cookies.Add(cookies);
            }
        }
        #endregion 读写Cookie


        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        /// <returns>客户端IP地址</returns>
        public static string GetClientIP()
        {
            string result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(result))
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }


            if (string.IsNullOrEmpty(result))
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }

            return result;
        }


        /// <summary>
        /// 提示错误信息！操作回滚！
        /// </summary>
        /// <param name="msg"></param>
        public static void ShowMsg(string msg)
        {
            HttpContext.Current.Response.Write("<script>alert(\"" + msg + "\");history.back();</script>");
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 提示成功信息！转向！
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="url"></param>
        public static void ShowMsg(string msg, string url)
        {
            HttpContext.Current.Response.Write("<script>alert(\"" + msg + "\");location.href='" + url + "';</script>");
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="scriptString"></param>
        /// <param name="endResponse"></param>
        public static void ExcuteScript(string scriptString, bool endResponse)
        {
            var strJavascript = new StringBuilder(200);
            strJavascript.Append("<script language=\"javascript\">\n");
            strJavascript.Append(scriptString + "\n");
            strJavascript.Append("</script>\n");
            HttpContext.Current.Response.Write(strJavascript);
            if (endResponse)
            {
                HttpContext.Current.Response.End();
            }
        }


        /// <summary>
        /// 获得指定Url参数的值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <returns>Url参数的值</returns>
        public static string GetQueryString(string strName)
        {
            return GetQueryString(strName, false);
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
        /// 获得指定Url参数的值
        /// </summary> 
        /// <param name="strName">Url参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>Url参数的值</returns>
        public static string GetQueryString(string strName, bool sqlSafeCheck)
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
        public static string GetFormString(string strName)
        {
            return GetFormString(strName, false);
        }

        /// <summary>
        /// 获得指定表单参数的值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>表单参数的值</returns>
        public static string GetFormString(string strName, bool sqlSafeCheck)
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
        /// 过滤HTML代码(常规替换方法)
        /// </summary>
        /// <param name="htmlString">Html代码</param>
        /// <returns>返回过滤过的Html代码</returns>
        public static string ClearHtml(string htmlString)
        {
            htmlString = Regex.Replace(htmlString, "<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, "<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, "-->", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, "<!--.*", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, "&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, "&(amp|#38);", "&", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, "&(lt|#60);", "<", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, "&(gt|#62);", ">", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, "&(nbsp|#160);", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, "&(iexcl|#161);", "\x00a1", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, "&(cent|#162);", "\x00a2", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, "&(pound|#163);", "\x00a3", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, "&(copy|#169);", "\x00a9", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            htmlString = htmlString.Replace("<", "");
            htmlString = htmlString.Replace(">", "");
            htmlString = htmlString.Replace("<.*?>", "");
            htmlString = htmlString.Replace(" ", "");
            htmlString = htmlString.Replace("'", "");
            htmlString = htmlString.Replace("&nbsp;", "");
            htmlString = Regex.Replace(htmlString, @"([\r\n])[\s]+", "<br />", RegexOptions.IgnoreCase);
            htmlString = HttpContext.Current.Server.HtmlEncode(htmlString).Trim();
            return htmlString;
        }

        /**/
        ///   <summary>
        ///   去除HTML标记
        ///   </summary>
        ///   <param   name="NoHTML">包括HTML的源码   </param>
        ///   <returns>已经去除后的文字</returns>
        public static string NoHTML(string Htmlstring)
        {
            //删除脚本
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "",
              RegexOptions.IgnoreCase);
            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "",
              RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "",
              RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"",
              RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&",
              RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<",
              RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">",
              RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ",
              RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1",
              RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2",
              RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3",
              RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9",
              RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "",
              RegexOptions.IgnoreCase);

            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();

            return Htmlstring;
        }
    }
}
