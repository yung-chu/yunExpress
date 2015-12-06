using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Reflection;


namespace LighTake.Infrastructure.Web.Validation
{
    /// <summary>
    /// 验证地址时如果选择了美国就必须填写州
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年12月17日
    /// 修改历史 : 无
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AddressStateAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "US Country must Required .";
        public string Country { get; private set; }

        public AddressStateAttribute(string country)
            : base(_defaultErrorMessage)
        {
            Country = country;
        }

        public override string FormatErrorMessage(string name)
        {
            return _defaultErrorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo countryProperty = validationContext.ObjectType.GetProperty(this.Country);

            object countryValue = countryProperty.GetValue(validationContext.ObjectInstance, null);

            if (countryValue != null && !string.IsNullOrEmpty(countryValue.ToString()) && countryValue.ToString() == "US" && (value == null || (value != null && string.IsNullOrEmpty(value.ToString()))))
                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));

            return null;
        }

    }
}
