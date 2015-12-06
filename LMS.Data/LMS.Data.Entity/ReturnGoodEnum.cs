using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Entity
{
	
    public partial class ReturnGood
    {       
 

    #region ReturnStatusEnum 状态

		/// <summary>
        /// 状态
        /// </summary>
        public enum ReturnStatusEnum : int
        {
			/// <summary>
			/// 未审核
			/// </summary>
			UnAudited = 1,

			/// <summary>
			/// 已审核
			/// </summary>
			Audited = 2,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static ReturnStatusEnum ParseToReturnStatus(int value)
        {				
		   try{

			 return	(ReturnStatusEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int ReturnStatusToValue(ReturnStatusEnum enumOption)
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
		public static string GetReturnStatusDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "未审核";
					break;
                case 2:
                    result = "已审核";
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
		public static int GetReturnStatus(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "unaudited":
                    result = 1;
					break;
                case "audited":
                    result = 2;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取状态的列表
        /// </summary>
		public static List<DataSourceBinder> GetReturnStatusList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "未审核",
					TextField_EN = "未审核"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "已审核",
					TextField_EN = "已审核"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidReturnStatus(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(ReturnStatusEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


 

    #region ReturnSourceStatusEnum 状态

		/// <summary>
        /// 状态
        /// </summary>
        public enum ReturnSourceStatusEnum : int
        {
			/// <summary>
			/// BS退货
			/// </summary>
			BSReturn = 1,

			/// <summary>
			/// CS退货
			/// </summary>
			CSReturn = 2,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static ReturnSourceStatusEnum ParseToReturnSourceStatus(int value)
        {				
		   try{

			 return	(ReturnSourceStatusEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int ReturnSourceStatusToValue(ReturnSourceStatusEnum enumOption)
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
		public static string GetReturnSourceStatusDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "BS退货";
					break;
                case 2:
                    result = "CS退货";
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
		public static int GetReturnSourceStatus(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "bsreturn":
                    result = 1;
					break;
                case "csreturn":
                    result = 2;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取状态的列表
        /// </summary>
		public static List<DataSourceBinder> GetReturnSourceStatusList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "BS退货",
					TextField_EN = "BS退货"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "CS退货",
					TextField_EN = "CS退货"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidReturnSourceStatus(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(ReturnSourceStatusEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion




        
    }

	public static class ReturnGoodExtensions
	{
		
		public static int GetReturnStatusValue(this ReturnGood.ReturnStatusEnum orderType)
        {
            return ReturnGood.ReturnStatusToValue(orderType);
        }
public static int GetReturnSourceStatusValue(this ReturnGood.ReturnSourceStatusEnum orderType)
        {
            return ReturnGood.ReturnSourceStatusToValue(orderType);
        }

	}
}
