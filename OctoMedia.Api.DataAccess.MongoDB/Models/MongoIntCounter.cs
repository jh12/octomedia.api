using MongoDB.Bson.Serialization.Attributes;

namespace OctoMedia.Api.DataAccess.MongoDB.Models
{
    public class MongoIntCounter
    {
        [BsonId]
        public string Key { get; set; } = null!;

        public int Value { get; set; }
    }
}