using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Shipping
{
    /// <summary>
    /// 一个包裹运输的详细信息
    /// </summary>
    public class ShippingDetails
    {
        public ShippingDetails(string reason)
            : this(null, null, 0M, false, reason)
        { }

        public ShippingDetails(ShippingMethod method, ShipmentPackage package, decimal cost = 0M, bool isSuccess = false, string reason = "None")
        {
            if (method == null || package == null)
            {
                Weight = 0M;
                MethodId = 0;
                Method = string.Empty;
                Discount = 0M;
                Cost = cost;
                IsSuccess = isSuccess;
                Reason = reason;
            }
            else
            {
                ShippingMethodCountry country = method.RestrictedCountries.First(p => p.CountryCode == package.CountryCode);

                Weight = package.TotalWeight;
                MethodId = method.ShippingMethodId;
                Method = method.DisplayName;
                Discount = method.Discount;
                Cost = cost;
                IsSuccess = isSuccess;
                Reason = reason;
                ShippingTime = country.ShippingTimeFrom + "~" + country.ShippingTimeTo + " Working days";
            }
        }

        /// <summary>
        /// 运输单 物品的总重量
        /// </summary>
        public decimal Weight { get; internal set; }

        /// <summary>
        /// 运输方式的Id
        /// </summary>
        public int MethodId { get; internal set; }

        /// <summary>
        /// 运输方式
        /// </summary>
        public string Method { get; internal set; }

        /// <summary>
        /// 运输商 给的折扣
        /// </summary>
        public decimal Discount { get; internal set; }

        /// <summary>
        /// Gets the shipping time.
        /// </summary>
        public string ShippingTime { get; internal set; }

        /// <summary>
        /// 一个运输包裹的根据折扣价算出的总费用
        /// </summary>
        /// <value>
        /// The cost.
        /// </value>
        public decimal Cost { get; internal set; }

        /// <summary>
        /// 运单是否能运输
        /// </summary>
        /// <value>
        /// 	<c>true</c> 运单可以走 <c>false</c>.
        /// </value>
        public bool IsSuccess { get; internal set; }


        /// <summary>
        /// 原因
        /// </summary>
        /// <value>
        /// The reason.
        /// </value>
        public string Reason { get; internal set; }

    }
}
