using System.Net;

namespace OctoMedia.Api.Common.Exceptions
{
    public class MimeTypeNotSupportedException : HttpResponseException
    {
        public readonly byte[] InputBytes;

        public MimeTypeNotSupportedException(byte[] inputBytes) : base(HttpStatusCode.BadRequest)
        {
            InputBytes = inputBytes;
        }

        public MimeTypeNotSupportedException(byte[] inputBytes, string value) : base(HttpStatusCode.BadRequest, value)
        {
            InputBytes = inputBytes;
        }
    }
}