using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;

namespace LMS.FrontDesk.Controllers.TrackController.Models
{
    public class ContactTable
    {

        public ContactTable()
        {
            OrderTrackingDetails = new List<OrderTrackingDetailModels>();
            ListInTrackingLogInfo = new List<InTrackingLogInfo>();
            OrderTrackingDetailModelInfo = new List<OrderTrackingDetailModelInfo>();
        }


        public string WaybillNumber { get; set; }
        public string FalseTrackNumber { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public string Remarks { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string InShippingMethodName { get; set; }

        //目的地国家
        public string Destination { get; set; }
        public string EnumStingStatus { get; set; }
        public string CurrentLocation { get; set; }
        public string TrackNumber { get; set; }
        //运单退回
        public bool WaybillReurnStatus { get; set; }
        //运单表的状态
        public int WaybillStatus { get; set; }

        //包裹状态  
        public int? PackageState { get; set; }
        //信息状态  
        public int? InfoState { get; set; }
        //收货几天
        public int? IntervalDays { get; set; }
        public DateTime? LastEventDate { get; set; }
        public string LastEventContent { get; set; }
        public bool IsHold { get; set; }

        //显示外部详细信息
        public List<OrderTrackingDetailModels> OrderTrackingDetails { get; set; }

        //内部数据列表
        public List<InTrackingLogInfo> ListInTrackingLogInfo { get; set; }

        //显示 内、外部轨迹信息 update
        public List<OrderTrackingDetailModelInfo> OrderTrackingDetailModelInfo { get; set; }

        //查不到的单号
        public string NoQueryWaybillnumber { get; set; }

    }

    public class OrderTrackingDetailModels
    {
        public string TrackingNumber { get; set; }
        public bool? IsDisplay { get; set; }
        public DateTime? ProcessDate { get; set; }
        public string ProcessContent { get; set; }
        public string ProcessLocation { get; set; }
        public DateTime? CreatedOn { get; set; }

    }

    public class OrderTrackingDetailModelInfo
    {
        public string WaybillNumber { get; set; }
        public DateTime? ProcessDate { get; set; }
        public string ProcessContent { get; set; }
        public string ProcessLocation { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
