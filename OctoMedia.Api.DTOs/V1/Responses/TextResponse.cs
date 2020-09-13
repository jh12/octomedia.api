namespace OctoMedia.Api.DTOs.V1.Responses
{
    public class TextResponse
    {
        public string Response { get; set; } = null!;

        public TextResponse()
        {
        }

        public TextResponse(string response)
        {
            Response = response;
        }
    }
}