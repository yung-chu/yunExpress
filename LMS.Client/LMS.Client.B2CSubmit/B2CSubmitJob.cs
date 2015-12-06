using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Client.B2CSubmit.Controller;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.B2CSubmit
{
    public class B2CSubmitJob
    {
        public static void Run()
        {
            var list = WayBillController.GetSubmitWayBillNumber();
            if (list.Any())
            {
                var preAlertBatchNo = WayBillController.GetSequenceNumber();//生成批次号
                foreach (var wayBillNumber in list)
                {
                    var model = WayBillController.GetWayBillInfoModel(wayBillNumber);
                    if (model != null && !model.WayBillNumber.IsNullOrWhiteSpace())
                    {
                        model.PreAlertBatchNo = preAlertBatchNo;
                        
                        var result= B2CController.SubmitB2C(model);
                        if (result.Status > 1)
                        {
                            WayBillController.UpdateB2CPreAlertLogs(result);
                        }
                    }
                    else
                    {
                        Log.Error("获取运单号为{0}的数据失败！".FormatWith(wayBillNumber));
                    }
                }
            }
        }
    }
}
