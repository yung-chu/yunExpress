using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.SubmitSF
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Log.Info("SubmitSFJob 任务开始运行");
                SubmitSfJob.SubmitSfOrder();
                SubmitSfJob.SubmitSebOrder();
                Log.Info("SubmitSFJob 任务运行结束");
            }
            catch (Exception ee)
            {
                Log.Exception(ee);
            }
        }
    }
}
