using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Enums
{
	
    public partial class OrderDetailEnum
    { 
		      
    #region ShippingStatusEnum ShippingStatus

		/// <summary>
        /// ShippingStatus
        /// </summary>
        public enum ShippingStatusEnum : int
        {
			/// <summary>
			/// 未发货
			/// </summary>
			UnShipped = 1,

			/// <summary>
			/// 已发货
			/// </summary>
			Shipped  = 2,

			/// <summary>
			/// 已取消
			/// </summary>
			Cancelled  = 3,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static ShippingStatusEnum ParseToShippingStatus(int value)
        {				
		   try{

			 return	(ShippingStatusEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int ShippingStatusToValue(ShippingStatusEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取ShippingStatus的描述
        /// </summary>
		public static string GetShippingStatusDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "UnShipped";
					break;
                case 2:
                    result = "Shipped";
					break;
                case 3:
                    result = "Cancelled";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}



				/// <summary>
        /// 获取ShippingStatus的描述
        /// </summary>
		public static string GetShippingStatusCNDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "未发货";
					break;
                case 2:
                    result = "已发货";
					break;
                case 3:
                    result = "已取消";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}


		/// <summary>
        /// 获取ShippingStatus的值
        /// </summary>
		public static int GetShippingStatus(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "unshipped":
                    result = 1;
					break;
                case "shipped ":
                    result = 2;
					break;
                case "cancelled ":
                    result = 3;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取ShippingStatus的列表
        /// </summary>
		public static List<DataSourceBinder> GetShippingStatusList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "未发货",
					TextField_EN = "UnShipped"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "已发货",
					TextField_EN = "Shipped"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "已取消",
					TextField_EN = "Cancelled"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidShippingStatus(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(ShippingStatusEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


        
    }

	public static class OrderDetailEnumExtensions
	{
		
		public static int GetShippingStatusValue(this OrderDetailEnum.ShippingStatusEnum orderType)
        {
            return OrderDetailEnum.ShippingStatusToValue(orderType);
        }

	}
}
