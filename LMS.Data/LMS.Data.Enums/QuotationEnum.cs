using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Enums
{
	
    public partial class QuotationEnum
    { 
		      
    #region StatusEnum Status

		/// <summary>
        /// Status
        /// </summary>
        public enum StatusEnum : int
        {
			/// <summary>
			/// 启用
			/// </summary>
			Enable = 1,

			/// <summary>
			/// 禁用
			/// </summary>
			Disable = 2,

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
                case 1:
                    result = "Enable";
					break;
                case 2:
                    result = "Disable";
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
                case 1:
                    result = "启用";
					break;
                case 2:
                    result = "禁用";
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
                case "enable":
                    result = 1;
					break;
                case "disable":
                    result = 2;
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
					ValueField = "1",
					TextField = "启用",
					TextField_EN = "Enable"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "禁用",
					TextField_EN = "Disable"
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


        
    }

	public static class QuotationEnumExtensions
	{
		
		public static int GetStatusValue(this QuotationEnum.StatusEnum orderType)
        {
            return QuotationEnum.StatusToValue(orderType);
        }

	}
}
