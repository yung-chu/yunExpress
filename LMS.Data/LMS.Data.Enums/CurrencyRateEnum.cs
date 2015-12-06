using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Enums
{
	
    public partial class CurrencyRateEnum
    { 
		      
    #region CurrencyRateTypeEnum 

		/// <summary>
        /// 
        /// </summary>
        public enum CurrencyRateTypeEnum : int
        {
			/// <summary>
			/// RMB
			/// </summary>
			RMB = 1,

			/// <summary>
			/// USD
			/// </summary>
			USD = 2,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static CurrencyRateTypeEnum ParseToCurrencyRateType(int value)
        {				
		   try{

			 return	(CurrencyRateTypeEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int CurrencyRateTypeToValue(CurrencyRateTypeEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取的描述
        /// </summary>
		public static string GetCurrencyRateTypeDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "RMB";
					break;
                case 2:
                    result = "USD";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}



				/// <summary>
        /// 获取的描述
        /// </summary>
		public static string GetCurrencyRateTypeCNDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "RMB";
					break;
                case 2:
                    result = "USD";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}


		/// <summary>
        /// 获取的值
        /// </summary>
		public static int GetCurrencyRateType(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "rmb":
                    result = 1;
					break;
                case "usd":
                    result = 2;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取的列表
        /// </summary>
		public static List<DataSourceBinder> GetCurrencyRateTypeList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "RMB",
					TextField_EN = "RMB"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "USD",
					TextField_EN = "USD"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidCurrencyRateType(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(CurrencyRateTypeEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


        
    }

	public static class CurrencyRateEnumExtensions
	{
		
		public static int GetCurrencyRateTypeValue(this CurrencyRateEnum.CurrencyRateTypeEnum orderType)
        {
            return CurrencyRateEnum.CurrencyRateTypeToValue(orderType);
        }

	}
}
