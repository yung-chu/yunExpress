using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Shopping
{
    /// <summary>
    /// 购物车中的项
    /// </summary>
    [Serializable]
    public class ShoppingCartItem : ICloneable
    {
        #region Ctor
        public ShoppingCartItem() { }

        public ShoppingCartItem(string sku, int quantity)
        {
            this.SKU = sku;
            Quantity = quantity;
        }

        public ShoppingCartItem(string sku, string productID, string productName, int quantity, string productImgUrl, decimal price)
        {
            SKU = sku;
            ProductID = productID;
            ProductName = productName;
            ImgUrl = productImgUrl;
            Quantity = quantity;
            UnitPrice = price;
        }


        public ShoppingCartItem(string sku, string productID, string productName, int quantity, string productImgUrl, decimal price, decimal discount)
        {
            SKU = sku;
            ProductID = productID;
            ProductName = productName;
            ImgUrl = productImgUrl;
            Quantity = quantity;
            UnitPrice = price;
            Discount = discount;
        }

        #endregion

        #region Property

        /// <summary>
        /// 货品编号(唯一)
        /// </summary>
        public string SKU
        {
            get;
            set;
        }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID
        {
            get;
            set;
        }
        /// <summary>
        /// 商品分类ID
        /// </summary>
        public string ProductCategoryID
        {
            get;
            set;
        }
        /// <summary>
        /// 商品名称        
        /// </summary>
        public string ProductName
        {
            get;
            set;
        }
        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImgUrl
        {
            get;
            set;
        }
        /// <summary>
        ///  数量
        /// </summary>
        public int Quantity
        {
            get;
            set;
        }
        /// <summary>
        ///  冻结的数量
        /// </summary>
        public int QuantityFreeze
        {
            get;
            set;
        }
        /// <summary>
        /// 原单价
        /// </summary>
        public decimal UnitPrice
        {
            get;
            set;
        }

        private decimal _salesPrice = decimal.Zero;
        /// <summary>
        /// 销售单价（美元）
        /// </summary>
        public decimal SalesPrice
        {
            get
            {
                if (_salesPrice <= 0)
                    return UnitPrice;
                return _salesPrice;
            }
            set
            {
                _salesPrice = value;
            }

        }

        /// <summary>
        /// 指导价格
        /// </summary>
        public decimal GuidePrice
        {
            get;
            set;
        }
        /// <summary>
        /// 市场单价（美元）
        /// </summary>
        public decimal MarketPrice
        {
            get;
            set;
        }
        /// <summary>
        /// 扣折
        /// </summary>
        public decimal Discount
        {
            get;
            set;
        }
        /// <summary>
        /// 小计
        /// </summary>
        public decimal Subtotal
        {
            get { return UnitPrice * Quantity; }
        }


        /// <summary>
        /// 折扣后的小计
        /// </summary>
        public decimal LineTotal
        {
            get;
            set;

        }
        /// <summary>
        /// 单元重量（单位g）
        /// </summary>
        public decimal Weight
        {
            get;
            set;

        }

        /// <summary>
        /// 长度（单位cm）
        /// </summary>
        public decimal Length
        {
            get;
            set;

        }

        /// <summary>
        /// 宽度（单位cm）
        /// </summary>
        public decimal Width
        {
            get;
            set;

        }

        /// <summary>
        /// 高度（单位cm）
        /// </summary>
        public decimal Height
        {
            get;
            set;

        }

        /// <summary>
        /// 体积
        /// </summary>
        public decimal Volume
        {
            get;
            set;
        }

        /// <summary>
        /// 总体积
        /// </summary>
        public decimal TotalVolume
        {
            get { return Volume * Quantity; }
        }

        /// <summary>
        /// 重量因子 购物车及算运费时要比实际重量乘以一个因子，如：1.07
        /// </summary>
        public decimal Factor
        {
            get;
            set;
        }
        /// <summary>
        /// 总重量 
        /// </summary>
        public decimal TotalWeight
        {
            get { return Volume * Quantity * Factor; }
        }
        /// <summary>
        /// 批量销售的状态
        /// </summary>
        public int WholeStatus
        {
            get;
            set;
        }
        /// <summary>
        /// 是否促销产品
        /// </summary>
        public bool IsPromotion
        {
            get;
            set;
        }


        #endregion

        #region ICloneable 成员

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion

    }
}
