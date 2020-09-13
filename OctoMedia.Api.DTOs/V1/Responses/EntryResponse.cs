using OctoMedia.Api.DTOs.Interfaces;

namespace OctoMedia.Api.DTOs.V1.Responses
{
    public class EntryResponse : IKeyed<int?>
    {
        public int? Key { get; set; }
        public string Response { get; set; }

        public EntryResponse()
        {
            
        }

        public EntryResponse(int? key, string response)
        {
            Key = key;
            Response = response;
        }
    }
}