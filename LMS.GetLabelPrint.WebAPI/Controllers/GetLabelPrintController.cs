using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using LMS.Data.Entity;
using LMS.GetLabelPrint.WebAPI.Models;
using LMS.GetLabelPrint.WebAPI.UserCenter;
using LMS.Services.LabelPrintWebAPIServices;
using LighTake.Infrastructure.Http;

namespace LMS.GetLabelPrint.WebAPI.Controllers
{
    [HTTPBasicAuthorize]
    public class GetLabelPrintController : BaseApiController
    {
        private static readonly string httpUrl = System.Configuration.ConfigurationManager.AppSettings["LabelPrintHTTPURL"].ToString();
        private static readonly int maxpagecount = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["MaxPageCount"].ToString());
        private string _httpurlFormat = "{0}typeId={1}&type=0&ids=";
        private ILabelPrintWebApiService _labelPrintWebApiService;

        public GetLabelPrintController(ILabelPrintWebApiService labelPrintWebApiService)
        {
            _labelPrintWebApiService = labelPrintWebApiService;
        }
        [HttpPost]
        public List<LabelPrintResponse> GetLabelPrintUrl(LabelPrintRequest model)
        {
            var list = new List<LabelPrintResponse>();

            if (model != null && model.OrderNumber != null && model.OrderNumber.Any())
            {
                var labellist = _labelPrintWebApiService.GetLabelPrintExtList(model.OrderNumber,
                                                                              HttpContext.Current.User.Identity.Name);
                var cdictionary = new Dictionary<string, LabelPrintExt>();
                labellist.ForEach(p =>
                    {
                        if (!string.IsNullOrWhiteSpace(p.CustomerOrderNumber))
                        {
                            cdictionary.Add(p.CustomerOrderNumber, p);
                        }
                    });
                var tdictionary = new Dictionary<string, LabelPrintExt>();
                labellist.ForEach(p =>
                    {
                        if (!string.IsNullOrWhiteSpace(p.Trackingnumber))
                        {
                            tdictionary.Add(p.Trackingnumber, p);
                        }
                    });
                var wdictionary = new Dictionary<string, LabelPrintExt>();
                labellist.ForEach(p =>
                {
                    if (!string.IsNullOrWhiteSpace(p.WayBillNumber))
                    {
                        tdictionary.Add(p.WayBillNumber, p);
                    }
                });
                var dictionary = new Dictionary<int, LabelPrintInfo>();
                var multiDictionary = new List<int>();
                var customerOrderIds = new List<int>();
                var nullOrderIds = new List<string>();
                model.OrderNumber.ForEach(p =>
                    {
                        customerOrderIds.Clear();
                        if (cdictionary.ContainsKey(p))
                        {
                            if (!dictionary.ContainsKey(cdictionary[p].CustomerOrderId))
                            {
                                dictionary.Add(cdictionary[p].CustomerOrderId, new LabelPrintInfo()
                                    {
                                        CustomerOrderId = cdictionary[p].CustomerOrderId,
                                        CustomerOrderNumber = cdictionary[p].CustomerOrderNumber,
                                        OriginalNumber = p,
                                        TrackingNumber = cdictionary[p].Trackingnumber,
                                        WayBillNumber = cdictionary[p].WayBillNumber
                                    });
                                customerOrderIds.Add(cdictionary[p].CustomerOrderId);
                            }
                        }
                        if (tdictionary.ContainsKey(p))
                        {
                            if (!dictionary.ContainsKey(tdictionary[p].CustomerOrderId))
                            {
                                dictionary.Add(tdictionary[p].CustomerOrderId, new LabelPrintInfo()
                                    {
                                        CustomerOrderNumber = tdictionary[p].CustomerOrderNumber,
                                        CustomerOrderId = tdictionary[p].CustomerOrderId,
                                        OriginalNumber = p,
                                        TrackingNumber = tdictionary[p].Trackingnumber,
                                        WayBillNumber = tdictionary[p].WayBillNumber
                                    });
                                customerOrderIds.Add(tdictionary[p].CustomerOrderId);
                            }
                        }
                        if (wdictionary.ContainsKey(p))
                        {
                            if (!dictionary.ContainsKey(wdictionary[p].CustomerOrderId))
                            {
                                dictionary.Add(wdictionary[p].CustomerOrderId, new LabelPrintInfo()
                                    {
                                        CustomerOrderId = wdictionary[p].CustomerOrderId,
                                        CustomerOrderNumber = wdictionary[p].CustomerOrderNumber,
                                        OriginalNumber = p,
                                        TrackingNumber = wdictionary[p].Trackingnumber,
                                        WayBillNumber = wdictionary[p].WayBillNumber
                                    });
                                customerOrderIds.Add(wdictionary[p].CustomerOrderId);
                            }
                        }
                        if (customerOrderIds.Count > 1)
                        {
                            if (cdictionary.ContainsKey(p))
                                multiDictionary.Add(cdictionary[p].CustomerOrderId);
                            if (tdictionary.ContainsKey(p))
                                multiDictionary.Add(tdictionary[p].CustomerOrderId);
                            if (wdictionary.ContainsKey(p))
                                multiDictionary.Add(wdictionary[p].CustomerOrderId);
                        }
                        else if (customerOrderIds.Count == 0)
                        {
                            nullOrderIds.Add(p);
                        }
                    });
                if (dictionary.Any())
                {
                    int i = 1;
                    var lpr = new LabelPrintResponse {Url = string.Format(_httpurlFormat, httpUrl, model.TypeId)};
                    foreach (var d in dictionary)
                    {
                        lpr.Url = lpr.Url + d.Key.ToString() + ",";
                        if (multiDictionary.Contains(d.Key))
                        {
                            lpr.LabelPrintInfos.Add(new LabelPrintInfo()
                                {
                                    CustomerOrderId = d.Value.CustomerOrderId,
                                    CustomerOrderNumber = d.Value.CustomerOrderNumber,
                                    ErrorBody = string.Format("单号{0}存在多条数据", d.Value.OriginalNumber),
                                    ErrorCode = 200,
                                    OriginalNumber = d.Value.OriginalNumber,
                                    Status = true,
                                    TrackingNumber = d.Value.TrackingNumber,
                                    WayBillNumber = d.Value.WayBillNumber
                                });
                        }
                        else
                        {
                            lpr.LabelPrintInfos.Add(new LabelPrintInfo()
                                {
                                    CustomerOrderId = d.Value.CustomerOrderId,
                                    CustomerOrderNumber = d.Value.CustomerOrderNumber,
                                    ErrorBody = "",
                                    ErrorCode = 300,
                                    OriginalNumber = d.Value.OriginalNumber,
                                    Status = true,
                                    TrackingNumber = d.Value.TrackingNumber,
                                    WayBillNumber = d.Value.WayBillNumber
                                });
                        }
                        if (i == maxpagecount)
                        {
                            list.Add(lpr);
                            lpr = new LabelPrintResponse {Url = string.Format(_httpurlFormat, httpUrl, model.TypeId)};
                            i = 0;
                        }
                        i++;
                    }
                    list.Add(lpr);
                }
                if (nullOrderIds.Any())
                {
                    var lpr = new LabelPrintResponse();
                    nullOrderIds.ForEach(p => lpr.LabelPrintInfos.Add(new LabelPrintInfo()
                        {
                            OriginalNumber = p,
                            Status = false,
                            ErrorCode = 100,
                            ErrorBody = string.Format("单号{0}不存在",p)
                        }));
                    list.Add(lpr);
                }
            }

            return list;
        }
    }
}
