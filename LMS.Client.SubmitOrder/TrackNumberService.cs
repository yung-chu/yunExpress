using LighTake.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Client.SubmitOrder
{
    /// <summary>
    /// 跟踪号服务简化版
    /// by daniel 2014-10-31
    /// </summary>
    public class TrackNumberService
    {
        /// <summary>
        /// 分配跟踪号[标准接口]
        /// 2014-10-29 by daniel
        /// </summary>
        /// <param name="shippingMethodId">运输方式Id</param>
        /// <param name="count">数据量</param>
        /// <param name="countryCode">国家代码</param>
        /// <returns></returns>
        public IList<string> TrackNumberAssignStandard(int shippingMethodId, int count, string countryCode)
        {
            using (RefAPI.TrackNumber.TrackNumberAssignClient client = new RefAPI.TrackNumber.TrackNumberAssignClient())
            {
                var re = client.TrackNumberAssignStandard(shippingMethodId, count, countryCode);

                if (re.IsSuccess)
                {
                    return re.Data.ToList();
                }
                else
                {
                    throw new BusinessLogicException(re.ErrorMessage);
                }
            }
        }
    }
}
