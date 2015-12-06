using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.ComponentModel;

namespace LighTake.Infrastructure.Web.Validation
{
    /// <summary>
    /// 验证属性值是否和指定值匹配
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年12月03日
    /// 修改历史 : 无
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class EqualsToAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "{0} must be {1}.";
        private object _matchValue = null;

        public EqualsToAttribute(object value)
        {
            _matchValue = value;
        }

        public EqualsToAttribute()
            : base(_defaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString, name, _matchValue);
        }

        public override bool IsValid(object value)
        {
            return Object.Equals(value, _matchValue);
        }
    }
}