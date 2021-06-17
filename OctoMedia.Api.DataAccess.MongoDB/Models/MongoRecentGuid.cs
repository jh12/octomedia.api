using System;

namespace OctoMedia.Api.DataAccess.MongoDB.Models
{
    public class MongoRecentGuid
    {
        public Guid Guid { get; set; }
        public DateTime Timestamp { get; set; }
    }
}