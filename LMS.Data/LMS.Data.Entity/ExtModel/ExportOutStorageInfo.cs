using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    public class ExportOutStorageInfo
    {
        public ExportOutStorageInfo()
        {
            ApplicationInfos = new List<ApplicationInfo>();
        }

        //运单信息表
        public string CustomerOrderNumber { get; set; }

        public string WayBillNumber { get; set; }
        /// <summary>
        /// 入仓运输方式
        /// </summary>
        public string InShippingMethodName { get; set; }
        /// <summary>
        /// 出仓运输方式
        /// </summary>
        public string OutShippingMethodName { get; set; }

        /// <summary>
        /// 物流商
        /// </summary>
        public string VenderName { get; set; }

        public string TrackingNumber { get; set; }

        /// <summary>
        /// 国家名称（英文）
        /// </summary>
        public string CountryName { get; set; }
        /// <summary>
        /// 国家名称（中文）
        /// </summary>
        public string CountryChineseName { get; set; }
       
        
        /// <summary>
        /// 收件人
        /// </summary>
        public string ShippingFirstName { get; set; }
        public string ShippingLastName { get; set; }
        public string ShippingCompany { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZip { get; set; }
        public string ShippingPhone { get; set; }
        public string ShippingTaxId { get; set; }

        /// <summary>
        /// 收件人
        /// </summary>
        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderCompany { get; set; }
        public string SenderAddress { get; set; }
        public string SenderCity { get; set; }
        public string SenderState { get; set; }
        public string SenderZip { get; set; }
        public string SenderPhone { get; set; }

        public bool IsReturn { get; set; }
        public string InsuredName { get; set; }
        public Nullable<decimal> InsureAmount { get; set; }
        public string SensitiveTypeName { get; set; }
        public string AppLicationTypeValue { get; set; }
        public Nullable<int> PackageNumber { get; set; }
        
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Height { get; set; }
        public decimal Weight { get; set; }
        public Nullable<decimal> SettleWeight { get; set; }
        //申报信息表
        public List<ApplicationInfo> ApplicationInfos { get; set; }


        public int AppLicationType { get; set; }
    }
}
