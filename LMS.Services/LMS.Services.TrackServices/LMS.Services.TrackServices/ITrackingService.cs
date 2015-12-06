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

		List<WayBillInfo> GetWayBillInfos(List<string> trackNumber);
			//获取内部跟踪号轨迹表
		List<InTackingLogInfo> GetInTrackingLogInfos(List<string> waybillNumber);
	}
}
