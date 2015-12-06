using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Core
{
    public interface IWorkContext
    {
        User User { get; set; }
        List<DataSourceBinder> BusinessModelList { get; }
        //Process process { get; set; }
    }
}
