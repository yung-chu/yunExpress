using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;

namespace LMS.Data.Repository
{
    /// <summary>
    ///【重要提示】:分配跟踪号,不在提供数据底层的接口，
    ///分配跟踪号请使用LMS.Services.TrackingNumberServices.TrackNumberAssignStandard方法
    ///2014-10-31 by daniel
    /// </summary>
    public partial interface ITrackingNumberDetailInfoRepository
    {
        SelectTrackingNumberExt GetTrackingNumberDetails(TrackingNumberParam param);

        /// <summary>
        /// 判断跟踪号是否重复
        /// </summary>
        /// <param name="trackNumbers"></param>
        /// <returns></returns>
        List<string> CheckRepeatedTrackNumbers(IEnumerable<string> trackNumbers);

        #region 已经被废弃
        //TrackingNumberDetailInfo GetTrackingNumberDetailInfo(int shippingMethodId, string countryCode);
        //IList<string> GetListByShippingMethodId(int shippingMethodId);

        /// <summary>
        /// 分配跟踪号[标准接口] 已经被废弃,不在提供数据底层的接口，
        /// 2014-10-31 by daniel
        /// </summary>
        /// <param name="shippingMethodId">运输方式</param>
        /// <param name="count">数量(0表示取全部)</param>
        /// <param name="countryCode">国家代码(可空)</param>
        /// <returns></returns>
        //IList<string> TrackNumberAssignStandard(int shippingMethodId, int count, string countryCode);
        //IList<TrackingNumberDetailInfo> GetListByShippingMethodId(int shippingMethodId, int top); 
        //IList<TrackingNumberDetailInfo> GetTrackingNumberDetailList();

        /// <summary>
        /// 根据运输方式Id和国家代码获取未使用的跟踪号
        /// </summary>
        /// <param name="shippingMethodId">运输方式Id</param>
        /// <param name="countryCode">国家代码</param>
        /// <param name="detailIds">要排除的TrackingNumberDetailID</param>
        /// <returns></returns>
        //TrackingNumberDetailInfo GetTrackingNumberDetailInfo(int shippingMethodId, string countryCode, List<int> detailIds);        
        #endregion
    }
}
