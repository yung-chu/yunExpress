using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Routing;
using LighTake.Infrastructure.Http.Exceptions;

namespace LighTake.Infrastructure.Http
{
    public class BaseApiController : ApiController
    {
        protected virtual string RouteName { get { return "DefaultApi"; } }

        protected string Controller
        {
            get { return Request.GetRouteData().Values["controller"].ToString(); }
        }

        protected string RequestAction
        {
            get
            {
                try
                {
                    return Request.GetRouteData().Values["action"].ToString();
                }
                catch (Exception)
                {
                    return string.Empty;
                }

            }
        }

        protected RouteValueDictionary DefaultRouteValues
        {
            get
            {
                return new RouteValueDictionary()
                           {
                               {"controller",Controller},
                               {"action",RequestAction}
                           };
            }
        }

        protected Uri GetLocation(object obj)
        {
            return GetLocation(RouteName, obj);
        }

        protected Uri GetLocation(string routeName, object obj)
        {
            var defaultRouteValues = DefaultRouteValues;
            foreach (var routeValue in new RouteValueDictionary(obj))
            {
                if (defaultRouteValues.ContainsKey(routeValue.Key))
                    defaultRouteValues[routeValue.Key] = routeValue.Value;
                else
                    defaultRouteValues.Add(routeValue.Key, routeValue.Value);
            }
            return new Uri(Url.Link(routeName, defaultRouteValues));
        }

        protected HttpResponseMessage CreateCreatedResponse<T>(string id, T value)
        {
            var response = Request.CreateResponse(HttpStatusCode.Created, value);
            response.Headers.Location = GetLocation(new { id });
            return response;
        }


        protected void CheckArgumentValidation()
        {
            if (!ModelState.IsValid)
            {
                throw new ArgumentValidateErrorException();
            }
        }
    }
}
