using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Client.SubmitSF.Model
{
    public class SebModel
    {
        public SebModel()
        {
            ApplicationInfo = new List<ApplicationInfoSfModel>();
        }
        /// <summary>
        /// 运单号
        /// </summary>
        public string WayBillNumber { get; set; }
        /// <summary>
        /// 收件人公司
        /// </summary>
        public string ShippingCompany { get; set; }
        /// <summary>
        /// 收件人名字
        /// </summary>
        public string ShippingName { get; set; }
        /// <summary>
        /// 收件人电话
        /// </summary>
        public string ShippingPhone { get; set; }
        /// <summary>
        /// 收件人地址
        /// </summary>
        public string ShippingAddress { get; set; }
        /// <summary>
        /// 申请包裹数量
        /// </summary>
        public int PackageNumber { get; set; }
        /// <summary>
        /// 收件人国家代码
        /// </summary>
        public string CountryCode { get; set; }
        /// <summary>
        /// 收件人邮编
        /// </summary>
        public string ShippingZip { get; set; }
        /// <summary>
        /// 收件人州省
        /// </summary>
        public string ShippingState { get; set; }
        /// <summary>
        /// 收件人城市
        /// </summary>
        public string ShippingCity { get; set; }
        /// <summary>
        /// 运输方式ID
        /// </summary>
        public int ShippingMethodId { get; set; }
        /// <summary>
        /// 申报信息
        /// </summary>
        public List<ApplicationInfoSfModel> ApplicationInfo { get; set; }
    }
}
