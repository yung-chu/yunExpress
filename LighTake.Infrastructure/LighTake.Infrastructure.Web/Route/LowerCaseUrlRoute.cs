using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Globalization;

namespace LighTake.Infrastructure.Web
{
    public class LowerCaseUrlRoute : System.Web.Routing.Route
    {
        private static readonly string[] requiredKeys = new[] { "area", "controller", "action" };

        public LowerCaseUrlRoute(string url, IRouteHandler routeHandler)
            : base(url, routeHandler)
        { }

        public LowerCaseUrlRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : base(url, defaults, routeHandler)
        { }

        public LowerCaseUrlRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler)
            : base(url, defaults, constraints, routeHandler)
        {
        }

        public LowerCaseUrlRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler)
            : base(url, defaults, constraints, dataTokens, routeHandler)
        {
        }


        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {

            var processedValues = new RouteValueDictionary(values);

            //LowerRouteValues(requestContext.RouteData.Values);
            LowerRouteValues(processedValues);
            //LowerRouteValues(Defaults);

            return base.GetVirtualPath(requestContext, processedValues);
        }


        private void LowerRouteValues(RouteValueDictionary values)
        {
            foreach (var key in requiredKeys)
            {
                if (values.ContainsKey(key) == false) continue;

                var value = values[key];
                if (value == null) continue;

                var valueString = Convert.ToString(value, CultureInfo.InvariantCulture);
                if (valueString == null) continue;

                values[key] = valueString.ToLower();
            }

            var otherKyes = values.Keys
                .Except(requiredKeys, StringComparer.InvariantCultureIgnoreCase)
                .ToArray();

            foreach (var key in otherKyes)
            {
                var value = values[key];
                values.Remove(key);
                values.Add(key.ToLower(), value);
            }
        }
    }
}
