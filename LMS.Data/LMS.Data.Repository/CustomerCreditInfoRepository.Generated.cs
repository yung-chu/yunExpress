using System;
using System.Linq;
using System.Collections.Generic;

using System.Linq.Expressions;
using LMS.Data.Context;
using LMS.Data.Entity;
using LighTake.Infrastructure.Seedwork.EF;

// This file is auto generated and will be overwritten as soon as the template is executed
// Do not modify this file...
	
namespace LMS.Data.Repository
{   
	public partial class CustomerCreditInfoRepository : Repository<CustomerCreditInfo>, ICustomerCreditInfoRepository
	{

		#region .Constructor

		public CustomerCreditInfoRepository(LMS_DbContext unitOfWork)
            : base(unitOfWork)
		{

		}

		#endregion
			
	}
}
