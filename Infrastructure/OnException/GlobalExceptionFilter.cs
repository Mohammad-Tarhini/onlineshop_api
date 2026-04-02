using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
namespace onlineshopowner_api.Infrastructure.OnException
{

    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var ex = context.Exception;

            var innerMessage = ex.InnerException != null ? ex.InnerException.Message : null;
            var innerStack = ex.InnerException != null ? ex.InnerException.StackTrace : null;

            HttpResponseMessage response;

            if (ex is DomainException)
            {
                response = context.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    ex.Message
                );
            }
            else
            {
                response = context.Request.CreateResponse(
                    HttpStatusCode.InternalServerError,
                    new
                    {
                        Message = "Unexpected server error",
                        Exception = ex.Message,
                        InnerException = innerMessage,
                        StackTrace = ex.StackTrace,
                        InnerStackTrace = innerStack
                    }
                );
            }

            context.Response = response;
        }
    }
}