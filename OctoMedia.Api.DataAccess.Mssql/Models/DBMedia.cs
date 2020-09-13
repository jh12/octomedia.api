using System;
using OctoMedia.Api.DTOs.V1.Media.Meta;

namespace OctoMedia.Api.DataAccess.Mssql.Models
{
    public class DBMedia
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? AuthorUsername { get; set; }
        public int? Height { get; set; }
        public int? Width { get; set; }
        public int SourceId { get; set; }
        public string? ImageUri { get; set; } = null!;
        public string FileTypeExtension { get; set; } = null!;
        public FileClass FileTypeClass { get; set; }
        public bool? Mature { get; set; }
        public bool? Approved { get; set; }
        public byte[]? FileHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Deleted { get; set; }
    }
}