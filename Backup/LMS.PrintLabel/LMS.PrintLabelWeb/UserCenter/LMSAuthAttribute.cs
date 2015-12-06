using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using LMS.Services.CustomerServices;
using LighTake.Infrastructure.Common.InversionOfControl;

namespace LMS.PrintLabelWeb.UserCenter
{
    public class LMSAuthAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public const string AuthorizationHeaderName = "Authorization";
        public const string WwwAuthenticationHeaderName = "WWW-Authenticate";
        public const string BasicAuthenticationScheme = "Basic";
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            else
            {
                AuthenticationHeaderValue token = this.GetAuthenticationHeaderValue(filterContext);
                if (null != token && token.Scheme == BasicAuthenticationScheme)
                {
                    string[] parsedHeader = ParseAuthorizationHeader(token.Parameter);
                    if (parsedHeader != null)
                    {
                        IPrincipal principal = null;
                        if (TryCreatePrincipal(parsedHeader[0], parsedHeader[1], out principal))
                        {
                            HttpContext.Current.User = principal;
                        }
                        else
                        {
                            HandleUnauthorizedRequest(filterContext);
                        }
                    }
                    else
                    {
                        HandleUnauthorizedRequest(filterContext);
                    }
                }
                else
                {
                    HandleUnauthorizedRequest(filterContext);
                }
            }
        }
        protected virtual AuthenticationHeaderValue GetAuthenticationHeaderValue(AuthorizationContext filterContext)
        {
            string rawValue = filterContext.RequestContext.HttpContext.Request.Headers[AuthorizationHeaderName];
            if (string.IsNullOrEmpty(rawValue))
            {
                return null;
            }
            string[] split = rawValue.Split(' ');
            if (split.Length != 2)
            {
                return null;
            }
            return new AuthenticationHeaderValue(split[0], split[1]);
        }
        private bool TryCreatePrincipal(string customerCode, string apiSecret, out IPrincipal principal)
        {
            var customerService = EngineContext.Current.Resolve<ICustomerService>() as CustomerService;
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
            string[] credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader)).Split(new[] { '&', ':' });
            if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[1]))
                return null;
            return credentials;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext actionContext)
        {
            string parameter = string.Format("realm=\"{0}\"", actionContext.RequestContext.HttpContext.Request.Url.DnsSafeHost);
            var challenge = new AuthenticationHeaderValue(BasicAuthenticationScheme, parameter);
            actionContext.HttpContext.Response.Headers[WwwAuthenticationHeaderName] = challenge.ToString();
            actionContext.Result = new HttpUnauthorizedResult();
        }
    }
}