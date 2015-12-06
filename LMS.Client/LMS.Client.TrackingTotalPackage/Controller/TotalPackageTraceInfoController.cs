using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DbHelper;
using LMS.Client.TrackingTotalPackage.Model;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Http;
using LighTake.Infrastructure.Http.Infrastructure;

namespace LMS.Client.TrackingTotalPackage.Controller
{
    public class TotalPackageTraceInfoController
    {
        private static readonly string _lmsCon = System.Configuration.ConfigurationManager.AppSettings["LMSCon"].ToString();
        private static readonly string _EUDDPG = System.Configuration.ConfigurationManager.AppSettings["EUDDPG"].ToString();
        
        private static readonly string LISAPIPath = System.Configuration.ConfigurationManager.AppSettings["LIS_API_Path"].ToString();

        private static readonly string EndDate = System.Configuration.ConfigurationManager.AppSettings["EndDate"];
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
 AND EXISTS(SELECT 1 FROM  TotalPackageInfos ti WHERE ti.TotalPackageNumber=tp.TotalPackageNumber AND ti.CreatedOn<='"+EndDate+"')");
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
        public static bool InsertInTrackingLogInfo(TotalPackageTraceInfoModel model,string shippingMethodConfig)
        {
            var result = false;
            Log.Info("LMS开始插入总包号为:{0} 的内部信息".FormatWith(model.TotalPackageNumber));
            DbUtility dbUtility = new SqlDbUtility(_lmsCon);
            var obj= dbUtility.ExecuteScalar("exec P_TotalPackageNumberJob {0},{1}", model.ID, shippingMethodConfig);
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
        /// 当配置
        /// </summary>
        /// <param name="model"></param>
        public static void InTotalPackageTraceInfo(TotalPackageTraceInfoModel model)
        {
            Log.Info("LMS开始插入总包号为:{0} 的总包号跟踪信息,事件码:{1}".FormatWith(model.TotalPackageNumber,model.TraceEventCode));
            DbUtility dbUtility = new SqlDbUtility(_lmsCon);
            dbUtility.ExecuteNonQuery(@"INSERT INTO TotalPackageTraceInfos
           ([TotalPackageNumber],[TraceEventCode],[TraceEventTime],[TraceEventAddress]
           ,[CreatedBy],[LastUpdatedBy],[TraceEventContent])
            VALUES({0},{1},{2},{3},{4},{5},{6})", model.TotalPackageNumber, model.TraceEventCode, model.TraceEventTime,
                                      model.TraceEventAddress, model.CreatedBy, model.CreatedBy, "");
            Log.Info("LMS完成插入总包号为:{0} 的总包号跟踪信息,事件码:{1}".FormatWith(model.TotalPackageNumber, model.TraceEventCode));
            
        }
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, List<ShippingMethodConfig>>GetShippingMethodConfig()
        {
            var dictionary = new Dictionary<int, List<ShippingMethodConfig>>();
            var list = GetShippingMethodsByIds();
            if (list.Any())
            {
                DbUtility dbUtility = new SqlDbUtility(_lmsCon);
                var obj =
                    dbUtility.ExecuteScalar("select ConfigurationValue from SystemConfigurations where DictionaryID='" +
                                            _EUDDPG + "'");
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
                                                    OffsetHours = Int32.Parse(element.SingleOrDefault(e => e.Attribute("CountryCode").Value == c && e.Name == "EventAddress").Attribute("OffsetHours").Value)
                                                }));
                                        }
                                        var other = x.Elements().Where(e => e.Attributes().All(p => p.Name != "CountryCode") && e.Name != "EventCode");
                                        config.Add(new ShippingMethodConfig()
                                            {
                                                CountryCode = "Other",
                                                EventContent = other.SingleOrDefault(e => e.Name == "EventContent").Value,
                                                ShippingMethodId = shippingmentod.ShippingMethodId,
                                                Address = other.SingleOrDefault(e => e.Name == "EventAddress").Value,
                                                OffsetHours = Int32.Parse(other.SingleOrDefault(e => e.Name == "EventAddress").Attribute("OffsetHours").Value)
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
        
    }
}
