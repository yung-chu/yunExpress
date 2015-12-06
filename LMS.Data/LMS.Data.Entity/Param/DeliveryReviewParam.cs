using LighTake.Infrastructure.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity.Param
{
    public class DeliveryReviewParam : SearchParam
    {
        public string VenderCode { get; set; }
        public string VenderName { get; set; }
        public string ShippingMethodId { get; set; }
        public string ShippingName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Status { get; set; }
        //public string CountryCode { get; set; }
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
        public string UserName { get; set; }

        public int[] ShippingMethodIds { get; set; }
        public bool? IsExportExcel { get; set; }
    }
}
