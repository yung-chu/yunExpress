using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using LMS.Client.TrackingTotalPackage.Controller;
using LMS.Client.TrackingTotalPackage.Model;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.TrackingTotalPackage
{
    class Program
    {
        private static readonly int _maxEventCode = Int32.Parse(ConfigurationManager.AppSettings["MaxEventCode"].ToString());

        static void Main(string[] args)
        {
            try
            {
                Log.Info("总包号跟踪记录信息Job 任务开始运行");
                var list = TotalPackageTraceInfoController.GetTotalPackageTraceInfo();
                if (list != null && list.Count > 0)
                {
                    var dictionary = TotalPackageTraceInfoController.GetShippingMethodConfig();
                    
                    foreach (var model in list)
                    {
                        var xml = new StringBuilder();
                        dictionary[model.TraceEventCode].ForEach(p =>
                            {
                                if (model.TraceEventCode > _maxEventCode)
                                {
                                    xml.Append(
                                        "<EventContent Address=\"{0}\" ShippingMentodId=\"{1}\"  CountryCode=\"{2}\" OffsetHours=\"{4}\">{3}</EventContent>"
                                            .FormatWith(p.Address, p.ShippingMethodId, p.CountryCode, p.EventContent,p.OffsetHours));
                                }
                                else
                                {
                                    xml.Append(
                                        "<EventContent ShippingMentodId=\"{0}\"  CountryCode=\"{1}\" OffsetHours=\"{3}\">{2}</EventContent>"
                                            .FormatWith(p.ShippingMethodId, p.CountryCode, p.EventContent,p.OffsetHours));
                                }
                            });
                        if (TotalPackageTraceInfoController.InsertInTrackingLogInfo(model, xml.ToString()))
                        {
                            if (model.TraceEventCode >= _maxEventCode&&model.TraceEventCode<dictionary.Keys.Max())
                            {
                                TotalPackageTraceInfoController.InTotalPackageTraceInfo(new TotalPackageTraceInfoModel()
                                    {
                                        CreatedBy = model.CreatedBy,
                                        TotalPackageNumber = model.TotalPackageNumber,
                                        TraceEventCode = model.TraceEventCode+1,
                                        TraceEventTime =(model.TraceEventCode==8?model.TraceEventTime.AddHours(48)
                                                 .AddMilliseconds(new Random().Next(-2*60*60*1000, 2*60*60*1000)):
                                            model.TraceEventTime.AddHours(24)
                                                 .AddMilliseconds(new Random().Next(-2*60*60*1000, 2*60*60*1000))),
                                        TraceEventAddress = ""
                                    });
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
    }
}
