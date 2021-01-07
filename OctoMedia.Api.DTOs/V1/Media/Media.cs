using System;
using OctoMedia.Api.DTOs.Interfaces;
using OctoMedia.Api.DTOs.V1.Media.Meta;

namespace OctoMedia.Api.DTOs.V1.Media
{
    public class Media : IEntity
    {
        public string? Title { get; set; }
        public string? Description { get; set; }

        public Author? Author { get; set; }
        public Dimension? Dimension { get; set; }

        public FileType FileType { get; set; } = null!;
        public Guid SourceId { get; set; }
        public Uri? ImageUri { get; set; }

        public bool? Mature { get; set; }
        public bool? Approved { get; set; }
        public bool Deleted { get; set; }

        public Media()
        {
            
        }

        public Media(string? title, string? description, Author? author, Dimension? dimension, Guid sourceId, Uri? imageUri, FileType fileType, bool? mature, bool? approved, bool deleted)
        {
            Title = title;
            Description = description;
            Author = author;
            Dimension = dimension;
            SourceId = sourceId;
            ImageUri = imageUri;
            FileType = fileType;
            Mature = mature;
            Approved = approved;
            Deleted = deleted;
        }
    }
}