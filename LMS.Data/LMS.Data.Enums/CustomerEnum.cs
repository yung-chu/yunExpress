using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Enums
{
	
    public partial class CustomerEnum
    { 
		      
    #region CustomerTypeEnum CustomerType

		/// <summary>
        /// CustomerType
        /// </summary>
        public enum CustomerTypeEnum : int
        {
			/// <summary>
			/// 普通客户
			/// </summary>
			Ordinary = 1,

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
                    result = "普通客户";
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
                    result = "普通客户";
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
                case "ordinary":
                    result = 1;
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
					TextField = "普通客户",
					TextField_EN = "普通客户"
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


    #region StatusEnum Status

		/// <summary>
        /// Status
        /// </summary>
        public enum StatusEnum : int
        {
			/// <summary>
			/// 启用
			/// </summary>
			On = 1,

			/// <summary>
			/// 禁用
			/// </summary>
			Off = 2,

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
                case "on":
                    result = 1;
					break;
                case "off":
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
					TextField_EN = "启用"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "禁用",
					TextField_EN = "禁用"
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

	public static class CustomerEnumExtensions
	{
		
		public static int GetCustomerTypeValue(this CustomerEnum.CustomerTypeEnum orderType)
        {
            return CustomerEnum.CustomerTypeToValue(orderType);
        }
public static int GetStatusValue(this CustomerEnum.StatusEnum orderType)
        {
            return CustomerEnum.StatusToValue(orderType);
        }

	}
}
