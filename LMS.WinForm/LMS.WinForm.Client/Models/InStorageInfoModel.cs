using LMS.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.WinForm.Client.Models
{
    public class InStorageInfoModel : LighTake.Infrastructure.Seedwork.Entity
    {
        public  string InStorageID { get; set; }
        public  string ReceivingClerk { get; set; }
        public  string CustomerCode { get; set; }
        public  decimal TotalWeight { get; set; }
        public  int TotalQty { get; set; }
        public  decimal MaterialsFee { get; set; }
        public  decimal TotalFee { get; set; }
        public  decimal Freight { get; set; }
        public  decimal Register { get; set; }
        public  decimal FuelCharge { get; set; }
        public  decimal Surcharge { get; set; }
        public  int Status { get; set; }
        public  DateTime CreatedOn { get; set; }
        public  string CreatedBy { get; set; }
        public  DateTime LastUpdatedOn { get; set; }
        public  string LastUpdatedBy { get; set; }
        public  string Remark { get; set; }

        public List<WayBillInfoModel> WayBillInfos { get; set; }
    }

    public class InstorageInfoViewModel
    {
        public string InStorageID { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public string PaymentName { get; set; }
        public decimal TotalFee { get; set; }
        public decimal Freight { get; set; }
        public decimal Register { get; set; }
        public decimal FuelCharge { get; set; }
        public decimal TariffPrepayFee { get; set; }
        public decimal Surcharge { get; set; }
        public int TotalQty { get; set; }
        public int TotalPieces { get; set; }//同次入仓的总件数
        public int Pieces { get; set; }
        public decimal? TotalWeight { get; set; }
        public decimal PhysicalTotalWeight { get; set; }
        public string InShippingMethodName { get; set; }
        public string CountryCode { get; set; }
        public string ChineseName { get; set; }
        public decimal? SettleWeight { get; set; }
        public decimal? Weight { get; set; }
        public string ReceivingClerk { get; set; }
        public DateTime CreatedOn { get; set; }
        public decimal WaybillNumberTotalFee { get; set; }
        public string CustomerOrderNumber { get; set; }

        public int TotalWayBill { get; set; }//同次入仓的总票数
        public string ChineseNameAndCode { get { return ChineseName + "(" + CountryCode + ")"; } }
    }
}
