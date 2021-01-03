using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OctoMedia.Api.Common.Exceptions;
using OctoMedia.Api.DTOs.V1.Responses;
using Serilog;

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
            if (context.Exception is NotImplementedException notImplemented)
            {
                _logger.Error(context.Exception, "Not implemented exception was thrown");

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
                _logger.Error(context.Exception, "An unhandled exception was thrown");

                ErrorResponse response = new ErrorResponse(httpResponse.Status, httpResponse.Value);

                context.Result = new ObjectResult(response)
                {
                    StatusCode = (int?) httpResponse.Status
                };
                context.ExceptionHandled = true;
            }
            else if(context.Exception != null)
                _logger.Error(context.Exception, "An unhandled exception was thrown");
        }
    }
}