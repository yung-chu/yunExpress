using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LighTake.Infrastructure.Web.Controllers;

namespace LMS.Controllers
{
    public class SecurityController : BaseController
    {
        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}
