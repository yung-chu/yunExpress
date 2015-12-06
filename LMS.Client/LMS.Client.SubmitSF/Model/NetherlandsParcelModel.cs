using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Client.SubmitSF.Model
{
    public class NetherlandsParcelModel
    {
        /// <summary>
        /// 运单号
        /// </summary>
        public string WayBillNumber { get; set; }
        /// <summary>
        /// 顺丰单号
        /// </summary>
        public string MailNo { get; set; }
        /// <summary>
        /// 原寄递代码
        /// </summary>
        public string OriginCode { get; set; }
        /// <summary>
        /// 目的地代码
        /// </summary>
        public string DestCode { get; set; }
        /// <summary>
        /// 跟踪号
        /// </summary>
        public string AgentMailNo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 状态 1-新建订单，2-确认订单
        /// </summary>
        public int Status { get; set; }
    }
}
