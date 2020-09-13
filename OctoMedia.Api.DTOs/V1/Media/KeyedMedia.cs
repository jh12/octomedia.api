using System;
using OctoMedia.Api.DTOs.Interfaces;
using OctoMedia.Api.DTOs.V1.Media.Meta;

namespace OctoMedia.Api.DTOs.V1.Media
{
    public class KeyedMedia : Media, IKeyed<int>
    {
        public int Key { get; set; }

        public KeyedMedia()
        {
            
        }

        public KeyedMedia(int key, string? title, string? description, Author? author, Dimension? dimension, int sourceId, Uri? imageUri, FileType fileType, bool? mature, bool? approved, bool deleted) : base(title, description, author, dimension, sourceId, imageUri, fileType, mature, approved, deleted)
        {
            Key = key;
        }
    }
}