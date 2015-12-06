using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LighTake.Infrastructure.Seedwork;

namespace LMS.UserCenter.Controllers.OrderController.Models
{
    public class EubWayBillFilter : SearchParam
    {
        /// <summary>
        /// 运输方式
        /// </summary>
        public int? ShippingMethodId { get; set; }

        public int TimeType { get; set; }

        public int QueryNumber { get; set; }

        public string Numbers { get; set; }

        /// <summary>
        /// 交货时间开始
        /// </summary>
        public DateTime? CreatedOnFrom { get; set; }

        /// <summary>
        /// 交货时间结束
        /// </summary>
        public DateTime? CreatedOnTo { get; set; }

        /// <summary>
        /// 发货国家
        /// </summary>
        public string CountryCode { get; set; }


        /// <summary>
        /// 客户订单状态
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 打印规格
        /// </summary>
        public int PrintFormat { get; set; }

        public string WayBillNumbers { get; set; }
    }
}