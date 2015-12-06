using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Entity
{
	
    public partial class Financial
    {       
 

    #region ReceivingExpenseStatusEnum 状态

		/// <summary>
        /// 状态
        /// </summary>
        public enum ReceivingExpenseStatusEnum : int
        {
			/// <summary>
			/// 未审核
			/// </summary>
			UnAudited = 1,

			/// <summary>
			/// 审核异常
			/// </summary>
			AuditAnomaly = 2,

			/// <summary>
			/// 已审核
			/// </summary>
			Audited = 3,

			/// <summary>
			/// 已出账单
			/// </summary>
			OutBilled = 4,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static ReceivingExpenseStatusEnum ParseToReceivingExpenseStatus(int value)
        {				
		   try{

			 return	(ReceivingExpenseStatusEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int ReceivingExpenseStatusToValue(ReceivingExpenseStatusEnum enumOption)
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
		public static string GetReceivingExpenseStatusDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "未审核";
					break;
                case 2:
                    result = "审核异常";
					break;
                case 3:
                    result = "已审核";
					break;
                case 4:
                    result = "已出账单";
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
		public static int GetReceivingExpenseStatus(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "unaudited":
                    result = 1;
					break;
                case "auditanomaly":
                    result = 2;
					break;
                case "audited":
                    result = 3;
					break;
                case "outbilled":
                    result = 4;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取状态的列表
        /// </summary>
		public static List<DataSourceBinder> GetReceivingExpenseStatusList()
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
					TextField = "审核异常",
					TextField_EN = "审核异常"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "已审核",
					TextField_EN = "已审核"
				},
				new DataSourceBinder{
					ValueField = "4",
					TextField = "已出账单",
					TextField_EN = "已出账单"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidReceivingExpenseStatus(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(ReceivingExpenseStatusEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


 

    #region DeliveryFeeStatusEnum 状态

		/// <summary>
        /// 状态
        /// </summary>
        public enum DeliveryFeeStatusEnum : int
        {
			/// <summary>
			/// 未审核
			/// </summary>
			UnAudited = 1,

			/// <summary>
			/// 审核异常
			/// </summary>
			AuditAnomaly = 2,

			/// <summary>
			/// 已审核
			/// </summary>
			Audited = 3,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static DeliveryFeeStatusEnum ParseToDeliveryFeeStatus(int value)
        {				
		   try{

			 return	(DeliveryFeeStatusEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int DeliveryFeeStatusToValue(DeliveryFeeStatusEnum enumOption)
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
		public static string GetDeliveryFeeStatusDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "未审核";
					break;
                case 2:
                    result = "审核异常";
					break;
                case 3:
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
		public static int GetDeliveryFeeStatus(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "unaudited":
                    result = 1;
					break;
                case "auditanomaly":
                    result = 2;
					break;
                case "audited":
                    result = 3;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取状态的列表
        /// </summary>
		public static List<DataSourceBinder> GetDeliveryFeeStatusList()
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
					TextField = "审核异常",
					TextField_EN = "审核异常"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "已审核",
					TextField_EN = "已审核"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidDeliveryFeeStatus(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(DeliveryFeeStatusEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion




        
    }

	public static class FinancialExtensions
	{
		
		public static int GetReceivingExpenseStatusValue(this Financial.ReceivingExpenseStatusEnum orderType)
        {
            return Financial.ReceivingExpenseStatusToValue(orderType);
        }
public static int GetDeliveryFeeStatusValue(this Financial.DeliveryFeeStatusEnum orderType)
        {
            return Financial.DeliveryFeeStatusToValue(orderType);
        }

	}
}
