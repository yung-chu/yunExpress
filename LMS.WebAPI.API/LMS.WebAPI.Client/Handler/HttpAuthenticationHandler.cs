using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dependencies;
using Autofac;
using Autofac.Integration.WebApi;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Services.CustomerServices;
using LMS.Services.FreightServices;
using LMS.WebAPI.Client.Helper;
using LMS.WebAPI.Client.Properties;
using LighTake.Infrastructure.Common;

namespace LMS.WebAPI.Client.Handler
{
    public class HttpAuthenticationHandler : DelegatingHandler
    {

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            bool _isValid = (GlobalConfig.IsValidCustomer == "true");
            var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
           
            if (_isValid)
            {
                var result = false;
                if (request.Headers.Authorization != null && request.Headers.Authorization.Parameter != null)
                {
                    try
                    {
                        string[] userInfo =
                            Encoding.UTF8.GetString(Convert.FromBase64String(request.Headers.Authorization.Parameter))
                                    .Split('&');
                        if (userInfo.Length >= 2)
                        {
                           var _customerService = GlobalConfiguration.Configuration.DependencyResolver.BeginScope().GetService(typeof(ICustomerService)) as CustomerService;
                           var customer= _customerService.GetCustomer(userInfo[0]);
                            if (customer != null && customer.ApiSecret == userInfo[1])
                            {
                                result = true;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        ;
                    }
                }
                if (!result)
                {
                    response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    {
                        Content = new StringContent(SerializeUtil.SerializeToXml(new Response<Item>()
                        {
                            ResultCode = ErrorCodeHelper.GetErrorCode(ErrorCode.Error1003),
                            ResultDesc = Resource.Error1003
                        }))
                    };
                    var tsc = new TaskCompletionSource<HttpResponseMessage>();
                    tsc.SetResult(response);
                    return tsc.Task;
                }

            }
            return base.SendAsync(request, cancellationToken);
        }



        //private bool ValidateApiKey(HttpRequestMessage request)
        //{
        //    bool isValid = (ConfigurationManager.AppSettings["IsValidCustomer"] == "true");
        //    if (isValid) return true;
        //    else
        //    {
        //        IEnumerable<string> mydata;

        //        if (request.Headers.TryGetValues("userKey", out mydata))
        //        {
        //            string key = mydata.First();
        //            //然后和config中的key做比较

        //            //客户端添加Header信息
        //            HttpClient httpClient = new HttpClient();
        //            httpClient.DefaultRequestHeaders.Add("userKey", "key值");
        //        }
        //    }
        //    return false;
        //    //var query = message.RequestUri.ParseQueryString();
        //    //string key = query["AppKey"];
        //    //return (AppKey == key);
        //}
    }
}