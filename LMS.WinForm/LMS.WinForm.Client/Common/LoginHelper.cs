using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LMS.WinForm.Client.Common
{
    public class LoginHelper
    {

        public static string CurrentUserName { get; set; }

        private static readonly string _file;

        static LoginHelper()
        {
            _file = AppDomain.CurrentDomain.BaseDirectory + "FileConfig.ini";
            if (!File.Exists(_file))
            {
                File.Create(_file);
            }  
        }

        /// <summary>
        /// 存储用户信息
        /// </summary>
        /// <param name="accountId">帐户名</param>
        public static void SaveUserInfo(string accountId)
        {
            //写入/更新键值  
            IniFileHelper.INIWriteValue(_file, "UserInfo", "AccountId", accountId);
        }
        /// <summary>
        /// 删除用户名和密码
        /// </summary>
        public static void DeleteUserInfo()
        {
            //写入/更新键值  

            IniFileHelper.INIDeleteKey(_file, "UserInfo", "AccountId");
        }

        /// <summary>
        /// 获取登录帐户名称
        /// </summary>
        /// <returns></returns>
        public static string GetUserName()
        {
            return IniFileHelper.INIGetStringValue(_file, "UserInfo", "AccountId", null);
        }

      

    }
}
