using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LMS.Data.Entity.Param;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial interface IReceivingBillRepository
    {
        IPagedList<ReceivingBill> GetPagedList(ReceivingBillParam param);
    }
}
