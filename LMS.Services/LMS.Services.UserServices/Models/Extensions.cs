using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Common;
using LMS.Data.Entity;
using LMS.Services.UserServices.PMSServiceReference;

namespace LMS.Services.UserServices
{
    internal static class Extensions
    {
        public static T ToLocal<T>(this PermissionModule remote) where T : Permission, new()
        {
            return new T
            {
                ModuleId = remote.ModuleID,
                NavigateUrl = remote.NavigateUrl,
                Name = remote.FullName,
                ParentID = remote.ParentID,
                PermissionCode = remote.Code
            };
        }

        public static Permission ToLocal(this PermissionModule remote)
        {
            return remote.ToLocal<Permission>();
        }

        public static User ToLocal(this UserInfo userInfo)
        {
            var user = new User
            {
                Birthday = userInfo.UserBaseInfo.Birthday,
                CompanyID = userInfo.UserBaseInfo.CompanyID,
                CompanyName = userInfo.UserBaseInfo.CompanyName,
                Duty = userInfo.UserBaseInfo.Duty,
                Email = userInfo.UserBaseInfo.Email,
                Enabled = userInfo.UserBaseInfo.Enabled,
                IPAddress = userInfo.UserBaseInfo.IPAddress,
                IsSuperAdmin = userInfo.UserBaseInfo.IsSuperAdmin,
                Mobile = userInfo.UserBaseInfo.Mobile,
                OICQ = userInfo.UserBaseInfo.OICQ,
                RealName = userInfo.UserBaseInfo.RealName,
                RoleID = userInfo.UserBaseInfo.RoleID,
                Sex = userInfo.UserBaseInfo.Sex,
                SystemCode = userInfo.UserBaseInfo.SystemCode,
                Title = userInfo.UserBaseInfo.Title,
                UserUame = userInfo.UserBaseInfo.UserName,
            };

            IList<PermissionMenuMenuItem> lstMenuItem = new List<PermissionMenuMenuItem>();
            IList<PermissionMenuMenuItem> lstRootMenuItem = new List<PermissionMenuMenuItem>();
            userInfo.PermissionList.Each(p =>
                                             {

                                                 var menuItem = p.ToLocal<PermissionMenuMenuItem>();
                                                 if (p.IsMenu)
                                                 {
                                                     if (p.ParentID == 0)
                                                     {
                                                         lstRootMenuItem.Add(menuItem);
                                                     }
                                                     else
                                                     {
                                                         lstMenuItem.Add(menuItem);
                                                     }
                                                 }
                                                 user.Permissions.Add(menuItem);
                                             });
            var menu = new PermissionMenu();
            user.Menu = menu;
            foreach (var rootMenuItem in lstRootMenuItem)
            {
                InitPermissionMenuItem(rootMenuItem, lstMenuItem);

                menu.Items.Add(rootMenuItem);
            }

            return user;
        }

        public static PermissionMenuMenuItem InitPermissionMenuItem(PermissionMenuMenuItem parentMenuItem, IEnumerable<PermissionMenuMenuItem> menuItems)
        {
            IEnumerable<PermissionMenuMenuItem> enumbSubMenuItem = menuItems.Where(p => p.ParentID == parentMenuItem.ModuleId);

            foreach (var permissionMenuMenuItem in enumbSubMenuItem)
            {
                InitPermissionMenuItem(permissionMenuMenuItem, menuItems);

                parentMenuItem.SubItems.Add(permissionMenuMenuItem);
            }
            return parentMenuItem;
        }
    }
}
