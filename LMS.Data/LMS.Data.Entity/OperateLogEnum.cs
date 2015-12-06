using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Entity
{
	
    public partial class OperateLog
    {       
 

    #region KeyWordTypeEnum 关键词类别

		/// <summary>
        /// 关键词类别
        /// </summary>
        public enum KeyWordTypeEnum : int
        {
			/// <summary>
			/// 请选择
			/// </summary>
			None = 0,

			/// <summary>
			/// 运单号
			/// </summary>
			WayBillNumber = 1,

			/// <summary>
			/// 订单号
			/// </summary>
			CustomerOrderNumber = 2,

			/// <summary>
			/// 客户编号
			/// </summary>
			CustomerCode = 3,

			/// <summary>
			/// 物流模板编号
			/// </summary>
			Lms_WayBillTemplateId = 4,

			/// <summary>
			/// 运费模板编号
			/// </summary>
			Lis_TemplateId = 5,

			/// <summary>
			/// 运输方式
			/// </summary>
			ShippingMethodId = 6,

			/// <summary>
			/// 类别编号
			/// </summary>
			CategoryId = 7,

			/// <summary>
			/// 新闻编号
			/// </summary>
			ArticleId = 8,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static KeyWordTypeEnum ParseToKeyWordType(int value)
        {				
		   try{

			 return	(KeyWordTypeEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int KeyWordTypeToValue(KeyWordTypeEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取关键词类别的描述
        /// </summary>
		public static string GetKeyWordTypeDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 0:
                    result = "请选择";
					break;
                case 1:
                    result = "运单号";
					break;
                case 2:
                    result = "订单号";
					break;
                case 3:
                    result = "客户编号";
					break;
                case 4:
                    result = "物流模板编号";
					break;
                case 5:
                    result = "运费模板编号";
					break;
                case 6:
                    result = "运输方式";
					break;
                case 7:
                    result = "类别编号";
					break;
                case 8:
                    result = "新闻编号";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取关键词类别的值
        /// </summary>
		public static int GetKeyWordType(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "none":
                    result = 0;
					break;
                case "waybillnumber":
                    result = 1;
					break;
                case "customerordernumber":
                    result = 2;
					break;
                case "customercode":
                    result = 3;
					break;
                case "lms_waybilltemplateid":
                    result = 4;
					break;
                case "lis_templateid":
                    result = 5;
					break;
                case "shippingmethodid":
                    result = 6;
					break;
                case "categoryid":
                    result = 7;
					break;
                case "articleid":
                    result = 8;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取关键词类别的列表
        /// </summary>
		public static List<DataSourceBinder> GetKeyWordTypeList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "0",
					TextField = "请选择",
					TextField_EN = "请选择"
				},
				new DataSourceBinder{
					ValueField = "1",
					TextField = "运单号",
					TextField_EN = "运单号"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "订单号",
					TextField_EN = "订单号"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "客户编号",
					TextField_EN = "客户编号"
				},
				new DataSourceBinder{
					ValueField = "4",
					TextField = "物流模板编号",
					TextField_EN = "物流模板编号"
				},
				new DataSourceBinder{
					ValueField = "5",
					TextField = "运费模板编号",
					TextField_EN = "运费模板编号"
				},
				new DataSourceBinder{
					ValueField = "6",
					TextField = "运输方式",
					TextField_EN = "运输方式"
				},
				new DataSourceBinder{
					ValueField = "7",
					TextField = "类别编号",
					TextField_EN = "类别编号"
				},
				new DataSourceBinder{
					ValueField = "8",
					TextField = "新闻编号",
					TextField_EN = "新闻编号"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidKeyWordType(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(KeyWordTypeEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion




        
    }

	public static class OperateLogExtensions
	{
		
		public static int GetKeyWordTypeValue(this OperateLog.KeyWordTypeEnum orderType)
        {
            return OperateLog.KeyWordTypeToValue(orderType);
        }

	}
}
