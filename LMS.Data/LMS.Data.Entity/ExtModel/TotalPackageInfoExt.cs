using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class TotalPackageInfoExt
    {
        public TotalPackageInfoExt()
        {
            Info = new TotalPackageInfo();
            TraceInfos = new List<TotalPackageTraceInfo>();
        }
        public TotalPackageInfo Info { get; set; }
        public List<TotalPackageTraceInfo> TraceInfos { get; set; }
    }
}
