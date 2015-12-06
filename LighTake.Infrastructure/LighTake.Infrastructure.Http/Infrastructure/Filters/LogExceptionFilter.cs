using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Http.Exceptions;

namespace LighTake.Infrastructure.Http.Filters
{
    public class LogExceptionFilter : ExceptionFilterAttribute
    {
        private const string S_RequestUrl = "RequestUrl:{0}  ";
        private const string S_RequestMethod = "HttpMethod:{0}  ";
        private const String S_Exception = "Exception:{0}";
        private const string S_parameterTemplate = "Name:{0} | Value:{1}<br/>";

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            string strException = string.Concat(string.Format(S_RequestUrl, actionExecutedContext.Request.RequestUri.AbsolutePath),
                                                string.Format(S_RequestMethod, actionExecutedContext.Request.Method),
                                                string.Format(S_Exception, actionExecutedContext.Exception));

            if (actionExecutedContext.Exception is ArgumentValidateErrorException)//如果是参数验证抛出的异常
            {
                strException = actionExecutedContext.ActionContext.ModelState.Aggregate(strException,
                    (current, modelStete) => string.Concat(current, string.Format(S_parameterTemplate, modelStete.Key, modelStete.Value)));

                Log.Error(strException);
                actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionExecutedContext.ActionContext.ModelState);
            }
            else
            {
                Log.Error(strException);
                actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, strException);
            }
        }
    }
}