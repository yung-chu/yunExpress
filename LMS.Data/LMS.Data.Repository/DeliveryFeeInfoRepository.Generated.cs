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
	public partial class DeliveryFeeInfoRepository : Repository<DeliveryFeeInfo>, IDeliveryFeeInfoRepository
	{

		#region .Constructor

		public DeliveryFeeInfoRepository(LMS_DbContext unitOfWork)
            : base(unitOfWork)
		{

		}

		#endregion
			
	}
}
