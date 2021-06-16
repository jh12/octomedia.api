namespace OctoMedia.Api.DataAccess.MongoDB
{
    internal class MongoDBCollectionNames
    {
        public const string SourceCollection = "api.sources";
        public const string MediaCollection = "api.medias";

        public const string RecentSourceCollection = "api.sources.recent";
        public const string RecentMediaCollection = "api.medias.recent";
        public const string RecentMediaWithFileCollection = "api.mediawithfiles.recent";

        // Counters
        public const string IntCounterCollection = "api.counters.int";
    }
}