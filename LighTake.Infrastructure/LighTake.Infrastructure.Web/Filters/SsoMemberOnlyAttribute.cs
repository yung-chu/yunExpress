using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using LighTake.Infrastructure.Common.Caching;
using LighTake.Infrastructure.Web.Controllers;
using SsoFramework;

namespace LighTake.Infrastructure.Web.Filters
{
    public class SsoMemberOnlyAttribute : MemberOnlyAttribute
    {
        public SsoMemberOnlyAttribute(IEnumerable<IgnoredActionMethod> ignoredActionMethods)
            : base(ignoredActionMethods)
        {
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //base.OnAuthorization(filterContext);
            //if (filterContext.Result != null)//站点没有登录

            #region 检查不需要权限认证的路径 zxq

            var controller = filterContext.Controller;
            if (controller == null) return;

            string strActualController = filterContext.RouteData.Values["controller"] == null
                                        ? string.Empty
                                        : filterContext.RouteData.Values["controller"].ToString().Trim().ToLowerInvariant();

            string strActualAction = filterContext.RouteData.Values["action"] == null
                                        ? string.Empty
                                        : filterContext.RouteData.Values["action"].ToString().Trim().ToLowerInvariant(); ;

            if (_ignoredActionMethods.Any(p => p.Controller.Trim().ToLowerInvariant() == strActualController &&
                                          p.Action.Trim().ToLowerInvariant() == strActualAction))
            {
                return;
            }

            if (_ignoredActionMethods.Any(p => p.Controller.Trim().ToLowerInvariant() == strActualController &&
                              p.Action.Trim().ToLowerInvariant() == "*"))
            {
                return;
            }

            #endregion

            {
                if (SsoEngine.IsAuthenticated)
                {
                    var ticket = SsoEngine.GetAuthenticatedTicket();
                    string newValue = ticket.UserId.Trim().ToLower() + "_" + ticket.TimeStamp.ToString("yyyyMMddHHmmss");

                    string oldValue = PersonalCache.Instance["UserLoginDateTime"] as string;
                    if (string.IsNullOrEmpty(oldValue))
                    {
                        PersonalCache.Instance["UserLoginDateTime"] = newValue;
                    }
                    else
                    {
                        if (!oldValue.Equals(newValue, StringComparison.InvariantCultureIgnoreCase))
                        {
                            Cache.Flush();
                            PersonalCache.Instance["UserLoginDateTime"] = newValue;
                        }
                    }

                    //var ticket = SsoEngine.GetAuthenticatedTicket();
                    //_authenticationService.SignIn(new UserData { UserName = ticket.UserId }, false);

                    //filterContext.Result = new RedirectResult(filterContext.RequestContext.HttpContext.Request.RawUrl);
                }
                else if (SsoEngine.IsAuthenticating)
                {
                    ResponseToken token;
                    if (SsoEngine.Authenticate(out token))
                    {
                        filterContext.Result = new RedirectResult(SsoEngine.GetRedirectToSetTicketUrl());
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult(SsoEngine.GetRedirectToLogonUrl());
                    }
                }
                else
                {
                    filterContext.Result = new RedirectResult(SsoEngine.GetRedirectToLogonUrl());
                }
            }
        }
    }
}
