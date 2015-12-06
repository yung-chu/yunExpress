using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Shopping
{
    /// <summary>
    /// 购物车类
    /// </summary>

    public class ShoppingCart
    {
        #region Ctor

        public ShoppingCart()
        {
            CartItems = new List<ShoppingCartItem>();
        }

        #endregion

        #region Properties
        /// <summary>
        /// 购物车明细
        /// </summary>
        public List<ShoppingCartItem> CartItems { get; set; }

        /// <summary>
        /// 锁定对象
        /// </summary>
        private readonly object _sync = new object();

        /// <summary>
        /// 购物车小计
        /// </summary>
        public decimal SubTotal
        {
            get
            {
                if (CartItems == null || CartItems.Count < 1)
                    return 0;
                return CartItems.Sum(I => I.LineTotal) - DiscountAmount;
            }
        }

        /// <summary>
        /// 购物车总计
        /// </summary>
        public decimal GrandTotal
        {
            get
            {
                if (CartItems == null || CartItems.Count < 1)
                    return 0;
                return SubTotal + ShippingFee - CouponAmount;
            }
        }

        /// <summary>
        /// 购物车商品的项数
        /// </summary>
        public int Count
        {
            get
            {
                if (CartItems == null || CartItems.Count < 1)
                    return 0;
                return CartItems.Count;
            }
        }

        /// <summary>
        /// 购物车商品的总数
        /// </summary>
        public int TotalQuantity
        {
            get
            {
                if (CartItems == null || CartItems.Count < 1)
                    return 0;
                return CartItems.Sum(I => I.Quantity);
            }
        }

        /// <summary>
        /// 折扣券编号
        /// </summary>
        public string CouponCode
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
        /// 总折扣金额
        /// </summary>
        public decimal DiscountTotal
        {
            get
            {
                if (CartItems == null || CartItems.Count < 1)
                    return 0;
                return DiscountAmount + CouponAmount;
            }
        }
        /// <summary>
        /// 折扣金额
        /// </summary>
        public decimal DiscountAmount
        {
            get;
            set;
        }
        /// <summary>
        /// 折扣卷折扣金额
        /// </summary>
        public decimal CouponAmount
        {
            get;
            set;
        }
        /// <summary>
        /// 折扣信息
        /// </summary>
        public string DiscountInfo
        {
            get;
            set;
        }
        /// <summary>
        /// 购物车中的商品总重量
        /// </summary>
        public decimal TotalWeight
        {
            get
            {
                if (CartItems == null || CartItems.Count < 1)
                    return 0;
                return CartItems.Sum(I => (I.Weight * I.Quantity));
            }
        }
        /// <summary>
        /// 购物车中的商品总体积
        /// </summary>
        public decimal TotalVolume
        {
            get
            {
                if (CartItems == null || CartItems.Count < 1)
                    return 0;
                return CartItems.Sum(I => I.Volume);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 加入一项到购物车
        /// </summary>
        /// <param name="cartItem"></param>
        public void Add(ShoppingCartItem cartItem)
        {
            lock (_sync)
            {
                //Add options logic here
                ShoppingCartItem existsItem = CartItems.Find(p => p.SKU == cartItem.SKU);

                if (existsItem != null)
                {
                    existsItem.Quantity += cartItem.Quantity;
                }
                else
                {
                    CartItems.Add(cartItem);
                }
            }

        }

        /// <summary>
        /// 更新购物车中的一项
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="quantity"></param>
        public void Update(string sku, int quantity)
        {
            lock (_sync)
            {
                ShoppingCartItem existsItem = CartItems.Find(p => p.SKU == sku);
                if (existsItem != null)
                {
                    existsItem.Quantity = quantity;
                }
                else
                {
                    CartItems.Add(new ShoppingCartItem
                    {
                        SKU = sku,
                        Quantity = quantity

                    });
                }
            }
        }

        /// <summary>
        /// 加入或更新购物车中的一项
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="quantity"></param>
        public void AddorUpdate(string sku, int quantity)
        {
            lock (_sync)
            {
                ShoppingCartItem existsItem = CartItems.Find(p => p.SKU == sku);
                if (existsItem != null)
                {
                    existsItem.Quantity += quantity;
                }
                else
                {
                    CartItems.Add(new ShoppingCartItem
                    {
                        SKU = sku,
                        Quantity = quantity

                    });
                }
            }
        }

        /// <summary>
        /// 添加多个商品到购物车中
        /// </summary>
        /// <param name="items">商品信息</param>
        public void AddItems(List<ShoppingCartItem> items)
        {
            lock (_sync)
            {
                CartItems.AddRange(items);
            }
        }

        /// <summary>
        /// 从购物车中移除一个商品
        /// </summary>
        /// <param name="sku">货品编号（唯一） </param>
        /// <returns>返回移除</returns>
        public void Remove(string sku)
        {
            lock (_sync)
            {
                if (CartItems != null) CartItems.RemoveAll(p => sku != null && p.SKU == sku);
            }
        }

        /// <summary>
        /// 判断指定的SKU商品是否存在于购物车中
        /// </summary>
        /// <param name="sku">货品编号（唯一）</param>
        /// <returns>TRUE:存在 FALSE:不存在</returns>
        public bool IsExistsItem(string sku)
        {
            return CartItems.Exists(p => p.SKU == sku);
        }

        /// <summary>
        /// 从购物车中获取指定的商品
        /// </summary>
        /// <param name="sku">货品编号（唯一）</param>
        /// <returns>返回购物车中指定SKU的商品</returns>
        public ShoppingCartItem FindItem(string sku)
        {
            return CartItems.Find(p => p.SKU == sku);
        }

        /// <summary>
        /// 清空购物车
        /// </summary>
        public void ClearAll()
        {
            CartItems.Clear();
            ShippingFee = 0;
            DiscountAmount = 0;
            CouponAmount = 0;
        }
        #endregion

        #region Services

        /// <summary>
        /// 从字符串中反序列化出购物车对象
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ShoppingCart Deserialize(string str)
        {

            var instance = new ShoppingCart();

            if (string.IsNullOrWhiteSpace(str))
                return instance;

            foreach (var item in str.Split(','))
            {
                var pair = item.Split('*');

                if (pair.Length != 2)
                    throw new FormatException();

                var id = pair[0];

                int quantity;

                if (!int.TryParse(pair[1], out quantity))
                    throw new FormatException();

                instance.AddorUpdate(id, quantity);
            }

            return instance;
        }

        /// <summary>
        /// 购物车序列化为字符串
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        public static string Serialize(ShoppingCart cart)
        {
            return string.Join(",", cart.CartItems.Select(pair => pair.SKU + "*" + pair.Quantity));
        }
        #endregion
    }
}
