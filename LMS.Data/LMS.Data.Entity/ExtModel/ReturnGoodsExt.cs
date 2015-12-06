using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity.ExtModel
{
    public class ReturnGoodsExt
    {
        public string ReGoodsId { get; set; }
        public string UserName { get; set; }
        public string WayBillNumber { get; set; }
        public string CustomerOrderNumber { get; set; }

        public string CustomerName { get; set; }

        public string CustomerCode { get; set; }

        public Guid CustomerID { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        public int PackageNumber { get; set; }

        /// <summary>
        /// 退货重量
        /// </summary>
        public decimal Weight { get; set; }
        /// <summary>
        /// 原单原重量
        /// </summary>
        public decimal WayBillWeight { get; set; }

        /// <summary>
        /// 国家名称
        /// </summary>
        public string CountryName { get; set; }

        public string ShippingMethodName { get; set; }
        /// <summary>
        /// 退货类型 1-为内部 2-为外部
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 退货原因
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 退货原因备注
        /// </summary>
        public string ReasonRemark { get; set; }
        /// <summary>
        /// 是否退运费
        /// </summary>
        public bool IsReturnShipping { get; set; }

        /// <summary>
        /// 是否直接退货
        /// </summary>
        public bool IsDirectReturnGoods { get; set; }

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
        /// <summary>
        /// 入仓单号
        /// </summary>
        public string InStorageID { get; set; }
    }
}
