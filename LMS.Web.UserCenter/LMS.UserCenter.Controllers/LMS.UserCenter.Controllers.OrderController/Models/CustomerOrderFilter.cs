using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LighTake.Infrastructure.Seedwork;

namespace LMS.UserCenter.Controllers.OrderController.Models
{
    public class CustomerOrderFilter : SearchParam
    {
        /// <summary>
        /// 运输方式
        /// </summary>
        public int? ShippingMethodId { get; set; }

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
        /// 客户订单号
        /// </summary>
        public string CustomerOrderNumber { get; set; }

        public string SearchContext { get; set; }

        public int? SearchWhere { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        public string WayBillNumber { get; set; }

        /// <summary>
        /// 客户订单状态
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 客户订单Hold状态
        /// </summary>
        public bool IsHold { get; set; }

        /// <summary>
        /// 是否已收货订单
        /// </summary>
        public bool IsReceived { get; set; }

        /// <summary>
        /// 是否已发货订单
        /// </summary>
        public bool IsDeliver { get; set; }

        /// <summary>
        /// 是否为已提交订单
        /// </summary>
        public bool IsSubmitted { get; set; }

        /// <summary>
        /// 是否已打印
        /// </summary>
        public bool? IsPrinted { get; set; }

        /// <summary>
        /// 所有订单
        /// </summary>
        public bool IsAll { get; set; }

        public int AddressLabelId { get; set; }

        public string PrintTypeId { get; set; }

        /// <summary>
        /// 所要导出的字段Ids
        /// </summary>
        public string FieldIds { get; set; }

    }



}