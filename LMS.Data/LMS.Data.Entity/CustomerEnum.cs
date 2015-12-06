using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Entity
{
	
    public partial class Customer
    {       
 

    #region StatusEnum 状态

		/// <summary>
        /// 状态
        /// </summary>
        public enum StatusEnum : int
        {
			/// <summary>
			/// 未审核
			/// </summary>
			Unaudited = 1,

			/// <summary>
			/// 已审核
			/// </summary>
			Enable = 2,

			/// <summary>
			/// 禁用
			/// </summary>
			Disable = 3,

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
        /// 获取状态的描述
        /// </summary>
		public static string GetStatusDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "未审核";
					break;
                case 2:
                    result = "已审核";
					break;
                case 3:
                    result = "禁用";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取状态的值
        /// </summary>
		public static int GetStatus(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "unaudited":
                    result = 1;
					break;
                case "enable":
                    result = 2;
					break;
                case "disable":
                    result = 3;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取状态的列表
        /// </summary>
		public static List<DataSourceBinder> GetStatusList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "未审核",
					TextField_EN = "未审核"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "已审核",
					TextField_EN = "已审核"
				},
				new DataSourceBinder{
					ValueField = "3",
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


 

    #region NumberTypeEnum 单号类型

		/// <summary>
        /// 单号类型
        /// </summary>
        public enum NumberTypeEnum : int
        {
			/// <summary>
			/// 交易号
			/// </summary>
			TransactionNo = 1,

			/// <summary>
			/// 运单号
			/// </summary>
			WayBill = 2,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static NumberTypeEnum ParseToNumberType(int value)
        {				
		   try{

			 return	(NumberTypeEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int NumberTypeToValue(NumberTypeEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取单号类型的描述
        /// </summary>
		public static string GetNumberTypeDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "交易号";
					break;
                case 2:
                    result = "运单号";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取单号类型的值
        /// </summary>
		public static int GetNumberType(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "transactionno":
                    result = 1;
					break;
                case "waybill":
                    result = 2;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取单号类型的列表
        /// </summary>
		public static List<DataSourceBinder> GetNumberTypeList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "交易号",
					TextField_EN = "交易号"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "运单号",
					TextField_EN = "运单号"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidNumberType(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(NumberTypeEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


 

    #region CustomerSourceTypeEnum 客户来源类型

		/// <summary>
        /// 客户来源类型
        /// </summary>
        public enum CustomerSourceTypeEnum : int
        {
			/// <summary>
			/// 普通客户
			/// </summary>
			Common = 1,

			/// <summary>
			/// 通途平台客户
			/// </summary>
			TongTu = 2,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static CustomerSourceTypeEnum ParseToCustomerSourceType(int value)
        {				
		   try{

			 return	(CustomerSourceTypeEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int CustomerSourceTypeToValue(CustomerSourceTypeEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取客户来源类型的描述
        /// </summary>
		public static string GetCustomerSourceTypeDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "普通客户";
					break;
                case 2:
                    result = "通途平台客户";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取客户来源类型的值
        /// </summary>
		public static int GetCustomerSourceType(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "common":
                    result = 1;
					break;
                case "tongtu":
                    result = 2;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取客户来源类型的列表
        /// </summary>
		public static List<DataSourceBinder> GetCustomerSourceTypeList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "普通客户",
					TextField_EN = "普通客户"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "通途平台客户",
					TextField_EN = "通途平台客户"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidCustomerSourceType(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(CustomerSourceTypeEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion




        
    }

	public static class CustomerExtensions
	{
		
		public static int GetStatusValue(this Customer.StatusEnum orderType)
        {
            return Customer.StatusToValue(orderType);
        }
public static int GetNumberTypeValue(this Customer.NumberTypeEnum orderType)
        {
            return Customer.NumberTypeToValue(orderType);
        }
public static int GetCustomerSourceTypeValue(this Customer.CustomerSourceTypeEnum orderType)
        {
            return Customer.CustomerSourceTypeToValue(orderType);
        }

	}
}
