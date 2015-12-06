using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace LighTake.Infrastructure.Web.Route
{
  public static class ModelRoutingExtensions
  {

    public static object ModelActionLink( this HtmlHelper htmlHelper, string text, object model )
    {
      return ModelActionLink( htmlHelper, text, model, null );
    }


    public static object ModelActionLink( this HtmlHelper htmlHelper, string text, object model, RouteValueDictionary routeValues )
    {
      return ModelActionLink( htmlHelper, text, model, routeValues, null );
    }


    public static object ModelActionLink( this HtmlHelper htmlHelper, string text, object model, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes )
    {

      if ( routeValues == null )
        routeValues = new RouteValueDictionary();

      if ( routeValues.ContainsKey( "model" ) )
        routeValues.Remove( "model" );

      routeValues.Add( "model", model );


      return htmlHelper.RouteLink( text, routeValues, htmlAttributes );
    }


    public static string ModelLink( this UrlHelper urlHelper, object model )
    {
      if ( urlHelper == null )
        throw new ArgumentNullException( "urlHelper" );

      var values =new RouteValueDictionary();
      values.Add( "model", model );

      return urlHelper.RouteUrl( values );
    }
  }
}
