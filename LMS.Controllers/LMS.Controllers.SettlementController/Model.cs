using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Web.Models;

namespace LMS.Controllers.SettlementController
{
    public class SettlementDetailViewModel
    {
        public string SettlementNumber { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int TotalNumber { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal TotalSettleWeight { get; set; }
        public decimal TotalFee { get; set; }
        public int Status { get; set; }
        public string SalesMan { get; set; }
        public string SalesManTel { get; set; }
        public string SettlementBy { get; set; }
        public Nullable<System.DateTime> SettlementOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }

        public virtual ICollection<SettlementDetailsInfo> SettlementDetailsInfos { get; set; }
    }


    public class NoSettlementListViewModel
    {
        public NoSettlementListViewModel()
        {
            InStorageInfos=new List<InStorageInfo>();
            InStorageProcesses=new List<InStorageProcess>();
        }
        public List<InStorageInfo> InStorageInfos { get; set; }
        public List<InStorageProcess> InStorageProcesses { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string InStorageIDs { get; set; }
    }

    public class SettlementSummaryModelViewModel
    {
        public SettlementSummaryModelViewModel()
        {
            StatusList =new List<SelectListItem>() { new SelectListItem() { Text = "全部", Value = null }, new SelectListItem() { Text = "未结清", Value = "1" }, new SelectListItem() { Text = "已结清", Value = "2" } };
            FilterModel = new SettlementSummaryModelFilterModel();
            PagedList = new PagedList<SettlementSummaryExt>();
        }

        public IPagedList<SettlementSummaryExt> PagedList { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public SettlementSummaryModelFilterModel FilterModel { get; set; }
    }

    public class SettlementSummaryModelFilterModel : SearchFilter
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int? Status { get; set; }
        public bool IsFirstIn { get; set; }
    }

    public class SettlementJsonModel
    {
        public string SettlementNumber { get; set; }
        public string CustomerCode { get; set; }
        public int TotalNumber { get; set; }
        public string TotalWeight { get; set; }
        public string TotalSettleWeight { get; set; }
        public string TotalFee { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
    }

    public class SettlementJson
    {
        public SettlementJson()
        {
            Data=new List<SettlementJsonModel>();
        }
        public int TotalNumber { get; set; }
        public string TotalWeight { get; set; }
        public string TotalSettleWeight { get; set; }
        public string TotalFee { get; set; }
        public List<SettlementJsonModel> Data { get; set; } 
    }

    public class  RechargeTypeModel
    {
        public int RechargeType { get; set; }
        public string RechargeTypeName { get; set; }
    }
    public class UpLoadFile
    {
        public string Address { get; set; }
    }
}