using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Entity
{
	
    public partial class TotalPackageConfig
    {       
 

    #region ShowTimeEnum 显示编辑时间

		/// <summary>
        /// 显示编辑时间
        /// </summary>
        public enum ShowTimeEnum : int
        {
			/// <summary>
			/// 到港时间
			/// </summary>
			ToPort = 4,

			/// <summary>
			/// 离港时间
			/// </summary>
			FromPort = 5,

			/// <summary>
			/// 到达机场时间
			/// </summary>
			ToAirport = 6,

			/// <summary>
			/// 机场清关完成时间
			/// </summary>
			CustomsClearance = 7,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static ShowTimeEnum ParseToShowTime(int value)
        {				
		   try{

			 return	(ShowTimeEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int ShowTimeToValue(ShowTimeEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取显示编辑时间的描述
        /// </summary>
		public static string GetShowTimeDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 4:
                    result = "到港时间";
					break;
                case 5:
                    result = "离港时间";
					break;
                case 6:
                    result = "到达机场时间";
					break;
                case 7:
                    result = "机场清关完成时间";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取显示编辑时间的值
        /// </summary>
		public static int GetShowTime(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "toport":
                    result = 4;
					break;
                case "fromport":
                    result = 5;
					break;
                case "toairport":
                    result = 6;
					break;
                case "customsclearance":
                    result = 7;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取显示编辑时间的列表
        /// </summary>
		public static List<DataSourceBinder> GetShowTimeList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "4",
					TextField = "到港时间",
					TextField_EN = "到港时间"
				},
				new DataSourceBinder{
					ValueField = "5",
					TextField = "离港时间",
					TextField_EN = "离港时间"
				},
				new DataSourceBinder{
					ValueField = "6",
					TextField = "到达机场时间",
					TextField_EN = "到达机场时间"
				},
				new DataSourceBinder{
					ValueField = "7",
					TextField = "机场清关完成时间",
					TextField_EN = "机场清关完成时间"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidShowTime(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(ShowTimeEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion




        
    }

	public static class TotalPackageConfigExtensions
	{
		
		public static int GetShowTimeValue(this TotalPackageConfig.ShowTimeEnum orderType)
        {
            return TotalPackageConfig.ShowTimeToValue(orderType);
        }

	}
}
