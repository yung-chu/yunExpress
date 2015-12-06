using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Enums
{
	
    public partial class OrderEnum
    { 
		      
    #region OrderTypeEnum OrderType

		/// <summary>
        /// OrderType
        /// </summary>
        public enum OrderTypeEnum : int
        {
			/// <summary>
			/// 团购订单
			/// </summary>
			General = 1,

			/// <summary>
			/// 重寄订单
			/// </summary>
			Redirect = 2,

			/// <summary>
			/// 样品订单
			/// </summary>
			Sample = 3,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static OrderTypeEnum ParseToOrderType(int value)
        {				
		   try{

			 return	(OrderTypeEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int OrderTypeToValue(OrderTypeEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取OrderType的描述
        /// </summary>
		public static string GetOrderTypeDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "团购订单";
					break;
                case 2:
                    result = "重寄订单";
					break;
                case 3:
                    result = "样品订单";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}



				/// <summary>
        /// 获取OrderType的描述
        /// </summary>
		public static string GetOrderTypeCNDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "团购订单";
					break;
                case 2:
                    result = "重寄订单";
					break;
                case 3:
                    result = "样品订单";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}


		/// <summary>
        /// 获取OrderType的值
        /// </summary>
		public static int GetOrderType(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "general":
                    result = 1;
					break;
                case "redirect":
                    result = 2;
					break;
                case "sample":
                    result = 3;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取OrderType的列表
        /// </summary>
		public static List<DataSourceBinder> GetOrderTypeList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "团购订单",
					TextField_EN = "团购订单"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "重寄订单",
					TextField_EN = "重寄订单"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "样品订单",
					TextField_EN = "样品订单"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidOrderType(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(OrderTypeEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


    #region DisplayRowsEnum DisplayRows

		/// <summary>
        /// DisplayRows
        /// </summary>
        public enum DisplayRowsEnum : int
        {
			/// <summary>
			/// Top 1000
			/// </summary>
			r_1000 = 1000,

			/// <summary>
			/// Top 3000
			/// </summary>
			r_3000 = 3000,

			/// <summary>
			/// Top 5000
			/// </summary>
			r_5000 = 5000,

			/// <summary>
			/// Top 10000
			/// </summary>
			r_10000 = 10000,

			/// <summary>
			/// Top  50000
			/// </summary>
			r_50000 = 50000,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static DisplayRowsEnum ParseToDisplayRows(int value)
        {				
		   try{

			 return	(DisplayRowsEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int DisplayRowsToValue(DisplayRowsEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取DisplayRows的描述
        /// </summary>
		public static string GetDisplayRowsDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1000:
                    result = "Top 1000";
					break;
                case 3000:
                    result = "Top 3000";
					break;
                case 5000:
                    result = "Top 5000";
					break;
                case 10000:
                    result = "Top 10000";
					break;
                case 50000:
                    result = "Top 50000";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}



				/// <summary>
        /// 获取DisplayRows的描述
        /// </summary>
		public static string GetDisplayRowsCNDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1000:
                    result = "Top 1000";
					break;
                case 3000:
                    result = "Top 3000";
					break;
                case 5000:
                    result = "Top 5000";
					break;
                case 10000:
                    result = "Top 10000";
					break;
                case 50000:
                    result = "Top  50000";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}


		/// <summary>
        /// 获取DisplayRows的值
        /// </summary>
		public static int GetDisplayRows(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "r_1000":
                    result = 1000;
					break;
                case "r_3000":
                    result = 3000;
					break;
                case "r_5000":
                    result = 5000;
					break;
                case "r_10000":
                    result = 10000;
					break;
                case "r_50000":
                    result = 50000;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取DisplayRows的列表
        /// </summary>
		public static List<DataSourceBinder> GetDisplayRowsList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1000",
					TextField = "Top 1000",
					TextField_EN = "Top 1000"
				},
				new DataSourceBinder{
					ValueField = "3000",
					TextField = "Top 3000",
					TextField_EN = "Top 3000"
				},
				new DataSourceBinder{
					ValueField = "5000",
					TextField = "Top 5000",
					TextField_EN = "Top 5000"
				},
				new DataSourceBinder{
					ValueField = "10000",
					TextField = "Top 10000",
					TextField_EN = "Top 10000"
				},
				new DataSourceBinder{
					ValueField = "50000",
					TextField = "Top  50000",
					TextField_EN = "Top 50000"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidDisplayRows(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(DisplayRowsEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


    #region StatusEnum Status

		/// <summary>
        /// Status
        /// </summary>
        public enum StatusEnum : int
        {
			/// <summary>
			/// 已审核
			/// </summary>
			Processing = 130,

			/// <summary>
			/// 已取消
			/// </summary>
			Canceled = 140,

			/// <summary>
			/// 已导入
			/// </summary>
			Imported = 200,

			/// <summary>
			/// 已打印
			/// </summary>
			Printed = 300,

			/// <summary>
			/// 已捡货
			/// </summary>
			Picked = 400,

			/// <summary>
			/// 已包装
			/// </summary>
			Packaged = 500,

			/// <summary>
			/// 部分发货
			/// </summary>
			PartialShipped = 600,

			/// <summary>
			/// 已交递
			/// </summary>
			HasHandover = 690,

			/// <summary>
			/// 全部发货
			/// </summary>
			FullShipped = 700,

			/// <summary>
			/// 退款取消
			/// </summary>
			RefundCancel = 900,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static StatusEnum ParseToStatus(int value)
        {				
		   try{

			 return	(StatusEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int StatusToValue(StatusEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取Status的描述
        /// </summary>
		public static string GetStatusDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 130:
                    result = "Processing";
					break;
                case 140:
                    result = "Canceled";
					break;
                case 200:
                    result = "Imported";
					break;
                case 300:
                    result = "Printed";
					break;
                case 400:
                    result = "Picked";
					break;
                case 500:
                    result = "Packaged";
					break;
                case 600:
                    result = "Partial Shipped";
					break;
                case 690:
                    result = "HasHandover";
					break;
                case 700:
                    result = "Full Shipped";
					break;
                case 900:
                    result = "Refund Cancel";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}



				/// <summary>
        /// 获取Status的描述
        /// </summary>
		public static string GetStatusCNDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 130:
                    result = "已审核";
					break;
                case 140:
                    result = "已取消";
					break;
                case 200:
                    result = "已导入";
					break;
                case 300:
                    result = "已打印";
					break;
                case 400:
                    result = "已捡货";
					break;
                case 500:
                    result = "已包装";
					break;
                case 600:
                    result = "部分发货";
					break;
                case 690:
                    result = "已交递";
					break;
                case 700:
                    result = "全部发货";
					break;
                case 900:
                    result = "退款取消";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}


		/// <summary>
        /// 获取Status的值
        /// </summary>
		public static int GetStatus(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "processing":
                    result = 130;
					break;
                case "canceled":
                    result = 140;
					break;
                case "imported":
                    result = 200;
					break;
                case "printed":
                    result = 300;
					break;
                case "picked":
                    result = 400;
					break;
                case "packaged":
                    result = 500;
					break;
                case "partialshipped":
                    result = 600;
					break;
                case "hashandover":
                    result = 690;
					break;
                case "fullshipped":
                    result = 700;
					break;
                case "refundcancel":
                    result = 900;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取Status的列表
        /// </summary>
		public static List<DataSourceBinder> GetStatusList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "130",
					TextField = "已审核",
					TextField_EN = "Processing"
				},
				new DataSourceBinder{
					ValueField = "140",
					TextField = "已取消",
					TextField_EN = "Canceled"
				},
				new DataSourceBinder{
					ValueField = "200",
					TextField = "已导入",
					TextField_EN = "Imported"
				},
				new DataSourceBinder{
					ValueField = "300",
					TextField = "已打印",
					TextField_EN = "Printed"
				},
				new DataSourceBinder{
					ValueField = "400",
					TextField = "已捡货",
					TextField_EN = "Picked"
				},
				new DataSourceBinder{
					ValueField = "500",
					TextField = "已包装",
					TextField_EN = "Packaged"
				},
				new DataSourceBinder{
					ValueField = "600",
					TextField = "部分发货",
					TextField_EN = "Partial Shipped"
				},
				new DataSourceBinder{
					ValueField = "690",
					TextField = "已交递",
					TextField_EN = "HasHandover"
				},
				new DataSourceBinder{
					ValueField = "700",
					TextField = "全部发货",
					TextField_EN = "Full Shipped"
				},
				new DataSourceBinder{
					ValueField = "900",
					TextField = "退款取消",
					TextField_EN = "Refund Cancel"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidStatus(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(StatusEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


    #region PaymentStatusEnum PaymentStatus

		/// <summary>
        /// PaymentStatus
        /// </summary>
        public enum PaymentStatusEnum : int
        {
			/// <summary>
			/// 未付款
			/// </summary>
			Unpaid = 1,

			/// <summary>
			/// Pending
			/// </summary>
			Pending = 2,

			/// <summary>
			/// 已付款
			/// </summary>
			Paid = 3,

			/// <summary>
			/// 部分退款
			/// </summary>
			PartiallyRefunded = 4,

			/// <summary>
			/// 全额退款
			/// </summary>
			Refunded = 5,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static PaymentStatusEnum ParseToPaymentStatus(int value)
        {				
		   try{

			 return	(PaymentStatusEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int PaymentStatusToValue(PaymentStatusEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取PaymentStatus的描述
        /// </summary>
		public static string GetPaymentStatusDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "Unpaid";
					break;
                case 2:
                    result = "Pending";
					break;
                case 3:
                    result = "Paid";
					break;
                case 4:
                    result = "PartiallyRefunded";
					break;
                case 5:
                    result = "Refunded";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}



				/// <summary>
        /// 获取PaymentStatus的描述
        /// </summary>
		public static string GetPaymentStatusCNDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "未付款";
					break;
                case 2:
                    result = "Pending";
					break;
                case 3:
                    result = "已付款";
					break;
                case 4:
                    result = "部分退款";
					break;
                case 5:
                    result = "全额退款";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}


		/// <summary>
        /// 获取PaymentStatus的值
        /// </summary>
		public static int GetPaymentStatus(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "unpaid":
                    result = 1;
					break;
                case "pending":
                    result = 2;
					break;
                case "paid":
                    result = 3;
					break;
                case "partiallyrefunded":
                    result = 4;
					break;
                case "refunded":
                    result = 5;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取PaymentStatus的列表
        /// </summary>
		public static List<DataSourceBinder> GetPaymentStatusList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "未付款",
					TextField_EN = "Unpaid"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "Pending",
					TextField_EN = "Pending"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "已付款",
					TextField_EN = "Paid"
				},
				new DataSourceBinder{
					ValueField = "4",
					TextField = "部分退款",
					TextField_EN = "PartiallyRefunded"
				},
				new DataSourceBinder{
					ValueField = "5",
					TextField = "全额退款",
					TextField_EN = "Refunded"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidPaymentStatus(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(PaymentStatusEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


    #region ShippingMethodEnum ShippingMethod

		/// <summary>
        /// ShippingMethod
        /// </summary>
        public enum ShippingMethodEnum : int
        {
			/// <summary>
			/// Airmail
			/// </summary>
			Airmail = 1,

			/// <summary>
			/// EMS
			/// </summary>
			EMS = 2,

			/// <summary>
			/// DHL
			/// </summary>
			DHL = 3,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static ShippingMethodEnum ParseToShippingMethod(int value)
        {				
		   try{

			 return	(ShippingMethodEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int ShippingMethodToValue(ShippingMethodEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取ShippingMethod的描述
        /// </summary>
		public static string GetShippingMethodDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "Airmail";
					break;
                case 2:
                    result = "EMS";
					break;
                case 3:
                    result = "DHL";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}



				/// <summary>
        /// 获取ShippingMethod的描述
        /// </summary>
		public static string GetShippingMethodCNDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "Airmail";
					break;
                case 2:
                    result = "EMS";
					break;
                case 3:
                    result = "DHL";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}


		/// <summary>
        /// 获取ShippingMethod的值
        /// </summary>
		public static int GetShippingMethod(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "airmail":
                    result = 1;
					break;
                case "ems":
                    result = 2;
					break;
                case "dhl":
                    result = 3;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取ShippingMethod的列表
        /// </summary>
		public static List<DataSourceBinder> GetShippingMethodList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "Airmail",
					TextField_EN = "Airmail"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "EMS",
					TextField_EN = "EMS"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "DHL",
					TextField_EN = "DHL"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidShippingMethod(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(ShippingMethodEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


    #region PaymentMethodEnum PaymentMethod

		/// <summary>
        /// PaymentMethod
        /// </summary>
        public enum PaymentMethodEnum : int
        {
			/// <summary>
			/// Paypal
			/// </summary>
			Paypal = 1,

			/// <summary>
			/// CreditCard
			/// </summary>
			CreditCard = 2,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static PaymentMethodEnum ParseToPaymentMethod(int value)
        {				
		   try{

			 return	(PaymentMethodEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int PaymentMethodToValue(PaymentMethodEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取PaymentMethod的描述
        /// </summary>
		public static string GetPaymentMethodDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "Paypal";
					break;
                case 2:
                    result = "CreditCard";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}



				/// <summary>
        /// 获取PaymentMethod的描述
        /// </summary>
		public static string GetPaymentMethodCNDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "Paypal";
					break;
                case 2:
                    result = "CreditCard";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}


		/// <summary>
        /// 获取PaymentMethod的值
        /// </summary>
		public static int GetPaymentMethod(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "paypal":
                    result = 1;
					break;
                case "creditcard":
                    result = 2;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取PaymentMethod的列表
        /// </summary>
		public static List<DataSourceBinder> GetPaymentMethodList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "Paypal",
					TextField_EN = "Paypal"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "CreditCard",
					TextField_EN = "CreditCard"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidPaymentMethod(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(PaymentMethodEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


        
    }

	public static class OrderEnumExtensions
	{
		
		public static int GetOrderTypeValue(this OrderEnum.OrderTypeEnum orderType)
        {
            return OrderEnum.OrderTypeToValue(orderType);
        }
public static int GetDisplayRowsValue(this OrderEnum.DisplayRowsEnum orderType)
        {
            return OrderEnum.DisplayRowsToValue(orderType);
        }
public static int GetStatusValue(this OrderEnum.StatusEnum orderType)
        {
            return OrderEnum.StatusToValue(orderType);
        }
public static int GetPaymentStatusValue(this OrderEnum.PaymentStatusEnum orderType)
        {
            return OrderEnum.PaymentStatusToValue(orderType);
        }
public static int GetShippingMethodValue(this OrderEnum.ShippingMethodEnum orderType)
        {
            return OrderEnum.ShippingMethodToValue(orderType);
        }
public static int GetPaymentMethodValue(this OrderEnum.PaymentMethodEnum orderType)
        {
            return OrderEnum.PaymentMethodToValue(orderType);
        }

	}
}
