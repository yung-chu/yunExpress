using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using LMS.Client.B2CTracking.Controller;
using LMS.Client.B2CTracking.Model;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.B2CTracking
{
    public class B2CTrackingJob
    {
        private static readonly int _maxEventCode = Int32.Parse(ConfigurationManager.AppSettings["MaxEventCode"]);

        private static readonly string _DDPRegisterShippingMethodCode =
            ConfigurationManager.AppSettings["DDPRegisterShippingMethodCode"];

        private static readonly string _DDPShippingMethodCode =
            ConfigurationManager.AppSettings["DDPShippingMethodCode"];

        private static readonly int _EUDDPMaxEventCode =
            Int32.Parse(ConfigurationManager.AppSettings["EUDDPMaxEventCode"]); 

        public static void Run()
        {
            Log.Info("开始抓取B2C跟踪信息");
            TrackController.GetTrackInfo();
            Log.Info("完成抓取B2C跟踪信息");
            Log.Info("开始生成内部跟踪信息");
            var shippingMethodIds = TotalPackageTraceInfoController.GetShippingMethodsByIds();
            var dictionary = TotalPackageTraceInfoController.GetShippingMethodConfig(shippingMethodIds);
            CreateInternalTrackInfo(shippingMethodIds, dictionary);
            Log.Info("完成生成内部跟踪信息");
            TotalPackageTraceInfoController.OutInsertB2CForeCastLogs();
            CreateCustomTrackInfo(shippingMethodIds, dictionary);
            TrackController.TrackInfoInsertLms();
        }
        public static void CreateInternalTrackInfo(List<ShippingMethodModel> shippingMethodIds, Dictionary<int, List<ShippingMethodConfig>> dictionary)
        {
            try
            {
                Log.Info("总包号跟踪记录信息Job 任务开始运行");
                var list = TotalPackageTraceInfoController.GetTotalPackageTraceInfo();
                if (list != null && list.Count > 0)
                {
                    
                    int rshippingMethodId =
                        shippingMethodIds.SingleOrDefault(p => p.Code == _DDPRegisterShippingMethodCode)
                                         .ShippingMethodId;
                    int shippingMethodId = shippingMethodIds.SingleOrDefault(p => p.Code == _DDPShippingMethodCode)
                                         .ShippingMethodId;
                    foreach (var model in list)
                    {
                        if (model.TraceEventCode <= _maxEventCode)
                        {
                            var xml = new StringBuilder();
                            dictionary[model.TraceEventCode].ForEach(p => xml.Append(
                                "<EventContent ShippingMentodId=\"{0}\"  CountryCode=\"{1}\" OffsetHours=\"{3}\">{2}</EventContent>"
                                    .FormatWith(p.ShippingMethodId, p.CountryCode, p.EventContent, p.OffsetHours)));
                             if (TotalPackageTraceInfoController.InsertInTrackingLogInfo(model, xml.ToString()))
                             {
                                 if (model.TraceEventCode == _maxEventCode)
                                 {
                                     TotalPackageTraceInfoController.InsertB2CForeCastLogs(model.TotalPackageNumber,
                                                                                           rshippingMethodId,
                                                                                           shippingMethodId);
                                 }
                             }
                        }
                    }
                }
                Log.Info("总包号跟踪记录信息Job 任务运行结束");
            }
            catch (Exception ee)
            {
                Log.Exception(ee);
            }
        }
        /// <summary>
        /// 创建欧洲专线平邮事件码大于10的自定义跟踪信息
        /// </summary>
        public static void CreateCustomTrackInfo(List<ShippingMethodModel> shippingMethodIds, Dictionary<int, List<ShippingMethodConfig>> dictionary)
        {
            try
            {
                Log.Info("开始插入欧洲专线平邮事件码大于10的自定义跟踪信息");
                var list = TotalPackageTraceInfoController.GetB2CForeCastLogs();
                if (list.Any())
                {
                    int shippingMethodId = shippingMethodIds.SingleOrDefault(p => p.Code == _DDPShippingMethodCode)
                                         .ShippingMethodId;
                    var config =
                        dictionary[_EUDDPMaxEventCode].Where(p => p.ShippingMethodId == shippingMethodId).ToList();
                    var addconfig = dictionary[_EUDDPMaxEventCode + 1].Where(p => p.ShippingMethodId == shippingMethodId).ToList();
                    var result = TrackController.GetB2CForeCastLogsSchema();
                    var i = 1;
                    foreach (var model in list)
                    {
                        DataRow row = result.NewRow();
                        row["WayBillNumber"] = model.WayBillNumber;
                        row["EventCode"] = _EUDDPMaxEventCode;
                        row["IsOkJob"] = 0;
                        row["CreatedOn"] = DateTime.Now;
                        row["LastUpdatedOn"] = DateTime.Now;
                        var c = config.SingleOrDefault(p => p.CountryCode.ToUpper() == model.EventContent.ToUpper()) ??
                                config.Single(p => p.CountryCode == "Other");
                        row["EventDate"] =
                            model.EventDate.AddHours(c.AddHours)
                                 .AddMilliseconds(new Random(i).Next(-2*60*60*1000, 2*60*60*1000));
                        row["EventContent"] = c.EventContent;
                        row["EventLocation"] = c.CountryCode == "Other" ? model.EventLocation : c.Address;
                        result.Rows.Add(row);
                        i++;
                        DataRow addrow = result.NewRow();
                        addrow["WayBillNumber"] = model.WayBillNumber;
                        addrow["EventCode"] = _EUDDPMaxEventCode+1;
                        addrow["IsOkJob"] = 0;
                        addrow["CreatedOn"] = DateTime.Now;
                        addrow["LastUpdatedOn"] = DateTime.Now;
                        var c1 = addconfig.SingleOrDefault(p => p.CountryCode.ToUpper() == model.EventContent.ToUpper()) ??
                                addconfig.Single(p => p.CountryCode == "Other");
                        addrow["EventDate"] = model.EventDate.AddHours(c.AddHours+c1.AddHours)
                                 .AddMilliseconds(new Random(i).Next(-2 * 60 * 60 * 1000, 2 * 60 * 60 * 1000));
                        addrow["EventContent"] = c1.EventContent;
                        addrow["EventLocation"] = c1.CountryCode == "Other" ? model.EventLocation : c1.Address;
                        result.Rows.Add(addrow);
                        i++;
                    }
                    TrackController.BulkToDb(result, "B2CForeCastLogs");
                    TotalPackageTraceInfoController.UpdateCustomeIsOkJob();
                    Log.Info("完成插入欧洲专线平邮事件码大于10的自定义跟踪信息");
                }
            }
            catch (Exception ee)
            {
                Log.Exception(ee);
            }
        }
    }
}
