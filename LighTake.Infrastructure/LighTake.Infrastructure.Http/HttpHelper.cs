using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Globalization;
using System.Xml.Serialization;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Http.Infrastructure;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http.Formatting;

namespace LighTake.Infrastructure.Http
{
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
            {
                //StringContent():转换成基于字符串的HTTP内容。
                //JsonConvert.SerializeObject(objToSend):将实体对象序列化成JSON
                switch (contentType)
                {
                    case EnumContentType.Json:
                        content = new StringContent(JsonConvert.SerializeObject(objToSend), Encoding.UTF8, "application/json");
                        break;
                    case EnumContentType.Xml:
                        content = new StringContent(SerializeUtil.SerializeToXml(objToSend), Encoding.UTF8, "application/xml");
                        break;
                    case EnumContentType.String:
                        content = new StringContent(objToSend.ToString());
                        break;
                }
            }
           

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
        public static ResponseResult<T> SendRequest<T>(string strUri, EnumHttpMethod method,
                                     EnumContentType contentType = EnumContentType.Json,
                                     Object objToSend = null, XmlSerializerNamespaces ns=null, long tick = 0)
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
            {
                //StringContent():转换成基于字符串的HTTP内容。
                //JsonConvert.SerializeObject(objToSend):将实体对象序列化成JSON
                switch (contentType)
                {
                    case EnumContentType.Json:
                        content = new StringContent(JsonConvert.SerializeObject(objToSend), Encoding.UTF8, "application/json");
                        break;
                    case EnumContentType.Xml:
                        if (null==ns)
                        {
                        content = new StringContent(SerializeUtil.SerializeToXml(objToSend), Encoding.UTF8, "application/xml");
                        }
                        else
                        {
                            content = new StringContent(SerializeUtil.SerializeToXml(objToSend,ns), Encoding.UTF8, "application/xml");
                        }
                        break;
                }
            }


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
            Task<HttpResponseMessage> rtnAll = client.SendAsync(requestMsg,HttpCompletionOption.ResponseContentRead);

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
                        rtnFinal =
                                resultMessage.Content.ReadAsAsync<T>(new MediaTypeFormatter[] { new XmlMediaTypeFormatter() });
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
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">要序列化的对象</param>
        /// <param name="strUri">请求的URI</param>
        /// <param name="ns">xml命名空间   var ns = new XmlSerializerNamespaces();ns.Add("req", "http://www.lightake.com");</param>
        /// <returns></returns>
        public static string PostSendRequest<T>(T t, string strUri, XmlSerializerNamespaces ns)
        {

            WebRequest request = WebRequest.Create(strUri);
            request.Method = "POST";
            request.ContentType = "text/xml";
            var stream = request.GetRequestStream();
            StreamWriter writer = new StreamWriter(stream);
            var soapWriter = new XmlSerializer(typeof(T));
            soapWriter.Serialize(writer, t, ns);
            writer.Close();
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();
            return responseFromServer;
        }


        public static ResponseResult<T> DoRequestBasic<T>(string strUri, EnumHttpMethod method, string username, string password,
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
            {
                //StringContent():转换成基于字符串的HTTP内容。
                //JsonConvert.SerializeObject(objToSend):将实体对象序列化成JSON
                switch (contentType)
                {
                    case EnumContentType.Json:
                        content = new StringContent(JsonConvert.SerializeObject(objToSend), Encoding.UTF8, "application/json");
                        break;
                    case EnumContentType.Xml:
                        content = new StringContent(SerializeUtil.SerializeToXml(objToSend), Encoding.UTF8, "application/xml");
                        break;
                }
            }


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
            //增加Basic验证
            
            requestMsg.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}&{1}", username, password)))); 

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

    }
}
