using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.InversionOfControl;

namespace LighTake.Infrastructure.Web.Filters
{
    public class IgnoredActionMethod
    {
        public string Controller { get; set; }

        public string Action { get; set; }
    }

    public class MemberOnlyAttribute : AuthorizeAttribute
    {
        protected readonly List<IgnoredActionMethod> _ignoredActionMethods = new List<IgnoredActionMethod>();
        //private readonly ICurrentUser _currentUser;
        protected IAuthenticationService _authenticationService;

        public MemberOnlyAttribute()
            : this(null)
        {
        }

        public MemberOnlyAttribute(IEnumerable<IgnoredActionMethod> ignoredActionMethods)
        {
            if (ignoredActionMethods != null)
            {
                _ignoredActionMethods.AddRange(ignoredActionMethods.ToList());
            }
            _authenticationService = EngineContext.Current.Resolve<IAuthenticationService>();
            //_currentUser = EngineContext.Current.Resolve<ICurrentUser>();
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var controller = filterContext.Controller;
            if (controller == null)
                return;

            //有配置不需过滤的Action
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

            try
            {
                if (_authenticationService.GetAuthenticatedUserData() == null)
                    filterContext.Result = LoginView(filterContext);
            }
            catch (Exception)
            {
                filterContext.Result = LoginView(filterContext);
            }

            //if (!HttpContext.Current.Request.IsAuthenticated)
            //{
            //    filterContext.Result = LoginView(filterContext);
            //}
            //if (!HttpContext.Current.Request.IsAuthenticated)
            //{
            //    if (_scmConfig.RunModel.Value == RunModelEnum.Develop)
            //    {
            //        _AuthenticationService.SignIn(new UserData
            //        {
            //            UserName = _scmConfig.RunModel.DevelopModel.UserName
            //        }, false);

            //        filterContext.Result = new RedirectResult(filterContext.RequestContext.HttpContext.Request.RawUrl);
            //    }
            //    else
            //    {
            //        filterContext.Result = LoginView();
            //    }
            //}
        }

        protected ActionResult LoginView(ControllerContext context)
        {
            return new RedirectToRouteResult("Login", new RouteValueDictionary
                {
                    {"ReturnUrl",context.HttpContext.Request.Url.ToString()}
                });
        }
    }
}
