using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class EubWayBillParam
    {
        public EubWayBillParam()
        {
            WayBillInfos=new List<WayBillInfo>();
        }

        public List<WayBillInfo> WayBillInfos { get; set; }


        public int PrintFormat { get; set; }

        public string PrintFormatValue { get; set; }
    }
}
