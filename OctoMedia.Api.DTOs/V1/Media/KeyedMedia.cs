using System;
using OctoMedia.Api.DTOs.Interfaces;
using OctoMedia.Api.DTOs.V1.Media.Meta;

namespace OctoMedia.Api.DTOs.V1.Media
{
    public class KeyedMedia : Media, IKeyed<Guid>
    {
        public Guid Key { get; set; }

        public KeyedMedia()
        {
            
        }

        public KeyedMedia(Guid key, string? title, string? description, Author? author, Dimension? dimension, Guid sourceId, Uri? imageUri, FileType fileType, bool? mature, bool? approved, bool deleted) : base(title, description, author, dimension, sourceId, imageUri, fileType, mature, approved, deleted)
        {
            Key = key;
        }
    }
}