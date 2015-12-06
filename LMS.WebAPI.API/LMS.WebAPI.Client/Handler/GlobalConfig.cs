using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace LMS.WebAPI.Client.Handler
{
    public class GlobalConfig
    {
        public static string IsValidCustomer = ConfigurationManager.AppSettings["IsValidCustomer"];

        public static string UseScrect = ConfigurationManager.AppSettings["UseScrect"];

        public static string CustomerCode = ConfigurationManager.AppSettings["CustomerCode"];

    }
}