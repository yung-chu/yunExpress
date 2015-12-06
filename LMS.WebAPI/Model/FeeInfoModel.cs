using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Xml.Serialization;

namespace LMS.WebAPI
{
    [Serializable]
    public class FeeInfoModel
    {
        /// <summary>
        /// 客户订单号   ---  包裹号
        /// </summary>
        [XmlElement("CustomerOrderNumber")]
        public string CustomerOrderNumber { get; set; }

        /// <summary>
        /// 跟踪单号
        /// </summary>
        [XmlElement("TrackingNumber")]
        public string TrackingNumber { get; set; }

        /// <summary>
        /// 计费重量
        /// </summary>
        [XmlElement("SettleWeight")]
        public decimal SettleWeight { get; set; }

        /// <summary>
        /// 总运费
        /// </summary>
        [XmlElement("TotalFee")]
        public decimal TotalFee { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [XmlElement("ErrorMsg")]
        public string ErrorMsg { get; set; }

    }
}