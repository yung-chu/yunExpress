using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Services.SequenceNumber
{
    public class PrefixCode
    {
        /// <summary>
        /// 报价单号前缀
        /// </summary>
        public const string QuationID = "GQ";

        /// <summary>
        /// 订单前缀
        /// </summary>
        public const string OrderID = "GO";

        public const string ReworkID = "GR";

        public const string SampleID = "GS";

        /// <summary>
        /// 入仓前缀
        /// </summary>
        public const string InStorageID = "IS";

        /// <summary>
        /// 出仓前缀
        /// </summary>
        public const string OutStorageID = "OS";

        /// <summary>
        /// 跟踪号前缀
        /// </summary>
        public const string TrackNumberID = "TN";

        /// <summary>
        /// 字典类型前缀
        /// </summary>
        public const string DictionaryTypeID = "DT";

        /// <summary>
        /// 退货
        /// </summary>
        public const string ReturnGoodsID = "RE";

        public const string CustomerAmountRecordID = "CA";

        //收货出账单号
        public const string ReceivingBillID = "CC";
        //总包号
        public const string TotalPackageID = "AIR";

        public const string SettlementID = "AC";
    }
}
