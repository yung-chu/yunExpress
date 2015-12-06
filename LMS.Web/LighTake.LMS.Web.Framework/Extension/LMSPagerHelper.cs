using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Web;

namespace LighTake.LMS.Web.Framework.Extension
{
    public static class LMSPagerHelper
    {
        public static MvcHtmlString GrouponPager<T>(this HtmlHelper helper, IPagedList<T> pagedList, RouteValueDictionary routeValues)
        {
            if (pagedList == null)
            { return helper.Pager(new PagerOptions(), null); }
            return helper.Pager(pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, new PagerOptions() { ShowPagingInfo = true, ShowPageSizeDropDownList = true }, null, routeValues, null);
        }
    }
}
