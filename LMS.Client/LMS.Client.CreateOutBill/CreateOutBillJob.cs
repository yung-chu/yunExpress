using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.CreateOutBill
{
    class CreateOutBillJob
    {
        static void Main(string[] args)
        {
            try
            {
                Log.Info("CreateOutBillJob 任务开始运行");
                CreateOutBill.CreateBill();
                Log.Info("CreateOutBillJob 任务运行结束");
            }
            catch (Exception ee)
            {
                Log.Exception(ee);
            }
        }
    }
}
