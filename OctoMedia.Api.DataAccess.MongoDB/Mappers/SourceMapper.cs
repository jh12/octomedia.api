using OctoMedia.Api.DataAccess.MongoDB.Models;
using OctoMedia.Api.DataAccess.MongoDB.Models.SourceAttachments;
using OctoMedia.Api.DTOs.V1.Media.Meta.Source;

namespace OctoMedia.Api.DataAccess.MongoDB.Mappers
{
    internal static class SourceMapper
    {
        public static KeyedSource Map(MongoSource source)
        {
            return new KeyedSource(source.Id, source.Title, source.SiteUri, source.RefererUri, source.Deleted);
        }

        public static MongoSource Map(KeyedSource source)
        {
            MongoSource mongoSource = Map(source);

            mongoSource.Id = source.Key;

            return mongoSource;
        }

        public static MongoSource Map(Source source)
        {
            return new MongoSource
            {
                Title = source.Title,
                SiteUri = source.SiteUri,
                RefererUri = source.RefererUri,
                Deleted = source.Deleted
            };
        }

        public static MongoRedditAttachment Map(RedditSource redditSource)
        {
            return new MongoRedditAttachment
            {
                Subreddit = redditSource.Subreddit,
                Post = redditSource.Post,
                User = redditSource.User,
                Nsfw = redditSource.Nsfw,
                PostedAt = redditSource.PostedAt
            };
        }
    }
}