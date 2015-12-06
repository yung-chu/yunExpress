using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Seedwork;

namespace LMS.Data.Entity
{
    public class EubWayBillApplicationInfoParam : SearchParam
    {
        public EubWayBillApplicationInfoParam()
        {
            ShippingMethods = new List<int>();
        }

        /// <summary>
        /// 运输方式
        /// </summary>
        public int? ShippingMethodId { get; set; }

        /// <summary>
        /// 时间类型
        /// </summary>
        public int TimeType { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? CreatedOnFrom { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? CreatedOnTo { get; set; }

        /// <summary>
        /// 发货国家
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 客房订单号
        /// </summary>
        public string CustomerOrderNumber { get; set; }

        /// <summary>
        /// 运单单号
        /// </summary>
        public string WayBillNumber { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNumber { get; set; }
        /// <summary>
        /// 跟踪号
        /// </summary>
        public string TrackNumber { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }

        public int? PrintFormat { get; set; }

        public List<int> ShippingMethods { get; set; }
    }
}
