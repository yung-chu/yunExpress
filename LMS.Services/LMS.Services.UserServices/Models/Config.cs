using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Common;

namespace LMS.Services.UserServices
{
    public class Config
    {
        private static string S_SystemCode = string.Empty;

        public static void SetSystemCode(string systemCode)
        {
            if (systemCode.IsNullOrEmpty())
            {
                throw new ArgumentException("参数值无效.", "systemCode");
            }

            S_SystemCode = systemCode;
        }

        public static string GetSystemCode()
        {
            if (S_SystemCode.IsNullOrEmpty())
            {
                throw new Exception("还未配置系统码.");
            }

            return S_SystemCode;
        }
    }
}
