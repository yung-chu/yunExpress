using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial interface INoForecastAbnormalRepository
    {
        IPagedList<NoForecastAbnormalExt> GetNoForecastAbnormalExtPagedList(NoForecastAbnormalParam param);

        List<NoForecastAbnormalExt> GetNoForecastList(IEnumerable<int> noForecastAbnormalIds);
    }
}
