using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Entity
{
	
    public partial class WayBillTemplateInfo
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


 

    #region TemplateTypeEnum 模板类型

		/// <summary>
        /// 模板类型
        /// </summary>
        public enum TemplateTypeEnum : int
        {
			/// <summary>
			/// 模板头
			/// </summary>
			Head = 1,

			/// <summary>
			/// 模板体
			/// </summary>
			Body = 2,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static TemplateTypeEnum ParseToTemplateType(int value)
        {				
		   try{

			 return	(TemplateTypeEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int TemplateTypeToValue(TemplateTypeEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取模板类型的描述
        /// </summary>
		public static string GetTemplateTypeDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "模板头";
					break;
                case 2:
                    result = "模板体";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取模板类型的值
        /// </summary>
		public static int GetTemplateType(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "head":
                    result = 1;
					break;
                case "body":
                    result = 2;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取模板类型的列表
        /// </summary>
		public static List<DataSourceBinder> GetTemplateTypeList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "模板头",
					TextField_EN = "模板头"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "模板体",
					TextField_EN = "模板体"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidTemplateType(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(TemplateTypeEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion




        
    }

	public static class WayBillTemplateInfoExtensions
	{
		
		public static int GetStatusValue(this WayBillTemplateInfo.StatusEnum orderType)
        {
            return WayBillTemplateInfo.StatusToValue(orderType);
        }
public static int GetTemplateTypeValue(this WayBillTemplateInfo.TemplateTypeEnum orderType)
        {
            return WayBillTemplateInfo.TemplateTypeToValue(orderType);
        }

	}
}
