using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace OctoMedia.Api.DataAccess.MongoDB.Models
{
    [BsonIgnoreExtraElements]
    public class MongoSource
    {
        [BsonId(IdGenerator = typeof(CombGuidGenerator))]
        public Guid Id { get; set; }

        public string? Title { get; set; }

        public Uri SiteUri { get; set; }
        public Uri? RefererUri { get; set; }

        public bool Deleted { get; set; }
    }
}