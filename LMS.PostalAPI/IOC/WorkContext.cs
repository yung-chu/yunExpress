using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LMS.Core;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.PostalAPI.IOC
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