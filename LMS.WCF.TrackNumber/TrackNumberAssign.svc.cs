using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using LMS.Data.Context;
using LMS.WCF.TrackNumber.Dto;
using LMS.Data.Repository;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.WCF.TrackNumber
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“TrackNumberAssign”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 TrackNumberAssign.svc 或 TrackNumberAssign.svc.cs，然后开始调试。
    public class TrackNumberAssign : ITrackNumberAssign
    {
        static object _trackNumberAssignLock = new object();
        static readonly TrackingNumberDetailInfoRepository _reposity = new TrackingNumberDetailInfoRepository(new LMS_DbContext());

        public ResutlInfo<string> TrackNumberAssignStandard(int shippingMethodId, int count, string countryCode)
        {
            ResutlInfo<string> re = new ResutlInfo<string>();
            re.IsSuccess = false;
            if (shippingMethodId <= 0)
            {
                re.ErrorMessage = "运输方式ID不能为空.";
                return re;
            }
            if (count <= 0 || count > 1000)
            {
                re.ErrorMessage = "每次只能获取1~1000个跟踪号.";
                return re;
            }
            lock (_trackNumberAssignLock)
            {
                try
                {
                    var dataList = _reposity.TrackNumberAssignStandard(shippingMethodId, count, countryCode);
                    re.IsSuccess = true;
                    re.Data = new List<string>(dataList);
                }
                catch (BusinessLogicException ex)
                {
                    re.ErrorMessage = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    re.ErrorMessage = "系统忙请稍后再试!";
                }
            }

            return re;
        }
    }
}
