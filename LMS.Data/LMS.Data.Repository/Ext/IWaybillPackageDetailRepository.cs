using System;
using System.Collections.Generic;

namespace LMS.Data.Repository
{
    public partial interface IWaybillPackageDetailRepository
    {
        string GetId();
        decimal GetWayBillListWeight(List<string> wayBillNumbers);
    }
}
