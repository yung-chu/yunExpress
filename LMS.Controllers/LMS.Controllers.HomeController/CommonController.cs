using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Services.CountryServices;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Seedwork;
using LighTake.Infrastructure.Web;
using LighTake.Infrastructure.Web.Controllers;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Models;

namespace PDM.Controllers
{
    public class CommonController : BaseController
    {
        private readonly IWorkContext _workContext;
        private readonly ICountryService _countryService;
        private readonly IWebHelper _webHelper;
        public CommonController(IWorkContext workContext, IWebHelper webHelper, ICountryService countryService)
        {
            _workContext = workContext;
            _webHelper = webHelper;
            _countryService = countryService;
        }

        [ChildActionOnly]
        public ActionResult LeftNavigation()
        {
            if (_workContext.User == null)
                return View(new Menu());

            string strPageVirtualUrl = _webHelper.GetThisPageVirtualUrl().ToLowerInvariant().Trim();
            User user = _workContext.User;
            PermissionMenu permissionMenu = user.Menu;

            var model = new Menu();
            foreach (var item in permissionMenu.Items)
            {
                MenuItem menuItem = new MenuItem
                {
                    Name = item.Name,
                    NavigateUrl = item.NavigateUrl,
                    PermissionCode = item.PermissionCode,
                    IsActive = !item.NavigateUrl.IsNullOrEmpty() && item.NavigateUrl.ToLowerInvariant().Trim().Equals(strPageVirtualUrl)
                };

                foreach (var subItem in item.SubItems)
                {
                    var tmpMenuItem = new MenuItem
                    {
                        Name = subItem.Name,
                        NavigateUrl = subItem.NavigateUrl,
                        PermissionCode = subItem.PermissionCode,
                        IsActive =
                            !subItem.NavigateUrl.IsNullOrEmpty() &&
                            subItem.NavigateUrl.ToLowerInvariant().Trim().Equals(strPageVirtualUrl)
                    };

                    if (!tmpMenuItem.IsActive)
                    {
                        var permissionItem = user.Permissions.FirstOrDefault(p => !p.NavigateUrl.IsNullOrWhiteSpace() && p.NavigateUrl.ToLowerInvariant().Equals(strPageVirtualUrl));
                        if (permissionItem != null)
                        {
                            permissionItem = user.Permissions.FirstOrDefault(p => p.ModuleId == permissionItem.ParentID);
                            if (permissionItem != null && subItem.ModuleId == permissionItem.ModuleId)
                            {
                                tmpMenuItem.IsActive = true;
                            }
                        }
                    }

                    menuItem.SubItems.Add(tmpMenuItem);
                    if (!menuItem.IsActive)
                    {
                        menuItem.IsActive = tmpMenuItem.IsActive;
                    }

                }
                model.Items.Add(menuItem);
            }

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult TopMenu()
        {
            var model = new Menu();
            User user = _workContext.User;
            PermissionMenu permissionMenu = user.Menu;

            foreach (var item in permissionMenu.Items)
            {
                MenuItem menuItem = new MenuItem
                {
                    Name = item.Name,
                    NavigateUrl = item.NavigateUrl,
                    PermissionCode = item.PermissionCode
                };

                foreach (var subItem in item.SubItems)
                {
                    menuItem.SubItems.Add(new MenuItem
                    {
                        Name = subItem.Name,
                        NavigateUrl = subItem.NavigateUrl,
                        PermissionCode = subItem.PermissionCode
                    });
                }
                model.Items.Add(menuItem);
            }

            return View(model);
        }


        public ActionResult SelectCountry()
        {
            return View();
        }
        public JsonResult GetSelectCountry(string keyword)
        {
            var list = _countryService.GetCountryList(keyword).ToModelAsCollection<Country, CountryModel>();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}