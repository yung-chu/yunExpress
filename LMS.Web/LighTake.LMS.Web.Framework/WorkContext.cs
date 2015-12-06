using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Services.UserServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.InversionOfControl;
using LighTake.Infrastructure.Web;

namespace LighTake.LMS.Web.Framework
{
    public class WorkContext : IWorkContext
    {
        private User _cachedUser;

        public User User
        {
            get { return GetCurrentUser(); }
            set { _cachedUser = value; }
        }

        protected User GetCurrentUser()
        {
            if (_cachedUser == null)
            {
                return EngineContext.Current.Resolve<IAuthenticationService>().GetAuthenticatedUser(
                                        EngineContext.Current.Resolve<IUserService>());
            }
            return null;
        }

        public Guid LoginId
        {
            get { return Guid.Empty; }
        }

        public string LoginName
        {
            get { return User.UserUame; }
        }

        public string Email
        {
            get { return User.Email; }
        }


        public List<DataSourceBinder> BusinessModelList
        {
            get { throw new NotImplementedException(); }
        }

        
    }
}
