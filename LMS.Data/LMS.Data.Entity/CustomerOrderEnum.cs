using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Entity
{
	
    public partial class CustomerOrder
    {       
 

    #region StatusEnum 状态

		/// <summary>
        /// 状态
        /// </summary>
        public enum StatusEnum : int
        {
			/// <summary>
			/// 未确定
			/// </summary>
			None = 1,

			/// <summary>
			/// 已确定
			/// </summary>
			OK = 2,

			/// <summary>
			/// 已提交
			/// </summary>
			Submitted = 4,

			/// <summary>
			/// 已收货
			/// </summary>
			Have = 5,

			/// <summary>
			/// 发货运输中
			/// </summary>
			Send = 6,

			/// <summary>
			/// 已删除
			/// </summary>
			Delete = 7,

			/// <summary>
			/// 退货在仓
			/// </summary>
			ReGoodsInStorage = 9,

			/// <summary>
			/// 已退回
			/// </summary>
			Return = 8,

			/// <summary>
			/// 已签收
			/// </summary>
			Delivered = 10,

			/// <summary>
			/// 提交中
			/// </summary>
			Submiting = 11,

			/// <summary>
			/// 提交失败
			/// </summary>
			SubmitFail = 12,

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
                    result = "未确定";
					break;
                case 2:
                    result = "已确定";
					break;
                case 4:
                    result = "已提交";
					break;
                case 5:
                    result = "已收货";
					break;
                case 6:
                    result = "发货运输中";
					break;
                case 7:
                    result = "已删除";
					break;
                case 9:
                    result = "退货在仓";
					break;
                case 8:
                    result = "已退回";
					break;
                case 10:
                    result = "已签收";
					break;
                case 11:
                    result = "提交中";
					break;
                case 12:
                    result = "提交失败";
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
                case "none":
                    result = 1;
					break;
                case "ok":
                    result = 2;
					break;
                case "submitted":
                    result = 4;
					break;
                case "have":
                    result = 5;
					break;
                case "send":
                    result = 6;
					break;
                case "delete":
                    result = 7;
					break;
                case "regoodsinstorage":
                    result = 9;
					break;
                case "return":
                    result = 8;
					break;
                case "delivered":
                    result = 10;
					break;
                case "submiting":
                    result = 11;
					break;
                case "submitfail":
                    result = 12;
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
					TextField = "未确定",
					TextField_EN = "未确定"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "已确定",
					TextField_EN = "已确定"
				},
				new DataSourceBinder{
					ValueField = "4",
					TextField = "已提交",
					TextField_EN = "已提交"
				},
				new DataSourceBinder{
					ValueField = "5",
					TextField = "已收货",
					TextField_EN = "已收货"
				},
				new DataSourceBinder{
					ValueField = "6",
					TextField = "发货运输中",
					TextField_EN = "发货运输中"
				},
				new DataSourceBinder{
					ValueField = "7",
					TextField = "已删除",
					TextField_EN = "已删除"
				},
				new DataSourceBinder{
					ValueField = "9",
					TextField = "退货在仓",
					TextField_EN = "退货在仓"
				},
				new DataSourceBinder{
					ValueField = "8",
					TextField = "已退回",
					TextField_EN = "已退回"
				},
				new DataSourceBinder{
					ValueField = "10",
					TextField = "已签收",
					TextField_EN = "已签收"
				},
				new DataSourceBinder{
					ValueField = "11",
					TextField = "提交中",
					TextField_EN = "提交中"
				},
				new DataSourceBinder{
					ValueField = "12",
					TextField = "提交失败",
					TextField_EN = "提交失败"
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


 

    #region ApplicationTypeEnum 申报类型

		/// <summary>
        /// 申报类型
        /// </summary>
        public enum ApplicationTypeEnum : int
        {
			/// <summary>
			/// Others
			/// </summary>
			Others = 4,

			/// <summary>
			/// Gift
			/// </summary>
			Gift = 1,

			/// <summary>
			/// Sameple
			/// </summary>
			Sameple = 2,

			/// <summary>
			/// Documents
			/// </summary>
			Documents = 3,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static ApplicationTypeEnum ParseToApplicationType(int value)
        {				
		   try{

			 return	(ApplicationTypeEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int ApplicationTypeToValue(ApplicationTypeEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取申报类型的描述
        /// </summary>
		public static string GetApplicationTypeDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 4:
                    result = "Others";
					break;
                case 1:
                    result = "Gift";
					break;
                case 2:
                    result = "Sameple";
					break;
                case 3:
                    result = "Documents";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取申报类型的值
        /// </summary>
		public static int GetApplicationType(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "others":
                    result = 4;
					break;
                case "gift":
                    result = 1;
					break;
                case "sameple":
                    result = 2;
					break;
                case "documents":
                    result = 3;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取申报类型的列表
        /// </summary>
		public static List<DataSourceBinder> GetApplicationTypeList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "4",
					TextField = "Others",
					TextField_EN = "Others"
				},
				new DataSourceBinder{
					ValueField = "1",
					TextField = "Gift",
					TextField_EN = "Gift"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "Sameple",
					TextField_EN = "Sameple"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "Documents",
					TextField_EN = "Documents"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidApplicationType(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(ApplicationTypeEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


 

    #region InsuredCalculationsTypeEnum 保险类型

		/// <summary>
        /// 保险类型
        /// </summary>
        public enum InsuredCalculationsTypeEnum : int
        {
			/// <summary>
			/// 每件6元
			/// </summary>
			每件6元 = 1,

			/// <summary>
			/// 按投保额度
			/// </summary>
			按投保额度 = 2,

			/// <summary>
			/// 每件8元
			/// </summary>
			每件8元 = 3,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static InsuredCalculationsTypeEnum ParseToInsuredCalculationsType(int value)
        {				
		   try{

			 return	(InsuredCalculationsTypeEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int InsuredCalculationsTypeToValue(InsuredCalculationsTypeEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取保险类型的描述
        /// </summary>
		public static string GetInsuredCalculationsTypeDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "每件6元";
					break;
                case 2:
                    result = "按投保额度";
					break;
                case 3:
                    result = "每件8元";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取保险类型的值
        /// </summary>
		public static int GetInsuredCalculationsType(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "每件6元":
                    result = 1;
					break;
                case "按投保额度":
                    result = 2;
					break;
                case "每件8元":
                    result = 3;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取保险类型的列表
        /// </summary>
		public static List<DataSourceBinder> GetInsuredCalculationsTypeList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "每件6元",
					TextField_EN = "每件6元"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "按投保额度",
					TextField_EN = "按投保额度"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "每件8元",
					TextField_EN = "每件8元"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidInsuredCalculationsType(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(InsuredCalculationsTypeEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


 

    #region SensitiveTypeInfosTypeEnum 保险类型

		/// <summary>
        /// 保险类型
        /// </summary>
        public enum SensitiveTypeInfosTypeEnum : int
        {
			/// <summary>
			/// 纯电池
			/// </summary>
			纯电池 = 1,

			/// <summary>
			/// 钮扣电池
			/// </summary>
			钮扣电池 = 2,

			/// <summary>
			/// 仿牌
			/// </summary>
			仿牌 = 3,

			/// <summary>
			/// 粉沫
			/// </summary>
			粉沫 = 4,

			/// <summary>
			/// 液体
			/// </summary>
			液体 = 5,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static SensitiveTypeInfosTypeEnum ParseToSensitiveTypeInfosType(int value)
        {				
		   try{

			 return	(SensitiveTypeInfosTypeEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int SensitiveTypeInfosTypeToValue(SensitiveTypeInfosTypeEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取保险类型的描述
        /// </summary>
		public static string GetSensitiveTypeInfosTypeDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "纯电池";
					break;
                case 2:
                    result = "钮扣电池";
					break;
                case 3:
                    result = "仿牌";
					break;
                case 4:
                    result = "粉沫";
					break;
                case 5:
                    result = "液体";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取保险类型的值
        /// </summary>
		public static int GetSensitiveTypeInfosType(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "纯电池":
                    result = 1;
					break;
                case "钮扣电池":
                    result = 2;
					break;
                case "仿牌":
                    result = 3;
					break;
                case "粉沫":
                    result = 4;
					break;
                case "液体":
                    result = 5;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取保险类型的列表
        /// </summary>
		public static List<DataSourceBinder> GetSensitiveTypeInfosTypeList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "纯电池",
					TextField_EN = "纯电池"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "钮扣电池",
					TextField_EN = "钮扣电池"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "仿牌",
					TextField_EN = "仿牌"
				},
				new DataSourceBinder{
					ValueField = "4",
					TextField = "粉沫",
					TextField_EN = "粉沫"
				},
				new DataSourceBinder{
					ValueField = "5",
					TextField = "液体",
					TextField_EN = "液体"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidSensitiveTypeInfosType(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(SensitiveTypeInfosTypeEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


 

    #region MoneyChangeTypeEnum 资金变动类型

		/// <summary>
        /// 资金变动类型
        /// </summary>
        public enum MoneyChangeTypeEnum : int
        {
			/// <summary>
			/// 充值
			/// </summary>
			Recharge = 1,

			/// <summary>
			/// 扣费
			/// </summary>
			Deductions = 2,

			/// <summary>
			/// 退钱
			/// </summary>
			Refund = 3,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static MoneyChangeTypeEnum ParseToMoneyChangeType(int value)
        {				
		   try{

			 return	(MoneyChangeTypeEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int MoneyChangeTypeToValue(MoneyChangeTypeEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取资金变动类型的描述
        /// </summary>
		public static string GetMoneyChangeTypeDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "充值";
					break;
                case 2:
                    result = "扣费";
					break;
                case 3:
                    result = "退钱";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取资金变动类型的值
        /// </summary>
		public static int GetMoneyChangeType(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "recharge":
                    result = 1;
					break;
                case "deductions":
                    result = 2;
					break;
                case "refund":
                    result = 3;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取资金变动类型的列表
        /// </summary>
		public static List<DataSourceBinder> GetMoneyChangeTypeList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "充值",
					TextField_EN = "充值"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "扣费",
					TextField_EN = "扣费"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "退钱",
					TextField_EN = "退钱"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidMoneyChangeType(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(MoneyChangeTypeEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


 

    #region FeeTypeEnum 费用类型

		/// <summary>
        /// 费用类型
        /// </summary>
        public enum FeeTypeEnum : int
        {
			/// <summary>
			/// 进账
			/// </summary>
			Fetched = 1,

			/// <summary>
			/// 附加费
			/// </summary>
			Surcharge = 2,

			/// <summary>
			/// 运费
			/// </summary>
			Freight = 3,

			/// <summary>
			/// 挂号费
			/// </summary>
			Register = 4,

			/// <summary>
			/// 燃油费
			/// </summary>
			FuelCharge = 5,

			/// <summary>
			/// 关键预付服务费
			/// </summary>
			TariffPrepayFee = 6,

			/// <summary>
			/// 特殊费
			/// </summary>
			SpecialFee = 7,

			/// <summary>
			/// 安全附加费
			/// </summary>
			SecurityAppendFee = 8,

			/// <summary>
			/// 超长超重超周长费
			/// </summary>
			OverWeightLengthGirthFee = 9,

			/// <summary>
			/// 增值税费
			/// </summary>
			AddedTaxFee = 10,

			/// <summary>
			/// 杂费
			/// </summary>
			OtherFee = 11,

			/// <summary>
			/// 偏远附加费
			/// </summary>
			RemoteAreaFee = 12,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static FeeTypeEnum ParseToFeeType(int value)
        {				
		   try{

			 return	(FeeTypeEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int FeeTypeToValue(FeeTypeEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取费用类型的描述
        /// </summary>
		public static string GetFeeTypeDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "进账";
					break;
                case 2:
                    result = "附加费";
					break;
                case 3:
                    result = "运费";
					break;
                case 4:
                    result = "挂号费";
					break;
                case 5:
                    result = "燃油费";
					break;
                case 6:
                    result = "关键预付服务费";
					break;
                case 7:
                    result = "特殊费";
					break;
                case 8:
                    result = "安全附加费";
					break;
                case 9:
                    result = "超长超重超周长费";
					break;
                case 10:
                    result = "增值税费";
					break;
                case 11:
                    result = "杂费";
					break;
                case 12:
                    result = "偏远附加费";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取费用类型的值
        /// </summary>
		public static int GetFeeType(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "fetched":
                    result = 1;
					break;
                case "surcharge":
                    result = 2;
					break;
                case "freight":
                    result = 3;
					break;
                case "register":
                    result = 4;
					break;
                case "fuelcharge":
                    result = 5;
					break;
                case "tariffprepayfee":
                    result = 6;
					break;
                case "specialfee":
                    result = 7;
					break;
                case "securityappendfee":
                    result = 8;
					break;
                case "overweightlengthgirthfee":
                    result = 9;
					break;
                case "addedtaxfee":
                    result = 10;
					break;
                case "otherfee":
                    result = 11;
					break;
                case "remoteareafee":
                    result = 12;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取费用类型的列表
        /// </summary>
		public static List<DataSourceBinder> GetFeeTypeList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "进账",
					TextField_EN = "进账"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "附加费",
					TextField_EN = "附加费"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "运费",
					TextField_EN = "运费"
				},
				new DataSourceBinder{
					ValueField = "4",
					TextField = "挂号费",
					TextField_EN = "挂号费"
				},
				new DataSourceBinder{
					ValueField = "5",
					TextField = "燃油费",
					TextField_EN = "燃油费"
				},
				new DataSourceBinder{
					ValueField = "6",
					TextField = "关键预付服务费",
					TextField_EN = "关键预付服务费"
				},
				new DataSourceBinder{
					ValueField = "7",
					TextField = "特殊费",
					TextField_EN = "特殊费"
				},
				new DataSourceBinder{
					ValueField = "8",
					TextField = "安全附加费",
					TextField_EN = "安全附加费"
				},
				new DataSourceBinder{
					ValueField = "9",
					TextField = "超长超重超周长费",
					TextField_EN = "超长超重超周长费"
				},
				new DataSourceBinder{
					ValueField = "10",
					TextField = "增值税费",
					TextField_EN = "增值税费"
				},
				new DataSourceBinder{
					ValueField = "11",
					TextField = "杂费",
					TextField_EN = "杂费"
				},
				new DataSourceBinder{
					ValueField = "12",
					TextField = "偏远附加费",
					TextField_EN = "偏远附加费"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidFeeType(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(FeeTypeEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion




        
    }

	public static class CustomerOrderExtensions
	{
		
		public static int GetStatusValue(this CustomerOrder.StatusEnum orderType)
        {
            return CustomerOrder.StatusToValue(orderType);
        }
public static int GetApplicationTypeValue(this CustomerOrder.ApplicationTypeEnum orderType)
        {
            return CustomerOrder.ApplicationTypeToValue(orderType);
        }
public static int GetInsuredCalculationsTypeValue(this CustomerOrder.InsuredCalculationsTypeEnum orderType)
        {
            return CustomerOrder.InsuredCalculationsTypeToValue(orderType);
        }
public static int GetSensitiveTypeInfosTypeValue(this CustomerOrder.SensitiveTypeInfosTypeEnum orderType)
        {
            return CustomerOrder.SensitiveTypeInfosTypeToValue(orderType);
        }
public static int GetMoneyChangeTypeValue(this CustomerOrder.MoneyChangeTypeEnum orderType)
        {
            return CustomerOrder.MoneyChangeTypeToValue(orderType);
        }
public static int GetFeeTypeValue(this CustomerOrder.FeeTypeEnum orderType)
        {
            return CustomerOrder.FeeTypeToValue(orderType);
        }

	}
}
