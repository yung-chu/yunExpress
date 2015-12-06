using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{
    public class MailExchangeBagLogsParam : SearchParam
    {
        public string TrackNumber { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
