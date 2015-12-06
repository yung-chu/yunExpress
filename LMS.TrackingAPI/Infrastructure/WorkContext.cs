using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Core;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.TrackingAPI.Infrastructure
{
    public class WorkContext : IWorkContext
    {

        public User User
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public List<DataSourceBinder> BusinessModelList
        {
            get { throw new NotImplementedException(); }
        }

    }
}
