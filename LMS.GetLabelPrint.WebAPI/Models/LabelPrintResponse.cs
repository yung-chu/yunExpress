using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.GetLabelPrint.WebAPI.Models
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
        //返回状态
        public bool Status { get; set; }
        //客户订单号ID
        public int CustomerOrderId { get; set; }
        //原始单号
        public string OriginalNumber { get; set; }
        //运单号
        public string WayBillNumber { get; set; }
        //跟踪号
        public string TrackingNumber { get; set; }
        //客户订单号
        public string CustomerOrderNumber { get; set; }
        //错误代码 100-单号不存在，200-单号获取多条数据,300-正确
        public int ErrorCode { get; set; }
        //错误信息
        public string ErrorBody { get; set; }
    }
}