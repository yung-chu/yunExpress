using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.WinForm.Client.Models
{
    [Serializable]
    public class UserModel
    {


        public System.Guid CustomerId { get; set; }

        /// <summary>
        /// 登陆名称
        /// </summary>
        public string UserUame { get; set; }



        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName
        {
            get;
            set;
        }



    }
}
