using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LighTake.Infrastructure.Common;
using LMS.Data.Entity;

namespace LMS.Controllers.FinancialController
{
	public class JobErrorLogs
	{
		public JobErrorLogs()
		{
			FilterModel = new JobErrorLogFilterModel();
			PagedList = new PagedList<GetJobErrorLogs>();
			StatusList=new List<SelectListItem>();
		}

		public IPagedList<GetJobErrorLogs> PagedList { get; set; }
		public JobErrorLogFilterModel FilterModel { get; set; }
		public IList<SelectListItem> StatusList { get; set; }
	}

	public class GetJobErrorLogs
	{
		public  string WayBillNumber { get; set; }
		public  Nullable<int> JobType { get; set; }
		public  Nullable<int> ErrorType { get; set; }
		public  string ErrorBody { get; set; }
		public  bool IsCorrect { get; set; }
		public  System.DateTime CreatedOn { get; set; }
		public  string CreatedBy { get; set; }

	}
}