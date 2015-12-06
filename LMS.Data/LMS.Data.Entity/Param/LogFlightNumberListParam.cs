using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{
    public class LogFlightNumberListParam : SearchParam
    {
        public string MailTotalPackageNumber { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
