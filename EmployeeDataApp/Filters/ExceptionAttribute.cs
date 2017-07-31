using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http.Filters;
using EmployeeDataApp.Exceptions;

namespace EmployeeDataApp.Filters
{
    public class ExceptionAttribute : Attribute, IExceptionFilter
    {
        public bool AllowMultiple => false;
        public Task ExecuteExceptionFilterAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            if (actionExecutedContext.Exception is IncorrectDataException)
            {
                var exception = (IncorrectDataException)actionExecutedContext.Exception;
                
                actionExecutedContext.Response = exception.Errors != null 
                    ? actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, exception.ModelState) 
                    : actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionExecutedContext.Exception.Message);
            }

            else
                actionExecutedContext.Response =
                    actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, actionExecutedContext.Exception);

            return Task.FromResult<object>(null);
        }
    }

}