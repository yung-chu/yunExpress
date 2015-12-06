using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;

namespace LMS.Services.PrintServices
{
    /// <summary>
    /// 该打印服务只供打印标签对外接口使用
    /// </summary>
    public interface IPrintService
    {
        /// <summary>
        /// 根据客户订单号和客户编码获取打印信息
        /// </summary>
        /// <param name="orderNumbers"></param>
        /// <param name="customerCode"></param>
        /// <returns></returns>
        List<LabelPrintModel> GetLabelPrint(List<string> orderNumbers, string customerCode, string templateTypeId);
    }
}
