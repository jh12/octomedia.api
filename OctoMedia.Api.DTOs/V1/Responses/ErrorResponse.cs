using System.Net;

namespace OctoMedia.Api.DTOs.V1.Responses
{
    public class ErrorResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Response { get; set; }

        public ErrorResponse()
        {
        }

        public ErrorResponse(HttpStatusCode statusCode, string response)
        {
            StatusCode = statusCode;
            Response = response;
        }
    }
}