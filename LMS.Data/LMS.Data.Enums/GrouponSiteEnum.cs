using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Enums
{
	
    public partial class GrouponSiteEnum
    { 
		      
    #region GrouponSiteTypeEnum 

		/// <summary>
        /// 
        /// </summary>
        public enum GrouponSiteTypeEnum : int
        {
			/// <summary>
			/// NoMoreRack
			/// </summary>
			NoMoreRack = 1,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static GrouponSiteTypeEnum ParseToGrouponSiteType(int value)
        {				
		   try{

			 return	(GrouponSiteTypeEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int GrouponSiteTypeToValue(GrouponSiteTypeEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取的描述
        /// </summary>
		public static string GetGrouponSiteTypeDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "NoMoreRack";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}



				/// <summary>
        /// 获取的描述
        /// </summary>
		public static string GetGrouponSiteTypeCNDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "NoMoreRack";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}


		/// <summary>
        /// 获取的值
        /// </summary>
		public static int GetGrouponSiteType(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "nomorerack":
                    result = 1;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取的列表
        /// </summary>
		public static List<DataSourceBinder> GetGrouponSiteTypeList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "NoMoreRack",
					TextField_EN = "NoMoreRack"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidGrouponSiteType(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(GrouponSiteTypeEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


        
    }

	public static class GrouponSiteEnumExtensions
	{
		
		public static int GetGrouponSiteTypeValue(this GrouponSiteEnum.GrouponSiteTypeEnum orderType)
        {
            return GrouponSiteEnum.GrouponSiteTypeToValue(orderType);
        }

	}
}
