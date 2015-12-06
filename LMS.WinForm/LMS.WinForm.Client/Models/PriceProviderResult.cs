using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.WinForm.Client.Models
{
    public class PriceProviderResult
    {
        public PriceProviderResult()
        {
            CanShipping = false;

        }

        public int? VenderId { get; set; }

        public int? CustomerTypeId { get; set; }

        public string ShippingMethodName { get; set; }

        public string ShippingMethodEnName { get; set; }

        public string ShippingMethodDisplayName { get; set; }

        /// <summary>
        /// 运输方式ID
        /// </summary>
        public int? ShippingMethodId { get; set; }

        /// <summary>
        /// 是否支持运输
        /// </summary>
        public bool CanShipping
        {
            get;
            set;
        }


        /// <summary>
        /// 大概运输时间
        /// </summary>
        public string DeliveryTime
        {
            get;
            set;
        }

        /// <summary>
        /// 结算重量
        /// </summary>
        public decimal Weight
        {
            get;
            set;
        }

        /// <summary>
        /// 运费
        /// </summary>

        public decimal ShippingFee
        {
            get;
            set;
        }

        /// <summary>
        /// 偏远地区附加费
        /// </summary>
        public decimal RemoteAreaFee
        {
            get; set; 
        }

        /// <summary>
        /// 挂号费
        /// </summary>
        public decimal RegistrationFee
        {
            get;
            set; 
        }

        /// <summary>
        /// 燃油费
        /// </summary>

        public decimal FuelFee
        {
            get;
            set; 
        }
        #region 新添加 add date 2014-04-23

        /// <summary>
        /// 实际重量
        /// </summary>
        public decimal ActualWeight
        {
            get;
            set;
        }
        /// <summary>
        /// 是否超重
        /// </summary>
        public bool IsOverWeight { get; set; }

        /// <summary>
        /// 是否超最大周围长
        /// </summary>
        public bool IsOverMaxGirth { get; set; }

        /// <summary>
        /// 超重/超长费(整票)
        /// </summary>
        public decimal OverWeightOrLengthFee
        {
            get;
            set;
        }

        /// <summary>
        /// 超周长费用(整票)
        /// </summary>
        public decimal OverGirthFee
        {
            get;
            set;
        }

        /// <summary>
        /// 增值税费(整票)
        /// </summary>

        public decimal AddedTaxFee
        {
            get;
            set;
        }

        /// <summary>
        /// 安全附加费用(整票)
        /// </summary>
        public decimal SecurityAppendFee
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// 其他费用
        /// </summary>
        public decimal OtherFee
        {
            get;
            set;
        }

        /// <summary>
        /// 关税预付服务费
        /// </summary>
        public decimal TariffPrepayFee
        {
            get;
            set;
        }
        /// <summary>
        /// 总费用
        /// </summary>
        public decimal Value
        {
            get;
            set;
        }

        /// <summary>
        /// 信息
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        public string Expression { get; set; }

        public string Remark { get; set; }
    }
}

