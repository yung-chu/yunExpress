using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity
{
    /// <summary>
    /// 入仓进度
    /// </summary>
    public class InStorageProcess
    {
        /// <summary>
        /// 入仓单号
        /// </summary>
        public string InStorageID { get; set; }
        /// <summary>
        /// 入仓的运单数据量
        /// </summary>
        public int? Total { get; set; }
        /// <summary>
        /// 已经完成入仓操作的数量
        /// </summary>
        public int? Completed { get; set; }
    }
}
