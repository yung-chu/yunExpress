using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{
    public class OutStorageListSearchParam : SearchParam
    {
        public string VenderCode { get; set; }
        public string OutStorageID { get; set; }
        public DateTime? OutStartDate { get; set; }
        public DateTime? OutEndDate { get; set; }
        public string PostBagNumber { get; set; }
    }
}
