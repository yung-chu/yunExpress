using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{
    public class FubListParam : SearchParam
    {
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
        public int DateWhere { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string CreatedBy { get; set; }
    }
}
