using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web.Mvc;


namespace LighTake.Infrastructure.Web.Validation
{
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property)]
    public class StringTrimExclude : Attribute
    {
        /// <summary>
        /// 是否排除字符串前后空格过滤
        /// </summary>
        public bool IsExclude { get; set; }
    }

    /// <summary>
    /// 模型绑定时去掉字符串前后的空格ModelBinder
    /// </summary>
    /// <remarks>
    /// 编制人员 : 周小强
    /// 完成时间 : 2014年4月29日
    /// 修改历史 : 无
    /// </remarks>
    public class StringTrimModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = base.BindModel(controllerContext, bindingContext);
            
            if (value is string) return (value as string).Trim();

            StringTrim(value);

            return value;
        }

        private void StringTrim(object value)
        {
            if(value==null) return;

            PropertyInfo[] entityProperties = value.GetType().GetProperties();

            foreach (PropertyInfo pInfo in entityProperties)
            {
                //检查该属性是否含有"排除"特性
                StringTrimExclude attr = Attribute.GetCustomAttribute(pInfo, typeof(StringTrimExclude)) as StringTrimExclude;
                if (attr != null && attr.IsExclude)
                {
                    continue;
                }

                if (!(pInfo.CanRead && pInfo.CanWrite)) continue;

                if (pInfo.PropertyType == typeof(string))
                {
                    string v = pInfo.GetValue(value, null) as string;
                    if (v != null)
                    {
                        pInfo.SetValue(value, v.Trim(), null);
                    }
                }

                ////递归遍历
                //else
                //{
                //    StringTrim(pInfo.GetValue(value, null));
                //}
            }
        }
    }
}
