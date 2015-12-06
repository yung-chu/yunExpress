using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Enums
{
	
    public partial class OrderPaymentEnum
    { 
		      
    #region OrderPaymentMethodEnum PaymentMethod

		/// <summary>
        /// PaymentMethod
        /// </summary>
        public enum OrderPaymentMethodEnum : int
        {
			/// <summary>
			/// Paypal
			/// </summary>
			Paypal = 1,

			/// <summary>
			/// Paypal IPN
			/// </summary>
			PaypalIPN  = 2,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static OrderPaymentMethodEnum ParseToOrderPaymentMethod(int value)
        {				
		   try{

			 return	(OrderPaymentMethodEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int OrderPaymentMethodToValue(OrderPaymentMethodEnum enumOption)
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
		public static string GetOrderPaymentMethodDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "Paypal";
					break;
                case 2:
                    result = "Paypal IPN";
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
		public static string GetOrderPaymentMethodCNDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "Paypal";
					break;
                case 2:
                    result = "Paypal IPN";
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
		public static int GetOrderPaymentMethod(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "paypal":
                    result = 1;
					break;
                case "paypalipn ":
                    result = 2;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取PaymentMethod的列表
        /// </summary>
		public static List<DataSourceBinder> GetOrderPaymentMethodList()
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
					TextField = "Paypal IPN",
					TextField_EN = "Paypal IPN"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidOrderPaymentMethod(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(OrderPaymentMethodEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


        
    }

	public static class OrderPaymentEnumExtensions
	{
		
		public static int GetOrderPaymentMethodValue(this OrderPaymentEnum.OrderPaymentMethodEnum orderType)
        {
            return OrderPaymentEnum.OrderPaymentMethodToValue(orderType);
        }

	}
}
