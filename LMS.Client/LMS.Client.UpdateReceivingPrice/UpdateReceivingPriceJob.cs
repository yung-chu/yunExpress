using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.UpdateReceivingPrice
{
    class UpdateReceivingPriceJob
    {
        static void Main(string[] args)
        {
            try
            {
                Log.Info("UpdateOutStoragePriceJob 任务开始运行");
                UpdateReceivingPrice.UpdatePrice();
                Log.Info("UpdateOutStoragePriceJob 任务运行结束");
            }
            catch (Exception ee)
            {
                Log.Exception(ee);
            }
        }
    }
}
