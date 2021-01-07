using System;
using OctoMedia.Api.DTOs.Interfaces;

namespace OctoMedia.Api.DTOs.V1.Responses
{
    public class CreatedResponse : IKeyed<Guid>
    {
        public Guid Key { get; set; }
        public string Response { get; set; } = null!;

        public CreatedResponse()
        {
        }

        public CreatedResponse(Guid key, string response)
        {
            Key = key;
            Response = response;
        }

        public CreatedResponse(Guid key)
        {
            Key = key;
            Response = "Created";
        }
    }
}