using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Services.UserServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.InversionOfControl;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Web;

namespace LMS.WebAPI
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
                var auth = GlobalConfiguration.Configuration.DependencyResolver.BeginScope()
                                   .GetService(typeof(IAuthenticationService)) as IAuthenticationService;
                var userService = GlobalConfiguration.Configuration.DependencyResolver.BeginScope()
                                   .GetService(typeof(IUserService)) as IUserService;
                return auth.GetAuthenticatedUser(userService);
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