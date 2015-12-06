using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Entity
{
	
    public partial class MailReturnGoodsLogs
    {       
 

    #region ReasonTypeEnum 福邮宝退件原因

		/// <summary>
        /// 福邮宝退件原因
        /// </summary>
        public enum ReasonTypeEnum : int
        {
			/// <summary>
			/// 海关未过
			/// </summary>
			CHNoPass = 1,

			/// <summary>
			/// 其他原因
			/// </summary>
			Other = 2,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static ReasonTypeEnum ParseToReasonType(int value)
        {				
		   try{

			 return	(ReasonTypeEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int ReasonTypeToValue(ReasonTypeEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取福邮宝退件原因的描述
        /// </summary>
		public static string GetReasonTypeDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "海关未过";
					break;
                case 2:
                    result = "其他原因";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取福邮宝退件原因的值
        /// </summary>
		public static int GetReasonType(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "chnopass":
                    result = 1;
					break;
                case "other":
                    result = 2;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取福邮宝退件原因的列表
        /// </summary>
		public static List<DataSourceBinder> GetReasonTypeList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "海关未过",
					TextField_EN = "海关未过"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "其他原因",
					TextField_EN = "其他原因"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidReasonType(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(ReasonTypeEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion




        
    }

	public static class MailReturnGoodsLogsExtensions
	{
		
		public static int GetReasonTypeValue(this MailReturnGoodsLogs.ReasonTypeEnum orderType)
        {
            return MailReturnGoodsLogs.ReasonTypeToValue(orderType);
        }

	}
}
