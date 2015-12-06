using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Entity
{
	
    public partial class WayBill
    {       
 

    #region StatusEnum 状态

		/// <summary>
        /// 状态
        /// </summary>
        public enum StatusEnum : int
        {
			/// <summary>
			/// 已提交
			/// </summary>
			Submitted = 3,

			/// <summary>
			/// 入仓中[入仓扫描保存处理中]
			/// </summary>
			InStoraging = 350,

			/// <summary>
			/// 已收货
			/// </summary>
			Have = 4,

			/// <summary>
			/// 待转单
			/// </summary>
			WaitOrder = 8,

			/// <summary>
			/// 发货运输中
			/// </summary>
			Send = 5,

			/// <summary>
			/// 已删除
			/// </summary>
			Delete = 6,

			/// <summary>
			/// 退货在仓
			/// </summary>
			ReGoodsInStorage = 9,

			/// <summary>
			/// 已退回
			/// </summary>
			Return = 7,

			/// <summary>
			/// 已签收
			/// </summary>
			Delivered = 10,

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
                case 3:
                    result = "已提交";
					break;
                case 350:
                    result = "入仓中[入仓扫描保存处理中]";
					break;
                case 4:
                    result = "已收货";
					break;
                case 8:
                    result = "待转单";
					break;
                case 5:
                    result = "发货运输中";
					break;
                case 6:
                    result = "已删除";
					break;
                case 9:
                    result = "退货在仓";
					break;
                case 7:
                    result = "已退回";
					break;
                case 10:
                    result = "已签收";
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
                case "submitted":
                    result = 3;
					break;
                case "instoraging":
                    result = 350;
					break;
                case "have":
                    result = 4;
					break;
                case "waitorder":
                    result = 8;
					break;
                case "send":
                    result = 5;
					break;
                case "delete":
                    result = 6;
					break;
                case "regoodsinstorage":
                    result = 9;
					break;
                case "return":
                    result = 7;
					break;
                case "delivered":
                    result = 10;
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
					ValueField = "3",
					TextField = "已提交",
					TextField_EN = "已提交"
				},
				new DataSourceBinder{
					ValueField = "350",
					TextField = "入仓中[入仓扫描保存处理中]",
					TextField_EN = "入仓中[入仓扫描保存处理中]"
				},
				new DataSourceBinder{
					ValueField = "4",
					TextField = "已收货",
					TextField_EN = "已收货"
				},
				new DataSourceBinder{
					ValueField = "8",
					TextField = "待转单",
					TextField_EN = "待转单"
				},
				new DataSourceBinder{
					ValueField = "5",
					TextField = "发货运输中",
					TextField_EN = "发货运输中"
				},
				new DataSourceBinder{
					ValueField = "6",
					TextField = "已删除",
					TextField_EN = "已删除"
				},
				new DataSourceBinder{
					ValueField = "9",
					TextField = "退货在仓",
					TextField_EN = "退货在仓"
				},
				new DataSourceBinder{
					ValueField = "7",
					TextField = "已退回",
					TextField_EN = "已退回"
				},
				new DataSourceBinder{
					ValueField = "10",
					TextField = "已签收",
					TextField_EN = "已签收"
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


 

    #region SearchFilterEnum 查询条件字段

		/// <summary>
        /// 查询条件字段
        /// </summary>
        public enum SearchFilterEnum : int
        {
			/// <summary>
			/// 订单号
			/// </summary>
			CustomerOrderNumber = 2,

			/// <summary>
			/// 运单号
			/// </summary>
			WayBillNumber = 1,

			/// <summary>
			/// 跟踪号
			/// </summary>
			TrackingNumber = 3,

			/// <summary>
			/// 入仓号
			/// </summary>
			InStorageNumber = 4,

			/// <summary>
			/// 出仓号
			/// </summary>
			OutStorageNumber = 5,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static SearchFilterEnum ParseToSearchFilter(int value)
        {				
		   try{

			 return	(SearchFilterEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int SearchFilterToValue(SearchFilterEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取查询条件字段的描述
        /// </summary>
		public static string GetSearchFilterDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 2:
                    result = "订单号";
					break;
                case 1:
                    result = "运单号";
					break;
                case 3:
                    result = "跟踪号";
					break;
                case 4:
                    result = "入仓号";
					break;
                case 5:
                    result = "出仓号";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取查询条件字段的值
        /// </summary>
		public static int GetSearchFilter(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "customerordernumber":
                    result = 2;
					break;
                case "waybillnumber":
                    result = 1;
					break;
                case "trackingnumber":
                    result = 3;
					break;
                case "instoragenumber":
                    result = 4;
					break;
                case "outstoragenumber":
                    result = 5;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取查询条件字段的列表
        /// </summary>
		public static List<DataSourceBinder> GetSearchFilterList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "2",
					TextField = "订单号",
					TextField_EN = "订单号"
				},
				new DataSourceBinder{
					ValueField = "1",
					TextField = "运单号",
					TextField_EN = "运单号"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "跟踪号",
					TextField_EN = "跟踪号"
				},
				new DataSourceBinder{
					ValueField = "4",
					TextField = "入仓号",
					TextField_EN = "入仓号"
				},
				new DataSourceBinder{
					ValueField = "5",
					TextField = "出仓号",
					TextField_EN = "出仓号"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidSearchFilter(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(SearchFilterEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


 

    #region DateFilterEnum 时间条件字段

		/// <summary>
        /// 时间条件字段
        /// </summary>
        public enum DateFilterEnum : int
        {
			/// <summary>
			/// 创建时间
			/// </summary>
			CreatedOn = 1,

			/// <summary>
			/// 收货时间
			/// </summary>
			TakeOverOn = 2,

			/// <summary>
			/// 发货时间
			/// </summary>
			DeliverOn = 3,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static DateFilterEnum ParseToDateFilter(int value)
        {				
		   try{

			 return	(DateFilterEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int DateFilterToValue(DateFilterEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取时间条件字段的描述
        /// </summary>
		public static string GetDateFilterDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "创建时间";
					break;
                case 2:
                    result = "收货时间";
					break;
                case 3:
                    result = "发货时间";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取时间条件字段的值
        /// </summary>
		public static int GetDateFilter(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "createdon":
                    result = 1;
					break;
                case "takeoveron":
                    result = 2;
					break;
                case "deliveron":
                    result = 3;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取时间条件字段的列表
        /// </summary>
		public static List<DataSourceBinder> GetDateFilterList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "创建时间",
					TextField_EN = "创建时间"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "收货时间",
					TextField_EN = "收货时间"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "发货时间",
					TextField_EN = "发货时间"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidDateFilter(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(DateFilterEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


 

    #region AbnormalTypeEnum 运单异常类型

		/// <summary>
        /// 运单异常类型
        /// </summary>
        public enum AbnormalTypeEnum : int
        {
			/// <summary>
			/// 拦截
			/// </summary>
			Intercepted = 1,

			/// <summary>
			/// 入仓异常
			/// </summary>
			InAbnormal = 2,

			/// <summary>
			/// 出仓异常
			/// </summary>
			OutAbnormal = 3,

			/// <summary>
			/// 退货异常
			/// </summary>
			ReGoodsAbnormal = 4,

			/// <summary>
			/// 入仓重量异常
			/// </summary>
			InStorageWeightAbnormal = 5,

			/// <summary>
			/// 提交顺丰异常
			/// </summary>
			SubmitSFAbnormal = 6,

			/// <summary>
			/// 预报异常
			/// </summary>
			Forecast = 7,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static AbnormalTypeEnum ParseToAbnormalType(int value)
        {				
		   try{

			 return	(AbnormalTypeEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int AbnormalTypeToValue(AbnormalTypeEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取运单异常类型的描述
        /// </summary>
		public static string GetAbnormalTypeDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "拦截";
					break;
                case 2:
                    result = "入仓异常";
					break;
                case 3:
                    result = "出仓异常";
					break;
                case 4:
                    result = "退货异常";
					break;
                case 5:
                    result = "入仓重量异常";
					break;
                case 6:
                    result = "提交顺丰异常";
					break;
                case 7:
                    result = "预报异常";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取运单异常类型的值
        /// </summary>
		public static int GetAbnormalType(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "intercepted":
                    result = 1;
					break;
                case "inabnormal":
                    result = 2;
					break;
                case "outabnormal":
                    result = 3;
					break;
                case "regoodsabnormal":
                    result = 4;
					break;
                case "instorageweightabnormal":
                    result = 5;
					break;
                case "submitsfabnormal":
                    result = 6;
					break;
                case "forecast":
                    result = 7;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取运单异常类型的列表
        /// </summary>
		public static List<DataSourceBinder> GetAbnormalTypeList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "拦截",
					TextField_EN = "拦截"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "入仓异常",
					TextField_EN = "入仓异常"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "出仓异常",
					TextField_EN = "出仓异常"
				},
				new DataSourceBinder{
					ValueField = "4",
					TextField = "退货异常",
					TextField_EN = "退货异常"
				},
				new DataSourceBinder{
					ValueField = "5",
					TextField = "入仓重量异常",
					TextField_EN = "入仓重量异常"
				},
				new DataSourceBinder{
					ValueField = "6",
					TextField = "提交顺丰异常",
					TextField_EN = "提交顺丰异常"
				},
				new DataSourceBinder{
					ValueField = "7",
					TextField = "预报异常",
					TextField_EN = "预报异常"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidAbnormalType(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(AbnormalTypeEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


 

    #region AbnormalStatusEnum 运单异常状态

		/// <summary>
        /// 运单异常状态
        /// </summary>
        public enum AbnormalStatusEnum : int
        {
			/// <summary>
			/// 未完成
			/// </summary>
			NO = 1,

			/// <summary>
			/// 已完成
			/// </summary>
			OK = 2,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static AbnormalStatusEnum ParseToAbnormalStatus(int value)
        {				
		   try{

			 return	(AbnormalStatusEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int AbnormalStatusToValue(AbnormalStatusEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取运单异常状态的描述
        /// </summary>
		public static string GetAbnormalStatusDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "未完成";
					break;
                case 2:
                    result = "已完成";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取运单异常状态的值
        /// </summary>
		public static int GetAbnormalStatus(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "no":
                    result = 1;
					break;
                case "ok":
                    result = 2;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取运单异常状态的列表
        /// </summary>
		public static List<DataSourceBinder> GetAbnormalStatusList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "未完成",
					TextField_EN = "未完成"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "已完成",
					TextField_EN = "已完成"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidAbnormalStatus(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(AbnormalStatusEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


 

    #region ScanTypesEnum 扫描方式

		/// <summary>
        /// 扫描方式
        /// </summary>
        public enum ScanTypesEnum : int
        {
			/// <summary>
			/// 按订单号
			/// </summary>
			None = 0,

			/// <summary>
			/// 按跟踪号
			/// </summary>
			TrackingNumber = 1,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static ScanTypesEnum ParseToScanTypes(int value)
        {				
		   try{

			 return	(ScanTypesEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int ScanTypesToValue(ScanTypesEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取扫描方式的描述
        /// </summary>
		public static string GetScanTypesDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 0:
                    result = "按订单号";
					break;
                case 1:
                    result = "按跟踪号";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取扫描方式的值
        /// </summary>
		public static int GetScanTypes(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "none":
                    result = 0;
					break;
                case "trackingnumber":
                    result = 1;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取扫描方式的列表
        /// </summary>
		public static List<DataSourceBinder> GetScanTypesList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "0",
					TextField = "按订单号",
					TextField_EN = "按订单号"
				},
				new DataSourceBinder{
					ValueField = "1",
					TextField = "按跟踪号",
					TextField_EN = "按跟踪号"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidScanTypes(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(ScanTypesEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


 

    #region ReturnGoodTypeEnum 退货类型

		/// <summary>
        /// 退货类型
        /// </summary>
        public enum ReturnGoodTypeEnum : int
        {
			/// <summary>
			/// 内部
			/// </summary>
			Internal = 1,

			/// <summary>
			/// 外部
			/// </summary>
			External = 2,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static ReturnGoodTypeEnum ParseToReturnGoodType(int value)
        {				
		   try{

			 return	(ReturnGoodTypeEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int ReturnGoodTypeToValue(ReturnGoodTypeEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取退货类型的描述
        /// </summary>
		public static string GetReturnGoodTypeDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "内部";
					break;
                case 2:
                    result = "外部";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取退货类型的值
        /// </summary>
		public static int GetReturnGoodType(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "internal":
                    result = 1;
					break;
                case "external":
                    result = 2;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取退货类型的列表
        /// </summary>
		public static List<DataSourceBinder> GetReturnGoodTypeList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "内部",
					TextField_EN = "内部"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "外部",
					TextField_EN = "外部"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidReturnGoodType(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(ReturnGoodTypeEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


 

    #region NoForecastAbnormalEnum 无预报异常状态

		/// <summary>
        /// 无预报异常状态
        /// </summary>
        public enum NoForecastAbnormalEnum : int
        {
			/// <summary>
			/// 有预报
			/// </summary>
			Forecasted = 1,

			/// <summary>
			/// 无预报
			/// </summary>
			NoForecast = 2,

			/// <summary>
			/// 无预报退回
			/// </summary>
			NoForecastRetrun = 3,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static NoForecastAbnormalEnum ParseToNoForecastAbnormal(int value)
        {				
		   try{

			 return	(NoForecastAbnormalEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int NoForecastAbnormalToValue(NoForecastAbnormalEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取无预报异常状态的描述
        /// </summary>
		public static string GetNoForecastAbnormalDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "有预报";
					break;
                case 2:
                    result = "无预报";
					break;
                case 3:
                    result = "无预报退回";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取无预报异常状态的值
        /// </summary>
		public static int GetNoForecastAbnormal(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "forecasted":
                    result = 1;
					break;
                case "noforecast":
                    result = 2;
					break;
                case "noforecastretrun":
                    result = 3;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取无预报异常状态的列表
        /// </summary>
		public static List<DataSourceBinder> GetNoForecastAbnormalList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "有预报",
					TextField_EN = "有预报"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "无预报",
					TextField_EN = "无预报"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "无预报退回",
					TextField_EN = "无预报退回"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidNoForecastAbnormal(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(NoForecastAbnormalEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion




        
    }

	public static class WayBillExtensions
	{
		
		public static int GetStatusValue(this WayBill.StatusEnum orderType)
        {
            return WayBill.StatusToValue(orderType);
        }
public static int GetSearchFilterValue(this WayBill.SearchFilterEnum orderType)
        {
            return WayBill.SearchFilterToValue(orderType);
        }
public static int GetDateFilterValue(this WayBill.DateFilterEnum orderType)
        {
            return WayBill.DateFilterToValue(orderType);
        }
public static int GetAbnormalTypeValue(this WayBill.AbnormalTypeEnum orderType)
        {
            return WayBill.AbnormalTypeToValue(orderType);
        }
public static int GetAbnormalStatusValue(this WayBill.AbnormalStatusEnum orderType)
        {
            return WayBill.AbnormalStatusToValue(orderType);
        }
public static int GetScanTypesValue(this WayBill.ScanTypesEnum orderType)
        {
            return WayBill.ScanTypesToValue(orderType);
        }
public static int GetReturnGoodTypeValue(this WayBill.ReturnGoodTypeEnum orderType)
        {
            return WayBill.ReturnGoodTypeToValue(orderType);
        }
public static int GetNoForecastAbnormalValue(this WayBill.NoForecastAbnormalEnum orderType)
        {
            return WayBill.NoForecastAbnormalToValue(orderType);
        }

	}
}
