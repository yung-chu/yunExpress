using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using LMS.Client.B2CTracking.Model;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.B2CTracking.Controller
{
    public class B2CController
    {
        /// <summary>
        /// 请求B2C跟踪API的地址
        /// </summary>
        private static readonly string B2CTrackUrl = ConfigurationManager.AppSettings["B2CTrackURL"];
        /// <summary>
        /// 用户ID
        /// </summary>
        private static readonly string Userid = ConfigurationManager.AppSettings["Userid"];
        /// <summary>
        /// 密码
        /// </summary>
        private static readonly string Mkey = ConfigurationManager.AppSettings["Mkey"];

        /// <summary>
        /// 按单号查询的格式
        /// </summary>
        private const string Keycodeurl = "{0}?Userid={1}&Password={2}&Keycode={3}";

        /// <summary>
        /// 按时间查询的格式
        /// </summary>
        private const string Datetimeurl = "{0}?Userid={1}&Password={2}&DateTime={3}";

        /// <summary>
        /// 按时间查询
        /// </summary>
        /// <param name="datetime">UTC 格式是20150104120000
        /// DateTime (YYYYMMDDhhmmss)
        ///o YYYY year
        ///o MM month
        ///o DD day
        ///o HH hours (24 notation)
        ///o mm minutes
        ///o ss seconds
        ///</param>
        /// <returns></returns>
        public static Tracking GetTrackInfoByTime(long datetime)
        {
            if (datetime > 0)
            {
                string url = Datetimeurl.FormatWith(B2CTrackUrl, Userid, Mkey, datetime);
                Log.Info("开始请求Url:{0}".FormatWith(url));
                try
                {
                    var xdoc=new XmlDocument();
                    xdoc.Load(url);
                    Log.Info("Url:{0},请求结果：{1}".FormatWith(url,xdoc.OuterXml));
                    return SerializeUtil.DeserializeFromXml<Tracking>(xdoc.OuterXml);
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
                Log.Info("完成请求Url:{0}".FormatWith(url));
            }
            return null;
        }
        public static string GetTrackInfoXmlByTime(long datetime)
        {
            if (datetime > 0)
            {
                string url = Datetimeurl.FormatWith(B2CTrackUrl, Userid, Mkey, datetime);
                Log.Info("开始请求Url:{0}".FormatWith(url));
                try
                {
                    var xdoc = new XmlDocument();
                    xdoc.Load(url);
                    Log.Info("Url:{0},请求结果：{1}".FormatWith(url, xdoc.OuterXml));
                    var selectSingleNode = xdoc.SelectSingleNode("/tracking");
                    if (selectSingleNode != null)
                        return selectSingleNode.InnerXml;
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
                Log.Info("完成请求Url:{0}".FormatWith(url));
            }
            return string.Empty;
        }
        /// <summary>
        /// 按单号查询跟踪信息
        /// </summary>
        /// <param name="ordernumber"></param>
        /// <returns></returns>
        public static Tracking GetTrackInfoByNumber(string ordernumber)
        {
            if (!ordernumber.IsNullOrWhiteSpace())
            {
                string url = Keycodeurl.FormatWith(B2CTrackUrl, Userid, Mkey, ordernumber);
                Log.Info("开始请求Url:{0}".FormatWith(url));
                try
                {
                    var xdoc = new XmlDocument();
                    xdoc.Load(url);
                    Log.Info("Url:{0},请求结果：{1}".FormatWith(url, xdoc.OuterXml));
                    return SerializeUtil.DeserializeFromXml<Tracking>(xdoc.OuterXml);
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
                Log.Info("完成请求Url:{0}".FormatWith(url));
            }
            return null;
        }
        public static string GetTrackInfoXmlByNumber(string ordernumber)
        {
            if (!ordernumber.IsNullOrWhiteSpace())
            {
                string url = Keycodeurl.FormatWith(B2CTrackUrl, Userid, Mkey, ordernumber);
                Log.Info("开始请求Url:{0}".FormatWith(url));
                try
                {
                    var xdoc = new XmlDocument();
                    xdoc.Load(url);
                    Log.Info("Url:{0},请求结果：{1}".FormatWith(url, xdoc.OuterXml));
                    var selectSingleNode = xdoc.SelectSingleNode("/tracking");
                    if (selectSingleNode != null)
                        return selectSingleNode.InnerXml;
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
                Log.Info("完成请求Url:{0}".FormatWith(url));
            }
            return string.Empty;
        }
    }
}
