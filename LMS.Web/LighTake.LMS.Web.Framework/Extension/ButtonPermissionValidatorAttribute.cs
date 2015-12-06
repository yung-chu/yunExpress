using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using LMS.Core;
using LMS.Services.UserServices;
using LighTake.Infrastructure.Common.InversionOfControl;
using LighTake.Infrastructure.Web.Filters;


namespace LighTake.Infrastructure.Web
{
    public class ButtonPermissionValidatorAttribute : MemberOnlyAttribute
    {
        private readonly int _permissionCode;
        private readonly IUserService _userService;
        private readonly IWorkContext _currentUser;

        public ButtonPermissionValidatorAttribute(int permissionCode)
        {
            _permissionCode = permissionCode;
            _userService = EngineContext.Current.Resolve<IUserService>();
            _currentUser = EngineContext.Current.Resolve<IWorkContext>();
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            //没有登录
            if (filterContext.Result != null)
            { return; }

            if (!_userService.Authorize(_currentUser.User.UserUame, _permissionCode))
            {
                filterContext.Result = AccessDeniedView(filterContext);
            }
        }

        protected ActionResult AccessDeniedView(AuthorizationContext filterContext)
        {
            return new RedirectToRouteResult("AccessDenied", null);
        }
    }
}