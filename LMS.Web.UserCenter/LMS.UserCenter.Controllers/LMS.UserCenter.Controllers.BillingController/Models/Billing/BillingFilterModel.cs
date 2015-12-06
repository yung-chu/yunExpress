using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LighTake.Infrastructure.Seedwork;

namespace LMS.UserCenter.Controllers.BillingController.Models
{
    public class BillingFilterModel : SearchParam
    {

        /// <summary>
        /// 客户编码
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 时间开始
        /// </summary>
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// 时间结束
        /// </summary>
        public DateTime? EndDateTime { get; set; }

    }
}