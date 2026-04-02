using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace onlineshopowner_api.Infrastructure.OnException
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {

            //    if (actionContext.ActionArguments.Any(x => x.Value == null))
            //    {
            //        actionContext.Response = actionContext.Request.CreateResponse(
            //            HttpStatusCode.BadRequest,
            //            new { Message = "Request body cannot be empty" }
            //        );
            //        return;
            //    }
            foreach (var arg in actionContext.ActionArguments)
            {
                var type = arg.Value?.GetType();

                if (arg.Value == null && type != null && !type.IsPrimitive && type != typeof(string))
                {
                    actionContext.Response = actionContext.Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        new { Message = "Request body cannot be empty" }
                    );
                    return;
                }
            }

            if (!actionContext.ModelState.IsValid)
            {
                var errors = actionContext.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new { Message = "Validation failed", Errors = errors }
                );
            }
        }
    }
}