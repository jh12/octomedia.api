using System.Net;

namespace OctoMedia.Api.Common.Exceptions.Entry
{
    public abstract class EntryBaseException<T> : HttpResponseException
    {
        public T? Key { get; set; }

        protected EntryBaseException(T key, HttpStatusCode statusCode) : base(statusCode)
        {
            Key = key;
        }

        protected EntryBaseException(T key, string message, HttpStatusCode statusCode) : base(statusCode, message)
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