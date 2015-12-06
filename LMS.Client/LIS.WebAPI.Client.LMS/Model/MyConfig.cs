using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace LIS.WebAPI.Client.LMS.Model
{
    public class MyConfig
    {
        public static string PostVenderPriceUrl =
            string.Concat(ConfigurationManager.AppSettings["WebAPIPath"], "API/LIS/PostVenderPrice");
    }
}
