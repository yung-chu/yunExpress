using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Services.TrackingNumberServices
{
    public interface ITrackingNumberService
    {

        void AddTrackingNumber(TrackingNumberInfo trackingNumberInfo);

        void UpdateTrackingNumber(TrackingNumberInfo trackingNumberInfo);

        TrackingNumberInfo GetTrackingNumberInfo(string id);

        /// <summary>
        /// 上传跟踪号
        /// Add by zhengsong
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        List<TrackingNumberDetailInfo> UploadTrackingNumberList(TrackingNumberInfo model);
        /// <summary>
        /// 更新TrackingNumberInfoDetails表运单号和状态，更新WayBillInfos表跟踪号
        /// </summary>
        /// <param name="wayBillInfo"></param>
        /// <returns></returns>
        bool UpdateTrackingNumberInfoDetailAndWayBillInfo(WayBillInfo wayBillInfo);

        IPagedList<CountryExt> GetPagedList(CountryParam param = null);
        List<TrackingNumberDetailInfo> GetTrackingNumberDetailById(string id);
        bool DisableTrackingNumber(string id);


        //TrackingNumberDetailInfo GetTrackingNumberDetailInfo(int shippingMethodId, string countryCode, List<int> detailIds);
        IPagedList<TrackingNumberDetailInfo> GetTrackingNumberPagedList(int page, int pageSize, string trackingNumberID);

        /// <summary>
        /// 分配跟踪号[标准接口]
        /// 2014-10-29 by daniel
        /// </summary>
        /// <param name="shippingMethodId">运输方式</param>
        /// <param name="count">数量(0表示取全部)</param>
        /// <param name="countryCode">国家代码(可空)</param>
        /// <returns></returns>     
        IList<string> TrackNumberAssignStandard(int shippingMethodId, int count, string countryCode);

        //List<TrackingNumberDetailInfo> GetTrackingNumberDetailList();
        List<TrackingNumberInfo> GetTrackingNumbers(List<int> shippingMethodIds );




		//
	    ShippingInfo GetShippingInfo(string waybillnumber);
    }
}
