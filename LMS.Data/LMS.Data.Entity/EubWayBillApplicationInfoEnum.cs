using System;
using System.Collections.Generic;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Entity
{
	
    public partial class EubWayBillApplicationInfo
    {       
 

    #region StatusEnum 状态

		/// <summary>
        /// 状态
        /// </summary>
        public enum StatusEnum : int
        {
			/// <summary>
			/// 未申请
			/// </summary>
			UnApply = 1,

			/// <summary>
			/// 已申请
			/// </summary>
			Apply = 2,

			/// <summary>
			/// 已下载
			/// </summary>
			DownLoad = 3,

			/// <summary>
			/// 已打印
			/// </summary>
			Printer = 4,

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
                    result = "未申请";
					break;
                case 2:
                    result = "已申请";
					break;
                case 3:
                    result = "已下载";
					break;
                case 4:
                    result = "已打印";
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
                case "unapply":
                    result = 1;
					break;
                case "apply":
                    result = 2;
					break;
                case "download":
                    result = 3;
					break;
                case "printer":
                    result = 4;
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
					TextField = "未申请",
					TextField_EN = "未申请"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "已申请",
					TextField_EN = "已申请"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "已下载",
					TextField_EN = "已下载"
				},
				new DataSourceBinder{
					ValueField = "4",
					TextField = "已打印",
					TextField_EN = "已打印"
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


 

    #region PrintFormatEnum 打印规格

		/// <summary>
        /// 打印规格
        /// </summary>
        public enum PrintFormatEnum : int
        {
			/// <summary>
			/// A4
			/// </summary>
			A4Page = 1,

			/// <summary>
			/// 4x4
			/// </summary>
			Page4x4 = 2,

			/// <summary>
			/// 4x6
			/// </summary>
			Page4x6 = 3,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static PrintFormatEnum ParseToPrintFormat(int value)
        {				
		   try{

			 return	(PrintFormatEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int PrintFormatToValue(PrintFormatEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取打印规格的描述
        /// </summary>
		public static string GetPrintFormatDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "A4";
					break;
                case 2:
                    result = "4x4";
					break;
                case 3:
                    result = "4x6";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取打印规格的值
        /// </summary>
		public static int GetPrintFormat(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "a4page":
                    result = 1;
					break;
                case "page4x4":
                    result = 2;
					break;
                case "page4x6":
                    result = 3;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取打印规格的列表
        /// </summary>
		public static List<DataSourceBinder> GetPrintFormatList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "A4",
					TextField_EN = "A4"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "4x4",
					TextField_EN = "4x4"
				},
				new DataSourceBinder{
					ValueField = "3",
					TextField = "4x6",
					TextField_EN = "4x6"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidPrintFormat(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(PrintFormatEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


 

    #region QueryNumberEnum 查询条件

		/// <summary>
        /// 查询条件
        /// </summary>
        public enum QueryNumberEnum : int
        {
			/// <summary>
			/// 运单号
			/// </summary>
			WayBillNumber = 1,

			/// <summary>
			/// 订单号
			/// </summary>
			OrderNumber = 2,

			/// <summary>
			/// 跟踪号
			/// </summary>
			TrackNumber = 3,

			/// <summary>
			/// 批次号
			/// </summary>
			BatchNumber = 4,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static QueryNumberEnum ParseToQueryNumber(int value)
        {				
		   try{

			 return	(QueryNumberEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int QueryNumberToValue(QueryNumberEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取查询条件的描述
        /// </summary>
		public static string GetQueryNumberDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "运单号";
					break;
                case 2:
                    result = "订单号";
					break;
                case 3:
                    result = "跟踪号";
					break;
                case 4:
                    result = "批次号";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取查询条件的值
        /// </summary>
		public static int GetQueryNumber(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "waybillnumber":
                    result = 1;
					break;
                case "ordernumber":
                    result = 2;
					break;
                case "tracknumber":
                    result = 3;
					break;
                case "batchnumber":
                    result = 4;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取查询条件的列表
        /// </summary>
		public static List<DataSourceBinder> GetQueryNumberList()
		{
			var list = new List<DataSourceBinder>
			{
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
					TextField = "跟踪号",
					TextField_EN = "跟踪号"
				},
				new DataSourceBinder{
					ValueField = "4",
					TextField = "批次号",
					TextField_EN = "批次号"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidQueryNumber(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(QueryNumberEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion


 

    #region TimeQueryEnum 时间查询

		/// <summary>
        /// 时间查询
        /// </summary>
        public enum TimeQueryEnum : int
        {
			/// <summary>
			/// 申请时间
			/// </summary>
			ApplyTime = 1,

			/// <summary>
			/// 创建时间
			/// </summary>
			CreateTime = 2,

        }

		/// <summary>
        /// 转换成枚举
        /// </summary>
		public static TimeQueryEnum ParseToTimeQuery(int value)
        {				
		   try{

			 return	(TimeQueryEnum)value;

			}catch(Exception ex){
			
			  throw new ArgumentException("value",ex);

			}
		}

	    /// <summary>
        /// 转换成枚举
        /// </summary>
		public static int TimeQueryToValue(TimeQueryEnum enumOption)
        {				
		   try{

			 return	 (int)enumOption;

			}catch(Exception ex){
			
			  throw new ArgumentException("enumOption",ex);

			}
		}


		/// <summary>
        /// 获取时间查询的描述
        /// </summary>
		public static string GetTimeQueryDescription(int id)
		{
			string result = "";
            switch (id)
            {
                case 1:
                    result = "申请时间";
					break;
                case 2:
                    result = "创建时间";
					break;
				default:
                    result = "--";
                    break;
            }
            return result;

		}

		/// <summary>
        /// 获取时间查询的值
        /// </summary>
		public static int GetTimeQuery(string name)
		{
			int result = 0;
            switch (name.ToLowerInvariant())
            {
                case "applytime":
                    result = 1;
					break;
                case "createtime":
                    result = 2;
					break;
				
            }
            return result;

		}

		/// <summary>
        /// 获取时间查询的列表
        /// </summary>
		public static List<DataSourceBinder> GetTimeQueryList()
		{
			var list = new List<DataSourceBinder>
			{
				new DataSourceBinder{
					ValueField = "1",
					TextField = "申请时间",
					TextField_EN = "申请时间"
				},
				new DataSourceBinder{
					ValueField = "2",
					TextField = "创建时间",
					TextField_EN = "创建时间"
				},
			};
            return list;

		}

		/// <summary>
        /// 枚举值是否有效
        /// </summary>
		public static void ValidateIsValidTimeQuery(int value)
		{
			if(value==0 ||  !Enum.IsDefined(typeof(TimeQueryEnum),value) ){
			   throw new ArgumentOutOfRangeException(String.Format("value[{0}]",value), "不能转换成有效的枚举");
			}             
		}

    #endregion




        
    }

	public static class EubWayBillApplicationInfoExtensions
	{
		
		public static int GetStatusValue(this EubWayBillApplicationInfo.StatusEnum orderType)
        {
            return EubWayBillApplicationInfo.StatusToValue(orderType);
        }
public static int GetPrintFormatValue(this EubWayBillApplicationInfo.PrintFormatEnum orderType)
        {
            return EubWayBillApplicationInfo.PrintFormatToValue(orderType);
        }
public static int GetQueryNumberValue(this EubWayBillApplicationInfo.QueryNumberEnum orderType)
        {
            return EubWayBillApplicationInfo.QueryNumberToValue(orderType);
        }
public static int GetTimeQueryValue(this EubWayBillApplicationInfo.TimeQueryEnum orderType)
        {
            return EubWayBillApplicationInfo.TimeQueryToValue(orderType);
        }

	}
}
