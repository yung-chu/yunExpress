using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace Wuyi.Common
{
    public class HttpRequestUnit
    {
        static HttpRequestUnit()
        {
            ServicePointManager.DefaultConnectionLimit = 500;
        }

        /// <summary>
        /// 默认UserAgent
        /// </summary>
        private string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/6.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E; InfoPath.2)";

        /// <summary>
        /// Cookie集合
        /// </summary>
        private CookieContainer Cookies = new CookieContainer();

        /// <summary>
        /// 最后一次返回请求的Url
        /// </summary>
        private String _retUrl = "";

        /// <summary>
        /// 返回
        /// </summary>
        public string RetUrl
        {
            get { return _retUrl; }
        }

        private Encoding responseEncoding;

        /// <summary>
        /// 最后响应的编码
        /// </summary>
        public Encoding ResponseEncoding
        {
            get { return responseEncoding; }
        }

        /// <summary>
        /// 通过GET方式请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetHttpResponse(string url)
        {
            return GetHttpResponse(url, null, null, null, Encoding.UTF8, ref Cookies, null, out _retUrl);
        }

        /// <summary>
        /// 通过GET方式请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="proxy">代理</param>
        /// <returns></returns>
        public string GetHttpResponse(string url, WebProxy proxy)
        {
            return GetHttpResponse(url, null, null, null, Encoding.UTF8, ref Cookies, proxy, out _retUrl);
        }

        /// <summary>
        /// 通过GET方式请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string GetHttpResponse(string url,Encoding encoding)
        {
            return GetHttpResponse(url, null, null, null, encoding, ref Cookies, null, out _retUrl);
        }

        /// <summary>
        /// 通过GET方式请求
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="timeout">超时时间</param>
        /// <param name="userAgent">浏览器标志</param>
        /// <param name="referer">引用页</param>
        /// <param name="requestEncoding">编码</param>
        /// <param name="cookies">CookieContainer</param>
        /// <param name="proxy">代理</param>
        /// <param name="returl">返回请求的Url</param>
        /// <returns></returns>
        public string GetHttpResponse(string url, int? timeout, string userAgent, string referer, Encoding requestEncoding, ref CookieContainer cookies, WebProxy proxy, out string returl)
        {
            returl = "";

            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentNullException("url");
                }

                if (requestEncoding == null)
                {
                    throw new ArgumentNullException("requestEncoding");
                }

                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
                
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

                request.Method = "GET";
                request.UserAgent = DefaultUserAgent;

                if (proxy != null)
                    request.Proxy = proxy;

                if (!string.IsNullOrEmpty(userAgent))
                {
                    request.UserAgent = userAgent;
                }

                if (timeout.HasValue)
                {
                    request.Timeout = timeout.Value;
                }

                if (cookies != null)
                {
                    request.CookieContainer = cookies;
                }

                if (referer != null)
                {
                    request.Referer = referer;
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();

                if (httpWebResponse.CharacterSet != null)
                {
                    requestEncoding = Encoding.GetEncoding(httpWebResponse.CharacterSet);
                }

                this.responseEncoding = requestEncoding;

                string html = "";

                using (Stream responseStream = httpWebResponse.GetResponseStream())
                {
                    using (StreamReader streamReader = new StreamReader(responseStream, requestEncoding))
                    {
                        html = streamReader.ReadToEnd();

                        foreach (Cookie cookie in httpWebResponse.Cookies)
                        {
                            cookies.Add(cookie);
                        }

                        returl = httpWebResponse.ResponseUri.ToString();
                    }
                }

                request.Abort();

                httpWebResponse.Close();

                return html;
            }
            catch (Exception er)
            {
                return er.Message;
            }
        }

        /// <summary>
        /// 通过Post方式请求
        /// </summary>
        /// <param name="url">要请求的地址</param>
        /// <param name="PostData">参数</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string PostHttpResponse(string url, string PostData, Encoding encoding)
        {
            return PostHttpResponse(url, PostData, null, null, null, encoding, ref Cookies, null, out _retUrl);
        }

        /// <summary>
        /// 通过Post方式请求
        /// </summary>
        /// <param name="url">要请求的地址</param>
        /// <param name="PostData">参数</param>
        /// <returns></returns>
        public string PostHttpResponse(string url, string PostData)
        {
            return PostHttpResponse(url, PostData, null, null, null, Encoding.UTF8, ref Cookies, null, out _retUrl);
        }

        /// <summary>
        /// 通过Post方式请求
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="PostData">参数</param>
        /// <param name="timeout">超时时间</param>
        /// <param name="userAgent"></param>
        /// <param name="referer">引用页</param>
        /// <param name="requestEncoding">编码</param>
        /// <param name="cookies">CookieContainer</param>
        /// <param name="proxy">代理</param>
        /// <param name="returl">返回请求的Url</param>
        /// <returns></returns>
        public string PostHttpResponse(string url, string PostData, int? timeout, string userAgent, string referer, Encoding requestEncoding, ref CookieContainer cookies, WebProxy proxy, out string returl)
        {
            returl = "";

            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentNullException("url");
                }

                if (requestEncoding == null)
                {
                    throw new ArgumentNullException("requestEncoding");
                }

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                
                if (proxy != null)
                    request.Proxy = proxy;

                if (!string.IsNullOrEmpty(userAgent))
                {
                    request.UserAgent = userAgent;
                }

                else
                {
                    request.UserAgent = DefaultUserAgent;
                }

                if (timeout.HasValue)
                {
                    request.Timeout = timeout.Value;
                }

                if (cookies != null)
                {
                    request.CookieContainer = cookies;
                }

                if (referer != null)
                {
                    request.Referer = referer;
                }

                //如果需要POST数据
                if (PostData != null)
                {
                    byte[] data = requestEncoding.GetBytes(PostData);
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
                HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();

                if (httpWebResponse.CharacterSet != null)
                {
                    requestEncoding = Encoding.GetEncoding(httpWebResponse.CharacterSet);
                }

                this.responseEncoding = requestEncoding;

                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, requestEncoding);
                string html = streamReader.ReadToEnd();

                foreach (Cookie cookie in httpWebResponse.Cookies)
                {
                    cookies.Add(cookie);
                }
                
                returl = httpWebResponse.ResponseUri.ToString();

                streamReader.Close();
                responseStream.Close();
                request.Abort();
                httpWebResponse.Close();

                return html;
            }
            catch (Exception er)
            {
                return er.Message;
            }
        }

        /// <summary>
        /// 检测证书
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受
        }

        public static string ToUrlEncode(string strCode)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(strCode);
            System.Text.RegularExpressions.Regex regKey = new System.Text.RegularExpressions.Regex("^[A-Za-z0-9]+$");
            for (int i = 0; i < byStr.Length; i++)
            {
                string strBy = Convert.ToChar(byStr[i]).ToString();
                if (regKey.IsMatch(strBy))
                {
                    //是字母或者数字则不进行转换
                    sb.Append(strBy);
                }
                else
                {
                    sb.Append(@"%" + Convert.ToString(byStr[i], 16).ToUpper());
                }
            }
            return (sb.ToString());
        }

        public static string ToUrlEncodeChinese(string strCode)
        {
            StringBuilder sb = new StringBuilder();

            Regex regKey = new Regex(@"[\u4e00-\u9fbb]");

            foreach (var s in strCode)
            {
                if (!regKey.IsMatch(s.ToString()))
                {
                    //是字母或者数字则不进行转换
                    sb.Append(s);
                }
                else
                {
                    sb.Append(HttpUtility.UrlEncode(s.ToString()));
                }
            }
            return (sb.ToString());
        }

        /// <summary>
        /// 从HTML文档中获取表达
        /// </summary>
        /// <param name="html"></param>
        /// <param name="name">表达名称</param>
        /// <returns>Value</returns>
        public string GetInputValue(string html,string name)
        {
            try
            {
                string pattern = string.Format("<input[^>]*name=\"{0}\"[^>]*value=\"([^\"]+)\"[^>]+>", name);

                string value = Regex.Match(html, pattern).Groups[1].ToString();

                return value;
            }
            catch
            {
            }

            return null;
        }
    }
}
