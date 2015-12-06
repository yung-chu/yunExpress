using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.WinForm.Client.Models
{
    public class InStorageRequestFormModel
    {
        public int CustomerType { get; set; }
        public string CustomerCode { get; set; }
        public int GoodsTypeID { get; set; }
        public int ShippingMethodId { get; set; }
        public string WayBillNumber { get; set; }
        public string TrackingNumber { get; set; }
        public decimal Weight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public bool ChkPrint { get; set; }
        public string PrintTemplateName { get; set; }
        public string OperatorUserName { get; set; }
        /// <summary>
        /// 是否为商业快递
        /// </summary>
        public bool IsBusinessExpress { get; set; }
    }
}
