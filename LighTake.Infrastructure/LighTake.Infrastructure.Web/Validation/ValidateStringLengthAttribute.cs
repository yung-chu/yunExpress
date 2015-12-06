using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Globalization;

namespace LighTake.Infrastructure.Web.Validation
{
    /// <summary>
    /// 验证字符串长度
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年12月03日
    /// 修改历史 : 无
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateStringLengthAttribute : ValidationAttribute, IClientValidatable
    {
        private const string _defaultErrorMessage = "{0} must be {1} to {2} characters.";
        private readonly int _minCharacters = 0;
        private readonly int _maxCharacters = 0;

        /// <summary>
        /// 验证字符串长度
        /// </summary>
        /// <param name="minCharacters">最小字符数</param>
        /// <param name="maxCharacters">最大字符数</param>
        public ValidateStringLengthAttribute(int minCharacters, int maxCharacters)
            : base(_defaultErrorMessage)
        {
            _minCharacters = minCharacters;
            _maxCharacters = maxCharacters;
        }

        /// <summary>
        /// 格式化错误信息
        /// </summary>
        /// <param name="name">属性名称</param>
        /// <returns>格式化后的错误信息</returns>
        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, _minCharacters, _maxCharacters);
        }

        /// <summary>
        /// 验证指定属性性是否符合规则
        /// </summary>
        /// <param name="value">指定属性性</param>
        /// <returns>符合:True 否则:False</returns>
        public override bool IsValid(object value)
        {
            string valueAsString = value as string;
            return (valueAsString != null && valueAsString.Length >= _minCharacters);
        }

        /// <summary>
        /// 获取客户端验证规则
        /// </summary>
        /// <param name="metadata">模型元数据</param>
        /// <param name="context">控制器上下文</param>
        /// <returns>客户端验证规则</returns>
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[]
            {
                new ModelClientValidationStringLengthRule(FormatErrorMessage(metadata.GetDisplayName()), _minCharacters, _maxCharacters)
            };
        }
    }
}