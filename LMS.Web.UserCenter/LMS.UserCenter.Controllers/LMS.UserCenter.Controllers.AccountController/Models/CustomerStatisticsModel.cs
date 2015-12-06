using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.UserCenter.Controllers.AccountController
{
    public class CustomerStatisticsModel
    {  
        /// <summary>
        /// 充值金额
        /// </summary>
        public decimal Recharge { get; set; }
        /// <summary>
        /// 扣费金额
        /// </summary>
        public decimal TakeOffMoney { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public decimal Balance { get; set; }
        /// <summary>
        /// 未确认订单数量
        /// </summary>
        public int UnconfirmOrder { get; set; }
        /// <summary>
        /// 已确认订单数量
        /// </summary>
        public int ConfirmOrder { get; set; }
        /// <summary>
        /// 已提交订单数量
        /// </summary>
        public int SubmitOrder { get; set; }
        /// <summary>
        /// 已收货订单数量
        /// </summary>
        public int HaveOrder { get; set; }

        /// <summary>
        /// 已发货订单数量
        /// </summary>
        public int SendOrder { get; set; }

        /// <summary>
        /// 已拦截订单数量
        /// </summary>
        public int HoldOrder { get; set; }

        public int TotalOrder { get; set; }
        /// <summary>
        /// Eub所有运单
        /// </summary>
        public int EubWayBillCount { get; set; }

        /// <summary>
        /// 提交中订单数量
        /// </summary>
        public int SubmitingOrder { get; set; }

        /// <summary>
        /// 失败订单数量
        /// </summary>
        public int SubmitFailOrder { get; set; }
    }
}