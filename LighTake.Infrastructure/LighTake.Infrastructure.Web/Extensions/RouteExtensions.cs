using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LighTake.Infrastructure.Web
{
    public static class RouteExtensions
    {

        public static UrlHelper UrlHelper(this HtmlHelper htmlHelper)
        {
            return new UrlHelper(htmlHelper.ViewContext.RequestContext);
        }

        public static string MergeRouteUrl(this UrlHelper urlHelper, object values)
        {
            var routeValues = MergeRouteValues(urlHelper.RequestContext.RouteData, values);

            return urlHelper.RouteUrl(routeValues);
        }

        public static string MergeRouteUrl(this UrlHelper urlHelper, IDictionary<string, object> values)
        {
            var routeValues = MergeRouteValues(urlHelper.RequestContext.RouteData, values);

            return urlHelper.RouteUrl(routeValues);
        }

        public static RouteValueDictionary MergeRouteValues(this RouteData routeData, object values)
        {
            return MergeRouteValues(routeData, new RouteValueDictionary(values));
        }

        public static RouteValueDictionary MergeRouteValues(this RouteData routeData, IDictionary<string, object> values)
        {
            var result = new RouteValueDictionary(routeData.Values);

            foreach (var key in values.Keys)
                result[key] = values[key];

            return result;

        }

        public static string GetUrl(this HtmlHelper helper, string actionName)
        {
            return GetUrl(helper.UrlHelper(), actionName, null, null);
        }

        public static string GetUrl(this HtmlHelper helper, string actionName, string controllerName)
        {
            return GetUrl(helper.UrlHelper(), actionName, controllerName, null);
        }

        public static string GetUrl(this HtmlHelper helper, string actionName, string controllerName, object values)
        {
            return GetUrl(helper.UrlHelper(), actionName, controllerName, values);
        }

        public static string GetUrl(this HtmlHelper helper, string actionName, object values)
        {
            return GetUrl(helper.UrlHelper(), actionName, null, values);
        }

        public static string GetUrl(this UrlHelper helper, string actionName, string controllerName)
        {
            return GetUrl(helper, actionName, controllerName, null);
        }

        public static string GetUrl(this UrlHelper helper, string actionName)
        {
            return GetUrl(helper, actionName, null, null);
        }

        public static string GetUrl(this UrlHelper helper, string actionName, string controllerName, object values)
        {
            var routeValues = values == null ? new RouteValueDictionary() : new RouteValueDictionary(values);

            var rq = helper.RequestContext.HttpContext.Request.QueryString;
            if (rq != null && rq.Count > 0)
            {
                var invalidParams = new[] { "x-requested-with", "xmlhttprequest", "page" };
                foreach (string key in rq.Keys)
                {
                    // add other url query string parameters (exclude PageIndexParameterName parameter value and X-Requested-With=XMLHttpRequest ajax parameter) to route value collection
                    if (!string.IsNullOrEmpty(key))
                    {
                        if (!routeValues.ContainsKey(key)) //Kevin.Mo 2012.10.08 当URL在查询参数中，就不再赋值
                            routeValues[key] = rq[key];
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(controllerName))
                controllerName = (string)helper.RequestContext.RouteData.Values["controller"];

            // action
            routeValues["action"] = actionName;
            // controller
            routeValues["controller"] = controllerName;

            // Return link
            var urlHelper = new UrlHelper(helper.RequestContext);

            return urlHelper.RouteUrl(routeValues);
        }


    }
}