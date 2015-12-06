using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Entity
{
	
    public partial class CustomerCreditInfo
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
			NoCheck = 1,

			/// <summary>
			/// 已审核
			/// </summary>
			Checked = 2,

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
                case "nocheck":
                    result = 1;
					break;
                case "checked":
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
					TextField = "未审核",
					TextField_EN = "未审核"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "已审核",
					TextField_EN = "已审核"
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

	public static class CustomerCreditInfoExtensions
	{
		
		public static int GetStatusValue(this CustomerCreditInfo.StatusEnum orderType)
        {
            return CustomerCreditInfo.StatusToValue(orderType);
        }

	}
}
