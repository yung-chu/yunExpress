using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace LighTake.Infrastructure.Web.Filters
{
    public class FormValueRequiredAttribute : ActionMethodSelectorAttribute
    {
        private readonly string[] _inputNames;

        public FormValueRequiredAttribute(params string[] inputNames)
        {
            //at least one submit button should be found
            _inputNames = inputNames;

        }

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            foreach (string inputName in _inputNames)
            {
                try
                {
                    string value = controllerContext.HttpContext.Request.Form[inputName];

                    if (!String.IsNullOrEmpty(value))
                    { return true; }
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
    }
}
