using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using LighTake.Infrastructure.Common;

namespace LighTake.Infrastructure.Web
{
    public static class Ext
    {
        public static List<SelectListItem> ListToSelectItem(this IEnumerable<DataSourceBinder> source, string defualtValue, bool isSelectAll)
        {
            var list = (from item in source
                        let isSelect = item.ValueField == defualtValue
                        select new SelectListItem()
                                   {
                                       Selected = isSelect,
                                       Text = item.TextField_EN,
                                       Value = item.ValueField
                                   }).ToList();

            if (isSelectAll)
            {
                list.Insert(0, new SelectListItem()
                             {
                                 Text = "",
                                 Value = ""
                             });
            }

            return list;
        }
    }
}
