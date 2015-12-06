using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DbHelper;
using LMS.Client.B2CTracking.Model;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Http;
using LighTake.Infrastructure.Http.Infrastructure;

namespace LMS.Client.B2CTracking.Controller
{
    public class TotalPackageTraceInfoController
    {
        private static readonly string _lmsCon = ConfigurationManager.AppSettings["LMSCon"];
        private static readonly string _TrackCon = ConfigurationManager.AppSettings["TrackCon"];
        private static readonly string LISAPIPath = ConfigurationManager.AppSettings["LIS_API_Path"];
        private static readonly string _EUDD = ConfigurationManager.AppSettings["EUDD"];
        private static readonly string StartDate = ConfigurationManager.AppSettings["StartDate"];

        /// <summary>
        /// 获取需要运行增加跟踪信息的总单号
        /// </summary>
        /// <returns></returns>
        public static List<TotalPackageTraceInfoModel> GetTotalPackageTraceInfo()
        {
            DbUtility dbUtility = new SqlDbUtility(_lmsCon);
            DataTable dt =
                dbUtility.ExecuteData(
                    @"  select ID,TotalPackageNumber,TraceEventCode,TraceEventTime,TraceEventAddress,CreatedBy
 from TotalPackageTraceInfos tp where IsJob=0 and TraceEventTime<=GETDATE() 
 AND EXISTS(SELECT 1 FROM  TotalPackageInfos ti WHERE ti.TotalPackageNumber=tp.TotalPackageNumber AND ti.CreatedOn>'" + StartDate + "')");
            var list = new List<TotalPackageTraceInfoModel>();
            if (dt != null && dt.Rows.Count > 0)
            {
                Log.Info("LMS总包号编辑时间记录总数是" + dt.Rows.Count);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var v = new TotalPackageTraceInfoModel();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        switch (dt.Columns[j].ColumnName)
                        {
                            case "TotalPackageNumber":
                                v.TotalPackageNumber = dt.Rows[i][j].ToString();
                                break;
                            case "TraceEventCode":
                                v.TraceEventCode = Int32.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "TraceEventTime":
                                v.TraceEventTime = DateTime.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "TraceEventAddress":
                                v.TraceEventAddress = dt.Rows[i][j].ToString();
                                break;
                            case "CreatedBy":
                                v.CreatedBy = dt.Rows[i][j].ToString();
                                break;
                            case "ID":
                                v.ID = Int32.Parse(dt.Rows[i][j].ToString());
                                break;
                        }
                    }
                    list.Add(v);
                }
            }
            Log.Info("LMS总包号编辑时间记录获取完毕！");
            return list;
        }

        /// <summary>
        /// 插入内部跟踪信息
        /// </summary>
        /// <param name="model"></param>
        public static bool InsertInTrackingLogInfo(TotalPackageTraceInfoModel model, string shippingMethodConfig)
        {
            var result = false;
            Log.Info("LMS开始插入总包号为:{0} 的内部信息".FormatWith(model.TotalPackageNumber));
            DbUtility dbUtility = new SqlDbUtility(_lmsCon);
            var obj = dbUtility.ExecuteScalar("exec P_TotalPackageNumberJob {0},{1}", model.ID, shippingMethodConfig);
            if (obj != null && obj.ToString() == "1")
            {
                dbUtility.ExecuteNonQuery(
                    "UPDATE TotalPackageTraceInfos SET IsJob=1,LastUpdatedOn=GETDATE() WHERE ID={0}", model.ID);
                result = true;
            }
            Log.Info("LMS完成插入总包号为:{0} 的内部信息".FormatWith(model.TotalPackageNumber));

            return result;
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, List<ShippingMethodConfig>> GetShippingMethodConfig(List<ShippingMethodModel> list)
        {
            var dictionary = new Dictionary<int, List<ShippingMethodConfig>>();
            //var list = GetShippingMethodsByIds();
            if (list.Any())
            {
                DbUtility dbUtility = new SqlDbUtility(_lmsCon);
                var obj =
                    dbUtility.ExecuteScalar("select ConfigurationValue from SystemConfigurations where ConfigurationKey='" +
                                            _EUDD + "'");
                if (obj != null)
                {
                    TextReader tr = new StringReader(obj.ToString());
                    var xml = XDocument.Load(tr);
                    var text = from t in xml.Descendants("ShippingMethod")
                               select new
                               {
                                   ShippingMethodCode = t.Element("ShippingMethodCode").Value,
                                   Events = t.Elements("Event").ToList()
                               };
                    if (text.Any())
                    {
                        foreach (var s in text)
                        {
                            if (s.Events.Any())
                            {
                                var shippingmentod = list.SingleOrDefault(p => p.Code == s.ShippingMethodCode);
                                if (shippingmentod != null && shippingmentod.ShippingMethodId > 0)
                                {
                                    foreach (var x in s.Events)
                                    {
                                        int eventCode = Int32.Parse(x.Element("EventCode").Value);
                                        var config = new List<ShippingMethodConfig>();
                                        var element = x.Elements().Where(e => e.Attributes().Any(p => p.Name == "CountryCode"));
                                        if (element.Any())
                                        {
                                            var a = element.Select(e => e.Attribute("CountryCode").Value).Distinct();
                                            config.AddRange(a.Select(c => new ShippingMethodConfig()
                                            {
                                                CountryCode = c,
                                                EventContent = element.SingleOrDefault(e => e.Attribute("CountryCode").Value == c && e.Name == "EventContent").Value,
                                                ShippingMethodId = shippingmentod.ShippingMethodId,
                                                Address = element.SingleOrDefault(e => e.Attribute("CountryCode").Value == c && e.Name == "EventAddress").Value,
                                                OffsetHours = Int32.Parse(element.SingleOrDefault(e => e.Attribute("CountryCode").Value == c && e.Name == "EventAddress").Attribute("OffsetHours").Value),
                                                AddHours = Int32.Parse(element.SingleOrDefault(e => e.Attribute("CountryCode").Value == c && e.Name == "EventAddress").Attribute("AddHours").Value)
                                            }));
                                        }
                                        var other = x.Elements().Where(e => e.Attributes().All(p => p.Name != "CountryCode") && e.Name != "EventCode");
                                        config.Add(new ShippingMethodConfig()
                                        {
                                            CountryCode = "Other",
                                            EventContent = other.SingleOrDefault(e => e.Name == "EventContent").Value,
                                            ShippingMethodId = shippingmentod.ShippingMethodId,
                                            Address = other.SingleOrDefault(e => e.Name == "EventAddress").Value,
                                            OffsetHours = Int32.Parse(other.SingleOrDefault(e => e.Name == "EventAddress").Attribute("OffsetHours").Value),
                                            AddHours = Int32.Parse(other.SingleOrDefault(e => e.Name == "EventAddress").Attribute("AddHours")!=null?other.SingleOrDefault(e => e.Name == "EventAddress").Attribute("AddHours").Value:"0")
                                        });
                                        if (dictionary.ContainsKey(eventCode))
                                        {
                                            dictionary[eventCode].AddRange(config);
                                        }
                                        else
                                        {
                                            dictionary.Add(eventCode, config);
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }
            return dictionary;
        }

        /// <summary>
        /// 获取所有运输方式
        /// </summary>
        /// <returns></returns>
        public static List<ShippingMethodModel> GetShippingMethodsByIds()
        {
            var url = LISAPIPath + "API/LIS/GetShippingMethods";
            var nameValueCollection = new NameValueCollection
                {
                    {"venderCode", ""},
                    {"shippingMethodType", "0"}
                };

            url = url.AppendUrlParameters(nameValueCollection);
            try
            {
                var list = HttpHelper.DoRequest<List<ShippingMethodModel>>(url, EnumHttpMethod.GET);
                Log.Info(list.RawValue);
                return list.Value;
            }
            catch (Exception ex)
            {
                Log.Error("错误地址：" + url);
                Log.Exception(ex);
            }
            return null;
        }
        /// <summary>
        /// 插入B2C预报记录表
        /// </summary>
        /// <param name="totalPackageNumber">总包号</param>
        /// <param name="registeredShippingMethodId">欧洲专线挂号运输方式ID</param>
        /// <param name="mailShippingMethodId">欧洲专线平邮运输方式ID</param>
        public static void InsertB2CForeCastLogs(string totalPackageNumber, int registeredShippingMethodId,
                                                 int mailShippingMethodId)
        {
            Log.Info("LMS开始插入总包号为:{0} 的B2C预报记录".FormatWith(totalPackageNumber));
            DbUtility dbUtility = new SqlDbUtility(_lmsCon);
            dbUtility.ExecuteNonQuery("exec P_TotalPackageNumberCreatedLog {0},{1},{2}", totalPackageNumber,
                                      registeredShippingMethodId, mailShippingMethodId);
            Log.Info("LMS完成插入总包号为:{0} 的B2C预报记录".FormatWith(totalPackageNumber));
        }
        /// <summary>
        /// 外部跟踪信息插入B2C预报记录表
        /// </summary>
        public static bool OutInsertB2CForeCastLogs()
        {
            Log.Info("LMS开始把外部跟踪信息插入B2C预报记录");
            DbUtility dbUtility = new SqlDbUtility(_TrackCon);
            var obj= dbUtility.ExecuteScalar("exec P_InsertB2CForeCastLogs");
            Log.Info("LMS完成把外部跟踪信息插入B2C预报记录");
            return obj.ToString() == "1";
        }
        /// <summary>
        /// 获取欧洲专线平邮需要自定义跟踪信息的单号
        /// </summary>
        /// <returns></returns>
        public static List<B2CForeCastLogs> GetB2CForeCastLogs()
        {
            Log.Info("开始获取欧洲专线平邮需要自定义跟踪信息的单号");
            string sql = @"WITH kk AS
                        (
			                SELECT WayBillNumber,EventLocation,EventContent
			                FROM dbo.B2CForeCastLogs
			                WHERE IsOkJob=4
		                ), ll AS
		                (
			                SELECT b.WayBillNumber,b.EventDate
			                FROM dbo.B2CForeCastLogs b
			                WHERE  EXISTS(SELECT 1 FROM kk WHERE kk.WayBillNumber=b.WayBillNumber) AND EventCode=10
		                ),hh AS
                        (
			                SELECT WayBillNumber
			                FROM B2CForeCastLogs d
			                WHERE EXISTS(SELECT 1 FROM kk WHERE kk.WayBillNumber=d.WayBillNumber) AND EventCode=11
		                )
		                SELECT kk.WayBillNumber,kk.EventLocation,ll.EventDate,kk.EventContent
                        FROM kk LEFT JOIN ll ON ll.WayBillNumber = kk.WayBillNumber
		                WHERE ll.EventDate IS NOT NULL AND NOT EXISTS(SELECT 1 FROM hh WHERE hh.WayBillNumber=kk.WayBillNumber)";
            DbUtility dbUtility = new SqlDbUtility(_TrackCon);
            var list = new List<B2CForeCastLogs>();
            DataTable dt = dbUtility.ExecuteData(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                Log.Info("获取欧洲专线平邮需要自定义跟踪信息的单号记录总数是" + dt.Rows.Count);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var v = new B2CForeCastLogs();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        switch (dt.Columns[j].ColumnName)
                        {
                            case "WayBillNumber":
                                v.WayBillNumber = dt.Rows[i][j].ToString();
                                break;
                            case "EventLocation":
                                v.EventLocation = dt.Rows[i][j].ToString();
                                break;
                            case "EventDate":
                                v.EventDate = DateTime.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "EventContent":
                                v.EventContent = dt.Rows[i][j].ToString();
                                break;
                        }
                    }
                    list.Add(v);
                }
            }
            Log.Info("完成获取欧洲专线平邮需要自定义跟踪信息的单号");
            return list;
        }
        /// <summary>
        /// 更新欧洲专线平邮自定义跟踪信息单号的状态为完成
        /// </summary>
        public static void UpdateCustomeIsOkJob()
        {
            Log.Info("开始更新欧洲专线平邮自定义跟踪信息单号的状态为完成");
            string sql = @"WITH kk AS
                        (
			                SELECT WayBillNumber,IsOkJob,LastUpdatedOn
			                FROM dbo.B2CForeCastLogs
			                WHERE IsOkJob=4
		                ),hh AS
                        (
			                SELECT WayBillNumber
			                FROM B2CForeCastLogs d
			                WHERE EXISTS(SELECT 1 FROM kk WHERE kk.WayBillNumber=d.WayBillNumber) AND EventCode=11
		                )
		                UPDATE kk
		                SET IsOkJob=1,LastUpdatedOn=GETDATE()
                        FROM kk 
		                WHERE EXISTS(SELECT 1 FROM hh WHERE hh.WayBillNumber=kk.WayBillNumber)";
            DbUtility dbUtility = new SqlDbUtility(_TrackCon);
            dbUtility.ExecuteNonQuery(sql);
            Log.Info("完成更新欧洲专线平邮自定义跟踪信息单号的状态为完成");
        }

    }
}
