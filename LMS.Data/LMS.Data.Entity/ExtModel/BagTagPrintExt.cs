using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity.ExtModel
{
    /// <summary>
    /// 袋牌打印模型
    /// </summary>
    public class BagTagPrintExt
    {
        /// <summary>
        /// 件数
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 总重量
        /// </summary>
        public decimal TotalWeight { get; set; }

        ///// <summary>
        ///// 流水号
        ///// </summary>
        //public string SerialNumber
        //{
        //    get
        //    {
        //        return BagTagNumber.Substring(BagTagNumber.Length - 4, 4);
        //    }
        //}

        /// <summary>
        /// 袋牌号
        /// </summary>
        public string BagTagNumber { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// 是否带电
        /// </summary>
        public bool HasBattery { get; set; }
    }

    public class ResultInfo
    {
        public bool Status { get; set; }
        public string Message { get; set; }
    }
}
