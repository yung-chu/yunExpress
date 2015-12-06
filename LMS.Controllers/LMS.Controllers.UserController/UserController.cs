using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using LighTake.Infrastructure.Web;
using LighTake.Infrastructure.Web.Controllers;
using LMS.Core;
using LMS.Services.UserServices;

namespace LMS.Controllers.UserController
{
    public class UserController : BaseController
    {
        private readonly IWorkContext _workContext;
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;

        public UserController(IWorkContext workContext, IUserService userService, IAuthenticationService authenticationService)
        {
            _workContext = workContext;
            _userService = userService;
            _authenticationService = authenticationService;
        }

        [NonAction]
        private ActionResult RedirectToHomePage()
        {
            return RedirectToRoute("HomePage");
        }

        [NonAction]
        private ActionResult RedirectToLoginPage()
        {
            return RedirectToRoute("Login");
        }

        public ActionResult Login()
        {
            LoginModel model = new LoginModel();
            if (_authenticationService.GetLoginErrorCount() >= LoginModel.S_LoginAllowedErrorCount)
            {
                model.DisplayValidationCode = true;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (string.IsNullOrEmpty(model.LoginName))
            {
                ModelState.AddModelError("", "用户名不能为空");
            }
            else
            {
                model.LoginName = model.LoginName.Trim();
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("", "密码不能为空");
            }

            string strSignInImageCode = _workContext.GetLoginImageCode();

            if (!string.IsNullOrWhiteSpace(strSignInImageCode))
            {
                if (string.IsNullOrEmpty(model.ValidationCode))
                {
                    ModelState.AddModelError("", "验证码不能为空");
                    _authenticationService.IncreaseLoginErrorCount();
                }
                else if (!strSignInImageCode.Equals(model.ValidationCode))
                {
                    ModelState.AddModelError("", "验证码输入有误");
                    _authenticationService.IncreaseLoginErrorCount();
                }
            }

            if (ModelState.IsValid)
            {
                string strMsg;
                var user = _userService.ValidateUser(model.LoginName, model.Password, out strMsg);
                if (user != null)
                {
                    _authenticationService.SignIn(new UserData { UserName = model.LoginName }, false);
                    _authenticationService.RemoveLoginErrorCount();
                    _authenticationService.RemoveLoginImageCode();
                    return RedirectToHomePage();
                }
                ModelState.AddModelError("", "用户名或密码有误.");
                _authenticationService.IncreaseLoginErrorCount();
            }

            if (_authenticationService.GetLoginErrorCount() >= LoginModel.S_LoginAllowedErrorCount)
            {
                model.DisplayValidationCode = true;
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            _authenticationService.SignOut();
            return RedirectToLoginPage();
        }
    }
}
