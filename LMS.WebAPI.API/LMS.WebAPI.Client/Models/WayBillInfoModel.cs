using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace LMS.WebAPI.Client.Models
{
    public class WayBillInfoModel
    {
        public string WayBillNumber { get; set; }
        public string TrackingNumber { get; set; }
        public string OrderNumber { get; set; }
        public string ShippingMethodCode { get; set; }
        //是否关税预付
        public bool? EnableTariffPrepay { get; set; }
        public int Status { get; set; }
        /// <summary>
        /// 包裹称重重量
        /// </summary>
        public decimal? Weight { get; set; }

        /// <summary>
        /// 包裹结算重量
        /// </summary>
        public decimal? SettleWeight { get; set; }
        public int PackageNumber { get; set; }


        public decimal? Length { get; set; }

        public decimal? Width { get; set; }

        public decimal? Height { get; set; }
        [XmlElement("ShippingInfo")]
        public ShippingInfoModel ShippingInfo { get; set; }

        [XmlElement("SenderInfo")]
        public SenderInfoModel SenderInfo { get; set; }
        public bool IsReturn { get; set; }

        /// <summary>
        ///  申报类型
        /// </summary>
        public int ApplicationType { get; set; }

        public int InsuranceType { get; set; }
        public decimal? InsureAmount { get; set; }
        public int? SensitiveTypeID { get; set; }
        private List<ApplicationInfoModel> _applicationInfos;

        [XmlArrayItem("ApplicationInfo")]
        public List<ApplicationInfoModel> ApplicationInfos
        {
            get { return _applicationInfos ?? (_applicationInfos = new List<ApplicationInfoModel>()); }
            set { _applicationInfos = value; }
        }
    }
}