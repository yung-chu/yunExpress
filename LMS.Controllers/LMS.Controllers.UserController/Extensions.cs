using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using LighTake.Infrastructure.Web;
using LighTake.Infrastructure.Web.Utities;
using LMS.Core;

namespace LMS.Controllers.UserController
{
    public static class Extensions
    {
        public static string GetLoginImageCode(this IWorkContext workContext)
        {
            string strCode = HttpContext.Current.Session["LoginImageCode"] as string;
            return strCode ?? string.Empty;
        }

        public static void RemoveLoginImageCode(this IAuthenticationService workContext)
        {
            var obj = HttpContext.Current.Session["LoginImageCode"];
            if (obj != null)
            {
                HttpContext.Current.Session.Remove("LoginImageCode");
            }
        }

        public static void SetLoginErrorCount(this IAuthenticationService workContext, int count)
        {
            WebTools.WriteCookie(Cookies.LoginErrorCount, (count < 0 ? 0 : count).ToString());
        }

        public static int GetLoginErrorCount(this IAuthenticationService workContext)
        {
            int intLoginErrorCount;
            int.TryParse(WebTools.GetCookie(Cookies.LoginErrorCount), out intLoginErrorCount);
            return intLoginErrorCount;
        }

        public static void IncreaseLoginErrorCount(this IAuthenticationService workContext)
        {
            workContext.SetLoginErrorCount(workContext.GetLoginErrorCount() + 1);
        }

        public static void RemoveLoginErrorCount(this IAuthenticationService workContext)
        {
            workContext.SetLoginErrorCount(0);
            WebTools.DelCookie(Cookies.LoginErrorCount);
        }
    }
}
