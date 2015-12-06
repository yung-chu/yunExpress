using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.BizLogging;
using LighTake.Infrastructure.Common.BizLogging.BizLogWcf;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Services.OperateLogServices
{
	public class OperateLogServices : IOperateLogServices
    {
		//写入
	    public bool WriteLog<T>(BizLog log,T details)
	    {
		    bool result;
		    try
		    {
				BizLogger.WriteLogBS(log, details);
			    result = true;
		    }
		    catch (Exception ex)
		    {
				result = false;
				Log.Exception(ex);
		    }
			return result;
	    }


		//查询
		public PagedList<BizLogModel> QueryBizLogInfo(BizLog log, int pageIndex, int pageSize, DateTime? startTime, DateTime? endTime, out int totalRecords)
		{
			return BizLogger.QueryBizLogInfo(log, pageIndex, pageSize, startTime, endTime, out totalRecords);
		}

		public List<BizLogModel> ShowBizLogInfomation(BizLog log)
		{
			return BizLogger.ShowBizLogInfo(log);
		}
    }
}
