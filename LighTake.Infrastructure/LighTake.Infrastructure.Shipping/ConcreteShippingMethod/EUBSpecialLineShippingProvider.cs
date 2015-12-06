using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Shipping.ConcreteShippingMethod
{
    public class EUBSpecialLineShippingProvider : DefaultShippingProvider
    {
        private static readonly decimal S_MIN_RATE = 7.8M;

        public EUBSpecialLineShippingProvider()
            : base(4)
        {}

        public override decimal GetFixedRate(ShipmentPackage shipmentPackage, out string errorMessage)
        {
            decimal dcmRate = base.GetFixedRate(shipmentPackage, out errorMessage);

            return dcmRate < S_MIN_RATE ? S_MIN_RATE : dcmRate;
        }
    }
}
