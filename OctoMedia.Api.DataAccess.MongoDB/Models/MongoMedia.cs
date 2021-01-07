using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using OctoMedia.Api.DTOs.V1.Media.Meta;

namespace OctoMedia.Api.DataAccess.MongoDB.Models
{
    internal class MongoMedia
    {
        [BsonId(IdGenerator = typeof(CombGuidGenerator))]
        public Guid? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public MongoMediaAuthor? Author { get; set; }
        public MongoMediaDimensions? Dimensions { get; set; }
        [BsonGuidRepresentation(GuidRepresentation.CSharpLegacy)]
        public Guid SourceId { get; set; }
        public Uri? ImageUri { get; set; }
        public MongoMediaFileType FileType { get; set; }
        public bool? Mature { get; set; }
        public bool? Approved { get; set; }
        public byte[]? FileHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Deleted { get; set; }

        // Migration data
        public int MigratedId { get; set; }
    }

    internal class MongoMediaAuthor
    {
        public string Username { get; set; }
    }

    internal class MongoMediaDimensions
    {
        public int Height { get; set; }
        public int Width { get; set; }
    }

    internal class MongoMediaFileType
    {
        public string Extension { get; set; }
        public FileClass Class { get; set; }
    }
}