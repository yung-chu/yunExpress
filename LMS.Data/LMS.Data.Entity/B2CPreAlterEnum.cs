using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Entity
{
	
    public partial class B2CPreAlter
    {       
 

    #region StatusEnum 预报状态

		/// <summary>
        /// 预报状态
        /// </summary>
        public enum StatusEnum : int
        {
			/// <summary>
			/// 未预报
			/// </summary>
			None = 0,

			/// <summary>
			/// 预报中
			/// </summary>
			Submit = 1,

			/// <summary>
			/// 已预报
			/// </summary>
			OK = 2,

			/// <summary>
			/// 预报失败
			/// </summary>
			SubmitFail = 3,

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
        /// 获取预报状态的描述
        /// </summary>
		public static string GetStatusDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 0:
                    result = "未预报";
					break;
                case 1:
                    result = "预报中";
					break;
                case 2:
                    result = "已预报";
					break;
                case 3:
                    result = "预报失败";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取预报状态的值
        /// </summary>
		public static int GetStatus(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "none":
                    result = 0;
					break;
                case "submit":
                    result = 1;
					break;
                case "ok":
                    result = 2;
					break;
                case "submitfail":
                    result = 3;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取预报状态的列表
        /// </summary>
		public static List<DataSourceBinder> GetStatusList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "0",
					TextField = "未预报",
					TextField_EN = "未预报"
				},
				new DataSourceBinder{
					ValueField = "1",
					TextField = "预报中",
					TextField_EN = "预报中"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "已预报",
					TextField_EN = "已预报"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "预报失败",
					TextField_EN = "预报失败"
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

	public static class B2CPreAlterExtensions
	{
		
		public static int GetStatusValue(this B2CPreAlter.StatusEnum orderType)
        {
            return B2CPreAlter.StatusToValue(orderType);
        }

	}
}
