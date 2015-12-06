using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Entity
{
	
    public partial class Settlement
    {       
 

    #region StatusEnum 结算清单状态

		/// <summary>
        /// 结算清单状态
        /// </summary>
        public enum StatusEnum : int
        {
			/// <summary>
			/// 未结清
			/// </summary>
			Outstanding = 1,

			/// <summary>
			/// 已结清
			/// </summary>
			OK = 2,

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
        /// 获取结算清单状态的描述
        /// </summary>
		public static string GetStatusDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "未结清";
					break;
                case 2:
                    result = "已结清";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取结算清单状态的值
        /// </summary>
		public static int GetStatus(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "outstanding":
                    result = 1;
					break;
                case "ok":
                    result = 2;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取结算清单状态的列表
        /// </summary>
		public static List<DataSourceBinder> GetStatusList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "未结清",
					TextField_EN = "未结清"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "已结清",
					TextField_EN = "已结清"
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

	public static class SettlementExtensions
	{
		
		public static int GetStatusValue(this Settlement.StatusEnum orderType)
        {
            return Settlement.StatusToValue(orderType);
        }

	}
}
