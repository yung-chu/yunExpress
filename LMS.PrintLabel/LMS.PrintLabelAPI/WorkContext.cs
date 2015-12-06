using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LMS.Core;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.PrintLabelAPI
{
    public class WorkContext : IWorkContext
    {
        public User User { get; set; }
        public List<DataSourceBinder> BusinessModelList { get; private set; }
        public Process process { get; set; }
    }
}