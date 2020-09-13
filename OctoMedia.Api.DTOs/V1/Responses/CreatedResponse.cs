using OctoMedia.Api.DTOs.Interfaces;

namespace OctoMedia.Api.DTOs.V1.Responses
{
    public class CreatedResponse : IKeyed<int>
    {
        public int Key { get; set; }
        public string Response { get; set; }

        public CreatedResponse()
        {
        }

        public CreatedResponse(int key, string response)
        {
            Key = key;
            Response = response;
        }

        public CreatedResponse(int key)
        {
            Key = key;
            Response = "Created";
        }
    }
}