using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Client.B2CTracking.Controller;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.B2CTracking
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Log.Info("开始运行B2CTrackingJob");
                var waybillnumber = System.Configuration.ConfigurationManager.AppSettings["TestWayBillNumber"];
                if (!string.IsNullOrWhiteSpace(waybillnumber))
                {
                    TrackController.GetTrackInfo(waybillnumber);
                }
                else
                {
                    B2CTrackingJob.Run();
                }
                
                Log.Info("完成运行B2CTrackingJob");
                
            }
            catch (Exception ee)
            {
                Log.Exception(ee);
            }
        }
    }
}
