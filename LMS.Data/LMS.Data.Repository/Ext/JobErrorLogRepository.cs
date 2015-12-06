using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using LighTake.Infrastructure.Common;
using LMS.Data.Context;
using LMS.Data.Entity;

namespace LMS.Data.Repository
{
    public partial class JobErrorLogRepository
	{
		public IPagedList<JobErrorLogExt> GetPagedList(JobErrorLogsParam param)
	    {
		    var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

			var numberList = new List<string>();
			Expression<Func<JobErrorLog, bool>> filter = p => true;
		
			if (!string.IsNullOrWhiteSpace(param.WayBillNumber))
		    {
				numberList = param.WayBillNumber.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
		    }


			//取当天记录
			if (param.StartTime != null && param.EndTime != null)
			{
				if (param.StartTime == param.EndTime)
				{
					param.EndTime = param.EndTime.Value.AddHours(23).AddMinutes(59);
				}
			}



			filter = filter.AndIf(a => numberList.Contains(a.WayBillNumber), numberList.Count!=0)
					.AndIf(a => a.CreatedOn >= param.StartTime, param.StartTime.HasValue)
					.AndIf(a => a.CreatedOn <= param.EndTime, param.EndTime.HasValue)
					.AndIf(a => a.JobType == param.JobType, param.JobType.HasValue)
		            .And(a=>a.ErrorType==2).And(a=>a.IsCorrect==false);//必要条件

			var result = from a in ctx.JobErrorLogs.Where(filter)
						 orderby  a.CreatedOn descending 
				         select new JobErrorLogExt
				         {
					         CreatedBy = a.CreatedBy,
							 CreatedOn = a.CreatedOn,
							 ErrorBody = a.ErrorBody,
							 WayBillNumber = a.WayBillNumber
				         } ;

			return result.ToPagedList(param.Page, param.PageSize);
	    }

	}
}
