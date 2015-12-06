using System;
using LMS.Client.UpdateOutStoragePrice.Controller;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.UpdateOutStoragePrice
{
    class UpdateOutStoragePriceJob
    {
        static void Main(string[] args)
        {
            try
            {
                Log.Info("UpdateOutStoragePriceJob 任务开始运行");
                UpdateOutStoragePrice.UpdatePrice();
                Log.Info("UpdateOutStoragePriceJob 任务运行结束");
            }
            catch (Exception ee)
            {
                Log.Exception(ee);
            }
        }
    }
}
