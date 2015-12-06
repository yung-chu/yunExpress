using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Entity
{
	
    public partial class WayBillEvent
    {       
 

    #region EventCodeEnum 操作码

		/// <summary>
        /// 操作码
        /// </summary>
        public enum EventCodeEnum : int
        {
			/// <summary>
			/// 提交
			/// </summary>
			Submit = 100,

			/// <summary>
			/// 入仓
			/// </summary>
			InStorage = 200,

			/// <summary>
			/// 出仓
			/// </summary>
			OutStorage = 300,

			/// <summary>
			/// 退货
			/// </summary>
			ReturnGood = 400,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static EventCodeEnum ParseToEventCode(int value)
        {				
		   try{

			 return	(EventCodeEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int EventCodeToValue(EventCodeEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取操作码的描述
        /// </summary>
		public static string GetEventCodeDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 100:
                    result = "提交";
					break;
                case 200:
                    result = "入仓";
					break;
                case 300:
                    result = "出仓";
					break;
                case 400:
                    result = "退货";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取操作码的值
        /// </summary>
		public static int GetEventCode(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "submit":
                    result = 100;
					break;
                case "instorage":
                    result = 200;
					break;
                case "outstorage":
                    result = 300;
					break;
                case "returngood":
                    result = 400;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取操作码的列表
        /// </summary>
		public static List<DataSourceBinder> GetEventCodeList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "100",
					TextField = "提交",
					TextField_EN = "提交"
				},
				new DataSourceBinder{
					ValueField = "200",
					TextField = "入仓",
					TextField_EN = "入仓"
				},
				new DataSourceBinder{
					ValueField = "300",
					TextField = "出仓",
					TextField_EN = "出仓"
				},
				new DataSourceBinder{
					ValueField = "400",
					TextField = "退货",
					TextField_EN = "退货"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidEventCode(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(EventCodeEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion




        
    }

	public static class WayBillEventExtensions
	{
		
		public static int GetEventCodeValue(this WayBillEvent.EventCodeEnum orderType)
        {
            return WayBillEvent.EventCodeToValue(orderType);
        }

	}
}
