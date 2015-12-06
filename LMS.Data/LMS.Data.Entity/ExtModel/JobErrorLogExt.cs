using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
	public class JobErrorLogExt : LighTake.Infrastructure.Seedwork.Entity
	{
		public virtual int JobErrorLogID { get; set; }
		public virtual string WayBillNumber { get; set; }
		public virtual Nullable<int> JobType { get; set; }
		public virtual Nullable<int> ErrorType { get; set; }
		public virtual string ErrorBody { get; set; }
		public virtual bool IsCorrect { get; set; }
		public virtual System.DateTime CreatedOn { get; set; }
		public virtual string CreatedBy { get; set; }
		public virtual System.DateTime LastUpdatedOn { get; set; }
		public virtual string LastUpdatedBy { get; set; }
	}
}
