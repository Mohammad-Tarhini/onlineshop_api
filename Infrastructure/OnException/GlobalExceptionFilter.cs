using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.ExceptionHandling;
using Newtonsoft.Json;
namespace onlineshopowner_api.Infrastructure.OnException
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        public  override void OnException(HttpActionExecutedContext context)
        {
            var exception = context.Exception;

            var errorResponse = new
            {
                Message = "An error occurred.",
                ExceptionMessage = exception.Message,
                ExceptionType = exception.GetType().FullName,
                StackTrace = exception.StackTrace,
                InnerException = exception.InnerException != null ? new
                {
                    Message = exception.InnerException.Message,
                    ExceptionMessage = exception.InnerException.Message,
                    ExceptionType = exception.InnerException.GetType().FullName,
                    StackTrace = exception.InnerException.StackTrace
                } : null
            };

            context.Response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, errorResponse);
        }
    }
}