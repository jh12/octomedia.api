using System;

namespace OctoMedia.Api.DataAccess.MongoDB.Models.SourceAttachments
{
    public class MongoRedditAttachment
    {
        public string Subreddit { get; set; } = null!;
        public string Post { get; set; } = null!;
        public string User { get; set; } = null!;
        public bool Nsfw { get; set; }
        public DateTime PostedAt { get; set; }
    }
}