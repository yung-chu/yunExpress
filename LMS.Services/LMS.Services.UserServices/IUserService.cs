using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;


namespace LMS.Services.UserServices
{
    public interface IUserService
    {
        User ValidateUser(string loginName, string password, out string msg);

        User GetUserByUsername(string userName);

        bool Authorize(string userName, string virtualUrl);
        bool Authorize(string userName, int moduleCode);
    }
}
