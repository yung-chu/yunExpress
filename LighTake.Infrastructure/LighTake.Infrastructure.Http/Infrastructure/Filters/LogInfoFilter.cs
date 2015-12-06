using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Filters;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;

namespace LighTake.Infrastructure.Http.Filters
{
    public class LogInfoFilter : ActionFilterAttribute
    {
        private const string S_RequestUrl = "{0} <br/> RequestUrl:{1}<br/>";
        private const string S_RequestMethod = "HttpMethod:{0}<br/>";
        private const string S_parameterTemplate = "Name:{0} | Value:{1}<br/>";

        private const string S_ResponseStateCode = "StateCode:{0}<br/>";

        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                                            HttpStatusCode.BadRequest, actionContext.ModelState);

            var dicArgument = actionContext.ActionArguments;

            var strLog = new StringBuilder();
            strLog.AppendFormat(S_RequestUrl, "ActionExecuting", actionContext.Request.RequestUri.AbsolutePath);
            strLog.AppendFormat(S_RequestMethod, actionContext.Request.Method);

            foreach (var item in dicArgument)
                strLog.AppendFormat(S_parameterTemplate, item.Key, SerializeUtil.SerializeToXml(item.Value));

            Log.Info(strLog.ToString());
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var strLog = new StringBuilder();
            strLog.AppendFormat(S_RequestUrl, "ActionExecuted", actionExecutedContext.Request.RequestUri.AbsolutePath);
            strLog.AppendFormat(S_RequestMethod, actionExecutedContext.Request.Method);
            if (actionExecutedContext.Response != null)
            {
                if (actionExecutedContext.Response != null)
                    strLog.AppendFormat(S_ResponseStateCode, actionExecutedContext.Response.StatusCode);

                Object tmpReturnValue;
                if (actionExecutedContext.Response.TryGetContentValue(out tmpReturnValue))
                {
                    strLog.AppendFormat(S_parameterTemplate, tmpReturnValue.GetType(), SerializeUtil.SerializeToXml(tmpReturnValue));
                }

                IEnumerable<Object> enmbReturnValue;
                if (actionExecutedContext.Response.TryGetContentValue(out enmbReturnValue) && enmbReturnValue.Any())
                {
                    strLog.AppendFormat(S_parameterTemplate, enmbReturnValue.GetType(), SerializeUtil.SerializeToXml(enmbReturnValue));
                }
            }
            Log.Info(strLog.ToString());
        }
    }
}