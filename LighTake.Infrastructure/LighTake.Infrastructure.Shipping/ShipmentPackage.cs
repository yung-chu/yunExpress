using System;
using System.Collections.Generic;
//using System.Linq;
using System.Linq;
using System.Text;

using LighTake.Infrastructure.Shopping;

namespace LighTake.Infrastructure.Shipping
{
    [Serializable]
    public class ShipmentPackage
    {
        #region Methods

        /// <summary>
        /// 净重：Gets total weight(kg)
        /// </summary>
        /// <returns>Total weight</returns>
        public decimal TotalWeight
        {
            get;
            private set;
        }

        /// <summary>
        /// 毛重：Gets total gross weight(kg)
        /// </summary>
        /// <returns>Total weight</returns>
        public decimal TotalGrossWeight
        {
            get
            {
                return (TotalWeight * 1.07m);
            }
        }

        /// <summary>
        /// 总宽度：Gets total width（cm）
        /// </summary>
        /// <returns>Total width</returns>
        public decimal TotalWidth
        {
            get;
            private set;
        }

        /// <summary>
        /// 总长度：Gets total length（cm）
        /// </summary>
        /// <returns>Total length</returns>
        public decimal TotalLength
        {
            get;
            private set;
        }

        /// <summary>
        /// 总高度：Gets total height（cm）
        /// </summary>
        /// <returns>Total height</returns>
        public decimal TotalHeight
        {
            get;
            private set;
        }

        /// <summary>
        /// 体积重：Gets total volume（kg）
        /// </summary>
        public decimal TotalVolume
        {
            get;
            private set;
        }

        /// <summary>
        /// 商品的总销售金额
        /// </summary>
        public decimal TotalAmount
        {
            get;
            private set;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a shopping cart items
        /// </summary>
        public List<ShoppingCartItem> ShoppingCartItems
        {
            set
            {
                TotalWeight = value.Sum(shoppingCartItem => shoppingCartItem.Weight * shoppingCartItem.Quantity) / 1000;
                TotalLength = value.Sum(shoppingCartItem => shoppingCartItem.Length * shoppingCartItem.Quantity);
                TotalWidth = value.Sum(shoppingCartItem => shoppingCartItem.Width * shoppingCartItem.Quantity);
                TotalHeight = value.Sum(shoppingCartItem => shoppingCartItem.Height * shoppingCartItem.Quantity);
                TotalVolume = value.Sum(p => p.Length * p.Width * p.Height * p.Quantity) / 5000;
                TotalAmount = value.Sum(p => p.SalesPrice * p.Quantity);
            }
        }

        public string CountryCode { get; set; }

        private bool _isSeparatePackage;
        /// <summary>
        /// 是否分包计算运费
        /// </summary>
        public bool IsSeparatePackage
        {
            get { return _isSeparatePackage; }
            set { _isSeparatePackage = value; }
        }

        #endregion
    }
}
