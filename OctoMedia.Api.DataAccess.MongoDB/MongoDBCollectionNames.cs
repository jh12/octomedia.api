namespace OctoMedia.Api.DataAccess.MongoDB
{
    internal class MongoDBCollectionNames
    {
        public const string SourceCollection = "api.sources";
        public const string MediaCollection = "api.medias";

        public const string RecentSourceCollection = "api.recent.sources";
        public const string RecentMediaCollection = "api.recent.medias";
        public const string RecentMediaWithFileCollection = "api.recent.mediawithfiles";

        // Counters
        public const string IntCounterCollection = "api.counters.int";
    }
}