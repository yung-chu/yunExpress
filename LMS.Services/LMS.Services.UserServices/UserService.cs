using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Common.Caching;
using LighTake.Infrastructure.Web.Utities;
using LMS.Data.Entity;
using LMS.Services.UserServices.PMSServiceReference;
using LighTake.Infrastructure.Common;

namespace LMS.Services.UserServices
{
    public class UserService : IUserService
    {
        protected string SystemCode = Config.GetSystemCode();
        protected const string USER_BY_USERNAME_KEY = "user.by.username-{0}";
        protected readonly IPermissionService _permissionService;

        public UserService(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public User ValidateUser(string loginName, string password, out string msg)
        {
            if (!string.IsNullOrEmpty(loginName))
            {
                loginName = loginName.Trim();
            }


            msg = string.Empty;

            string strMsg = string.Empty;
            try
            {
                User result = null;
                WCFExtension.Using(new PMSServiceClient(), clientService =>
                                                               {
                                                                   UserLoginResponse response = clientService.UserLogin(new UserLoginInfo
                                                                   {
                                                                       UserName = loginName,
                                                                       Password = password.ToMD5(),
                                                                       SystemCode = SystemCode
                                                                   });
                                                                   if (response.Success)
                                                                   {
                                                                       result = response.UserInfo.ToLocal();
                                                                   }
                                                                   else
                                                                   {
                                                                       strMsg = response.Message;
                                                                   }
                                                               });
                msg = strMsg;

                if (result != null)
                {
                    string key = string.Format(USER_BY_USERNAME_KEY, result.UserUame.Trim().ToLowerInvariant());
                    Cache.Add(key, result);
                    _permissionService.RefreshSystemPermissionCache();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("UserService.ValidateUser", ex);
            }
        }

        public User GetUserByUsername(string userName)
        {
            string key = string.Format(USER_BY_USERNAME_KEY, userName.Trim().ToLowerInvariant());
            return Cache.Get(key, () =>
            {
                User user = null;
                WCFExtension.Using(new PMSServiceClient(), serviceClient =>
                {
                    user = serviceClient.GetUserInfo(userName, SystemCode).ToLocal();
                });
                return user;
            });
        }

        public bool Authorize(string userName, string virtualUrl)
        {
            User user = GetUserByUsername(userName);
            virtualUrl = virtualUrl.ToLowerInvariant();
            if (virtualUrl.EndsWith("/"))
            {
                virtualUrl = virtualUrl.TrimEnd('/');
            }
            return user.Permissions.Count(p => !string.IsNullOrEmpty(p.NavigateUrl) && p.NavigateUrl.Trim().ToLowerInvariant().Equals(virtualUrl.Trim().ToLowerInvariant())) > 0;
        }

        public bool Authorize(string userName, int moduleCode)
        {
            return GetUserByUsername(userName).Permissions.Count(p => p.PermissionCode != null && p.PermissionCode.Trim() == moduleCode.ToString()) > 0;
        }
    }
}
