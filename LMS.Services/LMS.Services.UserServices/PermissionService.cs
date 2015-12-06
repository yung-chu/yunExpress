using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Caching;
using LMS.Data.Entity;
using LMS.Services.UserServices.PMSServiceReference;

namespace LMS.Services.UserServices
{
    public class PermissionService : IPermissionService
    {
        protected string SystemCode = Config.GetSystemCode();
        protected const string PERMISSIONS_BY_SYSTEMCODE_KEY = "permissions.by.systemcode-{0}";

        public IList<Permission> GetAllPermission()
        {
            string key = string.Format(PERMISSIONS_BY_SYSTEMCODE_KEY, SystemCode);
            return Cache.Get(key, () =>
               {
                   IList<Permission> lstResult = new List<Permission>();
                   WCFExtension.Using(new PMSServiceClient(), serviceClient =>
                                                                  {
                                                                      PermissionsResponse response = serviceClient.GetPermissionsBySystemCode(SystemCode);
                                                                      if (response.Success)
                                                                      {
                                                                          response.Permissions.Each(p => lstResult.Add(p.ToLocal()));
                                                                      }
                                                                      else
                                                                      {
                                                                          throw new Exception(response.Message);
                                                                      }
                                                                  });
                   return lstResult;
               });
        }

        public void RefreshSystemPermissionCache()
        {
            IList<Permission> lstResult = new List<Permission>();
            WCFExtension.Using(new PMSServiceClient(), serviceClient =>
                                                           {
                                                               PermissionsResponse response = serviceClient.GetPermissionsBySystemCode(SystemCode);
                                                               if (response.Success)
                                                               {
                                                                   response.Permissions.Each(p => lstResult.Add(p.ToLocal()));
                                                               }
                                                               else
                                                               {
                                                                   throw new Exception(response.Message);
                                                               }
                                                           });
            string key = string.Format(PERMISSIONS_BY_SYSTEMCODE_KEY, SystemCode);
            Cache.Add(key, lstResult);
        }

        public bool IsExistPermissionByUrl(string url)
        {
            IList<Permission> lstPermission = GetAllPermission();
            return lstPermission.Count(p => !p.NavigateUrl.IsNullOrWhiteSpace() && p.NavigateUrl.Trim().ToLowerInvariant().Equals(url.Trim().ToLowerInvariant())) > 0;
        }
    }
}
