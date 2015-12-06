using System.Web.Mvc;
using System.Web.Routing;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Web;

namespace LMS.FrontDesk.Framework
{
    public static class PagerHelper
    {
        public static MvcHtmlString LMSPager<T>(this HtmlHelper helper, IPagedList<T> pagedList, RouteValueDictionary routeValues)
        {
            if (pagedList == null)
            { return helper.Pager(new PagerOptions(), null); }
            return helper.Pager(pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, 
                new PagerOptions() 
                { AutoHide = false, ShowFirstLast = false, ShowPagingInfo = false,
                  ShowPageSizeDropDownList = false,
                  CurrentPagerItemWrapperFormatString = "<li class=\"page_itembox current\"><a href=\"\">{0}</a></li>", 
                    ContainerTagName = "ul", CssClass = "l pl2", 
                    PagerItemWrapperFormatString = "<li class=\"page_itembox\">{0}</li>" ,
                  NavigationPagerItemWrapperFormatString = "<li class=\"page_itembox\">{0}</li>"
                }, null, routeValues, null);
        }

        public static MvcHtmlString PagerLIst<T>(this HtmlHelper helper, IPagedList<T> pagedList, int? id)
        {
            if (pagedList == null)
            { return helper.Pager(new PagerOptions(), null); }
            return helper.Pager(pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, new PagerOptions() { ShowPagingInfo = true, ShowPageSizeDropDownList = true, CurrentPagerItemWrapperFormatString = "<span class=\"currentPage\">{0}</span>" }, null, id, null);
        }

    }
}
