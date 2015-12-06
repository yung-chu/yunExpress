using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.WinForm.Client.Models
{
    [Serializable]
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Pwd { get; set; }
    }
}
