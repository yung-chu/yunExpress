using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace LighTake.Infrastructure.Web.Route
{
  public class ModelRoute<T> : RouteBase where T : class
  {

    public ModelRoute( IModelUrlProvider<T> provider, string controller, string action )
    {
      Provider = provider;

      Controller = controller;

      Action = action;
    }



    protected const string parametersStartSymbol = "/p/";

    protected const string parameterKeyValueSeparatorSubstitute = "--";
    protected const string parameterKeyValueSeparator = "-";

    protected static Regex parameterKeyValueSeparatorRegex = new Regex( "(?<!-)-(?!-)", RegexOptions.Compiled );


    protected static Regex parameterSeparatorRegex = new Regex( "(?<!/p)/(?!p/)", RegexOptions.Compiled );
    protected static string parameterSeparator = "/";

    protected static readonly string[] reservedWords = new[] { "controller", "action", "model" };


    public override RouteData GetRouteData( HttpContextBase httpContext )
    {
      var virtualPath = httpContext.Request.AppRelativeCurrentExecutionFilePath + httpContext.Request.PathInfo;
      virtualPath = virtualPath.Substring( 2 ).Trim( '/' );

      if ( virtualPath == "" )
        return null;


      var parameterIndex = virtualPath.IndexOf( parametersStartSymbol );

      string path;
      string parametersExpression;
      if ( parameterIndex != -1 )
      {
        path = virtualPath.Substring( 0, parameterIndex );
        parametersExpression = virtualPath.Substring( parameterIndex + parametersStartSymbol.Length );
      }
      else
      {
        path = virtualPath;
        parametersExpression = null;
      }

      /**/

      //var path = virtualPath;



      var model = Provider.GetModel( path );

      if ( model == null )
        return null;




      var data = new RouteData( this, new MvcRouteHandler() );


      LoadRouteData( parametersExpression, data );

      //LoadRouteData( httpContext.Request.QueryString, data );


      data.Values.Add( "controller", Controller );
      data.Values.Add( "action", Action );
      data.Values.Add( "model", model );


      return data;
    }



    protected virtual void LoadRouteData( string expression, RouteData data )
    {
      if ( string.IsNullOrEmpty( expression ) )
        return;

      foreach ( var p in parameterSeparatorRegex.Split( expression ) )
      {
        string key;
        string value;


        var match = parameterKeyValueSeparatorRegex.Match( p );

        if ( match.Success )
        {
          var index = match.Index;

          key = p.Substring( 0, index ).Replace( parameterKeyValueSeparatorSubstitute, parameterKeyValueSeparator );
          value = p.Substring( index + parameterKeyValueSeparator.Length ).Replace( parameterKeyValueSeparatorSubstitute, parameterKeyValueSeparator );
        }
        else
        {
          key = p;
          value = "";
        }




        if ( reservedWords.Contains( key.ToLowerInvariant() ) )
          continue;

        data.Values[key] = value;
      }
    }

    /**/


    protected virtual void LoadRouteData( NameValueCollection values, RouteData data )
    {
      if ( values == null )
        return;

      foreach ( var key in values.AllKeys )
      {

        var value = values[key];

        data.Values[key] = value;
      }
    }




    public override VirtualPathData GetVirtualPath( RequestContext requestContext, RouteValueDictionary values )
    {
      var model = values["model"] as T;

      if ( model == null )
        return null;


      var path = Provider.GetPath( model );

      //path = "~/" + path;                                    //VirtualPathData要求的便是相对于~/的路径


      var parametersExpression = GenerateParameterExpression( values );

      if ( !string.IsNullOrEmpty( parametersExpression ) )
        path = path + parametersStartSymbol + parametersExpression;

      path = path.ToLowerInvariant();

      return new VirtualPathData( this, path );
    }


    protected virtual string GenerateParameterExpression( RouteValueDictionary values )
    {
      var paramCollection = new NameValueCollection();

      foreach ( var pair in values )
      {
        if ( reservedWords.Contains( pair.Key.ToLowerInvariant() ) )//去除保留参数
          continue;



        string value = null;
        if ( pair.Value != null )
          value = pair.Value.ToString();

        paramCollection.Add( pair.Key, value );
      }


      //return GenerateQueryString( paramCollection );//直接生成为QueryString。



      var parameters = new List<string>();

      foreach ( var pair in values.OrderBy( p => p.Key ) )
      {
        if ( reservedWords.Contains( pair.Key.ToLowerInvariant() ) )
          continue;

        if ( pair.Value == null )
        {
          parameters.Add( pair.Key );
          continue;
        }

        parameters.Add(
          pair.Key.Replace( parameterKeyValueSeparator, parameterKeyValueSeparatorSubstitute ) +
          parameterKeyValueSeparator +
          pair.Value.ToString().Replace( "_", "" ).Replace( parameterKeyValueSeparator, parameterKeyValueSeparatorSubstitute ) );
      }

      var parametersExpression = string.Join( parameterSeparator, parameters.ToArray() );
      return parametersExpression;

      /**/
    }



    public IModelUrlProvider<T> Provider
    {
      get;
      private set;
    }

    public string Controller
    {
      get;
      private set;
    }


    public string Action
    {
      get;
      private set;
    }






    /// <summary>
    /// 生成QueryString
    /// </summary>
    /// <param name="args">键值对</param>
    /// <returns>生成的QueryString</returns>
    public static string GenerateQueryString( System.Collections.Specialized.NameValueCollection args )
    {
      return GenerateQueryString( args, Encoding.UTF8 );
    }

    /// <summary>
    /// 生成QueryString
    /// </summary>
    /// <param name="args">键值对</param>
    /// <param name="encoding">URL编码</param>
    /// <returns>生成的QueryString</returns>
    public static string GenerateQueryString( System.Collections.Specialized.NameValueCollection args, Encoding encoding )
    {

      StringBuilder builder = new StringBuilder();

      for ( int i = 0; i < args.Count; i++ )
      {
        string key = args.GetKey( i );
        key = ((key != null) && (key.Length > 0)) ? (HttpUtility.UrlEncode( key, encoding ) + "=") : "";

        string[] values = args.GetValues( i );


        if ( i > 0 )
          builder.Append( "&" );

        if ( values.Length == 0 )
          builder.Append( key );
        else
        {
          for ( int j = 0; j < values.Length; j++ )
          {
            if ( j > 0 )
              builder.Append( "&" );

            builder.Append( key );
            builder.Append( HttpUtility.UrlEncode( values[j], encoding ) );
          }
        }
      }

      return builder.ToString();
    }


  }




}
