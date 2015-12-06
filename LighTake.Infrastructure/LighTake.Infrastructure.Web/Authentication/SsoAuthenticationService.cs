using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SsoFramework;

namespace LighTake.Infrastructure.Web
{
  public  class SsoAuthenticationService : IAuthenticationService
    {
        public void SignIn(UserData userData, bool createPersistentCookie)
        {

        }

        public void SignOut()
        {

        }

        public UserData GetAuthenticatedUserData()
        {
            var ssoTicket =SsoEngine.GetAuthenticatedTicket();

            if (ssoTicket == null) return null;

            return new UserData
                {
                    UserName = ssoTicket.UserId
                };
        }
    }
}
