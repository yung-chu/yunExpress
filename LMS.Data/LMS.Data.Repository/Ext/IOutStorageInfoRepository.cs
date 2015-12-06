using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using LMS.Data.Context;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial interface IOutStorageInfoRepository
    {
        List<string> GetTotalPackageNumberList(string venderCode);

        IPagedList<OutStorageInfoExt> GetOutStoragePagedList(OutStorageListSearchParam param);
    }
}
