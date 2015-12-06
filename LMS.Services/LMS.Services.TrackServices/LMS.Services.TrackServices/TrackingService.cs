using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LighTake.Infrastructure.Seedwork;


namespace LMS.Services.TrackServices
{
	public class TrackingService
	{
		private readonly IInTackingLogInfoRepository _inTackingLogInfoRepository;
		private readonly IWayBillInfoRepository _wayBillInfoRepository;
		public TrackingService(IInTackingLogInfoRepository inTackingLogInfoRepository,IWayBillInfoRepository wayBillInfoRepository)
		{
			_inTackingLogInfoRepository = inTackingLogInfoRepository;
			_wayBillInfoRepository = wayBillInfoRepository;
		}

		//获取运单列表信息
		public List<WayBillInfo> GetWayBillInfos(List<string> trackNumber)
		{
			return _wayBillInfoRepository.GetList(p => trackNumber.Contains(p.TrackingNumber));

		}


		//获取内部跟踪号轨迹表
		public List<InTackingLogInfo> GetInTrackingLogInfos(List<string> waybillNumber)
		{
			return _inTackingLogInfoRepository.GetList(a => waybillNumber.Contains(a.WaybillNumber));
		}
	}
}
