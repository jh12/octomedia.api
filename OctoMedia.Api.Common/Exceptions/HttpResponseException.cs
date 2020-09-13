using System;
using System.Net;

namespace OctoMedia.Api.Common.Exceptions
{
    public class HttpResponseException : Exception
    {
        public readonly HttpStatusCode Status;
        public readonly string? Value;

        public HttpResponseException(HttpStatusCode status)
        {
            Status = status;
        }

        public HttpResponseException(HttpStatusCode status, string value)
        {
            Status = status;
            Value = value;
        }
    }
}