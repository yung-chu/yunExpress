using System.Web;
using System.Web.Mvc;
using LMS.PrintLabelAPI.UserCenter;

namespace LMS.PrintLabelAPI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new LMSAuthAttribute());
        }
    }
}