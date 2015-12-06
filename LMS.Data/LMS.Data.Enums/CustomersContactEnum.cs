using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Enums
{
	
    public partial class CustomersContactEnum
    { 
		      
    #region CustomerTypeEnum CustomerType

		/// <summary>
        /// CustomerType
        /// </summary>
        public enum CustomerTypeEnum : int
        {
			/// <summary>
			/// Sale
			/// </summary>
			Sale = 1,

			/// <summary>
			/// CEO
			/// </summary>
			CEO = 2,

			/// <summary>
			/// CS
			/// </summary>
			CS = 3,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static CustomerTypeEnum ParseToCustomerType(int value)
        {				
		   try{

			 return	(CustomerTypeEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int CustomerTypeToValue(CustomerTypeEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取CustomerType的描述
        /// </summary>
		public static string GetCustomerTypeDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "Sale";
					break;
                case 2:
                    result = "CEO";
					break;
                case 3:
                    result = "CS";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}



				/// <summary>
        /// 获取CustomerType的描述
        /// </summary>
		public static string GetCustomerTypeCNDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "Sale";
					break;
                case 2:
                    result = "CEO";
					break;
                case 3:
                    result = "CS";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}


		/// <summary>
        /// 获取CustomerType的值
        /// </summary>
		public static int GetCustomerType(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "sale":
                    result = 1;
					break;
                case "ceo":
                    result = 2;
					break;
                case "cs":
                    result = 3;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取CustomerType的列表
        /// </summary>
		public static List<DataSourceBinder> GetCustomerTypeList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "Sale",
					TextField_EN = "Sale"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "CEO",
					TextField_EN = "CEO"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "CS",
					TextField_EN = "CS"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidCustomerType(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(CustomerTypeEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


        
    }

	public static class CustomersContactEnumExtensions
	{
		
		public static int GetCustomerTypeValue(this CustomersContactEnum.CustomerTypeEnum orderType)
        {
            return CustomersContactEnum.CustomerTypeToValue(orderType);
        }

	}
}
