using System.Web.Mvc;
using System.Web.Routing;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Web;

namespace LMS.UserCenter.Web.Framework.Extension
{
    public static class PagerHelper
    {
        public static MvcHtmlString Pager<T>(this HtmlHelper helper, IPagedList<T> pagedList, RouteValueDictionary routeValues)
        {
            if (pagedList == null)
            { return helper.Pager(new PagerOptions(), null); }
            return helper.Pager(pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, new PagerOptions() { ShowPagingInfo = true, ShowPageSizeDropDownList = true, CurrentPagerItemWrapperFormatString = "<span class=\"currentPage\">{0}</span>" }, null, routeValues, null);
        }


       

    }
}
