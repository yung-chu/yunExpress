using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Services.SF.References.Sfexpress;

namespace LMS.Services.SF
{
    public class LMSSFService
    {
        private static readonly SfexpressService ESfexpressServiceClient = new SfexpressServiceClient();
        public static string SfExpressService(string xml, string verifyCode)
        {
            return ESfexpressServiceClient.sfexpressService(xml, ChangeBase64(verifyCode));
        }
        public static string ChangeBase64(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                byte[] b = Encoding.Default.GetBytes(str);
                string returnstr = Convert.ToBase64String(b);
                return returnstr;
            }
            else
            {
                return "";
            }
        }
    }
}
