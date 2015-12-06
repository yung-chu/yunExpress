using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{             
    public class ExpressPrintWayBillParam:SearchParam
    {
        public int ShippingMethodId { get; set; }
        public string ShippingName { get; set; }
        public string CustomerCode { get; set; }
        public string NickName { get; set; }
        public string CountryCode { get; set; }
        public string VenderCode { get; set; }
        public decimal? StartWeight { get; set; }
        public decimal? EndWeight { get; set; }
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int ShippingMethodType { get; set; }
        public int IsVender { get; set; }

		//打印状态
	    public string PrintStatus { get; set; }
		
	    public List<string> WayBillNumberList { get; set; }

    }

}
