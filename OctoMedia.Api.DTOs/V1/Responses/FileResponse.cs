using OctoMedia.Api.DTOs.Interfaces;

namespace OctoMedia.Api.DTOs.V1.Responses
{
    public class FileResponse : IKeyed<int?>
    {
        public int? Key { get; set; }
        public string? Response { get; set; }

        public FileResponse()
        {
            
        }

        public FileResponse(int? key, string? response)
        {
            Key = key;
            Response = response;
        }
    }
}