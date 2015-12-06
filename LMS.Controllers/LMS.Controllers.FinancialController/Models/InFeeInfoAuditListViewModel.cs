using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Web.Models;

namespace LMS.Controllers.FinancialController
{
    public class InFeeInfoAuditListViewModel
    {
        public InFeeInfoAuditListViewModel()
        {
            PagedList = new PagedList<InFeeInfoAuditListModel>();
            InFeeInfoAuditList=new List<InFeeInfoAuditListModel>();
            SearchWheres=new List<SelectListItem>();
            FilterModel=new InFeeInfoAuditFilterModel();
            StatusList = new List<SelectListItem>();
            DateWheres=new List<SelectListItem>();
        }

        public IPagedList<InFeeInfoAuditListModel> PagedList { get; set; }
        public List<InFeeInfoAuditListModel> InFeeInfoAuditList { get; set; }
        public InFeeInfoAuditFilterModel FilterModel { get; set; }
        public IList<SelectListItem> SearchWheres { get; set; }
        public IList<SelectListItem> StatusList { get; set; }
        public IList<SelectListItem> DateWheres { get; set; }

    }

    public class InFeeInfoAuditListModel
    {
        //收货
        public string WayBillNumber { get; set; }
        //public string ReceivingBillID { get; set; }
        public string CustomerOrderNumber { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string Auditor { get; set; }
        public DateTime? AuditorDate { get; set; }
        public DateTime OutDateTime { get; set; }
        public int? OperationType { get; set; }
        //运单信息
        public string TrackingNumber { get; set; }
        public DateTime? InStorageCreatedOn { get; set; }
        public int? InShippingMethodID { get; set; }
        public string InShippingMethodName { get; set; }
        public string CountryCode { get; set; }
        public decimal Weight { get; set; }
        public decimal SettleWeight { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        //费用
        public decimal? Freight { get; set; }
        public decimal? Register { get; set; }
        public decimal? FuelCharge { get; set; }
        public decimal? Surcharge { get; set; }
        public decimal? TariffPrepayFee { get; set; }
        public decimal? TotalFee { get; set; }
        public decimal? SpecialFee { get; set; }
        public decimal? RemoteAreaFee { get; set; }
    }

    public class InFeeInfoAuditFilterModel : FinancialSearchFilter
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int? ShippingMethodId { get; set; }
        public string ShippingName { get; set; }
        public string CountryCode { get; set; }
        //public int DateWhere { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? SearchWhere { get; set; }
        public string SearchContext { get; set; }
        public int? Status { get; set; }
        public bool IsFistIn { get; set; }
    }
}