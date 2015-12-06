using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Data.Entity;
using LMS.Services.ReturnGoodsServices;
using LMS.Services.UserServices;
using LighTake.Infrastructure.Http;

namespace LMS.WebAPI.Controllers
{
    public class TestController : BaseApiController
    {
        private readonly IReturnGoodsService _returnGoodsService;
        private readonly IUserService _userService;
        public TestController(IReturnGoodsService returnGoodsService,IUserService userService)
        {
            _returnGoodsService = returnGoodsService;
            _userService = userService;
        }
        [HttpGet]
        public string Login(string userName)
        {
            string msg = string.Empty;
            User user = _userService.ValidateUser(userName, "123", out msg);
            return msg;
        }
        public string GetLogin(string userName, string pwd)
        {
            string msg = string.Empty;
            User user = _userService.ValidateUser(userName, pwd, out msg);
            return msg;
            //return "Hello";
        } 

    }
}
