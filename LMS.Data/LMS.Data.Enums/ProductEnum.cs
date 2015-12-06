using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Enums
{
	
    public partial class ProductEnum
    { 
		      
    #region StatusEnum Status

		/// <summary>
        /// Status
        /// </summary>
        public enum StatusEnum : int
        {
			/// <summary>
			/// 
			/// </summary>
			None = 0,

			/// <summary>
			/// 未上架
			/// </summary>
			New = 1,

			/// <summary>
			/// 上架
			/// </summary>
			Selling  = 2,

			/// <summary>
			/// 下架
			/// </summary>
			SoldOut  = 3,

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
                case 0:
                    result = "";
					break;
                case 1:
                    result = "未上架";
					break;
                case 2:
                    result = "上架";
					break;
                case 3:
                    result = "下架";
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
                case 0:
                    result = "";
					break;
                case 1:
                    result = "未上架";
					break;
                case 2:
                    result = "上架";
					break;
                case 3:
                    result = "下架";
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
                case "none":
                    result = 0;
					break;
                case "new":
                    result = 1;
					break;
                case "selling ":
                    result = 2;
					break;
                case "soldout ":
                    result = 3;
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
					ValueField = "0",
					TextField = "",
					TextField_EN = ""
				},
				new DataSourceBinder{
					ValueField = "1",
					TextField = "未上架",
					TextField_EN = "未上架"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "上架",
					TextField_EN = "上架"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "下架",
					TextField_EN = "下架"
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

	public static class ProductEnumExtensions
	{
		
		public static int GetStatusValue(this ProductEnum.StatusEnum orderType)
        {
            return ProductEnum.StatusToValue(orderType);
        }

	}
}
