using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity.ExtModel
{
    public class SettlementSummaryExt
    {
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        /// <summary>
        /// 已收货状态下的运单数
        /// </summary>
        public int HaveWaybillCount { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public decimal? Balance { get; set; }
        /// <summary>
        /// 业务经理
        /// </summary>
        public string SalesMan { get; set; }

        public ICollection<SettlementShippingMethodSummaryExt> SettlementShippingMethodSummaryExts { get; set; }
    }

    public class SettlementShippingMethodSummaryExt
    {
        public string CustomerCode { get; set; }
        public string ShippingMethodName { get; set; }
        public int HaveWaybillCount { get; set; }
        public decimal? TotalWeight { get; set; }
        public decimal? TotalSettleWeight { get; set; }
        public decimal? TotalFee { get; set; }
    }
}
