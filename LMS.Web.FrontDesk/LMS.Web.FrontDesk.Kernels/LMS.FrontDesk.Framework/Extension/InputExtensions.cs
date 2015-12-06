using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using LMS.Core;
using LighTake.Infrastructure.Common.Configuration;
using LighTake.Infrastructure.Common.Utities;

namespace LMS.FrontDesk.Framework
{
    public static class InputExtensions
    {
        public static MvcHtmlString ImageSmallControl(this HtmlHelper htmlHelper, string path, bool isRelativePath = true)
        {
            string strSrc = path;
            if (isRelativePath)
            { strSrc = string.Format("{0}{1}", GlobalConfig.WebImagePath_Small, path ?? string.Empty); }

            return MvcHtmlString.Create(
                string.Format("<img src='{0}' style='width:60px;height60px;' />", strSrc)
                   );
        }

        public static MvcHtmlString DateFormFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
                                                                Expression<Func<TModel, TProperty>> expression,
                                                                IDictionary<string, object> htmlAttributes = null)
        {
            return htmlHelper.DatetimeFormFor(expression, htmlAttributes, sysConfig.DefaultDateFormat);
        }

        public static MvcHtmlString DatetimeFormFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
                                                        Expression<Func<TModel, TProperty>> expression,
                                                        IDictionary<string, object> htmlAttributes = null)
        {
            return htmlHelper.DatetimeFormFor(expression, htmlAttributes, sysConfig.DefaultDatetimeFormat);
        }


        private static MvcHtmlString DatetimeFormFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
                                                        Expression<Func<TModel, TProperty>> expression,
                                                        IDictionary<string, object> htmlAttributes = null, string format = null)
        {
            var tmpHtmlAttribute = new Dictionary<string, object>
                                       {
                                           {
                                               "onclick",
                                               "WdatePicker({name:'simple', charset:'gb2312',dateFmt:'" + (string.IsNullOrEmpty(format) ? sysConfig.DefaultDateFormat:format) +"'})"
                                            },
                                           {"class", "txt txt_short Wdate"},
                                          
                                       };

            tmpHtmlAttribute.Union(htmlAttributes);

            return htmlHelper.TextBoxFor(expression, tmpHtmlAttribute);
        }

        public static MvcHtmlString DropDownListFormFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
                                                                        Expression<Func<TModel, TProperty>> expression,
                                                                        IEnumerable<SelectListItem> selectList,
                                                                        IDictionary<string, object> htmlAttributes = null)
        {
            var tmpHtmlAttribute = new Dictionary<string, object>
                                       {  
                                           {"class", "chzn-select select_middle"}
                                       };

            tmpHtmlAttribute.Union(htmlAttributes);

            return htmlHelper.DropDownListFor(expression, selectList, tmpHtmlAttribute);
        }

        public static MvcHtmlString TextBoxFormFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
                                                             Expression<Func<TModel, TProperty>> expression,
                                                             IDictionary<string, object> htmlAttributes = null)
        {
            var tmpHtmlAttribute = new Dictionary<string, object>
                                       {  
                                           {"class ", "txt txt_middle"}
                                       };

            tmpHtmlAttribute.Union(htmlAttributes);

            return htmlHelper.TextBoxFor(expression, tmpHtmlAttribute);
        }

       

        public static MvcHtmlString DropDownListTreeFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
                                                                       Expression<Func<TModel, TProperty>> expression,
                                                                       string id,
                                                                       string value,
                                                                       IList<TreeNode> selectList,
                                                                       IDictionary<string, object> htmlAttributes = null)
        {
            IDictionary<string, object> tmpDictionary = new Dictionary<string, object>()
                                                            {
                                                                {"id",id},
                                                                {"class","easyui-combotree"}
                                                            };
            if (htmlAttributes != null)
                tmpDictionary = tmpDictionary.Union(htmlAttributes);

            TreeNode tree = new TreeNodeHelper().GenerateTreeRoot(selectList as List<TreeNode>);

            string strscript1 = string.Format("{0}{1};", "var arrData=", tree.ToJsonTreeString());
            string strScript2 = string.Format("var $cbxCategory= $('{0}');$cbxCategory.combotree('loadData', arrData);", string.Format("{0}{1}", "#", id));
            string strScript3 = string.Format("$cbxCategory.combotree('setValue', '{0}');", value);

            var strScript = new StringBuilder();
            strScript.Append("<script language=\"javascript\" type=\"text/javascript\">");
            strScript.Append("$(document).ready(function () {");

            strScript.Append(strscript1);
            strScript.Append(strScript2);
            strScript.Append(strScript3);

            strScript.Append("});");
            strScript.Append("</script>");

            var output1 = htmlHelper.DropDownListFor(expression, new List<SelectListItem>(), tmpDictionary);
            var output2 = MvcHtmlString.Create(strScript.ToString());
            return MvcHtmlString.Create(output1 + output2.ToString());
        }



        private static IDictionary<TKey, TValue> Union<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second, bool overwrite = true)
        {
            first = first ?? new Dictionary<TKey, TValue>();

            if (second == null)
            { return first; }

            foreach (var keyValuePair in second)
            {
                if (first.ContainsKey(keyValuePair.Key))
                {
                    if (overwrite)
                    { first[keyValuePair.Key] = second[keyValuePair.Key]; }
                }
                else
                { first.Add(keyValuePair); }
            }

            return first;
        }
    }
}
