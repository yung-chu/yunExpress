using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;

namespace LighTake.Infrastructure.Web
{
    /// <summary>
    /// 用于解析正则表达式路由
    /// </summary>
    /// <example>
    /// routes.Add(new RegexRoute(@"^Books/((?<ssn>[\d]{3}(-?)[\d]{2}\1[\d]{4})|(?<query>.+)?)$", new MvcRouteHandler())
    /// {
    ///    Defaults = new RouteValueDictionary(new { controller = "Book", action = "Find" })
    /// });
    /// </example>
    /// <seealso cref="http://www.iridescence.no/post/Defining-Routes-using-Regular-Expressions-in-ASPNET-MVC.aspx"/>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年11月22日
    /// 修改历史 : #1 添加DataTokens 使其支持Area Namespace等定义 [Added By Kevin @ 2010.11.22 10:18:20]
    /// </remarks>
    public class RegexRoute : System.Web.Routing.Route
    {
        private readonly Regex _urlRegex;

        /// <summary>
        /// 使用指定的 URL 模式和处理程序类初始化 System.Web.Routing.Route 类的新实例
        /// </summary>
        /// <param name="urlPattern">路由的 URL 模式</param>
        /// <param name="routeHandler">处理路由请求的对象</param>
        public RegexRoute(string urlPattern, IRouteHandler routeHandler)
            : this(urlPattern, null, routeHandler)
        {

        }

        /// <summary>
        /// 使用指定的 URL 模式和处理程序类初始化 System.Web.Routing.Route 类的新实例
        /// </summary>
        /// <param name="urlPattern">路由的 URL 模式</param>
        /// <param name="defaults">默认路由</param>
        /// <param name="routeHandler">处理路由请求的对象</param>
        public RegexRoute(string urlPattern, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : base(null, defaults, routeHandler)
        {
            _urlRegex = new Regex(urlPattern, RegexOptions.Compiled);
        }

        /// <summary>
        /// 返回有关所请求路由的信息
        /// </summary>
        /// <param name="httpContext">一个对象，封装有关 HTTP 请求的信息</param>
        /// <returns>一个包含路由定义值的对象</returns>
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            string requestUrl = httpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(2) + httpContext.Request.PathInfo;

            Match match = _urlRegex.Match(requestUrl);

            RouteData data = null;

            if (match.Success)
            {
                data = new RouteData(this, this.RouteHandler);

                // 添加默认Controller Action
                if (this.Defaults != null)
                {
                    foreach (KeyValuePair<string, object> def in this.Defaults)
                    {
                        data.Values[def.Key] = def.Value;
                    }
                }

                // 添加Area,Namespace等数据 [Added By Kevin @ 2010.11.22 10:18:20]
                if (this.DataTokens != null)
                {
                    foreach (KeyValuePair<string, object> def in this.DataTokens)
                    {
                        data.DataTokens[def.Key] = def.Value;
                    }
                }

                // 添加正则表达式中的命名参数
                for (int i = 1; i < match.Groups.Count; i++)
                {
                    Group group = match.Groups[i];

                    if (group.Success)
                    {
                        string key = _urlRegex.GroupNameFromNumber(i);

                        if (!String.IsNullOrEmpty(key) && !Char.IsNumber(key, 0))
                        {
                            data.Values[key] = group.Value;
                        }
                    }
                }
            }

            return data;
        }
    }
}
