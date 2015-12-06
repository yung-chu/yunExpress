using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web.Mvc;

namespace LighTake.Infrastructure.Web
{
    public static class PagerExtension
    {
        public static RouteValueDictionary MergeRouteValues(UrlHelper urlHelper, RouteValueDictionary values)
        {
            var result = new RouteValueDictionary(urlHelper.RequestContext.RouteData.Values);
            
            foreach (var key in values.Keys)
                result[key] = values[key];

            return result;
        }

        public static RouteValueDictionary MergeRouteValues(this UrlHelper urlHelper, object values)
        {
            return MergeRouteValues(urlHelper, new RouteValueDictionary(values));
        }
    }
}
