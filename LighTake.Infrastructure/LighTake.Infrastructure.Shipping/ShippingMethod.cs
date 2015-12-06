using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Shipping
{
    /// <summary>
    /// 运输方式
    /// </summary>
    [Serializable]
    public class ShippingMethod
    {
        #region Field

        private decimal _discount;

        #endregion

        #region Properties

        /// <summary>
        /// 运输方式ID
        /// </summary>
        public int ShippingMethodId { get; set; }

        /// <summary>
        /// 运输方式的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 运输方式的显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 运输方式跟踪网址
        /// </summary>
        public string TrackingUrl { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 运输方给的折扣价
        /// </summary>
        /// <value>
        /// The discount.
        /// </value>
        public decimal Discount
        {
            get { return _discount; }
            set
            {
                if (value > 1 || value <= 0)
                {
                    throw new ArgumentException("discount");
                }
                _discount = value;
            }
        }


        /// <summary>
        /// 挂号费
        /// </summary>
        /// <value>
        /// The registration fee.
        /// </value>
        public decimal RegistrationFee { get; set; }
       
        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the restricted countries
        /// </summary>
        public virtual List<ShippingMethodCountry> RestrictedCountries { get; set; }

        #endregion
    }


}
