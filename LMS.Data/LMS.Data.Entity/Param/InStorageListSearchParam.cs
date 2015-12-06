using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{
    public class InStorageListSearchParam : SearchParam
    {
        public string CustomerCode { get; set; }
        public string InStorageID { get; set; }
        public DateTime? InStartDate { get; set; }
        public DateTime? InEndDate { get; set; }
        public int? ShippingMethodId { get; set; }
    }
}
