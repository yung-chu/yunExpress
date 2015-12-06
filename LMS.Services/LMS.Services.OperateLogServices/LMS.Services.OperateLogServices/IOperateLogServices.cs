using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.BizLogging;
using LighTake.Infrastructure.Common.BizLogging.BizLogWcf;

namespace LMS.Services.OperateLogServices
{
	public interface IOperateLogServices
	{    
		 bool WriteLog<T>(BizLog log,T details);

		 PagedList<BizLogModel> QueryBizLogInfo(BizLog log, int pageIndex, int pageSize, DateTime? startTime, DateTime? endTime, out int totalRecords);

		 //显示模块列表
		 List<BizLogModel> ShowBizLogInfomation(BizLog log);

	}
}
