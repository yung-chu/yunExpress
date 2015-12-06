using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Common;
using LMS.Data.Entity;

namespace LMS.Data.Repository
{
	public partial interface IJobErrorLogRepository
	{
		IPagedList<JobErrorLogExt> GetPagedList(JobErrorLogsParam param);
	}

}
