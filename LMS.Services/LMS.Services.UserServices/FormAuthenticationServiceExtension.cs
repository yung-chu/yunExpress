using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Web;
using LMS.Data.Entity;

namespace LMS.Services.UserServices
{
    public static class FormAuthenticationServiceExtension
    {
        public static User GetAuthenticatedUser(this IAuthenticationService authenticationService, IUserService userService)
        {
            UserData userData = authenticationService.GetAuthenticatedUserData();
            if (userData != null)
            { return userService.GetUserByUsername(userData.UserName); }

            return null;
        }

        public static UserData GetAuthenticatedUserData(this IAuthenticationService authenticationService)
        {
            return authenticationService.GetAuthenticatedUserData();
        }
    }
}
