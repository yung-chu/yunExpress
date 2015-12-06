using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace LighTake.Infrastructure.Web.Validation
{
    public class MustBeTrueAttribute : ValidationAttribute, IClientValidatable
    {
        public override bool IsValid(object value)
        {
            return value is bool && (bool)value;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "mustbetrue",
                ErrorMessage = this.ErrorMessage
            };
            rule.ValidationParameters["inputstring"] = "mustbetrue";

            yield return rule;
        }
    }
}
