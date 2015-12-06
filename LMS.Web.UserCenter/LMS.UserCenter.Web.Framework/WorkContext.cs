using System;
using System.Collections.Generic;
using System.Diagnostics;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Services.CustomerServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.InversionOfControl;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Web;

namespace LMS.UserCenter.Web.Framework
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
                var currentCustomer = GetAuthenticatedCustomer();
                if (currentCustomer != null)
                    _cachedUser = new User { UserUame = currentCustomer.CustomerCode, RealName = currentCustomer.Name, Email = currentCustomer.Email };
                return _cachedUser;
            }
            return _cachedUser;
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

        private Customer _cacheCustomer;
        private Customer GetAuthenticatedCustomer()
        {
            if (_cacheCustomer != null)
                return _cacheCustomer;

            UserData userData = EngineContext.Current.Resolve<IAuthenticationService>().GetAuthenticatedUserData();

            
            if (userData == null || string.IsNullOrEmpty(userData.UserName))
            {
                Log.Info("userData为空！");
                return null;
            }

            var customer = EngineContext.Current.Resolve<ICustomerService>().GetCustomer(userData.UserName.Trim());
            if (customer == null)
                return null;

            _cacheCustomer = customer;

            return customer;
        }

        public List<DataSourceBinder> BusinessModelList
        {
            get { throw new NotImplementedException(); }
        }
        
    }
}
