using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace LighTake.Infrastructure.Web
{

    public enum InputSize
    {
        Default = 2,
        Short = 1,
        Middle = 2,
        Long = 3
    }

    public static class FormExtensions
    {

        //可供搜索的下拉列表框
        //Add By zhengsong
        //Time:2014-09-19
        public static MvcHtmlString DropDownListControlFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
                                                                        Expression<Func<TModel, TProperty>> expression,
                                                                        IEnumerable<SelectListItem> selectList,
                                                                        IDictionary<string, object> htmlAttributes = null,
                                                                        InputSize size = InputSize.Default)
        {
            string strSize = "select_middle ";
            if (size == InputSize.Long)
                strSize = "select_long ";
            else if (size == InputSize.Short)
                strSize = "select_short ";

            var tmpHtmlAttribute = new Dictionary<string, object>
                                       {  
                                           {"class",string.Concat("select ",strSize,"chzn-select ")}
                                       };
            tmpHtmlAttribute.Union(htmlAttributes);
            return htmlHelper.DropDownListFor(expression, selectList, tmpHtmlAttribute);
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
