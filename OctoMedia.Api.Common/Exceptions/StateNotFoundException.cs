using System.Net;

namespace OctoMedia.Api.Common.Exceptions
{
    public class StateNotFoundException : HttpResponseException
    {
        public StateNotFoundException() : base(HttpStatusCode.NotFound)
        {
        }

        public StateNotFoundException(string value) : base(HttpStatusCode.NotFound, value)
        {
        }
    }
}