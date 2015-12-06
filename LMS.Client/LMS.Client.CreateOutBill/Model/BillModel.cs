using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Client.CreateOutBill.Model
{
    public class BillModel
    {
        /// <summary>
        /// 收货费用ID
        /// </summary>
        public int ReceivingExpenseId { get; set; }
        /// <summary>
        /// 运单号
        /// </summary>
        public string WayBillNumber { get; set; }
        /// <summary>
        /// 客户订单号
        /// </summary>
        public string CustomerOrderNumber { get; set; }
        /// <summary>
        /// 运单创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// 收货时间
        /// </summary>
        public DateTime InStorageCreatedOn { get; set; }
        /// <summary>
        /// 跟踪号
        /// </summary>
        public string TrackingNumber { get; set; }
        /// <summary>
        /// 发货国家
        /// </summary>
        public string ChineseName { get; set; }
        /// <summary>
        /// 运输方式
        /// </summary>
        public string InShippingMethodName { get; set; }
        /// <summary>
        /// 运输方式ID
        /// </summary>
        public int InShippingMethodId { get; set; }
        /// <summary>
        /// 入仓单号
        /// </summary>
        public string InStorageID { get; set; }
        /// <summary>
        /// 结算重量
        /// </summary>
        public decimal SettleWeight { get; set; }
        /// <summary>
        /// 称重重量
        /// </summary>
        public decimal Weight { get; set; }
        /// <summary>
        /// 件数
        /// </summary>
        public int CountNumber { get; set; }
        /// <summary>
        /// 运费
        /// </summary>
        public decimal Freight { get; set; }
        /// <summary>
        /// 燃油费
        /// </summary>
        public decimal FuelCharge { get; set; }
        /// <summary>
        /// 挂号费
        /// </summary>
        public decimal Register { get; set; }
        /// <summary>
        /// 附加费
        /// </summary>
        public decimal Surcharge { get; set; }
        /// <summary>
        /// 关税预付费
        /// </summary>
        public decimal TariffPrepayFee { get; set; }
        /// <summary>
        /// 特殊费
        /// </summary>
        public decimal SpecialFee { get; set; }
        /// <summary>
        /// 偏远附加费
        /// </summary>
        public decimal RemoteAreaFee { get; set; }
    }
}
