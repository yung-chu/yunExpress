using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.B2CSubmit
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Log.Info("B2CSubmitJob 任务开始运行");
                B2CSubmitJob.Run();
                Log.Info("B2CSubmitJob 任务运行结束");
            }
            catch (Exception ee)
            {
                Log.Exception(ee);
            }
        }
    }
}
