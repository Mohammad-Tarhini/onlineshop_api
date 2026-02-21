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

            // TODO: log exception here
            // _logger.Error(ex);

            HttpResponseMessage response;

            if (ex is DomainException)
            {
                response = context.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    ex.Message
                );
            }
            else if (ex is DbUpdateException)
            {
                response = context.Request.CreateErrorResponse(
                    HttpStatusCode.InternalServerError,
                    "Database error occurred"
                );
            }
            else
            {
                response = context.Request.CreateErrorResponse(
                    HttpStatusCode.InternalServerError,
                    "Unexpected server error"
                );
            }

            context.Response = response;
        }
    }
}