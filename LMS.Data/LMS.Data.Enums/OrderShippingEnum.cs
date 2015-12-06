using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Enums
{
	
    public partial class OrderShippingEnum
    { 
		      
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


        
    }

	public static class OrderShippingEnumExtensions
	{
		
		public static int GetShippingMethodValue(this OrderShippingEnum.ShippingMethodEnum orderType)
        {
            return OrderShippingEnum.ShippingMethodToValue(orderType);
        }

	}
}
