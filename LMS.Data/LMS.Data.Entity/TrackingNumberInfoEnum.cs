using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Entity
{
	
    public partial class TrackingNumberInfo
    {       
 

    #region StatusEnum 状态

		/// <summary>
        /// 状态
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
        /// 获取状态的描述
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
        /// 获取状态的值
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
        /// 获取状态的列表
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


 

    #region AddressLabelEnum 地址标签

		/// <summary>
        /// 地址标签
        /// </summary>
        public enum AddressLabelEnum : int
        {
			/// <summary>
			/// TNT地址标签
			/// </summary>
			TNT = 1,

			/// <summary>
			/// 中邮地址标签
			/// </summary>
			ChinaPost = 2,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static AddressLabelEnum ParseToAddressLabel(int value)
        {				
		   try{

			 return	(AddressLabelEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int AddressLabelToValue(AddressLabelEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取地址标签的描述
        /// </summary>
		public static string GetAddressLabelDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "TNT地址标签";
					break;
                case 2:
                    result = "中邮地址标签";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取地址标签的值
        /// </summary>
		public static int GetAddressLabel(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "tnt":
                    result = 1;
					break;
                case "chinapost":
                    result = 2;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取地址标签的列表
        /// </summary>
		public static List<DataSourceBinder> GetAddressLabelList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "TNT地址标签",
					TextField_EN = "TNT地址标签"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "中邮地址标签",
					TextField_EN = "中邮地址标签"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidAddressLabel(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(AddressLabelEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion




        
    }

	public static class TrackingNumberInfoExtensions
	{
		
		public static int GetStatusValue(this TrackingNumberInfo.StatusEnum orderType)
        {
            return TrackingNumberInfo.StatusToValue(orderType);
        }
public static int GetAddressLabelValue(this TrackingNumberInfo.AddressLabelEnum orderType)
        {
            return TrackingNumberInfo.AddressLabelToValue(orderType);
        }

	}
}
