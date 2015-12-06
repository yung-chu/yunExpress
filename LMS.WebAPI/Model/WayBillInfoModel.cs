using LighTake.Infrastructure.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.WebAPI.Model
{
    public class WayBillInfoModel : LighTake.Infrastructure.Seedwork.Entity
    {
        public WayBillInfoModel()
        {
            //ShippingInfo = new ShippingInfoModel();
            //ApplicationInfos = new List<ApplicationInfoModel>();
            //AbnormalWayBillLog = new AbnormalWayBillLogModel();
        }
        public string WayBillNumber { get; set; }
        public string CustomerOrderNumber { get; set; }
        public int CustomerOrderID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCodeName { get; set; }
        public string TrackingNumber { get; set; }
        public string RawTrackingNumber { get; set; }
        public Nullable<System.DateTime> TransferOrderDate { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public Nullable<decimal> SettleWeight { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Height { get; set; }
        public Nullable<int> GoodsTypeID { get; set; }
        public bool IsReturn { get; set; }
        public bool IsHold { get; set; }
        public bool IsBattery { get; set; }
        public int Status { get; set; }
        public string OutStorageID { get; set; }
        public string InStorageID { get; set; }
        public Nullable<int> ShippingInfoID { get; set; }
        public virtual Nullable<int> SenderInfoID { get; set; }
        public string CountryCode { get; set; }
        public Nullable<int> InsuredID { get; set; }
        public Nullable<int> AbnormalID { get; set; }
        public Nullable<int> InShippingMethodID { get; set; }
        public Nullable<int> OutShippingMethodID { get; set; }
        public string InShippingMethodName { get; set; }
        public string OutShippingMethodName { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public string VenderName { get; set; }
        public DateTime? AbnormalCreateOn { get; set; }
        public string AbnormalCreateBy { get; set; }
        public string AbnormalTypeName { get; set; }
        public string AbnormalDescription { get; set; }
        public string InShippingName { get; set; }
        public string OutShippingName { get; set; }
        public DateTime? InStorageTime { get; set; }
        public DateTime? OutStorageTime { get; set; }
        //public ShippingInfoModel ShippingInfo { get; set; }
        //public SenderInfoModel SenderInfo { get; set; }
        //public InsuredCalculationModel InsuredCalculation { get; set; }
        //public GoodsTypeInfoModel GoodsTypeInfo { get; set; }
        //public AbnormalWayBillLogModel AbnormalWayBillLog { get; set; }
        //public CustomerOrderInfoModel CustomerOrderInfo { get; set; }
        //public List<ApplicationInfoModel> ApplicationInfos { get; set; }
    }
}