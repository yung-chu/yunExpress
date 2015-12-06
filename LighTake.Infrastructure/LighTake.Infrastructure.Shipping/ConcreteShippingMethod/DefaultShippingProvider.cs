using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shipping.Repository;
using Shipping.Data;
using Shipping.Entity;
using LighTake.Infrastructure.Common;

namespace LighTake.Infrastructure.Shipping
{
    public class DefaultShippingProvider : IShippingProvider
    {
        private readonly ShippingMethodsRepository _shippingMethodsRepository = new ShippingMethodsRepository();
        private readonly ShippingMethodCountriesRepository _shippingMethodCountrysRepository = new ShippingMethodCountriesRepository();
        private readonly ShippingMethod _shippingMethod;

        public int ShippingMethodId { private set; get; }

        public DefaultShippingProvider(int shippingMethodId)
        {
            ShippingMethodId = shippingMethodId;

            _shippingMethod = _shippingMethodsRepository.First(p => p.ShippingMethodId == ShippingMethodId).Copy<ShippingMethod>();
            _shippingMethod.RestrictedCountries = _shippingMethodCountrysRepository.Find(p => p.ShippingMethodId.Equals(ShippingMethodId)).Copy<ShippingMethodCountry>();
        }

        public ShippingMethod ShippingMethod
        {
            get { return _shippingMethod; }
        }

        /// <summary>
        /// 包裹是否可以走这种运输方式
        /// </summary>
        /// <param name="shipmentPackage">The shipment package.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>
        ///   <c>true</c> if the specified shipment package is active; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsActive(ShipmentPackage shipmentPackage, out string errorMessage)
        {
            errorMessage = string.Empty;
            return ShippingMethod.RestrictedCountries.Count(p => p.CountryCode.Equals(shipmentPackage.CountryCode)) > 0;
        }

        /// <summary>
        /// 计算最终的运输费用
        /// </summary>
        /// <param name="shipmentPackage">The shipment package.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <remarks>(重量*单价+挂号费)*折扣</remarks>
        /// <returns></returns>
        public virtual decimal GetFixedRate(ShipmentPackage shipmentPackage, out string errorMessage)
        {
            errorMessage = string.Empty;

            decimal dcmUnitPrice = _shippingMethod.RestrictedCountries.First(p => p.CountryCode == shipmentPackage.CountryCode).UnitPrice;
            decimal dcmWeight = shipmentPackage.TotalWeight;
            decimal dcmFee = ShippingMethod.RegistrationFee;
            decimal dcmDiscount = ShippingMethod.Discount;
            if (dcmDiscount == decimal.Zero)
            {
                dcmDiscount = 1M;
            }

            //以千克计算价钱 
            return (dcmWeight * dcmUnitPrice + dcmFee) * dcmDiscount;
        }

        /// <summary>
        /// Gets the shipping details.
        /// </summary>
        /// <param name="shipmentPackage">The shipment package.</param>
        /// <returns></returns>
        public virtual ShippingDetails GetShippingDetails(ShipmentPackage shipmentPackage)
        {
            string strErrorMsg;
            if (!IsActive(shipmentPackage, out strErrorMsg))
            {
                return new ShippingDetails(strErrorMsg);
            }
            else
            {
                decimal dcmCost = GetFixedRate(shipmentPackage, out strErrorMsg);
                return new ShippingDetails(ShippingMethod, shipmentPackage, dcmCost, true);
            }
        }
    }
}
