namespace LighTake.Infrastructure.Common.BizLogging
{
    using System;   
    using LighTake.Infrastructure.Common.BizLogging.Enums;
   
    /// <summary>
    /// 业务日志
    /// Daniel
    /// v1.0 2014-8-25
    /// 
    /// </summary>
    public class BizLog
    {   

        /// <summary>
        /// 关键词，如客户订单号 , 运单号 , 请注意对应KeywordType类型
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// 关键词 类型
        /// </summary>
        public KeywordType KeywordType { get; set; }
       


        /// <summary>
        /// 用户编号,所在系统的用户编号或者编码,注意和UserType对应起来
        /// </summary>       
        public string UserCode { get; set; }  
        /// <summary>
        /// 用户真实名称,或者名称,越真实越好
        /// </summary>
        public string UserRealName { get; set; }
        /// <summary>
        /// 用户类型
        /// </summary>
        public UserType UserType { get; set; }




        /// <summary>
        /// [必填]系统类型,(如果没有请自己添加编码)
        /// </summary>
		public SystemType SystemCode { get; set; }   
        /// <summary>
        /// [必填]模块名称
        /// </summary>
        public string ModuleName { get; set; }
        



        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }    
        /// <summary>
        /// 网卡MAC地址
        /// </summary>
        public string Mac { get; set; }   
        /// <summary>
        ///相对URL
        /// </summary>
        public string URL { get; set; }
        

        /// <summary>
        /// [必填]操作的动作简要说明,如：修改运输方式
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// [必填]数据实体Json序列化之后的字符串 (基本原则什么数据变了就存什么) , 如：新的运输方式实体序列化的Json
        /// </summary>
        public string Details { get; set; }
    }
}
