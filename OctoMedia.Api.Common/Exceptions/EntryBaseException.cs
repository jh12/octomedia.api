using System;
using System.Net;

namespace OctoMedia.Api.Common.Exceptions
{
    public abstract class EntryBaseException : HttpResponseException
    {
        public int? Key { get; set; }

        protected EntryBaseException(int key, HttpStatusCode statusCode) : base(statusCode)
        {
            Key = key;
        }

        protected EntryBaseException(int key, string message, HttpStatusCode statusCode) : base(statusCode, message)
        {
            Key = key;
        }

        protected EntryBaseException(HttpStatusCode status) : base(status)
        {
        }

        protected EntryBaseException(HttpStatusCode status, string message) : base(status, message)
        {
        }
    }
}