using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class CreateInStorageExt
    {
        public CreateInStorageExt()
        {
            InStorage = new InStorageInfo();
            WayBillInfos = new List<WayBillInfoExt>();
        }
        public InStorageInfo InStorage { get; set; }
        public List<WayBillInfoExt> WayBillInfos { get; set; }
        public DateTime? BusinessDate { get; set; }//业务日期

    }
    public class WayBillInfoExt
    {
        public int CustomerType { get; set; }
        public string CustomerCode { get; set; }
        public int GoodsTypeID { get; set; }
        public int ShippingMethodId { get; set; }
        public string WayBillNumber { get; set; }
        public decimal Freight { get; set; }
        public decimal FuelCharge { get; set; }
        public decimal Register { get; set; }
        public decimal Surcharge { get; set; }
        public decimal TariffPrepay { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string TrackingNumber { get; set; }
        public decimal SettleWeight { get; set; }
        public decimal Weight { get; set; }

        public int? CustomerOrderID { get; set; }
    }
}
