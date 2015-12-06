using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.WCF.TrackNumber.Dto
{
    /// <summary>
    /// 接口返回数据结构
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResutlInfo<T>
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 返回错误原因
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public List<T> Data { get; set; }
    }
}
