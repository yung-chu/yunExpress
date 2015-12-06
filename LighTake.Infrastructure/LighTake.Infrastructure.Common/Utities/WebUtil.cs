using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace LighTake.Infrastructure.Common
{
    public class WebUtil
    {
        const string sUserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        const string sContentType = "application/x-www-form-urlencoded";
        const string sRequestEncoding = "utf-8";
        const string sResponseEncoding = "utf-8";

        /// <summary>
        /// Post data到url
        /// </summary>
        /// <param name="data">要post的数据</param>
        /// <param name="url">目标url</param>
        /// <returns>服务器响应</returns>
        public static string PostDataToUrl(string data, string url)
        {
            Encoding encoding = Encoding.GetEncoding(sRequestEncoding);
            byte[] bytesToPost = encoding.GetBytes(data);
            return PostDataToUrl(bytesToPost, url);
        }

        /// <summary>
        /// Post data到url
        /// </summary>
        /// <param name="data">要post的数据</param>
        /// <param name="url">目标url</param>
        /// <returns>服务器响应</returns>
        public static string PostDataToUrl(byte[] data, string url)
        {
            #region 创建httpWebRequest对象
            WebRequest webRequest = WebRequest.Create(url);
            HttpWebRequest httpRequest = webRequest as HttpWebRequest;
            if (httpRequest == null)
            {
                throw new ApplicationException(
                    string.Format("Invalid url string: {0}", url)
                    );
            }
            #endregion
            #region 填充httpWebRequest的基本信息
            httpRequest.UserAgent = sUserAgent;
            httpRequest.ContentType = sContentType;
            httpRequest.Method = "POST";
            #endregion
            #region 填充要post的内容
            httpRequest.ContentLength = data.Length;
            Stream requestStream = httpRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            #endregion
            #region 发送post请求到服务器并读取服务器返回信息
            Stream responseStream;
            try
            {
                responseStream = httpRequest.GetResponse().GetResponseStream();
            }
            catch (Exception e)
            {
                // log error WinForm调试方式
                //Console.WriteLine(
                //    string.Format("POST操作发生异常：{0}", e.Message)
                //    );
                throw e;
            }
            #endregion
            #region 读取服务器返回信息
            string stringResponse = string.Empty;
            using (StreamReader responseReader =
                new StreamReader(responseStream, Encoding.GetEncoding(sResponseEncoding)))
            {
                stringResponse = responseReader.ReadToEnd();
            }
            responseStream.Close();
            #endregion
            return stringResponse;
        }

        /// <summary>
        /// 将字符编码为Base64
        /// </summary>
        /// <param name="encodeType">编码方式</param>
        /// <param name="input">明文字符</param>
        /// <returns>字符串</returns>
        public static string EncodeBase64(string encodeType, string input)
        {
            string result = string.Empty;
            byte[] bytes = Encoding.GetEncoding(encodeType).GetBytes(input);
            try
            {
                result = Convert.ToBase64String(bytes);
            }
            catch
            {
                result = input;
            }
            return result;
        }

        /// <summary>
        /// 将字符编码为Base64
        /// </summary>
        /// <param name="encodeType">编码方式</param>
        /// <param name="input">明文字符</param>
        /// <returns>字符串</returns>
        public static string DecodeBase64(string encodeType, string input)
        {
            string decode = string.Empty;
            byte[] bytes = Convert.FromBase64String(input);
            try
            {
                decode = Encoding.GetEncoding(encodeType).GetString(bytes);
            }
            catch
            {
                decode = input;
            }
            return decode;
        }
    }
}
