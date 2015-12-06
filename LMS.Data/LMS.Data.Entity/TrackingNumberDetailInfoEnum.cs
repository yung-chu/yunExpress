using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Entity
{
	
    public partial class TrackingNumberDetailInfo
    {       
 

    #region StatusEnum 状态

		/// <summary>
        /// 状态
        /// </summary>
        public enum StatusEnum : int
        {
			/// <summary>
			/// 未使用
			/// </summary>
			NotUsed = 1,

			/// <summary>
			/// 已使用
			/// </summary>
			Used = 2,

			/// <summary>
			/// 无效
			/// </summary>
			Invalid = 3,

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
                    result = "未使用";
					break;
                case 2:
                    result = "已使用";
					break;
                case 3:
                    result = "无效";
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
                case "notused":
                    result = 1;
					break;
                case "used":
                    result = 2;
					break;
                case "invalid":
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
					TextField = "未使用",
					TextField_EN = "未使用"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "已使用",
					TextField_EN = "已使用"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "无效",
					TextField_EN = "无效"
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

	public static class TrackingNumberDetailInfoExtensions
	{
		
		public static int GetStatusValue(this TrackingNumberDetailInfo.StatusEnum orderType)
        {
            return TrackingNumberDetailInfo.StatusToValue(orderType);
        }

	}
}
