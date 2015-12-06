using System;
using System.Text;
using System.Web.Mvc;

namespace LighTake.Infrastructure.Web.UI
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString ValidationImageCode(this HtmlHelper helper, string id,
                                string controllerName, string actionNamek)
        {
            string strSrc = string.Format("/{0}/{1}?temp={2}", controllerName, actionNamek, DateTime.Now.Millisecond);
            var tagBuild = new TagBuilder("img");
            tagBuild.GenerateId(id);
            tagBuild.MergeAttribute("src", strSrc);
            tagBuild.MergeAttribute("alt", "Try another image");
            tagBuild.MergeAttribute("style", "cursor:pointer;");
            tagBuild.MergeAttribute("class", "vm");          

            string strAId = "a_" + id;
            var taga = new TagBuilder("a");
            taga.GenerateId(strAId);
            taga.SetInnerText("Try another");
            taga.MergeAttribute("style", "color:white;");
            taga.MergeAttribute("href", "javascript:void");


            var sb = new StringBuilder();
            sb.Append("<script language=\"javascript\" type=\"text/javascript\">");
            sb.Append("$(document).ready(function () {");
            sb.AppendFormat("var strSrc=\"{0}\";", strSrc);

            sb.AppendFormat("$(\"#{0}\").bind(\"click\", function () {{", id);
            sb.Append("var url = strSrc;");
            sb.Append("url += \"&\" + Math.random();");
            sb.Append("$(this).attr(\"src\", url);");
            sb.Append("});");

            sb.AppendFormat("$(\"#{0}\").bind(\"click\", function () {{", strAId);
            sb.Append("var url = strSrc;");
            sb.Append("url += \"&\" + Math.random();");
            sb.AppendFormat("$(\"#{0}\").attr(\"src\", url);", id);
            sb.Append("});");

            sb.Append("});");
            sb.Append("</script>");
            return MvcHtmlString.Create(tagBuild + "&nbsp;&nbsp;" + taga + sb);
        }
    }
}