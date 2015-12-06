using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using LMS.Data.Entity;
using LMS.PrintLabelAPI.Models;
using LMS.PrintLabelAPI.UserCenter;
using LMS.Services.PrintServices;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.PrintLabelAPI.Controllers
{
    [LMSAPIAuth]
    public class PrintUrlController : ApiController
    {
        private static readonly string httpUrl =
            System.Configuration.ConfigurationManager.AppSettings["LabelPrintHTTPURL"];

        private static readonly string templateTypeId =
            System.Configuration.ConfigurationManager.AppSettings["AddressLabelTemplateTypeId"];

        private readonly IPrintService _printService;
        public PrintUrlController(IPrintService printService)
        {
            _printService = printService;
        }
        /// <summary>
        /// 通过客户订单号查询地址标签打印地址
        /// </summary>
        /// <param name="orderNumbers">客户订单号集合</param>
        /// <returns></returns>
        [HttpPost]
        public Response<List<LabelPrintResponse>> GetLabelPrintUrl(List<string> orderNumbers)
        {
            var response = new Response<List<LabelPrintResponse>>();
            var list = new List<LabelPrintResponse>();
            try
            {
                if (orderNumbers.Any())
                {
                    var dictionary = new Dictionary<int, List<LabelPrintModel>>();
                    _printService.GetLabelPrint(orderNumbers.Distinct().ToList(), HttpContext.Current.User.Identity.Name,
                                                            templateTypeId).ForEach(p =>
                                                            {
                                                                if (dictionary.ContainsKey(p.ShippingMethodId))
                                                                {
                                                                    dictionary[p.ShippingMethodId].Add(p);
                                                                }
                                                                else
                                                                {
                                                                    dictionary.Add(p.ShippingMethodId, new List<LabelPrintModel> { p });
                                                                }
                                                            });
                    foreach (var d in dictionary)
                    {
                        if (d.Key == 0)
                        {
                            var lp = new LabelPrintResponse() { Url = "" };
                            d.Value.ForEach(p => lp.LabelPrintInfos.Add(new LabelPrintInfo()
                            {
                                ErrorBody = "不存在打印模板",
                                ErrorCode = 200,
                                OrderNumber = p.OrderNumber
                            }));
                            list.Add(lp);
                        }
                        else
                        {
                            if (d.Value[0].IsHavePrint)
                            {
                                const int pagesize = 20;//分批查询
                                int pageindex = 1;
                                do
                                {
                                    var l = d.Value.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                                    var lp = new LabelPrintResponse() { Url = httpUrl + string.Join(",", l.Select(p => p.OrderNumber)) };
                                    l.ForEach(p => lp.LabelPrintInfos.Add(new LabelPrintInfo()
                                    {
                                        ErrorBody = "",
                                        ErrorCode = 100,
                                        OrderNumber = p.OrderNumber
                                    }));
                                    list.Add(lp);
                                    pageindex++;
                                } while (d.Value.Count > (pageindex - 1) * pagesize);
                            }
                            else
                            {
                                var lp = new LabelPrintResponse() { Url = "" };
                                d.Value.ForEach(p => lp.LabelPrintInfos.Add(new LabelPrintInfo()
                                {
                                    ErrorBody = "不存在打印模板",
                                    ErrorCode = 200,
                                    OrderNumber = p.OrderNumber
                                }));
                                list.Add(lp);
                            }
                        }
                    }
                }
                response.Item = list;
                response.ResultCode = "0000";
                response.ResultDesc = "请求成功";
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                response.Item = list;
                response.ResultCode = "9999";
                response.ResultDesc = "接口异常";
            }
            
            return response;
        }
    }
}
