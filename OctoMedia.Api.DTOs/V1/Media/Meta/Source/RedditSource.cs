using System;

namespace OctoMedia.Api.DTOs.V1.Media.Meta.Source
{
    public class RedditSource : SourceAttachmentBase
    {
        public string Subreddit { get; set; } = null!;
        public string Post { get; set; } = null!;
        public string User { get; set; } = null!;
        public bool Nsfw { get; set; }
        public DateTime PostedAt { get; set; }

        public RedditSource()
        {
        }

        public RedditSource(string subreddit, string post, string user, bool nsfw, DateTime postedAt)
        {
            Subreddit = subreddit;
            Post = post;
            User = user;
            Nsfw = nsfw;
            PostedAt = postedAt;
        }
    }
}