using System;

namespace OctoMedia.Api.DataAccess.MongoDB.Models.SourceAttachments
{
    public class MongoRedditAttachment
    {
        public string Subreddit { get; set; }
        public string Post { get; set; }
        public string User { get; set; }
        public bool Nsfw { get; set; }
        public DateTime PostedAt { get; set; }
    }
}