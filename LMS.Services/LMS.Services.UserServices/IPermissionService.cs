using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;

namespace LMS.Services.UserServices
{
    public interface IPermissionService
    {
        IList<Permission> GetAllPermission();

        bool IsExistPermissionByUrl(string url);

        void RefreshSystemPermissionCache();
    }
}
