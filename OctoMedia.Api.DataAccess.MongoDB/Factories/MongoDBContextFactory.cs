using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OctoMedia.Api.Common.Options;
using OctoMedia.Api.DataAccess.MongoDB.Models;

namespace OctoMedia.Api.DataAccess.MongoDB.Factories
{
    public class MongoDBContextFactory : IStartable
    {
        private readonly MongoDBOptions _mongoDbOptions;

        public MongoDBContextFactory(IOptions<MongoDBOptions> mongoDbOptions)
        {
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public IMongoDatabase GetDatabase()
        {
            return GetClient().GetDatabase(_mongoDbOptions.Database);
        }

        public IMongoCollection<T> GetContext<T>(string collectionName)
        {
            IMongoDatabase database = GetDatabase();

            return database.GetCollection<T>(collectionName);
        }

        private MongoClient GetClient()
        {
            return new MongoClient(new MongoClientSettings
            {
                Server = MongoServerAddress.Parse(_mongoDbOptions.Server),
                Credential = MongoCredential.CreateCredential(_mongoDbOptions.Database, _mongoDbOptions.Username, _mongoDbOptions.Password)
            });
        }

        public void Start()
        {
            SeedDatabase();
        }

        private static object _lock = new();
        private static bool _isInitialized;

        private void SeedDatabase()
        {
            lock (_lock)
            {
                if (_isInitialized)
                    return;

                MongoClient client = GetClient();

                IMongoDatabase mongoDatabase = client.GetDatabase(_mongoDbOptions.Database);

                List<string> collectionNames = mongoDatabase.ListCollectionNames().ToList();

                // Media
                {
                    if (!collectionNames.Contains(MongoDBCollectionNames.MediaCollection))
                        mongoDatabase.CreateCollection(MongoDBCollectionNames.MediaCollection);

                    IndexKeysDefinition<MongoMedia> sourceIdDefinition = Builders<MongoMedia>.IndexKeys.Ascending(s => s.SourceId);
                    mongoDatabase.GetCollection<MongoMedia>(MongoDBCollectionNames.MediaCollection).Indexes.CreateOne(new CreateIndexModel<MongoMedia>(sourceIdDefinition));
                }

                // Source
                {
                    if (!collectionNames.Contains(MongoDBCollectionNames.SourceCollection))
                        mongoDatabase.CreateCollection(MongoDBCollectionNames.SourceCollection);

                    IndexKeysDefinition<MongoSource> siteUriDefinition = Builders<MongoSource>.IndexKeys.Hashed(s => s.SiteUri);

                    mongoDatabase.GetCollection<MongoSource>(MongoDBCollectionNames.SourceCollection).Indexes.CreateOne(new CreateIndexModel<MongoSource>(siteUriDefinition));
                }

                // Counters int
                {
                    if (!collectionNames.Contains(MongoDBCollectionNames.IntCounterCollection))
                        mongoDatabase.CreateCollection(MongoDBCollectionNames.IntCounterCollection);
                }

                _isInitialized = true;
            }
        }
    }
}