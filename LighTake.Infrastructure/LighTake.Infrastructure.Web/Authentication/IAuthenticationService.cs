using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LighTake.Infrastructure.Web
{
    public interface IAuthenticationService
    {
        void SignIn(UserData userData, bool createPersistentCookie);

        void SignOut();

        UserData GetAuthenticatedUserData();
    }
}
