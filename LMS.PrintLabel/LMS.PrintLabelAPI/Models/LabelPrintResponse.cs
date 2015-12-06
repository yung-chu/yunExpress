using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.PrintLabelAPI.Models
{
    public class LabelPrintResponse
    {
        public LabelPrintResponse()
        {
            LabelPrintInfos = new List<LabelPrintInfo>();
        }
        //打印URL
        public string Url { get; set; }
        //订单详细信息
        public List<LabelPrintInfo> LabelPrintInfos { get; set; }
        
    }
     public class  LabelPrintInfo
     {
         //客户订单号
         public string OrderNumber { get; set; }
         //错误码
         public int ErrorCode { get; set; }
         //错误信息
         public string ErrorBody { get; set; }
     }
}