using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace LighTake.Infrastructure.Seedwork
{
    /// <summary>
    /// 排序参数
    /// </summary>
    public class OrderByParam
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        public string OrderField { get; set; }

        /// <summary>
        /// 是否降序
        /// </summary>
        public bool Descending { get; set; }

        public TypeCode ValueType { get; set; }
    }
}
