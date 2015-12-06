using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;

namespace LMS.Services.EubWayBillServices
{
    public interface IEubWayBillService
    {
        /// <summary>
        /// 获取已申请的运单
        /// </summary>
        /// <returns></returns>
        IList<EubWayBillApplicationInfo> GetApplyList();

        void UpdateEubWayBillInfo(EubWayBillApplicationInfo eubWay);

        EubAccountInfo GetEubAccountInfo(int shippingMethodId);

        EubWayBillApplicationInfo GetEubWayBillApplicationInfo(int id);

        EubWayBillApplicationInfo GetEubWayBillApplicationInfo(string wayBillNumber);

        void StaticLabelDowLoad();
    }
}
