using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.WebAPI.Model
{
    [Serializable]
    public class InFeeTotalInfoExtModel
    {
        public string InStorageID { get; set; }
        public string WayBillNumber { get; set; }

        public string CustomerOrderNumber { get; set; }
        public string CustomerName { get; set; }

        public string CustomerCode { get; set; }

        public Guid CustomerID { get; set; }
        public string TrackingNumber { get; set; }
        public int WayBillStatus { get; set; }
        public int ShippingMethodID { get; set; }
        public string ShippingMethodName { get; set; }
        public string CountryCode { get; set; }
        public string ChineseName { get; set; }
        public decimal SettleWeight { get; set; }
        public decimal Weight { get; set; }
        public int PackageNumber { get; set; }
        ///// <summary>
        ///// 运费
        ///// </summary>
        //public decimal Freight { get; set; }
        ///// <summary>
        ///// 燃油费
        ///// </summary>
        //public decimal FuelCharge { get; set; }
        ///// <summary>
        ///// 挂号费
        ///// </summary>
        //public decimal Register { get; set; }
        ///// <summary>
        ///// 附加费
        ///// </summary>
        //public decimal Surcharge { get; set; }
        /// <summary>
        /// 关税预付服务费
        /// </summary>
        public decimal TariffPrepayFee { get; set; }
        /// <summary>
        /// 总费用
        /// </summary>
        public decimal TotalFee { get; set; }
    }
}