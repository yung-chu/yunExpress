using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Controllers.UserController
{
    public class LoginModel
    {
        public static readonly int S_LoginAllowedErrorCount = 3;

        public LoginModel()
        {
            ValidationCodeTagId = "loginImageCode";
            ValidationCodeController = "ValidationCode";
            ValidationCodeAction = "GetLoginValidationImageCode";
            DisplayValidationCode = false;
        }

        public string LoginName { get; set; }

        public string Password { get; set; }

        public string ValidationCode { get; set; }

        public bool DisplayValidationCode { get; set; }

        public string ValidationCodeTagId { get; private set; }

        public string ValidationCodeController { get; private set; }

        public string ValidationCodeAction { get; private set; }
    }
}
