using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Data.Entity;


namespace LMS.Services.TrackServices
{
	public interface ITrackingService
	{
        //获取运单信息
        WayBillInfo GetWayBillInfo(string waybillNumber);
        List<WayBillInfo> GetWayBillInfoList(List<string> trackNumber);

        //是否取到运单信息
        bool IsGetWayBillInfo(string waybillNumber);

        //修改运单状态
        void UpdateWayBillInfo(string waybillNumber);


        //获取内部跟踪号轨迹表
        List<InTrackingLogInfo> GetInTrackingLogInfos(string waybillNumber);
        List<InTrackingLogInfoExt> GetInTrackingLogInfoList(List<string> numberList);
	}
}
