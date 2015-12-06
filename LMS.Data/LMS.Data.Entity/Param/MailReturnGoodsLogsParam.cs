using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{
    public class MailReturnGoodsLogsParam : SearchParam
    {
        public string TrackNumber { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? ReasonType { get; set; }
        public string ReturnBy { get; set; }
    }

    public class MailHoldLogsParam : SearchParam
    {
        public string[] TrackNumbers { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
