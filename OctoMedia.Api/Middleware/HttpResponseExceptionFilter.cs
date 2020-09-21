using System;
using System.Data.SqlClient;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using OctoMedia.Api.Common.Exceptions;
using OctoMedia.Api.DTOs.V1.Responses;

namespace OctoMedia.Api.Middleware
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        private readonly ILogger _logger;
        public int Order => int.MaxValue - 10;

        public HttpResponseExceptionFilter(ILogger logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if(context.Exception != null)
                _logger.LogError(context.Exception, "An unhandled exception was thrown");

            if (context.Exception is NotImplementedException notImplemented)
            {
                ErrorResponse response = new ErrorResponse(HttpStatusCode.NotImplemented, notImplemented.Message);

                context.Result = new ObjectResult(response)
                {
                    StatusCode = (int?) HttpStatusCode.NotImplemented
                };
                context.ExceptionHandled = true;
            }
            else if (context.Exception is EntryBaseException entryException)
            {
                EntryResponse response = new EntryResponse(entryException.Key, entryException.Value);

                context.Result = new ObjectResult(response)
                {
                    StatusCode = (int?) entryException.Status
                };
                context.ExceptionHandled = true;
            }
            else if (context.Exception is HttpResponseException httpResponse)
            {
                ErrorResponse response = new ErrorResponse(httpResponse.Status, httpResponse.Value);

                context.Result = new ObjectResult(response)
                {
                    StatusCode = (int?) httpResponse.Status
                };
                context.ExceptionHandled = true;
            }
        }
    }
}