using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Web
{
    public class FakeAuthenticationService : IAuthenticationService
    {
        public void SignIn(UserData userData, bool createPersistentCookie)
        {
           
        }

        public void SignOut()
        {

        }

        public UserData GetAuthenticatedUserData()
        {
            return new UserData
                       {
                           UserName = "Leo"
                       };
        }
    }
}
