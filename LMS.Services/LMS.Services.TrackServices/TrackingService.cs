using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LighTake.Infrastructure.Seedwork;
using LMS.Services.TrackServices;


namespace LMS.Services.TrackServices
{
	public class TrackingService : ITrackingService
    {
        private readonly IInTrackingLogInfoRepository _inTackingLogInfoRepository;
        private readonly IWayBillInfoRepository _wayBillInfoRepository;


        public TrackingService(IInTrackingLogInfoRepository inTackingLogInfoRepository,
            IWayBillInfoRepository wayBillInfoRepository)
        {
            _inTackingLogInfoRepository = inTackingLogInfoRepository;
            _wayBillInfoRepository = wayBillInfoRepository;

        }

        //获取运单信息
        public WayBillInfo GetWayBillInfo(string waybillNumber)
        {
            return _wayBillInfoRepository.First(p => p.WayBillNumber.Contains(waybillNumber) || p.CustomerOrderNumber.Contains(waybillNumber) || p.TrackingNumber.Contains(waybillNumber));
        }

        //是否取到运单信息
        public bool IsGetWayBillInfo(string waybillNumber)
        {
            WayBillInfo getWayBillInfo = _wayBillInfoRepository.First(p => p.WayBillNumber == waybillNumber || p.CustomerOrderNumber == waybillNumber || p.TrackingNumber == waybillNumber);
            if (getWayBillInfo == null)
                return true;
            else
                return false;
        }

        //修改运单信息
        public void UpdateWayBillInfo(string waybillNumber)
        {
            WayBillInfo getWayBillInfo = _wayBillInfoRepository.First(p => p.WayBillNumber == waybillNumber);
            getWayBillInfo.Status = WayBill.StatusToValue(WayBill.StatusEnum.Delivered);
            _wayBillInfoRepository.Modify(getWayBillInfo);
            _wayBillInfoRepository.UnitOfWork.Commit();

        }


        //获取运单列表信息
        public List<WayBillInfo> GetWayBillInfoList(List<string> trackNumber)
        {
            return _wayBillInfoRepository.GetList(p => trackNumber.Contains(p.WayBillNumber) || trackNumber.Contains(p.CustomerOrderNumber) || trackNumber.Contains(p.TrackingNumber));
        }


        //获取内部跟踪号轨迹表
        public List<InTrackingLogInfo> GetInTrackingLogInfos(string waybillNumber)
        {
            return _inTackingLogInfoRepository.GetList(a => a.WayBillNumber == waybillNumber);
        }
        
        public List<InTrackingLogInfoExt> GetInTrackingLogInfoList(List<string> numberList)
        {
            var model = new List<InTrackingLogInfoExt>();
            if (numberList.Any())
            {        
                return _inTackingLogInfoRepository.GetInTrackingLogInfo(numberList);
            }

            return model;
	    }

    }
}
