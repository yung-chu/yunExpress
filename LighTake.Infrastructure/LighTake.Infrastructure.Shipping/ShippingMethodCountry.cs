using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Shipping
{
    public class ShippingMethodCountry
    {
        /// <summary>
        /// Gets or sets the shipping method id.
        /// </summary>
        /// <value>
        /// The shipping method id.
        /// </value>
        public int ShippingMethodId { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        /// <value>
        /// The country code.
        /// </value>
        public string CountryCode { get; set; }

        /// <summary>
        /// 运输到不同国家的单价
        /// </summary>
        /// <value>
        /// The unit price.
        /// </value>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 预计到货时间区间开始（天）
        /// </summary>
        public decimal ShippingTimeFrom { get; set; }

        /// <summary>
        /// 预计到货时间区间结束（天）
        /// </summary>
        public decimal ShippingTimeTo { get; set; }
    }
}
