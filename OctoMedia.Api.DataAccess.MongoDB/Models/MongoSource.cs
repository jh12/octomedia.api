using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace OctoMedia.Api.DataAccess.MongoDB.Models
{
    [BsonIgnoreExtraElements]
    internal class MongoSource
    {
        [BsonId(IdGenerator = typeof(CombGuidGenerator))]
        public Guid Id { get; set; }
        public MongoSourceAuthor? Author { get; set; }

        public string? Title { get; set; }

        public Uri SiteUri { get; set; } = null!;
        public Uri? RefererUri { get; set; }

        public bool Deleted { get; set; }
    }

    internal class MongoSourceAuthor
    {
        public string? Username { get; set; }
    }
}