/*
 ASP.NET MvcPager control
 Copyright:2009-2011 Webdiyer (http://en.webdiyer.com)
 Source code released under Ms-PL license
 */
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;
using LighTake.Infrastructure.Common;

namespace LighTake.Infrastructure.Web
{
    public static class PagerHelper
    {
        #region Html Pager

        public static MvcHtmlString Pager(this HtmlHelper helper, int totalCount, int pageSize, int pageIndex)
        {
            return Pager(helper, totalCount, pageSize, pageIndex, null, null, null, null, null, null);
        }

        public static MvcHtmlString Pager(this HtmlHelper helper, int totalCount, int pageSize, int pageIndex, PagerOptions pagerOptions)
        {
            return Pager(helper, totalCount, pageSize, pageIndex, null, null, pagerOptions, null, null, null);
        }

        public static MvcHtmlString Pager(this HtmlHelper helper, int totalCount, int pageSize, int pageIndex, RouteValueDictionary routeValues)
        {
            return Pager(helper, totalCount, pageSize, pageIndex, null, null, null, null, routeValues, null);
        }

        //public static MvcHtmlString Pager<T>(this HtmlHelper helper, IPagedList<T> pagedList, RouteValueDictionary routeValues)
        //{
        //    return Pager(helper, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, null, null, routeValues, null);
        //}

        public static MvcHtmlString Pager(this HtmlHelper helper, int TotalCount, int pageSize, int pageIndex, string actionName, string controllerName,
            PagerOptions pagerOptions, string routeName, object routeValues, object htmlAttributes)
        {
            var totalPageCount = (int)Math.Ceiling(TotalCount / (double)pageSize);
            var builder = new PagerBuilder
                (
                    helper,
                    actionName,
                    controllerName,
                    totalPageCount,
                    pageIndex,
                    pagerOptions,
                    routeName,
                    new RouteValueDictionary(routeValues),
                    new RouteValueDictionary(htmlAttributes)
                ) { PageSize = pageSize, TotalCount = TotalCount };
            return builder.RenderPager();
        }

        public static MvcHtmlString Pager(this HtmlHelper helper, int TotalCount, int pageSize, int pageIndex, string actionName, string controllerName,
            PagerOptions pagerOptions, string routeName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            var totalPageCount = (int)Math.Ceiling(TotalCount / (double)pageSize);
            var builder = new PagerBuilder
                (
                    helper,
                    actionName,
                    controllerName,
                    totalPageCount,
                    pageIndex,
                    pagerOptions,
                    routeName,
                    routeValues,
                    htmlAttributes
                )
                              {
                                  PageSize = pageSize,
                                  TotalCount = TotalCount
                              };
            return builder.RenderPager();
        }

        public static MvcHtmlString Pager(this HtmlHelper helper, PagerOptions pagerOptions, IDictionary<string, object> htmlAttributes)
        {
            return new PagerBuilder(helper, null, pagerOptions, htmlAttributes).RenderPager();
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, IPagedList<T> pagedList)
        {
            if (pagedList == null)
                return Pager(helper, (PagerOptions)null, null);
            return helper.Pager(pagedList, new RouteValueDictionary());
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, IPagedList<T> pagedList, PagerOptions pagerOptions)
        {
            if (pagedList == null)
                return Pager(helper, pagerOptions, null);
            return Pager(helper, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, pagerOptions, null, null, null);
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, IPagedList<T> pagedList, PagerOptions pagerOptions, object htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper, pagerOptions, new RouteValueDictionary(htmlAttributes));
            return Pager(helper, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, pagerOptions, null, null, htmlAttributes);
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, IPagedList<T> pagedList, PagerOptions pagerOptions, IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper, pagerOptions, htmlAttributes);
            return Pager(helper, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, pagerOptions, null, null, htmlAttributes);
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, IPagedList<T> pagedList, PagerOptions pagerOptions, string routeName, object routeValues)
        {
            if (pagedList == null)
                return Pager(helper, pagerOptions, null);
            return Pager(helper, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, pagerOptions, routeName, routeValues, null);
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, IPagedList<T> pagedList, PagerOptions pagerOptions, string routeName, RouteValueDictionary routeValues)
        {
            if (pagedList == null)
                return Pager(helper, pagerOptions, null);
            return Pager(helper, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, pagerOptions, routeName, routeValues, null);
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, IPagedList<T> pagedList, PagerOptions pagerOptions, string routeName, object routeValues, object htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper, pagerOptions, new RouteValueDictionary(htmlAttributes));
            return Pager(helper, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, pagerOptions, routeName, routeValues, htmlAttributes);
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, IPagedList<T> pagedList, PagerOptions pagerOptions, string routeName,
            RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper, pagerOptions, htmlAttributes);
            return Pager(helper, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, pagerOptions, routeName, routeValues, htmlAttributes);
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, IPagedList<T> pagedList, string routeName, object routeValues, object htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper, null, new RouteValueDictionary(htmlAttributes));
            return Pager(helper, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, null, routeName, routeValues, htmlAttributes);
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, IPagedList<T> pagedList, string routeName, RouteValueDictionary routeValues,
            IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper, null, htmlAttributes);
            return Pager(helper, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, null, routeName, routeValues, htmlAttributes);
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, IPagedList<T> pagedList, RouteValueDictionary routeValues)
        {
            if (pagedList == null)
            { return helper.Pager(new PagerOptions(), null); }
            return helper.Pager(pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, new PagerOptions() { ShowPagingInfo = true, ShowPageSizeDropDownList = true }, null, routeValues, null);
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, IPagedList<T> pagedList, object routeValues)
        {
            return helper.Pager(pagedList, new RouteValueDictionary(routeValues));
        }

        #endregion

        #region jQuery Ajax Pager

        private static MvcHtmlString AjaxPager(HtmlHelper html, PagerOptions pagerOptions, IDictionary<string, object> htmlAttributes)
        {
            return new PagerBuilder(html, null, pagerOptions, htmlAttributes).RenderPager();
        }

        public static MvcHtmlString AjaxPager(this HtmlHelper html, int TotalCount, int pageSize, int pageIndex, string actionName, string controllerName,
            string routeName, PagerOptions pagerOptions, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if (pagerOptions == null)
                pagerOptions = new PagerOptions();
            pagerOptions.UseJqueryAjax = true;

            var totalPageCount = (int)Math.Ceiling(TotalCount / (double)pageSize);
            var builder = new PagerBuilder(html, actionName, controllerName, totalPageCount, pageIndex, pagerOptions,
                                           routeName, new RouteValueDictionary(routeValues), ajaxOptions, new RouteValueDictionary(htmlAttributes)) { PageSize = pageSize, TotalCount = TotalCount };
            return builder.RenderPager();
        }

        public static MvcHtmlString AjaxPager(this HtmlHelper html, int TotalCount, int pageSize, int pageIndex, string actionName, string controllerName,
            string routeName, PagerOptions pagerOptions, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            if (pagerOptions == null)
                pagerOptions = new PagerOptions();
            pagerOptions.UseJqueryAjax = true;

            var totalPageCount = (int)Math.Ceiling(TotalCount / (double)pageSize);
            var builder = new PagerBuilder(html, actionName, controllerName, totalPageCount, pageIndex, pagerOptions,
                                           routeName, routeValues, ajaxOptions, htmlAttributes) { PageSize = pageSize, TotalCount = TotalCount };
            return builder.RenderPager();
        }

        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, IPagedList<T> pagedList, AjaxOptions ajaxOptions)
        {
            if (pagedList == null)
                return AjaxPager(html, (PagerOptions)null, null);
            return AjaxPager(html, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, null, null, null, ajaxOptions,
                             null);
        }

        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, IPagedList<T> pagedList, string routeName, AjaxOptions ajaxOptions)
        {
            if (pagedList == null)
                return AjaxPager(html, (PagerOptions)null, null);
            return AjaxPager(html, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, routeName, null, null, ajaxOptions,
                             null);
        }

        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, IPagedList<T> pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions)
        {
            if (pagedList == null)
                return AjaxPager(html, pagerOptions, null);
            return AjaxPager(html, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, null, pagerOptions, null, ajaxOptions,
                             null);
        }

        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, IPagedList<T> pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if (pagedList == null)
                return AjaxPager(html, pagerOptions, new RouteValueDictionary(htmlAttributes));
            return AjaxPager(html, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, null, pagerOptions, null,
                             ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, IPagedList<T> pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions,
            IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return AjaxPager(html, pagerOptions, htmlAttributes);
            return AjaxPager(html, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, null, pagerOptions, null,
                             ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, IPagedList<T> pagedList, string routeName, object routeValues, PagerOptions pagerOptions, AjaxOptions ajaxOptions)
        {
            if (pagedList == null)
                return AjaxPager(html, pagerOptions, null);
            return AjaxPager(html, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, routeName, pagerOptions, routeValues, ajaxOptions,
                             null);
        }

        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, IPagedList<T> pagedList, string routeName, object routeValues,
            PagerOptions pagerOptions, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if (pagedList == null)
                return AjaxPager(html, pagerOptions, new RouteValueDictionary(htmlAttributes));
            return AjaxPager(html, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, routeName, pagerOptions,
                             routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, IPagedList<T> pagedList, string routeName, RouteValueDictionary routeValues,
            PagerOptions pagerOptions, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return AjaxPager(html, pagerOptions, htmlAttributes);
            return AjaxPager(html, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, routeName, pagerOptions,
                             routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, IPagedList<T> pagedList, string actionName, string controllerName,
            PagerOptions pagerOptions, AjaxOptions ajaxOptions)
        {
            if (pagedList == null)
                return AjaxPager(html, pagerOptions, null);
            return AjaxPager(html, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, actionName, controllerName, null, pagerOptions, null, ajaxOptions,
                             null);
        }

        #endregion

        #region Microsoft Ajax Pager

        public static MvcHtmlString Pager(this AjaxHelper ajax, int TotalCount, int pageSize, int pageIndex, string actionName, string controllerName,
            string routeName, PagerOptions pagerOptions, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            var totalPageCount = (int)Math.Ceiling(TotalCount / (double)pageSize);
            var builder = new PagerBuilder(ajax, actionName, controllerName, totalPageCount, pageIndex, pagerOptions,
                                           routeName, new RouteValueDictionary(routeValues), ajaxOptions, new RouteValueDictionary(htmlAttributes)) { PageSize = pageSize, TotalCount = TotalCount };
            return builder.RenderPager();
        }

        public static MvcHtmlString Pager(this AjaxHelper ajax, int TotalCount, int pageSize, int pageIndex, string actionName, string controllerName,
            string routeName, PagerOptions pagerOptions, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            var totalPageCount = (int)Math.Ceiling(TotalCount / (double)pageSize);
            var builder = new PagerBuilder(ajax, actionName, controllerName, totalPageCount, pageIndex, pagerOptions,
                                           routeName, routeValues, ajaxOptions, htmlAttributes) { PageSize = pageSize, TotalCount = TotalCount };
            return builder.RenderPager();
        }

        private static MvcHtmlString Pager(AjaxHelper ajax, PagerOptions pagerOptions, IDictionary<string, object> htmlAttributes)
        {
            return new PagerBuilder(null, ajax, pagerOptions, htmlAttributes).RenderPager();
        }

        public static MvcHtmlString Pager<T>(this AjaxHelper ajax, IPagedList<T> pagedList, AjaxOptions ajaxOptions)
        {
            return pagedList == null ? Pager(ajax, (PagerOptions)null, null) : Pager(ajax, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, null, null, null, ajaxOptions, null);
        }

        public static MvcHtmlString Pager<T>(this AjaxHelper ajax, IPagedList<T> pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions)
        {
            return pagedList == null ? Pager(ajax, pagerOptions, null) : Pager(ajax, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex,
                null, null, null, pagerOptions, null, ajaxOptions, null);
        }

        public static MvcHtmlString Pager<T>(this AjaxHelper ajax, IPagedList<T> pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if (pagedList == null)
                return Pager(ajax, pagerOptions, new RouteValueDictionary(htmlAttributes));
            return Pager(ajax, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, null, pagerOptions, null, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString Pager<T>(this AjaxHelper ajax, IPagedList<T> pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(ajax, pagerOptions, htmlAttributes);
            return Pager(ajax, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, null, pagerOptions, null, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString Pager<T>(this AjaxHelper ajax, IPagedList<T> pagedList, string routeName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if (pagedList == null)
                return Pager(ajax, null, new RouteValueDictionary(htmlAttributes));
            return Pager(ajax, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, routeName, null, routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString Pager<T>(this AjaxHelper ajax, IPagedList<T> pagedList, string routeName, RouteValueDictionary routeValues,
            AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(ajax, null, htmlAttributes);
            return Pager(ajax, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, routeName, null, routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString Pager<T>(this AjaxHelper ajax, IPagedList<T> pagedList, string routeName, object routeValues, PagerOptions pagerOptions,
            AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if (pagedList == null)
                return Pager(ajax, pagerOptions, new RouteValueDictionary(htmlAttributes));
            return Pager(ajax, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, routeName, pagerOptions, routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString Pager<T>(this AjaxHelper ajax, IPagedList<T> pagedList, string routeName, RouteValueDictionary routeValues,
            PagerOptions pagerOptions, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(ajax, pagerOptions, htmlAttributes);
            return Pager(ajax, pagedList.TotalCount, pagedList.PageSize, pagedList.PageIndex, null, null, routeName, pagerOptions, routeValues, ajaxOptions, htmlAttributes);
        }
        #endregion
    }
}