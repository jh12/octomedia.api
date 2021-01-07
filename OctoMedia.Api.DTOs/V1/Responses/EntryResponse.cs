using System;
using OctoMedia.Api.DTOs.Interfaces;

namespace OctoMedia.Api.DTOs.V1.Responses
{
    public class EntryResponse : IKeyed<Guid?>
    {
        public Guid? Key { get; set; }
        public string? Response { get; set; }

        public EntryResponse()
        {
            
        }

        public EntryResponse(Guid? key, string? response)
        {
            Key = key;
            Response = response;
        }
    }
}