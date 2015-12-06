using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using LMS.Services.CustomerServices;

namespace LMS.GetLabelPrint.WebAPI.UserCenter
{
    public class HTTPBasicAuthorizeAttribute : AuthorizeAttribute
    {
        //public const string AuthorizationHeaderName = "Authorization";
        //public const string WwwAuthenticationHeaderName = "WWW-Authenticate";
        //public const string BasicAuthenticationScheme = "Basic";
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization != null)
            {
                string[] parsedHeader = ParseAuthorizationHeader(actionContext.Request.Headers.Authorization.Parameter);
                if (parsedHeader != null)
                {
                    IPrincipal principal = null;
                    if (TryCreatePrincipal(parsedHeader[0], parsedHeader[1], out principal))
                    {
                        HttpContext.Current.User = principal;
                        //IsAuthorized(actionContext);
                    }
                    else
                    {
                        HandleUnauthorizedRequest(actionContext);
                    }
                }
                else
                {
                    HandleUnauthorizedRequest(actionContext);
                }
            }
            else
            {
                HandleUnauthorizedRequest(actionContext);
            }
        }

        private bool TryCreatePrincipal(string customerCode, string apiSecret, out IPrincipal principal)
        {
            var customerService =
                GlobalConfiguration.Configuration.DependencyResolver.BeginScope().GetService(typeof(ICustomerService))
                as CustomerService;
            if (customerService != null)
            {
                var customer = customerService.GetCustomer(customerCode);
                if (customer != null && customer.ApiSecret == apiSecret)
                {
                    var gi = new GenericIdentity(customerCode);
                    Thread.CurrentPrincipal = new GenericPrincipal(gi, null);
                    principal = Thread.CurrentPrincipal;
                    return true;
                }
            }
            principal = null;
            return false;
        }
        private string[] ParseAuthorizationHeader(string authHeader)
        {
            string[] credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader)).Split(new[] { '&' });
            if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[1]))
                return null;
            return credentials;
        }
        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var challengeMessage = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            challengeMessage.Headers.Add("WWW-Authenticate", "Basic");
            throw new System.Web.Http.HttpResponseException(challengeMessage);
        }

    }
}