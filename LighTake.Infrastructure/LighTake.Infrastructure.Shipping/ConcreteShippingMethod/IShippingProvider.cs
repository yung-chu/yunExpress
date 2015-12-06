using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Shipping
{
    public partial interface IShippingProvider
    {
        int ShippingMethodId
        {
            get;
        }

        ShippingMethod ShippingMethod { get; }

        /// <summary>
        /// 判断运输方式是否可用
        /// </summary>
        /// <returns></returns>
        bool IsActive(ShipmentPackage shipmentPackage, out string errorMessage);

        /// <summary>
        /// 获取运费
        /// </summary>
        /// <param name="shipmentPackage"> </param>
        /// <param name="errorMessage"> </param>
        /// <returns></returns>
        decimal GetFixedRate(ShipmentPackage shipmentPackage, out string errorMessage);

        /// <summary>
        /// Gets the shipping details.
        /// </summary>
        /// <param name="shipmentPackage">The shipment package.</param>
        /// <returns></returns>
        ShippingDetails GetShippingDetails(ShipmentPackage shipmentPackage);
    }
}
