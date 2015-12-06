using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Controllers.WayBillController
{
    public class ExpressPrintWayBillViewModel
    {
        public ExpressPrintWayBillViewModel()
        {
            Param=new ExpressPrintWayBillParam();
            ShippingMethods=new List<SelectListItem>();
            SearchWheres=new List<SelectListItem>();
            VenderList=new List<SelectListItem>();
            ExpressPrintWayBills = new PagedList<ExpressPrintWayBillExt>();
			WayBillPrintLogLists = new List<WayBillPrintLogModel>();
        }
        public IPagedList<ExpressPrintWayBillExt> ExpressPrintWayBills { get; set; }
        public ExpressPrintWayBillParam Param { get; set; }
        public List<SelectListItem> ShippingMethods { get; set; }
        public List<SelectListItem> SearchWheres { get; set; }
        public List<SelectListItem> VenderList { get; set; }

		//快递打印日志列表
		public List<WayBillPrintLogModel> WayBillPrintLogLists { get; set; }

    }

    public class ExpressPrintWayBill
    {
        public string WayBillNumber { get; set; }
        public Nullable<int> CustomerOrderID { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string CustomerCode { get; set; }
        public string TrackingNumber { get; set; }
        public Nullable<decimal> SettleWeight { get; set; }
        public int Status { get; set; }
        public string CountryCode { get; set; }
        public Nullable<int> InShippingMethodID { get; set; }
        public Nullable<int> OutShippingMethodID { get; set; }
        public string OutShippingMethodName { get; set; }
        public string VenderCode { get; set; }
        public string VenderName { get; set; }
        public bool IsPrinter { get; set; }
        public DateTime CreatedOn { get; set; }

		//打印状态
	    public string IsPrintStatus { get; set; }

    }

	//快递打印日志列表
	public class WayBillPrintLogModel
	{
		 public string Waybillnumber { get; set; }
		 public string SendGoodsVender { get; set; }
		 public string SendGoodsChannel{ get; set; }
		 public string NewTrackNumber{ get; set; }
		 public string PrintPerson{ get; set; }
		 public Nullable<System.DateTime> PrintDate { get; set; }

	}
}