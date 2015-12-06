using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
//using System.Net.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Formatting;

namespace LMS.WinForm.Client.Common
{
    public enum EnumHttpMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }
    public enum EnumContentType
    {
        Json,
        Xml
    }

    public static class HttpContentTypes
    {
        public const string MultiPartFormData = "multipart/form-data";
        public const string TextPlain = "text/plain";
        public const string TextHtml = "text/html";
        public const string TextCsv = "text/csv";
        public const string ApplicationJson = "application/json";
        public const string ApplicationXml = "application/xml";
        public const string ApplicationXWwwFormUrlEncoded = "application/x-www-form-urlencoded";
        public const string ApplicationOctetStream = "application/octet-stream";
    }
    public class ResponseResult<T>
    {
        public ResponseResult()
        {
            Value = default(T);
        }

        public T Value { get; set; }

        public string RawValue { get; set; }
    }
    public class HttpHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">返回转换的类型</typeparam>
        /// <param name="strUri">地址</param>
        /// <param name="method">请求方式</param>
        /// <param name="contentType">请求接收类型</param>
        /// <param name="objToSend">发送的数据（只限于Post和Put）</param>
        /// <param name="tick">防止等幂提交（只限于Put和Delete）</param>
        /// <returns></returns>
        public static ResponseResult<T> DoRequest<T>(string strUri, EnumHttpMethod method,
                                     EnumContentType contentType = EnumContentType.Json,
                                     Object objToSend = null, long tick = 0)
        {
            string strToRequest = strUri;

            if (tick != 0 && (method == EnumHttpMethod.PUT || method == EnumHttpMethod.DELETE))
                strToRequest += "?UpdateTicks=" + tick.ToString(CultureInfo.InvariantCulture);//CultureInfo.InvariantCulture的作用是为了固定格式

            //表示一个HTTP请求消息。
            HttpRequestMessage requestMsg = new HttpRequestMessage();
            //设置HTTP接收请求的数据类型
            switch (contentType)
            {
                case EnumContentType.Json:
                    requestMsg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(HttpContentTypes.ApplicationJson));
                    break;
                case EnumContentType.Xml:
                    requestMsg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(HttpContentTypes.ApplicationXml));
                    break;
            }
            //请求数据的链接(Uri)
            requestMsg.RequestUri = new Uri(strToRequest);

            HttpContent content = null;

            if (objToSend != null)
                //StringContent():转换成基于字符串的HTTP内容。
                //JsonConvert.SerializeObject(objToSend):将实体对象序列化成JSON
                content = new StringContent(JsonConvert.SerializeObject(objToSend), Encoding.UTF8, "application/json");

            switch (method)
            {
                case EnumHttpMethod.POST:
                    requestMsg.Method = HttpMethod.Post;//Post用于新增
                    requestMsg.Content = content;
                    break;
                case EnumHttpMethod.PUT:
                    requestMsg.Method = HttpMethod.Put;//Put用于修改
                    requestMsg.Content = content;
                    break;
                case EnumHttpMethod.DELETE:
                    requestMsg.Method = HttpMethod.Delete;
                    break;
                default:
                    requestMsg.Method = HttpMethod.Get;//Get用于获取
                    break;
            }
            //client是HttpClient的对象：用于发送HTTP请求和接收HTTP响应
            HttpClient client = new HttpClient();
            //client.SendAsync:发送一个HTTP请求一个异步操作，并返回响应结果。 
            Task<HttpResponseMessage> rtnAll = client.SendAsync(requestMsg);

            #region 执行
            HttpResponseMessage resultMessage = null;
            Task<T> rtnFinal;

            try
            {
                //这一步是关键提交请求的过程,rtnAll.Result这里其实是一个多线程的处理，是.Net Framework 4.0的新特性之一
                resultMessage = rtnAll.Result;
            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.InnerExceptions)
                {
                    if (ex is HttpRequestException)
                    {
                        throw new Exception("发送网络请求失败", ex);
                    }
                }
            }
            finally
            {
                client.Dispose();
            }

            if (!resultMessage.IsSuccessStatusCode)//判断响应是否成功？
            {
                resultMessage.EnsureSuccessStatusCode();
            }

            try
            {
                switch (contentType)
                {
                    case EnumContentType.Xml:
                        rtnFinal = resultMessage.Content.ReadAsAsync<T>(new MediaTypeFormatter[] { new XmlMediaTypeFormatter() });
                        break;
                    default:
                        rtnFinal = resultMessage.Content.ReadAsAsync<T>(new MediaTypeFormatter[] { new JsonMediaTypeFormatter() });
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("服务器返回与请求不匹配", ex);
            }

            #endregion

            return new ResponseResult<T>()
            {
                Value = rtnFinal.Result,
                RawValue = resultMessage.Content.ReadAsStringAsync().Result
            };
        }

        /// <summary>
        /// 不需要返回值
        /// </summary>
        /// <param name="strUri">地址</param>
        /// <param name="method">请求方式</param>
        /// <param name="objToSend">发送的数据（只限于Post和Put）</param>
        /// <param name="tick">防止等幂提交（只限于Put和Delete）</param>
        /// <returns></returns>
        public static void DoRequest(string strUri, EnumHttpMethod method,
                                      Object objToSend = null, long tick = 0)
        {
            string strToRequest = strUri;

            if (tick != 0 && (method == EnumHttpMethod.PUT || method == EnumHttpMethod.DELETE))
                strToRequest += "?UpdateTicks=" + tick.ToString(CultureInfo.InvariantCulture);//CultureInfo.InvariantCulture的作用是为了固定格式

            //表示一个HTTP请求消息。
            HttpRequestMessage requestMsg = new HttpRequestMessage();
            //设置HTTP接收请求的数据类型
            requestMsg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(HttpContentTypes.ApplicationJson));
            //请求数据的链接(Uri)
            requestMsg.RequestUri = new Uri(strToRequest);

            HttpContent content = null;
            if (objToSend != null)
                //StringContent():转换成基于字符串的HTTP内容。
                //JsonConvert.SerializeObject(objToSend):将实体对象序列化成JSON
                content = new StringContent(JsonConvert.SerializeObject(objToSend), Encoding.UTF8, "application/json");

            switch (method)
            {
                case EnumHttpMethod.POST:
                    requestMsg.Method = HttpMethod.Post;//Post用于新增
                    requestMsg.Content = content;
                    break;
                case EnumHttpMethod.PUT:
                    requestMsg.Method = HttpMethod.Put;//Put用于修改
                    requestMsg.Content = content;
                    break;
                case EnumHttpMethod.DELETE:
                    requestMsg.Method = HttpMethod.Delete;
                    break;
            }
            //client是HttpClient的对象：用于发送HTTP请求和接收HTTP响应
            HttpClient client = new HttpClient();
            //client.SendAsync:发送一个HTTP请求一个异步操作，并返回响应结果。 
            Task<HttpResponseMessage> rtnAll = client.SendAsync(requestMsg);

            #region 执行
            HttpResponseMessage resultMessage = null;

            try
            {
                //这一步是关键提交请求的过程,rtnAll.Result这里其实是一个多线程的处理，是.Net Framework 4.0的新特性之一
                resultMessage = rtnAll.Result;
            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.InnerExceptions)
                {
                    if (ex is HttpRequestException)
                    {
                        throw new Exception("发送网络请求失败", ex);
                    }
                }
            }
            finally
            {
                client.Dispose();
            }
            if (!resultMessage.IsSuccessStatusCode)//判断响应是否成功？
            {
                resultMessage.EnsureSuccessStatusCode();
            }

            #endregion
        }
    }
}
