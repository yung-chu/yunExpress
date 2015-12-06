using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using LMS.Data.Entity;
using LMS.Services.CustomerServices;
using LMS.WebAPI.Client.Handler;
using LMS.WebAPI.Client.Helper;
using LighTake.Infrastructure.Http;

namespace LMS.WebAPI.Client
{
    public class ApiControllerBase : BaseApiController
    {

        #region 属性

        private string _customerCode;
        /// <summary>
        /// 客户代码
        /// </summary>
        public string CustomerCode
        {
            get { return _customerCode; }
        }
      
        private DateTime _LastRequstTime;
        /// <summary>
        /// 上一次请求的时间
        /// </summary>
        public DateTime LastRequstTime
        {
            get { return _LastRequstTime; }
        }

        #endregion

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            var request = controllerContext.Request;

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
                        var customer = _customerService.GetCustomer(userInfo[0]);
                        if (customer != null && customer.ApiSecret == userInfo[1])
                        {
                            _customerCode = customer.CustomerCode;
                        }
                    }
                }
                catch (Exception)
                {
                    ;
                }
            }
            //GetRequestHeaderValue(request.Headers);
        }

        public void GetRequestHeaderValue(HttpHeaders requestHeaders)
        {
            if (requestHeaders.Contains("CustomerCode"))
            {
                IEnumerable<string> codeHeader;
                requestHeaders.TryGetValues("CustomerCode", out codeHeader);
                string code = codeHeader.FirstOrDefault();
                _customerCode = code;
            }
            else
            {
                _customerCode = GlobalConfig.CustomerCode;
            }

        }

        private Task<HttpResponseMessage> SendError(string error, HttpStatusCode code)
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent(error);
            response.StatusCode = code;
            return Task<HttpResponseMessage>.Factory.StartNew(() => response);
        }

        public string GetErrorCode(ErrorCode errorCode)
        {
            return (int)errorCode == 0 ? "0000" : ((int)errorCode).ToString();
        }
    }
}