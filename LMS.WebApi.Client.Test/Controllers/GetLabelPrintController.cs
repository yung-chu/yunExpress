using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using LMS.Data.Entity;
using LMS.WebAPI.Client;
using LMS.WebAPI.Client.Models;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Http;
using LighTake.Infrastructure.Http.Infrastructure;
using NUnit.Framework;

namespace LMS.WebApi.Client.Test.Controllers
{
    [TestFixture]
    public class GetLabelPrintController
    {
        HttpClient client;
        private string _baseAddress = string.Empty;
        [SetUp]
        public void Setup()
        {
            _baseAddress = ConfigurationManager.AppSettings["labelBaseUri"];
            client = new HttpClient();
        }
        [Test]
        public void GetLabel()
        {
            var list = new List<string>
                {
                    "SO13032000561",
                    "SO13030800408",
                    "SO13031000081",
                    "asdasdas0124",
                    "12123"
                };
            var model = new LabelPrintRequest();
            model.OrderNumber.AddRange(list);
            model.TypeId = "DT1308100021";
            //string s = JsonHelper.JsonSerializer(model);
            //s.ToString();
            var l = HttpHelper.DoRequestBasic<List<LabelPrintResponse>>(_baseAddress + "GetLabelPrint/GetLabelPrintUrl",
                                                          EnumHttpMethod.POST, "C48233", "mwV1weJ1QVY=",
                                                          EnumContentType.Json, model);
            Assert.IsTrue(l.Value.Count > 0);
        }
        [Test]
        public void GetStr()
        {
            string str = string.Empty;
            string s = "The c# string length 你 好吗？";
            var list = s.StringSplitLengthWords(10);
            if (list != null)
            {
                
            }
            Assert.IsTrue(s.ToDBC().Length == s.Length);
        }
        private List<string> StringSplitLengthWords(string s, int len)
        {
            var list = new List<string>();
            s = s.ToDBC();
            if (len <= 2)
            {
                return list;
            }
            if (s.Length <= len)
            {
                list.Add(s);
            }
            else
            {
                if (s.Substring(len - 1, 1) == " ")
                {
                    list.Add(s.Substring(0, len));
                    list.AddRange(StringSplitLengthWords(s.Substring(len),len));
                }
                else
                {
                    list.Add(s.Substring(0, s.Substring(0,len).LastIndexOf(' ')+1));
                    list.AddRange(StringSplitLengthWords(s.Substring(s.Substring(0, len).LastIndexOf(' ') + 1), len));
                }
            }
            return list;
        }
    }
    public class LabelPrintRequest
    {
        public LabelPrintRequest()
        {
            OrderNumber = new List<string>();
        }
        public List<string> OrderNumber { get; set; }

        public string TypeId { get; set; }
    }
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
    public class LabelPrintInfo
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
